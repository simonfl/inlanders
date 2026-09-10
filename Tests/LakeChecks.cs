using Inlanders.Simulation;

public static class LakeChecks
{
    static void Check(bool ok,string why) { if(!ok) throw new Exception(why); }
    static void Until(World w,Func<bool> done,string step,int limit=16000)
    {
        for(int i=0;i<limit && !done();i++) { w.Tick(.1f); if(i%20==0) w.Validate(); }
        Check(done(),$"Lake stalled at {step}, {w.Food.Time:0}s: {w.LakeObjective}");
        Console.WriteLine($"LAKE {step}: {w.Food.Time:0}s, {w.Population} residents, {w.Food.EdibleStored} food, {w.DeliveredFish} fish delivered.");
    }
    static Cottage Build(World w,Cell cell,BuildingKind kind,bool rotated=false) => w.Place(cell,rotated,kind) ?? throw new Exception($"Lake rejected {kind} at {cell}: {w.PlacementProblem(cell,rotated,kind)}");
    public static void Run()
    {
        foreach(bool bread in new[]{false,true})
        {
            var w=World.NewCampaign(7);
            Check(w.CurrentCampaignHint()!=null && !w.AdvanceLakePhase(),"Lake opening/guidance invalid");
            string initial=w.SaveJson(); Check(World.LoadJson(initial).SaveJson()==initial,"Lake opening save changed");
            Build(w,new(3,4),BuildingKind.FishingDock);
            if(!bread) Build(w,new(-3,6),BuildingKind.Square);
            Build(w,bread?new(-3,6):new(-6,6),bread?BuildingKind.Farm:BuildingKind.VegetableGarden,!bread);
            if(bread) { Until(w,()=>w.Food.Time>20,"bakery access"); Build(w,new(-6,6),BuildingKind.Bakery,true); w.Assign(1,Role.Baker); }
            w.Assign(6,Role.Fisher); w.Assign(7,Role.Farmer);
            Until(w,()=>w.DeliveredFish>=4,bread?"bread route first catch":"central square / garden first catch");
            Check(w.AdvanceLakePhase(),"First catch phase rejected");
            Check(World.LoadJson(w.SaveJson()).SaveJson()==w.SaveJson(),"Lake growth phase save changed");
            foreach(var tree in w.Trees.Where(t=>t.Cell.X<3)) w.SetClearing(tree.Cell,true);
            Build(w,bread?new(0,3):new(4,10),BuildingKind.Cottage);
            Build(w,bread?new(5,9):new(4,-8),BuildingKind.Cottage,!bread);
            if(bread) Build(w,new(4,-8),BuildingKind.Square,true);
            Until(w,()=>w.Beds>=12 && w.InvitationProblem()==null,"homes ready");
            Check(w.InviteNewcomers(),"First lake invitation failed");
            w.Assign(8,Role.Logger); w.Assign(9,Role.Builder);
            Until(w,()=>w.InvitationProblem()==null,"second invitation"); Check(w.InviteNewcomers(),"Second lake invitation failed");
            Until(w,()=>w.LakeActionProblem()==null,"community ready");
            Check(w.AdvanceLakePhase(),"Lake assessment rejected");
            string assessment=w.SaveJson(); var copy=World.LoadJson(assessment);
            for(int i=0;i<100;i++) { w.Tick(.1f); copy.Tick(.1f); }
            Check(w.SaveJson()==copy.SaveJson(),"Lake assessment save diverged");
            Until(w,()=>w.Campaign!.Complete,"supported village");
            Check(w.Campaign!.Lake!.Meals==3 && World.LoadJson(w.SaveJson()).Campaign!.Complete,"Lake completion not saved");
            if(!bread)
            {
                System.IO.Directory.CreateDirectory("artifacts");
                System.IO.File.WriteAllText("artifacts/f11b2-complete.json",w.SaveJson());
                Recovery(w);
            }
        }
        LayoutRecovery();
        Console.WriteLine("PASS: narrow lake central recreation/garden and bread routes, growth, real services, fresh mixed meals and phase saves.");
    }
    static void LayoutRecovery()
    {
        var w=World.NewCampaign(7);
        Build(w,new(3,4),BuildingKind.FishingDock); Build(w,new(-3,6),BuildingKind.VegetableGarden);
        w.Assign(6,Role.Fisher); w.Assign(7,Role.Farmer);
        Until(w,()=>w.DeliveredFish>=4,"rough layout catch"); Check(w.AdvanceLakePhase(),"Rough layout catch rejected");
        foreach(var tree in w.Trees.Where(t=>t.Cell.X<3)) w.SetClearing(tree.Cell,true);
        Build(w,new(-6,6),BuildingKind.Cottage,true); Build(w,new(5,9),BuildingKind.Cottage); Build(w,new(4,-8),BuildingKind.Square,true);
        Until(w,()=>w.Beds>=12 && w.InvitationProblem()==null,"rough layout homes");
        w.InviteNewcomers(); w.Assign(8,Role.Logger); w.Assign(9,Role.Builder);
        Until(w,()=>w.InvitationProblem()==null,"rough layout newcomers"); w.InviteNewcomers();
        Until(w,()=>w.LakeActionProblem()==null,"rough layout assessment"); w.AdvanceLakePhase();
        int failures=0;
        for(int i=0;i<4000 && failures<2;i++)
        {
            float meal=w.Food.MealClock; w.Tick(.1f); w.Validate();
            if(w.Food.MealClock<meal && w.Campaign!.Lake!.Meals==0) failures++;
        }
        Check(failures==2 && !w.Campaign!.Complete && w.Campaign.Lake!.LastResult.Contains("shorten"),"Layout shortfall lacked actionable feedback");
        Until(w,()=>w.PlacementProblem(new(0,3),false,BuildingKind.Square)==null,"nearby square space");
        Build(w,new(0,3),BuildingKind.Square);
        string recovery=w.SaveJson(); Check(World.LoadJson(recovery).SaveJson()==recovery,"Layout recovery save changed");
        w=World.LoadJson(recovery);
        Until(w,()=>w.Campaign!.Complete,"nearby square recovery");
        Check(w.Food.Time<1200,"Nearby square did not recover materially faster than waiting out the rough layout");
    }
    static void Recovery(World completed)
    {
        var w=World.LoadJson(completed.SaveJson());
        var roles=w.People.Select(p=>p.Role).ToArray();
        foreach(var p in w.People) w.Assign(p.Id,Role.Unassigned);
        Until(w,()=>w.People.All(p=>p.Carried==0 && p.Task!=Work.Aboard),"return committed deliveries");
        w.Campaign!.Complete=false;
        w.Campaign.Lake=new LakeProgress { Phase=2,DeliveredBaseline=w.DeliveredBerries+w.DeliveredVegetables+w.DeliveredBread+w.DeliveredFish };
        for(int i=0;i<1300;i++) { w.Tick(.1f); w.Validate(); }
        Check(w.Campaign.Lake.Meals==0 && !w.Campaign.Complete && w.Campaign.Lake.LastResult.Contains("Fresh"),"Old food reserve completed lake assessment");
        string shortage=w.SaveJson(); Check(World.LoadJson(shortage).SaveJson()==shortage,"Failed proof did not save");
        for(int i=0;i<roles.Length;i++) w.Assign(i,roles[i]);
        Until(w,()=>w.Campaign!.Complete,"supply recovery");
        var unmet=World.LoadJson(completed.SaveJson()); unmet.Campaign!.Complete=false;
        unmet.Campaign.Lake=new LakeProgress { Phase=1 };
        foreach(var p in unmet.People) { p.LastRestTime=null; p.LastLeisureTime=null; }
        Check(!unmet.AdvanceLakePhase() && unmet.LakeActionProblem()!.Contains("rest"),"Buildings alone satisfied resident services");
    }
}

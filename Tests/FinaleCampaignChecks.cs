using Inlanders.Simulation;
using static FinaleDecisionChecks;

static class FinaleCampaignChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    static World Reload(World w,string? fixture=null)
    {
        w.Validate();string json=w.SaveJson();if(fixture!=null)File.WriteAllText($"artifacts/finale-campaign-{fixture}.json",json);
        var copy=World.LoadJson(json);Check(copy.SaveJson()==json,"Campaign phase save differs");return copy;
    }
    public static void Run()
    {
        Directory.CreateDirectory("artifacts");
        var opening=World.NewCampaign(10);
        Check(opening.InitialLogs==168 && opening.YardLogs==16 && opening.Trees.Count==15 && opening.Trees.Where(t=>t.Cell.X>6).Sum(t=>t.Logs)==96,"Finale material budget changed");
        Check(opening.Map.Water.Count(c=>opening.PlacementProblem(c,1,BuildingKind.Bridge)==null)>=3,"Finale lost alternative crossings");
        Check(opening.IsFinaleCampaign && !opening.AdvanceFinalePhase() && !opening.BeginSupper(),"Opening gates failed");
        // Isolate the early-supper gate with housing and bread otherwise ready.
        opening.Food.Bread=opening.Food.BakedBread=16;opening.Food.GrownGrain=opening.Food.UsedGrain=8;
        Check(opening.FinaleSupperProblem()!=null && !opening.CanCelebrate && !opening.BeginSupper(),"Early supper bypassed assessments");
        Reload(opening);
        RunRoute(false,false,false);RunRoute(true,false,false);RunRoute(true,true,false);RunRoute(true,false,true);
        Console.WriteLine("PASS: finale campaign routes, earned phases, early gate, extra residents, recovery and phase saves.");
    }
    internal static World RunRoute(bool local,bool poor,bool extra)
    {
        string stem=$"{(local?"local":"central")}-{(poor?"recovery":extra?"extra":"normal")}";
        var w=World.NewCampaign(10);
        var bridge=Build(w,new(6,3),BuildingKind.Bridge,1);
        Build(w,new(-2,0),BuildingKind.Cottage);
        Until(w,()=>w.PlacementProblem(new(1,5),0,BuildingKind.Cottage)==null,"opening home access");Build(w,new(1,5),BuildingKind.Cottage);
        w.Assign(7,Role.Logger);Until(w,()=>bridge.Complete && w.Beds>=12,"opening homes");Grow(w,12);w.Assign(8,Role.Farmer);w.Assign(9,Role.Builder);
        w=Reload(w,"prepared-first");Check(w.AdvanceFinalePhase(),"First assessment rejected");w=Reload(w,"assessing-first");
        Until(w,()=>w.Campaign!.Finale!.Phase==2,"first campaign assessment");
        float first=w.Food.Time;w=Reload(w,"earned-first");
        Check(!w.BeginSupper(),"First assessment alone allowed supper");
        SecondBuild(w,local);Until(w,()=>w.Beds>=20,"expanded homes");Grow(w,20);
        foreach(int id in new[]{10,12,14})w.Assign(id,Role.Farmer);
        if(extra)
        {
            Build(w,new(17,-5),BuildingKind.Cottage);Until(w,()=>w.Beds>=22 && w.InvitationProblem()==null,"extra home and provisions");
        }
        w=Reload(w,"prepared-second");Check(w.AdvanceFinalePhase(),"Second assessment rejected");
        if(extra)
        {
            Grow(w,22);
            Check(w.ReadCampaignConditions().Single(c=>c.Key=="rest").Required==17 && w.ReadCampaignConditions().Single(c=>c.Key=="recreation").Required==11,"Additional residents did not scale services");
        }
        w=Reload(w,"assessing-second");
        Until(w,()=>w.Campaign!.Finale!.Phase==4,"expanded campaign assessment");float supported=w.Food.Time;
        w=Reload(w,"earned-second");
        CelebrationBuild(w,local,poor);w.Assign(16,Role.Farmer);w.Assign(17,Role.Baker);if(!poor)w.Assign(19,Role.Baker);
        if(poor)
        {
            float end=w.Food.Time+600;Until(w,()=>w.Food.Time>=end,"poor production observation");
            Check(!w.CanCelebrate && w.Campaign!.Finale!.Phase==4,"Poor reserve erased earned services or no longer needs recovery");
            w=Reload(w,"poor");
            foreach(var site in w.Cottages.Where(c=>c.Kind is BuildingKind.Farm or BuildingKind.Bakery).ToArray())
            {Check(w.RequestDemolition(site.Id),"Recovery order failed");Until(w,()=>!w.Cottages.Contains(site),"recover poor production");}
            w=Reload(w);CelebrationBuild(w,local,false);w.Assign(19,Role.Baker);
        }
        Until(w,()=>w.CanCelebrate,"campaign supper reserve");w=Reload(w,"ready-supper");
        Check(w.BeginSupper(),"Supper rejected after earned service");w=Reload(w,"gathering");
        Until(w,()=>w.Food.SupperComplete,"shared campaign supper");
        Check(w.Campaign!.Complete && w.Campaign.Finale!.Phase==5 && w.Food.SupperBread==w.Population*2,"Supper did not immediately finish campaign");
        w=Reload(w,"complete");File.WriteAllText($"artifacts/finale-campaign-{stem}.json",w.SaveJson());
        var book=new CampaignBook();book.Capture(w);string bookPath=Path.Combine(Path.GetTempPath(),"inlanders-finale-"+Guid.NewGuid()+".json");book.SaveFile(bookPath);
        Check(CampaignBook.LoadFile(bookPath).Completed.Contains(10),"Completion not recorded");
        Console.WriteLine($"Finale campaign {stem}: first {first:F0}s, expanded {supported:F0}s, supper {w.Food.Time:F0}s; {w.Population} residents.");
        return w;
    }
}

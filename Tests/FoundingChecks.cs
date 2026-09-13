using Inlanders.Simulation;
using System.Text.Json;
static class FoundingChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    public static void Run()
    {
        Directory.CreateDirectory("artifacts/founding");
        foreach(string arm in new[]{"fish","woods","bulk"})
        {
            var w=World.NewFoundingSettlement();var events=new List<object>();
            Check(w.Housed==2 && w.Population==8 && w.Founding!=null && w.SharedWork && w.HasWorkplaceFood,"Opening rules");
            Check(w.InvitationProblem()!=null && !w.FinishFounding(),"Opening gate bypass");
            void Build(BuildingKind kind,Cell at)
            {
                var p=ReviewPlacement.Find(w,kind,at,(c,r)=>true,"founding")??throw new Exception("No site for "+kind);
                Check(w.Place(p.Actual,p.Rotation,kind)!=null,"Build rejected");events.Add(new{time=w.Food.Time,kind,cell=p.Actual});
            }
            foreach(var at in new[]{new Cell(-3,0),new(0,0),new(0,6)})Build(BuildingKind.Cottage,at);
            Build(arm=="woods"?BuildingKind.HuntingLodge:BuildingKind.FishingDock,arm=="woods"?new(-5,-2):new(3,3));
            if(arm=="bulk")foreach(var at in new[]{new Cell(-3,5),new(-6,6)})Build(BuildingKind.Cottage,at);
            float? done=null;double hungry=0;bool secondStage=false;
            for(int i=0;i<18000;i++)
            {
                w.Tick(.1f);hungry+=w.People.Count(p=>!p.Fed)*.1;
                if(arm!="bulk" && !secondStage && w.Housed==8 && w.Food.Time>=120)
                {Build(BuildingKind.Cottage,new(-3,5));Build(BuildingKind.Cottage,new(-6,6));Build(BuildingKind.VegetableGarden,new(-7,2));secondStage=true;}
                if(w.Population<12 && w.InvitationProblem()==null){Check(w.InviteNewcomers(),"Invitation rejected");Check(w.People.TakeLast(2).All(p=>p.SharedWorker),"Arrivals not shared workers");events.Add(new{time=w.Food.Time,invite=w.Population});}
                if(done==null && w.FinishFoundingProblem()==null){done=w.Food.Time;Check(w.FinishFounding(),"Finish rejected");}
                if(i%100==0)w.Validate();
                if(i==1199)w.SaveFile("artifacts/founding/"+arm+"-early.json");
                if(done!=null && w.Food.Time>done+180)break;
            }
            w.Validate();var json=w.SaveJson();Check(World.LoadJson(json).SaveJson()==json,"Save mismatch");
            w.SaveFile("artifacts/founding/"+arm+".json");
            var result=new{arm,done,hungry,w.Population,w.Housed,food=w.EdibleStored,events};
            File.WriteAllText("artifacts/founding/"+arm+"-report.json",JsonSerializer.Serialize(result,new JsonSerializerOptions{WriteIndented=true}));
            Console.WriteLine(JsonSerializer.Serialize(result));
            if(arm!="bulk")Check(done!=null,"Supported route did not settle");
        }
        Console.WriteLine("PASS founding routes/current saves; bulk result is a design check, not a difficulty assertion");
    }
}

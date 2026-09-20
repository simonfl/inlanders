using Inlanders.Simulation;
using System.Text.Json;

static class RiverFarmsteadChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    public static void Run()
    {
        const string folder="artifacts/river-farmstead";Directory.CreateDirectory(folder);
        var initial=World.NewRiverFarmstead();initial.SaveFile(folder+"/initial.json");
        var results=new List<object>();
        foreach(var arm in new[]{"cultivation","mixed","bread","delayed","homes-only"})
        {
            var w=World.LoadJson(initial.SaveJson());var orders=new List<object>();
            Check(w.Population==8 && w.Housed==2 && !w.FinishFounding(),"Invalid first scene");
            void Build(BuildingKind kind,Cell at)
            {
                var p=ReviewPlacement.Find(w,kind,at,(c,r)=>true,"river farmstead")??throw new Exception("No site for "+kind);
                Check(w.Place(p.Actual,p.Rotation,kind)!=null,"Order rejected");orders.Add(new{time=w.Food.Time,kind,p.Actual,p.Rotation});
            }
            foreach(var cell in new[]{new Cell(-3,6),new(1,0),new(1,6)})Build(BuildingKind.Cottage,cell);
            void Food()
            {
                if(arm=="bread"){Build(BuildingKind.Farm,new(3,3));Build(BuildingKind.Bakery,new(0,-3));}
                else {Build(BuildingKind.VegetableGarden,new(3,3));Build(arm=="cultivation"?BuildingKind.VegetableGarden:BuildingKind.FishingDock,arm=="cultivation"?new(4,7):new(8,3));}
            }
            if(arm!="delayed" && arm!="homes-only")Food();
            float? finished=null;double hungry=0,lateHungry=0;
            for(int i=0;i<9000;i++)
            {
                if(arm=="delayed" && i==4200)Food();
                w.Tick(.1f);hungry+=w.People.Count(p=>!p.Fed)*.1;
                if(i>=7200)lateHungry+=w.People.Count(p=>!p.Fed)*.1;
                if(finished==null && w.FinishFoundingProblem()==null){finished=w.Food.Time;Check(w.FinishFounding(),"Finish failed");}
                if(i%300==0)w.Validate();
                if(i==2999){w.SaveFile(folder+"/"+arm+"-early.json");w=World.LoadJson(w.SaveJson());}
            }
            w.Validate();Check(World.LoadJson(w.SaveJson()).SaveJson()==w.SaveJson(),"Current save differs");
            w.SaveFile(folder+"/"+arm+".json");
            var row=new{arm,finished,hungry,lateHungry,w.Population,w.Housed,stored=w.EdibleStored,w.Food.EatenVegetables,w.Food.EatenFish,w.Food.EatenBread,orders};
            results.Add(row);Console.WriteLine(JsonSerializer.Serialize(row));
            if(arm=="homes-only")Check(finished==null && lateHungry>0,"Starting provisions bypass livelihood");
            else Check(finished!=null && w.Population==8 && w.Housed==8,"Eight-person route cannot finish: "+arm);
        }
        File.WriteAllText(folder+"/report.json",JsonSerializer.Serialize(results,new JsonSerializerOptions{WriteIndented=true}));
        Console.WriteLine("PASS: matched ordinary routes, no-growth finish, recovery, provisions-only failure and current saves. Not a human playtest.");
    }
}

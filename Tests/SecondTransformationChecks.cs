using Inlanders.Simulation;
using System.Text.Json;
static class SecondTransformationChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    public static void Run()
    {
        Directory.CreateDirectory("artifacts/second-transformation");var rows=new List<object>();
        var initial=World.NewTransformationHamlet();
        for(int i=0;i<3000;i++)initial.Tick(.1f);
        Check(initial.FoundingHasNewFood,"Livelihood never delivered");
        Check(initial.CommonsProblem(new(1,3))!=null,"Working garden does not contest domestic ground");
        initial.SaveFile("artifacts/second-transformation/working.json");
        foreach(string arm in new[]{"keep-gardens","make-room-at-home"})
        {
            var w=World.LoadJson(initial.SaveJson());var trace=new List<string>();
            if(arm=="make-room-at-home")
            {
                var replacement=w.Place(new(0,-9),0,BuildingKind.VegetableGarden);Check(replacement!=null,"Replacement field unavailable");
                for(int i=0;i<1800 && !replacement!.Complete;i++)w.Tick(.1f);
                Check(replacement!.Complete,"Replacement never built");
                var garden=w.Cottages.Single(c=>c.Cell==new Cell(1,3));Check(w.RequestDemolition(garden.Id),"Garden removal rejected");
                for(int i=0;i<3000 && w.Cottages.Any(c=>c.Id==garden.Id);i++)w.Tick(.1f);
                Check(w.Cottages.All(c=>c.Id!=garden.Id),"Garden removal stalled");
                Check(w.SetCommons(new(1,3)),"Reclaimed home ground unavailable");
                trace.Add("Built northern garden before recovering timber from kitchen plot; made shared ground near homes.");
            }
            else {Check(w.SetCommons(new(-1,-3)),"Northern shared ground unavailable");trace.Add("Retained kitchen gardens; chose shared ground across the inlet, without relocating production.");}
            double hungry=0,walking=0;int meals=0;var seen=new HashSet<int>();
            for(int i=0;i<6000;i++)
            {
                w.Tick(.1f);hungry+=w.People.Count(p=>!p.Fed)*.1;walking+=w.People.Count(p=>p.Task is Work.ToMealSupply or Work.ToMealSeat)*.1;
                foreach(var p in w.People.Where(p=>p.Meal?.Commons==true))seen.Add(p.Id);
                if(i%500==0)w.Validate();
            }
            meals=seen.Count;Check(meals>0,"Chosen shared ground never used");
            var copy=World.LoadJson(w.SaveJson());for(int i=0;i<100;i++){w.Tick(.1f);copy.Tick(.1f);}Check(w.SaveJson()==copy.SaveJson(),"Transformed save differs");
            w.SaveFile("artifacts/second-transformation/"+arm+".json");rows.Add(new{arm,hungry,walking,residentsUsingSharedGround=meals,food=w.EdibleStored,trace});
        }
        var report=JsonSerializer.Serialize(new{assembly=typeof(SecondTransformationChecks).Assembly.ManifestModule.ModuleVersionId,humanPlay=false,scope="two spatial intentions after ordinary initial food delivery; different investment/time, not a throughput ranking",rows},new JsonSerializerOptions{WriteIndented=true});
        File.WriteAllText("artifacts/second-transformation/report.json",report);Console.WriteLine(report);
        Console.WriteLine("PASS: working plot competes with domestic ground; two ordinary transformations, actual shared meals and exact saves.");
    }
}

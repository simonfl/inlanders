using Inlanders.Simulation;
static class CommonsChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    public static void Run()
    {
        var baseline=GatheringChecks.Prepare();string json=baseline.SaveJson();
        Directory.CreateDirectory("artifacts/commons");baseline.SaveFile("artifacts/commons/untouched.json");
        var center=baseline.Map.Land.Where(c=>baseline.CommonsProblem(c)==null).OrderBy(c=>(c.Point-new Cell(17,9).Point).LengthSquared()).First();
        var w=World.LoadJson(json);Check(w.SetCommons(center),"Place failed");w.SaveFile("artifacts/commons/recurring.json");
        var eventWorld=World.LoadJson(json);Check(eventWorld.BeginGathering(center),"Event failed");eventWorld.SaveFile("artifacts/commons/one-shot.json");
        Check(w.Commons!.Places.All(c=>w.PlacementProblem(c,0,BuildingKind.SeatingGarden)!=null),"Construction overwrites planned places");
        int peak=0,visits=0;var seen=new HashSet<int>();
        for(int i=0;i<3000;i++)
        {
            w.Tick(.1f);if(i%30==0)w.Validate();
            var meals=w.People.Where(p=>p.Meal is {Commons:true,Carrying:true}).ToArray();peak=Math.Max(peak,meals.Length);
            foreach(var p in meals)if(seen.Add(p.Meal!.Id))visits++;
            if(i==1500){var copy=World.LoadJson(w.SaveJson());for(int j=0;j<50;j++){w.Tick(.1f);copy.Tick(.1f);}Check(w.SaveJson()==copy.SaveJson(),"Continuation differs");}
        }
        Check(visits>6 && peak<=6,"Ordinary meals failed or exceeded six places");
        Check(w.Food.Hunger==0,"Near commons causes hunger");
        Check(w.SetCommons(w.Map.Land.First(c=>c!=center && w.CommonsProblem(c)==null)),"Move failed");w.Validate();
        Check(w.RemoveCommons(),"Remove failed");for(int i=0;i<1000;i++)w.Tick(.1f);w.Validate();Check(w.People.All(p=>p.Meal?.Commons!=true),"Removed place still claims meals");
        Console.WriteLine($"PASS: commons {visits} real meal trips, peak {peak}, protected places, saved continuation, move/removal and food recovery; no enjoyment inference");
    }
}

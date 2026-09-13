using Inlanders.Simulation;
using System.Text.Json;
using System.Numerics;

// Bounded F31b experiment. These layouts are evidence, not prescribed player solutions.
static class SpatialChoiceChecks
{
    public static readonly string[] Arms={"unchanged","first-legal","home-court","garden-side","remote"};
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    public static World Prepare(string arm,World? source=null)
    {
        var w=World.LoadJson((source??World.NewCourtExperience(true)).SaveJson());
        int food=w.EdibleStored,people=w.Population,buildings=w.Cottages.Count;
        void Move(Cottage site,Cell requested,int facing)
        {
            var at=w.Map.Land.OrderBy(c=>(c.Point-requested.Point).LengthSquared()).ThenBy(c=>c.Z).ThenBy(c=>c.X)
                .First(c=>c!=site.Cell && w.RelocationProblem(site.Id,c,facing)==null);
            Check(w.MoveBuilding(site.Id,at,facing),"Invalid experimental move");
        }
        if(arm=="home-court")
        {
            foreach(var (home,at) in w.Cottages.Where(c=>c.Kind==BuildingKind.Cottage).Take(2).ToArray().Zip(new[]{new Cell(-7,1),new(-7,5)}))Move(home,at,1);
            Move(w.Cottages.First(c=>c.Kind==BuildingKind.VegetableGarden),new(-2,0),0);
        }
        else if(arm=="garden-side")
        {
            var home=w.Cottages.Where(c=>c.Kind==BuildingKind.Cottage && c.Cell.X>5).First();
            Move(home,new(15,8),3);
        }
        if(arm!="unchanged")
        {
            Cell target=arm switch{"home-court"=>new(-2,3),"garden-side"=>new(10,5),"remote"=>new(-10,-7),_=>new(-1,3)};
            var center=w.Map.Land.OrderBy(c=>(c.Point-target.Point).LengthSquared()).ThenBy(c=>c.Z).ThenBy(c=>c.X)
                .First(c=>w.CommonsProblem(c)==null);
            Check(w.SetCommons(center),"Experimental commons rejected");
        }
        Check(w.Population==people && w.Cottages.Count==buildings && w.EdibleStored==food,"Experiment changed starting population/capability/food");
        w.Validate();return w;
    }
    public static World Observed(string arm)
    {
        var w=Prepare(arm);for(int i=0;i<3600;i++)w.Tick(.1f);w.Validate();return w;
    }
    public static void Run()
    {
        const string dir="artifacts/spatial-choice";Directory.CreateDirectory(dir);
        var baseline=World.NewCourtExperience(true);baseline.SaveFile(dir+"/baseline.json");var results=new List<object>();
        foreach(string arm in Arms)
        {
            var w=Prepare(arm,baseline);w.SaveFile(dir+"/"+arm+"-start.json");
            var requests=new HashSet<int>();var consumed=new HashSet<int>();var diners=new HashSet<int>();
            float? first=null;float missed=0,travel=0;int peak=0;
            int meals=w.Food.EatenBerries+w.Food.EatenVegetables;
            for(int i=0;i<3600;i++)
            {
                foreach(var p in w.People.Where(p=>p.Meal?.Commons==true))requests.Add(p.Meal!.Id);
                var positions=w.People.Select(p=>p.Position).ToArray();var walking=w.People.Select(p=>p.Task is Work.ToMealSupply or Work.ToMealSeat or Work.ReturnMeal).ToArray();
                w.Tick(.1f);
                for(int n=0;n<w.Population;n++)if(walking[n])travel+=Vector2.Distance(positions[n],w.People[n].Position);
                missed+=w.People.Count(p=>!p.Fed)*.1f;
                foreach(var m in w.Food.MealConsumptions.Where(m=>requests.Contains(m.Request)))if(consumed.Add(m.Request)){diners.Add(m.Person);first??=w.Food.Time-baseline.Food.Time;}
                peak=Math.Max(peak,w.People.Count(p=>p.Task==Work.EatingMeal && p.Meal?.Commons==true));
                if(i%100==0)w.Validate();
                if(i==1199)w.SaveFile(dir+"/"+arm+"-early.json");
            }
            w.Validate();string saved=w.SaveJson();Check(World.LoadJson(saved).SaveJson()==saved,"Current-format experiment save failed");w.SaveFile(dir+"/"+arm+"-late.json");
            results.Add(new{arm,start=baseline.Food.Time,end=w.Food.Time,firstCommonsMeal=first,commonsMeals=consumed.Count,distinctDiners=diners.Count,peakDiners=peak,missedResidentSeconds=missed,mealTravel=travel,ordinaryMeals=w.Food.EatenBerries+w.Food.EatenVegetables-meals,food=w.EdibleStored,center=w.Commons?.Center,buildings=w.Cottages.Select(c=>new{c.Id,c.Kind,c.Cell,c.Rotation})});
        }
        // Same-age recovery: only change the remote place's position, keeping food and jobs intact.
        var weak=World.LoadFile(dir+"/remote-late.json");var recovered=World.LoadJson(weak.SaveJson());
        var recoveryAt=recovered.Map.Land.OrderBy(c=>(c.Point-new Cell(10,5).Point).LengthSquared()).First(c=>recovered.CommonsProblem(c)==null && recovered.CommonsFoodNearby(c));
        Check(recovered.SetCommons(recoveryAt),"Recovery move failed");
        for(int i=0;i<1800;i++){weak.Tick(.1f);recovered.Tick(.1f);}
        weak.Validate();recovered.Validate();weak.SaveFile(dir+"/remote-control.json");recovered.SaveFile(dir+"/remote-recovered.json");
        var report=new{results,recovery=new{start=baseline.Food.Time+360,end=recovered.Food.Time,controlDiner=weak.Commons?.FirstDiner,recoveredDiner=recovered.Commons?.FirstDiner,controlFood=weak.EdibleStored,recoveredFood=recovered.EdibleStored},limits="Simulated routes and still-ready worlds, not player preference, motion or performance evidence."};
        string json=JsonSerializer.Serialize(report,new JsonSerializerOptions{WriteIndented=true});File.WriteAllText(dir+"/report.json",json);
        foreach(var result in results)Console.WriteLine(JsonSerializer.Serialize(result));
        Console.WriteLine("PASS: matched starting resources, legal actual moves, six-minute continuations, current saves and same-age recovery comparison; no enjoyment verdict");
    }
}

using Inlanders.Simulation;
using System.Text.Json;

// Optimistic timing proxy only: fruit is accounted as vegetables, not a playable orchard.
static class OrchardComparison
{
    public sealed class Grove
    {
        public int Id {get;set;}
        public float Age {get;set;}
        public bool Mature {get;set;}
    }
    static void Check(bool value,string message){if(!value)throw new Exception(message);}
    static int Delivered(World w)=>w.Food.GrownVegetables-w.Cottages.Where(c=>c.Kind==BuildingKind.VegetableGarden).Sum(c=>c.Harvest)-w.People.Where(p=>p.Cargo==Resource.Vegetables).Sum(p=>p.Carried);
    static void Tick(World w,List<Grove> groves,float establishment,float repeat)
    {
        // Suppress only the garden growth clock; work, claims and delivery remain real.
        foreach(var g in groves)
        {
            var c=w.Cottages.SingleOrDefault(c=>c.Id==g.Id);
            if(c is {Complete:true,DemolitionRequested:false,Planted:true,Harvest:0})c.Growth=0;
        }
        w.Tick(.1f);
        foreach(var g in groves)
        {
            var c=w.Cottages.SingleOrDefault(c=>c.Id==g.Id);
            if(c==null || !c.Complete || c.DemolitionRequested)continue;
            if(g.Mature && !c.Planted){c.Planted=true;c.Growth=0;g.Age=0;}
            if(!c.Planted || c.Harvest>0)continue;
            g.Age+=.1f;float duration=g.Mature?repeat:establishment;
            c.Growth=Math.Min(.999f,g.Age/duration);
            if(g.Age>=duration){c.Growth=1;c.Harvest=8;w.Food.GrownVegetables+=8;g.Mature=true;}
        }
    }
    public static void Run()
    {
        Directory.CreateDirectory("artifacts/orchard");var reports=new List<object>();
        foreach(bool backgroundForager in new[]{true,false})
        foreach(float establishment in new[]{180f,300f})foreach(float repeat in new[]{60f,75f,90f})
        foreach(string route in new[]{"gardens","early-grove","mixed","replace-half"})
        {
            var w=World.NewQuarryMap();w.Food.InitialBerries=w.Food.Berries=24;
            w.Assign(4,Role.Farmer); // One forager, two farmers, identical in every opening.
            if(!backgroundForager)w.Assign(3,Role.Unassigned);
            var first=w.Cottages.Single(c=>c.Kind==BuildingKind.VegetableGarden);
            var second=w.Place(new(3,-5),0,BuildingKind.VegetableGarden)??throw new Exception("Second plot unavailable");
            var groves=new List<Grove>();
            if(route!="gardens")groves.Add(new(){Id=second.Id});
            if(route is "early-grove" or "replace-half")groves.Add(new(){Id=first.Id});
            double labor=0,travel=0,planting=0;float firstDelivery=-1,replaced=-1;int replacement=-1;
            bool sawMiss=false;int minimumFood=w.Food.EdibleStored;
            var checkpoints=new List<object>();
            for(int tick=0;tick<12000;tick++)
            {
                if(tick==3600)w.Assign(4,Role.Builder);
                if(tick==4800)w.Assign(4,Role.Farmer);
                if(route=="replace-half" && tick==6000)Check(w.RequestDemolition(second.Id),"Cannot abandon grove proxy");
                if(route=="replace-half" && tick>6000 && replacement<0 && !w.Cottages.Any(c=>c.Id==second.Id) && w.PlacementProblem(second.Cell,0,BuildingKind.VegetableGarden)==null)
                {replacement=w.Place(second.Cell,0,BuildingKind.VegetableGarden)!.Id;replaced=w.Food.Time;}
                Tick(w,groves,establishment,repeat);w.Validate();
                sawMiss|=w.Food.MealOutcomes.Any(m=>m.Skipped || !m.Timely);minimumFood=Math.Min(minimumFood,w.Food.EdibleStored);
                labor+=w.People.Count(p=>p.Role==Role.Farmer && p.WorkplaceId!=null)*.1;
                travel+=w.People.Count(p=>p.Role==Role.Farmer && p.Route.Count>0)*.1;
                planting+=w.People.Count(p=>p.Task==Work.Planting)*.1;
                if(firstDelivery<0 && Delivered(w)>0)firstDelivery=w.Food.Time;
                if(tick is 2999 or 5999 or 11999)
                {
                    var meal=w.ReadMealAssessment();
                    checkpoints.Add(new{seconds=Math.Round(w.Food.Time),grown=w.Food.GrownVegetables,delivered=Delivered(w),eaten=w.Food.EatenVegetables,
                        food=w.Food.EdibleStored,meal.Reliable,meal.FreshSupply,meal.Missed,meal.Skipped,farmerWorkSeconds=Math.Round(labor,1),farmerTravelSeconds=Math.Round(travel,1),sowingSeconds=Math.Round(planting,1)});
                }
                if(tick==5999)
                {
                    string save=w.SaveJson(),proxy=JsonSerializer.Serialize(groves);var copy=World.LoadJson(save);Check(copy.SaveJson()==save,"World roundtrip differs");
                    var clone=JsonSerializer.Deserialize<List<Grove>>(proxy)!;
                    for(int k=0;k<100;k++){Tick(w,groves,establishment,repeat);Tick(copy,clone,establishment,repeat);}
                    Check(w.SaveJson()==copy.SaveJson() && JsonSerializer.Serialize(groves)==JsonSerializer.Serialize(clone),"World/proxy continuation differs");
                    w=World.LoadJson(save);groves=JsonSerializer.Deserialize<List<Grove>>(proxy)!;
                }
            }
            if(route=="replace-half")Check(replacement>=0 && w.Cottages.Single(c=>c.Id==replacement).Complete,"Abandoned plot did not recover");
            var report=new{route,backgroundForager,establishment,repeat,plots=2,tiles=12,yield=8,sawMiss,minimumFood,firstDelivery=Math.Round(firstDelivery,1),replacementOrderedAt=route=="replace-half"?600:-1,replacedAt=Math.Round(replaced,1),checkpoints};
            reports.Add(report);Console.WriteLine(JsonSerializer.Serialize(report));
        }
        File.WriteAllText("artifacts/orchard/results.json",JsonSerializer.Serialize(reports,new JsonSerializerOptions{WriteIndented=true}));
        Console.WriteLine("PASS: orchard timing proxy, actual food work/meals, labor interruption, physical demolition/rebuild, conservation and world/proxy continuation across 48 comparisons.");
    }
}

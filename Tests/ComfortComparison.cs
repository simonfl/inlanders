using Inlanders.Simulation;
using System.Text.Json;

public static class ComfortComparison
{
    static Cottage Place(World w,Cell target,BuildingKind kind)
    {
        var cell=w.Map.Land.Where(c=>w.PlacementProblem(c,false,kind)==null)
            .OrderBy(c=>(c.Point-target.Point).LengthSquared()).ThenBy(c=>c.X).ThenBy(c=>c.Z).First();
        return w.Place(cell,false,kind)!;
    }
    static void Tick(World w) { w.Tick(.1f);w.Validate(); }
    public static void Run()
    {
        Directory.CreateDirectory("artifacts");
        foreach(string layout in new[]{"compact","dispersed"})
        foreach(string housing in new[]{"cottages","lodges","spare-lodge"})
        {
            var w=World.NewLargeMap(false,false);w.Food.InitialBerries=w.Food.Berries=256;
            foreach(var p in w.People) w.Assign(p.Id,p.Id<2?Role.Logger:p.Id<4?Role.Builder:Role.Farmer);
            foreach(var target in new[]{new Cell(0,0),new(3,0),new(0,4),new(3,4)}) Place(w,target,BuildingKind.VegetableGarden);
            Place(w,new(-4,0),BuildingKind.Sawmill);w.Assign(1,Role.Sawyer);
            int homeX=layout=="compact"?0:11;
            var homes=new List<Cottage>();
            foreach(int z in new[]{-5,9})
            {
                if(housing=="cottages") { homes.Add(Place(w,new(homeX,z),BuildingKind.Cottage));homes.Add(Place(w,new(homeX+3,z),BuildingKind.Cottage)); }
                else homes.Add(Place(w,new(homeX,z),BuildingKind.Lodge));
            }
            if(housing=="spare-lodge") homes.Add(Place(w,new(homeX+3,9),BuildingKind.Lodge));
            Place(w,new(3,7),BuildingKind.Square);
            for(int i=0;i<18000 && (!w.Cottages.All(c=>c.Complete) || w.AvailablePlanks<12);i++) Tick(w);
            if(!w.Cottages.All(c=>c.Complete) || w.AvailablePlanks<12) throw new Exception("Comfort comparison setup stalled");
            if(housing=="spare-lodge")
            {
                // Move two people to a third lodge: 4/2/2 residents, so partial occupancy is real.
                foreach(var p in w.People.Where(p=>p.HomeId==homes[1].Id).Take(2).ToArray())
                    if(!w.AssignHome(p.Id,homes[2].Id)) throw new Exception("Partial lodge assignment failed");
            }
            w.Assign(2,Role.Farmer); // Same free construction worker in every branch.
            for(int i=0;i<1800;i++) Tick(w);
            string baseline=w.SaveJson();
            File.WriteAllText($"artifacts/comfort-{layout}-{housing}-start.json",baseline);
            foreach(string choice in new[]{"ordinary","improve","near-homes","food"})
                Compare(baseline,layout,housing,choice,homes.Select(c=>c.Id).ToArray());
        }
    }
    static void Compare(string baseline,string layout,string housing,string choice,int[] homeIds)
    {
        var w=World.LoadJson(baseline);float start=w.Food.Time;
        int initialDelivered=w.DeliveredVegetables,initialRest=w.People.Sum(p=>p.RestVisits),initialLeisure=w.People.Sum(p=>p.LeisureVisits);
        int initialLogs=w.Cottages.Where(c=>Buildings.Get(c.Kind).Material==Resource.Logs).Sum(c=>c.Delivered),initialPlanks=w.Cottages.Where(c=>Buildings.Get(c.Kind).Material==Resource.Planks).Sum(c=>c.Delivered);
        var projects=new List<Cottage>();bool done=choice=="ordinary";float? ready=done?0:null;
        if(choice=="improve")
        {
            projects.Add(Place(w,new(1,-2),BuildingKind.Carpenter));w.Assign(2,Role.Carpenter);
            foreach(int id in homeIds) if(!w.RequestImprovement(id)) throw new Exception("Comfort order rejected");
        }
        if(choice=="near-homes") foreach(int z in new[]{-3,7}) projects.Add(Place(w,new(-3,z),BuildingKind.Lodge));
        if(choice=="food") { projects.Add(Place(w,new(4,2),BuildingKind.VegetableGarden));projects.Add(Place(w,new(4,5),BuildingKind.Pantry));w.SetPantryTarget(projects[1].Id,0); }
        double restTravel=0,mealTravel=0,leisureTravel=0,projectWork=0,rested=0,recreation=0,hunger=0;
        int timely=0,missed=0,skipped=0;var observed=w.Food.MealOutcomes.Select(m=>m.Request).ToHashSet();
        int at600=0;
        for(int i=0;i<12000;i++)
        {
            if(!done && projects.All(c=>c.Complete) && (choice!="improve" || homeIds.All(id=>w.Cottages.Single(c=>c.Id==id).Improved)))
            {
                done=true;ready=w.Food.Time-start;
                if(choice=="improve") w.Assign(2,Role.Farmer);
                if(choice=="near-homes") foreach(var p in w.People)
                    if(!w.AssignHome(p.Id,projects[p.Id/4].Id)) throw new Exception("Relocation failed");
            }
            foreach(var p in w.People)
            {
                if(p.Task==Work.ToRest) restTravel+=.1;
                if(p.Task is Work.ToMealSupply or Work.ToMealSeat or Work.ReturnMeal) mealTravel+=.1;
                if(p.Task==Work.ToLeisure) leisureTravel+=.1;
                if(p.Task is Work.ToComfortPlanks or Work.ToComfortHome or Work.ToComfortInstall or Work.InstallingComfort or Work.ToMaterials or Work.ToCottage or Work.Building) projectWork+=.1;
                if(w.RecentlyRested(p)) rested+=.1;
                if(w.ReadHappiness(p).Leisure>0) recreation+=.1;
            }
            hunger+=w.Food.Hunger*.1;Tick(w);
            foreach(var m in w.Food.MealOutcomes.Where(m=>observed.Add(m.Request))) {if(m.Skipped)skipped++;else if(m.Timely)timely++;else missed++;}
            if(i==5999) at600=w.DeliveredVegetables-initialDelivered;
        }
        if(!done) throw new Exception($"Project stalled: {layout} {housing} {choice}");
        string saved=w.SaveJson();if(World.LoadJson(saved).SaveJson()!=saved) throw new Exception("Comparison save changed");
        var result=new {layout,housing,choice,setupSeconds=ready,logs=w.Cottages.Where(c=>Buildings.Get(c.Kind).Material==Resource.Logs).Sum(c=>c.Delivered)-initialLogs,
            planks=w.Cottages.Sum(c=>(Buildings.Get(c.Kind).Material==Resource.Planks?c.Delivered:0)+c.ImprovementPlanks)-initialPlanks,
            occupiedHomes=homeIds.Select(id=>new {id,residents=w.People.Count(p=>p.HomeId==id)}),
            delivered10m=at600,delivered20m=w.DeliveredVegetables-initialDelivered,
            restVisits=w.People.Sum(p=>p.RestVisits)-initialRest,leisureVisits=w.People.Sum(p=>p.LeisureVisits)-initialLeisure,
            restTravel,mealTravel,leisureTravel,projectWork,restCoverage=rested/9600,leisureCoverage=recreation/9600,hunger,timely,missed,skipped,
            remainingLogs=w.Available,remainingPlanks=w.AvailablePlanks};
        string json=JsonSerializer.Serialize(result);Console.WriteLine(json);
        File.WriteAllText($"artifacts/comfort-{layout}-{housing}-{choice}.json",json);
        File.WriteAllText($"artifacts/comfort-{layout}-{housing}-{choice}-world.json",saved);
    }
}

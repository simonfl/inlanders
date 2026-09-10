using System;
using System.Linq;
using Inlanders.Simulation;

public static class VegetableChecks
{
    static void Check(bool ok,string message) { if(!ok) throw new Exception(message); }
    static void Step(World w,int count=1) { for(int i=0;i<count;i++) { w.Tick(.1f); w.Validate(); } }
    static void Until(World w,Func<bool> done,string message) { for(int i=0;i<12000 && !done();i++) Step(w); Check(done(),message); }
    public static void Run()
    {
        var w=World.NewScenario(); w.Food.InitialBerries=w.Food.Berries=200;
        var garden=w.Place(new(3,-3),true,BuildingKind.VegetableGarden)!;
        Until(w,()=>garden.Complete,"Garden did not build");
        var phases=new System.Collections.Generic.HashSet<Work>();
        bool growth=false,ripe=false,partial=false;
        for(int i=0;i<6000 && (w.Food.GrownVegetables<16 || phases.Count<4);i++)
        {
            Step(w);
            var farmer=w.People[6];
            if(garden.Planted && garden.Growth>0 && garden.Growth<1 && !growth)
            {
                growth=true; var clone=World.LoadJson(w.SaveJson());
                Check(clone.SaveJson()==w.SaveJson(),"Growing garden save differs");
            }
            if(garden.Harvest==8) ripe=true;
            if(garden.Harvest==6) partial=true;
            if(farmer.WorkplaceId==garden.Id && farmer.Task is Work.ToFarm or Work.Planting or Work.Harvesting or Work.ToPantry && phases.Add(farmer.Task))
            {
                string saved=w.SaveJson(); var restored=World.LoadJson(saved);
                Check(restored.SaveJson()==saved,"Garden task changed on load");
                var uninterrupted=World.LoadJson(saved);
                Step(restored,200); Step(uninterrupted,200);
                Check(restored.SaveJson()==uninterrupted.SaveJson(),"Garden continuation diverged");
                var reassigned=World.LoadJson(saved); reassigned.Assign(6,Role.Builder);
                Step(reassigned,300);
                Check(reassigned.People[6].Cargo!=Resource.Vegetables || reassigned.People[6].Carried==0,"Reassignment stranded vegetables");
            }
        }
        Check(growth && ripe && partial && phases.Count==4 && w.Food.GrownVegetables>=16,"Garden cycle missed stages");
        Check(w.Food.GrownGrain==0 && w.Food.Grain==0,"Vegetables polluted grain accounting");
        var report=w.ReadEconomy();
        Check(report.Stocks.Single(s=>s.Resource==Resource.Vegetables).AtWorkplaces==garden.Harvest,"Ripe vegetables not shown separately");
        Check(!report.Issues.Any(i=>i.Build==BuildingKind.Farm),"Garden farmers incorrectly require a grain farm");

        // Precise meal preference and newcomer coverage use only stored edible food.
        var meals=new World(); foreach(var p in meals.People) meals.Assign(p.Id,Role.Unassigned);
        meals.Food.InitialBerries=meals.Food.Berries=2;
        meals.Food.Vegetables=meals.Food.GrownVegetables=4;
        meals.Food.Bread=meals.Food.BakedBread=4; meals.Food.UsedGrain=meals.Food.GrownGrain=2;
        Check(meals.ReadEconomy().Meals==1,"Vegetables omitted from food coverage");
        Until(meals,()=>meals.Food.MealConsumptions.Select(m=>m.Person).Distinct().Count()==8,"Physical meals did not reach everyone");
        Check(meals.Food.EatenBerries==2 && meals.Food.EatenVegetables==3 && meals.Food.EatenBread==3 && meals.Food.Hunger==0,"Balanced meal consumption wrong");
        Check(!meals.CanCelebrate,"Vegetables replaced supper bread");
        meals.Food.Vegetables+=20; meals.Food.GrownVegetables+=20;
        Check(meals.Food.EdibleStored>=meals.ArrivalFoodRequired,"Vegetables omitted from arrival reserve");
        var arrivals=PopulationChecks.Ready();
        arrivals.Food.EatenBerries+=arrivals.Food.Berries; arrivals.Food.Berries=0;
        arrivals.Food.Vegetables=arrivals.Food.GrownVegetables=20;
        Check(arrivals.InviteNewcomers(),"Vegetable-fed newcomers rejected"); arrivals.Validate();

        // The same farmer can supply both fields without mixing their outputs.
        var grain=w.Place(new(6,0),false,BuildingKind.Farm)!;
        Until(w,()=>grain.Complete,"Mixed field not built");
        Until(w,()=>w.Food.GrownGrain>=6,"Shared farmer starved the grain field");
        int grown=w.Food.GrownVegetables;
        Until(w,()=>w.Food.GrownVegetables>grown,"Shared farmer starved the vegetable garden");
        Console.WriteLine("PASS: garden construction, 8-unit growth/harvest cycles, food conservation, four saved/interrupted work phases, mixed farming, meal order, coverage and vegetable-fed arrivals.");
    }
}

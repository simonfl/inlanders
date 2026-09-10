using System;
using System.Linq;
using Inlanders.Simulation;

public static class HappinessChecks
{
    static void Check(bool ok,string message) { if(!ok) throw new Exception(message); }
    static void Step(World w,int n=1) { for(int i=0;i<n;i++) { w.Tick(.1f); w.Validate(); } }
    public static void Run()
    {
        var w=new World(40);
        foreach(var p in w.People) w.Assign(p.Id,Role.Unassigned);
        Check(w.ReadHappiness(w.People[0]).Score==40,"Initial satisfaction wrong");
        w.Food.Vegetables=w.Food.GrownVegetables=20;
        w.Food.Bread=w.Food.BakedBread=20; w.Food.UsedGrain=w.Food.GrownGrain=10;
        Check(w.ReadHappiness(w.People[0]).Choice==0,"Stock delivery changed past meal choices");
        w.Food.MealClock=59.9f; Step(w,2);
        Check(w.Food.LastMealChoices==3 && w.ReadHappiness(w.People[0]).Choice==20,"Meal choices not recorded");
        string saved=w.SaveJson(); Check(World.LoadJson(saved).SaveJson()==saved,"Meal history save changed");
        var square=w.Place(new(3,0),false,BuildingKind.Square)!;
        w.Assign(0,Role.Logger); w.Assign(1,Role.Builder);
        for(int i=0;i<4000 && !w.People.Any(p=>p.LastLeisureTime!=null);i++) Step(w);
        var visitor=w.People.FirstOrDefault(p=>p.LastLeisureTime!=null);
        Check(visitor!=null && w.ReadHappiness(visitor).Leisure==20,"Completed break did not improve mood");
        var clone=World.LoadJson(w.SaveJson());
        Check(clone.People[visitor!.Id].LastLeisureTime==visitor.LastLeisureTime,"Visit history lost");
        // Prevent another visit so the old benefit can expire.
        foreach(var p in clone.People) { clone.Assign(p.Id,Role.Unassigned); p.NextLeisureTime=clone.Food.Time+1000; }
        Step(clone,1201);
        Check(clone.ReadHappiness(clone.People[visitor.Id]).Leisure==0,"Leisure benefit never expired");
        var housed=PopulationChecks.Ready();
        Check(housed.ReadHappiness(housed.People[0]).Housing==10,"Completed housing missing");
        housed.Food.EatenBerries+=housed.Food.Berries; housed.Food.Berries=0;
        housed.Food.MealClock=59.9f; Step(housed,2);
        Check(housed.ReadHappiness(housed.People[0]).Meals==0 && housed.Food.LastMealChoices==0,"Missing meal not reflected");
        Check(housed.Food.WorkEfficiency==.5f,"Happiness added a hunger penalty");
        saved=housed.SaveJson(); var report=housed.ReadHappiness(housed.People[0]); _=housed.VillageHappiness;
        Check(saved==housed.SaveJson(),"Happiness read changed simulation");
        Console.WriteLine("PASS: meal-choice snapshots, actual square visits, benefit expiry, housing, hunger, saves and read-only happiness.");
    }
}

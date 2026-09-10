using System;
using System.Linq;
using Inlanders.Simulation;

public static class GardenLessonChecks
{
    static void Check(bool ok,string message) { if(!ok) throw new Exception(message); }
    static void Meal(World w) { w.Food.MealClock=59.9f; w.Tick(.2f); w.Validate(); }
    public static void Run()
    {
        var w=World.NewCampaign(5);
        foreach(var p in w.People) w.Assign(p.Id,Role.Unassigned);
        w.Food.Bread=w.Food.BakedBread=16; w.Food.GrownGrain=w.Food.UsedGrain=8;
        Meal(w); Check(w.Food.VegetableChoiceMeals==0,"Berries and bread bypassed vegetable lesson");
        w.Food.Vegetables=w.Food.GrownVegetables=8;
        var farmer=w.People[0]; farmer.Carried=2; farmer.Cargo=Resource.Vegetables; w.Food.Vegetables-=2;
        Check(w.DeliveredVegetables==6,"Carried vegetables counted as delivered");
        Meal(w); Check(w.Food.VegetableChoiceMeals==1,"Stored vegetable choices not counted");
        string saved=w.SaveJson(); var copy=World.LoadJson(saved);
        Check(copy.SaveJson()==saved && copy.Food.VegetableChoiceMeals==1,"Partial meal goal not saved");
        w.Food.EatenBerries+=w.Food.Berries; w.Food.Berries=0;
        w.Food.EatenBread+=w.Food.Bread; w.Food.Bread=0;
        Meal(w); Check(w.Food.VegetableChoiceMeals==1,"Vegetables alone counted as variety");
        Check(!w.Campaign!.Complete,"Meal progress bypassed garden/delivery goals");
        Console.WriteLine("PASS: vegetable participation, carried-food exclusion, partial-goal save and no single-food shortcut.");
    }
}

using Inlanders.Simulation;

public static class MealVarietyChecks
{
    static void Check(bool ok, string reason) { if (!ok) throw new Exception(reason); }
    static void Meal(World w)
    {
        float boundary=w.Food.Time+60-w.Food.MealClock;
        while(w.Food.Time<=boundary) { w.Tick(.1f); w.Validate(); }
    }
    static World Village(int berries, int vegetables, int bread)
    {
        var w = new World(); foreach (var p in w.People) { w.Assign(p.Id, Role.Unassigned); p.NextMealTime=0; }
        w.Food.InitialBerries = w.Food.Berries = berries;
        w.Food.GrownVegetables = w.Food.Vegetables = vegetables;
        w.Food.BakedBread = w.Food.Bread = bread;
        w.Food.GrownGrain = w.Food.UsedGrain = bread / 2;
        return w;
    }
    public static void Run()
    {
        var w = Village(30, 1, 0);
        Check(w.Food.LastMealChoices == 0 && w.MealVarietyScore == 0 && w.LastMealSummary == "No meal served yet.", "New village invented a meal");
        Meal(w);
        Check(w.Food.LastMealBerries == 7 && w.Food.LastMealVegetables == 1 && w.MealVarietyScore == 4, "One vegetable gave a full variety reward or went uneaten");
        Check(w.Food.VegetableChoiceMeals == 0, "Token vegetable passed campaign threshold");
        Meal(w); Check(w.Food.LastMealChoices == 1 && w.MealVarietyScore == 0 && w.Food.VegetableChoiceMeals == 0, "One vegetable repeatedly qualified");
        w = Village(20, 20, 20); Meal(w);
        Check(w.Food.LastMealBerries == 3 && w.Food.LastMealVegetables == 3 && w.Food.LastMealBread == 2 && w.MealVarietyScore == 20, "Balanced three-food meal incorrect");
        string saved = w.SaveJson(); var clone = World.LoadJson(saved);
        Check(clone.SaveJson() == saved && clone.LastMealSummary == w.LastMealSummary, "Meal portions were not saved exactly");
        Meal(w); Meal(clone); Check(w.SaveJson() == clone.SaveJson(), "Meal continuation diverged");
        w = Village(20, 20, 0); Meal(w);
        Check(w.MealVarietyScore == 16 && w.Food.VegetableChoiceMeals == 1, "Two-food village was not viable");
        w = Village(1, 1, 2); Meal(w);
        Check(w.Food.LastMealBerries + w.Food.LastMealVegetables + w.Food.LastMealBread == 4 && w.Food.Hunger == .5f && w.Food.VegetableChoiceMeals == 0, "Variety stranded food or hid a shortage");
        w = Village(24, 2, 0);
        w.People.Add(new() { Id = 8, Name = "Ninth" }); w.People.Add(new() { Id = 9, Name = "Tenth" });
        Meal(w); Check(w.Food.VegetableChoiceMeals == 0 && w.Food.LastMealRequired == 10, "Larger population used the eight-person threshold");
        w.Food.Vegetables += 3; w.Food.GrownVegetables += 3;
        Meal(w); Check(w.Food.VegetableChoiceMeals == 1, "Rounded-up quarter threshold not met");
        Console.WriteLine("PASS: actual meal portions, proportional variety, token-food exclusion, balanced service, shortages, expanded population and exact meal saves.");
    }
}

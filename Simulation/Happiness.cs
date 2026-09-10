using System;
using System.Linq;

namespace Inlanders.Simulation;

public sealed record HappinessReport(int Meals, int Choice, int Housing, int Leisure, int FoodChoices, bool Creative = false, int Rest = 0)
{
    public int Score => 10 + Meals + Choice + Housing + Rest + Leisure;
    public string Mood => Score >= 85 ? "Cheerful" : Score >= 70 ? "Content" : Score >= 40 ? "Settling in" : "Unsettled";
    public string Reasons => (Creative ? "Food needs disabled in Creative: +50/50" : $"Meals: +{Meals}/30\nVillage meal variety: +{Choice}/20 ({FoodChoices} types eaten)") + $"\nAssigned home: +{Housing}/10\nRecent home rest: +{Rest}/10\nRecent square break: +{Leisure}/20\nStarting optimism: +10";
}
public sealed partial class World
{
    public HappinessReport ReadHappiness(Villager person) => new(
        Creative ? 30 : (int)MathF.Round(30 * (1 - Food.Hunger)),
        Creative ? 20 : MealVarietyScore,
        person.HomeId!=null ? 10 : 0,
        person.LastLeisureTime is float last && Food.Time - last < 120 ? 20 : 0,
        Food.LastMealChoices, Creative, RecentlyRested(person) ? 10 : 0);
    public int VillageHappiness => (int)Math.Round(People.Average(p => ReadHappiness(p).Score));
    // Reward portions outside the dominant food, relative to a balanced three-food meal for this population.
    public int MealVarietyScore => Food.LastMealRequired == 0 ? 0 : Math.Min(20, (int)MathF.Round(20f *
        Food.LastMealNonDominant /
        Math.Max(1, Food.LastMealRequired - (int)Math.Ceiling(Food.LastMealRequired / 3f))));
    public string LastMealSummary => Food.LastMealRequired == 0 ? "No meal served yet." :
        $"Last meal: {Food.LastMealServed}/{Food.LastMealRequired} portions eaten\n{Food.LastMealBerries} berries · {Food.LastMealVegetables} vegetables · {Food.LastMealBread} bread"+(Food.LastMealFish>0 ? $" · {Food.LastMealFish} fish" : "")+$"\nVillage meal variety: +{MealVarietyScore}/20. Three balanced foods can earn full credit; every food type is not required.";
    private void ValidateHappiness()
    {
        if (Food.LastMealRequired < 0 || Food.LastMealBerries < 0 || Food.LastMealVegetables < 0 || Food.LastMealBread < 0 || Food.LastMealFish < 0 ||
            Food.LastMealServed > Food.LastMealRequired ||
            Food.LastMealChoices != new[] { Food.LastMealBerries, Food.LastMealVegetables, Food.LastMealBread, Food.LastMealFish }.Count(n => n > 0))
            throw new InvalidOperationException("Invalid actual meal history");
        if (Food.VegetableChoiceMeals < 0 || Food.LastMealChoices is < 0 or > 4 || People.Any(p => p.LastLeisureTime is float t &&
            (!float.IsFinite(t) || t < 0 || t > Food.Time)))
            throw new InvalidOperationException("Invalid happiness history");
    }
}

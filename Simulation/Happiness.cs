using System;
using System.Linq;

namespace Inlanders.Simulation;

public sealed record HappinessReport(int Meals, int Choice, int Housing, int Leisure, int FoodChoices, bool Creative = false)
{
    public int Score => 10 + Meals + Choice + Housing + Leisure;
    public string Mood => Score >= 85 ? "Cheerful" : Score >= 70 ? "Content" : Score >= 40 ? "Settling in" : "Unsettled";
    public string Reasons => Creative ? $"Food needs disabled in Creative: +50/50\nHousing coverage: +{Housing}/20\nRecent square break: +{Leisure}/20\nStarting optimism: +10" : $"Meals: +{Meals}/30\nFood choice: +{Choice}/20 ({FoodChoices} types at last meal)\nHousing coverage: +{Housing}/20\nRecent square break: +{Leisure}/20\nStarting optimism: +10";
}
public sealed partial class World
{
    public HappinessReport ReadHappiness(Villager person) => new(
        Creative ? 30 : (int)MathF.Round(30 * (1 - Food.Hunger)),
        Creative ? 20 : Math.Max(0, Food.LastMealChoices - 1) * 10,
        (int)MathF.Round(20 * Housed / (float)Population),
        person.LastLeisureTime is float last && Food.Time - last < 120 ? 20 : 0,
        Food.LastMealChoices, Creative);
    public int VillageHappiness => (int)Math.Round(People.Average(p => ReadHappiness(p).Score));
    private void ValidateHappiness()
    {
        if (Food.VegetableChoiceMeals < 0 || Food.LastMealChoices is < 0 or > 3 || People.Any(p => p.LastLeisureTime is float t &&
            (!float.IsFinite(t) || t < 0 || t > Food.Time)))
            throw new InvalidOperationException("Invalid happiness history");
    }
}

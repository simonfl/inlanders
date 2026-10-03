using System;
using System.Linq;

namespace Inlanders.Simulation;

public sealed record HappinessReport(int Meals, int Choice, int Housing, int Leisure, int FoodChoices, bool Creative = false, int Rest = 0, bool PublicLife = false)
{
    public int Score => PublicLife ? (Creative || Meals>0?40:0)+(Housing>0?20:0)+(Rest>0?20:0)+(Leisure>0?20:0) : 10 + Meals + Choice + Housing + Rest + Leisure;
    public string Mood => PublicLife ? "Daily life" : Score >= 85 ? "Cheerful" : Score >= 70 ? "Content" : Score >= 40 ? "Settling in" : "Unsettled";
    public string Reasons => PublicLife ? (Meals>0?"Ate an ordinary meal":"Missing a recent meal"+(Creative?" · hunger penalties off":""))+"\n"+(Housing>0?"Has a home":"Needs a home")+"\n"+(Rest>0?"Rested at home recently":"Has not rested at home recently")+"\n"+(Leisure>0?"Enjoyed a recent break":"Has not taken a recent break") : (Creative ? "Food mood penalties disabled in Creative: +50/50" : $"Meals: +{Meals}/30\nVillage meal variety: +{Choice}/20 ({FoodChoices} types eaten)") + $"\nAssigned home: +{Housing}/10\nRecent home rest: +{Rest}/10\nRecent recreation: +{Leisure}/20\nStarting optimism: +10";
}
public sealed partial class World
{
    public HappinessReport ReadHappiness(Villager person) => new(
        (Creative && PublicPlace==null) || person.Fed ? 30 : 0,
        PublicPlace!=null ? 0 : Creative ? 20 : MealVarietyScore,
        person.HomeId!=null ? 10 : 0,
        person.LastLeisureTime is float last && Food.Time - last < person.LastLeisureWindow ? 20 : 0,
        Food.LastMealChoices, Creative, RecentlyRested(person) ? 10 : 0, PublicPlace!=null);
    public string PublicDailyLife(Villager person)
    {
        var meal=Food.MealConsumptions.LastOrDefault(m=>m.Person==person.Id);
        string eating=meal==null?"No eating recorded in the last three minutes.":$"Ate {meal.Kind.ToString().ToLowerInvariant()} {(int)(Food.Time-meal.Time)}s ago.";
        return person.Status+"\n"+eating+"\n"+RestSummary(person)+"\n"+RecreationSummary(person);
    }
    public int VillageHappiness => (int)Math.Round(People.Average(p => ReadHappiness(p).Score));
    // Reward portions outside the dominant food, relative to a balanced three-food meal for this population.
    public int MealVarietyScore => Food.LastMealRequired == 0 ? 0 : Math.Min(20, (int)MathF.Round(20f *
        Food.LastMealNonDominant /
        Math.Max(1, Food.LastMealRequired - (int)Math.Ceiling(Food.LastMealRequired / 3f))));
    public string LastMealSummary => Food.LastMealRequired == 0 ? "No meal served yet." :
        $"Latest minute snapshot: {Food.LastMealServed}/{Food.LastMealRequired} residents ate\n{Food.LastMealBerries} berries · {Food.LastMealVegetables} vegetables · {Food.LastMealBread} bread"+(Food.LastMealFish>0 ? $" · {Food.LastMealFish} fish" : "")+(Food.LastMealGame>0 ? $" · {Food.LastMealGame} game" : "")+(Food.LastMealFruit>0 ? $" · {Food.LastMealFruit} fruit" : "")+$"\nUses each resident's latest eaten portion in that minute; collected food does not count. "+(PublicPlace!=null?"Food variety is optional.":$"Village meal variety: +{MealVarietyScore}/20. Three balanced foods can earn full credit; every food type is not required.");
    private void ValidateHappiness()
    {
        if (Food.LastMealRequired < 0 || Food.LastMealBerries < 0 || Food.LastMealVegetables < 0 || Food.LastMealBread < 0 || Food.LastMealFish < 0 || Food.LastMealGame < 0 ||
            Food.LastMealServed > Food.LastMealRequired ||
            Food.LastMealFruit<0 || Food.LastMealChoices != new[] { Food.LastMealBerries, Food.LastMealVegetables, Food.LastMealBread, Food.LastMealFish, Food.LastMealGame, Food.LastMealFruit }.Count(n => n > 0))
            throw new InvalidOperationException("Invalid actual meal history");
        if (People.Any(p=>p.LastLeisureWindow is not (120 or 240)) || Food.VegetableChoiceMeals < 0 || Food.LastMealChoices<0 || Food.LastMealChoices>EdibleKinds.Length || People.Any(p => p.LastLeisureTime is float t &&
            (!float.IsFinite(t) || t < 0 || t > Food.Time)))
            throw new InvalidOperationException("Invalid happiness history");
    }
}

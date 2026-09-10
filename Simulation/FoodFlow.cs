using System;
using System.Collections.Generic;
using System.Linq;

namespace Inlanders.Simulation;

public sealed record FoodFlowEvent(float Time, int Berries = 0, int Vegetables = 0, int Bread = 0, int Eaten = 0, int Required = 0, int Fish = 0, int Game = 0);
public sealed record FoodFlowReport(float Seconds, int Berries, int Vegetables, int Bread, int Eaten, int Required, int Fish = 0, int Game = 0)
{
    public int Delivered => Berries + Vegetables + Bread + Fish + Game;
}
public sealed partial class World
{
    public const float FoodFlowWindow = 180;
    public List<FoodFlowEvent> RecentFood { get; private set; } = new();
    public FoodFlowReport ReadFoodFlow() => new(Math.Min(Food.Time, FoodFlowWindow), RecentFood.Sum(e => e.Berries),
        RecentFood.Sum(e => e.Vegetables), RecentFood.Sum(e => e.Bread), RecentFood.Sum(e => e.Eaten), RecentFood.Sum(e => e.Required),RecentFood.Sum(e=>e.Fish),RecentFood.Sum(e=>e.Game));
    private void RecordFoodDelivery(Resource food, int amount)
    {
        if (food is Resource.Berries or Resource.Vegetables or Resource.Bread or Resource.Fish or Resource.Game)
            RecentFood.Add(new(Food.Time, food == Resource.Berries ? amount : 0, food == Resource.Vegetables ? amount : 0, food == Resource.Bread ? amount : 0,Fish:food==Resource.Fish ? amount : 0,Game:food==Resource.Game ? amount : 0));
    }
    private void ValidateFoodFlow()
    {
        if (RecentFood == null || RecentFood.Any(e => e == null || !float.IsFinite(e.Time) || e.Time < 0 || e.Time > Food.Time ||
            e.Berries < 0 || e.Vegetables < 0 || e.Bread < 0 || e.Fish<0 || e.Game<0 || e.Eaten < 0 || e.Required < 0) ||
            RecentFood.Zip(RecentFood.Skip(1)).Any(pair => pair.First.Time > pair.Second.Time))
            throw new InvalidOperationException("Invalid recent food history");
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Inlanders.Simulation;

public sealed class BerryBush
{
    public int Id { get; init; }
    public Cell Cell { get; init; }
    public int Ripe { get; set; } = 8;
    public float Regrowth { get; set; }
    public int? Owner { get; set; }
    public Cell Access => new(Cell.X + 1, Cell.Z);
}
public sealed class FoodState
{
    public int LastMealChoices { get; set; } = 1;
    public int InitialBerries { get; set; } = 24;
    public int Berries { get; set; } = 24;
    public int Vegetables { get; set; }
    public int GrownVegetables { get; set; }
    public int EatenVegetables { get; set; }
    public int EdibleStored => Berries + Vegetables + Bread;
    public int Grain { get; set; }
    public int Bread { get; set; }
    public int GatheredBerries { get; set; }
    public int GrownGrain { get; set; }
    public int BakedBread { get; set; }
    public int UsedGrain { get; set; }
    public int EatenBerries { get; set; }
    public int EatenBread { get; set; }
    public int SupperBread { get; set; }
    public float Time { get; set; }
    public float MealClock { get; set; }
    public float Hunger { get; set; }
    public bool Celebrating { get; set; }
    public bool SupperComplete { get; set; }
    public float MeetingClock { get; set; }
    public float WorkEfficiency => 1 - 0.5f * Hunger;
    public int Day => 1 + (int)(Time / 60);
}

public sealed partial class World
{
    public int SupperCost => Population * 2;
    public FoodState Food { get; private set; } = new();
    public List<BerryBush> Bushes { get; } = new();
    public List<Cell> MeetingSpots { get; } = new();
    public int ReservedGrain => People.Where(v => v.Task == Work.ToGrain).Sum(v => v.FoodReserved);
    public bool CanCelebrate => Housed == Population && Food.Bread >= SupperCost && !Food.Celebrating && !Food.SupperComplete && (Campaign?.Level != 4 || HasBuilding(BuildingKind.Square)) && SupperSpots().Count == Population;

    private void InitializeFood()
    {
        foreach (var cell in new[] { new Cell(-7,2), new(-8,4), new(0,-2) }) Bushes.Add(new BerryBush { Id = Bushes.Count, Cell = cell });
    }
    public static World NewScenario()
    {
        var world = new World();
        var roles = new[] { Role.Logger, Role.Logger, Role.Builder, Role.Builder, Role.Forager, Role.Forager, Role.Farmer, Role.Baker };
        for (int i = 0; i < 8; i++) world.Assign(i, roles[i]);
        return world;
    }
    private void ReleaseFoodClaims(Villager v)
    {
        if (v.BushId is int id) Bushes.Single(b => b.Id == id).Owner = null;
        v.BushId = null; v.WorkplaceId = null; v.FoodReserved = 0;
    }
    private static bool IsField(Cottage c) => c.Kind is BuildingKind.Farm or BuildingKind.VegetableGarden;
    private bool FreeStation(Cottage c, int capacity = 1) => People.Count(v => v.WorkplaceId == c.Id) < capacity;
    private Cottage? FoodSite(BuildingKind kind, Func<Cottage, bool> condition, int capacity = 1) =>
        Cottages.Where(c => c.Complete && c.Kind == kind && FreeStation(c, capacity) && condition(c))
            .OrderByDescending(c => c.Priority).ThenBy(c => c.Id).FirstOrDefault();
    private void ClaimFoodWork(Villager v)
    {
        if (v.Role == Role.Forager)
        {
            var hut = FoodSite(BuildingKind.ForagerHut, _ => true, 2);
            if (hut == null) { v.Status = "Needs a finished forager hut with a free worker slot (2 per hut)"; return; }
            var bush = Bushes.Where(b => b.Ripe > 0 && b.Owner == null && Accessible(b.Access)).OrderBy(b => (b.Access.Point - v.Position).LengthSquared()).FirstOrDefault();
            if (bush == null) { v.Status = "Waiting for ripe reachable berries or another forager; a bridge may open more patches"; return; }
            v.WorkplaceId = hut.Id; v.BushId = bush.Id; bush.Owner = v.Id;
            Go(v, bush.Access, Work.ToBush, "Walking to ripe berries"); return;
        }
        if (v.Role == Role.Farmer)
        {
            var fields = Cottages.Where(c => c.Complete && IsField(c) && FreeStation(c)).OrderByDescending(c => c.Priority).ThenBy(c => c.Id);
            var farm = fields.FirstOrDefault(c => c.Harvest > 0) ?? fields.FirstOrDefault(c => !c.Planted);
            if (farm == null) { v.Status = Cottages.Any(c => c.Complete && IsField(c)) ? "Waiting for crops to ripen or a free field" : "Needs a finished farm or vegetable garden"; return; }
            v.WorkplaceId = farm.Id;
            Go(v, farm.Entrance, Work.ToFarm, farm.Harvest > 0 ? "Walking to harvest grain" : "Walking to sow grain"); return;
        }
        var bakery = FoodSite(BuildingKind.Bakery, c => c.OutputBread > 0) ?? FoodSite(BuildingKind.Bakery, c => c.InputGrain > 0)
            ?? FoodSite(BuildingKind.Bakery, _ => Food.Grain - ReservedGrain >= 2);
        if (bakery == null)
        {
            v.Status = Cottages.Any(c => c.Complete && c.Kind == BuildingKind.Bakery) ? "Waiting for 2 unreserved grain or a free bakery" : "Needs a finished bakery"; return;
        }
        v.WorkplaceId = bakery.Id;
        if (bakery.OutputBread > 0) Go(v, bakery.Entrance, Work.ToBread, "Collecting baked bread");
        else if (bakery.InputGrain > 0) Go(v, bakery.Entrance, Work.ToOven, "Resuming the bakery's batch");
        else { v.FoodReserved = 2; Go(v, YardAccess, Work.ToGrain, "Fetching 2 reserved grain for the bakery"); }
    }
    private void TickFoodWork(Villager v, float dt)
    {
        Cottage Station() => Cottages.Single(c => c.Id == v.WorkplaceId);
        void CarryFood(Resource resource, int amount)
        {
            v.Cargo = resource; v.Carried = amount;
            Go(v, YardAccess, Work.ToPantry, $"Carrying {amount} {resource.ToString().ToLowerInvariant()} to the pantry");
        }
        switch (v.Task)
        {
            case Work.ToBush: v.Task = Work.Foraging; v.Timer = 0; v.Status = "Picking ripe berries"; break;
            case Work.Foraging:
                if (v.Timer < 2) break;
                var bush = Bushes.Single(b => b.Id == v.BushId); int picked = Math.Min(2, bush.Ripe);
                bush.Ripe -= picked; bush.Owner = null; v.BushId = null; Food.GatheredBerries += picked; CarryFood(Resource.Berries, picked); break;
            case Work.ToFarm:
                v.Task = Station().Harvest > 0 ? Work.Harvesting : Work.Planting; v.Timer = 0;
                v.Status = v.Task == Work.Harvesting ? Station().Kind == BuildingKind.VegetableGarden ? "Harvesting ripe vegetables" : "Harvesting ripe grain" : "Sowing the next crop"; break;
            case Work.Planting:
                if (v.Timer < 4) break;
                Station().Planted = true; Station().Growth = 0; Finish(v); break;
            case Work.Harvesting:
                if (v.Timer < 2) break;
                var farm = Station(); int grain = Math.Min(2, farm.Harvest); farm.Harvest -= grain;
                if (farm.Harvest == 0) { farm.Planted = false; farm.Growth = 0; }
                CarryFood(farm.Kind == BuildingKind.VegetableGarden ? Resource.Vegetables : Resource.Grain, grain); break;
            case Work.ToGrain:
                Food.Grain -= v.FoodReserved; v.Carried = v.FoodReserved; v.Cargo = Resource.Grain; v.FoodReserved = 0;
                Go(v, Station().Entrance, Work.ToOven, "Delivering grain to the oven"); break;
            case Work.ToOven:
                var oven = Station(); oven.InputGrain += v.Carried; v.Carried = 0;
                v.Task = Work.Baking; v.Status = "Baking 2 grain into 4 loaves"; break;
            case Work.Baking:
                var bakery = Station(); bakery.BakeProgress += dt / 10;
                if (bakery.BakeProgress < 1) break;
                Food.UsedGrain += bakery.InputGrain; Food.BakedBread += bakery.InputGrain * 2;
                bakery.OutputBread += bakery.InputGrain * 2; bakery.InputGrain = 0; bakery.BakeProgress = 0;
                v.Task = Work.ToBread; break;
            case Work.ToBread:
                var shop = Station(); int bread = Math.Min(2, shop.OutputBread); shop.OutputBread -= bread;
                CarryFood(Resource.Bread, bread); break;
            case Work.ToPantry:
                switch (v.Cargo)
                {
                    case Resource.Vegetables: Food.Vegetables += v.Carried; break;
                    case Resource.Berries: Food.Berries += v.Carried; break;
                    case Resource.Grain: Food.Grain += v.Carried; break;
                    case Resource.Bread: Food.Bread += v.Carried; break;
                    default: throw new InvalidOperationException("Non-food cargo delivered to pantry");
                }
                v.Carried = 0; Finish(v); break;
            case Work.ToSupper: v.Task = Work.Supper; v.Status = "Sharing the first village supper"; break;
        }
    }
    private void AdvanceFoodTime(float dt)
    {
        Food.Time += dt;
        foreach (var bush in Bushes)
        {
            if (bush.Ripe == 8) { bush.Regrowth = 0; continue; }
            bush.Regrowth += dt;
            while (bush.Regrowth >= 8 && bush.Ripe < 8) { bush.Regrowth -= 8; bush.Ripe++; }
        }
        foreach (var farm in Cottages.Where(c => IsField(c) && c.Complete && c.Planted && c.Growth < 1))
        {
            farm.Growth = Math.Min(1, farm.Growth + dt / (farm.Kind == BuildingKind.VegetableGarden ? 60 : 45));
            if (farm.Growth == 1)
            {
                if (farm.Kind == BuildingKind.VegetableGarden) { farm.Harvest = 8; Food.GrownVegetables += 8; }
                else { farm.Harvest = 6; Food.GrownGrain += 6; }
            }
        }
        if (Food.Celebrating)
        {
            if (People.All(v => v.Task == Work.Supper)) Food.MeetingClock += dt;
            if (Food.MeetingClock >= 6)
            {
                Food.Celebrating = false; Food.SupperComplete = true; History.Add("The first village supper");
                foreach (var person in People) Finish(person);
            }
            return;
        }
        Food.MealClock += dt;
        while (Food.MealClock >= 60)
        {
            Food.LastMealChoices = (Food.Berries > 0 ? 1 : 0) + (Food.Vegetables > 0 ? 1 : 0) + (Food.Bread > 0 ? 1 : 0);
            Food.MealClock -= 60; int berries = Math.Min(Population, Food.Berries); Food.Berries -= berries; Food.EatenBerries += berries;
            int vegetables = Math.Min(Population - berries, Food.Vegetables); Food.Vegetables -= vegetables; Food.EatenVegetables += vegetables;
            int bread = Math.Min(Population - berries - vegetables, Food.Bread); Food.Bread -= bread; Food.EatenBread += bread;
            Food.Hunger = (Population - berries - vegetables - bread) / (float)Population;
        }
    }
    private List<Cell> SupperSpots()
    {
        var square = Cottages.FirstOrDefault(c => c.Kind == BuildingKind.Square && c.Complete);
        var center = square?.Entrance ?? YardAccess;
        var reachable = Reachable(YardAccess, Blocked);
        return reachable.Where(c => square == null || (c.Point - center.Point).LengthSquared() <= 16)
            .OrderBy(c => (c.Point - center.Point).LengthSquared()).ThenBy(c => c.Z).ThenBy(c => c.X).Take(Population).ToList();
    }
    public bool BeginSupper()
    {
        if (!CanCelebrate) return false;
        MeetingSpots.Clear();
        MeetingSpots.AddRange(SupperSpots());
        if (MeetingSpots.Count != Population) return false;
        Food.Bread -= SupperCost; Food.SupperBread += SupperCost;
        foreach (var v in People) Interrupt(v);
        Food.Celebrating = true; Food.MeetingClock = 0; _retry = 0; return true;
    }
    private void ValidateFood()
    {
        void Check(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }
        int Cargo(Resource resource) => People.Where(v => v.Cargo == resource).Sum(v => v.Carried);
        Check(Food.Berries >= 0 && Food.Grain >= ReservedGrain && Food.Bread >= 0, "Negative or over-reserved food");
        Check(Food.InitialBerries >= 0 && Food.Berries + Cargo(Resource.Berries) + Food.EatenBerries == Food.InitialBerries + Food.GatheredBerries, "Berry conservation failed");
        Check(Food.Grain + Cargo(Resource.Grain) + Cottages.Where(c => c.Kind == BuildingKind.Farm).Sum(c => c.Harvest) + Cottages.Sum(c => c.InputGrain) + Food.UsedGrain == Food.GrownGrain, "Grain conservation failed");
        Check(Food.Bread + Cargo(Resource.Bread) + Cottages.Sum(c => c.OutputBread) + Food.EatenBread + Food.SupperBread == Food.BakedBread, "Bread conservation failed");
        Check(Food.Vegetables >= 0 && Food.GrownVegetables >= 0 && Food.EatenVegetables >= 0 &&
            Food.Vegetables + Cargo(Resource.Vegetables) + Cottages.Where(c=>c.Kind==BuildingKind.VegetableGarden).Sum(c=>c.Harvest) + Food.EatenVegetables == Food.GrownVegetables, "Vegetable conservation failed");
        Check(Food.BakedBread == Food.UsedGrain * 2, "Recipe conversion failed");
        foreach (var bush in Bushes)
        {
            var owners = People.Where(v => v.BushId == bush.Id).ToArray();
            Check(bush.Ripe is >= 0 and <= 8 && owners.Length == (bush.Owner.HasValue ? 1 : 0) && (owners.Length == 0 || owners[0].Id == bush.Owner), "Berry source ownership mismatch");
        }
        foreach (var c in Cottages)
        {
            Check(People.Count(v => v.WorkplaceId == c.Id) <= (c.Kind == BuildingKind.ForagerHut ? 2 : 1), "Production capacity exceeded");
            Check(c.Harvest >= 0 && c.InputGrain is >= 0 and <= 2 && c.OutputBread is >= 0 and <= 4, "Invalid production buffer");
        }
        foreach (var v in People)
        {
            Check(v.WorkplaceId == null || Cottages.Any(c => c.Id == v.WorkplaceId && c.Complete), "Missing food workplace");
            Check(v.FoodReserved == 0 || (v.FoodReserved == 2 && v.Task == Work.ToGrain), "Orphaned grain claim");
        }
    }
}

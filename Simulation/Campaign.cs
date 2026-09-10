using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Inlanders.Simulation;

public sealed class CampaignState
{
    public RiverProgress? River { get; set; }
    public int Level { get; set; }
    public bool Complete { get; set; }
    public bool Guidance { get; set; } = true;
    public HashSet<string> Dismissed { get; set; } = new();
}

public enum CampaignGoalKind { Housing, ForagerHut, DeliveredBerries, Farm, Bakery, DeliveredBread, Sawmill, Lodge, TreesPlanted, Square, Supper, VegetableGarden, DeliveredVegetables, VegetableChoiceMeals }
public sealed record CampaignGoal(CampaignGoalKind Kind, string Label, int Target);
public sealed record CampaignLevel(int Id, string Title, string Arrival, params CampaignGoal[] Goals);
public sealed record CampaignHint(string Id, string Text);

public sealed partial class World
{
    public CampaignState? Campaign { get; private set; }
    public static readonly CampaignLevel[] CampaignLevels =
    {
        new(1, "A place to stay", "Eight arrivals need food and homes. Start with a forager hut, then cottages. All buildings and tools remain available.",
            new(CampaignGoalKind.ForagerHut, "Forager hut", 1), new(CampaignGoalKind.DeliveredBerries, "Fresh berries delivered", 24), new(CampaignGoalKind.Housing, "Neighbors housed", 8)),
        new(2, "Bread for the table", "The berry hamlet has homes. Add a farm and bakery to bring bread to the table.",
            new(CampaignGoalKind.Farm, "Farm", 1), new(CampaignGoalKind.Bakery, "Bakery", 1), new(CampaignGoalKind.DeliveredBread, "Loaves delivered", 16)),
        new(3, "Room among the trees", "Four neighbors still need beds. Turn timber into planks for a lodge, and plant the next generation of woodland.",
            new(CampaignGoalKind.Sawmill, "Sawmill", 1), new(CampaignGoalKind.Lodge, "Lodge", 1), new(CampaignGoalKind.Housing, "Neighbors housed", 8), new(CampaignGoalKind.TreesPlanted, "Trees planted by loggers", 4)),
        new(4, "A place for everyone", "Your homes, farm, and bakery are ready. Build a village square and set aside two loaves per person. Host supper from Goals and watch everyone gather.",
            new(CampaignGoalKind.Square, "Village square", 1), new(CampaignGoalKind.Housing, "Neighbors housed", 8), new(CampaignGoalKind.Supper, "Village supper shared", 1)),
        new(5, "More for the table", "The village has homes and berries. Add a vegetable garden, then serve two full meals with at least a quarter vegetables and a quarter other food. The gardener visit is optional.",
            new(CampaignGoalKind.VegetableGarden, "Vegetable garden", 1), new(CampaignGoalKind.DeliveredVegetables, "Vegetables delivered", 16), new(CampaignGoalKind.VegetableChoiceMeals, "Full meals: at least ¼ vegetables and ¼ other food", 2)),
        new(6, "Across the river", "The west bank is a home, but room and timber are limited. Choose a crossing, prepare homes and food for newcomers, and build a working village on both banks. Goals explains each expansion; all buildings remain available.")
    };
    public int DeliveredBerries => Food.Berries + Food.EatenBerries + Food.TradedBerries - Food.InitialBerries;
    public int DeliveredVegetables => Food.Vegetables + Food.EatenVegetables;
    public int DeliveredBread => Food.Bread + Food.EatenBread + Food.SupperBread;
    public bool HasBuilding(BuildingKind kind) => Cottages.Any(c => c.Kind == kind && c.Complete);
    public bool HasForagerHut => HasBuilding(BuildingKind.ForagerHut);
    private CampaignGoal[] ActiveGoals => Campaign == null ? Array.Empty<CampaignGoal>() : CampaignLevels[Campaign.Level - 1].Goals;
    private int GoalValue(CampaignGoalKind kind) => kind switch
    {
        CampaignGoalKind.Housing => Housed,
        CampaignGoalKind.DeliveredBerries => DeliveredBerries,
        CampaignGoalKind.DeliveredBread => DeliveredBread,
        CampaignGoalKind.DeliveredVegetables => DeliveredVegetables,
        CampaignGoalKind.VegetableChoiceMeals => Food.VegetableChoiceMeals,
        CampaignGoalKind.TreesPlanted => TreesPlanted,
        CampaignGoalKind.Supper => Food.SupperComplete ? 1 : 0,
        _ => HasBuilding(kind switch {
            CampaignGoalKind.VegetableGarden => BuildingKind.VegetableGarden,
            CampaignGoalKind.ForagerHut => BuildingKind.ForagerHut, CampaignGoalKind.Farm => BuildingKind.Farm,
            CampaignGoalKind.Bakery => BuildingKind.Bakery, CampaignGoalKind.Sawmill => BuildingKind.Sawmill,
            CampaignGoalKind.Lodge => BuildingKind.Lodge, CampaignGoalKind.Square => BuildingKind.Square,
            _ => throw new ArgumentOutOfRangeException(nameof(kind))
        }) ? 1 : 0
    };
    public double CampaignProgress => IsRiverCampaign ? RiverCompletion : ActiveGoals.Length == 0 ? 0 : ActiveGoals.Average(g => Math.Clamp(GoalValue(g.Kind) / (double)g.Target, 0, 1));
    public string CampaignObjective => IsRiverCampaign ? RiverObjective : string.Join("\n", ActiveGoals.Select(g => $"{g.Label}: {Math.Min(g.Target, GoalValue(g.Kind))} / {g.Target}")) +
        (Campaign?.Level is 1 or 2 or 5 ? "\nMeals never erase delivery progress." : Campaign?.Level == 4 && !Food.SupperComplete ? $"\nBread for supper: {Food.Bread} / {SupperCost}" : "");
    private void UpdateCampaign()
    {
        if (Campaign != null && CampaignProgress >= 1) Campaign.Complete = true;
    }
    public static World NewCampaign(int level)
    {
        if (!CampaignLevels.Any(l => l.Id == level)) throw new ArgumentOutOfRangeException(nameof(level));
        if(level==6) { var river=NewRiverSettlement(); river.Campaign=new() { Level=6, River=new() }; river.Validate(); return river; }
        var w = level >= 3 ? NewLargeMap(false, false) : new World();
        w.Campaign = new() { Level = level };
        w.Map.Name = CampaignLevels[level - 1].Title;
        w.Food.InitialBerries = w.Food.Berries = level == 5 ? 48 : 96;
        void Ready(Cell cell, BuildingKind kind)
        {
            var site = w.Place(cell, false, kind) ?? throw new InvalidOperationException("Invalid campaign starting site");
            site.Delivered = site.Required; site.Construction = 1; w.InitialLogs += site.Required;
        }
        if (level > 1)
        {
            Ready(new(0, 0), BuildingKind.ForagerHut);
            Ready(new(3, 0), BuildingKind.Cottage); Ready(new(6, 0), BuildingKind.Cottage);
            if (level is 2 or 4 or 5) { Ready(new(3, 6), BuildingKind.Cottage); Ready(new(-5, 6), BuildingKind.Cottage); }
        }
        if (level == 4) { Ready(new(3,-3), BuildingKind.Farm); Ready(new(6,-3), BuildingKind.Bakery); }
        var roles = new[] { Role.Logger, Role.Logger, Role.Builder, Role.Builder, Role.Forager, Role.Forager, Role.Unassigned, Role.Unassigned };
        if (level == 4) { roles[6] = Role.Farmer; roles[7] = Role.Baker; }
        if (level == 5) roles = new[] { Role.Logger, Role.Builder, Role.Builder, Role.Forager, Role.Forager, Role.Farmer, Role.Unassigned, Role.Unassigned };
        for (int i = 0; i < 8; i++) w.Assign(i, roles[i]);
        w.ReconcileHomes(); w.Validate(); return w;
    }
    public CampaignHint? CurrentCampaignHint()
    {
        if (Campaign == null || !Campaign.Guidance || Campaign.Complete) return null;
        if(IsRiverCampaign) return Campaign.Dismissed.Contains("river") ? null : new("river", RiverActionProblem() ?? "Use the phase button in Goals when you are ready. Meals share available food types; Economy shows recent deliveries and what was eaten.");
        var hints = new List<CampaignHint>();
        void Hint(string id, string text, bool when = true) { if (when) hints.Add(new(id, text)); }
        bool planned = Cottages.Any(c => !c.Complete);
        Hint("hunger", "Food ran short. Staff a finished forager hut in People. Nobody dies; regular meals restore work speed.", Food.Hunger > 0);
        Hint("welcome", "WASD moves the camera; the wheel zooms. Space pauses. Open Build [B] for a forager hut, then cottages. R rotates entrances.", Campaign.Level == 1 && Cottages.Count == 0);
        Hint("builders", "Plans need builders. Open People [V] and use + beside Builder.", planned && !People.Any(v => v.Role == Role.Builder));
        Hint("loggers", "Assign loggers in People. Timber must reach the yard or a stockpile before builders can collect it.", planned && Available == 0 && !People.Any(v => v.Role == Role.Logger));
        Hint("hut", "Build a forager hut to keep the village fed. Each hut supports two foragers.", !HasForagerHut);
        Hint("foragers", "Assign foragers in People to staff the hut. Berries count after arriving at storage.", HasForagerHut && !People.Any(v => v.Role == Role.Forager));
        if (Campaign.Level == 1)
        {
            Hint("construction", "Builders carry logs from storage to plans. Select a plan to change priority.", planned);
            Hint("homes", "Cottages house two, lodges four. Finish beds for all eight neighbors.", Housed < 8);
            Hint("deliveries", "Let foragers deliver 24 fresh berries. Bushes regrow; meals never undo progress.", DeliveredBerries < 24);
        }
        if (Campaign.Level is 2 or 4)
        {
            Hint("farm", "Place a farm and assign a farmer. Sowing starts a 45-second growing cycle; harvested grain travels to storage.", !HasBuilding(BuildingKind.Farm));
            Hint("farmer", "Assign a farmer in People. One worker tends each farm.", !People.Any(v => v.Role == Role.Farmer));
            Hint("bakery", "Build a bakery. A baker fetches 2 grain and makes 4 loaves; grain alone cannot feed people.", !HasBuilding(BuildingKind.Bakery));
            Hint("baker", "Assign a baker in People. Loaves count when delivered to storage.", !People.Any(v => v.Role == Role.Baker));
            Hint("bread", "Watch crops ripen, grain arrive, and the baker deliver loaves. Meals never erase the delivery milestone.", Campaign.Level == 2);
        }
        if (Campaign.Level == 3)
        {
            Hint("sawmill", "Build a sawmill. A sawyer turns 2 logs into 4 planks for lodges.", !HasBuilding(BuildingKind.Sawmill));
            Hint("sawyer", $"Assign a sawyer in People. New mills start with an adjustable {PlankStockTarget}-plank stock target.", !People.Any(v => v.Role == Role.Sawyer));
            Hint("lodge", $"Build a lodge for four neighbors. It costs {Buildings.Get(BuildingKind.Lodge).CostText}; you can place the plan before they arrive.", !HasBuilding(BuildingKind.Lodge));
            Hint("plant", "Choose Plant alders in Build and mark four spots. Loggers must actually plant them. Maturity takes 180 seconds but is not required for this lesson.", TreesPlanted < 4);
        }
        if (Campaign.Level == 4)
        {
            Hint("square", "Build a village square. Leave one walkable tile per villager within four tiles of its entrance for guests.", !HasBuilding(BuildingKind.Square));
            Hint("final-homes", "Finish housing for everyone before hosting supper.", Housed < Population);
            Hint("supper", $"Stock {SupperCost} loaves, then press Host supper in Goals. Leave {Population} clear nearby tiles. The meal finishes after everyone arrives.");
        }
        if (Campaign.Level == 5)
        {
            Hint("garden", $"Vegetable gardens produce food directly. Place one near the yard; each costs {Buildings.Get(BuildingKind.VegetableGarden).CostText}.", !Cottages.Any(c => c.Kind == BuildingKind.VegetableGarden));
            Hint("garden-build", "Builders supply and finish the garden. Keep its entrance connected to the yard.", !HasBuilding(BuildingKind.VegetableGarden));
            Hint("garden-farmer", "Farmers tend grain fields and vegetable gardens. Assign a farmer in People.", !People.Any(p => p.Role == Role.Farmer));
            Hint("garden-harvest", "Vegetables grow for one minute after sowing. Farmers carry the harvest to storage; only delivered vegetables count.", DeliveredVegetables == 0);
            Hint("garden-choice", $"Serve everyone, with at least {(Population + 3) / 4} vegetable portions and {(Population + 3) / 4} other food portions per meal. Meals share available types. Economy shows what was eaten; two qualifying meals are needed.");
        }
        return hints.FirstOrDefault(h => !Campaign.Dismissed.Contains(h.Id));
    }
}
// One resumable snapshot per settlement; completion survives replay and loading an older save.
public sealed class CampaignBook
{
    public int Version { get; set; } = 2;
    public int ActiveLevel { get; set; }
    public Dictionary<int, string> Settlements { get; set; } = new();
    public Dictionary<int, string> BeforeReplay { get; set; } = new();
    public HashSet<int> Completed { get; set; } = new();
    public void Capture(World world)
    {
        if (world.Creative) throw new InvalidOperationException("Creative settlements use separate saves.");
        ActiveLevel = world.Campaign?.Level ?? 0;
        Settlements[ActiveLevel] = world.SaveJson();
        if (world.Campaign?.Complete == true) Completed.Add(ActiveLevel);
    }
    public void SaveFile(string path)
    {
        string full = Path.GetFullPath(path); Directory.CreateDirectory(Path.GetDirectoryName(full)!);
        File.WriteAllText(full + ".tmp", JsonSerializer.Serialize(this));
        if (File.Exists(full)) File.Replace(full + ".tmp", full, full + ".bak");
        else File.Move(full + ".tmp", full);
    }
    public static CampaignBook LoadFile(string path)
    {
        var book = JsonSerializer.Deserialize<CampaignBook>(File.ReadAllText(path)) ?? throw new InvalidDataException("Empty campaign");
        if (book.Version != 2 || book.Settlements == null || book.BeforeReplay == null || book.Completed == null || (book.ActiveLevel < 0 || book.ActiveLevel > World.CampaignLevels.Length) ||
            !book.Settlements.ContainsKey(book.ActiveLevel) || book.Completed.Any(i => i < 1 || i > World.CampaignLevels.Length)) throw new InvalidDataException("Invalid campaign progress");
        foreach (var (id, json) in book.Settlements.Concat(book.BeforeReplay))
            if ((id < 0 || id > World.CampaignLevels.Length) || (World.LoadJson(json).Campaign?.Level ?? 0) != id) throw new InvalidDataException("Campaign snapshot does not match level");
        return book;
    }
}

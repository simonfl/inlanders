using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Inlanders.Simulation;

public sealed class CampaignState
{
    public int Level { get; set; }
    public bool Complete { get; set; }
    public bool Guidance { get; set; } = true;
    public HashSet<string> Dismissed { get; set; } = new();
}

public enum CampaignGoalKind { Housing, ForagerHut, DeliveredBerries }
public sealed record CampaignGoal(CampaignGoalKind Kind, string Label, int Target);
public sealed record CampaignLevel(int Id, string Title, string Arrival, params CampaignGoal[] Goals);
public sealed record CampaignHint(string Id, string Text);

public sealed partial class World
{
    public CampaignState? Campaign { get; private set; }
    public static readonly CampaignLevel[] CampaignLevels =
    {
        new(1, "A place to stay", "The berry camp is running. Give eight arrivals a home among the trees. All buildings are available; four cottages are a good start.", new CampaignGoal(CampaignGoalKind.Housing, "Completed housing", 8)),
        new(2, "The berry clearing", "This hamlet has homes, but needs a dependable food supply. Build a forager hut and bring fresh berries back to storage.",
            new CampaignGoal(CampaignGoalKind.ForagerHut, "Completed forager hut", 1), new CampaignGoal(CampaignGoalKind.DeliveredBerries, "Fresh berries delivered", 24))
    };
    public int DeliveredBerries => Food.Berries + Food.EatenBerries - Food.InitialBerries;
    public bool HasForagerHut => Cottages.Any(c => c.Kind == BuildingKind.ForagerHut && c.Complete);
    private CampaignGoal[] ActiveGoals => Campaign == null ? Array.Empty<CampaignGoal>() : CampaignLevels[Campaign.Level - 1].Goals;
    private int GoalValue(CampaignGoalKind kind) => kind switch
    {
        CampaignGoalKind.Housing => Housed, CampaignGoalKind.ForagerHut => HasForagerHut ? 1 : 0,
        CampaignGoalKind.DeliveredBerries => DeliveredBerries, _ => throw new ArgumentOutOfRangeException(nameof(kind))
    };
    public double CampaignProgress => ActiveGoals.Length == 0 ? 0 : ActiveGoals.Average(g => Math.Clamp(GoalValue(g.Kind) / (double)g.Target, 0, 1));
    public string CampaignObjective => string.Join("\n", ActiveGoals.Select(g => $"{g.Label}: {Math.Min(g.Target, GoalValue(g.Kind))} / {g.Target}")) +
        (Campaign?.Level == 2 ? "\nMeals never erase delivery progress." : "");
    private void UpdateCampaign()
    {
        if (Campaign != null && CampaignProgress >= 1) Campaign.Complete = true;
    }
    public static World NewCampaign(int level)
    {
        if (!CampaignLevels.Any(l => l.Id == level)) throw new ArgumentOutOfRangeException(nameof(level));
        var w = new World { Campaign = new() { Level = level } };
        w.Food.InitialBerries = w.Food.Berries = 64;
        // Starting structures are paid for in the conservation ledger, separate from harvestable timber.
        void Ready(Cell cell, BuildingKind kind)
        {
            var site = w.Place(cell, false, kind) ?? throw new InvalidOperationException("Invalid campaign starting site");
            site.Delivered = site.Required; site.Construction = 1; w.InitialLogs += site.Required;
        }
        if (level == 1) Ready(new(0, 0), BuildingKind.ForagerHut);
        else
        {
            foreach (var cell in new[] { new Cell(3, 0), new(6, 0), new(3, 6), new(-5, 6) }) Ready(cell, BuildingKind.Cottage);
            // Distinct berry clearing, with three patches at different distances from the yard.
            w.Bushes.Clear();
            foreach (var cell in new[] { new Cell(-7, 2), new(-8, 5), new(1, -3) }) w.Bushes.Add(new() { Id = w.Bushes.Count, Cell = cell });
        }
        var roles = new[] { Role.Logger, Role.Logger, Role.Builder, Role.Builder, level == 1 ? Role.Forager : Role.Unassigned,
            level == 1 ? Role.Forager : Role.Unassigned, Role.Unassigned, Role.Unassigned };
        for (int i = 0; i < 8; i++) w.Assign(i, roles[i]);
        w.Validate(); return w;
    }
    public CampaignHint? CurrentCampaignHint()
    {
        if (Campaign == null || !Campaign.Guidance || Campaign.Complete) return null;
        var hints = new List<CampaignHint>();
        bool planned = Cottages.Any(c => !c.Complete);
        if (Food.Hunger > 0) hints.Add(new("hunger", "Food ran short. Finish a forager hut and assign foragers in People. Nobody dies; regular meals restore work speed."));
        if (Campaign.Level == 1)
        {
            if (!planned && Housed == 0) hints.Add(new("welcome", "WASD moves the camera; the wheel zooms. Space pauses. Open Build [B] and choose a cottage. R rotates its entrance. The staffed berry camp supplies food."));
            if (planned && !People.Any(v => v.Role == Role.Builder)) hints.Add(new("builders", "Plans need builders. Open People [V] and use + beside Builder to assign someone."));
            if (planned && Available == 0 && !People.Any(v => v.Role == Role.Logger)) hints.Add(new("loggers", "Builders are waiting for timber. Assign loggers in People; logs must reach the yard before builders can collect them."));
            if (planned) hints.Add(new("construction", "Watch logs travel from trees to the yard, then to your plan. Select a villager to see their task. Select a plan to change construction priority."));
            if (Housed > 0) hints.Add(new("homes", "Your first home is ready. Keep building until all eight have beds. Cottages house two; lodges house four. Choose any layout with clear entrances."));
        }
        else
        {
            if (!HasForagerHut) hints.Add(new("hut", "Open Build [B] and place a forager hut. Loggers supply timber and builders construct it. Nearby berry patches mean shorter trips."));
            else if (!People.Any(v => v.Role == Role.Forager)) hints.Add(new("foragers", "The hut is ready, but it needs staff. Open People [V] and press + beside Forager. One hut supports two active foragers."));
            else hints.Add(new("deliveries", "Foragers pick berries and carry them to storage. Only delivered berries count. Bushes regrow, so an idle forager may simply be waiting."));
            if (Food.Time >= 60) hints.Add(new("meals", "Eight villagers eat eight food each day, using berries before bread. Eating these berries does not undo your delivery goal."));
        }
        return hints.FirstOrDefault(h => !Campaign.Dismissed.Contains(h.Id));
    }
}

// One resumable snapshot per settlement; completion survives replay and loading an older save.
public sealed class CampaignBook
{
    public int Version { get; set; } = 1;
    public int ActiveLevel { get; set; }
    public Dictionary<int, string> Settlements { get; set; } = new();
    public Dictionary<int, string> BeforeReplay { get; set; } = new();
    public HashSet<int> Completed { get; set; } = new();
    public void Capture(World world)
    {
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
        if (book.Version != 1 || book.Settlements == null || book.BeforeReplay == null || book.Completed == null || book.ActiveLevel is < 0 or > 2 ||
            !book.Settlements.ContainsKey(book.ActiveLevel) || book.Completed.Any(i => i is < 1 or > 2)) throw new InvalidDataException("Invalid campaign progress");
        foreach (var (id, json) in book.Settlements.Concat(book.BeforeReplay))
            if (id is < 0 or > 2 || (World.LoadJson(json).Campaign?.Level ?? 0) != id) throw new InvalidDataException("Campaign snapshot does not match level");
        return book;
    }
}

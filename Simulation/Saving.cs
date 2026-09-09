using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Inlanders.Simulation;

public sealed class WorldSave
{
    public int Version { get; set; } = 8;
    public HashSet<Cell> Paths { get; set; } = new();
    public MapLayout? Map { get; set; }
    public CampaignState? Campaign { get; set; }
    public int TreesPlanted { get; set; }
    public int Planks { get; set; }
    public int SawnLogs { get; set; }
    public int GrownLogs { get; set; }
    public int InitialLogs { get; set; }
    public int Stored { get; set; }
    public int NextSite { get; set; }
    public int NextTree { get; set; }
    public float Retry { get; set; }
    public List<Villager> People { get; set; } = new();
    public List<TimberTree> Trees { get; set; } = new();
    public List<Cottage> Buildings { get; set; } = new();
    public List<BerryBush> Bushes { get; set; } = new();
    public FoodState Food { get; set; } = new();
    public List<Cell> MeetingSpots { get; set; } = new();
    public List<string> History { get; set; } = new();
}
public sealed partial class World
{
    private static readonly JsonSerializerOptions SaveOptions = new() { IncludeFields = true, IgnoreReadOnlyProperties = true, WriteIndented = true };
    public string SaveJson()
    {
        Validate();
        return JsonSerializer.Serialize(new WorldSave
        {
            TreesPlanted = TreesPlanted, Paths = Paths, Map = Map, Campaign = Campaign, InitialLogs = InitialLogs, GrownLogs = GrownLogs, Stored = Stored, Planks = Planks, SawnLogs = SawnLogs, NextSite = _nextSite, NextTree = _nextTree, Retry = _retry,
            People = People, Trees = Trees, Buildings = Cottages, Bushes = Bushes, Food = Food, MeetingSpots = MeetingSpots, History = History
        }, SaveOptions);
    }
    public static World LoadJson(string json)
    {
        var s = JsonSerializer.Deserialize<WorldSave>(json, SaveOptions) ?? throw new InvalidDataException("Empty save file");
        if (s.Version is not (1 or 2 or 3 or 4 or 5 or 6 or 7 or 8)) throw new InvalidDataException($"Unsupported save version {s.Version}");
        if (s.Version >= 5 && s.Map == null) throw new InvalidDataException("Save is missing map layout");
        var map = s.Map ?? new MapLayout(); map.Validate();
        if (s.Campaign != null && (s.Campaign.Level is < 1 or > 4 || s.Campaign.Dismissed == null)) throw new InvalidDataException("Invalid campaign state");
        if (s.People == null || s.People.Count != 8 || !s.People.Select(v => v.Id).SequenceEqual(Enumerable.Range(0,8)) ||
            s.Trees == null || s.Buildings == null || s.Bushes == null || s.Bushes.Count == 0 || s.Food == null || s.MeetingSpots == null || s.History == null)
            throw new InvalidDataException("Save is missing settlement data");
        if (s.Bushes.Select(b => b.Id).Distinct().Count() != s.Bushes.Count || s.Trees.Select(t => t.Id).Distinct().Count() != s.Trees.Count || s.Buildings.Select(c => c.Id).Distinct().Count() != s.Buildings.Count ||
            s.NextSite <= s.Buildings.Select(c => c.Id).DefaultIfEmpty(0).Max() || s.NextTree <= s.Trees.Select(t => t.Id).DefaultIfEmpty(-1).Max())
            throw new InvalidDataException("Invalid entity identifiers");
        bool Finite(float n) => float.IsFinite(n) && n >= 0;
        if (!Finite(s.Food.Time) || !Finite(s.Food.MealClock) || s.Food.MealClock >= 60 || !Finite(s.Food.MeetingClock) ||
            !Finite(s.Food.Hunger) || s.Food.Hunger > 1 || !float.IsFinite(s.Retry) || (s.Food.Celebrating && s.MeetingSpots.Count != 8))
            throw new InvalidDataException("Invalid clock or celebration state");
        foreach (var p in s.People)
            if (p.Route == null || p.Name == null || !float.IsFinite(p.Position.X) || !float.IsFinite(p.Position.Y) || !Finite(p.Timer) ||
                !Enum.IsDefined(p.Role) || !Enum.IsDefined(p.Task) || !Enum.IsDefined(p.Cargo)) throw new InvalidDataException("Invalid worker state");
        foreach (var b in s.Buildings)
            if (!Enum.IsDefined(b.Kind) || !Finite(b.Construction) || b.Construction > 1 || !Finite(b.Growth) || b.Growth > 1 ||
                !Finite(b.BakeProgress) || b.BakeProgress > 1 || b.Priority is < 0 or > 2) throw new InvalidDataException("Invalid building state");
        var w = new World(0) { TreesPlanted = s.TreesPlanted, InitialLogs = s.InitialLogs, GrownLogs = s.GrownLogs, Stored = s.Stored, Planks = s.Planks, SawnLogs = s.SawnLogs, _nextSite = s.NextSite, _nextTree = s.NextTree, _retry = s.Retry, Food = s.Food };
        w.Campaign = s.Campaign; w.Map = map;
        w.People.Clear(); w.People.AddRange(s.People); w.Trees.Clear(); w.Trees.AddRange(s.Trees);
        w.Cottages.AddRange(s.Buildings); w.Bushes.Clear(); w.Bushes.AddRange(s.Bushes);
        w.MeetingSpots.AddRange(s.MeetingSpots); w.History.AddRange(s.History);
        if (s.TreesPlanted < 0) throw new InvalidDataException("Invalid planting count");
        if (s.Paths == null || s.Paths.Any(c => w.Blocked(c) || map.Water.Contains(c))) throw new InvalidDataException("Invalid path tiles");
        w.Paths = s.Paths;
        w.Validate(); w.ValidateMapOccupancy(); return w;
    }
    public void SaveFile(string path)
    {
        string json = SaveJson(); var full = Path.GetFullPath(path);
        Directory.CreateDirectory(Path.GetDirectoryName(full)!);
        File.WriteAllText(full + ".tmp", json);
        if (File.Exists(full)) File.Replace(full + ".tmp", full, full + ".bak");
        else File.Move(full + ".tmp", full);
    }
    public static World LoadFile(string path) => LoadJson(File.ReadAllText(path));
}

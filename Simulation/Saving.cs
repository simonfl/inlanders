using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Inlanders.Simulation;

public sealed class WorldSave
{
    public int Version { get; set; } = 1;
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
            InitialLogs = InitialLogs, Stored = Stored, NextSite = _nextSite, NextTree = _nextTree, Retry = _retry,
            People = People, Trees = Trees, Buildings = Cottages, Bushes = Bushes, Food = Food, MeetingSpots = MeetingSpots, History = History
        }, SaveOptions);
    }
    public static World LoadJson(string json)
    {
        var s = JsonSerializer.Deserialize<WorldSave>(json, SaveOptions) ?? throw new InvalidDataException("Empty save file");
        if (s.Version != 1) throw new InvalidDataException($"Unsupported save version {s.Version}");
        if (s.People == null || s.People.Count != 8 || !s.People.Select(v => v.Id).SequenceEqual(Enumerable.Range(0,8)) ||
            s.Trees == null || s.Buildings == null || s.Bushes == null || s.Bushes.Count != 3 || s.Food == null || s.MeetingSpots == null || s.History == null)
            throw new InvalidDataException("Save is missing settlement data");
        if (s.Trees.Select(t => t.Id).Distinct().Count() != s.Trees.Count || s.Buildings.Select(c => c.Id).Distinct().Count() != s.Buildings.Count ||
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
        var w = new World(0) { InitialLogs = s.InitialLogs, Stored = s.Stored, _nextSite = s.NextSite, _nextTree = s.NextTree, _retry = s.Retry, Food = s.Food };
        w.People.Clear(); w.People.AddRange(s.People); w.Trees.Clear(); w.Trees.AddRange(s.Trees);
        w.Cottages.AddRange(s.Buildings); w.Bushes.Clear(); w.Bushes.AddRange(s.Bushes);
        w.MeetingSpots.AddRange(s.MeetingSpots); w.History.AddRange(s.History);
        w.Validate(); return w;
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

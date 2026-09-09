using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Inlanders.Simulation;

public sealed class MapLayout
{
    public string Name { get; set; } = "Original clearing";
    public int MinX { get; set; } = -8;
    public int MinZ { get; set; } = -7;
    public int Width { get; set; } = 17;
    public int Depth { get; set; } = 15;
    public HashSet<Cell> Water { get; set; } = new();
    public HashSet<Cell> Excluded { get; set; } = new();
    public int MaxX => MinX + Width - 1;
    public int MaxZ => MinZ + Depth - 1;
    public bool Contains(Cell c) => c.X >= MinX && c.X <= MaxX && c.Z >= MinZ && c.Z <= MaxZ && !Excluded.Contains(c);
    public IEnumerable<Cell> Land
    {
        get { for (int x = MinX; x <= MaxX; x++) for (int z = MinZ; z <= MaxZ; z++) { var c = new Cell(x, z); if (Contains(c) && !Water.Contains(c)) yield return c; } }
    }
    public bool OriginalOutline => MinX == -8 && MinZ == -7 && Width == 17 && Depth == 15 && Excluded.Count == 0 && Water.Count == 0;
    public void Validate()
    {
        if (Name == null || Width is < 4 or > 128 || Depth is < 4 or > 128 || MinX is < -128 or > 128 || MinZ is < -128 or > 128 ||
            Water == null || Excluded == null || Water.Any(c => !Contains(c)) || Excluded.Any(c => c.X < MinX || c.X > MaxX || c.Z < MinZ || c.Z > MaxZ) || Excluded.Count >= Width * Depth)
            throw new InvalidDataException("Invalid map layout");
    }
}

public sealed partial class World
{
    public MapLayout Map { get; private set; } = new();
    public static World NewLargeMap(bool withWater = true)
    {
        var w = NewScenario();
        w.Map = new() { Name = "Three clearings", MinX = -16, MinZ = -16, Width = 32, Depth = 32 };
        // An authored, connected outline: broad center, tapered corners and two shallow inlets.
        for (int x = -16; x <= 15; x++) for (int z = -16; z <= 15; z++)
            if (Math.Abs(x) + Math.Abs(z) > 25 || (x < -12 && z > 5 && z < 11) || (x > 11 && z < -4 && z > -10)) w.Map.Excluded.Add(new(x, z));
        foreach (var cell in new[] { new Cell(-12, -8), new(-9, -10), new(-6, -12), new(-2, -12), new(4, -12), new(8, -11),
            new(11, -2), new(12, 2), new(11, 6), new(8, 10), new(4, 12), new(-1, 12), new(-8, 11), new(-11, 3) })
            w.Trees.Add(new() { Id = w._nextTree++, Cell = cell, Logs = 8 });
        if (withWater) for (int z = -9; z <= 9; z++) w.Map.Water.Add(new(7, z));
        w.InitialLogs = w.Trees.Sum(t => t.Logs);
        foreach (var cell in new[] { new Cell(-10, -6), new(8, 6), new(-5, 10) }) w.Bushes.Add(new() { Id = w.Bushes.Count, Cell = cell });
        w.Food.InitialBerries = w.Food.Berries = 64;
        w.Validate(); w.ValidateMapOccupancy(); return w;
    }
    private HashSet<Cell> Reachable(Cell start, Func<Cell, bool> blocked)
    {
        var reached = new HashSet<Cell>(); var pending = new Queue<Cell>();
        if (blocked(start)) return reached;
        reached.Add(start); pending.Enqueue(start);
        while (pending.TryDequeue(out var c))
            foreach (var n in new[] { new Cell(c.X + 1, c.Z), new(c.X - 1, c.Z), new(c.X, c.Z + 1), new(c.X, c.Z - 1) })
                if (!reached.Contains(n) && !blocked(n)) { reached.Add(n); pending.Enqueue(n); }
        return reached;
    }
    private void ValidateMapOccupancy()
    {
        Map.Validate();
        var occupied = new HashSet<Cell> { Stockpile };
        foreach (var cell in Trees.Select(t => t.Cell).Concat(Bushes.Select(b => b.Cell)))
            if (!Map.Contains(cell) || Map.Water.Contains(cell) || !occupied.Add(cell)) throw new InvalidDataException("Invalid resource terrain");
        foreach (var site in Cottages)
        {
            foreach (var cell in Footprint(site.Cell, site.Rotated, site.Kind))
                if (!Map.Contains(cell) || Map.Water.Contains(cell) != (site.Kind == BuildingKind.Bridge) || !occupied.Add(cell))
                    throw new InvalidDataException("Invalid building terrain");
            if (site.Kind == BuildingKind.Bridge && new[] { Door(site.Cell, site.Rotated), FarBank(site.Cell, site.Rotated) }.Any(c => !Map.Contains(c) || Map.Water.Contains(c) || Blocked(c)))
                throw new InvalidDataException("Bridge needs clear dry banks");
        }
        if (!Map.Contains(Stockpile) || Map.Water.Contains(Stockpile)) throw new InvalidDataException("Yard outside dry land");
        var reached = Reachable(YardAccess, Blocked);
        // Resources may wait on the far bank; existing villagers and building entrances must remain usable.
        if (Trees.Select(t => t.Access).Concat(Bushes.Select(b => b.Access)).Any(c => Blocked(c) || (Map.Water.Count == 0 && !reached.Contains(c))) ||
            Cottages.Select(c => c.Entrance).Concat(People.Select(At)).Concat(MeetingSpots).Any(c => !reached.Contains(c)))
            throw new InvalidDataException("Map cuts off village access");
    }
}

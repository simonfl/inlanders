using System;
using System.Linq;

namespace Inlanders.Simulation;

public sealed partial class World
{
    public const float TreeGrowthSeconds = 180;
    public const int TreeYield = 8;
    public int GrownLogs { get; private set; }

    public bool CanPlantTree(Cell cell)
    {
        if (Food.Celebrating || !Inside(cell)) return false;
        var existing = Trees.FirstOrDefault(t => t.Cell == cell);
        if (existing != null && (existing.Salvage || !existing.Felled || existing.Logs != 0 || existing.Owner != null)) return false;
        if (existing == null && Blocked(cell)) return false;
        var access = Trees.Select(t => t.Access).Concat(Bushes.Select(b => b.Access))
            .Concat(Cottages.Select(c => c.Entrance)).Append(YardAccess).Append(new Cell(cell.X + 1, cell.Z)).ToArray();
        if (access.Contains(cell) || People.Any(v => At(v) == cell || (v.Route.TryPeek(out var next) && next == cell))) return false;
        bool Obstacle(Cell c) => c == cell || Blocked(c);
        return access.Concat(People.Select(At)).All(c => FindPath(YardAccess, c, Obstacle) != null);
    }

    public TimberTree? PlantTree(Cell cell)
    {
        if (!CanPlantTree(cell)) return null;
        var tree = Trees.FirstOrDefault(t => t.Cell == cell);
        if (tree == null) { tree = new TimberTree { Id = _nextTree++, Cell = cell }; Trees.Add(tree); }
        tree.Felled = false; tree.Growth = 0; tree.NeedsPlanting = true;
        foreach (var v in People.Where(v => v.Route.Count > 0)) SetRoute(v, v.Destination);
        _retry = 0;
        return tree;
    }

    private void AdvanceWoodland(float dt)
    {
        foreach (var tree in Trees.Where(t => !t.NeedsPlanting && t.Growth < 1))
        {
            tree.Growth = Math.Min(1, tree.Growth + dt / TreeGrowthSeconds);
            if (tree.Growth < 1) continue;
            tree.Logs = TreeYield; GrownLogs += TreeYield; _retry = 0;
        }
    }
}

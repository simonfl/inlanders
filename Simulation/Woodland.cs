using System;
using System.Linq;

namespace Inlanders.Simulation;

public sealed partial class World
{
    public const float TreeGrowthSeconds = 180;
    public const int TreeYield = 8;
    public int GrownLogs { get; private set; }

    public bool CanPlantTree(Cell cell) => PlantingProblem(cell) == null;

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
        foreach (var tree in Trees.Where(t => !t.ClearRequested && !t.NeedsPlanting && t.Growth < 1))
        {
            tree.Growth = Math.Min(1, tree.Growth + dt / TreeGrowthSeconds);
            if (tree.Growth < 1) continue;
            tree.Logs = TreeYield; GrownLogs += TreeYield; _retry = 0;
        }
    }
}

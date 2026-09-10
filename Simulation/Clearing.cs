using System.Linq;
using System.Numerics;

namespace Inlanders.Simulation;

public sealed partial class World
{
    public string? ClearingProblem(Cell cell)
    {
        if (Food.Celebrating) return "Wait until the village supper is over.";
        if (!Map.Contains(cell)) return "Choose a tree or stump inside the map.";
        var tree = Trees.FirstOrDefault(t => t.Cell == cell);
        if (tree == null) return "Choose a tree, sapling, planting marker, or exhausted stump.";
        if (tree.Salvage) return "Loggers already collect salvage piles; these do not need clearing orders.";
        return null;
    }
    public bool SetClearing(Cell cell, bool requested)
    {
        if (ClearingProblem(cell) != null) return false;
        var tree = Trees.Single(t => t.Cell == cell);
        if (Creative && requested) return ClearImmediately(tree);
        if (tree.ClearRequested == requested) return true;
        // Stop conflicting planting/root work. Harvesting already in progress finishes normally.
        if (tree.Owner is int owner && People[owner].Task is Work.ToSapling or Work.PlantingTree or Work.ToClearStump or Work.ClearingStump)
            Interrupt(People[owner]);
        tree.ClearRequested = requested; _retry = 0;
        return true;
    }
    private bool ClaimClearing(Villager v)
    {
        var tree = Trees.Where(t => t.ClearRequested && t.Owner == null && Accessible(t.Access))
            .OrderBy(t => Vector2.DistanceSquared(v.Position, t.Access.Point)).ThenBy(t => t.Id).FirstOrDefault();
        if (tree == null) return false;
        tree.Owner = v.Id; v.TreeId = tree.Id;
        Go(v, tree.Access, tree.Logs > 0 ? Work.ToTree : Work.ToClearStump,
            tree.Logs > 0 ? "Collecting timber from a clearing order" : "Walking to clear roots");
        return true;
    }
}

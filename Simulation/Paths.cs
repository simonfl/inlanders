using System.Collections.Generic;
using System.Linq;

namespace Inlanders.Simulation;

public sealed partial class World
{
    public HashSet<Cell> Paths { get; private set; } = new();
    public int PathsRevision { get; private set; }
    public string? PathProblem(Cell cell, bool remove = false) => Food.Celebrating ? "Wait until supper is over." :
        remove ? (Paths.Contains(cell) ? null : "There is no path here to remove.") : Blocked(cell) ? "Paint paths on clear land, including entrances and access points." : null;
    public bool SetPath(Cell cell, bool present)
    {
        if (PathProblem(cell, !present) != null) return false;
        bool changed = present ? Paths.Add(cell) : Paths.Remove(cell);
        if (!changed) return true;
        PathsRevision++;
        foreach (var v in People.Where(v => v.Route.Count > 0)) SetRoute(v, v.Destination);
        return true;
    }
    private void RemovePaths(IEnumerable<Cell> cells)
    {
        foreach (var cell in cells) if (Paths.Remove(cell)) PathsRevision++;
    }
}

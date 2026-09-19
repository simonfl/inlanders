using System.Collections.Generic;
using System.Linq;

namespace Inlanders.Simulation;

public sealed partial class World
{
    public HashSet<Cell> Paths { get; private set; } = new();
    public int PathsRevision { get; private set; }
    public string? PathProblem(Cell cell, bool remove = false) => Food.Celebrating ? "Wait until supper is over." :
        remove ? (Paths.Contains(cell) ? null : "There is no path here to remove.") : Map.Water.Contains(cell) ? "Paths need dry land; bridges already provide a crossing." : Blocked(cell) ? "Paint paths on clear land, including entrances and access points." : null;
    public bool SetPath(Cell cell, bool present)
    {
        if (PathProblem(cell, !present) != null) return false;
        if(present) ManagedWoodland.Remove(cell);
        bool changed = present ? Paths.Add(cell) : Paths.Remove(cell);
        if (!changed) return true;
        PathsRevision++;
        foreach (var v in People.Where(v => v.Route.Count > 0)) SetRoute(v, v.Destination);
        return true;
    }
    public string? PathConnection(Cell start, Cell end, out List<Cell> route)
    {
        route = new();
        var problem = PathProblem(start) ?? PathProblem(end);
        if (problem != null) return problem;
        var found = FindPath(start, end, Blocked);
        if (found == null) return "No walking route connects these places. Add a bridge or clear an approach first.";
        route.Add(start); route.AddRange(found);
        return null;
    }
    public bool ConnectPaths(Cell start, Cell end)
    {
        if (PathConnection(start, end, out var route) != null) return false;
        bool changed = false;
        foreach (var cell in route)
        {
            if (Map.Water.Contains(cell)) continue; // Existing bridges already carry walkers.
            ManagedWoodland.Remove(cell);
            changed |= Paths.Add(cell);
        }
        if (changed)
        {
            PathsRevision++;
            foreach (var v in People.Where(v => v.Route.Count > 0)) SetRoute(v, v.Destination);
        }
        return true;
    }
    private void RemovePaths(IEnumerable<Cell> cells)
    {
        foreach (var cell in cells) if (Paths.Remove(cell)) PathsRevision++;
    }
}

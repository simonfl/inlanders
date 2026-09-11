using System.Collections.Generic;
using System.Linq;

namespace Inlanders.Simulation;

public sealed partial class World
{
    // Planned crossings block navigation too: a trip must not depend on rowing
    // through a bridge which can finish while the boat is away.
    private bool BoatBlocked(Cell cell) => !Map.Contains(cell) || !Map.Water.Contains(cell) ||
        Cottages.Any(c => c.Kind == BuildingKind.Bridge && c.Cell == cell);

    public List<Cell>? FindBoatRoute(Cell start, Cell destination)
    {
        if (BoatBlocked(start) || BoatBlocked(destination)) return null;
        var pending = new Queue<Cell>(); pending.Enqueue(start);
        var previous = new Dictionary<Cell, Cell> { [start] = start };
        while (pending.TryDequeue(out var cell))
        {
            if (cell == destination)
            {
                var route = new List<Cell>();
                while (cell != start) { route.Add(cell); cell = previous[cell]; }
                route.Reverse(); return route;
            }
            foreach (var next in new[] { new Cell(cell.X+1,cell.Z), new(cell.X-1,cell.Z), new(cell.X,cell.Z+1), new(cell.X,cell.Z-1) })
                if (!BoatBlocked(next) && previous.TryAdd(next, cell)) pending.Enqueue(next);
        }
        return null;
    }

    public Cell DockLaunch(Cell cell, int rotated) => Map.Water.Contains(FarBank(cell, rotated))
        ? FarBank(cell, rotated) : Door(cell, rotated);

    public Cell DockEntrance(Cell cell, int rotated) => DockLaunch(cell, rotated) == FarBank(cell, rotated)
        ? Door(cell, rotated) : FarBank(cell, rotated);

    public string? DockProblem(Cell cell, int rotated)
    {
        var launch = DockLaunch(cell, rotated);
        var entrance = DockEntrance(cell, rotated);
        if (!Map.Contains(cell) || Map.Water.Contains(cell) || BoatBlocked(launch))
            return "Choose dry shore with open water beside it. Turn the dock to face the water.";
        if (!Map.Contains(entrance) || Map.Water.Contains(entrance))
            return "The dock needs a dry entrance opposite its launch.";
        if (!Map.LevelGround(new[] { cell, entrance, launch }))
            return "Choose a level shoreline for the dock and launch.";
        if (Cottages.Any(c => c.Kind == BuildingKind.FishingDock && c.Launch == launch))
            return "Another dock needs this launch. Choose a different landing.";
        var problem = CheckPlacement(new HashSet<Cell> { cell }, entrance);
        if (problem != null) return problem;
        if (!Map.FishingGrounds.Any(h => FindBoatRoute(launch, h.Cell) != null))
            return "No fishing ground is reachable from this launch.";
        return null;
    }
    public string FishingSurvey(Cell cell,int rotated)
    {
        var launch=DockLaunch(cell,rotated);
        var grounds=Map.FishingGrounds.Select(g=>(Ground:g,Route:FindBoatRoute(launch,g.Cell))).Where(t=>t.Route!=null).ToArray();
        return string.Join("\n",grounds.Select(t=>$"{t.Ground.Name}: {AvailableFish(t.Ground)} available, +{t.Ground.RegrowthPerSecond*60:0.#}/min; {t.Route!.Count} water tiles away."));
    }
}

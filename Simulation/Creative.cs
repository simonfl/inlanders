using System.Linq;

namespace Inlanders.Simulation;

public sealed partial class World
{
    public bool Creative { get; private set; }

    public static World NewCreative(bool large = false)
    {
        var world = large ? NewLargeMap() : NewScenario();
        world.Creative = true;
        world.History.Add("Creative mode: instant free buildings and no food needs.");
        return world;
    }

    public string? RemovalProblem(int id)
    {
        var site = Cottages.FirstOrDefault(c => c.Id == id);
        if (site == null || !site.Complete) return "Choose a completed building.";
        if (Food.Celebrating) return "Wait until supper finishes.";
        if(site.Kind==BuildingKind.FishingDock && site.Boat?.FisherId!=null) return "Pause the dock and wait for its fisher to return before removing it.";
        if (site.Kind != BuildingKind.Bridge) return null;
        var before = Reachable(YardAccess, Blocked);
        var after = Reachable(YardAccess, c => c == site.Cell || Blocked(c) || Cottages.Any(b => b.Kind == BuildingKind.Bridge && b.DemolitionRequested && b.Cell == c));
        var access = Cottages.Where(c => c != site).Select(c => c.Entrance)
            .Concat(Trees.Select(t => t.Access)).Concat(Bushes.Select(b => b.Access)).Concat(Map.StoneDeposits.Select(d=>d.Access))
            .Concat(People.Where(p => p.LeisureSiteId != null || p.Task is Work.ToRest or Work.Resting).Select(p => p.Destination));
        if (access.Where(before.Contains).Concat(People.Select(At))
            .Concat(People.Where(p => p.Route.Count > 0).Select(p => p.Route.Peek())).Any(c => !after.Contains(c)))
            return "This bridge keeps villagers or resources connected. Build another crossing first.";
        return null;
    }

    public bool RemoveBuilding(int id)
    {
        if (!Creative || RemovalProblem(id) != null) return false;
        var site = Cottages.Single(c => c.Id == id);
        var affected = People.Where(p => p.SiteId == id || p.WorkplaceId == id || p.StorageId == id ||
            p.HaulTargetId == id || p.LeisureSiteId == id).ToArray();
        // Remove the destination before returning cargo, so it cannot be chosen again.
        Cottages.Remove(site);
        ReconcileHomes();
        _yardLogs += site.StoredLogs + site.InputLogs + (site.Material == Resource.Logs ? site.Delivered : 0);
        _yardPlanks += site.StoredPlanks + site.OutputPlanks + (site.Material == Resource.Planks ? site.Delivered : 0);
        _stone+=site.DeliveredStone;
        Food.Grain += site.InputGrain + (site.Kind == BuildingKind.Farm ? site.Harvest : 0);
        Food.Vegetables += site.Kind == BuildingKind.VegetableGarden ? site.Harvest : 0;
        Food.Bread += site.OutputBread;
        foreach (var person in affected) Interrupt(person);
        foreach (var person in People.Where(p => p.Route.Count > 0)) SetRoute(person, person.Destination);
        History.Add($"Removed {site.Kind} {id}; stored goods returned to the yard.");
        _retry = 0;
        return true;
    }

    private bool ClearImmediately(TimberTree tree)
    {
        if (tree.Owner is int owner) Interrupt(People[owner]);
        if (tree.Material == Resource.Logs) _yardLogs += tree.Logs;
        else if(tree.Material==Resource.Stone) _stone+=tree.Logs;
        else _yardPlanks += tree.Logs;
        Trees.Remove(tree);
        _retry = 0;
        return true;
    }
}

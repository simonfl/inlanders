using System;
using System.Linq;

namespace Inlanders.Simulation;

public sealed record WorkplaceReport(string State, string Detail, Cell? Source = null, int? SourceBuilding = null);

public sealed partial class World
{
    public static Resource? ProductionOutput(BuildingKind kind) => kind switch
    {
        BuildingKind.ForagerHut => Resource.Berries, BuildingKind.Farm => Resource.Grain,
        BuildingKind.VegetableGarden => Resource.Vegetables, BuildingKind.Bakery => Resource.Bread,
        BuildingKind.Sawmill => Resource.Planks, _ => null
    };
    public bool SetWorkplacePaused(int id, bool paused)
    {
        var site = Cottages.FirstOrDefault(c => c.Id == id && c.Complete && ProductionOutput(c.Kind) != null);
        if (site == null || Food.Celebrating) return false;
        site.WorkPaused = paused; _retry = 0; return true;
    }
    public bool SetOutputTarget(int id, int target)
    {
        var site = Cottages.FirstOrDefault(c => c.Id == id && c.Complete && ProductionOutput(c.Kind) != null);
        if (site == null || Food.Celebrating || target < -1 || target > 200) return false;
        site.OutputTarget = target; _retry = 0; return true;
    }
    // Count committed batches/crops as well as stock, so workers cannot all spend the same headroom.
    public int ProductionCommitted(Resource resource)
    {
        int cargo = People.Where(p => p.Cargo == resource).Sum(p => p.Carried);
        int Crops(BuildingKind kind, int yield) => Cottages.Where(c => c.Kind == kind).Sum(c => c.Harvest + (c.Planted && c.Harvest == 0 ? yield : 0)) +
            People.Count(p => p.WorkplaceId is int id && Cottages.Any(c => c.Id == id && c.Kind == kind && !c.Planted && c.Harvest == 0) && p.Task is Work.ToFarm or Work.Planting) * yield;
        return resource switch
        {
            Resource.Planks => PendingPlanks,
            Resource.Berries => Food.Berries + cargo + People.Where(p => p.Task is Work.ToBush or Work.Foraging && p.BushId != null).Sum(p => Math.Min(2, Bushes.Single(b => b.Id == p.BushId).Ripe)),
            Resource.Grain => Food.Grain + cargo + Cottages.Sum(c => c.InputGrain) + Crops(BuildingKind.Farm, 6),
            Resource.Vegetables => Food.Vegetables + cargo + Crops(BuildingKind.VegetableGarden, 8),
            Resource.Bread => Food.Bread + cargo + Cottages.Sum(c => c.OutputBread + c.InputGrain * 2) +
                People.Where(p => p.Task == Work.ToGrain).Sum(p => p.FoodReserved * 2) + People.Where(p => p.Task == Work.ToOven).Sum(p => p.Carried * 2),
            _ => Stored + cargo
        };
    }
    private bool BelowOutputTarget(Cottage site) => site.OutputTarget < 0 || ProductionOutput(site.Kind) is Resource output && ProductionCommitted(output) < site.OutputTarget;
    private string ProductionWait(Role role)
    {
        var sites = Cottages.Where(c => c.Complete && Buildings.Get(c.Kind).Worker == role).ToArray();
        return sites.Length == 0 ? $"Needs a finished workplace for {role.ToString().ToLowerInvariant()}s" :
            string.Join("; ", sites.Select(c => $"{Buildings.Get(c.Kind).Name} {c.Id}: {ReadWorkplace(c).State}"));
    }

    public WorkplaceReport ReadWorkplace(Cottage site)
    {
        if (!site.Complete) return new("Under construction", "Builders must finish this workplace first.");
        var workers = People.Where(p => p.WorkplaceId == site.Id).ToArray();
        if (Food.Celebrating) return new("Village supper", "Work resumes after everyone gathers.");
        if (site.WorkPaused) return new(workers.Length > 0 ? "Pausing · finishing work" : "Paused", "No new jobs. Current work and deliveries finish; planted crops keep growing. Resume to collect any remaining output.");
        if (workers.Length > 0)
        {
            var p = workers[0];
            Cell? source = p.BushId is int bush ? Bushes.Single(b => b.Id == bush).Access :
                p.Task is Work.ToGrain or Work.ToOven or Work.ToPantry or Work.ToStockpile ? YardAccess :
                p.Task == Work.ToSawLogs ? StorageAccess(p.StorageId) : null;
            string state = p.Task switch
            {
                Work.ToGrain or Work.ToSawLogs => "Fetching input", Work.ToOven or Work.ToSawmill => "Delivering input",
                Work.ToPantry or Work.ToStockpile => "Delivering output", Work.ToBread or Work.ToPlanks => "Collecting output",
                Work.Baking => "Baking", Work.Sawing => "Sawing", Work.Planting => "Sowing",
                Work.Harvesting => "Harvesting", Work.Foraging => "Picking berries", _ => "Walking to work"
            };
            return new(state, string.Join("\n", workers.Select(w => $"{w.Name}: {w.Status}")), source, p.Task == Work.ToSawLogs ? p.StorageId : null);
        }
        bool remaining = site.Harvest > 0 || site.InputGrain > 0 || site.OutputBread > 0 || site.InputLogs > 0 || site.OutputPlanks > 0;
        if (!remaining && site.Planted) return new("Growing", $"Crop {site.Growth:P0}. A farmer returns when ripe.");
        if (!remaining && !BelowOutputTarget(site)) return new("Target met", "Stored goods and committed production cover this workplace's target. New work resumes when they fall below it.");
        var role = Buildings.Get(site.Kind).Worker;
        if (role != null && !People.Any(p => p.Role == role)) return new("No staff", $"Assign a {role.ToString()!.ToLowerInvariant()} in People. Assignments are village-wide.");
        if (site.Kind == BuildingKind.Bakery && !remaining && Food.Grain - ReservedGrain < 2)
            return new("Missing grain", "Needs 2 unreserved grain in the pantry. Growing or carried grain is not available yet.", YardAccess);
        if (site.Kind == BuildingKind.Sawmill && !remaining && !TryLogSource(site.Entrance, 2, out _))
            return new("Missing logs", "Needs 2 unreserved logs at a reachable store.", YardAccess);
        if (site.Kind == BuildingKind.ForagerHut && !Bushes.Any(b => b.Ripe > 0 && b.Owner == null && Accessible(b.Access)))
            return new("Waiting for berries", "Berries are regrowing, claimed, or beyond reach. Inspect foragers and the map.");
        return new(remaining ? "Waiting for collection or work" : "Waiting for a worker", "Workers share workplaces and may be finishing another job, returning cargo, or taking a break.");
    }
    private void ValidateProduction()
    {
        foreach (var c in Cottages)
            if (c.OutputTarget < -1 || c.OutputTarget > 200 || (ProductionOutput(c.Kind) == null && (c.WorkPaused || c.OutputTarget != -1)))
                throw new InvalidOperationException("Invalid workplace controls");
    }
}

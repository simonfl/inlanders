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
        BuildingKind.Orchard => Resource.Fruit,
        BuildingKind.HuntingLodge => Resource.Game, BuildingKind.Quarry => Resource.Stone, BuildingKind.Sawmill => Resource.Planks, BuildingKind.FishingDock => Resource.Fish, _ => null
    };
    public bool SetWorkplacePaused(int id, bool paused)
    {
        var site = Cottages.FirstOrDefault(c => c.Id == id && c.Complete && !c.DemolitionRequested && (ProductionOutput(c.Kind) != null || c.Kind==BuildingKind.Carpenter));
        if (site == null || Food.Celebrating) return false;
        site.WorkPaused = paused;
        if(paused && site.Kind==BuildingKind.FishingDock && site.Boat?.FisherId is int fisher) Interrupt(People[fisher]);
        _retry = 0; return true;
    }
    public bool SetOutputTarget(int id, int target)
    {
        var site = Cottages.FirstOrDefault(c => c.Id == id && c.Complete && !c.DemolitionRequested && ProductionOutput(c.Kind) != null);
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
            Resource.Game => StoredFood(Resource.Game)+cargo+People.Where(p=>p.HabitatId!=null).Sum(p=>p.Reserved),
            Resource.Stone => Stone+cargo+People.Where(p=>p.DepositId!=null).Sum(p=>p.Reserved),
            Resource.Planks => PendingPlanks,
            Resource.Fish => StoredFood(Resource.Fish) + cargo + Cottages.Sum(c=>(c.Boat?.Fish??0)+(c.Boat?.ReservedCatch??0)),
            Resource.Berries => StoredFood(Resource.Berries) + cargo + People.Where(p => p.Task is Work.ToBush or Work.Foraging && p.BushId != null).Sum(p => Math.Min(2, Bushes.Single(b => b.Id == p.BushId).Ripe)),
            Resource.Grain => StoredGrain + cargo + Cottages.Sum(c => c.InputGrain) + Crops(BuildingKind.Farm, 6),
            Resource.Vegetables => StoredFood(Resource.Vegetables) + cargo + Crops(BuildingKind.VegetableGarden, 8),
            Resource.Fruit => StoredFood(Resource.Fruit) + cargo + Crops(BuildingKind.Orchard, 8),
            Resource.Bread => StoredFood(Resource.Bread) + cargo + Cottages.Sum(c => c.OutputBread + c.InputGrain * 2) +
                People.Where(p => p.Task == Work.ToGrain).Sum(p => p.FoodReserved * 2) + People.Where(p => p.Task == Work.ToOven).Sum(p => p.Carried * 2),
            _ => Stored + cargo
        };
    }
    private bool BelowOutputTarget(Cottage site) => site.OutputTarget < 0 || ProductionOutput(site.Kind) is Resource output && ProductionCommitted(output) < site.OutputTarget;
    private string ProductionWait(Villager person)
    {
        var role=person.Role;
        if(person.AssignedWorkplaceId is int assigned)
        {
            var site=Cottages.Single(c=>c.Id==assigned);
            return !FreeStation(site)?"Assigned workplace: waiting for current worker to finish":
                $"Assigned {Buildings.Get(site.Kind).Name} {assigned}: {ReadWorkplace(site).State}. Choose Automatic to work elsewhere.";
        }
        var sites = Cottages.Where(c => c.Complete && Buildings.Get(c.Kind).Worker == role).ToArray();
        if(sites.Length>0 && sites.All(c=>AssignedWorkers(c.Id)>=Buildings.Get(c.Kind).Slots))return "Workplaces fully assigned — choose another site or release an assigned slot";
        return sites.Length == 0 ? $"Needs a finished workplace for {role.ToString().ToLowerInvariant()}s" :
            string.Join("; ", sites.Select(c => $"{Buildings.Get(c.Kind).Name} {c.Id}: {ReadWorkplace(c).State}"));
    }

    public WorkplaceReport ReadWorkplace(Cottage site)
    {
        if (site.DemolitionRequested) return new("Demolition ordered", "Production stopped; builders recover goods, dismantle the building and haul its timber.");
        if (!site.Complete) return new("Under construction", "Builders must finish this workplace first.");
        var workers = People.Where(p => p.WorkplaceId == site.Id).ToArray();
        if (Food.Celebrating) return new("Village supper", "Work resumes after everyone gathers.");
        if (site.WorkPaused && site.Kind==BuildingKind.FishingDock) return new(workers.Length>0 ? "Recalling boat" : "Paused", "No new trips. A fisher at sea returns before changing jobs or leaving the dock.");
        if (site.WorkPaused) return new(workers.Length > 0 ? "Pausing · finishing work" : "Paused", "No new jobs. Current work and deliveries finish; planted crops keep growing. Resume to collect any remaining output.");
        if(site.Kind==BuildingKind.FishingDock && site.Boat?.FisherId is int fisher)
            return new(People[fisher].Status, $"{People[fisher].Name} · {site.Boat.Fish} fish aboard · {site.Boat.ReservedCatch} catch reserved. Fish count as pantry supply only after delivery.");
        if (workers.Length > 0)
        {
            if(site.Kind==BuildingKind.Carpenter) return new("Home improvement",string.Join("\n",workers.Select(p=>$"{p.Name}: {p.Status}")),workers[0].ComfortHomeId is int homeId?Cottages.Single(c=>c.Id==homeId).Entrance:null,workers[0].ComfortHomeId);
            var p = workers[0];
            Cell? source = p.HabitatId is int habitat ? Map.Wildlife.Single(h=>h.Id==habitat).Cell : p.BushId is int bush ? Bushes.Single(b => b.Id == bush).Access :
                p.Task==Work.ToStockpile ? StorageAccess(p.StorageId) :
                p.Task==Work.ToPantry ? p.GrainDestinationId is int grainDestination?GrainAccess(grainDestination):FoodAccess(p.FoodDestinationId) :
                p.Task==Work.ToGrain ? GrainAccess(p.GrainSourceId) :
                p.Task==Work.ToOven ? LocalGrainSupply?site.Entrance:YardAccess :
                p.Task == Work.ToSawLogs ? StorageAccess(p.StorageId) : null;
            string state = p.Task switch
            {
                Work.ToGrain or Work.ToSawLogs => "Fetching input", Work.ToOven or Work.ToSawmill => "Delivering input",
                Work.ToPantry or Work.ToStockpile => "Delivering output", Work.ToBread or Work.ToPlanks => "Collecting output",
                Work.Baking => "Baking", Work.Sawing => "Sawing", Work.Planting => "Sowing",
                Work.Hunting => "Hunting", Work.Quarrying => "Quarrying", Work.Harvesting => "Harvesting", Work.Foraging => "Picking berries", _ => "Walking to work"
            };
            return new(state, string.Join("\n", workers.Select(w => $"{w.Name}: {w.Status}")), source, p.Task==Work.ToGrain?p.GrainSourceId:p.Task==Work.ToPantry && p.GrainDestinationId!=null?p.GrainDestinationId:p.Task is Work.ToSawLogs or Work.ToStockpile ? p.StorageId : null);
        }
        if(site.Kind==BuildingKind.Carpenter) return new("Waiting for home orders",$"{Cottages.Count(c=>c.ImprovementRequested && !c.DemolitionRequested)} pending. Order improvements on occupied homes; assign a carpenter and supply planks.");
        bool remaining = site.Harvest > 0 || site.InputGrain > 0 || site.OutputBread > 0 || site.InputLogs > 0 || site.OutputPlanks > 0;
        if(site.Kind==BuildingKind.Orchard && !remaining && site.Planted)return new(site.OrchardMature?"Fruit growing":"Trees establishing",$"{site.Growth:P0} · about {(1-site.Growth)*(site.OrchardMature?60:180):0}s until ripe. Automatic farmers can work elsewhere. Assigned farmers wait for this orchard; choose Automatic in People to release them. Mature trees stay for repeat harvests.");
        if (!remaining && site.Planted) return new("Growing", $"Crop {site.Growth:P0}. A farmer returns when ripe.");
        if (!remaining && !BelowOutputTarget(site)) return new("Target met", "Stored goods and committed production cover this workplace's target. New work resumes when they fall below it.");
        var role = Buildings.Get(site.Kind).Worker;
        if (role != null && !People.Any(p => p.Role == role && (p.AssignedWorkplaceId==null || p.AssignedWorkplaceId==site.Id))) return new("No staff", $"Assign a {role.ToString()!.ToLowerInvariant()} in People. Workers assigned elsewhere do not take jobs here.");
        if (site.Kind == BuildingKind.Bakery && !remaining && !TryGrainSource(site.Entrance,site.Entrance,out _))
            return new("Missing grain", LocalGrainSupply?"Needs 2 grain at a reachable farm store or central pantry. Harvest and deliver grain first.":"Needs 2 unreserved grain in the pantry. Growing or carried grain is not available yet.", YardAccess);
        if (site.Kind == BuildingKind.Sawmill && !remaining && !TryLogSource(site.Entrance, 2, out _))
            return new("Missing logs", "Needs 2 unreserved logs at a reachable store.", YardAccess);
        if (site.Kind == BuildingKind.ForagerHut && !Bushes.Any(b => b.Ripe > 0 && b.Owner == null && Accessible(b.Access)))
            return new("Waiting for berries", "Berries are regrowing, claimed, or beyond reach. Inspect foragers and the map.");
        if(site.Kind==BuildingKind.HuntingLodge) return new(HuntingGrounds(site.Cell).Any(h=>AvailableGame(h)>0)?"Waiting for a hunter":"Habitat recovering or claimed",WildlifeSurvey(site.Cell)+"\nPause hunting to restore stock; retain or regrow mature trees to restore capacity.");
        if(site.Kind==BuildingKind.Quarry) return new(QuarryDeposits(site).Any(d=>AvailableDeposit(d)>0)?"Waiting for a quarrier":"Outcrop exhausted or claimed",QuarrySurvey(site.Cell));
        if(site.Kind==BuildingKind.FishingDock)
        {
            var grounds=Map.FishingGrounds.Where(g=>FindBoatRoute(site.Launch,g.Cell)!=null).ToArray();
            if(grounds.Length==0) return new("No reachable fishing ground","A crossing blocks access to the fishing grounds. Restore a water route before assigning trips.");
            return new(grounds.All(g=>AvailableFish(g)==0)?"Waiting for fish":"Waiting for a fisher",
                string.Join("\n",grounds.Select(g=>$"{g.Name}: {AvailableFish(g)} available / {g.Capacity} capacity · +{g.RegrowthPerSecond*60:0.#}/min. Stock is shared by all docks.")));
        }
        return new(remaining ? "Waiting for collection or work" : "Waiting for a worker", "Workers share workplaces and may be finishing another job, returning cargo, or taking a break.");
    }
    private void ValidateProduction()
    {
        ValidateWorkplaceAssignments();
        foreach (var c in Cottages)
            if (c.OutputTarget < -1 || c.OutputTarget > 200 || (ProductionOutput(c.Kind) == null && (c.WorkPaused && !c.DemolitionRequested && c.Kind!=BuildingKind.Carpenter || c.OutputTarget != -1)))
                throw new InvalidOperationException("Invalid workplace controls");
    }
}

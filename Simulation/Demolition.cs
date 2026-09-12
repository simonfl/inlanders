using System;
using System.Linq;

namespace Inlanders.Simulation;

public sealed partial class World
{
    public const float DismantleSeconds = 12;
    public bool RequestDemolition(int id)
    {
        var site = Cottages.FirstOrDefault(c => c.Id == id);
        if (Creative || site == null || site.DemolitionRequested || RemovalProblem(id) != null) return false;
        site.DemolitionRequested = true; site.DemolitionWasPaused = site.WorkPaused; site.WorkPaused = true;
        StopImprovement(site);
        if(site.Kind==BuildingKind.Pantry) ClosePantry(id);
        ReconcileHomes();
        foreach (var person in People.Where(p => p.SiteId == id || p.WorkplaceId == id || p.StorageId == id || p.HaulTargetId == id || p.LeisureSiteId == id).ToArray()) Interrupt(person);
        History.Add($"Demolition ordered for {site.Kind} {id}; builders recover goods and timber."); _retry = 0; return true;
    }
    public bool CancelDemolition(int id)
    {
        var site = Cottages.FirstOrDefault(c => c.Id == id && c.DemolitionRequested);
        if (site == null || site.DemolitionProgress > 0 || Food.Celebrating) return false;
        foreach (var person in People.Where(p => p.SiteId == id).ToArray()) Interrupt(person);
        site.DemolitionRequested = false; site.WorkPaused = site.DemolitionWasPaused; ReconcileHomes(); _retry = 0; return true;
    }
    private bool ClaimDemolition(Villager v)
    {
        var site = Cottages.Where(c => c.DemolitionRequested && c.Builder == null).OrderByDescending(c => c.Priority).ThenBy(c => c.Id).FirstOrDefault();
        if (site == null) return false;
        site.Builder = v.Id; v.SiteId = site.Id;
        Go(v, site.Entrance, Work.ToDemolish, $"Walking to dismantle {site.Kind} {site.Id}"); return true;
    }
    private void TickDemolition(Villager v, float dt)
    {
        var site = Cottages.Single(c => c.Id == v.SiteId);
        if (v.Task == Work.ToDemolish) { v.Task = Work.Demolishing; v.Timer = 0; return; }
        bool Take(int count, Action<int> set, Resource resource,bool alreadyStored=false)
        {
            if (count == 0) return false;
            int amount = Math.Min(2, count); set(count - amount);
            site.Builder = null; Finish(v); v.Carried = amount; v.Cargo = resource;
            if (resource is Resource.Logs or Resource.Planks or Resource.Stone) ReturnTimber(v);
            else { v.FoodTransfer=alreadyStored; Go(v, YardAccess, Work.ToPantry, $"Recovering {amount} {resource} from demolition"); }
            return true;
        }
        for(int k=0;k<EdibleKinds.Length;k++)
        {
            int index=k;
            if(Take(site.PantryFood[index],n=>site.PantryFood[index]=n,EdibleKinds[index],true)) return;
        }
        if (Take(site.StoredLogs, n => site.StoredLogs = n, Resource.Logs) ||
            Take(site.StoredPlanks, n => site.StoredPlanks = n, Resource.Planks) ||
            Take(site.StoredStone, n => site.StoredStone = n, Resource.Stone) ||
            Take(site.InputLogs, n => site.InputLogs = n, Resource.Logs) || Take(site.OutputPlanks, n => site.OutputPlanks = n, Resource.Planks) ||
            Take(site.InputGrain, n => site.InputGrain = n, Resource.Grain) || Take(site.OutputBread, n => site.OutputBread = n, Resource.Bread) ||
            Take(site.Harvest, n => { site.Harvest = n; if (n == 0) { site.Planted = false; site.Growth = 0; } }, site.Kind==BuildingKind.Orchard?Resource.Fruit:site.Kind == BuildingKind.Farm ? Resource.Grain : Resource.Vegetables)) return;
        if (site.DemolitionProgress < 1)
        {
            v.Status = $"Dismantling {site.Kind} {site.Id}";
            site.DemolitionProgress = Math.Min(1, site.DemolitionProgress + dt / DismantleSeconds); return;
        }
        if (Take(site.ImprovementPlanks,n=>site.ImprovementPlanks=n,Resource.Planks)) return;
        if (Take(site.Delivered, n => site.Delivered = n, site.Material)) return;
        if (Take(site.DeliveredStone,n=>site.DeliveredStone=n,Resource.Stone)) return;
        if (RemovalProblem(site.Id) is string problem) { v.Status = "Demolition waiting: " + problem; return; }
        site.Builder = null; Finish(v); Cottages.Remove(site);
        foreach (var person in People.Where(p => p.Route.Count > 0)) SetRoute(person, person.Destination);
        History.Add($"Demolished {site.Kind} {site.Id}; goods and timber recovered."); _retry = 0;
    }
    private void ValidateDemolition()
    {
        foreach (var site in Cottages)
        {
            if (!float.IsFinite(site.DemolitionProgress) || site.DemolitionProgress < 0 || site.DemolitionProgress > 1 ||
                site.DemolitionRequested && (Creative || !site.Complete || !site.WorkPaused) || !site.DemolitionRequested && site.DemolitionProgress != 0)
                throw new InvalidOperationException("Invalid demolition state");
            if (site.DemolitionRequested && (site.StoredLogs > StockpileCapacity || People.Any(p => p.WorkplaceId == site.Id || p.StorageId == site.Id || p.HaulTargetId == site.Id || p.LeisureSiteId == site.Id)))
                throw new InvalidOperationException("Demolition site still has a service or storage claim");
        }
        foreach (var person in People.Where(p => p.Task is Work.ToDemolish or Work.Demolishing))
            if (!Cottages.Any(c => c.Id == person.SiteId && c.DemolitionRequested && c.Builder == person.Id)) throw new InvalidOperationException("Invalid demolition worker");
    }
}

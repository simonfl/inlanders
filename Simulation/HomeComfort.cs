using System;
using System.Linq;

namespace Inlanders.Simulation;

public sealed partial class World
{
    public static int ComfortCost(Cottage home) => Buildings.Get(home.Kind).Beds*2;
    private static bool ComfortWork(Villager p) => p.Task is Work.ToComfortInstall or Work.InstallingComfort;
    private bool ComfortSpotReserved(Cell cell) => People.Any(p=>ComfortWork(p) && p.Destination==cell);
    public int ComfortIncoming(Cottage home) => People.Where(p=>p.ComfortHomeId==home.Id).Sum(p=>p.Reserved);
    public string? ImprovementProblem(int id)
    {
        var home=Cottages.FirstOrDefault(c=>c.Id==id);
        if(home==null || !IsHome(home)) return "Choose a completed home not marked for demolition.";
        if(Food.Celebrating) return "Wait until supper finishes.";
        if(home.Improved) return "This home is already improved.";
        if(home.ImprovementRequested) return "An improvement is already ordered.";
        if(home.ImprovementPlanks>0 || People.Any(p=>p.ComfortHomeId==id)) return "Wait for cancelled materials to be recovered.";
        if(!People.Any(p=>p.HomeId==id)) return "Assign a resident before improving this home.";
        return null;
    }
    public bool RequestImprovement(int id)
    {
        if(ImprovementProblem(id)!=null) return false;
        var home=Cottages.Single(c=>c.Id==id);
        home.ImprovementOrderedAt=Food.Time;
        if(Creative) { home.Improved=true;home.ImprovementProgress=1; }
        else home.ImprovementRequested=true;
        _retry=0; return true;
    }
    public bool CancelImprovement(int id)
    {
        var home=Cottages.FirstOrDefault(c=>c.Id==id && c.ImprovementRequested && !c.DemolitionRequested);
        if(home==null || Food.Celebrating) return false;
        StopImprovement(home); return true;
    }
    private void StopImprovement(Cottage home)
    {
        home.ImprovementRequested=false; if(!home.Improved) home.ImprovementProgress=0;
        foreach(var p in People.Where(p=>p.ComfortHomeId==home.Id).ToArray()) Interrupt(p);
        _retry=0;
    }
    public string ComfortSummary(Cottage home)
    {
        if(home.Improved) return "Improved home · rest benefit lasts 5m; next rest due after 4m. No extra beds.";
        if(!home.ImprovementRequested) return home.ImprovementPlanks>0 ? $"Cancelled · builders recover {home.ImprovementPlanks} planks." : $"Improve for {ComfortCost(home)} planks. Rest benefit lasts 5m instead of 4m; visits are due after 4m instead of 3m.";
        var worker=People.FirstOrDefault(p=>p.ComfortHomeId==home.Id);
        string reason=worker!=null?$"{worker.Name}: {worker.Status}": !People.Any(p=>p.HomeId==home.Id)?"Waiting for a resident.":
            !Cottages.Any(c=>c.Kind==BuildingKind.Carpenter && c.Complete && !c.DemolitionRequested && !c.WorkPaused)?"Needs an open carpenter workshop.":
            !People.Any(p=>p.Role==Role.Carpenter)?"Assign a carpenter in People.":home.ImprovementPlanks+ComfortIncoming(home)<ComfortCost(home) && AvailablePlanks==0?"Waiting for unreserved planks.":"Waiting for a carpenter and reachable free work spot.";
        return $"Home improvement · {home.ImprovementPlanks}/{ComfortCost(home)} planks delivered · {ComfortIncoming(home)} committed · installed {home.ImprovementProgress:P0}\n{reason}";
    }
    private Cell? ComfortSpot(Villager worker,Cottage home)
    {
        var busy=People.Where(p=>p.Task is Work.ToRest or Work.Resting || p.LeisureSiteId!=null || ComfortWork(p)).Select(p=>p.Destination).ToHashSet();
        return new[]{home.Entrance,new(home.Entrance.X-1,home.Entrance.Z),new(home.Entrance.X+1,home.Entrance.Z),new(home.Entrance.X,home.Entrance.Z-1),new(home.Entrance.X,home.Entrance.Z+1)}
            .Where(c=>!Blocked(c) && !MealSpotReserved(c) && !busy.Contains(c)).Cast<Cell?>().FirstOrDefault(c=>FindPath(At(worker),c!.Value,Blocked)!=null);
    }
    private bool ClaimComfortRecovery(Villager worker)
    {
        var home=Cottages.Where(c=>!c.DemolitionRequested && !c.ImprovementRequested && !c.Improved && c.ImprovementPlanks>0 && !People.Any(p=>p.ComfortHomeId==c.Id))
            .FirstOrDefault(c=>FindPath(At(worker),c.Entrance,Blocked)!=null);
        if(home==null) return false;
        worker.ComfortHomeId=home.Id;Go(worker,home.Entrance,Work.ToComfortRecovery,"Collecting cancelled improvement planks");return true;
    }
    private void ClaimComfort(Villager worker)
    {
        var workshop=FoodSite(worker,BuildingKind.Carpenter,c=>!c.DemolitionRequested && FindPath(At(worker),c.Entrance,Blocked)!=null);
        if(workshop==null) {worker.Status=ProductionWait(worker);return;}
        foreach(var home in Cottages.Where(c=>IsHome(c) && c.ImprovementRequested && People.Any(p=>p.HomeId==c.Id) && !People.Any(p=>p.ComfortHomeId==c.Id)).OrderBy(c=>c.ImprovementOrderedAt).ThenBy(c=>c.Id))
        {
            if(FindPath(At(worker),home.Entrance,Blocked)==null) continue;
            if(home.ImprovementPlanks<ComfortCost(home))
            {
                if(!TryMaterialSource(home.Entrance,Resource.Planks,1,out int? source)) continue;
                worker.Reserved=Math.Min(2,Math.Min(ComfortCost(home)-home.ImprovementPlanks,AvailableMaterialAt(source,Resource.Planks)));
                worker.Cargo=Resource.Planks;worker.StorageId=source;worker.ComfortHomeId=home.Id;worker.WorkplaceId=workshop.Id;
                Go(worker,StorageAccess(source),Work.ToComfortPlanks,"Collecting planks for home improvement");return;
            }
            if(ComfortSpot(worker,home) is not Cell spot) continue;
            worker.ComfortHomeId=home.Id;worker.WorkplaceId=workshop.Id;
            Go(worker,spot,Work.ToComfortInstall,"Walking to improve an occupied home");return;
        }
        worker.Status="Waiting for occupied-home orders, planks or a free installation spot";
    }
    private bool TickComfort(Villager worker,float dt)
    {
        if(worker.Task is not (Work.ToComfortPlanks or Work.ToComfortHome or Work.ToComfortInstall or Work.InstallingComfort or Work.ToComfortRecovery)) return false;
        var home=Cottages.Single(c=>c.Id==worker.ComfortHomeId);
        switch(worker.Task)
        {
            case Work.ToComfortPlanks:
                ChangeMaterial(worker.StorageId,Resource.Planks,-worker.Reserved);worker.StorageId=null;worker.Carried=worker.Reserved;
                Go(worker,home.Entrance,Work.ToComfortHome,"Delivering improvement planks");break;
            case Work.ToComfortHome:
                home.ImprovementPlanks+=worker.Carried;worker.Carried=0;Finish(worker);break;
            case Work.ToComfortInstall:
                worker.Task=Work.InstallingComfort;worker.Timer=0;worker.Status="Installing home furnishings";break;
            case Work.InstallingComfort:
                if(!People.Any(p=>p.HomeId==home.Id)) { Finish(worker);break; }
                home.ImprovementProgress=Math.Min(1,home.ImprovementProgress+dt/(ComfortCost(home)*3));
                if(home.ImprovementProgress>=1) {home.Improved=true;home.ImprovementRequested=false;History.Add($"{home.Kind} {home.Id} improved");Finish(worker);}break;
            case Work.ToComfortRecovery:
                int count=Math.Min(2,home.ImprovementPlanks);home.ImprovementPlanks-=count;Finish(worker);
                worker.Cargo=Resource.Planks;worker.Carried=count;ReturnTimber(worker);break;
        }
        return true;
    }
    private void ValidateComfort()
    {
        foreach(var home in Cottages)
        {
            bool hasState=home.Improved || home.ImprovementRequested || home.ImprovementPlanks!=0 || home.ImprovementProgress!=0;
            if(hasState && (Buildings.Get(home.Kind).Beds==0 || !home.Complete) || home.ImprovementPlanks<0 || home.ImprovementPlanks+ComfortIncoming(home)>ComfortCost(home) ||
                !float.IsFinite(home.ImprovementProgress) || home.ImprovementProgress<0 || home.ImprovementProgress>1 || !float.IsFinite(home.ImprovementOrderedAt) || home.ImprovementOrderedAt<0 || home.ImprovementOrderedAt>Food.Time ||
                home.Improved && (home.ImprovementRequested || home.ImprovementProgress!=1 || !Creative && !home.DemolitionRequested && home.ImprovementPlanks!=ComfortCost(home))) throw new InvalidOperationException("Invalid home improvement");
            if(People.Count(p=>p.ComfortHomeId==home.Id)>1) throw new InvalidOperationException("Home improvement claimed twice");
        }
        foreach(var p in People)
        {
            bool active=p.Task is Work.ToComfortPlanks or Work.ToComfortHome or Work.ToComfortInstall or Work.InstallingComfort or Work.ToComfortRecovery;
            if(active!=(p.ComfortHomeId!=null)) throw new InvalidOperationException("Orphaned comfort job");
            if(!active) continue;
            var home=Cottages.FirstOrDefault(c=>c.Id==p.ComfortHomeId && !c.DemolitionRequested);
            if(home==null || !IsHome(home) || p.SiteId!=null || p.Task!=Work.ToComfortRecovery && (!home.ImprovementRequested || !Cottages.Any(c=>c.Id==p.WorkplaceId && c.Kind==BuildingKind.Carpenter && c.Complete && !c.DemolitionRequested))) throw new InvalidOperationException("Invalid comfort destination");
            if(p.Task is Work.ToComfortPlanks or Work.ToComfortHome && (p.Cargo!=Resource.Planks || p.Reserved is <1 or >2 || p.Carried!=(p.Task==Work.ToComfortHome?p.Reserved:0))) throw new InvalidOperationException("Invalid comfort shipment");
            if(ComfortWork(p) && (p.Carried!=0 || p.Reserved!=0 || Blocked(p.Destination))) throw new InvalidOperationException("Invalid comfort installation");
        }
    }
}

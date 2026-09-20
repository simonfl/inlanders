using System;
using System.Linq;

namespace Inlanders.Simulation;

public sealed partial class World
{
    public string? RelocationProblem(int id)
    {
        var site=Cottages.FirstOrDefault(c=>c.Id==id);
        if(!Creative && !IsArrangementCourt && Founding==null)return "Moving buildings is available in founding, Creative or Willow court.";
        if(Founding!=null && site!=null && Buildings.Get(site.Kind).Beds==0 && Buildings.Get(site.Kind).RecreationSlots==0)
        {
            if(!Founding.RiverFarmstead)return "Rearrange homes and gathering places for free. Rebuild workplaces to change their sites.";
            if(site.Kind is BuildingKind.Farm or BuildingKind.Orchard or BuildingKind.Bridge)
                return "Grain fields, orchards and crossings keep their sites. Rebuild to change the land you use.";
            if((ProductionOutput(site.Kind)!=null || site.Kind==BuildingKind.Carpenter) && !site.WorkPaused)
                return "Pause production first, then move this workplace. Goods and settings stay with it.";
        }
        if(!Creative && IsArrangementCourt && Neighborhood!.Arrangement!.BuildingId is int trial && trial!=id)return "Restore the current trial before trying another building.";
        if(site==null || !site.Complete || site.DemolitionRequested)return "Choose a finished building that is not being demolished.";
        if(Food.Celebrating)return "Wait until supper finishes.";
        if(site.Boat?.FisherId!=null)return "Pause the dock and wait for its fisher to return before moving it.";
        return null;
    }
    private bool MoveAffects(Villager p,Cottage site)=>p.SiteId==site.Id || p.WorkplaceId==site.Id || p.StorageId==site.Id || p.HaulTargetId==site.Id ||
        p.LeisureSiteId==site.Id || p.ComfortHomeId==site.Id || p.FoodSourceId==site.Id || p.FoodDestinationId==site.Id || p.GrainSourceId==site.Id || p.GrainDestinationId==site.Id ||
        p.Meal is {} meal && meal.SourceId==site.Id || p.HomeId==site.Id && (p.Task is Work.ToRest or Work.Resting || IdleHomeJourney(p));
    public string? RelocationProblem(int id,Cell at,int rotation)
    {
        string? problem=RelocationProblem(id);if(problem!=null)return problem;
        if(rotation is <0 or >3)return "Choose one of four building orientations.";
        var site=Cottages.Single(c=>c.Id==id);
        if(site.Cell==at && site.Rotation==rotation)return null;
        var before=Reachable(YardAccess,Blocked);
        var access=Cottages.Where(c=>c!=site).Select(c=>c.Entrance).Concat(Trees.Select(t=>t.Access)).Concat(Bushes.Select(b=>b.Access))
            .Concat(Map.StoneDeposits.Select(d=>d.Access)).Concat(Map.Wildlife.Select(h=>h.Cell))
            .Concat(People.Where(p=>!MoveAffects(p,site) && p.Route.Count>0).Select(p=>p.Destination)).Where(before.Contains)
            .Concat(People.Select(At)).ToArray();
        int index=Cottages.IndexOf(site);var original=site.Cell;int facing=site.Rotation;bool bridgeFar=site.BridgeFromFar,dockFar=site.DockFromFar;
        // Inspect proposed geometry only. Restore the exact object/order before returning,
        // including on rejection; no worker, goods or route changes occur in this query.
        Cottages.RemoveAt(index);
        try
        {
            problem=PlacementProblem(at,rotation,site.Kind);if(problem!=null)return problem;
            site.BridgeFromFar=site.Kind==BuildingKind.Bridge && !Accessible(Door(at,rotation));
            site.DockFromFar=site.Kind==BuildingKind.FishingDock && DockEntrance(at,rotation)==FarBank(at,rotation);
            site.Cell=at;site.Rotation=rotation;Cottages.Insert(index,site);
            var after=Reachable(YardAccess,Blocked);
            if(!after.Contains(site.Entrance) || access.Any(c=>!after.Contains(c)))return "Moving this building would disconnect a resident, workplace or resource. Keep another route open.";
            return null;
        }
        finally
        {
            Cottages.Remove(site);site.Cell=original;site.Rotation=facing;site.BridgeFromFar=bridgeFar;site.DockFromFar=dockFar;Cottages.Insert(index,site);
        }
    }
    public bool MoveBuilding(int id,Cell at,int rotation)
    {
        if(RelocationProblem(id,at,rotation)!=null)return false;
        var site=Cottages.Single(c=>c.Id==id);if(site.Cell==at && site.Rotation==rotation)return true;
        if(!Creative && IsArrangementCourt && Neighborhood!.Arrangement is {BuildingId:null} trial)
        {trial.BuildingId=id;trial.Original=site.Cell;trial.Rotation=site.Rotation;}
        foreach(var person in People.Where(p=>MoveAffects(p,site)).ToArray())Interrupt(person);
        int index=Cottages.IndexOf(site);Cottages.RemoveAt(index);
        site.BridgeFromFar=site.Kind==BuildingKind.Bridge && !Accessible(Door(at,rotation));
        site.DockFromFar=site.Kind==BuildingKind.FishingDock && DockEntrance(at,rotation)==FarBank(at,rotation);
        site.Cell=at;site.Rotation=rotation;Cottages.Insert(index,site);
        if(IsVegetablePlot(site.Kind) && site.Harvest==0)
        {
            site.Planted=false;site.Growth=0;
            History.Add("Kitchen garden moved: sow a fresh crop here. Ripe produce and stored food are retained.");
        }
        RemovePaths(Footprint(at,rotation,site.Kind));
        ManagedWoodland.ExceptWith(Footprint(at,rotation,site.Kind).Append(site.Entrance));
        if(site.Kind==BuildingKind.Bridge){ManagedWoodland.Remove(Door(at,rotation));ManagedWoodland.Remove(FarBank(at,rotation));}
        if(site.Boat is {} boat){boat.Position=site.Launch.Point;boat.Heading=rotation*MathF.PI/2+(site.DockFromFar?MathF.PI:0);}
        ReconcileHomes();
        foreach(var person in People.Where(p=>p.Route.Count>0))SetRoute(person,person.StorageId==id && person.Task==Work.ToStockpile?site.Entrance:person.Destination);
        History.Add($"Moved {Buildings.Get(site.Kind).Name} {id} to {at.X}, {at.Z}.");_retry=0;return true;
    }
    public Cell RelocationEntrance(int id,Cell at,int rotation)
    {
        var site=Cottages.Single(c=>c.Id==id);int index=Cottages.IndexOf(site);Cottages.RemoveAt(index);
        try{return site.Kind==BuildingKind.FishingDock?DockEntrance(at,rotation):site.Kind==BuildingKind.Bridge?BridgeEntrance(at,rotation):Door(at,rotation);}
        finally{Cottages.Insert(index,site);}
    }
}

using System;
using System.Linq;
namespace Inlanders.Simulation;
public sealed record GroupPlan(World? Result,string? Problem);
public sealed partial class World
{
    public bool GroupEligible(Cottage site)=>PublicPlace!=null && site.Complete && !site.DemolitionRequested && (Buildings.Get(site.Kind).Beds>0 || site.Kind==BuildingKind.Farm || IsVegetablePlot(site.Kind)) && !site.ImprovementRequested;
    public Cell[] GroupPaths(int[] ids)
    {
        var sites=Cottages.Where(c=>ids.Contains(c.Id)).ToArray();if(sites.Length<2)return Array.Empty<Cell>();
        var ground=sites.SelectMany(c=>Footprint(c).Append(c.Entrance)).ToArray();
        var protectedGround=Cottages.Where(c=>!ids.Contains(c.Id)).SelectMany(c=>Footprint(c).Append(c.Entrance).Concat(ClaimedHomeYardPlaces(c)))
            .Concat(SharedPlaces.SelectMany(c=>c.Places.Append(c.Center))).Append(YardAccess).ToHashSet();
        int minX=ground.Min(c=>c.X),maxX=ground.Max(c=>c.X),minZ=ground.Min(c=>c.Z),maxZ=ground.Max(c=>c.Z);
        return Paths.Where(c=>c.X>=minX && c.X<=maxX && c.Z>=minZ && c.Z<=maxZ && !protectedGround.Contains(c)).OrderBy(c=>c.Z).ThenBy(c=>c.X).ToArray();
    }
    public GroupPlan PreviewGroup(int[] ids,Cell target,int turn=0,bool carryPaths=false)
    {
        if(PublicPlace==null || Food.Celebrating)return new(null,"Choose an ordinary village outside a gathering.");
        if(ids.Length<2 || ids.Length>16 || ids.Distinct().Count()!=ids.Length || turn is <0 or >3)return new(null,"Choose two to sixteen different finished homes or fields.");
        var homes=ids.Select(id=>Cottages.FirstOrDefault(c=>c.Id==id)).ToArray();
        if(homes.Any(c=>c==null || !GroupEligible(c)))return new(null,"Choose finished homes or fields; finish or cancel furnishing first.");
        var copy=LoadJson(SaveJson());var problem=copy.ApplyGroupGeometry(ids,target,turn,carryPaths);
        return problem==null?new(copy,null):new(null,problem);
    }
    private string? ApplyGroupGeometry(int[] ids,Cell target,int turn,bool carryPaths)
    {
        var order=Cottages.ToArray();var selected=ids.Select(id=>Cottages.Single(c=>c.Id==id)).ToArray();var pivot=selected[0].Cell;
        if(target==pivot && turn==0)return null;
        var carried=carryPaths?GroupPaths(ids):Array.Empty<Cell>();
        var carriedSet=carried.ToHashSet();var offsets=new[]{new Cell(1,0),new Cell(-1,0),new Cell(0,1),new Cell(0,-1)};
        var junctions=carried.Where(c=>offsets.Any(d=>Paths.Contains(new(c.X+d.X,c.Z+d.Z)) && !carriedSet.Contains(new(c.X+d.X,c.Z+d.Z)))).ToHashSet();
        var before=Reachable(YardAccess,Blocked);
        var access=Cottages.Select(c=>c.Entrance).Concat(Trees.Select(t=>t.Access)).Concat(Bushes.Select(b=>b.Access))
            .Concat(Map.StoneDeposits.Select(d=>d.Access)).Concat(Map.Wildlife.Select(h=>h.Cell)).Where(before.Contains).ToHashSet();
        foreach(var site in selected)access.Remove(site.Entrance);
        foreach(var person in People.Where(p=>selected.Any(s=>MoveAffects(p,s) || p.HomeId==s.Id && p.QuietSharedCenter!=null)).ToArray())Interrupt(person);
        foreach(var site in selected)Cottages.Remove(site);
        foreach(var site in selected)
        {
            site.Cell=RotateOffset(target,site.Cell.X-pivot.X,site.Cell.Z-pivot.Z,turn);site.Rotation=(site.Rotation+turn)%4;
            var problem=PlacementProblem(site.Cell,site.Rotation,site.Kind,site.PlotRows);if(problem!=null)return problem;
            Cottages.Add(site);
        }
        Cottages.Clear();Cottages.AddRange(order);
        foreach(var site in selected)
        {
            if(site.Improved && (YardClaimProblem(site,site.YardSide) is {} conflict || HomeYardPlaces(site).Length!=2))return "A furnished yard needs two clear places in the new arrangement.";
            if((site.Kind==BuildingKind.Farm || IsVegetablePlot(site.Kind)) && site.Harvest==0){site.Planted=false;site.Growth=0;}
            RemovePaths(Footprint(site));ManagedWoodland.ExceptWith(Footprint(site).Append(site.Entrance));
        }
        if(carried.Length>0)
        {
            var moved=carried.Select(c=>RotateOffset(target,c.X-pivot.X,c.Z-pivot.Z,turn)).ToArray();
            if(moved.Any(c=>PathProblem(c)!=null))return "A carried path meets blocked ground. Change the arrangement or leave paths in place.";
            RemovePaths(carried.Where(c=>!junctions.Contains(c)));foreach(var c in moved){Paths.Add(c);ManagedWoodland.Remove(c);}PathsRevision++;
        }
        var after=Reachable(YardAccess,Blocked);
        if(access.Concat(selected.Select(c=>c.Entrance)).Concat(People.Select(At)).Concat(People.Where(p=>p.Route.Count>0).Select(p=>p.Destination)).Any(c=>!after.Contains(c)))return "Keep residents, resources and building entrances connected.";
        foreach(var p in People.Where(p=>p.Route.Count>0))SetRoute(p,p.Destination);
        ReconcileHomes();_retry=0;History.Add($"Rearranged {selected.Length} places together. Households and furnishings stay; growing crops restart, ripe harvest and stored goods remain.");
        Validate();ValidateMapOccupancy();return null;
    }
}

using System;
using System.Linq;
namespace Inlanders.Simulation;
public sealed record GroupPlan(World? Result,string? Problem);
public sealed partial class World
{
    public bool GroupEligible(Cottage site)=>PublicPlace!=null && site.Complete && !site.DemolitionRequested && Buildings.Get(site.Kind).Beds>0 && !site.ImprovementRequested;
    public GroupPlan PreviewGroup(int[] ids,Cell target,int turn=0)
    {
        if(PublicPlace==null || Food.Celebrating)return new(null,"Choose an ordinary village outside a gathering.");
        if(ids.Length<2 || ids.Length>16 || ids.Distinct().Count()!=ids.Length || turn is <0 or >3)return new(null,"Choose two to sixteen different finished homes.");
        var homes=ids.Select(id=>Cottages.FirstOrDefault(c=>c.Id==id)).ToArray();
        if(homes.Any(c=>c==null || !GroupEligible(c)))return new(null,"Choose finished homes; finish or cancel furnishing first.");
        var copy=LoadJson(SaveJson());var problem=copy.ApplyGroupGeometry(ids,target,turn);
        return problem==null?new(copy,null):new(null,problem);
    }
    private string? ApplyGroupGeometry(int[] ids,Cell target,int turn)
    {
        var order=Cottages.ToArray();var selected=ids.Select(id=>Cottages.Single(c=>c.Id==id)).ToArray();var pivot=selected[0].Cell;
        if(target==pivot && turn==0)return null;
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
            RemovePaths(Footprint(site));ManagedWoodland.ExceptWith(Footprint(site).Append(site.Entrance));
        }
        var after=Reachable(YardAccess,Blocked);
        if(access.Concat(selected.Select(c=>c.Entrance)).Concat(People.Select(At)).Concat(People.Where(p=>p.Route.Count>0).Select(p=>p.Destination)).Any(c=>!after.Contains(c)))return "Keep residents, resources and building entrances connected.";
        foreach(var p in People.Where(p=>p.Route.Count>0))SetRoute(p,p.Destination);
        ReconcileHomes();_retry=0;History.Add($"Rearranged {selected.Length} homes together. Households and furnishings stay with their homes.");
        Validate();ValidateMapOccupancy();return null;
    }
}

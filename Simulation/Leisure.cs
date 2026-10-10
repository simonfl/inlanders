using System;
using System.Linq;

namespace Inlanders.Simulation;

public sealed partial class World
{
    private bool ClaimLeisure(Villager v)
    {
        if (Food.Time < Math.Max(15 + v.Id % 4 * 2, v.NextLeisureTime) || v.Carried != 0) return false;
        var reserved = People.Where(p => p.LeisureSiteId != null || p.Task is Work.ToRest or Work.Resting).Select(p => p.Destination).ToHashSet();
        var choice=Cottages.Where(c=>c.Complete && !c.DemolitionRequested && Buildings.Get(c.Kind).RecreationSlots>0 &&
                People.Count(p=>p.LeisureSiteId==c.Id)<Buildings.Get(c.Kind).RecreationSlots)
            .SelectMany(square=>Map.Land.Where(c=>(c.Point-square.Entrance.Point).LengthSquared()<=4 && !Blocked(c) &&
                !MealSpotReserved(c) && !ComfortSpotReserved(c) && !reserved.Contains(c))
                .Select(c=>new{Square=square,Cell=c,Cost=TravelCost(At(v),c)}))
            .Where(x=>x.Cost<int.MaxValue).OrderBy(x=>x.Cost).ThenBy(x=>x.Square.Id).ThenBy(x=>x.Cell.Z).ThenBy(x=>x.Cell.X).FirstOrDefault();
        if(choice!=null)
        {
            v.LeisureSiteId=choice.Square.Id;
            Go(v,choice.Cell,Work.ToLeisure,$"Heading to {Buildings.Get(choice.Square.Kind).Name} for a break");
            return true;
        }
        return false;
    }

    private void ValidateLeisure()
    {
        var visitors = People.Where(v => v.LeisureSiteId != null).ToArray();
        if (visitors.Select(v => v.Destination).Distinct().Count() != visitors.Length ||
            visitors.GroupBy(v => v.LeisureSiteId).Any(g => g.Count() > Buildings.Get(Cottages.Single(c=>c.Id==g.Key).Kind).RecreationSlots))
            throw new InvalidOperationException("Overbooked leisure spots");
        foreach (var v in People)
        {
            if(v.LastLeisureSiteId is int previous && (previous<1 || previous>=_nextSite || v.LastLeisureTime==null)) throw new InvalidOperationException("Invalid previous recreation venue");
            if (!float.IsFinite(v.NextLeisureTime) || v.NextLeisureTime < 0 || v.LeisureVisits < 0 ||
                (v.LeisureSiteId != null) != (v.Task is Work.ToLeisure or Work.Leisure))
                throw new InvalidOperationException("Invalid leisure state");
            if (v.LeisureSiteId is int id && (!Cottages.Any(c => c.Id == id && c.Complete && !c.DemolitionRequested && Buildings.Get(c.Kind).RecreationSlots>0 &&
                    (c.Entrance.Point - v.Destination.Point).LengthSquared() <= 4) || Blocked(v.Destination) ||
                    v.Carried != 0 || v.Reserved != 0 || v.WorkplaceId != null || v.SiteId != null || v.TreeId != null))
                throw new InvalidOperationException("Invalid recreation visit");
        }
    }
}

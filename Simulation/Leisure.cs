using System;
using System.Linq;

namespace Inlanders.Simulation;

public sealed partial class World
{
    private bool ClaimLeisure(Villager v)
    {
        if (Food.Time < Math.Max(15 + v.Id % 4 * 2, v.NextLeisureTime) || v.Carried != 0) return false;
        foreach (var square in Cottages.Where(c => c.Complete && c.Kind == BuildingKind.Square)
                     .OrderBy(c => (c.Entrance.Point - v.Position).LengthSquared()).ThenBy(c => c.Id))
        {
            if (People.Count(p => p.LeisureSiteId == square.Id) >= 4) continue;
            var reserved = People.Where(p => p.LeisureSiteId != null).Select(p => p.Destination).ToHashSet();
            var spot = Map.Land.Where(c => (c.Point - square.Entrance.Point).LengthSquared() <= 4 && !Blocked(c) && !reserved.Contains(c))
                .OrderBy(c => (c.Point - square.Entrance.Point).LengthSquared()).ThenBy(c => c.Z).ThenBy(c => c.X)
                .Cast<Cell?>().FirstOrDefault(c => FindPath(At(v), c!.Value, Blocked) != null);
            if (spot == null) continue;
            v.LeisureSiteId = square.Id;
            Go(v, spot.Value, Work.ToLeisure, "Heading to the square for a break");
            return true;
        }
        return false;
    }

    private void ValidateLeisure()
    {
        var visitors = People.Where(v => v.LeisureSiteId != null).ToArray();
        if (visitors.Select(v => v.Destination).Distinct().Count() != visitors.Length ||
            visitors.GroupBy(v => v.LeisureSiteId).Any(g => g.Count() > 4))
            throw new InvalidOperationException("Overbooked leisure spots");
        foreach (var v in People)
        {
            if (!float.IsFinite(v.NextLeisureTime) || v.NextLeisureTime < 0 || v.LeisureVisits < 0 ||
                (v.LeisureSiteId != null) != (v.Task is Work.ToLeisure or Work.Leisure))
                throw new InvalidOperationException("Invalid leisure state");
            if (v.LeisureSiteId is int id && (!Cottages.Any(c => c.Id == id && c.Complete && c.Kind == BuildingKind.Square &&
                    (c.Entrance.Point - v.Destination.Point).LengthSquared() <= 4) || Blocked(v.Destination) ||
                    v.Carried != 0 || v.Reserved != 0 || v.WorkplaceId != null || v.SiteId != null || v.TreeId != null))
                throw new InvalidOperationException("Invalid square visit");
        }
    }
}

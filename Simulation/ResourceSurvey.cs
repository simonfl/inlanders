using System;
using System.Collections.Generic;
using System.Linq;

namespace Inlanders.Simulation;

public enum SourceKind { Stone, Fish, Woodland, Berries }
public readonly record struct SourceKey(SourceKind Kind,int Id);
public sealed record ResourceSource(SourceKey Key,Cell Cell,string Name);
public sealed record ResourceSurvey(ResourceSource Source,string Detail,string WorkplaceHeading,int[] Workplaces);

public sealed partial class World
{
    public IEnumerable<ResourceSource> ResourceSources() =>
        Map.StoneDeposits.Select(d=>new ResourceSource(new(SourceKind.Stone,d.Id),d.Cell,$"Stone outcrop {d.Id}"))
        .Concat(Map.FishingGrounds.Select(h=>new ResourceSource(new(SourceKind.Fish,h.Id),h.Cell,h.Name)))
        .Concat(Map.Wildlife.Select(h=>new ResourceSource(new(SourceKind.Woodland,h.Id),h.Cell,WoodsName(h))))
        .Concat(Bushes.Select(b=>new ResourceSource(new(SourceKind.Berries,b.Id),b.Cell,$"Berry patch {b.Id}")));

    public ResourceSurvey? ReadResourceSurvey(SourceKey key)
    {
        var source=ResourceSources().FirstOrDefault(s=>s.Key==key);
        if(source==null) return null;
        string At(Cell cell) => $"Location ({cell.X}, {cell.Z})";
        int[] Sites(Func<Cottage,bool> eligible) => Cottages.Where(c=>!c.DemolitionRequested && eligible(c)).OrderBy(c=>c.Id).Select(c=>c.Id).ToArray();
        switch(key.Kind)
        {
            case SourceKind.Berries:
                var bush=Bushes.Single(b=>b.Id==key.Id);int claimed=bush.Owner==null?0:Math.Min(2,bush.Ripe);
                return new(source,$"{At(bush.Cell)}\n\n{bush.Ripe-claimed} berries available · {claimed} being picked\n{bush.Ripe}/8 ripe berries · renewal up to7.5/minute.\nShared by all huts. A worker must reach the picking spot, gather and carry fruit back.\nPicking access ({bush.Access.X}, {bush.Access.Z}): {(Accessible(bush.Access)?"reachable from the yard":"no route from the yard")}",
                    "HUTS WITH A WALKING ROUTE",Sites(c=>c.Kind==BuildingKind.ForagerHut && FindPath(c.Entrance,bush.Access,Blocked)!=null));
            case SourceKind.Stone:
                var d=Map.StoneDeposits.Single(s=>s.Id==key.Id);
                return new(source,$"{At(d.Cell)}\n\n{AvailableDeposit(d)} stone available · {d.Remaining-AvailableDeposit(d)} reserved\n{d.Remaining} remaining of {d.Capacity} initial stone\n"+
                    $"{(d.Remaining==0?"Exhausted. ":"")}Finite deposit; no regrowth.\n\nWorking access ({d.Access.X}, {d.Access.Z}): {(Accessible(d.Access)?"reachable from the yard":"no route from the yard")}.\nCamps within 4 tiles share this deposit. Stone reaches central storage only after hauling.",
                    "CAMPS IN RANGE · access still required",Sites(c=>c.Kind==BuildingKind.Quarry && (c.Cell.Point-d.Cell.Point).LengthSquared()<=16));
            case SourceKind.Fish:
                var fish=Map.FishingGrounds.Single(s=>s.Id==key.Id);
                return new(source,$"{At(fish.Cell)}\n\n{AvailableFish(fish)} fish available · {fish.Available-AvailableFish(fish)} reserved\n{fish.Stock:0.#}/{fish.Capacity} habitat stock\nRecovery: +{fish.RegrowthPerSecond*60:0.#} fish/minute, up to capacity.\n\nDocks share this stock. Boats need a clear water route; bridges can obstruct it. A planned dock must be built first. Fish become food after unloading and pantry delivery.",
                    "DOCKS WITH A WATER ROUTE",Sites(c=>c.Kind==BuildingKind.FishingDock && FindBoatRoute(c.Launch,fish.Cell)!=null));
            default:
                var h=Map.Wildlife.Single(s=>s.Id==key.Id);
                return new(source,$"{At(h.Cell)}\n\n{AvailableGame(h)} game available · {ClaimedGame(h)} reserved\n{h.Stock:0.#}/{HabitatCapacity(h)} habitat stock\n{HabitatTrees(h)} mature trees within 5 tiles\nRecovery: +{HabitatRecovery(h):0.#} game/minute.\n\nTracking clearing: {(Accessible(h.Cell)?"reachable from the yard":"no route from the yard")}.\nLodges within 8 tiles share stock. Pause hunting for stock recovery; retain or regrow mature trees for capacity. Protect restoration planting orders with Preserve trees so loggers do not harvest them again after maturity. Reserved outings can finish after tree loss. Game becomes food after pantry delivery.",
                    "LODGES IN RANGE · access still required",Sites(c=>c.Kind==BuildingKind.HuntingLodge && (c.Cell.Point-h.Cell.Point).LengthSquared()<=64));
        }
    }
}

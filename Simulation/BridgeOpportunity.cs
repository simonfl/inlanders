using System;
using System.Linq;
namespace Inlanders.Simulation;
public sealed record BridgeOpportunity(int GroundTiles,int Workplaces,int Beds,int RipeFood,int StoredFood,int LostFishingConnections)
{
    public string Summary=>$"Opens {GroundTiles} ground tiles · reconnects {Workplaces} workplaces / {Beds} beds\n{RipeFood} ripe portions to collect · {StoredFood} stored portions become reachable"+(LostFishingConnections>0?$"\nCuts off {LostFishingConnections} dock-to-ground fishing routes. Boats cannot pass under bridges.":"");
}
public sealed partial class World
{
    public BridgeOpportunity? ReadBridgeOpportunity(Cell at,int rotation)
    {
        if(PlacementProblem(at,rotation,BuildingKind.Bridge)!=null)return null;
        var before=Reachable(YardAccess,Blocked);var after=Reachable(YardAccess,c=>c!=at && Blocked(c));
        var sites=Cottages.Where(c=>c.Complete && !c.DemolitionRequested && !before.Contains(c.Entrance) && after.Contains(c.Entrance)).ToArray();
        var openStores=FoodStores().ToHashSet();int lost=0;
        foreach(var dock in Cottages.Where(c=>c.Kind==BuildingKind.FishingDock && !c.DemolitionRequested))
        {
            var waterBefore=Reachable(dock.Launch,BoatBlocked);var waterAfter=Reachable(dock.Launch,c=>c==at || BoatBlocked(c));
            lost+=Map.FishingGrounds.Count(g=>waterBefore.Contains(g.Cell) && !waterAfter.Contains(g.Cell));
        }
        return new(after.Count(c=>!Map.Water.Contains(c) && !before.Contains(c)),sites.Count(c=>Buildings.Get(c.Kind).Beds==0),sites.Sum(c=>Buildings.Get(c.Kind).Beds),
            sites.Where(c=>ProductionOutput(c.Kind) is {} food && EdibleKinds.Contains(food)).Sum(c=>c.Harvest),
            sites.Where(c=>openStores.Contains(c.Id)).Sum(c=>c.PantryFood.Sum()),lost);
    }
}

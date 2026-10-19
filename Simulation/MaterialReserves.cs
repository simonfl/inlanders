using System;
using System.Linq;
namespace Inlanders.Simulation;
public sealed partial class World
{
    public int GrainReserveTarget => Math.Max(6,(Population*2+BreadPerGrain-1)/BreadPerGrain);
    public int MaterialStockTarget(Resource resource)
    {
        if(resource==Resource.Logs)return TimberStockTarget;
        if(resource is not (Resource.Planks or Resource.Stone))throw new ArgumentException("Not a building material");
        return 24+Cottages.Where(c=>!c.Complete && !c.ConstructionPaused && !c.DemolitionRequested && (resource==Resource.Stone || c.Material==resource)).Sum(c=>c.Remaining(resource))+
            (resource==Resource.Planks?Cottages.Where(c=>c.ImprovementRequested && !c.DemolitionRequested).Sum(c=>Math.Max(0,ComfortCost(c)-c.ImprovementPlanks-ComfortIncoming(c))):0);
    }
}

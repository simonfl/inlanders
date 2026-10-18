using System.Linq;
namespace Inlanders.Simulation;
public sealed partial class World
{
    public static BuildingKind? SupplyProducerKind(Resource material)=>material switch
    {Resource.Grain=>BuildingKind.Farm,Resource.Planks=>BuildingKind.Sawmill,Resource.Stone=>BuildingKind.Quarry,_=>null};
    // This is an inspectable source of future output, not a claim that stock exists.
    public Cottage? ReadSupplyProducer(int consumer,Resource material)
    {
        var destination=Cottages.FirstOrDefault(c=>c.Id==consumer);
        if(destination==null || SupplyProducerKind(material) is not {} kind)return null;
        return Cottages.Where(c=>c.Id!=consumer && c.Kind==kind && !c.DemolitionRequested)
            .Select(c=>(Site:c,Cost:TravelCost(c.Entrance,destination.Entrance)))
            .Where(c=>c.Cost<int.MaxValue).OrderByDescending(c=>c.Site.Complete)
            .ThenBy(c=>c.Site.Complete?c.Site.WorkPaused:c.Site.ConstructionPaused)
            .ThenBy(c=>c.Cost).ThenBy(c=>c.Site.Id).Select(c=>c.Site).FirstOrDefault();
    }
}

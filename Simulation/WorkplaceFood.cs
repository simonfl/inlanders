using System.Linq;

namespace Inlanders.Simulation;

public sealed partial class World
{
    public const int WorkplaceFoodReserve=4;
    public bool HasWorkplaceFood=>Neighborhood?.WorkplaceFood==true;
    // Bounded workflow comparison: these are the three edible producers used by the river slice.
    public bool IsWorkplaceFoodStore(Cottage site)=>HasWorkplaceFood && site.Kind is BuildingKind.ForagerHut or BuildingKind.VegetableGarden or BuildingKind.Bakery;
    public static World NewWorkplaceFoodExperiment()
    {
        var world=NewNeighborhoodLandscapeExperiment();world.Neighborhood!.WorkplaceFood=true;return world;
    }
    private bool DeliverWorkplaceFood(Villager person)
    {
        if(!EdibleKinds.Contains(person.Cargo) || person.WorkplaceId is not int id)return false;
        var site=Cottages.FirstOrDefault(c=>c.Id==id && IsWorkplaceFoodStore(c) && c.Complete && !c.DemolitionRequested);
        if(site==null || PantrySpace(id)<person.Carried)return false;
        person.FoodDestinationId=id;
        Go(person,site.Entrance,Work.ToPantry,$"Storing {person.Carried} {person.Cargo} at {FoodStoreName(id)}");return true;
    }
    private bool ClaimWorkplaceDistribution(Villager person,Cottage destination,int need)
    {
        foreach(var source in Cottages.Where(c=>IsWorkplaceFoodStore(c) && c.Complete && !c.DemolitionRequested)
            .OrderBy(c=>TravelCost(At(person),c.Entrance)+TravelCost(c.Entrance,destination.Entrance)).ThenBy(c=>c.Id))
        {
            int surplus=EdibleKinds.Sum(k=>FoodAvailableAt(source.Id,k))-WorkplaceFoodReserve;
            if(surplus<=0)continue;
            var kind=EdibleKinds.OrderByDescending(k=>FoodAvailableAt(source.Id,k)).First();
            int amount=System.Math.Min(4,System.Math.Min(need,System.Math.Min(surplus,FoodAvailableAt(source.Id,kind))));
            if(amount<=0 || !Accessible(source.Entrance))continue;
            person.Cargo=kind;person.PantryReserved=amount;person.FoodSourceId=source.Id;
            person.FoodDestinationId=destination.Id;person.FoodTransfer=true;
            Go(person,source.Entrance,Work.ToFoodPickup,$"Collecting {amount} {kind} at {FoodStoreName(source.Id)} for pantry {destination.Id}");return true;
        }
        return false;
    }
}

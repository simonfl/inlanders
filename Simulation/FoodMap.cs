using System.Linq;
namespace Inlanders.Simulation;

public sealed record FoodMapStore(int? Id, string Name, Cell Access, int Available, int Claimed, int Incoming, bool Paused, bool Reachable);
public sealed partial class World
{
    public FoodMapStore[] ReadFoodMap()
    {
        var reachable=Reachable(YardAccess,Blocked);
        return new int?[]{null}.Concat(Cottages.Where(c=>c.Complete && !c.DemolitionRequested && IsFoodStore(c) && (!IsWelcomeStore(c) || c.Id==Neighborhood?.VenueId || c.PantryFood.Sum()>0)).Select(c=>(int?)c.Id))
            .Select(id=>new FoodMapStore(id,FoodStoreName(id),FoodAccess(id),EdibleKinds.Sum(k=>FoodAvailableAt(id,k)),
                EdibleKinds.Sum(k=>FoodReservedAt(id,k)),id is int n?FoodIncoming(n):People.Where(p=>p.FoodDestinationId==null && p.Task is Work.ToPantry or Work.ToFoodPickup && EdibleKinds.Contains(p.Cargo)).Sum(p=>p.Task==Work.ToFoodPickup?p.PantryReserved:p.Carried),
                id is int p && Cottages.Single(c=>c.Id==p).WorkPaused,reachable.Contains(FoodAccess(id)))).ToArray();
    }
    public bool IsFoodRoute(int workerId)
    {
        var p=People.Single(v=>v.Id==workerId);
        return p.Task is Work.ToBush or Work.ToFarm or Work.ToGrain or Work.ToOven or Work.ToBread or Work.ToPantry or Work.ToFoodPickup or Work.ToMealSupply or Work.ToMealSeat or Work.ReturnMeal or Work.ToHunt or Work.ToDock || PassengerBoat(p)!=null;
    }
}

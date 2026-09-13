using System;
using System.Collections.Generic;
using System.Linq;

namespace Inlanders.Simulation;

public sealed partial class World
{
    public const int PantryCapacity=24;
    private Cottage Pantry(int id) => Cottages.Single(c=>c.Id==id && IsFoodStore(c) && c.Complete);
    private IEnumerable<int?> FoodStores() => new int?[]{null}.Concat(Cottages.Where(c=>(c.Kind==BuildingKind.Pantry && !c.WorkPaused || IsWorkplaceFoodStore(c)) && c.Complete && !c.DemolitionRequested).Select(c=>(int?)c.Id));
    public Cell FoodAccess(int? id) => id is int n?Pantry(n).Entrance:YardAccess;
    public int FoodAt(int? id,Resource kind) => id is int n ? Pantry(n).PantryFood[Array.IndexOf(EdibleKinds,kind)] : CentralFood(kind);
    public int StoredFood(Resource kind) => CentralFood(kind)+Cottages.Sum(c=>c.PantryFood[Array.IndexOf(EdibleKinds,kind)]);
    public int EdibleStored => EdibleKinds.Sum(StoredFood);
    public int FoodReservedAt(int? id,Resource kind) => People.Count(p=>p.Meal is {Reserved:true} r && r.SourceId==id && r.Kind==kind)+
        People.Where(p=>p.Task==Work.ToFoodPickup && p.FoodSourceId==id && p.Cargo==kind).Sum(p=>p.PantryReserved);
    public int FoodAvailableAt(int? id,Resource kind) => FoodAt(id,kind)-FoodReservedAt(id,kind);
    public int FoodIncoming(int id) => People.Where(p=>p.FoodDestinationId==id).Sum(p=>p.Task==Work.ToFoodPickup?p.PantryReserved:p.Task==Work.ToPantry?p.Carried:0);
    private int PantrySpace(int id) => PantryCapacity-Pantry(id).PantryFood.Sum()-FoodIncoming(id);
    private void ChangeFoodAt(int? id,Resource kind,int amount)
    {
        if(id is int n) Pantry(n).PantryFood[Array.IndexOf(EdibleKinds,kind)]+=amount;
        else ChangeCentralFood(kind,amount);
    }
    public bool SetPantryTarget(int id,int target)
    {
        var pantry=Cottages.FirstOrDefault(c=>c.Id==id && c.Kind==BuildingKind.Pantry && !c.DemolitionRequested);
        if(pantry==null || target<0 || target>PantryCapacity) return false;
        pantry.PantryTarget=target; _retry=0; return true;
    }
    private void DeliverFood(Villager person)
    {
        person.FoodDestinationId=null;
        if(DeliverLocalGrain(person))return;
        if(DeliverWorkplaceFood(person))return;
        if(EdibleKinds.Contains(person.Cargo))
        {
            var destination=FoodStores().Where(id=>id==null || !IsWorkplaceFoodStore(Pantry(id.Value)) && PantrySpace(id.Value)>=person.Carried)
                .OrderBy(id=>TravelCost(At(person),FoodAccess(id))).ThenBy(id=>id??0).First();
            person.FoodDestinationId=destination;
        }
        Go(person,FoodAccess(person.FoodDestinationId),Work.ToPantry,$"Carrying {person.Carried} {person.Cargo} to "+(person.FoodDestinationId is int id?$"pantry {id}":"the central pantry"));
    }
    private bool ClaimPantryHauling(Villager person)
    {
        if(ClaimWelcomeDelivery(person))return true;
        int waiting=People.Count(p=>p.Meal is {Reserved:false,Carrying:false,Eaten:false,Closed:false});
        int surplus=Math.Max(0,EdibleKinds.Sum(k=>FoodAvailableAt(null,k))-waiting);
        foreach(var pantry in Cottages.Where(c=>c.Kind==BuildingKind.Pantry && c.Complete && !c.DemolitionRequested && !c.WorkPaused).OrderByDescending(c=>c.Priority).ThenBy(c=>c.Id))
        {
            int need=Math.Min(PantrySpace(pantry.Id),pantry.PantryTarget-pantry.PantryFood.Sum()-FoodIncoming(pantry.Id));
            if(need<=0)continue;
            if(HasWorkplaceFood && ClaimWorkplaceDistribution(person,pantry,need))return true;
            if(surplus==0) continue;
            var foods=EdibleKinds.Where(k=>FoodAvailableAt(null,k)>0).OrderBy(k=>FoodAt(pantry.Id,k)).ToArray();
            if(foods.Length==0) continue;
            person.Cargo=foods[0]; person.PantryReserved=Math.Min(4,Math.Min(surplus,Math.Min(need,FoodAvailableAt(null,person.Cargo))));
            person.FoodSourceId=null; person.FoodDestinationId=pantry.Id; person.FoodTransfer=true;
            Go(person,YardAccess,Work.ToFoodPickup,$"Collecting {person.PantryReserved} {person.Cargo} for pantry {pantry.Id}"); return true;
        }
        foreach(var pantry in Cottages.Where(c=>IsFoodStore(c) && c.Complete && !c.DemolitionRequested && c.Id!=Neighborhood?.VenueId).OrderBy(c=>TravelCost(At(person),c.Entrance)))
        {
            int spare=EdibleKinds.Sum(k=>FoodAvailableAt(pantry.Id,k))-(IsWorkplaceFoodStore(pantry)?WorkplaceFoodReserve:pantry.PantryTarget);
            if(spare<=0) continue;
            var kind=EdibleKinds.OrderByDescending(k=>FoodAvailableAt(pantry.Id,k)).First();
            person.Cargo=kind; person.PantryReserved=Math.Min(4,Math.Min(spare,FoodAvailableAt(pantry.Id,kind)));
            person.FoodSourceId=pantry.Id; person.FoodDestinationId=null; person.FoodTransfer=true;
            Go(person,pantry.Entrance,Work.ToFoodPickup,$"Returning {person.PantryReserved} surplus {kind} from pantry {pantry.Id}"); return true;
        }
        return false;
    }
    private void PickupPantryShipment(Villager person)
    {
        ChangeFoodAt(person.FoodSourceId,person.Cargo,-person.PantryReserved); person.Carried=person.PantryReserved; person.PantryReserved=0; person.FoodSourceId=null;
        Go(person,FoodAccess(person.FoodDestinationId),Work.ToPantry,person.FoodDestinationId is int id?$"Supplying {FoodStoreName(id)}":"Returning surplus food to the central pantry");
    }
    private void ClosePantry(int id)
    {
        foreach(var p in People.Where(p=>p.FoodDestinationId==id || p.FoodSourceId==id || p.Meal is {Reserved:true} r && r.SourceId==id).ToArray()) Interrupt(p);
    }
    private void ValidatePantries()
    {
        foreach(var c in Cottages)
        {
            if(c.PantryFood==null || c.PantryFood.Length!=EdibleKinds.Length || c.PantryFood.Any(n=>n<0) || c.PantryTarget<0 || c.PantryTarget>PantryCapacity ||
                !IsFoodStore(c) && c.PantryFood.Any(n=>n!=0) || c.PantryFood.Sum()+FoodIncoming(c.Id)>PantryCapacity)
                throw new InvalidOperationException("Invalid pantry stock/capacity");
            if(IsFoodStore(c) && c.Complete) foreach(var kind in EdibleKinds)
                if(FoodAvailableAt(c.Id,kind)<0) throw new InvalidOperationException("Pantry food overreserved");
        }
        foreach(var p in People)
        {
            if(p.FoodDestinationId!=null && p.Task is not (Work.ToFoodPickup or Work.ToPantry)) throw new InvalidOperationException("Orphan pantry destination");
            if(p.FoodSourceId is int source && (p.Task!=Work.ToFoodPickup || !Cottages.Any(c=>c.Id==source && IsFoodStore(c) && c.Complete && !c.DemolitionRequested))) throw new InvalidOperationException("Missing pantry source");
            if(p.FoodDestinationId is int id && !Cottages.Any(c=>c.Id==id && IsFoodStore(c) && c.Complete && !c.DemolitionRequested)) throw new InvalidOperationException("Missing pantry destination");
            if(p.PantryReserved<0 || p.PantryReserved>4 || (p.PantryReserved>0)!=(p.Task==Work.ToFoodPickup) || p.Task==Work.ToFoodPickup && (p.Carried!=0 || !EdibleKinds.Contains(p.Cargo) || p.FoodDestinationId==p.FoodSourceId || !p.FoodTransfer)) throw new InvalidOperationException("Invalid food shipment claim");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
namespace Inlanders.Simulation;

public sealed class SharedCommons
{
    public int? FirstDiner { get; set; }
    public Cell Center { get; set; }
    public Cell[] Places { get; set; }=Array.Empty<Cell>();
}
public sealed partial class World
{
    private List<SharedCommons>? SharedPlaceList=>Founding?.SharedPlaces??Neighborhood?.SharedPlaces;
    public IReadOnlyList<SharedCommons> SharedPlaces=>SharedPlaceList is {} list?list:Array.Empty<SharedCommons>();
    public SharedCommons? Commons=>SharedPlaces.FirstOrDefault();
    public SharedCommons? SharedPlaceAt(Cell cell)=>SharedPlaces.FirstOrDefault(c=>c.Center==cell || c.Places.Contains(cell));
    public Cell[] CommonsPlaces(Cell center,int seats=6,Cell? replacing=null)
    {
        if(seats is not (2 or 4 or 6))return Array.Empty<Cell>();
        var domestic=Cottages.SelectMany(ClaimedHomeYardPlaces).ToHashSet();
        var other=SharedPlaces.Where(c=>c.Center!=replacing).SelectMany(c=>c.Places.Append(c.Center)).ToHashSet();
        var oldSeats=SharedPlaces.Where(c=>c.Center==replacing).SelectMany(c=>c.Places).ToHashSet();
        if(!Map.Contains(center) || Blocked(center) || domestic.Contains(center) || other.Contains(center))return Array.Empty<Cell>();
        var reachable=Reachable(YardAccess,Blocked);
        return Map.Land.Where(c=>(c.Point-center.Point).LengthSquared() is >=2 and <=8 && !Blocked(c) && reachable.Contains(c) && !domestic.Contains(c) && !other.Contains(c) &&
            !(Gathering is {Active:true} g && g.Seats.Values.Contains(c)) && !ComfortSpotReserved(c) &&
            !People.Any(p=>(p.Meal is {Reserved:true} or {Carrying:true}) && p.Meal.Seat==c && !oldSeats.Contains(c) || (p.Task is Work.ToRest or Work.Resting || p.LeisureSiteId!=null) && p.Destination==c))
            .OrderBy(c=>(c.Point-center.Point).LengthSquared()).ThenBy(c=>c.Z).ThenBy(c=>c.X).Take(seats).ToArray();
    }
    public bool CanArrangeCommons=>Founding?.RiverFarmstead==true || IsArrangementCourt || Neighborhood?.Complete==true;
    public bool CommonsFoodNearby(Cell center)=>FoodStores().Any(id=>(FoodAccess(id).Point-center.Point).LengthSquared()<=64 && EdibleKinds.Any(k=>FoodAvailableAt(id,k)>0) && FindPath(FoodAccess(id),center,Blocked)!=null);
    public string? CommonsProblem(Cell center,int seats=6,Cell? replacing=null)=>!CanArrangeCommons?"Welcome the newcomers first.":seats is not (2 or 4 or 6)?"Choose two, four or six places.":CommonsPlaces(center,seats,replacing).Length<seats?$"Choose open ground with {seats} reachable places, clear of other yards and shared places.":null;
    public bool SetCommons(Cell center,int seats=6)=>Commons is {} current?MoveCommons(current.Center,center,seats):AddCommons(center,seats);
    public bool AddCommons(Cell center,int seats=6)
    {
        if(CommonsProblem(center,seats)!=null)return false;
        SharedPlaceList!.Add(new(){Center=center,Places=CommonsPlaces(center,seats)});return true;
    }
    public bool MoveCommons(Cell original,Cell center,int seats=6)
    {
        var place=SharedPlaces.FirstOrDefault(c=>c.Center==original);
        if(place==null || CommonsProblem(center,seats,original)!=null)return false;
        var places=CommonsPlaces(center,seats,original);
        if(original==center && place.Places.SequenceEqual(places))return true;
        InterruptCommonsMeals(place);place.Center=center;place.Places=places;place.FirstDiner=null;return true;
    }
    private void InterruptCommonsMeals(SharedCommons place)
    {
        foreach(var p in People.Where(p=>p.Meal?.Commons==true && place.Places.Contains(p.Meal.Seat)).ToArray())
        {p.Meal!.Commons=false;InterruptMeal(p);}
    }
    public bool RemoveCommons(Cell center)
    {
        var place=SharedPlaces.FirstOrDefault(c=>c.Center==center);if(place==null)return false;
        InterruptCommonsMeals(place);SharedPlaceList!.Remove(place);return true;
    }
    public bool RemoveCommons()
    {
        if(SharedPlaces.Count==0)return false;
        foreach(var place in SharedPlaces.ToArray())RemoveCommons(place.Center);return true;
    }
    private Cell? AvailableCommonsPlace(Cell supply)=>SharedPlaces.Where(c=>(supply.Point-c.Center.Point).LengthSquared()<=64).SelectMany(c=>c.Places)
        .Where(c=>!People.Any(p=>(p.Meal is {Reserved:true} or {Carrying:true}) && p.Meal.Seat==c) && !Blocked(c) && FindPath(supply,c,Blocked)!=null)
        .OrderBy(c=>TravelCost(supply,c)).ThenBy(c=>c.Z).ThenBy(c=>c.X).Cast<Cell?>().FirstOrDefault();
    private void ValidateCommons()
    {
        if((Founding!=null || Neighborhood!=null) && SharedPlaceList==null)throw new InvalidOperationException("Missing shared places");
        var claimed=new HashSet<Cell>();
        foreach(var c in SharedPlaces)
        {
            if(c==null || !CanArrangeCommons || c.FirstDiner is int diner && (diner<0 || diner>=Population) || !Map.Contains(c.Center) || Blocked(c.Center) || c.Places==null || c.Places.Length is not (2 or 4 or 6) || c.Places.Distinct().Count()!=c.Places.Length ||
                c.Places.Any(p=>!Map.Contains(p) || Blocked(p) || (p.Point-c.Center.Point).LengthSquared()>8 || FindPath(YardAccess,p,Blocked)==null) || !c.Places.Append(c.Center).All(claimed.Add))
                throw new InvalidOperationException("Invalid or overlapping shared places");
        }
        foreach(var p in People.Where(p=>p.Meal is {Commons:true,Reserved:true} or {Commons:true,Carrying:true}))
            if(!SharedPlaces.Any(c=>c.Places.Contains(p.Meal!.Seat)))throw new InvalidOperationException("Meal outside its shared place");
    }
}

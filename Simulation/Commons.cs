using System;
using System.Collections.Generic;
using System.Linq;
namespace Inlanders.Simulation;

public enum SharedPlaceLayout { Gathered, Line }
public sealed class SharedCommons
{
    public SharedPlaceLayout Layout { get; set; }
    public int Rotation { get; set; }
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
    public Cell[] CommonsPlaces(Cell center,int seats=6,Cell? replacing=null,SharedPlaceLayout layout=SharedPlaceLayout.Gathered,int rotation=0)
    {
        if(seats is not (2 or 4 or 6) || !Enum.IsDefined(layout) || rotation is <0 or >3)return Array.Empty<Cell>();
        var domestic=Cottages.SelectMany(ClaimedHomeYardPlaces).ToHashSet();
        var other=SharedPlaces.Where(c=>c.Center!=replacing).SelectMany(c=>c.Places.Append(c.Center)).ToHashSet();
        var oldSeats=SharedPlaces.Where(c=>c.Center==replacing).SelectMany(c=>c.Places).ToHashSet();
        if(!Map.Contains(center) || Blocked(center) || domestic.Contains(center) || other.Contains(center))return Array.Empty<Cell>();
        var reachable=Reachable(YardAccess,Blocked);
        bool Free(Cell c)=>Map.Contains(c) && !Blocked(c) && reachable.Contains(c) && !domestic.Contains(c) && !other.Contains(c) &&
            !(Gathering is {Active:true} g && g.Seats.Values.Contains(c)) && !ComfortSpotReserved(c) &&
            !People.Any(p=>(p.Meal is {Reserved:true} or {Carrying:true}) && p.Meal.Seat==c && !oldSeats.Contains(c) || (p.Task is Work.ToRest or Work.Resting || p.LeisureSiteId!=null) && p.Destination==c);
        if(layout==SharedPlaceLayout.Line)return Enumerable.Range(-seats/2,seats+1).Where(x=>x!=0).Select(x=>RotateOffset(center,x,1,rotation)).Where(Free).ToArray();
        Cell Local(Cell c)=>RotateOffset(new(0,0),c.X-center.X,c.Z-center.Z,(4-rotation)%4);
        return Map.Land.Where(c=>(c.Point-center.Point).LengthSquared() is >=2 and <=8 && Free(c))
            .OrderBy(c=>(c.Point-center.Point).LengthSquared()).ThenBy(c=>Local(c).Z).ThenBy(c=>Local(c).X).Take(seats).ToArray();
    }

    public bool CanArrangeCommons=>Founding?.RiverFarmstead==true || IsArrangementCourt || Neighborhood?.Complete==true;
    public bool CommonsFoodNearby(Cell center)=>FoodStores().Any(id=>(FoodAccess(id).Point-center.Point).LengthSquared()<=64 && EdibleKinds.Any(k=>FoodAvailableAt(id,k)>0) && FindPath(FoodAccess(id),center,Blocked)!=null);
    public string? CommonsProblem(Cell center,int seats=6,Cell? replacing=null,SharedPlaceLayout layout=SharedPlaceLayout.Gathered,int rotation=0)=>!CanArrangeCommons?"Welcome the newcomers first.":seats is not (2 or 4 or 6)?"Choose two, four or six places.":!Enum.IsDefined(layout) || rotation is <0 or >3?"Choose a seating layout and one of four directions.":CommonsPlaces(center,seats,replacing,layout,rotation).Length<seats?$"This arrangement needs {seats} clear, reachable places. Move, turn or use fewer seats.":null;
    public bool SetCommons(Cell center,int seats=6)=>Commons is {} current?MoveCommons(current.Center,center,seats):AddCommons(center,seats);
    public bool AddCommons(Cell center,int seats=6,SharedPlaceLayout layout=SharedPlaceLayout.Gathered,int rotation=0)
    {
        if(CommonsProblem(center,seats,null,layout,rotation)!=null)return false;
        SharedPlaceList!.Add(new(){Center=center,Places=CommonsPlaces(center,seats,null,layout,rotation),Layout=layout,Rotation=rotation});return true;
    }
    public bool MoveCommons(Cell original,Cell center,int seats=6,SharedPlaceLayout layout=SharedPlaceLayout.Gathered,int rotation=0)
    {
        var place=SharedPlaces.FirstOrDefault(c=>c.Center==original);
        if(place==null || CommonsProblem(center,seats,original,layout,rotation)!=null)return false;
        var places=CommonsPlaces(center,seats,original,layout,rotation);
        if(original==center && place.Places.SequenceEqual(places) && place.Layout==layout && place.Rotation==rotation)return true;
        InterruptCommonsMeals(place);place.Center=center;place.Places=places;place.Layout=layout;place.Rotation=rotation;place.FirstDiner=null;return true;
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
            if(c==null || !Enum.IsDefined(c.Layout) || c.Rotation is <0 or >3 || !CanArrangeCommons || c.FirstDiner is int diner && (diner<0 || diner>=Population) || !Map.Contains(c.Center) || Blocked(c.Center) || c.Places==null || c.Places.Length is not (2 or 4 or 6) || c.Places.Distinct().Count()!=c.Places.Length ||
                c.Places.Any(p=>!Map.Contains(p) || Blocked(p) || (p.Point-c.Center.Point).LengthSquared()>(c.Layout==SharedPlaceLayout.Line?10:8) || FindPath(YardAccess,p,Blocked)==null) || !c.Places.Append(c.Center).All(claimed.Add))
                throw new InvalidOperationException("Invalid or overlapping shared places");
        }
        foreach(var p in People.Where(p=>p.Meal is {Commons:true,Reserved:true} or {Commons:true,Carrying:true}))
            if(!SharedPlaces.Any(c=>c.Places.Contains(p.Meal!.Seat)))throw new InvalidOperationException("Meal outside its shared place");
    }
}

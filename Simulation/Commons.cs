using System;
using System.Linq;
namespace Inlanders.Simulation;

public sealed class SharedCommons
{
    public Cell Center { get; set; }
    public Cell[] Places { get; set; }=Array.Empty<Cell>();
}
public sealed partial class World
{
    public SharedCommons? Commons=>Neighborhood?.Commons;
    public Cell[] CommonsPlaces(Cell center)
    {
        if(!Map.Contains(center) || Blocked(center))return Array.Empty<Cell>();
        var reachable=Reachable(YardAccess,Blocked);
        return Map.Land.Where(c=>(c.Point-center.Point).LengthSquared() is >=2 and <=8 && !Blocked(c) && reachable.Contains(c) &&
            !(Gathering is {Active:true} g && g.Seats.Values.Contains(c)) && !ComfortSpotReserved(c) &&
            !People.Any(p=>(p.Meal is {Reserved:true} or {Carrying:true}) && p.Meal.Seat==c || (p.Task is Work.ToRest or Work.Resting || p.LeisureSiteId!=null) && p.Destination==c))
            .OrderBy(c=>(c.Point-center.Point).LengthSquared()).ThenBy(c=>c.Z).ThenBy(c=>c.X).Take(6).ToArray();
    }
    public string? CommonsProblem(Cell center)=>Neighborhood?.Complete!=true?"Welcome the newcomers first.":CommonsPlaces(center).Length<6?"Choose open ground with six reachable places nearby.":null;
    public bool SetCommons(Cell center)
    {
        if(CommonsProblem(center)!=null)return false;
        var places=CommonsPlaces(center);RemoveCommons();Neighborhood!.Commons=new(){Center=center,Places=places};return true;
    }
    public bool RemoveCommons()
    {
        if(Commons==null)return false;
        Neighborhood!.Commons=null;
        foreach(var p in People.Where(p=>p.Meal?.Commons==true).ToArray()){p.Meal!.Commons=false;InterruptMeal(p);}
        return true;
    }
    private Cell? AvailableCommonsPlace(Cell supply)
    {
        if(Commons is not {} commons || (supply.Point-commons.Center.Point).LengthSquared()>64)return null;
        return commons.Places.Where(c=>!People.Any(p=>(p.Meal is {Reserved:true} or {Carrying:true}) && p.Meal.Seat==c) &&
            !Blocked(c) && FindPath(supply,c,Blocked)!=null).Cast<Cell?>().FirstOrDefault();
    }
    private void ValidateCommons()
    {
        if(Commons is not {} c){if(People.Any(p=>p.Meal?.Commons==true))throw new InvalidOperationException("Meal references missing commons");return;}
        if(Neighborhood?.Complete!=true || !Map.Contains(c.Center) || Blocked(c.Center) || c.Places==null || c.Places.Length!=6 || c.Places.Distinct().Count()!=6 ||
            c.Places.Any(p=>!Map.Contains(p) || Blocked(p) || (p.Point-c.Center.Point).LengthSquared()>8 || FindPath(YardAccess,p,Blocked)==null))
            throw new InvalidOperationException("Invalid shared commons");
        foreach(var p in People.Where(p=>p.Meal is {Commons:true,Reserved:true} or {Commons:true,Carrying:true}))
            if(!c.Places.Contains(p.Meal!.Seat))throw new InvalidOperationException("Meal outside its commons");
    }
}


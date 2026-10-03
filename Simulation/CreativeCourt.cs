using System;
using System.Linq;

namespace Inlanders.Simulation;

public sealed partial class World
{
    public bool SimulatesMeals => !Creative || IsArrangementCourt || Founding?.TransformationHamlet==true;
    public static World NewCreativeCourt() => CreativeCourtFrom(NewArrangementCourt());

    // Copy the actual village, including residents and food claims. This is also the
    // matched baseline for comparing free arrangement with a constrained court.
    public static World CreativeCourtFrom(World source)
    {
        if(!source.IsArrangementCourt)throw new ArgumentException("Choose a Willow court village.",nameof(source));
        var world=LoadJson(source.SaveJson());
        world.Creative=true;world.Food.Hunger=0;
        world.Neighborhood!.Arrangement!.BuildingId=null;
        world.History.Add("Free arrangement: instant free construction and moves; real meals without hunger penalties. Welcoming is optional.");
        world.Validate();return world;
    }

    // An available worker remains available while walking home: Waiting already
    // allows meal requests and fresh work to replace this unclaimed journey.
    private bool IdleHomeJourney(Villager p)=>SharedWork && p.SharedWorker && p.Task==Work.Waiting && p.Route.Count>0;
    public SharedCommons? QuietSharedPlace(Villager p)=>PublicPlace!=null && p.SharedWorker && p.Task==Work.Waiting
        ?SharedPlaces.FirstOrDefault(c=>c.Center==p.QuietSharedCenter && c.Places.Contains(p.Route.Count>0?p.Destination:At(p))):null;
    private void EndQuietVisit(Villager p)
    {
        if(p.QuietSharedCenter==null)return;
        p.QuietSharedCenter=null;p.QuietVisitUntil=0;p.NextQuietVisitTime=Food.Time+32+(p.Id*7%19);
    }
    private bool WaitAtSharedPlace(Villager p)
    {
        if(PublicPlace==null || p.HomeId is not int id)return false;
        var home=Cottages.FirstOrDefault(h=>h.Id==id);if(home==null)return false;
        bool Free(Cell c)=>!Blocked(c) && !People.Any(o=>o.Id!=p.Id &&
            (At(o)==c || IdleHomeJourney(o) && o.Destination==c || (o.Meal is {Reserved:true} or {Carrying:true}) && o.Meal.Seat==c));
        var current=QuietSharedPlace(p);
        if(current!=null)
        {
            var target=p.Route.Count>0?p.Destination:At(p);
            if(p.Route.Count==0 && p.QuietVisitUntil==0)p.QuietVisitUntil=Food.Time+18+(p.Id*7%15);
            if(Free(target) && (p.QuietVisitUntil==0 || Food.Time<p.QuietVisitUntil))
            {p.Status=p.Route.Count>0?"Heading to shared ground — available for work":"Sitting together — available for work";return true;}
            EndQuietVisit(p);p.Route.Clear(); // Return home; meals and new work may replace this trip.
        }
        if(Food.Time<p.NextQuietVisitTime || Food.Time<p.Id*3)return false;
        var seat=SharedPlaces.Where(c=>(c.Center.Point-home.Entrance.Point).LengthSquared()<=64).SelectMany(c=>c.Places)
            .Where(Free).OrderBy(c=>(c.Point-p.Position).LengthSquared()).ThenBy(c=>c.Z).ThenBy(c=>c.X)
            .Cast<Cell?>().FirstOrDefault(c=>FindPath(At(p),c!.Value,Blocked)!=null);
        if(seat is not Cell at)return false;
        p.QuietSharedCenter=SharedPlaceAt(at)!.Center;p.QuietVisitUntil=0;
        Go(p,at,Work.Waiting,"Heading to shared ground — available for work");return true;
    }
    public Villager? QuietCompanion(Villager person)
    {
        var place=QuietSharedPlace(person);if(place==null || person.Route.Count>0 || place.Layout!=SharedPlaceLayout.Gathered)return null;
        var visitors=People.Where(p=>p.Route.Count==0 && QuietSharedPlace(p)==place).OrderBy(p=>p.Id).ToList();
        while(visitors.Count>1)
        {
            var first=visitors[0];visitors.RemoveAt(0);
            var other=visitors.Where(p=>(p.Position-first.Position).LengthSquared()<=6.25f).OrderBy(p=>(p.Position-first.Position).LengthSquared()).ThenBy(p=>p.Id).FirstOrDefault();
            if(other==null)continue;visitors.Remove(other);
            if(first==person)return other;if(other==person)return first;
        }
        return null;
    }
    private void WaitNearHome(Villager person)
    {
        if(!SharedWork || person.HomeId is not int id)return;
        if(WaitAtSharedPlace(person))return;
        if(IdleHomeJourney(person)){person.Status="Heading home while work is quiet";return;}
        var home=Cottages.FirstOrDefault(h=>h.Id==id && IsHome(h));if(home==null)return;
        var occupied=People.Where(p=>p.Id!=person.Id).Select(At)
            .Concat(People.Where(p=>p.Id!=person.Id && (p.Task is Work.ToRest or Work.Resting or Work.ToLeisure or Work.Leisure || IdleHomeJourney(p))).Select(p=>p.Destination)).ToHashSet();
        var yard=HomeYardPlaces(home);
        bool Free(Cell c)=>!Blocked(c) && !occupied.Contains(c) && !MealSpotReserved(c) && !ComfortSpotReserved(c) &&
            c!=YardAccess && !Cottages.Any(s=>s.Entrance==c) && ((c.Point-home.Entrance.Point).LengthSquared()<=4 || yard.Contains(c));
        if(Free(At(person)) && (yard.Length==0 || yard.Contains(At(person)))){person.Status=yard.Length>0?"Mending at home — available for work":"At home — available for work";return;}
        var spot=Map.Land.Where(Free).OrderBy(c=>yard.Contains(c)?0:1).ThenBy(c=>(c.Point-home.Entrance.Point).LengthSquared())
            .ThenBy(c=>c.Z).ThenBy(c=>c.X).Cast<Cell?>().FirstOrDefault(c=>FindPath(At(person),c!.Value,Blocked)!=null);
        if(spot is Cell target)Go(person,target,Work.Waiting,"Heading home while work is quiet");
    }
}

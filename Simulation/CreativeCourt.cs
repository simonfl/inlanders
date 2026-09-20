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
    private void WaitNearHome(Villager person)
    {
        if(!SharedWork || person.HomeId is not int id)return;
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Inlanders.Simulation;

public enum BoatPhase { Moored, Outbound, Fishing, Returning }
public sealed class FishingBoat
{
    public Vector2 Position { get; set; }
    public Queue<Cell> Route { get; set; } = new();
    public BoatPhase Phase { get; set; }
    public int? FisherId { get; set; }
    public int? GroundId { get; set; }
    public int ReservedCatch { get; set; }
    public int Fish { get; set; }
    public float Timer { get; set; }
    public float Heading { get; set; }
}

public sealed partial class World
{
    public int AvailableFish(FishHabitat ground) => ground.Available - Cottages.Sum(c => c.Boat?.GroundId == ground.Id ? c.Boat.ReservedCatch : 0);
    public FishingBoat? PassengerBoat(Villager person) => person.Task == Work.Aboard
        ? Cottages.FirstOrDefault(c => c.Id == person.WorkplaceId)?.Boat : null;
    private void ClaimFishing(Villager person)
    {
        foreach(var dock in Cottages.Where(c => c.Kind == BuildingKind.FishingDock && c.Complete && !c.DemolitionRequested && !c.WorkPaused && BelowOutputTarget(c))
            .OrderBy(c => Vector2.DistanceSquared(c.Entrance.Point,person.Position)).ThenBy(c=>c.Id))
        {
            if(dock.Boat?.FisherId != null || !Accessible(dock.Entrance)) continue;
            var trips=Map.FishingGrounds.Where(g=>AvailableFish(g)>0)
                .Select(g=>(Ground:g,Route:FindBoatRoute(dock.Launch,g.Cell)))
                .Where(t=>t.Route!=null).OrderByDescending(t=>Math.Min(4,AvailableFish(t.Ground))).ThenBy(t=>t.Route!.Count).ThenBy(t=>t.Ground.Id);
            foreach(var trip in trips)
            {
                dock.Boat ??= new FishingBoat { Position=dock.Launch.Point,Heading=(dock.Rotated?MathF.PI/2:0)+(dock.DockFromFar?MathF.PI:0) };
                var boat=dock.Boat;
                boat.FisherId=person.Id; boat.GroundId=trip.Ground.Id;
                boat.ReservedCatch=Math.Min(4,AvailableFish(trip.Ground));
                person.WorkplaceId=dock.Id;
                Go(person,dock.Entrance,Work.ToDock,$"Walking to fishing dock {dock.Id}");
                return;
            }
        }
        person.Status=ProductionWait(Role.Fisher);
    }
    private void ReturnBoat(Cottage dock)
    {
        var boat=dock.Boat!;
        if(boat.Phase==BoatPhase.Returning) return;
        boat.ReservedCatch=0; boat.Phase=BoatPhase.Returning; boat.Timer=0;
        var start=new Cell((int)MathF.Round(boat.Position.X),(int)MathF.Round(boat.Position.Y));
        var route=FindBoatRoute(start,dock.Launch) ?? throw new InvalidOperationException("Boat lost its landing route");
        boat.Route=new Queue<Cell>(route);
        if(Vector2.DistanceSquared(start.Point,boat.Position)>.0001f) boat.Route=new Queue<Cell>(new[]{start}.Concat(route));
        People[boat.FisherId!.Value].Status=boat.Fish>0 ? $"Returning with {boat.Fish} fish" : "Returning to the dock";
    }
    // True means the resident must stay attached until the boat reaches land.
    private bool InterruptFishing(Villager person)
    {
        if(person.Task is not (Work.ToDock or Work.Aboard)) return false;
        var dock=Cottages.Single(c=>c.Id==person.WorkplaceId);
        if(person.Task==Work.Aboard) { ReturnBoat(dock); return true; }
        var boat=dock.Boat!;
        boat.FisherId=null; boat.GroundId=null; boat.ReservedCatch=0;
        return false;
    }
    private void TickFishing(Villager person,float dt)
    {
        var dock=Cottages.Single(c=>c.Id==person.WorkplaceId);
        var boat=dock.Boat!;
        if(person.Task==Work.ToDock)
        {
            boat.Route=new Queue<Cell>(FindBoatRoute(dock.Launch,Map.FishingGrounds.Single(g=>g.Id==boat.GroundId).Cell)
                ?? throw new InvalidOperationException("Fishing route changed before boarding"));
            boat.Phase=BoatPhase.Outbound; boat.Timer=0; person.Task=Work.Aboard; person.Status="Rowing to fishing grounds";
            return;
        }
        if(boat.Route.TryPeek(out var next))
        {
            var offset=next.Point-boat.Position; float distance=offset.Length(),travel=1.5f*dt;
            if(distance>.0001f) boat.Heading=MathF.Atan2(-offset.X,-offset.Y);
            if(distance<=travel) { boat.Position=next.Point; boat.Route.Dequeue(); }
            else boat.Position+=offset/distance*travel;
            return;
        }
        switch(boat.Phase)
        {
            case BoatPhase.Outbound:
                boat.Phase=BoatPhase.Fishing; boat.Timer=0; person.Status="Fishing with a net"; break;
            case BoatPhase.Fishing:
                boat.Timer+=dt;
                if(boat.Timer<8) break;
                var ground=Map.FishingGrounds.Single(g=>g.Id==boat.GroundId);
                boat.Fish=ground.Take(boat.ReservedCatch); Food.CaughtFish+=boat.Fish;
                ReturnBoat(dock); break;
            case BoatPhase.Returning:
                boat.Timer+=dt; person.Status="Unloading at the dock";
                if(boat.Timer<1) break;
                person.Cargo=Resource.Fish; person.Carried=boat.Fish;
                boat.Fish=0; boat.FisherId=null; boat.GroundId=null; boat.Phase=BoatPhase.Moored; boat.Timer=0;
                Finish(person);
                if(person.Carried>0) Go(person,YardAccess,Work.ToPantry,$"Carrying {person.Carried} fish to the pantry");
                break;
        }
    }

    private void ValidateFishing()
    {
        void Check(bool ok,string why) { if(!ok) throw new InvalidOperationException(why); }
        foreach(var dock in Cottages)
        {
            if(dock.Boat is not FishingBoat boat) continue;
            Check(dock.Kind==BuildingKind.FishingDock && dock.Complete,"Boat without a completed dock");
            Check(Enum.IsDefined(boat.Phase) && float.IsFinite(boat.Position.X) && float.IsFinite(boat.Position.Y) &&
                float.IsFinite(boat.Heading) && float.IsFinite(boat.Timer) && boat.Timer>=0 && boat.Timer<8,"Invalid boat state");
            var cell=new Cell((int)MathF.Round(boat.Position.X),(int)MathF.Round(boat.Position.Y));
            Check(!BoatBlocked(cell) && boat.Route!=null && boat.Route.All(c=>!BoatBlocked(c)),"Boat outside navigable water");
            var previous=boat.Position;
            foreach(var waypoint in boat.Route!)
            {
                var offset=waypoint.Point-previous;
                Check(MathF.Abs(offset.X)+MathF.Abs(offset.Y)<=1.001f && (MathF.Abs(offset.X)<.001f || MathF.Abs(offset.Y)<.001f),"Boat route skips water tiles");
                previous=waypoint.Point;
            }
            Check(boat.Fish is >=0 and <=4 && boat.ReservedCatch is >=0 and <=4 && (boat.Fish==0 || boat.ReservedCatch==0),"Invalid boat catch");
            Check(boat.FisherId==null ? boat.Phase==BoatPhase.Moored && boat.Fish==0 && boat.ReservedCatch==0 && boat.GroundId==null :
                People.Any(p=>p.Id==boat.FisherId && p.WorkplaceId==dock.Id && (boat.Phase==BoatPhase.Moored ? p.Task==Work.ToDock : p.Task==Work.Aboard)),"Boat passenger mismatch");
            Check(boat.Phase!=BoatPhase.Moored || boat.Position==dock.Launch.Point && boat.Route!.Count==0,"Moored boat away from launch");
            Check(boat.GroundId==null || Map.FishingGrounds.Any(g=>g.Id==boat.GroundId),"Missing fishing ground");
            Check(boat.FisherId==null || boat.GroundId!=null,"Active trip without fishing ground");
            Check(boat.Phase is not (BoatPhase.Outbound or BoatPhase.Fishing) || boat.Fish==0 && boat.ReservedCatch>0,"Invalid outbound catch");
            Check(boat.Phase!=BoatPhase.Returning || boat.ReservedCatch==0 && previous==dock.Launch.Point,"Return route misses landing");
            Check(boat.Phase is not (BoatPhase.Outbound or BoatPhase.Fishing) || previous==Map.FishingGrounds.Single(g=>g.Id==boat.GroundId).Cell.Point,"Fishing route misses habitat");
            Check(boat.ReservedCatch==0 || boat.GroundId!=null && boat.Phase is BoatPhase.Moored or BoatPhase.Outbound or BoatPhase.Fishing,"Invalid fish reservation");
        }
        foreach(var ground in Map.FishingGrounds) Check(AvailableFish(ground)>=0,"Fish stock claimed twice");
        foreach(var person in People.Where(p=>p.Task is Work.ToDock or Work.Aboard))
        {
            Check(Cottages.Any(c=>c.Id==person.WorkplaceId && c.Boat?.FisherId==person.Id),"Fisher without boat ownership");
            if(person.Task==Work.Aboard) Check(person.Route.Count==0 && person.Position==Cottages.Single(c=>c.Id==person.WorkplaceId).Entrance.Point && person.Carried==0,"Invalid passenger land state");
        }
    }
}

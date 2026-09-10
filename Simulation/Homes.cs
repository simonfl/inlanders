using System;
using System.Linq;

namespace Inlanders.Simulation;

public sealed partial class World
{
    public const float RestSeconds=6, RestInterval=180, RecentRestWindow=240;
    public int ResidentsWithHomes => People.Count(p=>p.HomeId!=null);
    public int ResidentsRested => People.Count(RecentlyRested);
    public bool RecentlyRested(Villager person) => person.LastRestTime is float last && Food.Time-last<RecentRestWindow;
    private bool IsHome(Cottage home) => home.Complete && !home.DemolitionRequested && Buildings.Get(home.Kind).Beds>0;
    private void ReconcileHomes()
    {
        var homes=Cottages.Where(IsHome).ToArray();
        foreach(var person in People)
        {
            if(person.HomeId!=null && !homes.Any(h=>h.Id==person.HomeId))
            {
                if(person.Task is Work.ToRest or Work.Resting) Interrupt(person);
                person.HomeId=null;
            }
            if(person.HomeId!=null) continue;
            var home=homes.Where(h=>People.Count(p=>p.HomeId==h.Id)<Buildings.Get(h.Kind).Beds)
                .OrderBy(h=>(h.Entrance.Point-person.Position).LengthSquared()).ThenBy(h=>h.Id).FirstOrDefault();
            if(home==null) continue;
            person.HomeId=home.Id; person.NextRestTime=Food.Time+45+person.Id%8*6;
        }
    }
    public string? HomeAssignmentProblem(int personId,int homeId)
    {
        if(Food.Celebrating) return "Move home after supper finishes.";
        var person=People.FirstOrDefault(p=>p.Id==personId);
        var home=Cottages.FirstOrDefault(h=>h.Id==homeId && IsHome(h));
        if(person==null || home==null) return "Choose a resident and a completed home.";
        if(person.HomeId==homeId) return null;
        return People.Count(p=>p.HomeId==homeId)>=Buildings.Get(home.Kind).Beds ? "This home has no spare bed." : null;
    }
    public bool AssignHome(int personId,int homeId)
    {
        if(HomeAssignmentProblem(personId,homeId)!=null) return false;
        var person=People[personId]; if(person.HomeId==homeId) return true;
        if(person.Task is Work.ToRest or Work.Resting) Interrupt(person);
        person.HomeId=homeId; person.NextRestTime=Math.Min(person.NextRestTime,Food.Time+15); _retry=0;
        return true;
    }
    private Cell? RestSpot(Villager person,Cottage home)
    {
        var occupied=People.Where(p=>p.Task is Work.ToRest or Work.Resting or Work.ToLeisure or Work.Leisure).Select(p=>p.Destination).ToHashSet();
        return new[]{home.Entrance,new(home.Entrance.X-1,home.Entrance.Z),new(home.Entrance.X+1,home.Entrance.Z),new(home.Entrance.X,home.Entrance.Z+1),new(home.Entrance.X,home.Entrance.Z-1)}
            .Where(c=>!Blocked(c) && !occupied.Contains(c)).Cast<Cell?>().FirstOrDefault(c=>FindPath(At(person),c!.Value,Blocked)!=null);
    }
    private bool ClaimRest(Villager person)
    {
        if(person.Carried>0 || Food.Time<person.NextRestTime || person.HomeId is not int id) return false;
        var home=Cottages.FirstOrDefault(h=>h.Id==id && IsHome(h));
        if(home==null || RestSpot(person,home) is not Cell spot) return false;
        Go(person,spot,Work.ToRest,$"Heading home to {home.Kind} {id}"); return true;
    }
    public string RestSummary(Villager person)
    {
        if(person.HomeId==null) return "No assigned home — finish housing with a spare bed.";
        if(person.Task==Work.ToRest) return "Heading home; rest counts after the visit.";
        if(person.Task==Work.Resting) return "Resting at home.";
        string recent=RecentlyRested(person) ? $"Rested {(int)(Food.Time-person.LastRestTime!.Value)}s ago." : "No rest in the last four minutes.";
        return recent+(Food.Time<person.NextRestTime ? $" Next visit due in {(int)Math.Ceiling(person.NextRestTime-Food.Time)}s, between jobs." : person.Task!=Work.Waiting ? " Due after the current job or break." : " Waiting for a reachable free spot beside home.");
    }
    public string RecreationSummary(Villager person)
    {
        if(person.Task is Work.ToLeisure or Work.Leisure) return person.Task==Work.ToLeisure ? "Going to a recreation venue." : "Taking a recreation break.";
        if(person.LastLeisureTime is float last && Food.Time-last<person.LastLeisureWindow) return $"Recreation break completed {(int)(Food.Time-last)}s ago; benefit lasts another {(int)(person.LastLeisureWindow-(Food.Time-last))}s.";
        var squares=Cottages.Where(c=>Buildings.Get(c.Kind).RecreationSlots>0 && c.Complete && !c.DemolitionRequested).ToArray();
        if(squares.Length==0) return "No open square or hall — build a place to meet.";
        if(squares.All(s=>People.Count(p=>p.LeisureSiteId==s.Id)>=Buildings.Get(s.Kind).RecreationSlots)) return "Recreation venues are busy; squares serve four, halls eight.";
        if(Food.Time<person.NextLeisureTime) return "Waiting between visits; another break is due later.";
        return person.Task!=Work.Waiting ? "A recreation break can follow the current job or rest." : "Waiting for a reachable free recreation spot.";
    }
    private void ValidateHomes()
    {
        foreach(var person in People)
        {
            var home=Cottages.FirstOrDefault(h=>h.Id==person.HomeId && IsHome(h));
            if(person.HomeId!=null && home==null || !float.IsFinite(person.NextRestTime) || person.NextRestTime<0 || person.RestVisits<0 ||
                person.LastRestTime is float last && (!float.IsFinite(last) || last<0 || last>Food.Time)) throw new InvalidOperationException("Invalid resident home or rest history");
            if(person.Task is Work.ToRest or Work.Resting && (home==null || (person.Destination.Point-home.Entrance.Point).LengthSquared()>1 || Blocked(person.Destination) ||
                person.Carried!=0 || person.Reserved!=0 || person.SiteId!=null || person.WorkplaceId!=null || person.LeisureSiteId!=null)) throw new InvalidOperationException("Invalid home rest visit");
        }
        if(People.Where(p=>p.HomeId!=null).GroupBy(p=>p.HomeId).Any(g=>g.Count()>Buildings.Get(Cottages.Single(h=>h.Id==g.Key).Kind).Beds)) throw new InvalidOperationException("Home over capacity");
        var resting=People.Where(p=>p.Task is Work.ToRest or Work.Resting).ToArray();
        if(resting.Select(p=>p.Destination).Distinct().Count()!=resting.Length) throw new InvalidOperationException("Home rest spots overlap");
    }
}

using System;
using System.Linq;

namespace Inlanders.Simulation;

public sealed record DailyJourney(int Person,int? Source,string Heading,string Detail,Cell[] Route,bool Claimed,bool CanInspect=true,string InspectLabel="Inspect food");
public sealed partial class World
{
    public DailyJourney ReadDailyJourney(int personId)
    {
        var p=People.Single(p=>p.Id==personId);var meal=p.Meal;
        var last=Food.MealConsumptions.LastOrDefault(m=>m.Person==personId);
        string eaten=last==null?"":$"Last meal: {last.Kind.ToString().ToLowerInvariant()}, {(int)(Food.Time-last.Time)}s ago. ";
        if(meal is {Reserved:true} or {Carrying:true})
        {
            int? source=p.Task==Work.ReturnMeal?null:meal.SourceId;
            bool exists=source==null || Cottages.Any(c=>c.Id==source);
            return new(p.Id,exists?source:null,p.Task==Work.EatingMeal?"Eating here":p.Task==Work.ReturnMeal?"Returning an uneaten meal":meal.Carrying?"Carrying a meal to a seat":"Collecting a meal",
                eaten+(exists?FoodStoreName(source):"Food collected before its source was removed")+" · "+p.Status,p.Route.ToArray(),true,exists);
        }
        if(Founding!=null && (p.Task!=Work.Waiting || p.Route.Count>0 || AvailableAtHome(p)))
        {
            bool home=p.Task is Work.ToRest or Work.Resting || p.Task==Work.Waiting;
            int? place=home?p.HomeId:p.Route.Count>0?Cottages.FirstOrDefault(c=>c.Entrance==p.Destination)?.Id:p.LeisureSiteId??p.SiteId??p.WorkplaceId??p.StorageId;
            if(place!=null && !Cottages.Any(c=>c.Id==place))place=null;
            string activityHeading=home?(p.Task==Work.Resting?"Resting at home":p.Route.Count>0?"On the way home":"At home · available for work"):
                p.Task is Work.ToLeisure or Work.Leisure?"Taking a break":p.Route.Count>0?"On the way to work":"Working here";
            
            return new(p.Id,place,activityHeading,eaten+p.Status+". "+(p.Route.Count>0?$"{p.Route.Count} steps remaining on the current journey. ":"")+
                (p.SharedWorker?"Shares available village jobs.":"Has a dedicated role."),p.Route.ToArray(),true,place!=null || p.Route.Count>0,home?"Inspect home":place!=null?"Inspect place":"Show route");
        }
        var stores=FoodStores().Where(s=>EdibleKinds.Any(k=>FoodAvailableAt(s,k)>0)).ToArray();
        var routes=stores.Select(s=>new{Id=s,Route=FindPath(At(p),FoodAccess(s),Blocked)})
            .Where(s=>s.Route!=null).OrderBy(s=>s.Route!.Count).ToArray();
        if(routes.Length==0)return new(p.Id,null,stores.Length==0?"No free meal available":"Food is out of reach",
            eaten+(stores.Length==0?"Inspect a producer: it may need work, ingredients or time to grow.":"Check the crossing and open entrances."),Array.Empty<Cell>(),false);
        var near=routes[0];
        bool waiting=meal is {Eaten:false,Closed:false};
        string heading=waiting && p.Task!=Work.Waiting?"Finishing another activity before eating":waiting?"Waiting for a collection spot and seat":"Between meals";
        float next=meal is {Eaten:true}?meal.Due+60:p.NextMealTime;
        return new(p.Id,near.Id,heading,eaten+(waiting?p.Status+". ":$"Next request in {Math.Max(0,(int)(next-Food.Time))}s. ")+
            $"Available now: {FoodStoreName(near.Id)} · {near.Route!.Count} steps from here. Not reserved; the next meal may use another source.",near.Route.ToArray(),false);
    }
}

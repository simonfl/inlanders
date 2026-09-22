using System;
using System.Linq;
using System.Numerics;
namespace Inlanders.Simulation;
public sealed record PlaceJourney(int Person,string Name,string Relation,string Activity,Vector2 Position,Cell[] Steps,Resource? Cargo,int Amount);
public sealed partial class World
{
    // Actual committed trips only. Household trips may be elsewhere; their relation is named explicitly.
    public PlaceJourney[] ReadPlaceJourneys(int siteId)
    {
        if(!Cottages.Any(c=>c.Id==siteId))return Array.Empty<PlaceJourney>();
        return People.Where(p=>p.Route.Count>0).Select(p=>{
            string? relation=p.Meal is {Reserved:true} or {Carrying:true} && p.Meal.SourceId==siteId?"Meal from this place":
                p.FoodSourceId==siteId || p.FoodDestinationId==siteId?"Food delivery":
                p.WorkplaceId==siteId || p.SiteId==siteId?"Work at this place":
                p.AssignedWorkplaceId==siteId?"Assigned worker’s trip":
                p.ComfortHomeId==siteId?"Furnishing this home":p.HomeId==siteId?"Household trip":
                p.LeisureSiteId==siteId?"A visit here":null;
            return relation==null?null:new PlaceJourney(p.Id,p.Name,relation,p.Status,p.Position,p.Route.ToArray(),p.Carried>0?p.Cargo:null,p.Carried);
        }).OfType<PlaceJourney>().OrderBy(p=>p.Person).ToArray();
    }
}

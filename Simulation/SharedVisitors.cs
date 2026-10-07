using System;
using System.Linq;
namespace Inlanders.Simulation;
public sealed record SharedVisitor(int Person,string Activity);
public sealed partial class World
{
    public SharedVisitor[] ReadSharedVisitors(Cell center)
    {
        var place=SharedPlaces.FirstOrDefault(c=>c.Center==center);if(place==null)return Array.Empty<SharedVisitor>();
        return People.Select(p=>{
            string? activity=QuietSharedPlace(p)==place?(p.Route.Count>0?"Coming to sit":"Sitting here"):
                p.Meal is {Commons:true} meal && (meal.Reserved || meal.Carrying) && place.Places.Contains(meal.Seat)?
                    (p.Task==Work.EatingMeal?"Eating here":p.Task==Work.ToMealSeat?"Bringing a meal":"Collecting a meal"):null;
            return activity==null?null:new SharedVisitor(p.Id,activity);
        }).OfType<SharedVisitor>().OrderBy(v=>v.Person).ToArray();
    }
}

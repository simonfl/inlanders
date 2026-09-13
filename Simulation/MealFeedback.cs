using System;
using System.Linq;

namespace Inlanders.Simulation;

public sealed partial class World
{
    public bool NeedsMealAttention(Villager person) => !Creative && (!person.Fed ||
        Food.MealOutcomes.Any(m=>m.Person==person.Id && m.Time>Food.Time-FoodFlowWindow && (!m.Timely || m.Skipped)));

    public string MealSummary(Villager person)
    {
        if(Creative) return "Food needs are disabled in Creative.";
        var last=Food.MealConsumptions.LastOrDefault(m=>m.Person==person.Id);
        string history=last==null ? "No eating recorded in the last three minutes." :
            $"Ate {last.Kind.ToString().ToLowerInvariant()} {(int)(Food.Time-last.Time)}s ago"+(last.Late?" (late).":".");
        string state=person.Fed?"Nourished. ":"Hungry. ";
        var r=person.Meal;
        if(r is {Eaten:true}) return state+history+" Next request follows the current meal window.";
        if(r==null) return state+history+$" Next request in {Math.Max(0,(int)Math.Ceiling(person.NextMealTime-Food.Time))}s.";
        string timing=r.Closed?"Deadline missed; late eating restores nourishment but not reliable service.":$"{Math.Max(0,(int)Math.Ceiling(r.Due+60-Food.Time))}s until the deadline.";
        string activity;
        if(person.Task==Work.ReturnMeal) activity="Returning the uneaten portion to central storage.";
        else if(r.Carrying) activity=person.Task==Work.EatingMeal?"Eating the collected portion.":"Carrying food to a nearby seat; it has not been eaten yet.";
        else if(r.Reserved) activity=r.Welcome?"Collecting a portion at the welcome table.":r.SourceId is int id?$"Collecting reserved food at pantry {id}.":"Collecting reserved food at central storage.";
        else if(person.Task!=Work.Waiting) activity="Meal requested; finishing the current job or visit. Shorter trips leave more time to eat.";
        else if(!FoodStores().Any(id=>EdibleKinds.Any(k=>FoodAvailableAt(id,k)>0))) activity="No unreserved food at an open pantry. Check production and incoming deliveries.";
        else if(!FoodStores().Any(id=>EdibleKinds.Any(k=>FoodAvailableAt(id,k)>0) && FindPath(At(person),FoodAccess(id),Blocked)!=null)) activity="Stored food is unreachable. Check paths and crossings to an open pantry.";
        else activity="Waiting for a reachable collection spot and free seat near food storage.";
        return state+history+"\n"+activity+"\n"+timing;
    }
}

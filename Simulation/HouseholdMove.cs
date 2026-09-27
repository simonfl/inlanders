using System;
using System.Linq;
namespace Inlanders.Simulation;
public sealed partial class World
{
    public string? HouseholdMoveProblem(int from,int to)
    {
        if(PublicPlace==null)return "Choose a public village.";
        if(Food.Celebrating)return "Wait until supper finishes.";
        var source=Cottages.FirstOrDefault(c=>c.Id==from && IsHome(c));var target=Cottages.FirstOrDefault(c=>c.Id==to && IsHome(c));
        if(source==null || target==null || from==to)return "Choose a different finished home.";
        int arriving=People.Count(p=>p.HomeId==from),returning=People.Count(p=>p.HomeId==to);
        if(arriving==0)return "No neighbors live in the first home.";
        if(arriving>Buildings.Get(target.Kind).Beds || returning>Buildings.Get(source.Kind).Beds)return "Both households must fit their new homes.";
        return null;
    }
    public bool MoveHousehold(int from,int to)
    {
        if(HouseholdMoveProblem(from,to)!=null)return false;
        var residents=People.Where(p=>p.HomeId==from || p.HomeId==to).ToArray();
        foreach(var person in residents)
        {
            int next=person.HomeId==from?to:from;
            if(person.Task is Work.ToRest or Work.Resting || IdleHomeJourney(person))Interrupt(person);
            person.HomeId=next;person.NextRestTime=Math.Min(person.NextRestTime,Food.Time+15);
        }
        History.Add($"Households changed homes {from} and {to}. Current work and meals continue; next home visits use the chosen houses.");_retry=0;return true;
    }
}

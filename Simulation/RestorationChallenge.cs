using System;
using System.Linq;
namespace Inlanders.Simulation;
public sealed partial class World
{
    public static World NewRestorationChallenge(bool relaxed=false)
    {
        var w=NewNewcomerChallenge(false,true);w.Challenge!.Restoration=true;
        foreach(var person in w.People){w.Interrupt(person);person.Position=w.YardAccess.Point;}
        foreach(var home in w.Cottages.Where(w.IsHome).OrderByDescending(c=>c.Cell.Z).Take(2).ToArray())w.Cottages.Remove(home);
        // A first crossing opens usable island ground; the old fields need a second crossing.
        for(int x=-10;x<11;x++)w.Map.Water.Add(new(x,-7));
        foreach(var field in w.Cottages.Where(c=>c.Kind==BuildingKind.VegetableField)){field.Cell=new(field.Cell.X,-9);field.Planted=true;field.Growth=1;field.Harvest=VegetableYield(field);w.Food.GrownVegetables+=field.Harvest;}
        w.Paths.RemoveWhere(w.Map.Water.Contains);
        w.ReconcileHomes();
        w._yardLogs=10;w.InitialLogs=w.SawnLogs+w.Trees.Sum(t=>t.Logs)+w.Cottages.Sum(c=>c.Delivered)+w._yardLogs;
        w.Food.InitialBerries=w.Food.Berries=24;
        w.Founding!.StartingBuildings=w.Cottages.Select(b=>new StartingBuilding{Id=b.Id,Cell=b.Cell,Rotation=b.Rotation,Kind=b.Kind}).ToList();
        w.Map.Name="Après la crue · After the flood";
        w.History.Clear();w.History.Add("The flood took two cottages and cut off the northern fields. Twelve neighbors remain, with eight beds and two meals of provisions. Rebuild homes and restore food for the people already here. A near crossing opens an island for cultivation or homes. Cross the second channel to recover the northern fields, establish new cultivation on opened ground, or choose another livelihood on the home bank. No new arrivals are required.");
        w.Validate();w.ValidateMapOccupancy();return relaxed?RelaxedHamletFrom(w):w;
    }
    public int RestorationFed=>Challenge?.Meals.Count(p=>p.Key<12 && p.Value>=2)??0;
    public string? RestorationProblem()=>ChallengeHoused<12?"Four displaced neighbors need reachable homes.":People.Any(p=>!p.Fed)?"Restore meals for residents who missed one.":RestorationFed<12?"Let all twelve residents eat two ordinary meals during recovery.":EdibleStored<24?"Keep 24 edible portions ready for the next two village meals.":null;
    public string RestorationStatus=>Challenge?.Complete==true?"Restoration complete":ChallengeHoused<12?$"Rebuild · {ChallengeHoused}/12 housed":People.Any(p=>!p.Fed)?"Recovery · meals needed":RestorationFed<12?$"Recovery · {RestorationFed}/12 fed twice":$"Provisions · {Math.Min(24,EdibleStored)}/24";
}

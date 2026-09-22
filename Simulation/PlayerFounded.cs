using System.Linq;
namespace Inlanders.Simulation;
public sealed partial class World
{
    public static World NewPlayerFounded(bool relaxed=false)
    {
        var w=NewTransformationHamlet(false,true,false,true);
        foreach(var person in w.People)w.Interrupt(person);
        w.People.RemoveRange(InitialPopulation,w.People.Count-InitialPopulation);
        w.Cottages.Clear();w.Paths.Clear();w._nextSite=1;
        foreach(var person in w.People){person.HomeId=null;person.Position=new(-2+person.Id%4,3+person.Id/4);}
        w.Founding!.PlayerFounded=true;w.Founding.StartingBuildings.Clear();w.Founding.Settled.Clear();
        w._yardLogs=48;w.InitialLogs=w.Trees.Sum(t=>t.Logs)+w._yardLogs+w.SawnLogs;
        w.Food.InitialBerries=w.Food.Berries=120;
        w.Map.Name="Une place à nous · A place of our own";
        w.History.Clear();w.History.Add("Eight neighbors arrive with timber and provisions. Choose where to make homes and how to live from this land. Gardens, grain and an oven, or a river landing offer different working places. There is time to try, move and change your mind; more neighbors are optional.");
        w.ReconcileHomes();w.Validate();w.ValidateMapOccupancy();return relaxed?RelaxedHamletFrom(w):w;
    }
}

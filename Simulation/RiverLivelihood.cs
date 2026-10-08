using System.Linq;
namespace Inlanders.Simulation;
public sealed partial class World
{
    public static World NewRiverLivelihood(bool relaxed=false)
    {
        var w=NewRiverFrontage(false,true,true);
        foreach(var person in w.People)w.Interrupt(person);
        foreach(var site in w.Cottages.Where(c=>ProductionOutput(c.Kind)!=null).ToArray())
        {w._yardLogs+=site.Delivered;w.Cottages.Remove(site);}
        w.Founding!.PlayerFounded=true;w.Founding.StartingBuildings=w.Cottages.Select(c=>new StartingBuilding{Id=c.Id,Cell=c.Cell,Rotation=c.Rotation,Kind=c.Kind}).ToList();
        w.Map.Name="Prendre racine · A living from the river land";
        w.History.Clear();w.History.Add("The homes are ready, and provisions give twelve neighbors time to settle. Choose how to live from this land: cultivate near the homes, work the river, gather from the woods, or combine them. The timber for the fields and landing is yours to invest differently. No required building or growth target.");
        w.Validate();w.ValidateMapOccupancy();return relaxed?RelaxedHamletFrom(w):w;
    }
}

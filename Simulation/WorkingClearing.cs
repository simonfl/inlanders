using System;
using System.Linq;
namespace Inlanders.Simulation;
public sealed partial class World
{
    public static World NewWorkingClearing(bool relaxed=false)
    {
        var w=NewRiverFrontage();w.Founding!.PlayerFounded=false;w.Founding.WorkingClearing=true;w.Founding.AcrossTheInlet=false;
        foreach(var p in w.People)p.Position=w.YardAccess.Point;
        Cottage Ready(Cell at,BuildingKind kind,int turn=0,int rows=0)
        {
            var b=w.Place(at,turn,kind,rows)??throw new InvalidOperationException($"Clearing {kind}: {w.PlacementProblem(at,turn,kind,rows)}");
            b.Delivered=b.Required;b.Construction=1;b.EstablishmentPending=false;return b;
        }
        foreach(var at in new[]{new Cell(-3,9),new(3,9),new(-3,5),new(3,5)})Ready(at,BuildingKind.Cottage);
        foreach(var (at,growth) in new[]{(new Cell(-1,-5),.6f),(new Cell(-1,-9),.2f)}){var field=Ready(at,BuildingKind.VegetableField,1,2);field.Planted=true;field.Growth=growth;}
        w.ReconcileHomes();foreach(var p in w.People)p.Position=w.Cottages.Single(c=>c.Id==p.HomeId).Entrance.Point;
        foreach(var home in w.Cottages.Where(c=>c.Kind==BuildingKind.Cottage))w.ConnectPaths(w.YardAccess,home.Entrance);
        w._yardLogs=16;w.InitialLogs=w.SawnLogs+w.Trees.Sum(t=>t.Logs)+w.Cottages.Sum(c=>c.Delivered)+w._yardLogs;
        w.Food.InitialBerries=w.Food.Berries=80;
        w.Founding.StartingBuildings=w.Cottages.Select(c=>new StartingBuilding{Id=c.Id,Cell=c.Cell,Rotation=c.Rotation,Kind=c.Kind}).ToList();
        w.Map.Name="La clairière · A working clearing";
        w.History.Clear();w.History.Add("Eight neighbors have homes beside the river. Their two small working strips lie toward the woods, apart from the houses. Sixteen logs, four planks and eighty food portions leave time to choose. Bring growing ground closer, extend it, make a landing, or leave this modest place as it is. No required improvement or arrivals.");
        w.Validate();w.ValidateMapOccupancy();return relaxed?RelaxedHamletFrom(w):w;
    }
}

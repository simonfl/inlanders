using System.Linq;
namespace Inlanders.Simulation;

public sealed record GardenEntrancePlan(Cell Cell,int Rotation,Cell Entrance,int Before,int After,Cell[] Route,Cell[] NewRoute,string? Problem);
public sealed partial class World
{
    // A gate change keeps the exact planted ground. This is not free relocation,
    // and it does not relax the existing policy for moving crops or buildings.
    public GardenEntrancePlan? ReadGardenEntrance(int id)
    {
        var garden=Cottages.FirstOrDefault(c=>c.Id==id && c.Kind==BuildingKind.VegetableGarden && c.Complete && !c.DemolitionRequested);
        if(Riverside==null || garden==null)return null;
        var at=RotateOffset(garden.Cell,0,1-garden.Depth,garden.Rotation);int rotation=(garden.Rotation+2)%4;
        var entrance=Door(at,rotation);
        string? problem=Food.Celebrating?"Wait until supper finishes.":
            !Footprint(garden).ToHashSet().SetEquals(Footprint(at,rotation,garden.Kind))?"Keep the same planted ground.":
            Riverside.Plans.Any(p=>Footprint(p.Cell,p.Rotation,p.Kind).Contains(entrance))?"A clearing plan uses the new entrance.":RelocationGeometryProblem(garden,at,rotation);
        var route=FindPath(garden.Entrance,YardAccess,Blocked)?.ToArray() ?? System.Array.Empty<Cell>();
        var next=FindPath(entrance,YardAccess,Blocked)?.ToArray() ?? System.Array.Empty<Cell>();
        return new(at,rotation,entrance,route.Length,next.Length,route,next,problem);
    }
    public bool ChangeGardenEntrance(int id)
    {
        if(ReadGardenEntrance(id) is not {Problem:null} plan)return false;
        var garden=Cottages.Single(c=>c.Id==id);
        foreach(var p in People.Where(p=>MoveAffects(p,garden)).ToArray())Interrupt(p);
        garden.Cell=plan.Cell;garden.Rotation=plan.Rotation;
        foreach(var p in People.Where(p=>p.Route.Count>0))SetRoute(p,p.Destination);
        History.Add($"Garden entrance changed. Walk to food stores: {plan.Before} → {plan.After} steps. Crop and planted ground retained.");
        _retry=0;return true;
    }
}

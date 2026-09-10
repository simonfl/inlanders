using System.Linq;
using System.Numerics;

namespace Inlanders.Simulation;

public sealed record SupplyRoute(int WorkerId, string Worker, string Purpose, string Destination,
    Resource? Cargo, int Amount, Vector2 Start, Cell[] Steps, bool OnWater)
{
    public float Distance
    {
        get { float distance=0; var point=Start; foreach(var step in Steps) { distance+=Vector2.Distance(point,step.Point); point=step.Point; } return distance; }
    }
}

public sealed partial class World
{
    // Observe the route already claimed by the simulation; never recompute a hypothetical trip.
    public SupplyRoute[] ReadSupplyRoutes() => People.Select(p=>
    {
        var boat=PassengerBoat(p);
        bool water=boat!=null;
        var steps=(water ? boat!.Route : p.Route).ToArray();
        if(steps.Length==0 || !water && p.Task is not (Work.ToTree or Work.ToStockpile or Work.ToMaterials or Work.ToCottage or
            Work.ToBush or Work.ToFarm or Work.ToGrain or Work.ToOven or Work.ToBread or Work.ToPantry or
            Work.ToSawLogs or Work.ToSawmill or Work.ToPlanks or Work.ToHaulPickup or Work.ToHaulDrop or Work.ToDock or Work.ToQuarry)) return null;
        int amount=water ? boat!.Fish : p.Carried;
        Resource? cargo=amount>0 ? water ? Resource.Fish : p.Cargo : null;
        string store=p.StorageId is int id ? $"Stockpile {id}" : p.Cargo==Resource.Stone ? "Central stone store" : p.Cargo==Resource.Planks ? "Central plank store" : "Timber yard";
        string destination=water ? boat!.Phase==BoatPhase.Returning ? "Fishing dock" : "Fishing ground" : p.Task switch {
            Work.ToPantry or Work.ToGrain=>"Central pantry",
            Work.ToStockpile=>store,
            Work.ToHaulPickup or Work.ToHaulDrop or Work.ToSawLogs=>store,
            Work.ToMaterials=>store,
            Work.ToTree=>"Timber", Work.ToBush=>"Berry patch", Work.ToFarm=>"Field / garden",
            Work.ToOven or Work.ToBread=>"Bakery", Work.ToSawmill or Work.ToPlanks=>"Sawmill",
            Work.ToQuarry=>"Stone outcrop", Work.ToDock=>"Fishing dock", _=>"Construction site"
        };
        return new SupplyRoute(p.Id,p.Name,p.Status,destination,cargo,amount,water?boat!.Position:p.Position,steps,water);
    }).OfType<SupplyRoute>().ToArray();
}

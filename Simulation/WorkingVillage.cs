using System;
using System.Linq;
namespace Inlanders.Simulation;

public sealed partial class World
{
    public static World NewWorkingVillage()
    {
        var w=NewRiverFarmstead();w.Cottages.Clear();w._nextSite=1;
        foreach(var p in w.People)p.HomeId=null;
        w.Map.Name="Le détour · The long way home";w.Founding!.WorkingVillage=true;
        // A tidal inlet separates worked northern ground from the settled home bank.
        // Its western head remains walkable; a crossing is a choice, not an access gate.
        for(int x=0;x<=8;x++)w.Map.Water.Add(new(x,2));
        Cottage Ready(Cell at,BuildingKind kind)
        {
            var c=w.Place(at,0,kind)??throw new InvalidOperationException($"Working village {kind} {at}: {w.PlacementProblem(at,0,kind)}");
            c.Delivered=c.Required;c.Construction=1;return c;
        }
        foreach(var at in new[]{new Cell(2,6),new(6,6),new(2,10),new(6,10)})Ready(at,BuildingKind.Cottage);
        var field=Ready(new(3,-3),BuildingKind.Farm);field.Planted=true;field.Growth=.65f;
        Ready(new(6,-3),BuildingKind.Bakery);
        w._yardLogs=16;w.InitialLogs=w.Trees.Sum(t=>t.Logs)+w.Cottages.Sum(c=>c.Delivered)+w._yardLogs;
        w.History.Clear();w.History.Add("The houses stand south of the inlet; worked grain land and the oven are north. Life works, but the long way round shapes every day. What would you change?");
        w.ReconcileHomes();w.Validate();w.ValidateMapOccupancy();return w;
    }
}

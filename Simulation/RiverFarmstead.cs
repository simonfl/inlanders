using System;
using System.Linq;

namespace Inlanders.Simulation;

public sealed partial class World
{
    public static World NewRiverFarmstead()
    {
        var w = new World(0);
        w.Trees.Clear(); w.Bushes.Clear(); w._nextTree = 0;
        w.Map = new() { Name = "Une place à nous · A place of our own", MinX = -12, MinZ = -12, Width = 28, Depth = 26 };
        // An open river edge, useful cleared ground, and a woodlot to retain or cut.
        for (int z = w.Map.MinZ; z <= w.Map.MaxZ; z++)
            for (int x = w.Map.MinX; x <= w.Map.MaxX; x++)
            {
                var cell = new Cell(x,z);
                if (x < -10 && (z < -9 || z > 10)) w.Map.Excluded.Add(cell);
                else if (x >= 9 + (z < -5 ? 1 : z > 6 ? -1 : 0)) w.Map.Water.Add(cell);
            }
        foreach (var cell in new[] { new Cell(-9,-8), new(-6,-8), new(-3,-8), new(-9,-5), new(-6,-5), new(-3,-5),
            new(-10,7), new(-7,9), new(-4,11), new(0,11), new(4,11), new(5,-9), new(1,-10), new(-10,0) })
            w.Trees.Add(new() { Id=w._nextTree++, Cell=cell, Logs=8 });
        foreach (var cell in new[] { new Cell(-8,-2), new(-9,4) })
            w.Bushes.Add(new() { Id=w.Bushes.Count, Cell=cell });
        w.Map.FishingGrounds.Add(new() { Id=0, Name="Reed shallows", Cell=new(11,3), Capacity=16, Stock=16, RegrowthPerSecond=.1f });
        w.Map.FishingGrounds.Add(new() { Id=1, Name="Upstream water", Cell=new(12,-6), Capacity=16, Stock=16, RegrowthPerSecond=1f/15 });
        w.Map.StoneDeposits.Add(new() { Id=0, Cell=new(-10,10), Capacity=36, Remaining=36 });
        var habitat = new WoodlandHabitat { Id=0, Cell=new(-6,-7) };
        habitat.Stock=w.HabitatCapacity(habitat); w.Map.Wildlife.Add(habitat);
        var home=w.Place(new(-3,0),0,BuildingKind.Cottage) ?? throw new InvalidOperationException("Farmstead home rejected");
        home.Delivered=home.Required; home.Construction=1;
        w._yardLogs=16; w.InitialLogs=w.Trees.Sum(t=>t.Logs)+home.Required+w._yardLogs;
        w.Food.InitialBerries=w.Food.Berries=48;
        w.SharedWork=true; w.LocalGrainSupply=true; w.Founding=new() { RiverFarmstead=true };
        foreach(var person in w.People) w.Assign(person.Id,Role.Unassigned);
        w.History.Add("A place of our own. The river brought us here; the work of making a home begins on its banks.");
        w.ReconcileHomes(); w.Validate(); w.ValidateMapOccupancy(); return w;
    }

    public bool FoundingHasNewFood => DeliveredEdible>0;

    private string? FarmsteadReadyProblem() => Housed<Population ? "Give everyone a finished home." :
        DeliveredEdible==0 ? "Establish a food source and bring its first food into storage." :
        People.Any(p=>!p.Fed) ? "Some neighbors missed a meal. Restore food service before finishing." : null;
}

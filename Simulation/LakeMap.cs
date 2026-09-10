using System;
using System.Linq;

namespace Inlanders.Simulation;

public sealed partial class World
{
    // Authored lake shared by the seventh campaign and focused fishery checks.
    public static World NewLakeMap()
    {
        var w=new World(0);
        w.Trees.Clear(); w.Bushes.Clear(); w._nextTree=0;
        w.Map=new() { Name="Life by the lake",MinX=-12,MinZ=-13,Width=31,Depth=27 };
        for(int x=w.Map.MinX;x<=w.Map.MaxX;x++) for(int z=w.Map.MinZ;z<=w.Map.MaxZ;z++)
        {
            var cell=new Cell(x,z);
            if(Math.Pow((x-3)/15d,2)+Math.Pow(z/13d,2)>1) w.Map.Excluded.Add(cell);
            else if(Math.Pow((x-8)/5.5d,2)+Math.Pow(z/8.5d,2)<=1) w.Map.Water.Add(cell);
        }
        foreach(var cell in new[]{new Cell(-8,-4),new(-5,-5),new(-2,-5),new(1,-6),new(-8,4),new(-5,7),new(-2,9),new(4,-10),new(9,-10),new(15,-4),new(15,4),new(8,10)})
            w.Trees.Add(new() { Id=w._nextTree++,Cell=cell,Logs=8 });
        foreach(var cell in new[]{new Cell(-8,0),new(1,7),new(14,6)}) w.Bushes.Add(new() { Id=w.Bushes.Count,Cell=cell });
        w.InitialLogs=w.Trees.Sum(t=>t.Logs);
        void Ready(Cell cell,BuildingKind kind)
        {
            var site=w.Place(cell,false,kind) ?? throw new InvalidOperationException($"Lake setup rejected {kind} at {cell}: {w.PlacementProblem(cell,false,kind)}");
            site.Delivered=site.Required; site.Construction=1; w.InitialLogs+=site.Required;
        }
        Ready(new(-6,3),BuildingKind.ForagerHut);
        foreach(var cell in new[]{new Cell(-6,-2),new(-3,0),new(0,0),new(0,6)}) Ready(cell,BuildingKind.Cottage);
        w.Map.FishingGrounds.Add(new() { Id=0,Name="Willow shallows",Cell=new(5,3),Capacity=12,Stock=12,RegrowthPerSecond=1f/15 });
        w.Map.FishingGrounds.Add(new() { Id=1,Name="Deep reed beds",Cell=new(11,1),Capacity=24,Stock=24,RegrowthPerSecond=.12f });
        w.Food.InitialBerries=w.Food.Berries=40;
        var roles=new[]{Role.Logger,Role.Logger,Role.Builder,Role.Builder,Role.Forager,Role.Forager,Role.Unassigned,Role.Unassigned};
        for(int i=0;i<w.Population;i++) w.Assign(i,roles[i]);
        w.ReconcileHomes(); w.Validate(); w.ValidateMapOccupancy(); return w;
    }
}

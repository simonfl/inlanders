using System;
using System.Linq;

namespace Inlanders.Simulation;

public sealed partial class World
{
    // F26b2a layout prototype. Campaign progression is added separately.
    public static World NewQuarryMap()
    {
        var w=new World(0);
        w.Trees.Clear();w.Bushes.Clear();w._nextTree=0;
        w.Map=new(){Name="Built to last",MinX=-10,MinZ=-9,Width=28,Depth=20};
        for(int x=-10;x<=17;x++)for(int z=-9;z<=10;z++)
            if(!(x<=5 || x<=9 && z>=-3 && z<=2 || x>=10 && z>=-7 && z<=7))w.Map.Excluded.Add(new(x,z));
        foreach(var cell in new[]{new Cell(-9,4),new(-9,7),new(-5,-8),new(-1,-7),new(4,-7),new(11,5),new(13,4),new(15,0)})
            w.Trees.Add(new(){Id=w._nextTree++,Cell=cell,Logs=8});
        foreach(var cell in new[]{new Cell(-9,0),new(-8,9)})w.Bushes.Add(new(){Id=w.Bushes.Count,Cell=cell});
        w.Map.StoneDeposits.AddRange(new[]{new StoneDeposit{Id=0,Cell=new(-7,-5),Capacity=8,Remaining=8},new StoneDeposit{Id=1,Cell=new(14,-5),Capacity=36,Remaining=36}});
        w._yardLogs=16;w.InitialLogs=80;
        void Ready(Cell cell,BuildingKind kind)
        {
            var site=w.Place(cell,0,kind)??throw new InvalidOperationException($"Quarry map {kind} at {cell}: {w.PlacementProblem(cell,0,kind)}");
            site.Delivered=site.Required;site.Construction=1;w.InitialLogs+=site.Required;
        }
        foreach(var cell in new[]{new Cell(-6,0),new(-6,4),new(-1,7),new(3,7)})Ready(cell,BuildingKind.Cottage);
        Ready(new(-6,7),BuildingKind.ForagerHut);Ready(new(2,0),BuildingKind.VegetableGarden);
        w.Food.InitialBerries=w.Food.Berries=72;
        var roles=new[]{Role.Logger,Role.Builder,Role.Builder,Role.Forager,Role.Forager,Role.Farmer,Role.Unassigned,Role.Unassigned};
        for(int i=0;i<w.Population;i++)w.Assign(i,roles[i]);
        w.ReconcileHomes();w.Validate();w.ValidateMapOccupancy();return w;
    }
}

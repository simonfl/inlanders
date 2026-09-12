using System;
using System.Linq;

namespace Inlanders.Simulation;

public sealed partial class World
{
    // F26c2a authored layout; campaign progression is a separate chunk.
    public static World NewLivingWoodsMap()
    {
        var w=NewQuarryMap();
        w.Map=new(){Name="The living woods",MinX=-12,MinZ=-11,Width=30,Depth=24};
        for(int x=-12;x<=17;x++)for(int z=-11;z<=12;z++)
            if(x+z>23 || x-z>23 || -x+z>20 || -x-z>19)w.Map.Excluded.Add(new(x,z));
        w.Trees.Clear();w._nextTree=0;
        foreach(var cell in new[]{new Cell(-9,-5),new(-8,-8),new(-5,-8),new(-3,-6),new(-7,-3),new(-4,-3),
            new(6,-6),new(7,-8),new(10,-8),new(12,-6),new(11,-3),new(8,-3),new(-10,6),new(12,6),new(7,8)})
            w.Trees.Add(new(){Id=w._nextTree++,Cell=cell,Logs=8});
        foreach(var cell in new[]{new Cell(-6,-5),new(9,-5)})
        {
            var h=new WoodlandHabitat{Id=w.Map.Wildlife.Count,Cell=cell};h.Stock=w.HabitatCapacity(h);w.Map.Wildlife.Add(h);
        }
        w.InitialLogs=w.YardLogs+w.Trees.Sum(t=>t.Logs)+w.Cottages.Sum(c=>c.Delivered);
        w.Food.InitialBerries=w.Food.Berries=32;
        w.Validate();w.ValidateMapOccupancy();return w;
    }
}

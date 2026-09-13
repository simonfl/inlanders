using System;
using System.Linq;

namespace Inlanders.Simulation;

public sealed partial class World
{
    // Comparison-only landscape: identical west hamlet, resource quantities and rules.
    public static World NewNeighborhoodLandscapeExperiment()
    {
        var w=NewNeighborhoodExperiment();
        w.Map.Name="Landing and meadow — experiment";
        w.Map.Width=35;
        for(int x=6;x<=w.Map.MaxX;x++)for(int z=w.Map.MinZ;z<=w.Map.MaxZ;z++)
        {
            bool landing=x<=10 && z>=-1 && z<=5;
            bool passage=x>=11 && x<=13 && z>=3 && z<=5;
            bool meadow=x>=14 && x<=24 && z>=-4 && z<=12 && Math.Abs(x-19)+Math.Abs(z-4)<=13;
            if(landing || passage || meadow)w.Map.Excluded.Remove(new(x,z));
            else w.Map.Excluded.Add(new(x,z));
        }
        w.Trees.RemoveAll(t=>t.Cell.X>5);
        foreach(var cell in new[]{new Cell(14,-1),new(16,-2),new(18,-1),new(14,1),new(18,2),
            new(16,4),new(18,6),new(14,9),new(16,11),new(18,10)})
            w.Trees.Add(new(){Id=w._nextTree++,Cell=cell,Logs=8});
        w.Bushes.RemoveAll(b=>b.Cell.X>5);
        foreach(var cell in new[]{new Cell(14,7),new(19,8)})
            w.Bushes.Add(new(){Id=w.Bushes.Count,Cell=cell});
        // Initial resources are relocated, never minted or removed during play.
        w.Validate();w.ValidateMapOccupancy();return w;
    }
}

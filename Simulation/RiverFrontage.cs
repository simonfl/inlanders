using System;
using System.Linq;
namespace Inlanders.Simulation;
public sealed partial class World
{
    // Same people, reserves and rules as the inlet; geography is the comparison.
    public static World NewRiverFrontage(bool relaxed=false)
    {
        var w=NewPlayerFounded();w.Founding!.RiverFrontage=true;
        w.Map.Name="La terre au fleuve · River frontage";w.Map.Water.Clear();
        for(int z=w.Map.MinZ;z<=w.Map.MaxZ;z++)for(int x=w.Map.MinX;x<=w.Map.MaxX;x++)
        {
            int edge=z<-8?7:z<-3?8:z<4?10:z<9?9:7;
            var c=new Cell(x,z);if(w.Map.Contains(c) && x>=edge)w.Map.Water.Add(c);
        }
        w.Map.FishingGrounds.Clear();
        w.Map.FishingGrounds.Add(new(){Id=0,Name="Sheltered bend",Cell=new(11,8),Capacity=16,Stock=16,RegrowthPerSecond=.1f});
        w.Map.FishingGrounds.Add(new(){Id=1,Name="Upper river",Cell=new(11,-8),Capacity=16,Stock=16,RegrowthPerSecond=1f/15});
        w.History.Clear();w.History.Add("A broad river bends beside open growing ground and a rising woodlot. Settle near the landing, gather around a shared clearing, or stretch homes along the bank. The same eight neighbors and starting supplies as the inlet; no required arrangement.");
        w.Validate();w.ValidateMapOccupancy();return relaxed?RelaxedHamletFrom(w):w;
    }
}

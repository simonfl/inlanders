using System.Linq;

namespace Inlanders.Simulation;

public sealed partial class World
{
    // Shared authored layout for the decision experiments and campaign level ten.
    public static World NewLastingVillageMap()
    {
        var w=NewQuarryMap();
        w.Map=new(){Name="A lasting village · prototype",RiverMeadow=true,MinX=-10,MinZ=-8,Width=36,Depth=22};
        for(int x=w.Map.MinX;x<=w.Map.MaxX;x++)for(int z=w.Map.MinZ;z<=w.Map.MaxZ;z++)
        {
            bool inlet=x>=3 && x<=5 && z>=2 && z<=5 && z!=3;
            bool west=x>=-9 && x<=5 && z>=-1 && z<=9 && !inlet &&
                !(z==-1 && (x<-7 || x>3)) && !(z==9 && x<-5);
            bool east=x>=7 && x<=24 && z>=-6 && z<=12 && x+z<=32 && x-z<=28 &&
                !(x==7 && z<-2) && !(z==-6 && x<10) && !(x==24 && z>=2 && z<=6) && !(z==12 && x<8);
            bool water=x==6 && z>=-3 && z<=12 || inlet ||
                x>=4 && x<=5 && z>=-3 && z<=-2 || x>=5 && x<=7 && z==12;
            if(water)w.Map.Water.Add(new(x,z));
            else if(!west && !east)w.Map.Excluded.Add(new(x,z));
        }
        w.Bushes.RemoveAll(b=>b.Id!=0);
        w.Trees.Clear();w._nextTree=0;
        foreach(var cell in new[]{new Cell(-8,2),new(-9,5),new(-8,8),
            new(8,-5),new(11,-5),new(12,-4),new(14,-6),new(19,-4),new(21,-2),
            new(23,1),new(22,4),new(10,11),new(12,10),new(14,11),new(17,10)})
            w.Trees.Add(new(){Id=w._nextTree++,Cell=cell,Logs=8});
        w.Food.InitialBerries=w.Food.Berries=32;
        w.InitialLogs=w.YardLogs+w.Trees.Sum(t=>t.Logs)+w.Cottages.Sum(c=>c.Delivered);
        w.Validate();w.ValidateMapOccupancy();return w;
    }
}

using System;
using System.Linq;
namespace Inlanders.Simulation;
public sealed partial class World
{
    public static World RelaxedHamletFrom(World source)
    {
        if(source.Founding?.TransformationHamlet!=true)throw new ArgumentException("Choose the transformation hamlet.");
        var w=LoadJson(source.SaveJson());w.Creative=true;w.Food.Hunger=0;
        w.History.Add("Relaxed hamlet: the same meals, work and homes; free construction and no hunger penalties.");w.Validate();return w;
    }
    public static World NewTransformationHamlet(bool relaxed=false)
    {
        var w=NewRiverFarmstead();w.Cottages.Clear();w.Trees.Clear();w.Bushes.Clear();w._nextSite=1;w._nextTree=0;
        foreach(var p in w.People)p.HomeId=null;
        w.Map=new(){Name="Entre bois et rivière · Between wood and water",MinX=-12,MinZ=-14,Width=28,Depth=29};
        w.Founding!.TransformationHamlet=true;
        for(int z=w.Map.MinZ;z<=w.Map.MaxZ;z++)for(int x=w.Map.MinX;x<=w.Map.MaxX;x++)
        {
            var c=new Cell(x,z);
            if(x<-10 || z>12 && x<-6 || z<-12 && x<-5)w.Map.Excluded.Add(c);
            else if(x>=9 || z==0 && x>=-4)w.Map.Water.Add(c);
        }
        // The western woodlot rises gently; the house bank, inlet and northern meadow remain level.
        w.Map.Heights=new float[(w.Map.Width+1)*(w.Map.Depth+1)];
        for(int z=0;z<=w.Map.Depth;z++)for(int x=0;x<=w.Map.Width;x++)
            w.Map.Heights[z*(w.Map.Width+1)+x]=.3f*Math.Clamp(-(w.Map.MinX+x-.5f)-5.5f,0,4);
        Cottage Ready(Cell c,BuildingKind kind,int rotation=0)
        {
            var b=w.Place(c,rotation,kind)??throw new InvalidOperationException($"Hamlet {kind} {c}: {w.PlacementProblem(c,0,kind)}");
            b.Delivered=b.Required;b.Construction=1;return b;
        }
        foreach(var c in new[]{new Cell(-3,7),new(1,7),new(5,7),new(-3,11),new(1,11),new(5,11)})Ready(c,BuildingKind.Cottage,c.X==-3?1:c.X==1?3:c.Z==11?2:0);
        foreach(var c in new[]{new Cell(-1,-5),new(4,-5)}){var b=Ready(c,BuildingKind.VegetableGarden);b.Planted=true;b.Growth=.6f;}
        // These working woods compete with nearby domestic expansion; the northern meadow is further away.
        foreach(var c in new[]{new Cell(-8,3),new(-8,6),new(-8,9),new(-6,12),new(-8,-3),new(-8,-6),new(-8,-9),new(-5,-10),new(-2,-11),new(6,13)})
            w.Trees.Add(new(){Id=w._nextTree++,Cell=c,Logs=8,Preserved=true});
        w.Bushes.Add(new(){Id=0,Cell=new(-8,12)});
        w.Map.FishingGrounds.Add(new(){Id=0,Name="Southern shallows",Cell=new(11,8),Capacity=16,Stock=16,RegrowthPerSecond=.1f});
        w.Map.FishingGrounds.Add(new(){Id=1,Name="Upper river",Cell=new(11,-8),Capacity=16,Stock=16,RegrowthPerSecond=1f/15});
        var habitat=new WoodlandHabitat{Id=0,Cell=new(-6,-6)};habitat.Stock=w.HabitatCapacity(habitat);w.Map.Wildlife.Add(habitat);
        w.Map.StoneDeposits.Add(new(){Id=0,Cell=new(-8,-12),Capacity=36,Remaining=36});
        w._yardLogs=12;w.InitialLogs=w.Trees.Sum(t=>t.Logs)+w.Cottages.Sum(c=>c.Delivered)+w._yardLogs;
        w.Food.InitialBerries=w.Food.Berries=72;
        if(!w.InviteNewcomers() || !w.InviteNewcomers())throw new InvalidOperationException("Hamlet households refused");
        w.History.Clear();w.History.Add("Twelve neighbors, crowded homes and two gardens beyond the inlet. Keep the woodland, open it for homes, or work the distant meadow. The first timber is limited; choose what to change first.");
        w.ReconcileHomes();w.Validate();w.ValidateMapOccupancy();return relaxed?RelaxedHamletFrom(w):w;
    }
}

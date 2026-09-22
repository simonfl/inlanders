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
    public static World NewTransformationHamlet(bool relaxed=false,bool cultivatedBank=false,bool groupedFarmsteads=false,bool acrossTheInlet=false)
    {
        if(groupedFarmsteads || acrossTheInlet)cultivatedBank=true;
        if(groupedFarmsteads && acrossTheInlet)throw new ArgumentException("Choose one layout.");
        var w=NewRiverFarmstead();w.Cottages.Clear();w.Trees.Clear();w.Bushes.Clear();w._nextSite=1;w._nextTree=0;
        foreach(var p in w.People)p.HomeId=null;
        w.Map=new(){Name="Entre bois et rivière · Between wood and water",MinX=-12,MinZ=-14,Width=28,Depth=29};
        w.Founding!.TransformationHamlet=true;w.Founding.CultivatedBank=cultivatedBank;w.Founding.GroupedFarmsteads=groupedFarmsteads;w.Founding.AcrossTheInlet=acrossTheInlet;
        if(cultivatedBank)w.Map.Name="La rive cultivée · The cultivated bank";
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
        var homes=cultivatedBank && !acrossTheInlet?new[]{new Cell(-3,6),new(1,6),new(-3,11),new(1,11),new(-3,-4),new(1,-4)}:new[]{new Cell(-3,7),new(1,7),new(5,7),new(-3,11),new(1,11),new(5,11)};
        if(groupedFarmsteads)
        {
            foreach(var (at,turn) in new[]{(new Cell(-1,10),1),(new Cell(1,6),0),(new Cell(0,-6),1),(new Cell(4,-2),2),(new Cell(-4,5),1),(new Cell(-3,-3),0)})Ready(at,BuildingKind.Cottage,turn);
        }
        else foreach(var c in homes)Ready(c,BuildingKind.Cottage,cultivatedBank && !acrossTheInlet?0:c.X==-3?1:c.X==1?3:c.Z==11?2:0);
        foreach(var p in w.People)p.Position=w.Cottages[p.Id/2].Entrance.Point;
        foreach(var c in acrossTheInlet?new[]{new Cell(1,-5),new(5,-5),new(5,3)}:groupedFarmsteads?new[]{new Cell(5,11),new(5,-5),new(0,3)}:cultivatedBank?new[]{new Cell(5,6),new(5,12),new(5,-5)}:new[]{new Cell(1,3),new(5,3),new(4,-5)})
        {var b=Ready(c,cultivatedBank && (acrossTheInlet?c.Z<0:groupedFarmsteads?c.X==5:c.Z>0)?BuildingKind.VegetableField:BuildingKind.VegetableGarden,0);b.Planted=true;b.Growth=.6f;}
        // These working woods compete with nearby domestic expansion; the northern meadow is further away.
        foreach(var c in new[]{new Cell(-8,3),new(-8,6),new(-8,9),new(-6,12),new(-8,-3),new(-8,-6),new(-8,-9),new(-5,-10),new(-2,-11),new(6,13)})
            w.Trees.Add(new(){Id=w._nextTree++,Cell=c,Logs=8,Preserved=true});
        w.Bushes.Add(new(){Id=0,Cell=new(-8,12)});
        w.Map.FishingGrounds.Add(new(){Id=0,Name="Southern shallows",Cell=new(11,8),Capacity=16,Stock=16,RegrowthPerSecond=.1f});
        w.Map.FishingGrounds.Add(new(){Id=1,Name="Upper river",Cell=new(11,-8),Capacity=16,Stock=16,RegrowthPerSecond=1f/15});
        var habitat=new WoodlandHabitat{Id=0,Cell=new(-6,-6)};habitat.Stock=w.HabitatCapacity(habitat);w.Map.Wildlife.Add(habitat);
        w.Map.StoneDeposits.Add(new(){Id=0,Cell=new(-8,-12),Capacity=36,Remaining=36});
        w._yardPlanks=4;w.SawnLogs=2; // Existing sawn supplies: one modest domestic intervention in both comparisons.
        w._yardLogs=12;w.InitialLogs=w.SawnLogs+w.Trees.Sum(t=>t.Logs)+w.Cottages.Sum(c=>c.Delivered)+w._yardLogs;
        w.Food.InitialBerries=w.Food.Berries=72;
        if(!w.InviteNewcomers() || !w.InviteNewcomers())throw new InvalidOperationException("Hamlet households refused");
        w.History.Clear();w.History.Add("Three gardens feed twelve neighbors. The kitchen plots fill the open ground by the houses. Keep food close, or make room for a shared place and grow beyond the inlet. The woodlot and northern meadow offer different ways to reshape this hamlet.");
        if(cultivatedBank){w.History.Clear();w.History.Add("Three home groups share two working fields and a northern kitchen garden. Fields occupy36 cultivated tiles in total with the garden, yielding48 vegetables per combined crop instead of the compact village’s18 tiles/24 vegetables; larger harvests take more collection work. Four planks can furnish one home. Keep crops nearby, change a yard, or open shared ground. Northern neighbors walk around the inlet; a crossing is one possible improvement, not an objective.");}
        if(groupedFarmsteads){w.Map.Name="Des maisons parmi les champs · Homes among the fields";w.History.Clear();w.History.Add("Six homes, two fields and one kitchen garden, with the same people, crop capacity, woodland and supplies as the cultivated bank. Homes face smaller working clearings on both sides of the inlet. Keep a yard close, open shared ground or change the routes; no prescribed improvement.");}
        if(acrossTheInlet){w.Map.Name="Across the inlet";w.History.Clear();w.History.Add("Most crops grow beyond the inlet; the homes share a crowded bank. The kitchen garden and existing stores keep daily life going. Keep this place, bring work and home closer, or change the journey between them. Nothing has to be built to finish.");}
        // These are actual editable paths: they affect travel, and stop at real entrances.
        foreach(var site in w.Cottages.OrderBy(c=>IsVegetablePlot(c.Kind)?0:1).ThenBy(c=>c.Id))
            if(!w.ConnectPaths(w.YardAccess,site.Entrance))throw new InvalidOperationException("Hamlet approach unavailable");
        w.Founding.StartingBuildings=w.Cottages.Select(c=>new StartingBuilding{Id=c.Id,Cell=c.Cell,Rotation=c.Rotation,Kind=c.Kind}).ToList();
        w.ReconcileHomes();w.Validate();w.ValidateMapOccupancy();return relaxed?RelaxedHamletFrom(w):w;
    }
}


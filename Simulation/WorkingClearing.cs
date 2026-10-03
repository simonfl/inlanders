using System;
using System.Linq;
namespace Inlanders.Simulation;
public sealed partial class World
{
    public static World NewWorkingClearing(bool relaxed=false,bool original=false)
    {
        var w=NewRiverFrontage();w.Founding!.PlayerFounded=false;w.Founding.WorkingClearing=true;w.Founding.AcrossTheInlet=false;
        // A gradual bend connects the field ground to the home shore.
        // Shared ground and nearby cultivation may coexist; this is not a binary puzzle.
        for(int z=-6;z<=10;z++)
        {
            int edge=z<-5?8:z<-3?7:z<-1?6:z<3?5:z<9?6:7;
            for(int x=edge;x<=w.Map.MaxX;x++)if(w.Map.Contains(new(x,z)))w.Map.Water.Add(new(x,z));
        }
        for(int z=0;z<=w.Map.Depth;z++)for(int x=0;x<=w.Map.Width;x++)
        {
            float px=w.Map.MinX+x-.5f,pz=w.Map.MinZ+z-.5f;
            float rise=.28f*Math.Clamp(pz-10.5f,0,3)*Math.Clamp((6-px)/3,0,1);
            int index=z*(w.Map.Width+1)+x;w.Map.Heights[index]=Math.Max(w.Map.Heights[index],rise);
        }
        foreach(var p in w.People)p.Position=w.YardAccess.Point;
        Cottage Ready(Cell at,BuildingKind kind,int turn=0,int rows=0)
        {
            var b=w.Place(at,turn,kind,rows)??throw new InvalidOperationException($"Clearing {kind} {at}/{turn}: {w.PlacementProblem(at,turn,kind,rows)}");
            b.Delivered=b.Required;b.Construction=1;b.EstablishmentPending=false;return b;
        }
        var homes=original?new[]{(new Cell(-3,9),0),(new Cell(3,9),0),(new Cell(-3,5),0),(new Cell(3,5),0)}:
            new[]{(new Cell(-3,9),0),(new Cell(3,9),3),(new Cell(-3,5),1),(new Cell(3,3),0)};
        foreach(var (at,turn) in homes)Ready(at,BuildingKind.Cottage,turn);
        foreach(var (at,growth) in original?new[]{(new Cell(-1,-5),.6f),(new Cell(-1,-9),.2f)}:new[]{(new Cell(-1,-1),.6f),(new Cell(-1,-5),.2f)}){var field=Ready(at,BuildingKind.VegetableField,1,2);field.Planted=true;field.Growth=growth;}
        w.ReconcileHomes();foreach(var p in w.People)p.Position=w.Cottages.Single(c=>c.Id==p.HomeId).Entrance.Point;
        foreach(var home in w.Cottages.Where(c=>c.Kind==BuildingKind.Cottage))w.ConnectPaths(w.YardAccess,home.Entrance);
        w._yardLogs=16;w.InitialLogs=w.SawnLogs+w.Trees.Sum(t=>t.Logs)+w.Cottages.Sum(c=>c.Delivered)+w._yardLogs;
        w.Food.InitialBerries=w.Food.Berries=80;
        w.Founding.StartingBuildings=w.Cottages.Select(c=>new StartingBuilding{Id=c.Id,Cell=c.Cell,Rotation=c.Rotation,Kind=c.Kind}).ToList();
        w.Map.Name="La clairière · A working clearing";
        w.History.Clear();w.History.Add("Eight neighbors have homes beside the river. Their two working strips lead from the homes toward the woods. Open ground joins the homes, fields and river. Bring work closer, make room to share a meal, or combine them in your own arrangement. Sixteen logs, four planks and eighty food portions leave time to choose. Bring growing ground closer, extend it, make a landing, or leave this modest place as it is. No required improvement or arrivals.");
        w.Validate();w.ValidateMapOccupancy();return relaxed?RelaxedHamletFrom(w):w;
    }
}

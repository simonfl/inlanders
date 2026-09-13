using System;
using System.Linq;

namespace Inlanders.Simulation;

public sealed class ArrangementTrial
{
    public int? BuildingId { get; set; }
    public Cell Original { get; set; }
    public int Rotation { get; set; }
}

public sealed partial class World
{
    public bool IsArrangementCourt=>Neighborhood?.Arrangement!=null;
    public static World NewArrangementCourt()
    {
        var w=NewInheritedShoreline();w.Neighborhood!.Arrangement=new();w.Map.Name="Willow court";
        var homes=w.Cottages.Where(c=>Buildings.Get(c.Kind).Beds>0).ToArray();
        var positions=new[]{(new Cell(-4,0),0),(new Cell(-1,0),0),(new Cell(2,1),1),(new Cell(-4,5),2)};
        for(int i=0;i<homes.Length;i++){homes[i].Cell=positions[i].Item1;homes[i].Rotation=positions[i].Item2;}
        var venue=w.Cottages.Single(c=>c.Kind==BuildingKind.SeatingGarden);venue.Cell=new(-1,3);
        var ground=w.Cottages.SelectMany(c=>Footprint(c.Cell,c.Rotation,c.Kind).Append(c.Entrance)).ToHashSet();
        // Relocate conflicting inherited trees, preserving timber capability and accounting.
        foreach(var tree in w.Trees.Where(t=>ground.Contains(t.Cell) || ground.Contains(t.Access) || t.Cell.X>=9 && t.Cell.X<=16 && t.Cell.Z>=0 && t.Cell.Z<=8).ToArray())
        {
            var at=w.Map.Land.Where(c=>!ground.Contains(c) && !ground.Contains(new(c.X+1,c.Z)) &&
                !w.Blocked(c) && !w.Blocked(new(c.X+1,c.Z)) && !w.Trees.Any(t=>t.Access==c))
                .OrderByDescending(c=>Math.Abs(c.X)+Math.Abs(c.Z)).First();
            w.Trees[w.Trees.IndexOf(tree)]=new(){Id=tree.Id,Cell=at,Logs=tree.Logs,Growth=tree.Growth};
        }
        w.Paths.Clear();
        for(int x=-5;x<=3;x++)for(int z=1;z<=4;z++)if(!w.Blocked(new(x,z)))w.Paths.Add(new(x,z));
        foreach(var site in w.Cottages)
        {
            var route=w.FindPath(site.Entrance,w.YardAccess,w.Blocked);
            if(route!=null)foreach(var c in route)w.Paths.Add(c);
        }
        foreach(var person in w.People)
        {
            var home=homes.Single(c=>c.Id==person.HomeId);
            person.Position=home.Entrance.Point;person.Route.Clear();
        }
        w.History.Clear();w.History.Add("Willow court: choose a household, follow its meal, and try one building in another place. Welcoming neighbors is optional.");
        w.Validate();w.ValidateMapOccupancy();return w;
    }
    public string? RestoreArrangementProblem()
    {
        var trial=Neighborhood?.Arrangement;
        return trial?.BuildingId is int id?RelocationProblem(id,trial.Original,trial.Rotation):"No arrangement to restore.";
    }
    public bool RestoreArrangement()
    {
        var trial=Neighborhood?.Arrangement;
        if(trial?.BuildingId is not int id || RestoreArrangementProblem()!=null)return false;
        if(!MoveBuilding(id,trial.Original,trial.Rotation))return false;
        trial.BuildingId=null;return true;
    }
}

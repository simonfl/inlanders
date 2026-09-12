using System.Collections.Generic;
using System.Linq;

namespace Inlanders.Simulation;

public sealed partial class World
{
    public string? BushMoveProblem(int id)
    {
        if(!Creative)return "Moving berry bushes is available in Creative.";
        if(!Bushes.Any(b=>b.Id==id))return "Choose an existing berry bush.";
        return Food.Celebrating?"Wait until supper finishes.":null;
    }
    public string? BushMoveProblem(int id,Cell at)
    {
        if(BushMoveProblem(id) is string problem)return problem;
        var bush=Bushes.Single(b=>b.Id==id);
        if(bush.Cell==at)return null;
        var access=new Cell(at.X+1,at.Z);
        if(Paths.Contains(at))return "Keep paths clear. Choose bare ground for the bush.";
        if(ManagedWoodland.Contains(at) || ManagedWoodland.Contains(access))return "Keep managed woodland orders clear, including the picking spot.";
        if(Map.Water.Contains(at) || Map.Water.Contains(access))return "The bush and its east-side picking spot need dry land.";
        var before=Reachable(YardAccess,Blocked);
        var protectedTrips=People.Where(p=>p.BushId!=id && p.Route.Count>0).Select(p=>p.Destination).Where(before.Contains).ToArray();
        int index=Bushes.IndexOf(bush);Bushes.RemoveAt(index);
        try
        {
            var placementProblem=CheckPlacement(new HashSet<Cell>{at},access);
            if(placementProblem!=null)return placementProblem;
            var after=Reachable(YardAccess,c=>Blocked(c) || c==at);
            if(protectedTrips.Any(c=>!after.Contains(c)))return "Keep existing worker destinations reachable.";
            return null;
        }
        finally{Bushes.Insert(index,bush);}
    }
    public bool MoveBush(int id,Cell at)
    {
        if(BushMoveProblem(id,at)!=null)return false;
        var bush=Bushes.Single(b=>b.Id==id);
        if(bush.Cell==at)return true;
        // Harvested cargo no longer has a BushId and keeps its delivery/food claims.
        foreach(var person in People.Where(p=>p.BushId==id).ToArray())Interrupt(person);
        bush.Cell=at;
        foreach(var person in People.Where(p=>p.Route.Count>0))SetRoute(person,person.Destination);
        History.Add($"Moved berry bush {id} to {at.X}, {at.Z}; ripe berries and regrowth retained.");
        _retry=0;return true;
    }
}

using System;
using System.Linq;
using System.Collections.Generic;

namespace Inlanders.Simulation;

public sealed class CourtStartingBuilding
{
    public int Id { get; set; }
    public Cell Cell { get; set; }
    public int Rotation { get; set; }
    public BuildingKind Kind { get; set; }
}
public sealed class CourtExperience
{
    public bool Finite { get; set; }
    public bool Finished { get; set; }
    public List<CourtStartingBuilding> StartingBuildings { get; set; } = new();
}
public sealed partial class World
{
    public CourtExperience? CourtStudy=>Neighborhood?.Arrangement?.Experience;
    public static World NewCourtExperience(bool finite)
    {
        var w=NewCreativeCourt();
        foreach(var target in new[]{new Cell(-5,-3),new(-1,-3),new(1,6),new(10,6)})
        {
            var cell=w.Map.Land.OrderBy(c=>(c.Point-target.Point).LengthSquared()).ThenBy(c=>c.Z).ThenBy(c=>c.X)
                .First(c=>w.PlacementProblem(c,0,BuildingKind.Cottage)==null);
            if(w.Place(cell,0,BuildingKind.Cottage)==null)throw new InvalidOperationException("Court home placement failed");
        }
        if(!w.InviteNewcomers())throw new InvalidOperationException("Court invitation failed");
        for(int i=0;i<920;i++)w.Tick(.1f);
        if(w.Population!=16 || w.Housed!=16)throw new InvalidOperationException("Court requires sixteen housed residents");
        w.Neighborhood!.Arrangement!.Experience=new(){Finite=finite,StartingBuildings=w.Cottages.Select(c=>new CourtStartingBuilding{Id=c.Id,Cell=c.Cell,Rotation=c.Rotation,Kind=c.Kind}).ToList()};
        // Both arms use identical village state and simulation; only the brief/end point differs.
        w.Validate();return w;
    }
    public bool FinishCourtPlace()
    {
        if(CourtStudy is not {Finite:true,Finished:false} study)return false;
        study.Finished=true;return true;
    }
    private void ValidateCourtExperience()
    {
        if(CourtStudy is not {} study)return;
        if(!Creative || study.Finished && !study.Finite || study.StartingBuildings==null || study.StartingBuildings.Count==0 ||
            study.StartingBuildings.Select(b=>b.Id).Distinct().Count()!=study.StartingBuildings.Count ||
            study.StartingBuildings.Any(b=>!Map.Contains(b.Cell) || b.Rotation is <0 or >3 || !Enum.IsDefined(b.Kind)))
            throw new InvalidOperationException("Invalid court experience");
    }
}

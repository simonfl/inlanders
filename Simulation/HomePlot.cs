using System;
using System.Linq;
namespace Inlanders.Simulation;
public sealed record HomePlot(string? Problem,Cell[] Yard);
public sealed partial class World
{
    public HomePlot PreviewHomePlot(Cell at,int rotation,BuildingKind kind,int side)
    {
        if(PublicPlace==null || !Enum.IsDefined(kind) || Buildings.Get(kind).Beds==0 || side is <0 or >3)return new("Choose a home and yard side.",Array.Empty<Cell>());
        if(PlacementProblem(at,rotation,kind) is {} placement)return new(placement,Array.Empty<Cell>());
        var home=new Cottage{Id=_nextSite,Cell=at,Rotation=rotation,Kind=kind,Construction=1,YardSide=side};
        var footprint=Footprint(at,rotation,kind).ToHashSet();
        var places=YardPlaces(home,side).Where(c=>!footprint.Contains(c) && FindPath(home.Entrance,c,p=>Blocked(p)||footprint.Contains(p))!=null).ToArray();
        return new(YardClaimProblem(home,side)??(places.Length==0?"This side needs open ground for the yard. Choose another side or move the home.":null),places);
    }
    public Cottage? PlaceHomePlot(Cell at,int rotation,BuildingKind kind,int side)
    {
        if(PreviewHomePlot(at,rotation,kind,side).Problem!=null)return null;
        var home=Place(at,rotation,kind)!;home.YardSide=side;home.PlannedYard=true;ProcessPlannedYards();return home;
    }
    private void ProcessPlannedYards()
    {
        foreach(var home in Cottages.Where(c=>c.PlannedYard && c.Complete && !c.DemolitionRequested))
            if(ImprovementProblem(home.Id)==null)RequestImprovement(home.Id);
    }
    public string PlannedYardSummary(Cottage home)=>!home.PlannedYard?"":
        $"Yard planned · {(Creative?"free":ComfortCost(home)+" planks")} · "+(!home.Complete?"furnished after building and occupation.":ImprovementProblem(home.Id)??"ready for furnishing.");
}

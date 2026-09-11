using System;
using System.Collections.Generic;

namespace Inlanders.Simulation;

public sealed partial class World
{
    // Quarter-turns around the placement cell, matching Godot's positive Y rotation:
    // the entrance starts at +Z, then faces +X, -Z, and -X.
    public static Cell RotateOffset(Cell origin,int x,int z,int rotation)
    {
        if(rotation is <0 or >3)throw new ArgumentOutOfRangeException(nameof(rotation));
        return rotation switch {0=>new(origin.X+x,origin.Z+z),1=>new(origin.X+z,origin.Z-x),2=>new(origin.X-x,origin.Z-z),_=>new(origin.X-z,origin.Z+x)};
    }
    // Two-axis shorthand used by authored scenarios and their fixtures. Saved buildings use Rotation 0–3 only.
    public Cottage? Place(Cell c,bool turned,BuildingKind kind=BuildingKind.Cottage)=>Place(c,turned?1:0,kind);
    public string? PlacementProblem(Cell c,bool turned,BuildingKind kind=BuildingKind.Cottage)=>PlacementProblem(c,turned?1:0,kind);
    public bool CanPlace(Cell c,bool turned)=>CanPlace(c,turned?1:0);
    public static Cell Door(Cell c,bool turned)=>Door(c,turned?1:0);
    public static Cell FarBank(Cell c,bool turned)=>FarBank(c,turned?1:0);
    public static IEnumerable<Cell> Footprint(Cell c,bool turned,BuildingKind kind=BuildingKind.Cottage)=>Footprint(c,turned?1:0,kind);
    public string? BridgeProblem(Cell c,bool turned)=>BridgeProblem(c,turned?1:0);
    public Cell BridgeEntrance(Cell c,bool turned)=>BridgeEntrance(c,turned?1:0);
    public string? DockProblem(Cell c,bool turned)=>DockProblem(c,turned?1:0);
    public Cell DockEntrance(Cell c,bool turned)=>DockEntrance(c,turned?1:0);
    public Cell DockLaunch(Cell c,bool turned)=>DockLaunch(c,turned?1:0);
    public string FishingSurvey(Cell c,bool turned)=>FishingSurvey(c,turned?1:0);
}

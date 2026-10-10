using System.Collections.Generic;
using System.Linq;
namespace Inlanders.Simulation;
public sealed record GroundPreparation(Cell[] Trees,int Logs);
public sealed partial class World
{
    // Only propose clearance when the ordinary footprint would otherwise be legal.
    // This is a clearing order, not a queued building or a promise of future legality.
    public GroundPreparation? PreviewGroundPreparation(Cell at,int rotation,BuildingKind kind,int rows=0)
    {
        if(rotation is <0 or >3 || rows!=0 && (kind!=BuildingKind.VegetableField || rows is <1 or >8) ||
            kind is BuildingKind.Bridge or BuildingKind.FishingDock || PlacementProblem(at,rotation,kind,rows)==null)return null;
        var footprint=Footprint(at,rotation,kind,rows).ToHashSet();var door=Door(at,rotation);
        var trees=Trees.Where(t=>footprint.Contains(t.Cell) || t.Cell==door).ToArray();
        if(trees.Length==0 || trees.Any(t=>t.Salvage) || !footprint.Append(door).All(Map.Contains) || !Map.LevelGround(footprint.Append(door)))return null;
        var ids=trees.Select(t=>t.Id).ToHashSet();
        if(kind==BuildingKind.HuntingLodge && !HuntingGrounds(at).Any(h=>Trees.Any(t=>!ids.Contains(t.Id) && h.Contains(t.Cell) && !t.Felled && !t.NeedsPlanting && t.Growth>=1)))return null;
        if(kind==BuildingKind.Quarry && !Map.StoneDeposits.Any(d=>d.Remaining>0 && (d.Cell.Point-at.Point).LengthSquared()<=16 && Accessible(d.Access)))return null;
        if(CheckPlacement(footprint,door,null,ids)!=null)return null;
        return new(trees.Select(t=>t.Cell).ToArray(),trees.Sum(t=>t.Logs));
    }
    public bool PrepareBuildingGround(Cell at,int rotation,BuildingKind kind,int rows=0)
    {
        var plan=PreviewGroundPreparation(at,rotation,kind,rows);if(plan==null)return false;
        foreach(var cell in plan.Trees)if(!SetClearing(cell,true))return false;
        return true;
    }
}

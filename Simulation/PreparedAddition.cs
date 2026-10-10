using System;
using System.Linq;
namespace Inlanders.Simulation;
public sealed record PreparedAddition(Cell Cell,int Rotation,BuildingKind Kind,int Rows,Cell[] Clearing);
public sealed partial class World
{
    public PreparedAddition? PendingAddition {get;private set;}
    public bool PlanPreparedAddition(Cell at,int rotation,BuildingKind kind,int rows=0)
    {
        if(PendingAddition!=null)return false;
        var ground=PreviewGroundPreparation(at,rotation,kind,rows);if(ground==null)return false;
        PendingAddition=new(at,rotation,kind,rows,ground.Trees);
        if(!PrepareBuildingGround(at,rotation,kind,rows)){PendingAddition=null;return false;}
        AdvancePreparedAddition();return true;
    }
    public string PreparedAdditionStatus()
    {
        if(PendingAddition is not {} p)return "No addition is waiting for ground.";
        int clearing=p.Clearing.Count(c=>Trees.Any(t=>t.Cell==c && t.ClearRequested));
        if(clearing>0)return $"Preparing ground: {clearing} trees or roots remain. Workers collect timber, then remove roots.";
        return PlacementProblem(p.Cell,p.Rotation,p.Kind,p.Rows) ?? "Ground ready; ordinary construction begins next.";
    }
    public bool CancelPreparedAddition()
    {
        if(PendingAddition is not {} p || Food.Celebrating)return false;
        foreach(var cell in p.Clearing)
        {
            var tree=Trees.FirstOrDefault(t=>t.Cell==cell);if(tree==null)continue;
            SetClearing(cell,false);if(!tree.Felled)SetTreePreserved(cell,true);
        }
        PendingAddition=null;return true;
    }
    private void AdvancePreparedAddition()
    {
        if(PendingAddition is not {} p || p.Clearing.Any(c=>Trees.Any(t=>t.Cell==c)))return;
        if(Place(p.Cell,p.Rotation,p.Kind,p.Rows)!=null)PendingAddition=null;
    }
    private void ValidatePreparedAddition()
    {
        if(PendingAddition is not {} p)return;
        if(!Enum.IsDefined(p.Kind) || p.Kind is BuildingKind.Bridge or BuildingKind.FishingDock || p.Rotation is <0 or >3 ||
            p.Rows!=0 && (p.Kind!=BuildingKind.VegetableField || p.Rows is <1 or >8) || p.Clearing==null || p.Clearing.Length==0 ||
            p.Clearing.Distinct().Count()!=p.Clearing.Length || !Footprint(p.Cell,p.Rotation,p.Kind,p.Rows).Append(Door(p.Cell,p.Rotation)).All(Map.Contains) ||
            p.Clearing.Any(c=>!Footprint(p.Cell,p.Rotation,p.Kind,p.Rows).Append(Door(p.Cell,p.Rotation)).Contains(c)))
            throw new InvalidOperationException("Invalid prepared addition");
    }
}

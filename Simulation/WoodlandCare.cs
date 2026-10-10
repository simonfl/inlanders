using System;
using System.Linq;
namespace Inlanders.Simulation;
public enum WoodlandIntent { Keep, Renew, Clear }
public sealed record WoodlandCarePlan(Cell[] Cells,int Timber,int MatureTrees,string? Problem);
public sealed partial class World
{
    public WoodlandCarePlan PreviewWoodlandCare(Cell first,Cell last,WoodlandIntent intent)
    {
        bool InsideArea(Cell c)=>c.X>=Math.Min(first.X,last.X) && c.X<=Math.Max(first.X,last.X) && c.Z>=Math.Min(first.Z,last.Z) && c.Z<=Math.Max(first.Z,last.Z);
        var trees=Trees.Where(t=>!t.Salvage && InsideArea(t.Cell)).OrderBy(t=>t.Id).ToArray();
        var cells=trees.Select(t=>t.Cell).ToArray();
        if(intent==WoodlandIntent.Renew)
        {
            if(first==last && cells.Length==0)cells=new[]{first};
            else if(first!=last)
            {
                // Leave a walking tile between new trees. Existing trees remain explicit selections.
                var fresh=Map.Land.Where(c=>InsideArea(c) && c.X%2==0 && c.Z%2==0 && !Trees.Any(t=>t.Cell==c) && ManagedWoodlandProblem(c)==null).ToArray();
                cells=cells.Concat(fresh).Distinct().ToArray();
            }
        }
        string? problem=!Enum.IsDefined(intent)?"Choose a woodland intention.":cells.Length==0?"Choose trees or stumps; click empty ground to start one new tree.":null;
        foreach(var cell in cells)
        {
            var tree=Trees.FirstOrDefault(t=>t.Cell==cell);
            // Reconsider a clearing order in this same proposal; never restore felled timber.
            var issue=tree?.ClearRequested==true && intent!=WoodlandIntent.Clear
                ? Food.Celebrating?"Wait until the village supper is over.":intent==WoodlandIntent.Keep && tree.Felled?"This tree has fallen; choose Harvest & renew to regrow it.":null
                :intent switch{WoodlandIntent.Keep=>PreserveTreeProblem(cell),WoodlandIntent.Renew=>ManagedWoodlandProblem(cell),_=>ClearingProblem(cell)};
            problem??=issue;
        }
        var newGround=cells.Where(c=>!Trees.Any(t=>t.Cell==c)).ToHashSet();
        if(intent==WoodlandIntent.Renew && newGround.Count>0)problem??=CheckPlacement(newGround,new(newGround.First().X+1,newGround.First().Z));
        if(intent==WoodlandIntent.Renew && ManagedWoodland.Union(cells).Count()>ManagedWoodlandLimit)problem=$"Choose fewer spots: a village can renew {ManagedWoodlandLimit}.";
        return new(cells,trees.Sum(t=>t.Logs),trees.Count(t=>!t.Felled && !t.NeedsPlanting && t.Growth>=1),problem);
    }
    public bool ApplyWoodlandCare(Cell first,Cell last,WoodlandIntent intent)
    {
        var plan=PreviewWoodlandCare(first,last,intent);if(plan.Problem!=null)return false;
        foreach(var cell in plan.Cells)
        {
            if(intent!=WoodlandIntent.Clear && Trees.Any(t=>t.Cell==cell && t.ClearRequested))SetClearing(cell,false);
            bool applied=intent switch{WoodlandIntent.Keep=>SetTreePreserved(cell,true),WoodlandIntent.Renew=>SetHarvestGrove(cell),_=>SetClearing(cell,true)};
            if(!applied)throw new InvalidOperationException("Validated woodland care changed during application");
        }
        return true;
    }
}

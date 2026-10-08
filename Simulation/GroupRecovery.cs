using System.Linq;
namespace Inlanders.Simulation;
public sealed record GroupPosition(int Id,Cell Cell,int Rotation,int Rows);
public sealed record GroupRecovery(Cell Anchor,int Turn,GroupPosition[] Applied,Cell[] RemovedPaths,Cell[] AddedPaths);
public sealed partial class World
{
    public GroupRecovery RememberGroup(World applied,int[] ids,int turn)=>new(Cottages.Single(c=>c.Id==ids[0]).Cell,(4-turn)%4,
        ids.Select(id=>{var c=applied.Cottages.Single(c=>c.Id==id);return new GroupPosition(id,c.Cell,c.Rotation,c.PlotRows);}).ToArray(),Paths.Except(applied.Paths).ToArray(),applied.Paths.Except(Paths).ToArray());
    public GroupPlan PreviewGroupRecovery(GroupRecovery recovery)
    {
        if(recovery.Applied.Any(old=>!Cottages.Any(c=>c.Id==old.Id && c.Cell==old.Cell && c.Rotation==old.Rotation && c.PlotRows==old.Rows)))return new(null,"A selected place has changed since the group move. Arrange it directly instead.");
        if(recovery.AddedPaths.Any(c=>!Paths.Contains(c)) || recovery.RemovedPaths.Any(Paths.Contains))return new(null,"These paths have changed since the group move. Arrange the group directly instead.");
        var plan=PreviewGroup(recovery.Applied.Select(c=>c.Id).ToArray(),recovery.Anchor,recovery.Turn);
        if(plan.Result is not {} result)return plan;
        if(recovery.RemovedPaths.Any(c=>result.PathProblem(c)!=null))return new(null,"An old approach is now blocked. Clear it or arrange the group directly.");
        result.SetPaths(recovery.AddedPaths,false);result.SetPaths(recovery.RemovedPaths,true);
        result.Validate();result.ValidateMapOccupancy();return plan;
    }
}

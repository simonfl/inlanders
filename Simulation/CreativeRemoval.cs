using System;
using System.Collections.Generic;
using System.Linq;

namespace Inlanders.Simulation;

public sealed record BuildingRemovalTarget(int Id,Cell Cell,int Rotation,BuildingKind Kind);
public sealed record CreativeRemovalSelection(IReadOnlyList<BuildingRemovalTarget> Buildings,
    IReadOnlyList<Decoration> Decorations,IReadOnlyList<Cell> Paths)
{
    public int Count=>Buildings.Count+Decorations.Count+Paths.Count;
}
public sealed record CreativeRemovalResult(World? World,string? Problem);

public sealed partial class World
{
    public CreativeRemovalSelection SelectCreativeRemoval(Cell first,Cell last)
    {
        bool Inside(Cell c)=>c.X>=Math.Min(first.X,last.X) && c.X<=Math.Max(first.X,last.X) &&
            c.Z>=Math.Min(first.Z,last.Z) && c.Z<=Math.Max(first.Z,last.Z);
        return new(Cottages.Where(c=>Footprint(c.Cell,c.Rotation,c.Kind).Any(Inside))
                .Select(c=>new BuildingRemovalTarget(c.Id,c.Cell,c.Rotation,c.Kind)).ToArray(),
            Decorations.Where(d=>Inside(d.Cell)).ToArray(),Paths.Where(Inside).ToArray());
    }

    // This transaction never edits the source. Call again on confirmation, then publish
    // the returned world once. Never commit a preview copy after live simulation advances.
    public CreativeRemovalResult PrepareCreativeRemoval(CreativeRemovalSelection selection)
    {
        CreativeRemovalResult Reject(string reason)=>new(null,reason);
        if(!Creative)return Reject("Multiple removal is available in Creative only.");
        if(selection.Count==0)return Reject("Select buildings, decorations or paths to remove.");
        if(Food.Celebrating)return Reject("Wait until supper finishes.");
        var buildings=selection.Buildings.Distinct().OrderBy(b=>b.Id).ToArray();
        var decorations=selection.Decorations.Distinct().ToArray();var paths=selection.Paths.Distinct().ToArray();
        foreach(var target in buildings)
        {
            var site=Cottages.FirstOrDefault(c=>c.Id==target.Id);
            if(site==null || site.Cell!=target.Cell || site.Rotation!=target.Rotation || site.Kind!=target.Kind)
                return Reject("A selected building changed. Select the area again.");
            if(!site.Complete)return Reject("Finish or cancel selected construction plans individually.");
        }
        if(decorations.Any(d=>!Decorations.Contains(d)) || paths.Any(c=>!Paths.Contains(c)))
            return Reject("A selected decoration or path changed. Select the area again.");
        var copy=LoadJson(SaveJson());
        // Removing ornaments first can open an alternative route for a selected bridge.
        foreach(var decoration in decorations)
            if(!copy.RemoveDecoration(decoration.Cell))return Reject("A selected decoration cannot be removed.");
        var pending=buildings.ToList();
        while(pending.Count>0)
        {
            var target=pending.FirstOrDefault(b=>copy.RemovalProblem(b.Id)==null);
            if(target==null)return Reject(copy.RemovalProblem(pending[0].Id)??"The selected buildings cannot be removed together.");
            if(!copy.RemoveBuilding(target.Id))return Reject("A selected building cannot be removed.");
            pending.Remove(target);
        }
        foreach(var cell in paths)if(!copy.SetPath(cell,false))return Reject("A selected path cannot be removed.");
        copy.Validate();return new(copy,null);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace Inlanders.Simulation;

public sealed partial class World
{
    public const int ManagedWoodlandLimit=32;
    public HashSet<Cell> ManagedWoodland { get; private set; } = new();
    public string? PreserveTreeProblem(Cell cell)
    {
        if(Food.Celebrating) return "Wait until supper finishes.";
        var tree=Trees.FirstOrDefault(t=>t.Cell==cell);
        return tree==null || tree.Salvage || tree.Felled ? "Choose a living tree or sapling to preserve." :
            tree.ClearRequested ? "Cancel the clearing order before preserving this tree." : null;
    }
    public bool SetTreePreserved(Cell cell,bool preserved)
    {
        if(PreserveTreeProblem(cell)!=null) return false;
        var tree=Trees.Single(t=>t.Cell==cell);
        if(tree.Preserved==preserved) return true;
        if(preserved && tree.Owner is int owner && People[owner].Task is Work.ToTree or Work.Chopping) Interrupt(People[owner]);
        tree.Preserved=preserved; _retry=0; return true;
    }
    public string? ManagedWoodlandProblem(Cell cell,bool remove=false)
    {
        if(Food.Celebrating) return "Wait until supper finishes.";
        if(remove) return ManagedWoodland.Contains(cell) ? null : "This tile is not in a managed grove.";
        if(ManagedWoodland.Contains(cell)) return null;
        if(ManagedWoodland.Count>=ManagedWoodlandLimit) return $"A village can manage {ManagedWoodlandLimit} tree spots. Remove a spot before adding another.";
        var tree=Trees.FirstOrDefault(t=>t.Cell==cell);
        if(tree!=null) return tree.Salvage ? "Collect salvage before marking a grove." : tree.ClearRequested ? "Cancel the clearing order before marking a grove." : null;
        if(Paths.Contains(cell)) return "Remove the path before marking this tree spot.";
        return PlantingProblem(cell);
    }
    public bool SetManagedWoodland(Cell cell,bool managed)
    {
        if(ManagedWoodlandProblem(cell,!managed)!=null) return false;
        if(managed) ManagedWoodland.Add(cell); else ManagedWoodland.Remove(cell);
        _retry=0; return true;
    }
    private bool ClaimGrovePlanting(Villager worker)
    {
        foreach(var cell in ManagedWoodland.OrderBy(c=>(c.Point-worker.Position).LengthSquared()).ThenBy(c=>c.X).ThenBy(c=>c.Z))
        {
            var existing=Trees.FirstOrDefault(t=>t.Cell==cell);
            if(existing!=null && (!existing.Felled || existing.Logs>0 || existing.Owner!=null || existing.ClearRequested)) continue;
            if(PlantingProblem(cell)!=null) continue;
            var tree=PlantTree(cell)!;
            tree.Owner=worker.Id; worker.TreeId=tree.Id;
            Go(worker,tree.Access,Work.ToSapling,"Replanting a managed grove"); return true;
        }
        return false;
    }
    private void ValidateManagedWoodland()
    {
        if(ManagedWoodland==null || ManagedWoodland.Count>ManagedWoodlandLimit || ManagedWoodland.Any(c=>!Map.Contains(c) || Map.Water.Contains(c) || Paths.Contains(c) ||
            Decorations.Any(d=>d.Cell==c) || Cottages.Any(b=>Footprint(b.Cell,b.Rotation,b.Kind).Contains(c)) || Trees.Any(t=>t.Cell==c && t.ClearRequested)))
            throw new InvalidOperationException("Invalid managed woodland");
        if(Trees.Any(t=>t.Preserved && (t.Felled || t.Salvage || t.ClearRequested || t.Owner is int owner && People[owner].Task is Work.ToTree or Work.Chopping)))
            throw new InvalidOperationException("Preserved tree has conflicting work");
    }
}

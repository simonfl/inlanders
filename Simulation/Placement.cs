using System.Collections.Generic;
using System.Linq;

namespace Inlanders.Simulation;

public sealed partial class World
{
    // A null problem is the authoritative permission to place; UI and commands use the same checks.
    public string? PlacementProblem(Cell cell, bool rotated, BuildingKind kind = BuildingKind.Cottage) => kind == BuildingKind.FishingDock ? DockProblem(cell, rotated) : kind == BuildingKind.Bridge ? BridgeProblem(cell, rotated) :
        Footprint(cell, rotated).Append(Door(cell, rotated)).All(Map.Contains) && !Map.LevelGround(Footprint(cell, rotated).Append(Door(cell, rotated))) ? "Choose level ground for the footprint and entrance." :
        CheckPlacement(Footprint(cell, rotated).ToHashSet(), Door(cell, rotated));

    public string? PlantingProblem(Cell cell)
    {
        var tree = Trees.FirstOrDefault(t => t.Cell == cell);
        if (tree?.ClearRequested == true) return "This spot is marked for clearing. Cancel its clearing order before replanting.";
        if (Food.Celebrating) return "Wait until the village supper is over.";
        if (tree != null && (tree.Salvage || !tree.Felled || tree.Logs > 0 || tree.Owner != null))
            return tree.NeedsPlanting ? "This spot is already marked for planting." : tree.Growth < 1 ? "A sapling is already growing here." :
                tree.Felled ? "Collect the remaining timber or salvage before replanting." : "A living tree already occupies this spot.";
        return CheckPlacement(new HashSet<Cell> { cell }, new(cell.X + 1, cell.Z), tree);
    }

    private string? CheckPlacement(HashSet<Cell> footprint, Cell entrance, TimberTree? reusableStump = null)
    {
        if (Food.Celebrating) return "Wait until the village supper is over.";
        if (footprint.Any(c => !Inside(c)) || !Inside(entrance)) return "Keep the footprint and its entrance inside the buildable map.";
        if (footprint.Any(Map.Water.Contains)) return "Water needs a bridge; buildings and planting require dry land.";
        if (Decorations.Any(d => footprint.Contains(d.Cell))) return "Remove decorations from this footprint first.";
        if (footprint.Contains(Stockpile)) return "The timber yard occupies this spot.";
        var tree = Trees.FirstOrDefault(t => t != reusableStump && footprint.Contains(t.Cell));
        if (tree != null) return tree.Salvage ? "A salvage pile occupies this spot; let loggers collect it." : tree.ClearRequested ? "Loggers must finish clearing this spot before you can build." : tree.Felled ? "A stump occupies this spot. Use Clear trees & stumps [C] to make it buildable, or replant it." : "A tree or planting spot occupies this footprint.";
        if (Bushes.Any(b => footprint.Contains(b.Cell))) return "Berry bushes occupy this footprint.";
        var site = Cottages.FirstOrDefault(c => Footprint(c.Cell, c.Rotated, c.Kind).Any(footprint.Contains));
        if (site != null) return $"This overlaps {site.Kind} {site.Id}{(site.Complete ? "" : " (under construction)")}.";
        if (Blocked(entrance)) return "The marked entrance is blocked. Move or rotate the plan.";
        if (footprint.Contains(YardAccess)) return "Keep the timber yard's collection point clear.";
        if (Cottages.Any(c => c.Kind == BuildingKind.Bridge && (footprint.Contains(FarBank(c.Cell, c.Rotated)) || footprint.Contains(Door(c.Cell, c.Rotated))))) return "Keep the far bank of the bridge clear.";
        if (Cottages.Any(c => footprint.Contains(c.Entrance))) return "This would cover another building's entrance.";
        if (Trees.Any(t => footprint.Contains(t.Access)) || Bushes.Any(b => footprint.Contains(b.Access))) return "Workers need this spot to reach trees or berry bushes.";
        var worker = People.FirstOrDefault(v => footprint.Contains(At(v)) || (v.Route.TryPeek(out var next) && footprint.Contains(next)));
        if (worker != null) return $"{worker.Name} is standing here or stepping into this footprint. Wait or choose another spot.";
        bool Obstacle(Cell c) => Blocked(c) || footprint.Contains(c);
        var access = Trees.Select(t => t.Access).Concat(Bushes.Select(b => b.Access)).Concat(Cottages.Select(c => c.Entrance)).Concat(People.Where(v => v.LeisureSiteId != null || v.Task is Work.ToRest or Work.Resting).Select(v => v.Destination)).Append(YardAccess).Append(entrance);
        var reached = Reachable(YardAccess, Obstacle);
        var before = Reachable(YardAccess, Blocked);
        if (!reached.Contains(entrance) || access.Where(before.Contains).Concat(People.Select(At)).Any(c => !reached.Contains(c)))
            return "This would cut off a route between villagers, resources, or buildings and the timber yard.";
        return null;
    }
}

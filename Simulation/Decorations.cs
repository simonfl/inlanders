using System;
using System.Collections.Generic;
using System.Linq;

namespace Inlanders.Simulation;

public enum DecorationKind { Flowers, Shrub, Fence, OrnamentalTree, Pebbles, Sunflowers }
public sealed record Decoration(Cell Cell, DecorationKind Kind, bool Rotated = false)
{
    public bool Solid => Kind != DecorationKind.Pebbles;
}
public sealed partial class World
{
    public List<Decoration> Decorations { get; private set; } = new();
    public int DecorationRevision { get; private set; }

    public string? DecorationProblem(Cell cell, DecorationKind kind, bool remove = false)
    {
        if (Food.Celebrating) return "Wait until supper is over.";
        if (remove) return Decorations.Any(d => d.Cell == cell) ? null : "There is no decoration here to remove.";
        if (!Enum.IsDefined(kind)) return "Choose a decoration.";
        if (kind == DecorationKind.Sunflowers && !SunflowersUnlocked) return SunflowerLockReason;
        if (Decorations.Any(d => d.Cell == cell)) return "Remove the existing decoration first.";
        if (kind == DecorationKind.Pebbles)
            return Blocked(cell) || Map.Water.Contains(cell) ? "Place ground cover on clear dry land." : null;
        // Decoration needs no service entrance, but must preserve all existing access.
        return CheckPlacement(new HashSet<Cell> { cell }, YardAccess);
    }

    public bool PlaceDecoration(Cell cell, DecorationKind kind, bool rotated = false)
    {
        if (DecorationProblem(cell, kind) != null) return false;
        var item = new Decoration(cell, kind, rotated); Decorations.Add(item);
        if (item.Solid) RemovePaths(new[] { cell });
        DecorationRevision++;
        foreach (var v in People.Where(v => v.Route.Count > 0)) SetRoute(v, v.Destination);
        return true;
    }

    public bool RemoveDecoration(Cell cell)
    {
        if (DecorationProblem(cell, default, true) != null) return false;
        Decorations.RemoveAll(d => d.Cell == cell); DecorationRevision++;
        foreach (var v in People.Where(v => v.Route.Count > 0)) SetRoute(v, v.Destination);
        return true;
    }

    private void ValidateDecorations()
    {
        if (Decorations.Select(d => d.Cell).Distinct().Count() != Decorations.Count ||
            Decorations.Any(d => !Enum.IsDefined(d.Kind) || (d.Kind == DecorationKind.Sunflowers && !SunflowersUnlocked) || !Map.Contains(d.Cell) || Map.Water.Contains(d.Cell) ||
                d.Cell == Stockpile || Trees.Any(t => t.Cell == d.Cell) || Bushes.Any(b => b.Cell == d.Cell) ||
                Cottages.Any(c => Footprint(c.Cell,c.Rotated,c.Kind).Contains(d.Cell)) || (d.Solid && Paths.Contains(d.Cell))))
            throw new InvalidOperationException("Invalid decorations");
    }
}

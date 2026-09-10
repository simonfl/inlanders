using System.Linq;

namespace Inlanders.Simulation;

public sealed partial class World
{
    private bool Accessible(Cell cell) => Map.Water.Count == 0 || FindPath(YardAccess, cell, Blocked) != null;
    public Cell BridgeEntrance(Cell cell, bool rotated) => Accessible(Door(cell, rotated)) ? Door(cell, rotated) : FarBank(cell, rotated);
    public static Cell FarBank(Cell cell, bool rotated) => rotated ? new(cell.X - 1, cell.Z) : new(cell.X, cell.Z - 1);
    public string? BridgeProblem(Cell cell, bool rotated)
    {
        if (Food.Celebrating) return "Wait until supper is over.";
        if (!Map.Contains(cell) || !Map.Water.Contains(cell)) return "Place the bridge on a one-tile-wide stretch of water.";
        if (Cottages.Any(c => c.Kind == BuildingKind.FishingDock && c.Launch == cell)) return "Keep the fishing dock's launch clear.";
        foreach(var dock in Cottages.Where(c=>c.Boat?.FisherId!=null))
        {
            var boat=dock.Boat!;
            var position=new Cell((int)System.MathF.Round(boat.Position.X),(int)System.MathF.Round(boat.Position.Y));
            var reachable=Reachable(dock.Launch,c=>c==cell || BoatBlocked(c));
            if(!reachable.Contains(position) || boat.Route.Contains(cell) || boat.GroundId is int ground && !reachable.Contains(Map.FishingGrounds.Single(g=>g.Id==ground).Cell))
                return "This crossing would block an active fishing trip. Pause the dock and let its boat return first.";
        }
        if (Cottages.Any(c => Footprint(c.Cell, c.Rotated, c.Kind).Contains(cell))) return "A bridge already occupies this crossing.";
        var near = Door(cell, rotated); var far = FarBank(cell, rotated);
        if (!Map.Contains(near) || !Map.Contains(far) || Map.Water.Contains(near) || Map.Water.Contains(far))
            return "Both ends need dry banks. Press R to turn the crossing.";
        if (!Map.LevelGround(new[] { near, cell, far })) return "Bridges need level riverbanks.";
        if (Blocked(near) || Blocked(far)) return "Clear both banks before planning a bridge.";
        if (!Accessible(near) && !Accessible(far)) return "Builders need a route from the yard to the marked entrance bank.";
        return null;
    }
}

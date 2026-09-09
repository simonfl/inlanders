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
        if (Cottages.Any(c => Footprint(c.Cell, c.Rotated, c.Kind).Contains(cell))) return "A bridge already occupies this crossing.";
        var near = Door(cell, rotated); var far = FarBank(cell, rotated);
        if (!Map.Contains(near) || !Map.Contains(far) || Map.Water.Contains(near) || Map.Water.Contains(far))
            return "Both ends need dry banks. Press R to turn the crossing.";
        if (Blocked(near) || Blocked(far)) return "Clear both banks before planning a bridge.";
        if (!Accessible(near) && !Accessible(far)) return "Builders need a route from the yard to the marked entrance bank.";
        return null;
    }
}

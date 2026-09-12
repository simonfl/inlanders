using System.Linq;

namespace Inlanders.Simulation;

public sealed partial class World
{
    private string? GatewayProblem(Cell cell)
    {
        if(Blocked(cell) || Map.Water.Contains(cell))return "Place a gateway on clear dry land; paths may pass through.";
        if(cell==YardAccess || MeetingSpots.Contains(cell) || Map.Wildlife.Any(h=>h.Cell==cell) ||
            Trees.Any(t=>t.Access==cell) || Bushes.Any(b=>b.Access==cell) || Map.StoneDeposits.Any(d=>d.Access==cell) ||
            Cottages.Any(c=>c.Entrance==cell || c.Kind==BuildingKind.Bridge && FarBank(c.Cell,c.Rotation)==cell) ||
            MealSpotReserved(cell) || ComfortSpotReserved(cell))return "Keep entrances and working or seating spots clear; choose an ordinary path tile.";
        return null;
    }
    // Side edges only. Rotation changes the gateway's facing, never its walkability.
    public static bool FenceOffersConnection(Decoration decoration,Cell direction)=>decoration.Kind==DecorationKind.Fence ||
        decoration.Kind==DecorationKind.Gateway && (decoration.Facing%2==0?direction.Z==0:direction.X==0);
}

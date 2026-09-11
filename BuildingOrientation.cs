using Godot;
using Inlanders.Simulation;

public partial class Game
{
    private Vector3 BuildingPosition(Cell cell,int rotation,BuildingKind kind,float lift=0)
    {
        bool compact=kind is BuildingKind.Bridge or BuildingKind.FishingDock or BuildingKind.SeatingGarden;
        var offset=compact?Vector3.Zero:new Vector3(0,0,-.5f).Rotated(Vector3.Up,rotation*Mathf.Pi/2);
        return OnGround(cell.X+offset.X,cell.Z+offset.Z,lift);
    }
}

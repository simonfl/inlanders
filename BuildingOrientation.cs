using Godot;
using Inlanders.Simulation;

public partial class Game
{
    private Vector3 BuildingPosition(Cell cell,int rotation,BuildingKind kind,float lift=0)
    {
        var offset=new Vector3(0,0,(1-Buildings.Get(kind).Depth)/2f).Rotated(Vector3.Up,rotation*Mathf.Pi/2);
        return OnGround(cell.X+offset.X,cell.Z+offset.Z,lift);
    }
}

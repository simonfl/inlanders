using Godot;
using Inlanders.Simulation;

public partial class Game
{
    private Vector3 BuildingPosition(Cottage site)=>BuildingPosition(site.Cell,site.Rotation,site.Kind,0,site.PlotRows);
    private Vector3 BuildingPosition(Cell cell,int rotation,BuildingKind kind,float lift=0,int rows=0)
    {
        var offset=new Vector3(0,0,(1-(rows>0?rows:Buildings.Get(kind).Depth))/2f).Rotated(Vector3.Up,rotation*Mathf.Pi/2);
        return OnGround(cell.X+offset.X,cell.Z+offset.Z,lift);
    }
}

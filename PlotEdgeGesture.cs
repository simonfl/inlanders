using Godot;
using Inlanders.Simulation;
using System.Linq;
public partial class Game
{
    private bool _dragPlotEdge;
    private Vector3 RevisionEdge(Cottage site,int rows)
    {
        var offset=new Vector3(0,0,1-rows).Rotated(Vector3.Up,site.Rotation*Mathf.Pi/2);
        return OnGround(site.Cell.X+offset.X,site.Cell.Z+offset.Z,.13f);
    }
    private bool HandlePlotEdge(InputEvent input)
    {
        var site=_world.Cottages.FirstOrDefault(c=>c.Id==_reshapingPlot);
        if(site==null || !_revisionPanel.IsVisibleInTree()){_dragPlotEdge=false;return false;}
        if(input is InputEventMouseButton{ButtonIndex:MouseButton.Left} click)
        {
            if(!click.Pressed && _dragPlotEdge){_dragPlotEdge=false;return true;}
            if(click.Pressed && !PointerOverHud(click.Position) && click.Position.DistanceTo(_camera.UnprojectPosition(RevisionEdge(site,_revisionRows)))<34){_dragPlotEdge=true;return true;}
        }
        if(_dragPlotEdge && input is InputEventMouseMotion motion)
        {
            _pointerPosition=motion.Position;
            if(!PointerOverHud(motion.Position) && Ground(motion.Position) is Vector3 at)
            {
                float dx=at.X-site.Cell.X,dz=at.Z-site.Cell.Z;
                float localZ=site.Rotation switch{0=>dz,1=>dx,2=>-dz,_=>-dx};
                _revisionRows=System.Math.Clamp(1-Mathf.RoundToInt(localZ),1,8);_nextWorkCard=0;
            }
            return true;
        }
        return false;
    }
}

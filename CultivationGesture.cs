using Godot;
using Inlanders.Simulation;
public partial class Game
{
    private Cell? _plotAnchor;
    private bool HandleCultivationGesture(InputEvent input)
    {
        if(!PlotActive){_plotAnchor=null;return false;}
        if(_plotAnchor!=null && (input is InputEventKey{Pressed:true,Keycode:Key.Escape} || input is InputEventMouseButton{Pressed:true,ButtonIndex:MouseButton.Right}))
        {_plotAnchor=null;RefreshGhost();return true;}
        if(input is InputEventMouseButton{ButtonIndex:MouseButton.Left} click)
        {
            _pointerPosition=click.Position;
            if(click.Pressed)
            {
                if(PointerOverHud(click.Position) || Ground(click.Position) is not Vector3 point)return false;
                _plotAnchor=new Cell(Mathf.RoundToInt(point.X),Mathf.RoundToInt(point.Z));_hover=_plotAnchor.Value;RefreshGhost();return true;
            }
            if(_plotAnchor is Cell anchor)
            {
                _plotAnchor=null;
                if(!PointerOverHud(click.Position)){_hover=anchor;PlaceCottage(anchor);}else RefreshGhost();
                return true;
            }
        }
        if(_plotAnchor is Cell start && input is InputEventMouseMotion motion)
        {
            _pointerPosition=motion.Position;
            if(!PointerOverHud(motion.Position) && Ground(motion.Position) is Vector3 point)
            {
                int dx=Mathf.RoundToInt(point.X)-start.X,dz=Mathf.RoundToInt(point.Z)-start.Z;
                if(dx!=0 || dz!=0){bool alongX=System.Math.Abs(dx)>System.Math.Abs(dz);_rotation=alongX?(dx<0?1:3):(dz<0?0:2);_plotRows=System.Math.Clamp(System.Math.Max(System.Math.Abs(dx),System.Math.Abs(dz))+1,1,8);}
                _hover=start;_livelihoodSite=null;RefreshGhost();
            }
            return true;
        }
        return false;
    }
}

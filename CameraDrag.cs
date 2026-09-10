using Godot;

public partial class Game
{
    private MouseButton _cameraDragButton;
    private Vector2 _cameraDragStart, _cameraDragLast;
    private bool _cameraDragging;

    private void CancelCameraDrag()
    {
        _cameraDragButton=MouseButton.None;
        _cameraDragging=false;
    }

    private bool BeginCameraDrag(InputEventMouseButton mouse)
    {
        if (PointerOverHud(mouse.Position) || _pathStroke || _woodlandStroke) return false;
        if (mouse.ButtonIndex is not (MouseButton.Right or MouseButton.Middle) &&
            !(mouse.ButtonIndex==MouseButton.Left && !_placing)) return false;
        _cameraDragButton=mouse.ButtonIndex;
        _cameraDragStart=_cameraDragLast=mouse.Position;
        _cameraDragging=false;
        return true;
    }

    // A horizontal projection plane keeps dragging smooth across terrain and beyond map edges.
    private Vector3 CameraDragPoint(Vector2 screen)
    {
        var origin=_camera.ProjectRayOrigin(screen);
        var direction=_camera.ProjectRayNormal(screen);
        return origin+direction*((_focus.Y-origin.Y)/direction.Y);
    }

    private bool HandleCameraDrag(InputEvent input)
    {
        if (_cameraDragButton==MouseButton.None) return false;
        if (input is InputEventKey { Pressed:true }) { CancelCameraDrag(); return false; }
        if (input is InputEventMouseButton button)
        {
            if (button.ButtonIndex!=_cameraDragButton) { CancelCameraDrag(); return false; }
            if (!button.Pressed)
            {
                bool select=_cameraDragButton==MouseButton.Left && !_cameraDragging && !PointerOverHud(button.Position);
                CancelCameraDrag();
                if(select && !_watching && !_placing) SelectAtPointer(button.Position);
            }
            return true;
        }
        if (input is not InputEventMouseMotion motion) return false;
        var mask=_cameraDragButton==MouseButton.Left?MouseButtonMask.Left:
            _cameraDragButton==MouseButton.Right?MouseButtonMask.Right:MouseButtonMask.Middle;
        if ((motion.ButtonMask & mask)==0) { CancelCameraDrag(); return false; }
        if (!_cameraDragging && motion.Position.DistanceTo(_cameraDragStart)<6) return true;
        _cameraDragging=true; _followPerson=false; _watchOrbit=false;
        _focus+=CameraDragPoint(_cameraDragLast)-CameraDragPoint(motion.Position);
        _cameraDragLast=motion.Position;
        UpdateCamera();
        return true;
    }
}

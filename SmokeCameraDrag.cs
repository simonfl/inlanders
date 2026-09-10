using Godot;
using System;
using System.Threading.Tasks;
using Inlanders.Simulation;

public partial class Game
{
    private async Task CheckCameraDrag()
    {
        void Check(bool ok,string why) { if(!ok) throw new Exception(why); }
        async Task Frames() { for(int i=0;i<4;i++) await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame); }
        void ButtonAt(Vector2 at,MouseButton button,bool pressed) => Input.ParseInputEvent(new InputEventMouseButton {Position=at,GlobalPosition=at,ButtonIndex=button,Pressed=pressed});
        void Move(Vector2 at,MouseButton button) => Input.ParseInputEvent(new InputEventMouseMotion {Position=at,GlobalPosition=at,ButtonMask=button==MouseButton.Left?MouseButtonMask.Left:button==MouseButton.Right?MouseButtonMask.Right:MouseButtonMask.Middle});
        _paused=true;CloseManagementUi();
        foreach(var size in new[]{new Vector2I(1440,900),new(960,640)})
        foreach(var angle in new[]{0f,Mathf.Pi/2})
        foreach(var button in new[]{MouseButton.Left,MouseButton.Right,MouseButton.Middle})
        {
            GetWindow().Size=size;_focus=Vector3.Zero;_angle=angle;UpdateCamera();await Frames();
            Vector2 start=new(size.X*.4f,size.Y*.45f),end=start+new Vector2(65,30);
            string state=_world.SaveJson();var old=_focus;int selected=_selectedPerson;
            var anchor=CameraDragPoint(start);ButtonAt(start,button,true);Move(end,button);await Frames();
            Check(_focus!=old,"Drag did not pan");
            Check(_camera.UnprojectPosition(anchor).DistanceTo(end)<2,"Drag did not follow pointer at rotated camera");
            ButtonAt(end,button,false);await Frames();
            Check(_world.SaveJson()==state && _selectedPerson==selected,"Drag changed village/selection");
            Check(_cameraDragButton==MouseButton.None,"Drag release stuck");
        }
        _focus=Vector3.Zero;UpdateCamera();CloseManagementUi();await Frames();
        var personPoint=_camera.UnprojectPosition(_people[0].Body.Position+Vector3.Up*.6f);
        var clickFocus=_focus;
        ButtonAt(personPoint,MouseButton.Left,true);Move(personPoint+new Vector2(2,1),MouseButton.Left);ButtonAt(personPoint,MouseButton.Left,false);
        await Frames();Check(_selectedPerson==0 && _focus==clickFocus,"Small pointer movement stopped click selection");CloseManagementUi();
        BeginPlacement(BuildingKind.Cottage);await Frames();
        Vector2 point=new(380,280);string before=_world.SaveJson();
        ButtonAt(point,MouseButton.Right,true);Move(point+new Vector2(40,20),MouseButton.Right);ButtonAt(point,MouseButton.Right,false);await Frames();
        Check(_placing && _world.SaveJson()==before && !_pathStroke && !_woodlandStroke,"Build-mode panning placed/painted or cancelled tool");
        await Press(Key.Escape);await Frames();
        var focus=_focus;var hudPoint=_topBar.GetGlobalRect().GetCenter();
        ButtonAt(hudPoint,MouseButton.Right,true);Move(hudPoint+new Vector2(30,0),MouseButton.Right);ButtonAt(hudPoint,MouseButton.Right,false);await Frames();
        Check(_focus==focus,"HUD-origin drag moved camera");
        ToggleWatch();ToggleWatchOrbit();await Frames();
        _followPerson=true;
        ButtonAt(point,MouseButton.Left,true);Move(point+new Vector2(40,0),MouseButton.Left);await Frames();
        Check(!_watchOrbit && !_followPerson,"Manual drag did not stop orbit/following");
        _Notification((int)NotificationWMWindowFocusOut);Check(_cameraDragButton==MouseButton.None,"Focus loss kept drag active");
        ToggleCleanWatch();await Frames();
        var hiddenControls=_watchBar.GetGlobalRect().GetCenter();focus=_focus;
        ButtonAt(hiddenControls,MouseButton.Right,true);Move(hiddenControls+new Vector2(25,0),MouseButton.Right);ButtonAt(hiddenControls,MouseButton.Right,false);await Frames();
        Check(_focus!=focus,"Hidden Watch controls blocked drag");
        ExitWatch();await Frames();
    }
}

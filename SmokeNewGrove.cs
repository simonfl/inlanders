using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
public partial class Game
{
    private async void RunNewGroveSmoke()
    {
        try
        {
            async System.Threading.Tasks.Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
            AdoptWorld(World.NewRiverFrontage(false,true,true));_paused=true;CloseManagementUi();GetWindow().Size=new(960,640);
            _focus=OnGround(-6,2);_camera.Size=20;UpdateCamera();BeginWoodlandCare();await Frames();
            var a=_camera.UnprojectPosition(OnGround(-8,0));var b=_camera.UnprojectPosition(OnGround(-4,4));string before=_world.SaveJson();
            Input.ParseInputEvent(new InputEventMouseButton{Position=a,GlobalPosition=a,ButtonIndex=MouseButton.Left,Pressed=true});await Frames();
            Input.ParseInputEvent(new InputEventMouseMotion{Position=b,GlobalPosition=b,ButtonMask=MouseButtonMask.Left});await Frames();
            Input.ParseInputEvent(new InputEventMouseButton{Position=b,GlobalPosition=b,ButtonIndex=MouseButton.Left,Pressed=false});await Frames();
            var plan=_world.PreviewWoodlandCare(_careFirst,_careLast,WoodlandIntent.Renew);var fresh=plan.Cells.Where(c=>!_world.Trees.Any(t=>t.Cell==c)).ToArray();
            if(fresh.Length<3 || _careApply.Disabled || before!=_world.SaveJson() || _carePanel.GetGlobalRect().End.Y>_hud.Size.Y-75)throw new Exception("Native grove proposal failed/overflowed");
            await Capture("artifacts/190-grove-proposal-960.png");await UiClick(_careApply);_speed=6;double start=_uiTime;
            while(_uiTime-start<85 && fresh.Any(c=>!_world.Trees.Any(t=>t.Cell==c && !t.NeedsPlanting && t.Growth>=1)))await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
            _paused=true;if(fresh.Any(c=>!_world.Trees.Any(t=>t.Cell==c && !t.NeedsPlanting && t.Growth>=1)))throw new Exception("Native grove not mature");
            await Capture("artifacts/190-grove-grown-960.png");_world.Validate();GD.Print("PASS native grove drag, pure spaced preview, fit, apply/watch, actual planting and maturity.");GetTree().Quit();
        }
        catch(Exception e){GD.PrintErr(e);GetTree().Quit(1);}
    }
}

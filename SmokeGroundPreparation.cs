using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
public partial class Game
{
    private async void RunGroundPreparationSmoke()
    {
        try
        {
            async System.Threading.Tasks.Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
            AdoptWorld(World.NewRiverFrontage(false,true,true));_paused=true;CloseManagementUi();GetWindow().Size=new(960,640);
            var target=(from c in _world.Map.Land from r in Enumerable.Range(0,4) let p=_world.PreviewGroundPreparation(c,r,BuildingKind.SeatingGarden) where p!=null select(c,r,p)).First();
            _focus=OnGround(target.c.X,target.c.Z);_camera.Size=18;UpdateCamera();BeginPlacement(BuildingKind.SeatingGarden);_rotation=target.r;
            var point=_camera.UnprojectPosition(OnGround(target.c.X,target.c.Z));Input.ParseInputEvent(new InputEventMouseMotion{Position=point,GlobalPosition=point});await Frames();
            string untouched=_world.SaveJson();await Click(point);await Frames();
            if(_groundPreparation==null || _ghostValid || _world.SaveJson()!=untouched)throw new Exception("Ordinary click cleared or invalid preparation preview");
            await Capture("artifacts/187-prepare-footprint-960.png");
            foreach(bool pressed in new[]{true,false}){Input.ParseInputEvent(new InputEventMouseButton{Position=point,GlobalPosition=point,ButtonIndex=MouseButton.Left,Pressed=pressed,ShiftPressed=true});await Frames();}
            if(!target.p!.Trees.All(c=>_world.Trees.Single(t=>t.Cell==c).ClearRequested) || !_placing)throw new Exception("Explicit footprint clearing failed");
            _paused=false;_speed=6;double start=_uiTime;
            while(_uiTime-start<80 && _world.PlacementProblem(target.c,target.r,BuildingKind.SeatingGarden)!=null)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
            _paused=true;await Frames();if(!_ghostValid)throw new Exception("Prepared footprint never legal");
            await Click(point);await Frames();var site=_world.Cottages.Last();if(site.Kind!=BuildingKind.SeatingGarden || site.Cell!=target.c)throw new Exception("Ordinary placement after clearance failed");
            _paused=false;start=_uiTime;while(_uiTime-start<50 && !site.Complete)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);_paused=true;
            if(!site.Complete)throw new Exception("Cleared-ground addition never completed");
            await Capture("artifacts/187-built-on-prepared-ground-960.png");_world.Validate();
            GD.Print("PASS native footprint preview, ordinary-click purity, Shift clearing, real roots removal, placement and construction.");GetTree().Quit();
        }
        catch(Exception e){GD.PrintErr(e);GetTree().Quit(1);}
    }
}

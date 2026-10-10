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
            await Capture("artifacts/191-prepare-footprint-960.png");
            foreach(bool pressed in new[]{true,false}){Input.ParseInputEvent(new InputEventMouseButton{Position=point,GlobalPosition=point,ButtonIndex=MouseButton.Left,Pressed=pressed,ShiftPressed=true});await Frames();}
            if(!target.p!.Trees.All(c=>_world.Trees.Single(t=>t.Cell==c).ClearRequested) || _placing || _world.PendingAddition==null)throw new Exception("Explicit footprint clearing failed");
            await Frames();if(!_additionCard.Visible || _additionCard.GetGlobalRect().End.Y>_hud.Size.Y-75 || _additionCard.GetGlobalRect().End.X>_hud.Size.X)throw new Exception($"Prepared addition card visible={_additionCard.Visible}, rect={_additionCard.GetGlobalRect()}");
            await Capture("artifacts/191-prepared-card-960.png");
            await Press(Key.F5);string pending=_world.SaveJson();await Press(Key.F9);await Frames();
            if(_world.SaveJson()!=pending || _world.PendingAddition==null)throw new Exception("Saved preparation intent lost");
            ShowPreparedAddition();await Frames();await UiClick(_additionCancel);await Frames();
            if(_world.PendingAddition!=null || _world.Trees.Any(t=>target.p.Trees.Contains(t.Cell) && (t.ClearRequested || !t.Preserved)))throw new Exception("Native plan cancellation failed");
            BeginPlacement(BuildingKind.SeatingGarden);_rotation=target.r;await Frames();
            foreach(bool pressed in new[]{true,false}){Input.ParseInputEvent(new InputEventMouseButton{Position=point,GlobalPosition=point,ButtonIndex=MouseButton.Left,Pressed=pressed,ShiftPressed=true});await Frames();}
            if(_world.PendingAddition==null)throw new Exception("Replanned intention missing");
            var retained=target.p.Trees.First();if(!_world.ApplyWoodlandCare(retained,retained,WoodlandIntent.Keep))throw new Exception("Reconsidered tree failed");
            ShowPreparedAddition();await Frames();if(!_additionResume.Visible || !_additionText.Text.Contains("Woodland care changed"))throw new Exception("Conflict not actionable");
            if(_additionCard.GetGlobalRect().End.Y>_hud.Size.Y-75)throw new Exception("Conflict card does not fit compact view");
            await Capture("artifacts/197-reconsidered-addition-960.png");await UiClick(_additionResume);await Frames();
            if(_world.PreparedAdditionConflict())throw new Exception("Explicit resume did not reconcile woodland");
            _paused=false;_speed=6;double start=_uiTime;
            while(_uiTime-start<80 && !_world.Cottages.Any(c=>c.Cell==target.c && c.Kind==BuildingKind.SeatingGarden))await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
            _paused=true;await Frames();var site=_world.Cottages.Single(c=>c.Cell==target.c && c.Kind==BuildingKind.SeatingGarden);
            _paused=false;start=_uiTime;while(_uiTime-start<70 && !_world.People.Any(p=>p.LastLeisureSiteId==site.Id))await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);_paused=true;
            if(!site.Complete || !_world.People.Any(p=>p.LastLeisureSiteId==site.Id))throw new Exception("Prepared addition never completed a real visit");
            await Capture("artifacts/197-reconsidered-used-960.png");_world.Validate();
            GD.Print("PASS native footprint preview, ordinary-click purity, explicit prepared addition, real roots removal, ordinary construction and completed resident visit.");GetTree().Quit();
        }
        catch(Exception e){GD.PrintErr(e);GetTree().Quit(1);}
    }
}

using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;
public partial class Game
{
    private async Task ProbeGrainRelocation()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        async Task Point(Cell at){var p=_camera.UnprojectPosition(OnGround(at.X,at.Z));Input.ParseInputEvent(new InputEventMouseMotion{Position=p,GlobalPosition=p});await Frames();}
        var field=_world.Cottages.Single(c=>c.Kind==BuildingKind.Farm);Check(field.Complete && field.Planted,"Import lacks established grain");
        ShowWorkplaceCard(field.Id);await Frames();await UiClick(_workCardPause);await Frames();
        Check(field.WorkPaused && !_workCardMove.Disabled,"Actual pause did not enable moving");string saved=_world.SaveJson();
        await UiClick(_workCardMove);await Frames();await Press(Key.Escape);await Frames();Check(saved==_world.SaveJson(),"Cancelled move changed world");
        await UiClick(_workCardPause);await Frames();Check(!field.WorkPaused,"Manual resume failed");
        var original=field.Cell;float growth=field.Growth;
        await UiClick(_workCardMove);await Frames();Check(field.WorkPaused && _resumeMovedWork && _movingSite==field.Id,"Pause & move did not begin");
        await Press(Key.Escape);await Frames();Check(!field.WorkPaused && field.Cell==original && field.Growth==growth,"Intent cancel failed to resume/retain crop");
        await UiClick(_workCardMove);await Frames();await Press(Key.F5);await Frames();
        Check(!field.WorkPaused && !MoveIntentActive && _movingSite<0 && !World.LoadFile(CurrentSavePath).Cottages.Single(c=>c.Id==field.Id).WorkPaused,"Manual save retained temporary move pause");
        ShowWorkplaceCard(field.Id);await Frames();await UiClick(_workCardMove);await Frames();saved=_world.SaveJson();_focus=OnGround(0,-4);_camera.Size=27;UpdateCamera();await Frames();
        var bad=_world.Stockpile;await Point(bad);Check(!_ghostValid,"Stockpile overlap accepted");await Click(_camera.UnprojectPosition(OnGround(bad.X,bad.Z)));await Frames();Check(saved==_world.SaveJson(),"Rejected move changed crop");
        var target=_world.Map.Land.OrderBy(c=>c.Z).ThenBy(c=>c.X).First(c=>c!=field.Cell && _world.RelocationProblem(field.Id,c,field.Rotation)==null);
        _focus=OnGround(target.X,target.Z);_camera.Size=24;UpdateCamera();await Frames();await Point(target);Check(_ghostValid,"Legal ground preview rejected");await CaptureReviewBundle("grain-move-proposal");
        await Click(_camera.UnprojectPosition(OnGround(target.X,target.Z)));await Frames();Check(field.Cell==target && !field.WorkPaused && !field.Planted,"Move/crop consequence failed");
        Check(!_resumeMovedWork,"Completed intent retained pause ownership");int grown=_world.Food.GrownGrain;
        _paused=false;_speed=6;double start=_uiTime;while(_world.Food.GrownGrain==grown && _uiTime-start<70)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
        _paused=true;Check(_world.Food.GrownGrain>grown,"Moved field never harvested after resuming");ClearSelection();_focus=OnGround(0,1);_camera.Size=29;UpdateCamera();await Frames();await CaptureReviewBundle("grain-rearranged-working");_world.Validate();
        GD.Print("PASS: actual manually paused move/cancel and working grain Pause & move/cancel/reject/place/automatic resume, real crop reset and subsequent harvest at6x.");
    }
}

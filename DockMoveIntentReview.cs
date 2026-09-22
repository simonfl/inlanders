using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;
public partial class Game
{
    private async Task ProbeDockMoveIntent()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        var dock=_world.Cottages.Single(c=>c.Kind==BuildingKind.FishingDock);Check(dock.Boat?.FisherId!=null,"Import has no fisher at sea");
        ShowWorkplaceCard(dock.Id);await Frames();var cell=dock.Cell;var boat=dock.Boat!.Position;
        await UiClick(_workCardMove);await Frames();Check(_waitingMove==dock.Id && dock.WorkPaused && dock.Boat.Position==boat && dock.Cell==cell,"Dock intent teleported or failed to wait");
        await CaptureReviewBundle("move-waits-for-boat");await UiClick(_workCardMove);await Frames();Check(!dock.WorkPaused && _waitingMove<0,"Wait cancellation failed to resume");
        await UiClick(_workCardMove);await Frames();_paused=false;_speed=6;double start=_uiTime;
        while(_movingSite<0 && _uiTime-start<45)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
        _paused=true;Check(_movingSite==dock.Id && dock.Boat.FisherId==null && _resumeMovedWork,"Safe return did not open move preview");
        await CaptureReviewBundle("returned-dock-move-preview");await Press(Key.Escape);await Frames();Check(!dock.WorkPaused && dock.Cell==cell && _waitingMove<0 && _movingSite<0,"Returned move cancel failed");_world.Validate();
        GD.Print("PASS: actual dock Pause & move waits for real return, cancel resumes, returned boat opens preview without teleport, Escape restores work.");
    }
}

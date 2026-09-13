using Godot;
using Inlanders.Simulation;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async Task ProbeCourtExperience()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        async Task ClickGoal(Button b){_drawerPages[2].EnsureControlVisible(b);await Frames();await UiClick(b);await Frames();}
        ShowMainMenu();await Frames();await UiClick(_mainButtons["Settlements"]);await Frames();await UiClick(_mainButtons["Court comparison"]);await Frames();
        await CaptureReviewBundle("court-choice");await UiClick(_mainButtons["New · A place to gather"]);await Frames();
        Check(_world.CourtStudy is {Finite:true,Finished:false} && _world.Population==16 && _paused && CurrentSavePath==CourtStudyPath(true),"Finite entry/rules/slot failed");
        ToggleDrawer(2);await Frames();Check(_menuButtons[2].Text=="Your place" && !_neighborhoodGoals.IsVisibleInTree() && _courtFinishButton.Visible,"Welcome goals leaked into court brief");
        await CaptureReviewBundle("court-finite-brief");string initial=_world.SaveJson();
        await ClickGoal(_courtBeforeButton);Check(_courtStartingLayout!.Visible && _world.SaveJson()==initial,"Starting footprint view changed village");CloseDrawer();await Frames();
        await CaptureReviewBundle("court-starting-footprints");
        var home=_world.Cottages.First(c=>c.Kind==BuildingKind.Cottage);var original=home.Cell;
        ShowDailyLife(_world.People.First(p=>p.HomeId==home.Id).Id);await Frames();await UiClick(_dailyMove);await Frames();
        var at=_world.Map.Land.OrderBy(c=>(c.Point-new Cell(-7,2).Point).LengthSquared()).First(c=>c!=home.Cell && _world.RelocationProblem(home.Id,c,_rotation)==null);
        _focus=OnGround(at.X,at.Z);UpdateCamera();await Frames();var point=_camera.UnprojectPosition(OnGround(at.X,at.Z));
        Input.ParseInputEvent(new InputEventMouseMotion{Position=point,GlobalPosition=point});await Frames();await Click(point);await Frames();
        Check(home.Cell==at && _world.CourtStudy!.StartingBuildings.Single(b=>b.Id==home.Id).Cell==original,"Editing destroyed starting reference");
        Check(_courtStartingLayout!.Visible,"Moving hid the starting layout comparison");
        ClearSelection();ToggleDrawer(2);await Frames();await ClickGoal(_courtFinishButton);
        Check(_world.CourtStudy!.Finished && _paused && _courtLeaveButton.IsVisibleInTree() && !_courtStartingLayout.Visible,"Finish did not expose ending");
        _noticeUntil=0;await Frames();Check(_drawerPages[2].ScrollVertical==0,"Ending title scrolled out of view");await CaptureReviewBundle("court-finished");
        await ClickGoal(_courtWatchButton);Check(_watching && !_paused && _speed==1 && !_courtStartingLayout.Visible,"Ending watch failed");
        await Frames();ExitWatch();_paused=true;await Frames();SaveWorld();string finished=_world.SaveJson();
        ReturnToMainMenu();await Frames();await UiClick(_mainButtons["Continue"]);await Frames();Check(_world.SaveJson()==finished && CurrentSavePath==CourtStudyPath(true),"Continue lost finished place");
        ToggleDrawer(2);await Frames();await ClickGoal(_courtReopenButton);Check(!_world.CourtStudy!.Finished,"Reopen failed");SaveWorld();string finite=File.ReadAllText(CourtStudyPath(true));
        ReturnToMainMenu();await Frames();CourtExperienceMenu();await Frames();await UiClick(_mainButtons["New · An open court"]);await Frames();
        Check(_world.CourtStudy is {Finite:false,Finished:false} && CurrentSavePath==CourtStudyPath(false),"Open arm/rules/slot failed");ToggleDrawer(2);await Frames();
        Check(!_courtFinishButton.Visible && !_courtLeaveButton.Visible && !_neighborhoodGoals.IsVisibleInTree(),"Open arm contains assigned ending");await CaptureReviewBundle("court-open-brief");
        await Press(Key.F5);string saved=_world.SaveJson();await Press(Key.F9);await Frames();Check(_world.SaveJson()==saved && File.ReadAllText(CourtStudyPath(true))==finite,"Open load overwrote finite save");
        Reset();await Frames();Check(_world.CourtStudy is {Finite:false} && _world.Population==16,"Reset changed comparison arm");
        CloseDrawer();ClearSelection();_world.Validate();
        GD.Print("PASS: finite/open menu, brief, read-only starting footprints, actual home move, player ending/watch/reopen, Continue, separate slots, F5/F9 and reset (scripted UI)");
    }
}

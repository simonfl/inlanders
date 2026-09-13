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
        ShowMainMenu();await Frames();Check(_mainButtons.ContainsKey("Play") && _mainButtons.ContainsKey("Free arrangement") && !_mainButtons.ContainsKey("Settlements"),"Main menu still exposes experiment taxonomy");
        await CaptureReviewBundle("current-main-menu");await UiClick(_mainButtons["Play"]);await Frames();await UiClick(_mainButtons["Short introduction · A place to gather"]);await Frames();
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
        ClearSelection();ToggleDrawer(2);await Frames();Check(_courtFinishButton.Disabled,"Untouched meal project can finish");
        await ClickGoal(_courtMealPlace);Check(_gatherPlanning && _planningCommons,"Meal-place planner is welcome gated");
        var mealAt=_world.Map.Land.OrderBy(c=>(c.Point-new Cell(10,5).Point).LengthSquared()).First(c=>_world.CommonsProblem(c)==null && _world.CommonsFoodNearby(c));
        _focus=OnGround(mealAt.X,mealAt.Z);UpdateCamera();await Frames();var mealPoint=_camera.UnprojectPosition(OnGround(mealAt.X,mealAt.Z));
        await Click(mealPoint);await Frames();await UiClick(_gatherPlanStart);await Frames();Check(_world.Commons!=null,"Meal place not installed");
        for(int i=0;i<1800 && _world.Commons!.FirstDiner==null;i++){_world.Tick(.1f);if(i%100==0){RenderActors(0);UpdateHud();await Frames();}}
        Check(_world.Commons!.FirstDiner!=null,"Place did not serve a real meal");RenderActors(0);_focus=OnGround(3,3);UpdateCamera();await Frames();await CaptureReviewBundle("court-inhabited-meal-place");
        // A same-count move after placing the commons must refresh surface clipping.
        var beside=_world.Cottages.Where(c=>c.Kind==BuildingKind.Cottage).OrderBy(c=>(c.Cell.Point-_world.Commons!.Center.Point).LengthSquared()).First();
        var away=_world.Map.Land.OrderBy(c=>(c.Point-new Cell(14,9).Point).LengthSquared()).First(c=>c!=beside.Cell && _world.RelocationProblem(beside.Id,c,0)==null);
        Check(_world.MoveBuilding(beside.Id,away,0),"Move-after-commons fixture failed");
        CreateActors();RenderActors(0);UpdateCommonsView();await Frames();
        var excluded=_world.Cottages.SelectMany(c=>World.Footprint(c.Cell,c.Rotation,c.Kind)).Concat(_world.Trees.Select(t=>t.Cell)).Concat(_world.Bushes.Select(b=>b.Cell)).Concat(_world.Map.StoneDeposits.Select(d=>d.Cell)).Concat(_world.Decorations.Where(d=>d.Solid).Select(d=>d.Cell)).ToHashSet();
        Check(_commonsGroundExcluded.SetEquals(excluded),"Commons surface retained pre-move building footprint");
        ToggleDrawer(2);await Frames();await ClickGoal(_courtFinishButton);
        Check(_world.CourtStudy!.Finished && _paused && _courtLeaveButton.IsVisibleInTree() && !_courtStartingLayout.Visible,"Finish did not expose ending");
        _noticeUntil=0;await Frames();Check(_drawerPages[2].ScrollVertical==0,"Ending title scrolled out of view");await CaptureReviewBundle("court-finished");
        await ClickGoal(_courtWatchButton);Check(_watching && !_paused && _speed==1 && !_courtStartingLayout.Visible,"Ending watch failed");
        await Frames();ExitWatch();_paused=true;await Frames();SaveWorld();string finished=_world.SaveJson();
        ReturnToMainMenu();await Frames();await UiClick(_mainButtons["Continue"]);await Frames();Check(_world.SaveJson()==finished && CurrentSavePath==CourtStudyPath(true),"Continue lost finished place");
        ToggleDrawer(2);await Frames();await ClickGoal(_courtReopenButton);Check(!_world.CourtStudy!.Finished,"Reopen failed");SaveWorld();string finite=File.ReadAllText(CourtStudyPath(true));
        ReturnToMainMenu();await Frames();await UiClick(_mainButtons["Free arrangement"]);await Frames();await UiClick(_mainButtons["New · An open court"]);await Frames();
        Check(_world.CourtStudy is {Finite:false,Finished:false} && CurrentSavePath==CourtStudyPath(false),"Open arm/rules/slot failed");ToggleDrawer(2);await Frames();
        Check(!_courtFinishButton.Visible && !_courtLeaveButton.Visible && !_neighborhoodGoals.IsVisibleInTree() && !_objective.Text.Contains("Choose a meal place"),"Open arm contains assigned ending or task");await CaptureReviewBundle("court-open-brief");
        await Press(Key.F5);string saved=_world.SaveJson();await Press(Key.F9);await Frames();Check(_world.SaveJson()==saved && File.ReadAllText(CourtStudyPath(true))==finite,"Open load overwrote finite save");
        ReturnToMainMenu();await Frames();await UiClick(_mainButtons["Play"]);await Frames();await UiClick(_mainButtons["Short introduction · A place to gather"]);await Frames();
        Check(_mainButtons.ContainsKey("Resume · A place to gather") && !_mainButtons.ContainsKey("New · An open court"),"Project page mixes save modes");
        await UiClick(_mainButtons["New · A place to gather"]);await Frames();await UiClick(_mainButtons["Cancel"]);await Frames();
        Check(File.ReadAllText(CourtStudyPath(true))==finite,"Cancelled New replaced project");
        await UiClick(_mainButtons["Resume · A place to gather"]);await Frames();Check(_world.SaveJson()==finite,"Explicit project resume chose another save");
        ReturnToMainMenu();await Frames();await UiClick(_mainButtons["Free arrangement"]);await Frames();await UiClick(_mainButtons["Resume · An open court"]);await Frames();Check(_world.SaveJson()==saved,"Explicit free resume chose another save");
        ReturnToMainMenu();await Frames();await UiClick(_mainButtons["Continue"]);await Frames();Check(_world.SaveJson()==saved,"Continue did not follow free arrangement");
        ReturnToMainMenu();await Frames();await UiClick(_mainButtons["Earlier prototypes"]);await Frames();await UiClick(_mainButtons["Earlier settlements"]);await Frames();
        Check(new[]{"New neighborhood","New Willow court","New Willow inlet","New meadow settlement"}.All(_mainButtons.ContainsKey),"Archived settlements are unreachable");
        await CaptureReviewBundle("archived-settlements");await UiClick(_mainButtons["Back"]);await Frames();
        Check(_mainButtons.ContainsKey("Earlier free court"),"Archive Back returned to wrong page");await UiClick(_mainButtons["Earlier free court"]);await Frames();Check(_mainButtons.ContainsKey("New free arrangement"),"Old free court disappeared");
        await UiClick(_mainButtons["Back"]);await Frames();await UiClick(_mainButtons["Back"]);await Frames();
        Check(File.ReadAllText(CourtStudyPath(true))==finite && File.ReadAllText(CourtStudyPath(false))==saved,"Browsing archive changed current saves");
        await UiClick(_mainButtons["Continue"]);await Frames();
        Reset();await Frames();Check(_world.CourtStudy is {Finite:false} && _world.Population==16,"Reset changed comparison arm");
        CloseDrawer();ClearSelection();_world.Validate();
        GD.Print("PASS: finite/open menu, brief, read-only starting footprints, actual home move, player ending/watch/reopen, Continue, separate slots, explicit resumes, cancelled replacement, archive navigation, F5/F9 and reset (scripted UI)");
    }
}

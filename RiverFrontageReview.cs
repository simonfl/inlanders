using Godot;
using Inlanders.Simulation;
using System;
using System.Threading.Tasks;
using System.Linq;
public partial class Game
{
    private async Task ProbeRiverFrontage()
    {
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        if(_reviewRequest!.RootElement.GetProperty("scenario").GetString()=="river-hamlet")
        {
            ShowMainMenu();await Frames();await UiClick(_mainButtons["Play"]);await Frames();await UiClick(_mainButtons["Earlier beginnings"]);await Frames();await UiClick(_mainButtons["Shape an inhabited hamlet"]);await Frames();
            await UiClick(_mainButtons["Landscape: inlet · change"]);await Frames();await CaptureReviewBundle("inhabited-frontage-menu");
            await UiClick(_mainButtons["New hamlet"]);await Frames();
            if(_world.PublicPlace is not {RiverFrontage:true,PlayerFounded:false} || _world.Housed!=12)throw new Exception("Inhabited frontage entry failed");
            await ProbeHousehold();
            _paused=false;_speed=6;double turnStart=_uiTime;while(_uiTime-turnStart<3)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);_paused=true;
            var turnHome=_world.Cottages.ToArray().First(c=>c.Kind==BuildingKind.Cottage && Enumerable.Range(0,4).Any(r=>r!=c.Rotation && _world.RelocationProblem(c.Id,c.Cell,r)==null));
            ShowWorkplaceCard(turnHome.Id);await Frames();await UiClick(_homeOptionsButton);await Frames();string turnBefore=_world.SaveJson();var anchor=turnHome.Cell;
            await UiClick(_turnHomeButton);await Frames();await UiClick(_turnRight);await Frames();await CaptureReviewBundle("home-direction-preview");await Press(Key.Escape);await Frames();
            if(_world.SaveJson()!=turnBefore)throw new Exception("Cancelled home turn mutated village");
            await UiClick(_turnHomeButton);await Frames();for(int i=0;i<4 && _turnApply.Disabled;i++){await UiClick(_turnRight);await Frames();}
            int wanted=_homeTurn;await UiClick(_turnApply);await Frames();if(turnHome.Rotation!=wanted || turnHome.Cell!=anchor || _turnHome>=0)throw new Exception("Home turn failed");_world.Validate();await CaptureReviewBundle("home-turned");
            if(!_homeAppearance.IsVisibleInTree() || _workCard.GetGlobalRect().End.Y>_hud.Size.Y-75)throw new Exception("Home editing overflows compact view");
            await UiClick(_homeAppearance);await Press(Key.Down);await Press(Key.Enter);await Frames();
            if(turnHome.Finish==CottageFinish.Automatic)throw new Exception("Appearance choice not applied");await Press(Key.Escape);await Frames();await CaptureReviewBundle("home-appearance-chosen");
            ShowWorkplaceCard(turnHome.Id);await Frames();await UiClick(_homeOptionsButton);await Frames();string pathBefore=_world.SaveJson();
            await UiClick(_placePath);await Frames();if(_pathAnchor!=turnHome.Entrance)throw new Exception("Path origin lost selected entrance");await Press(Key.Escape);await Frames();if(_world.SaveJson()!=pathBefore || _workCardSite!=turnHome.Id)throw new Exception("Cancelled path lost state/context");
            await UiClick(_homeOptionsButton);await Frames();await UiClick(_placePath);await Frames();
            var destination=_world.Map.Land.Where(c=>c!=turnHome.Entrance && !_world.Paths.Contains(c) && _world.PathConnection(turnHome.Entrance,c,out _)==null).OrderBy(c=>(c.Point-turnHome.Entrance.Point).LengthSquared()).First();
            _focus=OnGround(destination.X,destination.Z);_camera.Size=18;UpdateCamera();await Frames();var pathPoint=_camera.UnprojectPosition(OnGround(destination.X,destination.Z));Input.ParseInputEvent(new InputEventMouseMotion{Position=pathPoint,GlobalPosition=pathPoint});await Frames();await CaptureReviewBundle("path-from-selected-home");await Click(pathPoint);await Frames();
        await UiClick(_pathProposalApply);await Frames();
            if(!_world.Paths.Contains(destination) || _placing || _workCardSite!=turnHome.Id)throw new Exception("Selected-place path did not apply/return");_world.Validate();await CaptureReviewBundle("home-path-connected");
            await Press(Key.F5);string appearanceSave=_world.SaveJson();await Press(Key.F9);await Frames();if(appearanceSave!=_world.SaveJson())throw new Exception("Home appearance/turn save differs");

            CloseManagementUi();_paused=true;var origin=_focus;float zoom=_camera.Size;string before=_world.SaveJson();
            await OpenMenu(2);await Frames();await UiClick(_landSurvey);await Frames();
            foreach(var button in _landViewButtons){await UiClick(button);await Frames();if(!_surveying || !_paused || before!=_world.SaveJson())throw new Exception("Land views changed simulation");}
            await CaptureReviewBundle("look-over-the-land");await Press(Key.Escape);await Frames();
            if(_surveying || _focus!=origin || _camera.Size!=zoom || before!=_world.SaveJson())throw new Exception("Land survey return lost view/state");
            await OpenMenu(2);await Frames();await UiClick(_landSurvey);await Frames();await UiClick(_landViewButtons[0]);await Frames();
            var chosen=_focus;float chosenZoom=_camera.Size;await UiClick(_surveyBuildHere);await Frames();
            if(_surveying || !_drawer.Visible || _focus!=chosen || _camera.Size!=chosenZoom || before!=_world.SaveJson())throw new Exception("Survey build lost chosen view");
            await UiClick(_essentialChoices[BuildingKind.VegetableField]);await Frames();
            if(!_placing || _focus!=chosen)throw new Exception("Survey-to-field placement lost place");
            await Press(Key.Escape);await Frames();if(before!=_world.SaveJson())throw new Exception("Survey action cancel changed village");
            await CaptureReviewBundle("survey-to-building");
            SelectPerson(0);await Frames();await UiClick(_happinessButton);await Frames();
            if(_happinessButton.Text!="Daily life" || _happinessButton.Text.Contains("/100") || _happinessReasons.Text.Contains("balanced") || _staffing.Text.Contains("Village happiness"))throw new Exception("Public mood still advertises a completion score");
            if(before!=_world.SaveJson())throw new Exception("Mood inspection changed village");await CaptureReviewBundle("ordinary-life-mood");
            GD.Print("PASS: land views/action/cancellation and score-free public mood inspection; person selected by fixture API.");return;
        }
        _frontageStart=false;ShowMainMenu();await Frames();await UiClick(_mainButtons["Play"]);await Frames();
        await UiClick(_mainButtons["Earlier beginnings"]);await Frames();await UiClick(_mainButtons["Establish a farmstead"]);await Frames();
        await UiClick(_mainButtons["Landscape: inlet · change"]);await Frames();
        await CaptureReviewBundle("frontage-menu");
        await UiClick(_mainButtons["New farmstead"]);await Frames();
        if(_world.PublicPlace?.RiverFrontage!=true)throw new Exception("Frontage menu opened wrong world");
        await Press(Key.F5);string saved=_world.SaveJson();await Press(Key.F9);await Frames();
        if(_world.SaveJson()!=saved || _world.PublicPlace?.RiverFrontage!=true)throw new Exception("Frontage save identity lost");
        await ProbePlayerFounded();
        GD.Print("PASS: actual river-frontage menu, exact save identity, ordinary home/garden construction, meals and rest.");
    }
}

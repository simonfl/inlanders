using Godot;
using System;
using System.Threading.Tasks;
using Inlanders.Simulation;
using System.Linq;
public partial class Game
{
    private async Task ProbeWorkingClearing()
    {
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        ShowMainMenu();await Frames();await UiClick(_mainButtons["Play"]);await Frames();await UiClick(_mainButtons["Earlier beginnings"]);await Frames();await UiClick(_mainButtons["Tend a working clearing"]);await Frames();await CaptureReviewBundle("working-clearing-menu");
        foreach(var label in new[]{"New clearing","New relaxed clearing"})if(!_mainPanel.GetGlobalRect().Encloses(_mainButtons[label].GetGlobalRect()))throw new Exception("Mode choice hidden below menu fold");
        await UiClick(_mainButtons["New clearing"]);await Frames();if(_world.PublicPlace?.WorkingClearing!=true || _world.Population!=8)throw new Exception("Wrong clearing entry");
        await ProbeHousehold();string before=_world.SaveJson();await Press(Key.F5);await Press(Key.F9);await Frames();if(before!=_world.SaveJson())throw new Exception("Clearing save identity");
        _paused=false;_speed=6;double began=_uiTime;while(_uiTime-began<12)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);_paused=true;_world.Validate();await CaptureReviewBundle("working-clearing-life");
        var home=_world.Cottages.First(c=>c.Kind==BuildingKind.Cottage);ShowWorkplaceCard(home.Id);await Frames();await UiClick(_homeOptionsButton);await Frames();if(_householdChange.Visible || _workCardDetails.Visible)throw new Exception("Secondary actions crowd arrangement tray");await CaptureReviewBundle("home-arrangement-tray");string moveBefore=_world.SaveJson();
        var at=_world.Map.Land.OrderBy(c=>(c.Point-new Cell(2,1).Point).LengthSquared()).First(c=>_world.RelocationProblem(home.Id,c,home.Rotation)==null);
        await UiClick(_workCardMove);await Frames();_focus=OnGround(at.X,at.Z);_camera.Size=22;UpdateCamera();await Frames();var point=_camera.UnprojectPosition(OnGround(at.X,at.Z));Input.ParseInputEvent(new InputEventMouseMotion{Position=point,GlobalPosition=point});await Frames();
        if(!_ghostValid || _livelihoodSite is not {Route.Length:>0} || !_hint.Text.Contains("Possible food walk"))throw new Exception("Moved home lacks connection preview");await CaptureReviewBundle("moving-home-food-connection");await Press(Key.Escape);await Frames();
        if(_world.SaveJson()!=moveBefore || _workCardSite!=home.Id)throw new Exception("Move proposal cancellation changed world/context");
        ShowWorkplaceCard(home.Id);await Frames();await UiClick(_homeOptionsButton);await Frames();await UiClick(_placePath);await Frames();
        var field=_world.Cottages.First(c=>c.Kind==BuildingKind.VegetableField);var destination=field.Entrance;
        _focus=OnGround(destination.X,destination.Z);_camera.Size=24;UpdateCamera();await Frames();var pathPoint=_camera.UnprojectPosition(OnGround(destination.X,destination.Z));
        Input.ParseInputEvent(new InputEventMouseMotion{Position=pathPoint,GlobalPosition=pathPoint});await Frames();await CaptureReviewBundle("clearing-path-proposal");await Click(pathPoint);await Frames();
        await UiClick(_pathProposalApply);await Frames();
        if(!_world.Paths.Contains(destination) || _placing)throw new Exception("Clearing connection did not apply");UpdateWorkedLandscape(true);await Frames();_world.Validate();await CaptureReviewBundle("clearing-path-opened");
        ShowWorkplaceCard(home.Id);await Frames();await UiClick(_homeOptionsButton);await Frames();
        var returnFocus=_focus;float returnZoom=_camera.Size,returnAngle=_angle;string beforeWatch=_world.SaveJson();
        await UiClick(_workCardWatchPlace);await Frames();
        if(!_watching || _followPerson || _world.SaveJson()!=beforeWatch || !_watchReturn.Text.Contains("this place"))throw new Exception("Local observation changed world or lacks return context");
        await CaptureReviewBundle("watch-home-and-working-ground");await Press(Key.Escape);await Frames();
        if(_workCardSite!=home.Id || !_homeOptions || _focus!=returnFocus || _camera.Size!=returnZoom || _angle!=returnAngle)throw new Exception("Local observation lost arrangement or camera context");
        GD.Print("PASS working clearing actual menu, household controls, save identity and ordinary life");
    }
}

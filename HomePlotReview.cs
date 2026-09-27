using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;
public partial class Game
{
    private async Task ProbeHomePlot()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        CloseManagementUi();ClearSelection();await Frames();string before=_world.SaveJson();await UiClick(_firstPlaceChoices[BuildingKind.Cottage]);await Frames();await Press(Key.Y);await Frames();
        Check(_homePlotSide==0 && _homePlotPanel.Visible,"Home-and-yard option inaccessible");
        _rotation=0;_focus=OnGround(1,8);_camera.Size=20;UpdateCamera();await Frames();
        var point=_camera.UnprojectPosition(OnGround(1,8));Input.ParseInputEvent(new InputEventMouseMotion{Position=point,GlobalPosition=point});await Frames();
        Check(_ghostValid && _homePlotInfo.Text.Contains("4 planks"),"Plot/cost preview wrong");await CaptureReviewBundle("home-and-yard-proposal");
        await Press(Key.Escape);await Frames();Check(before==_world.SaveJson() && !_homePlotPanel.Visible,"Cancelled plot changed world");
        await UiClick(_firstPlaceChoices[BuildingKind.Cottage]);await Press(Key.Y);await Frames();Input.ParseInputEvent(new InputEventMouseMotion{Position=point,GlobalPosition=point});await Frames();await Click(point);await Frames();
        var home=_world.Cottages.Single();Check(home.PlannedYard && _workCardFurnish.Text=="Cancel planned yard","Combined plan did not queue furnishing");await CaptureReviewBundle("home-and-yard-order");
        await Press(Key.Escape);await Press(Key.B);await Frames();await UiClick(_categoryButtons.First(b=>b.Text.TrimStart('›',' ')==BuildingCategoryNames[2]));await Frames();_drawerPages[1].EnsureControlVisible(_kindButtons[BuildingKind.VegetableGarden]);await Frames();await UiClick(_kindButtons[BuildingKind.VegetableGarden]);CloseDrawer();await Frames();
        var gardenPoint=_camera.UnprojectPosition(OnGround(4,8));Input.ParseInputEvent(new InputEventMouseMotion{Position=gardenPoint,GlobalPosition=gardenPoint});await Frames();Check(_ghostValid && _livelihoodSite?.Destination=="Planned home","Adjacent garden fails to connect the planned home");await CaptureReviewBundle("planned-food-connection");await Click(gardenPoint);await Press(Key.Escape);await Frames();
        _paused=false;_speed=6;double start=_uiTime;
        while(_uiTime-start<65 && !(home.Improved && _world.People.Any(p=>p.HomeId==home.Id && p.Task==Work.EatingMeal && p.Meal?.Kind==Inlanders.Simulation.Resource.Vegetables && _world.AtFurnishedHome(p))))await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
        _paused=true;Check(home.Improved && _world.People.Any(p=>p.HomeId==home.Id && p.Task==Work.EatingMeal && p.Meal?.Kind==Inlanders.Simulation.Resource.Vegetables && _world.AtFurnishedHome(p)),"Planned yard did not become an actual home meal");
        ClearSelection();await CaptureReviewBundle("planned-home-in-use");_world.Validate();
        GD.Print("PASS: actual optional home/yard choice, pure cancel, combined order, nearby independently placed garden, real construction/furnishing and ordinary meal from the chosen garden at home.");
    }
}

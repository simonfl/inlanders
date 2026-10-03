using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;
public partial class Game
{
    private async Task ProbeCultivation()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        CloseManagementUi();ClearSelection();await Press(Key.B);await Frames();await UiClick(_categoryButtons.First(b=>b.Text.TrimStart('›',' ')==BuildingCategoryNames[2]));await Frames();_drawerPages[1].EnsureControlVisible(_kindButtons[BuildingKind.VegetableField]);await Frames();await UiClick(_kindButtons[BuildingKind.VegetableField]);CloseDrawer();await Frames();
        await UiClick(_plotLess);await UiClick(_plotLess);await Frames();Check(_plotRows==3,"Length controls failed");
        _rotation=0;_focus=OnGround(1,-5);_camera.Size=22;UpdateCamera();await Frames();
        var point=_camera.UnprojectPosition(OnGround(1,-5));Input.ParseInputEvent(new InputEventMouseMotion{Position=point,GlobalPosition=point});await Frames();Check(_ghostValid,"Strip proposal invalid");
        Check(_plotInfo.Text.Contains("12 vegetables") && _hint.Text.Contains("6 logs"),"Size/cost feedback disagrees");await CaptureReviewBundle("chosen-cultivated-ground");
        await Click(point);await Press(Key.Escape);await Frames();var field=_world.Cottages.Single();Check(field.PlotRows==3 && World.Footprint(field).Count()==9,"Placed wrong extent");
        _paused=false;_speed=6;double start=_uiTime;while(_uiTime-start<80 && _world.Food.EatenVegetables==0)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
        _paused=true;Check(field.Complete && _world.Food.GrownVegetables>=12 && _world.Food.EatenVegetables>0,"Chosen ground did not feed actual meal");ShowWorkplaceCard(field.Id);await Frames();await CaptureReviewBundle("cultivation-in-use");_world.Validate();
        if(_reviewRequest!.RootElement.GetProperty("scenario").GetString()=="plot-revision")
        {
            _paused=false;start=_uiTime;while(_uiTime-start<60 && (field.Harvest>0 || field.Planted || _world.People.Any(p=>p.WorkplaceId==field.Id)))await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
            _paused=true;_nextWorkCard=0;await Frames();Check(!field.WorkPaused && !_reshapePlot.Disabled,$"Finished field cannot reshape: paused={field.WorkPaused}, ripe={field.Harvest}, disabled={_reshapePlot.Disabled}");
            string saved=_world.SaveJson();await UiClick(_reshapePlot);await Frames();await UiClick(_revisionLess);await Frames();await CaptureReviewBundle("release-growing-ground");
            Check(_workCard.GetGlobalRect().End.Y<_hud.Size.Y-75,"Revision card overflows compact view");await Press(Key.Escape);await Frames();Check(_world.SaveJson()==saved,"Cancelled revision changed world");
            await UiClick(_reshapePlot);await Frames();await UiClick(_revisionLess);await Frames();await UiClick(_revisionApply);await Frames();Check(field.Depth==2 && field.PreparedRows==3 && !field.WorkPaused,"Revision lost extent/state");
            Check(!field.WorkPaused,"Revision left work paused");await CaptureReviewBundle("ground-released");_world.Validate();
            await UiClick(_reshapePlot);await Frames();await UiClick(_revisionMore);await UiClick(_revisionMore);await Frames();
            Check(_revisionInfo.Text.Contains("2 extra logs") && !_revisionApply.Disabled,"Extension cost/proposal missing");await CaptureReviewBundle("extend-existing-ground");
            await UiClick(_revisionApply);await Frames();Check(field.Depth==4 && field.Required==8 && !field.Complete,"Extension skipped preparation");
            _paused=false;start=_uiTime;while(_uiTime-start<35 && !field.Complete)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
            _paused=true;Check(field.Complete,"Extended ground was not prepared");_world.Validate();await CaptureReviewBundle("extended-ground-prepared");
        }
        GD.Print("PASS: catalogue to actual variable strip, visible matching cost/extent, real preparation/crop/vegetable meal.");
    }
}

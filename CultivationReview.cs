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
        GD.Print("PASS: catalogue to actual variable strip, visible matching cost/extent, real preparation/crop/vegetable meal.");
    }
}

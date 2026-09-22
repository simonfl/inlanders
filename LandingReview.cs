using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;
public partial class Game
{
    private async Task ProbeLanding()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        async Task Point(Cell cell){var p=_camera.UnprojectPosition(OnGround(cell.X,cell.Z));Input.ParseInputEvent(new InputEventMouseMotion{Position=p,GlobalPosition=p});await Frames();}
        await Press(Key.B);await Frames();
        await UiClick(_categoryButtons.First(b=>b.Text.TrimStart('›',' ')==BuildingCategoryNames[2]));await Frames();
        await UiClick(_kindButtons[BuildingKind.FishingDock]);await Frames();await Press(Key.Escape);await Frames();
        Check(!_placing,"Cancel left landing preview active");
        BeginPlacement(BuildingKind.FishingDock);CloseDrawer();_rotation=1;_focus=OnGround(7,7);_camera.Size=18;UpdateCamera();await Frames();
        string initial=_world.SaveJson();await Point(new(5,7));Check(!_ghostValid,"Overlapping landing was accepted");
        await Click(_camera.UnprojectPosition(OnGround(5,7)));await Frames();Check(initial==_world.SaveJson(),"Rejected landing changed world");
        await Point(new(8,8));Check(_ghostValid,"Valid landing preview rejected: "+_placementProblem);await CaptureReviewBundle("landing-proposal");
        await Click(_camera.UnprojectPosition(OnGround(8,8)));await Frames();
        var dock=_world.Cottages.SingleOrDefault(c=>c.Kind==BuildingKind.FishingDock);Check(dock!=null,"World click did not place landing");
        await Press(Key.Escape);_paused=false;_speed=6;
        double start=_uiTime;while(dock!.PantryFood.Sum()==0 && _uiTime-start<65)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
        _paused=true;Check(dock.Complete && dock.PantryFood.Sum()>0,"Ordinary construction/fishing did not reach stored catch");
        FrameMap();await Frames();await CaptureReviewBundle("landing-working");_world.Validate();
        GD.Print("PASS: catalogue landing choice/cancel, rejected overlap, valid three-cell world placement and actual construction/boat/catch at6x.");
    }
}

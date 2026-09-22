using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;
public partial class Game
{
    private async Task ProbePlayerFounded()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        Check(_world.PublicPlace?.PlayerFounded==true && _world.Cottages.Count==0,"Not an open founding");
        // Intention: four homes share southern ground; keep a garden close without filling the shore.
        foreach(var plan in new[]{(BuildingKind.Cottage,new Cell(-3,7)),(BuildingKind.Cottage,new Cell(1,7)),(BuildingKind.Cottage,new Cell(-3,11)),(BuildingKind.Cottage,new Cell(1,11)),(BuildingKind.VegetableGarden,new Cell(4,3)),(BuildingKind.VegetableGarden,new Cell(4,7))})
        {
            await Press(Key.B);await Frames();await UiClick(_categoryButtons.First(b=>b.Text.TrimStart('›',' ')==BuildingCategoryNames[plan.Item1==BuildingKind.Cottage?1:2]));await Frames();
            _drawerPages[1].EnsureControlVisible(_kindButtons[plan.Item1]);await Frames();await UiClick(_kindButtons[plan.Item1]);CloseDrawer();_rotation=0;_focus=OnGround(0,7);_camera.Size=23;UpdateCamera();await Frames();
            var point=_camera.UnprojectPosition(OnGround(plan.Item2.X,plan.Item2.Z));Input.ParseInputEvent(new InputEventMouseMotion{Position=point,GlobalPosition=point});await Frames();Check(_ghostValid,"Founding proposal rejected: "+_placementProblem);await Click(point);await Frames();await Press(Key.Escape);await Frames();
        }
        _paused=false;_speed=6;double start=_uiTime;
        while(_uiTime-start<65 && (_world.Housed<8 || !_world.People.Any(p=>p.HomeId!=null && p.Task==Work.Resting)))await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
        _paused=true;Check(_world.Housed==8 && _world.People.Any(p=>p.HomeId!=null && p.Task==Work.Resting),"Built homes did not become inhabited/rested");
        ClearSelection();_camera.Size=26;UpdateCamera();await Frames();await CaptureReviewBundle("player-established-homes");_world.Validate();
        GD.Print("PASS: actual catalogue and world placement of chosen homes/gardens, real construction, household occupation and home rest at6x. No strategic preference claim.");
    }
}

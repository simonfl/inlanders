using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;
public partial class Game
{
    private async Task ProbeOvenWorkyard()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        foreach(var plan in new[]{(BuildingKind.Farm,new Cell(-3,-5)),(BuildingKind.Bakery,new Cell(-1,-3))})
        {
            await Press(Key.B);await Frames();
            await UiClick(_categoryButtons.First(b=>b.Text.TrimStart('›',' ')==BuildingCategoryNames[2]));await Frames();
            _drawerPages[1].EnsureControlVisible(_kindButtons[plan.Item1]);await Frames();await UiClick(_kindButtons[plan.Item1]);CloseDrawer();
            _rotation=0;_focus=OnGround(-2,-3);_camera.Size=18;UpdateCamera();await Frames();
            var point=_camera.UnprojectPosition(OnGround(plan.Item2.X,plan.Item2.Z));Input.ParseInputEvent(new InputEventMouseMotion{Position=point,GlobalPosition=point});await Frames();
            Check(_ghostValid,"Workyard proposal rejected: "+_placementProblem);await CaptureReviewBundle("workyard-"+plan.Item1+"-proposal");
            await Click(point);await Frames();await Press(Key.Escape);await Frames();
            Check(_world.Cottages.Any(c=>c.Kind==plan.Item1 && c.Cell==plan.Item2),"Workyard world placement failed");
        }
        var oven=_world.Cottages.Single(c=>c.Kind==BuildingKind.Bakery);_paused=false;_speed=6;
        bool baked=false,meal=false;double start=_uiTime;
        while(_uiTime-start<90 && !meal)
        {
            await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
            if(!baked && _world.People.Any(p=>p.WorkplaceId==oven.Id && p.Task==Work.Baking))
            {baked=true;await CaptureReviewBundle("workyard-baking");}
            meal=_world.People.Any(p=>p.Meal is {Carrying:true,Kind:Inlanders.Simulation.Resource.Bread} && p.Meal.SourceId==oven.Id);
        }
        _paused=true;Check(baked && meal,"Ordinary workyard did not progress from baking to carried meal");
        Check(_cottages[oven.Id].Body.GetNodeOrNull<Node3D>("OvenGlow")!=null,"Workyard glow missing");
        _camera.Size=23;_focus=OnGround(0,1);UpdateCamera();await Frames();await CaptureReviewBundle("workyard-bread-meal");_world.Validate();
        GD.Print("PASS: real catalogue/world grain and oven placement, normal construction, baking and carried bread meal at6x.");
    }
}

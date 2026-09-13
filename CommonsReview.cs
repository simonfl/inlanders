using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;
public partial class Game
{
    private async Task ProbeCommons()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<4;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        var center=_world.Map.Land.Where(c=>_world.CommonsProblem(c)==null).OrderBy(c=>(c.Point-new Cell(17,9).Point).LengthSquared()).First();
        await OpenMenu(2);await UiClick(_commonsEntry);await Frames();
        _focus=OnGround(center.X,center.Z);_camera.Size=17;UpdateCamera();await Frames();
        await Click(_camera.UnprojectPosition(OnGround(center.X,center.Z)));await Frames();
        Check(!_gatherPlanStart.Disabled,"Commons ground plan refused");await CaptureReviewBundle("commons-plan");
        await UiClick(_gatherPlanStart);await Frames();Check(_world.Commons!=null,"Commons confirmation failed");
        await CaptureReviewBundle("commons-empty");
        for(int i=0;i<1800 && !_world.People.Any(p=>p.Meal?.Commons==true && p.Task==Work.EatingMeal);i++)
        {_world.Tick(.1f);if(i%50==0){RenderActors(0);UpdateHud();await Frames();}}
        Check(_world.People.Any(p=>p.Meal?.Commons==true && p.Task==Work.EatingMeal),"No ordinary commons meal");
        RenderActors(0);UpdateHud();await Frames();await CaptureReviewBundle("commons-ordinary-meal");
        await Press(Key.F5);string saved=_world.SaveJson();await Press(Key.F9);await Frames();Check(_world.SaveJson()==saved,"Commons save/load differs");
        await OpenMenu(2);await UiClick(_commonsEntry);await Frames();await Press(Key.Escape);Check(_world.SaveJson()==saved,"Cancelled rearrangement changed place");
        await OpenMenu(2);await UiClick(_commonsRemove);await Frames();Check(_world.Commons==null,"Commons removal failed");
        _world.Validate();await CaptureReviewBundle("commons-removed");GD.Print("PASS: commons ground placement, ordinary real meal, save/load, cancelled rearrangement and removal");
    }
}

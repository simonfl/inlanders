using Godot;
using System;
using System.Linq;
using Inlanders.Simulation;
using System.Threading.Tasks;
public partial class Game
{
    private async Task ProbeRiverLivelihood()
    {
        async Task Frames(){for(int i=0;i<6;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        _riverRelaxed=false;ShowMainMenu();await Frames();await UiClick(_mainButtons["Play"]);await Frames();await UiClick(_mainButtons["Establish life by the river"]);await Frames();await CaptureReviewBundle("river-livelihood-menu");
        await UiClick(_mainButtons["New settlement"]);await Frames();
        if(_world.PublicPlace is not {RiverLandscape:true,PlayerFounded:true} || _world.Housed!=12 || _world.Cottages.Count!=6)throw new Exception("Wrong livelihood opening");
        if(!_firstPlace.IsVisibleInTree())throw new Exception("Arrival has no first livelihood choices");
        await CaptureReviewBundle("first-livelihood-choices");
        await Press(Key.U);await Frames();var berrySource=_world.ResourceSources().Single(s=>s.Key.Kind==SourceKind.Berries);
        SelectResourceSource(berrySource.Key,true);await Frames();_inspectionScroll.EnsureControlVisible(_surveyBuildHere);await Frames();
        await UiClick(_surveyBuildHere);await Frames();if(!_placing || _buildKind!=BuildingKind.ForagerHut || _surveying)throw new Exception("Source-led berry planning failed");
        _focus=OnGround(0,-7);_camera.Size=22;UpdateCamera();await Frames();
        var berryPoint=_camera.UnprojectPosition(OnGround(0,-7));Input.ParseInputEvent(new InputEventMouseMotion{Position=berryPoint,GlobalPosition=berryPoint});await Frames();
        if(!_ghostValid)throw new Exception("Berry hut proposal refused");
        await CaptureReviewBundle("berry-gathering-proposal");await Press(Key.Escape);await Frames();
        await UiClick(_firstPlaceChoices[BuildingKind.VegetableField]);await Frames();
        while(_rotation!=1)await Press(Key.R);
        _focus=OnGround(2,-7);_camera.Size=22;UpdateCamera();await Frames();
        var point=_camera.UnprojectPosition(OnGround(2,-7));Input.ParseInputEvent(new InputEventMouseMotion{Position=point,GlobalPosition=point});await Frames();
        if(!_ghostValid)throw new Exception("First cultivation refused: "+_placementProblem);
        await Click(point);await Press(Key.Escape);await Frames();
        var field=_world.Cottages.Single(c=>c.Kind==BuildingKind.VegetableField);
        _paused=false;_speed=6;double start=_uiTime;
        while(_uiTime-start<75 && _world.Food.EatenVegetables==0)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
        _paused=true;if(!field.Complete || _world.Food.EatenVegetables==0)throw new Exception("Ordinary first food never fed a neighbor");
        await CaptureReviewBundle("chosen-food-in-use");
        await ProbeHousehold();string saved=_world.SaveJson();await Press(Key.F5);await Press(Key.F9);await Frames();if(saved!=_world.SaveJson())throw new Exception("Livelihood save differs");
        await Press(Key.H);await Frames();await CaptureReviewBundle("river-livelihood-watch");await Press(Key.Escape);await Frames();if(_watching)throw new Exception("Watch exit failed");
        ReturnToMainMenu();await Frames();await UiClick(_mainButtons["Play"]);await Frames();await CaptureReviewBundle("public-river-beginnings");await UiClick(_mainButtons["Establish life by the river"]);await Frames();
        await UiClick(_mainButtons["Mode: Normal · change"]);await Frames();await CaptureReviewBundle("river-relaxed-menu");await UiClick(_mainButtons["New settlement"]);await Frames();
        if(_world.PublicPlace is not {Relaxed:true,RiverLandscape:true,PlayerFounded:true})throw new Exception("Wrong relaxed start");
        GD.Print("PASS river livelihood ordinary menu, homes, current save and Watch.");
    }
}

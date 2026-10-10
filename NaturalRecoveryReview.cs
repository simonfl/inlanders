using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;
public partial class Game
{
    private async Task ProbeNaturalRecovery()
    {
        async Task Frames(){for(int i=0;i<6;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        if(_world.EdibleStored>=_world.Population*2)throw new Exception("Recovery fixture not low");
        await Click(_placeFood.GetGlobalRect().GetCenter());await Frames();if(!_drawer.Visible || _tabs.CurrentTab!=4)throw new Exception("Low food cannot open supply investigation");
        await CaptureReviewBundle("low-reserve-investigation");await Press(Key.U);await Frames();
        var berry=_world.ResourceSources().Single(s=>s.Key.Kind==SourceKind.Berries);SelectResourceSource(berry.Key,true);await Frames();
        _inspectionScroll.EnsureControlVisible(_surveyBuildHere);await Frames();await UiClick(_surveyBuildHere);await Frames();
        while(_rotation!=2)await Press(Key.R);
        _focus=OnGround(-4,10);_camera.Size=20;UpdateCamera();await Frames();var point=_camera.UnprojectPosition(OnGround(-4,10));
        Input.ParseInputEvent(new InputEventMouseMotion{Position=point,GlobalPosition=point});await Frames();
        if(!_ghostValid)throw new Exception("Recovery hut blocked: "+_placementProblem);
        await Click(point);await Press(Key.Escape);await Frames();
        var hut=_world.Cottages.Single(c=>c.Kind==BuildingKind.ForagerHut);float start=_world.Food.Time;double wall=_uiTime;int lateMisses=0;
        _paused=false;_speed=6;
        while(_world.Food.Time-start<600)
        {
            if(_uiTime-wall>150)throw new Exception("Recovery observation timeout");
            await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);if(_world.Food.Time-start>300)lateMisses+=_world.People.Count(p=>!p.Fed);
        }
        _paused=true;if(!hut.Complete || _world.Food.GatheredBerries==0 || lateMisses>0 || _world.Cottages.Any(c=>World.IsVegetablePlot(c.Kind)))throw new Exception("Non-cultivation recovery failed");
        await CaptureReviewBundle("shore-and-berries-recovered");SaveWorld();string saved=_world.SaveJson();await Press(Key.F9);await Frames();if(saved!=_world.SaveJson())throw new Exception("Natural recovery save differs");
        GD.Print("PASS native low reserve, source-led berry investment, real ten-minute shore recovery with final five minutes fed, no cultivation and exact save.");
    }
}

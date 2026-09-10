using Godot;
using Inlanders.Simulation;
using System;
using System.Threading.Tasks;

public partial class Game
{
    private async Task CheckCameraViewsUi()
    {
        var previous=_world; var size=GetWindow().Size;
        async Task Frames() { for(int i=0;i<4;i++) await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame); }
        try
        {
            AdoptWorld(World.NewLargeMap()); _paused=true;
            _focus=new(8,0,5); _camera.Size=18; _angle=1.2f; UpdateCamera();
            _viewName.Text="East orchard"; await UiClick(_viewSave[0]); await Frames();
            var view=_world.CameraViews[0] ?? throw new Exception("Camera Set button failed");
            if(view.Name!="East orchard") throw new Exception("View name lost");
            string saved=_world.SaveJson();
            FrameMap(); await UiClick(_viewRecall[0]); await Frames();
            if((_focus-new Vector3(8,0,5)).Length()>.01f || Math.Abs(_camera.Size-18)>.01f || Math.Abs(_angle-1.2f)>.001f)
                throw new Exception("View did not restore camera");
            if(saved!=_world.SaveJson()) throw new Exception("Recall changed simulation");
            ToggleWatch(); FrameMap(); await Press(Key.Key1); await Frames();
            if(!_watching || _focus.X!=8) throw new Exception("Watch-mode recall failed");
            ExitWatch();
            AdoptWorld(World.LoadJson(saved)); _paused=true; await Frames();
            await Press(Key.Key1); await Frames();
            if(_focus.X!=8 || _world.CameraViews[0]?.Name!="East orchard") throw new Exception("Loaded view recall failed");
            _viewName.Text="WWWWWWWWWWWWWWWWWWWWWWWW"; StoreCameraView(1);
            GetWindow().Size=new(960,640); await UiClick(_viewSave[2]); await Frames();
            if(_drawer.GetGlobalRect().End.X>400) throw new Exception("Long view name widened drawer");
            await Capture("artifacts/f12e-views-960.png");
            await UiClick(_viewClear[0]); await Frames();
            if(!_viewRecall[0].Disabled) throw new Exception("Cleared view still enabled");
            AdoptWorld(new World()); await Frames();
            if(_world.CameraViews[1]!=null || !_viewRecall[1].Disabled) throw new Exception("Views leaked across settlements");
            GD.Print("PASS: named view controls, focus/zoom/orbit recall, watch shortcut, saves, clearing, long names and world switching.");
        }
        finally { GetWindow().Size=size; AdoptWorld(previous); _paused=true; }
    }
}

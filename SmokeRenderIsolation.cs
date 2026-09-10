using Godot;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async Task CheckFrameSyncUi()
    {
        var size=GetWindow().Size; bool sync=_frameSync; string path=_atmospherePath;
        _atmospherePath="artifacts/f23d-settings.cfg"; _paused=true;
        string saved=_world.SaveJson();
        try
        {
            foreach(int width in new[]{960,1440})
            {
                GetWindow().Size=new(width,width==960?640:900); CloseDrawer(); ToggleDrawer(3);
                for(int i=0;i<5;i++) await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
                _frameSync=false; ApplyAtmosphere(); await UiClick(_frameSyncButton);
                if(!_frameSync || DisplayServer.WindowGetVsyncMode()!=DisplayServer.VSyncMode.Enabled) throw new Exception("Frame sync did not enable");
                _frameSync=false; LoadAtmosphere(); ApplyAtmosphere();
                if(!_frameSync) throw new Exception("Frame sync preference did not reload");
                await UiClick(_frameSyncButton);
                if(_frameSync || DisplayServer.WindowGetVsyncMode()!=DisplayServer.VSyncMode.Disabled) throw new Exception("Frame sync did not disable");
                _frameSync=true; LoadAtmosphere(); ApplyAtmosphere();
                if(_frameSync || _world.SaveJson()!=saved) throw new Exception("Frame sync changed village or lost off preference");
                await Capture($"artifacts/f23d-settings-{width}.png");
            }
            GD.Print("PASS: frame sync control, on/off persistence, current window setting and read-only village at 960/1440.");
        }
        finally { _atmospherePath=path;_frameSync=sync;ApplyAtmosphere();GetWindow().Size=size;CloseManagementUi(); }
    }
    private async Task ProfileRenderIsolation()
    {
        async Task Sample(string label)
        {
            for(int i=0;i<15;i++) await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
            var frames=new double[120];
            for(int i=0;i<frames.Length;i++) { ulong start=Time.GetTicksUsec(); await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame); frames[i]=(Time.GetTicksUsec()-start)/1000d; }
            Array.Sort(frames);
            GD.Print($"ISOLATION {label}: median {frames[60]:0.00}ms p95 {frames[114]:0.00}ms draws {Performance.GetMonitor(Performance.Monitor.RenderTotalDrawCallsInFrame):0} nodes {GetTree().GetNodeCount()}");
        }
        string saved=_world.SaveJson();
        DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Enabled);
        await Sample("sync-on-paused");
        await Capture("artifacts/f23d-sync-on.png");
        DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Disabled);
        await Sample("baseline-paused"); await Capture("artifacts/f23d-baseline.png");
        SetProcess(false); await Sample("no-game-process"); SetProcess(true);
        bool shadows=_sun.ShadowEnabled;
        _sun.ShadowEnabled=false; await Sample("no-sun-shadows"); _sun.ShadowEnabled=shadows;
        foreach(var p in _people) p.Body.Hide(); await Sample("no-residents"); foreach(var p in _people) p.Body.Show();
        foreach(var c in _cottages.Values) c.Body.Hide(); await Sample("no-buildings"); foreach(var c in _cottages.Values) c.Body.Show();
        _landscape.Hide(); await Sample("no-landscape"); _landscape.Show();
        SetProcess(false); _dynamic.Hide(); _landscape.Hide(); _hud.Hide();
        await Sample("empty-scene-no-process");
        _dynamic.Show(); _landscape.Show(); _hud.Show(); SetProcess(true);
        GetWindow().Size=new(960,640); await Sample("960-baseline"); GetWindow().Size=new(1440,900);
        await Sample("baseline-restored");
        if(_world.SaveJson()!=saved) throw new Exception("Paused isolation changed simulation");
        // Compare actual running villages from the same start. Equal rendered-frame counts
        // cover different elapsed village times; draw counts can change as meals progress.
        foreach(bool sync in new[]{true,false})
        {
            AdoptWorld(Inlanders.Simulation.World.LoadJson(saved)); CloseManagementUi();
            _focus=OnGround(1,2);_camera.Size=26;UpdateCamera();
            DisplayServer.WindowSetVsyncMode(sync?DisplayServer.VSyncMode.Enabled:DisplayServer.VSyncMode.Disabled);
            _paused=false; await Sample(sync?"sync-on-live":"sync-off-live"); _paused=true;
        }
        AdoptWorld(Inlanders.Simulation.World.LoadJson(saved)); CloseManagementUi();_paused=true;
        _focus=OnGround(1,2);_camera.Size=26;UpdateCamera();
        DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Disabled);
    }
}

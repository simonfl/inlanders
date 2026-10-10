using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
public partial class Game
{
    private async void RunStaticBatchSmoke()
    {
        try
        {
            string path=OS.GetCmdlineUserArgs().First(a=>a.StartsWith("--batch-snapshot="))[17..];string json=System.IO.File.ReadAllText(path);
            GetWindow().Size=new(1440,900);DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Disabled);
            foreach(bool colored in new[]{false,true,false})
            {
                _coloredStaticBatches=colored;AdoptWorld(World.LoadJson(json));_paused=true;string baseline=_world.SaveJson();CloseManagementUi();_angle=.72f+Mathf.Pi;_camera.Size=29;UpdateCamera();
                for(int i=0;i<30;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
                double[] frames=new double[240];for(int i=0;i<frames.Length;i++){ulong start=Time.GetTicksUsec();await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);frames[i]=(Time.GetTicksUsec()-start)/1000d;}
                Array.Sort(frames);GD.Print($"BATCH colored={colored}: median={frames[120]:F3} p95={frames[228]:F3} max={frames[^1]:F3} draws={Performance.GetMonitor(Performance.Monitor.RenderTotalDrawCallsInFrame)} nodes={GetTree().GetNodeCount()}");
                await Capture($"artifacts/193-{(_world.PublicPlace!=null?"river":"dense")}-batch-{colored}.png");if(_world.SaveJson()!=baseline)throw new Exception("Batch comparison changed world");
            }
            GD.Print("PASS matched static batch render comparison; no simulation mutation.");GetTree().Quit();
        }
        catch(Exception e){GD.PrintErr(e);GetTree().Quit(1);}
    }
}

using Godot;
using Inlanders.Simulation;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;

public partial class Game
{
    private async Task CheckFrameStalls()
    {
        string folder="artifacts/frame-stalls";Directory.CreateDirectory(folder);
        const string snapshot="artifacts/large-village/dense.json";
        if(!File.Exists(snapshot))throw new Exception("Generate the fixture first: Test.ps1 -FinaleCampaign, then Play.ps1 -LargeVillageSmokeTest.");
        string saved=File.ReadAllText(snapshot);
        _frameTraces.Capacity=12000;
        bool quick=OS.GetCmdlineUserArgs().Contains("--cold-stall-probe");int seconds=quick?10:30;
        var results=new List<object>();bool sync=_frameSync;int cap=Engine.MaxFps;
        _frameSync=false;Engine.MaxFps=0;_goldenHour=false;ApplyAtmosphere();GetWindow().Size=new(1440,900);
        async Task Wait(float seconds)=>await ToSignal(GetTree().CreateTimer(seconds),SceneTreeTimer.SignalName.Timeout);
        var options=new JsonSerializerOptions{WriteIndented=true,IncludeFields=true};
        object Stats(double[] values){Array.Sort(values);return new{median=values[values.Length/2],p95=values[(int)(values.Length*.95)],p99=values[(int)(values.Length*.99)],max=values[^1]};}
        try
        {
            foreach(var run in (quick?new[]{(name:"cold-1x",speed:1,warm:0)}:new[]{(name:"cold-1x",speed:1,warm:0),(name:"cold-4x",speed:4,warm:0),(name:"warm-4x",speed:4,warm:20)}))
            {
                _traceFrames=false;AdoptWorld(World.LoadJson(saved));_paused=true;CloseManagementUi();_noticeUntil=0;
                _showWorldLabels=false;ApplyWorldLabels();_focus=new(3,0,3);_camera.Size=32;_angle=.72f;UpdateCamera();_accumulator=0;_speed=run.speed;
                for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
                _paused=false;if(run.warm>0)await Wait(run.warm);
                // Begin recording at a process boundary. The following frame's delta
                // belongs to the preceding frame's CPU/render work, not its own work.
                string startState=_world.SaveJson();
                _frameTraces.Clear();_traceFrames=true;await Wait(seconds);_traceFrames=false;_paused=true;
                var frames=_frameTraces.Where(f=>f.WallMs>0).ToArray();
                if(frames.Length<300)throw new Exception("Too few normal-process samples");
                var control=World.LoadJson(startState);for(int i=0;i<_frameTraces.Sum(f=>f.Ticks);i++)control.Tick(.1f);
                if(control.SaveJson()!=_world.SaveJson())throw new Exception("Normal frame tracing changed simulation continuation");
                _world.Validate();string final=_world.SaveJson();if(World.LoadJson(final).SaveJson()!=final)throw new Exception("Live trace save differs");
                File.WriteAllText(folder+"/"+run.name+(quick?"-probe-frames.json":"-frames.json"),JsonSerializer.Serialize(frames,options));
                var report=new{run.name,run.speed,warmSeconds=run.warm,recordSeconds=seconds,samples=frames.Length,
                    renderer=RenderingServer.GetCurrentRenderingMethod(),adapter=RenderingServer.GetVideoAdapterName(),vsync=DisplayServer.WindowGetVsyncMode().ToString(),cap=Engine.MaxFps,
                    population=_world.Population,window="1440x900",cameraSize=32,
                    frameMs=Stats(frames.Select(f=>f.WallMs).ToArray()),processMs=Stats(frames.Select(f=>f.ProcessMs).ToArray()),
                    stallsOver50=frames.Count(f=>f.WallMs>50),stallsOver100=frames.Count(f=>f.WallMs>100),
                    bytesPerFrame=Stats(frames.Select(f=>(double)f.AllocatedBytes).ToArray()),gcFrames=frames.Count(f=>f.Gen0+f.Gen1+f.Gen2>0),
                    slowest=frames.OrderByDescending(f=>f.WallMs).Take(12).ToArray(),slowestProcess=frames.OrderByDescending(f=>f.ProcessMs).Take(6).ToArray()};
                results.Add(report);File.WriteAllText(folder+(quick?"/cold-probe.json":"/results.json"),JsonSerializer.Serialize(results,options));
                GD.Print($"FRAME STALLS {run.name}: {frames.Length} frames; over100ms={frames.Count(f=>f.WallMs>100)}; max={frames.Max(f=>f.WallMs):F1}ms; process max={frames.Max(f=>f.ProcessMs):F1}ms.");
            }
        }
        finally{_traceFrames=false;_paused=true;_frameSync=sync;Engine.MaxFps=cap;ApplyAtmosphere();}
        GD.Print($"PASS: normal-process {(quick?"cold probe":"cold/warm traces")}, work/mesh/GC correlation, exact tick replay and current saves.");
    }
}

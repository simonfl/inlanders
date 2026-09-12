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
    private async Task CheckComposition()
    {
        const string folder="artifacts/composition";Directory.CreateDirectory(folder);
        async Task Frame()=>await ToSignal(RenderingServer.Singleton,RenderingServer.SignalName.FramePostDraw);
        async Task Settle(){for(int i=0;i<8;i++)await Frame();}
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        int Triangles(Node n)=>(n is MeshInstance3D m?m.Mesh.GetFaces().Length/3:n is MultiMeshInstance3D mm?mm.Multimesh.Mesh.GetFaces().Length/3*mm.Multimesh.InstanceCount:0)+n.GetChildren().Sum(Triangles);
        int Meshes(Node n)=>(n is MeshInstance3D || n is MultiMeshInstance3D?1:0)+n.GetChildren().Sum(Meshes);
        var reports=new List<object>();bool sync=_frameSync,motion=_foliageMotion;int cap=Engine.MaxFps;
        _frameSync=false;Engine.MaxFps=0;_foliageMotion=false;_goldenHour=false;ApplyAtmosphere();GetWindow().Size=new(1440,900);
        _frameTraces.Capacity=12000;
        try
        {
            foreach(var scene in new[]{(name:"ordinary",path:"artifacts/large-village/ordinary.json"),(name:"dense",path:"artifacts/large-village/dense.json"),(name:"lake",path:"artifacts/f11b2-complete.json")})
            {
                if(!File.Exists(scene.path))throw new Exception("Missing composition fixture: "+scene.path);
                string saved=File.ReadAllText(scene.path);int originalDecor=-1,originalLand=-1;
                foreach(bool natural in new[]{false,true})
                {
                    string version=natural?"after":"before";_naturalShore=natural;
                    AdoptWorld(World.LoadJson(saved));_paused=true;_speed=1;_accumulator=0;CloseManagementUi();_noticeUntil=0;
                    _showWorldLabels=false;ApplyWorldLabels();_focus=scene.name=="lake"?new(4,0,1):new(3,0,3);_camera.Size=32;
                    string initial=_world.SaveJson();
                    for(int turn=0;turn<2;turn++)
                    {
                        _angle=.72f+turn*Mathf.Pi/2;UpdateCamera();await Settle();
                        await Capture($"{folder}/{scene.name}-{version}-{turn}-hud.png");
                        ToggleWatch();ToggleCleanWatch();await Settle();
                        await Capture($"{folder}/{scene.name}-{version}-{turn}-clean.png");ExitWatch();
                    }
                    Check(_world.SaveJson()==initial,"Composition changed paused world");
                    int landscape=Triangles(_landscape),decor=Triangles(_decorationView);
                    if(!natural){originalDecor=decor;originalLand=landscape;}
                    else {Check(decor==originalDecor,"Composition altered decoration batching");Check(landscape<=originalLand,"Shore treatment added terrain geometry");}
                    _angle=.72f;UpdateCamera();await Settle();
                    // Normal _Process, raw frame intervals, and exact fixed-tick replay.
                    _frameTraces.Clear();_traceFrames=true;_paused=false;
                    for(int i=0;i<180;i++)await Frame();
                    _traceFrames=false;_paused=true;
                    var control=World.LoadJson(initial);for(int i=0;i<_frameTraces.Sum(f=>f.Ticks);i++)control.Tick(.1f);
                    Check(control.SaveJson()==_world.SaveJson(),"Composition changed simulation continuation");
                    _world.Validate();Check(World.LoadJson(_world.SaveJson()).SaveJson()==_world.SaveJson(),"Composition save failed");
                    var times=_frameTraces.Where(f=>f.WallMs>0).Select(f=>f.WallMs).OrderBy(x=>x).ToArray();
                    reports.Add(new{scene=scene.name,version,population=_world.Population,landscapeTriangles=landscape,landscapeMeshes=Meshes(_landscape),decorationTriangles=decor,
                        draws=Performance.GetMonitor(Performance.Monitor.RenderTotalDrawCallsInFrame),samples=times.Length,medianMs=times[times.Length/2],p95Ms=times[(int)(times.Length*.95)],maxMs=times[^1],
                        ticks=_frameTraces.Sum(f=>f.Ticks),resolution="1440x900",zoom=32,renderer=RenderingServer.GetCurrentRenderingMethod(),adapter=RenderingServer.GetVideoAdapterName(),vsync=DisplayServer.WindowGetVsyncMode().ToString()});
                }
            }
            File.WriteAllText(folder+"/results.json",JsonSerializer.Serialize(reports,new JsonSerializerOptions{WriteIndented=true}));
        }
        finally{_traceFrames=false;_naturalShore=true;_paused=true;_frameSync=sync;_foliageMotion=motion;Engine.MaxFps=cap;ExitWatch();ApplyAtmosphere();}
        GD.Print("PASS: three populated composition comparisons, two rotations, HUD/clean views, bounded geometry, normal-process timing, exact tick replay and saves.");
    }
}

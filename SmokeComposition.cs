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
        bool ground=OS.GetCmdlineUserArgs().Contains("--ground-color");
        string folder=ground?"artifacts/ground-color":"artifacts/composition";Directory.CreateDirectory(folder);
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
            var scenes=new List<(string name,string path)>{("ordinary","artifacts/large-village/ordinary.json"),("dense","artifacts/large-village/dense.json"),("lake","artifacts/f11b2-complete.json")};
            if(ground)
            {
                var raised=World.NewLargeMap();foreach(var cell in raised.Map.Land.Where(c=>c.Z==3 && c.X<5))raised.SetPath(cell,true);
                File.WriteAllText(folder+"/raised.json",raised.SaveJson());File.WriteAllText(folder+"/original.json",World.NewScenario().SaveJson());
                scenes.Add(("raised",folder+"/raised.json"));scenes.Add(("original",folder+"/original.json"));
            }
            foreach(var scene in scenes)
            {
                if(!File.Exists(scene.path))throw new Exception("Missing composition fixture: "+scene.path);
                string saved=File.ReadAllText(scene.path);int originalDecor=-1,originalLand=-1;
                foreach(bool natural in new[]{false,true})
                {
                    string version=natural?"after":"before";_naturalShore=ground || natural;_smoothGround=!ground || natural;
                    AdoptWorld(World.LoadJson(saved));_paused=true;_speed=1;_accumulator=0;CloseManagementUi();_noticeUntil=0;
                    _showWorldLabels=false;ApplyWorldLabels();_focus=scene.name=="lake"?new(4,0,1):scene.name=="raised"?new(0,0,0):new(3,0,3);_camera.Size=scene.name=="raised"?42:32;
                    string initial=_world.SaveJson();
                    foreach(int width in ground?new[]{960,1440}:new[]{1440})
                    for(int turn=0;turn<2;turn++)
                    {
                        GetWindow().Size=new(width,width==960?640:900);
                        _angle=.72f+turn*Mathf.Pi/2;UpdateCamera();await Settle();
                        string label=$"{scene.name}-{version}-{turn}"+(ground?$"-{width}":"");
                        await Capture($"{folder}/{label}-hud.png");
                        ToggleWatch();ToggleCleanWatch();await Settle();
                        await Capture($"{folder}/{label}-clean.png");ExitWatch();
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
                        ticks=_frameTraces.Sum(f=>f.Ticks),resolution="1440x900",zoom=_camera.Size,renderer=RenderingServer.GetCurrentRenderingMethod(),adapter=RenderingServer.GetVideoAdapterName(),vsync=DisplayServer.WindowGetVsyncMode().ToString()});
                    if(ground && natural && (scene.name=="lake" || scene.name=="raised" || scene.name=="original"))await CheckGroundColorPreview(folder,scene.name);
                }
            }
            File.WriteAllText(folder+"/results.json",JsonSerializer.Serialize(reports,new JsonSerializerOptions{WriteIndented=true}));
        }
        finally{_traceFrames=false;_naturalShore=true;_smoothGround=true;_paused=true;_frameSync=sync;_foliageMotion=motion;Engine.MaxFps=cap;ExitWatch();ApplyAtmosphere();}
        GD.Print($"PASS: {(ground?"five ground-color scenes and placement checks":"three populated composition comparisons")}, two rotations, HUD/clean views, bounded geometry, normal-process timing, exact tick replay and saves.");
    }
}

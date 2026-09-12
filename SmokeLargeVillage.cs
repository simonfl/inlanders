using Godot;
using Inlanders.Simulation;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Stopwatch=System.Diagnostics.Stopwatch;

public partial class Game
{
    private async Task ProfileLargeVillage()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frame()=>await ToSignal(RenderingServer.Singleton,RenderingServer.SignalName.FramePostDraw);
        string folder="artifacts/large-village";Directory.CreateDirectory(folder);
        var ordinary=World.LoadJson(File.ReadAllText("artifacts/finale-campaign-local-normal.json"));
        var dense=World.LoadJson(File.ReadAllText("artifacts/finale-campaign-local-extra.json"));
        Check(!ordinary.Creative && !dense.Creative,"Profile requires normal simulation");
        foreach(var p in dense.People.Where(p=>p.Role==Role.Unassigned).Take(3))dense.Assign(p.Id,Role.Builder);
        while(dense.Cottages.Sum(c=>Buildings.Get(c.Kind).Beds)<32)
        {
            var cell=dense.Map.Land.Where(c=>dense.PlacementProblem(c,0,BuildingKind.Cottage)==null)
                .OrderBy(c=>(c.Point-new Cell(8,4).Point).LengthSquared()).First();
            Check(dense.Place(cell,0,BuildingKind.Cottage)!=null,"Profile housing refused");
        }
        for(int i=0;i<10000 && dense.Beds<32;i++)dense.Tick(.1f);
        Check(dense.Beds>=32,"Profile housing stalled");
        while(dense.Population<32)Check(dense.InviteNewcomers(),"Profile invitations refused: "+dense.InvitationProblem());
        for(int n=0;n<2;n++)
        {
            var cell=dense.Map.Land.Where(c=>dense.PlacementProblem(c,0,BuildingKind.VegetableGarden)==null)
                .OrderBy(c=>(c.Point-new Cell(8,4).Point).LengthSquared()).First();
            Check(dense.Place(cell,0,BuildingKind.VegetableGarden)!=null,"Profile garden refused");
        }
        dense.Assign(24,Role.Farmer);dense.Assign(25,Role.Farmer);dense.Assign(26,Role.Hauler);dense.Assign(27,Role.Hauler);
        for(int i=0;i<1200;i++)dense.Tick(.1f);
        foreach(var c in dense.People.SelectMany(p=>p.Route).Distinct().ToArray())dense.SetPath(c,true);
        foreach(var c in dense.Map.Land.OrderBy(c=>dense.Cottages.Min(b=>(b.Cell.Point-c.Point).LengthSquared())).ThenBy(c=>c.Z).ThenBy(c=>c.X))
        {
            if(dense.Decorations.Count>=120)break;
            if(dense.Paths.Contains(c))continue;
            dense.PlaceDecoration(c,(DecorationKind)(dense.Decorations.Count%4));
        }
        Check(dense.Decorations.Count>=80,"Dense fixture lacks decorations");dense.Validate();
        File.WriteAllText(folder+"/ordinary.json",ordinary.SaveJson());File.WriteAllText(folder+"/dense.json",dense.SaveJson());
        var reports=new List<object>();bool previousSync=_frameSync;int oldCap=Engine.MaxFps;
        SetProcess(false);Engine.MaxFps=0;_frameSync=false;_goldenHour=false;ApplyAtmosphere();
        DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Disabled);GetWindow().Size=new(1440,900);
        object Distribution(List<double> values)
        {
            var sorted=values.OrderBy(v=>v).ToArray();return new{median=sorted[sorted.Length/2],p95=sorted[(int)(sorted.Length*.95)],max=sorted[^1]};
        }
        try
        {
            await CheckDecorationClusterRendering(dense);
            foreach(var fixture in new[]{(name:"ordinary",world:ordinary),(name:"dense",world:dense)})
            {
                string saved=fixture.world.SaveJson();
                foreach(string mode in (OS.GetCmdlineUserArgs().Contains("--decoration-isolation") ? new[]{"render-floor","render-no-decor"} : new[]{"render-floor","live-1x","live-4x","orbit-1x","economy-1x"}))
                {
                    var watch=Stopwatch.StartNew();AdoptWorld(World.LoadJson(saved));double adoption=watch.Elapsed.TotalMilliseconds;
                    _paused=mode.StartsWith("render-");_speed=mode=="live-4x"?4:1;_accumulator=0;_focus=new(3,0,3);_angle=.72f;_camera.Size=32;UpdateCamera();
                    CloseManagementUi();_noticeUntil=0;_showWorldLabels=false;ApplyWorldLabels();

                    RenderActors(0);RenderFoodViews();UpdateHud();_decorationView.Visible=mode!="render-no-decor";await Frame();double firstFrame=watch.Elapsed.TotalMilliseconds;
                    for(int i=0;i<30;i++)await Frame();
                    double menuOpenCpuMs=0,menuOpenFirstFrameMs=0;
                    if(mode=="economy-1x"){watch.Restart();ToggleDrawer(4);UpdateHud();menuOpenCpuMs=watch.Elapsed.TotalMilliseconds;await Frame();menuOpenFirstFrameMs=watch.Elapsed.TotalMilliseconds;}
                    var frames=new List<double>();var simulation=new List<double>();var scene=new List<double>();var ui=new List<double>();var audioTimes=new List<double>();
                    var control=World.LoadJson(saved);int ticks=0,cargoFrames=0,movingFrames=0;ulong previous=Time.GetTicksUsec();
                    for(int i=0;i<180;i++)
                    {
                        float dt=(float)Math.Min(.1,(Time.GetTicksUsec()-previous)/1000000d);previous=Time.GetTicksUsec();watch.Restart();
                        if(!_paused){_accumulator+=dt*_speed;_clock+=dt*_speed;while(_accumulator>=.1f){_world.Tick(.1f);_accumulator-=.1f;ticks++;}}
                        simulation.Add(watch.Elapsed.TotalMilliseconds);watch.Restart();
                        if(!mode.StartsWith("render-"))
                        {
                            if(mode=="orbit-1x"){_angle+=dt*.3f;UpdateCamera();}
                            RenderActors(dt);UpdateAtmosphere();RenderFoodViews();
                        }
                        scene.Add(watch.Elapsed.TotalMilliseconds);watch.Restart();
                        if(!mode.StartsWith("render-"))UpdateHud();
                        ui.Add(watch.Elapsed.TotalMilliseconds);watch.Restart();
                        if(!mode.StartsWith("render-"))UpdateAudio(dt);
                        audioTimes.Add(watch.Elapsed.TotalMilliseconds);
                        cargoFrames+=_world.People.Count(p=>p.Carried>0);movingFrames+=_world.People.Count(p=>p.Route.Count>0);
                        await Frame();frames.Add((Time.GetTicksUsec()-previous)/1000d);
                    }
                    // Replay the exact tick count outside the measured render frames.
                    for(int i=0;i<ticks;i++)control.Tick(.1f);
                    Check(control.SaveJson()==_world.SaveJson(),"Profile rendering changed simulation continuation");
                    Check(_world.SaveJson()==World.LoadJson(_world.SaveJson()).SaveJson(),"Profile save differs");
                    if(mode=="live-1x")await Capture(folder+"/"+fixture.name+".png");
                    var report=new{fixture=fixture.name,mode,population=_world.Population,buildings=_world.Cottages.Count,paths=_world.Paths.Count,decorations=_world.Decorations.Count,
                        mapWidth=_world.Map.Width,mapDepth=_world.Map.Depth,resolution="1440x900",zoom=32,speed=_speed,vsync=DisplayServer.WindowGetVsyncMode().ToString(),maxFps=Engine.MaxFps,
                        renderer=RenderingServer.GetCurrentRenderingMethod(),adapter=RenderingServer.GetVideoAdapterName(),samples=frames.Count,ticks,cargoFrames,movingFrames,
                        adoptionCpuMs=adoption,adoptionThroughFirstFrameMs=firstFrame,menuOpenCpuMs,menuOpenFirstFrameMs,frameMs=Distribution(frames),simulationCpuMs=Distribution(simulation),sceneCpuMs=Distribution(scene),uiCpuMs=Distribution(ui),audioCpuMs=Distribution(audioTimes),
                        draws=Performance.GetMonitor(Performance.Monitor.RenderTotalDrawCallsInFrame),objects=Performance.GetMonitor(Performance.Monitor.RenderTotalObjectsInFrame),nodes=GetTree().GetNodeCount()};
                    reports.Add(report);GD.Print(System.Text.Json.JsonSerializer.Serialize(report));
                    File.WriteAllText(folder+(OS.GetCmdlineUserArgs().Contains("--decoration-isolation")?"/isolation.json":"/results.json"),System.Text.Json.JsonSerializer.Serialize(reports,new System.Text.Json.JsonSerializerOptions{WriteIndented=true}));
                }
            }
        }
        finally{_decorationView.Visible=true;_paused=true;_frameSync=previousSync;Engine.MaxFps=oldCap;ApplyAtmosphere();SetProcess(true);}
        GD.Print("PASS: representative normal/dense profiles, actual work/cargo, frame distributions, isolated CPU phases and exact simulation continuation.");
    }
}

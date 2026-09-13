using Godot;
using Inlanders.Simulation;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;

public partial class Game
{
    private JsonDocument? _reviewRequest;
    private string _reviewDirectory="";
    private int _reviewCapture;
    private bool _reviewCapturing;
    private readonly System.Diagnostics.Stopwatch _reviewTimer=System.Diagnostics.Stopwatch.StartNew();
    private static string ReviewHash(string path)=>Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path)));

    private void ConfigureReviewSession()
    {
        var args=OS.GetCmdlineUserArgs();int index=Array.IndexOf(args,"--review-run");
        if(index<0)return;
        if(index+1>=args.Length)throw new ArgumentException("--review-run needs a request file from Review.ps1");
        _reviewRequest=JsonDocument.Parse(File.ReadAllText(args[index+1]));
        var request=_reviewRequest.RootElement;
        _storybookScene=request.TryGetProperty("storybook",out var storybook) && storybook.GetBoolean();
        // Godot loads the managed assembly from bytes, so Assembly.Location can be empty.
        if(request.GetProperty("assemblyHash").GetString()!=ReviewHash(ProjectSettings.GlobalizePath("res://.godot/mono/temp/bin/Debug/Inlanders.dll")))throw new Exception("Review build hash differs from request");
        if(request.GetProperty("fixture").GetProperty("fixtureHash").GetString()!=ReviewHash(request.GetProperty("fixturePath").GetString()!))throw new Exception("Review fixture hash differs from request");
        _reviewDirectory=request.GetProperty("runDirectory").GetString()!;
        Directory.CreateDirectory(_reviewDirectory);
        string saves=Path.Combine(_reviewDirectory,"session");Directory.CreateDirectory(saves);
        _savePath=Path.Combine(saves,"settlement.json");_campaignPath=Path.Combine(saves,"campaign.json");
        _neighborhoodPath=Path.Combine(saves,"neighborhood.json");
        _continuePath=Path.Combine(saves,"continue.json");_largeSavePath=Path.Combine(saves,"three-clearings.json");
        _creativeSavePath=Path.Combine(saves,"creative.json");_creativeLargeSavePath=Path.Combine(saves,"creative-three-clearings.json");
        _atmospherePath=Path.Combine(saves,"atmosphere.cfg");_audioSettingsPath=Path.Combine(saves,"audio.cfg");
    }

    private async void StartReviewSession()
    {
        try
        {
            var request=_reviewRequest!.RootElement;
            int speed=request.GetProperty("speed").GetInt32();if(speed is not (1 or 3 or 6))throw new Exception("Invalid review speed");
            GetWindow().Size=new(request.GetProperty("width").GetInt32(),request.GetProperty("height").GetInt32());
            EnterFromMenu(World.LoadFile(request.GetProperty("fixturePath").GetString()!));
            _speed=speed;_paused=true;_accumulator=0;CloseManagementUi();_noticeUntil=0;
            _focus=new(request.GetProperty("focusX").GetSingle(),0,request.GetProperty("focusZ").GetSingle());
            _angle=.72f+request.GetProperty("turn").GetInt32()*Mathf.Pi/2;
            if(request.TryGetProperty("angle",out var angle))_angle=angle.GetSingle();
            _camera.Size=request.GetProperty("zoom").GetSingle();UpdateCamera();
            if(request.TryGetProperty("view",out var view))
            {
                _goldenHour=view.GetProperty("goldenHour").GetBoolean();_foliageMotion=view.GetProperty("foliage").GetBoolean();
                _showWorldLabels=view.GetProperty("labels").GetBoolean();_frameSync=view.GetProperty("vsync").GetString()!="Disabled";
                Engine.MaxFps=view.GetProperty("maxFps").GetInt32();ApplyAtmosphere();ApplyWorldLabels();
                var audio=request.GetProperty("audio");_effectsVolume=audio.GetProperty("effects").GetInt32();_musicVolume=audio.GetProperty("music").GetInt32();_ambienceVolume=audio.GetProperty("nature").GetInt32();
                _soundMuted=audio.GetProperty("muted").GetBoolean();_musicMuted=audio.GetProperty("musicMuted").GetBoolean();ApplyAudioSettings();
                var selected=request.GetProperty("selected");int site=selected.GetProperty("siteId").GetInt32(),person=selected.GetProperty("personId").GetInt32();
                if(site>=0)SelectBuilding(site);
                else if(person>=0)SelectPerson(person);
            }
            GetWindow().Title=$"Inlanders review — {request.GetProperty("scenario").GetString()} — F8 capture";
            // Capture from the same normal process/UI used for interactive inspection.
            if(request.GetProperty("scenario").GetString()!.StartsWith("neighborhood",StringComparison.Ordinal)) {
                ToggleDrawer(2);UpdateCampaignUi();
                if(!_objective.IsVisibleInTree() || !_neighborhoodGoals.IsVisibleInTree() || string.IsNullOrWhiteSpace(_objective.Text))throw new Exception("Neighborhood goals are missing");
            }
            await CaptureReviewBundle();
            if(request.TryGetProperty("observeSeconds",out var observe) && observe.GetSingle()>0)
            {
                float start=_world.Food.Time;double wallStart=_reviewTimer.Elapsed.TotalSeconds;
                _frameTraces.Clear();_traceFrames=true;
                CloseManagementUi();_paused=false;
                while(_world.Food.Time-start<observe.GetSingle())
                {
                    if(_reviewTimer.Elapsed.TotalSeconds-wallStart>180)throw new TimeoutException("Observation exceeded 180 wall seconds; inspect performance before retrying.");
                    await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
                }
                _paused=true;_traceFrames=false;_world.Validate();
                File.WriteAllText(Path.Combine(_reviewDirectory,"frames.json"),JsonSerializer.Serialize(_frameTraces,new JsonSerializerOptions{IncludeFields=true}));
                File.WriteAllText(Path.Combine(_reviewDirectory,"observation.json"),JsonSerializer.Serialize(new{
                    startSimulationSeconds=start,endSimulationSeconds=_world.Food.Time,speed=_speed,
                    wallSeconds=_reviewTimer.Elapsed.TotalSeconds-wallStart,
                    mode=request.TryGetProperty("movieFps",out var movieFps) && movieFps.GetInt32()>0?
                        $"Engine movie at {movieFps.GetInt32()} fixed frames/second; recording/encoding time is not native frame performance":
                        "Normal process advancement with before/after snapshots; not a recording or human observation",
                    tasks=_world.People.GroupBy(p=>p.Task.ToString()).ToDictionary(g=>g.Key,g=>g.Count())},new JsonSerializerOptions{WriteIndented=true}));
                await CaptureReviewBundle();
            }
            if(request.TryGetProperty("probeControls",out var probe) && probe.GetBoolean())await ProbeReviewControls();
            if(request.GetProperty("captureOnly").GetBoolean())GetTree().Quit();
        }
        catch(Exception e) { GD.PushError(e.ToString());GetTree().Quit(1); }
    }

    private async Task ProbeReviewControls()
    {
        void Check(bool value,string why){if(!value)throw new Exception(why);}
        Check(_paused && !_atMainMenu,"Review did not enter paused ordinary play");
        await ProbeSceneStudy();
        float initialSpeed=_speed,initialTime=_world.Food.Time;
        for(int i=0;i<3;i++){await UiClick(_speedButton);Check(_speed is 1 or 3 or 6,"Non-player speed selected");}
        Check(_speed==initialSpeed,"Speed cycle differs from normal controls");
        await Press(Key.B);Check(_drawer.Visible,"Construction browsing inaccessible");await Press(Key.Escape);
        await Press(Key.Space);Check(!_paused,"Normal pause control failed");
        ulong deadline=Time.GetTicksMsec()+5000;
        while(_world.Food.Time<=initialTime && Time.GetTicksMsec()<deadline)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
        await Press(Key.Space);Check(_paused && _world.Food.Time>initialTime,"Normal process did not advance simulation");
        SelectPerson(0);int previous=_reviewCapture;await Press(Key.F8);
        while(_reviewCapturing)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
        Check(_reviewCapture==previous+1 && File.Exists(Path.Combine(_reviewDirectory,$"capture-{_reviewCapture:0000}","manifest.json")),"F8 did not finish a bundle");
        _world.Validate();
        if(_world.Neighborhood!=null && _reviewRequest!.RootElement.GetProperty("scenario").GetString() is "neighborhood" or "neighborhood-landscape" or "neighborhood-workplace-food" or "neighborhood-journey")await ProbeNeighborhoodFlow();
        if(_reviewRequest!.RootElement.GetProperty("scenario").GetString()=="neighborhood-journey")await ProbeNeighborhoodJourney();
        if(_world.SharedWork)await ProbeSharedStaffing();
        File.WriteAllText(Path.Combine(_reviewDirectory,"checks.json"),JsonSerializer.Serialize(new{passed=true,scriptedUi=true,initialTime,finalTime=_world.Food.Time,speed=_speed,selectedPerson=_selectedPerson,checks=new[]{"paused startup","normal speed cycle","catalog","normal process ticks","F8 bundle"}},new JsonSerializerOptions{WriteIndented=true}));
        GD.Print("PASS: review normal controls, process advancement, selection and F8 capture (scripted UI probe)");
    }

    private async Task CaptureReviewBundle()
    {
        if(_reviewCapturing || _reviewRequest==null)return;
        _reviewCapturing=true;bool wasPaused=_paused;_paused=true;
        try
        {
            string directory=Path.Combine(_reviewDirectory,$"capture-{++_reviewCapture:0000}");Directory.CreateDirectory(directory);
            _world.Validate();string saved=_world.SaveJson();
            RenderActors(0);RenderFoodViews();UpdateHud();
            // Let deferred layout and GPU draws settle without advancing the world.
            for(int i=0;i<3;i++)await ToSignal(RenderingServer.Singleton,RenderingServer.SignalName.FramePostDraw);
            await Capture(Path.Combine(directory,"view.png"));
            if(_world.SaveJson()!=saved)throw new Exception("World changed during paused review capture");
            File.WriteAllText(Path.Combine(directory,"world.json"),saved);
            var person=_world.People.FirstOrDefault(p=>p.Id==_selectedPerson);
            var site=_world.Cottages.FirstOrDefault(c=>c.Id==_selectedSite);
            var record=new
            {
                request=_reviewRequest.RootElement,
                capturedUtc=DateTime.UtcNow.ToString("o"),processWallSeconds=_reviewTimer.Elapsed.TotalSeconds,
                simulationSeconds=_world.Food.Time,pausedBeforeCapture=wasPaused,pausedDuringCapture=true,speed=_speed,
                executionMode="normal Godot process; snapshot temporarily pauses simulation",
                worldHash=ReviewHash(Path.Combine(directory,"world.json")),imageHash=ReviewHash(Path.Combine(directory,"view.png")),
                camera=new{focusX=_focus.X,focusY=_focus.Y,focusZ=_focus.Z,angle=_angle,zoom=_camera.Size},
                window=new{width=GetWindow().Size.X,height=GetWindow().Size.Y},
                selected=new{personId=_selectedPerson,siteId=_selectedSite,personStatus=person?.Status,role=person?.Role.ToString(),task=person?.Task.ToString(),siteKind=site?.Kind.ToString()},
                village=new{population=_world.Population,buildings=_world.Cottages.Count,map=_world.Map.Name,objective=_world.CampaignObjective},
                rendering=new{renderer=RenderingServer.GetCurrentRenderingMethod(),adapter=RenderingServer.GetVideoAdapterName(),vsync=DisplayServer.WindowGetVsyncMode().ToString(),maxFps=Engine.MaxFps,goldenHour=_goldenHour,foliage=_foliageMotion,labels=_showWorldLabels,storybook=_storybookScene},
                audio=new{effects=_effectsVolume,music=_musicVolume,nature=_ambienceVolume,muted=_soundMuted,musicMuted=_musicMuted}
            };
            File.WriteAllText(Path.Combine(directory,"manifest.json"),JsonSerializer.Serialize(record,new JsonSerializerOptions{WriteIndented=true}));
            GD.Print($"REVIEW CAPTURE: {directory}");
        }
        catch(Exception e) { GD.PushError("Review capture failed: "+e);if(_reviewRequest.RootElement.GetProperty("captureOnly").GetBoolean())throw; }
        finally{_paused=wasPaused;_reviewCapturing=false;}
    }
}

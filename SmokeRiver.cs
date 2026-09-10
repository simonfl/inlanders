using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async void RunRiverSmoke()
    {
        _campaignPath=System.IO.Path.Combine(System.IO.Path.GetTempPath(),"inlanders-river-"+Guid.NewGuid()+".json");
        try { await CheckRiverUi(); if(OS.GetCmdlineUserArgs().Contains("--render-profile")) await ProfileVillageRendering(); GetTree().Quit(); }
        catch(Exception e) { GD.PrintErr("RIVER SMOKE FAIL: "+e); GetTree().Quit(1); }
        finally { foreach(string suffix in new[]{"",".bak",".tmp"}) if(System.IO.File.Exists(_campaignPath+suffix)) System.IO.File.Delete(_campaignPath+suffix); }
    }
    private async Task CheckRiverUi()
    {
        async Task Frames() { for(int i=0;i<4;i++) await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame); }
        async Task Until(Func<bool> ready,string step)
        {
            for(int i=0;i<24000 && !ready();i++) { _world.Tick(.1f); if(i%100==0) await Frames(); }
            _world.Validate(); await Frames(); if(!ready()) throw new Exception("Rendered river stalled: "+step+"\n"+_world.RiverObjective);
        }
        void Build(Cell c,BuildingKind kind,bool rotated=false) { if(_world.Place(c,rotated,kind)==null) throw new Exception("River placement rejected: "+c+" "+kind); }
        SwitchCampaign(6,false); _paused=true; await Frames();
        foreach(int width in new[]{1440,960})
        {
            GetWindow().Size=new(width,width==960?640:900); FrameMap(); await OpenMenu(2); _drawerPages[2].ScrollVertical=0; await Frames();
            if(!_riverAction.Visible || !_riverAction.Disabled || !_objective.Text.Contains("East-bank beds")) throw new Exception("River opening guidance missing");
            await Capture($"artifacts/f11b-river-opening-{width}.png");
        }
        Build(new(5,2),BuildingKind.Bridge,true); Build(new(-1,6),BuildingKind.VegetableGarden);
        _world.Assign(6,Role.Farmer); _world.Assign(7,Role.Logger);
        await Until(()=>_world.HasBuilding(BuildingKind.Bridge),"crossing");
        Build(new(8,0),BuildingKind.Cottage); Build(new(8,4),BuildingKind.Cottage); Build(new(8,7),BuildingKind.VegetableGarden);
        await Until(()=>_world.Beds>=12 && _world.InvitationProblem()==null,"first homes");
        _world.InviteNewcomers(); _world.Assign(8,Role.Farmer); _world.Assign(9,Role.Builder);
        await Until(()=>_world.InvitationProblem()==null,"second pair"); _world.InviteNewcomers(); _world.Assign(10,Role.Logger); _world.Assign(11,Role.Forager);
        await Until(()=>_world.Available>=22,"reserve construction timber");
        foreach(var p in _world.People.Where(p=>p.Role==Role.Logger).ToArray()) _world.Assign(p.Id,Role.Unassigned);
        await Frames(); await UiClick(_riverAction); await Frames();
        if(_world.Campaign!.River!.Phase!=1) throw new Exception("First assessment button failed");
        await Until(()=>_world.Campaign.River.Meals>=2,"first proof");
        await Press(Key.F5); string proof=_world.SaveJson(); _world.Tick(1); await Press(Key.F9); await Frames();
        if(_world.SaveJson()!=proof) throw new Exception("River manual checkpoint lost assessment");
        await UiClick(_riverAction); await Frames();
        Build(new(11,6),BuildingKind.Cottage); Build(new(14,10),BuildingKind.Cottage); Build(new(11,0),BuildingKind.Square); Build(new(9,-6),BuildingKind.VegetableGarden);
        await Until(()=>_world.Beds>=16 && _world.InvitationProblem()==null,"final homes");
        _world.InviteNewcomers(); _world.Assign(12,Role.Farmer); _world.Assign(13,Role.Forager);
        await Until(()=>_world.InvitationProblem()==null,"fourth pair"); _world.InviteNewcomers(); _world.Assign(14,Role.Farmer); _world.Assign(15,Role.Forager);
        await Until(()=>_world.RiverActionProblem()==null,"east-bank recreation");
        await UiClick(_riverAction); await Frames();
        if(_world.Campaign!.River!.Phase!=3) throw new Exception("Final assessment button failed");
        _drawerPages[2].ScrollVertical=0; await Frames(); await Capture("artifacts/f11b-river-assessment-960.png");
        _drawerPages[2].ScrollVertical=380; await Frames(); await Capture("artifacts/f11b-river-square-help-960.png");
        await Until(()=>_world.Campaign.Complete,"final proof");
        if(!_campaignBook!.Completed.Contains(6) || _nextLevel.Visible!=(World.CampaignLevels.Length>6)) throw new Exception("River completion or next-level action incorrect");
        GetWindow().Size=new(1440,900); CloseDrawer(); FrameMap(); await Frames(); await Capture("artifacts/f11b-river-complete.png");
        double[] elapsed=new double[120];
        _paused=false;
        for(int i=0;i<elapsed.Length;i++) { ulong start=Time.GetTicksUsec(); await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame); elapsed[i]=(Time.GetTicksUsec()-start)/1000d; }
        _paused=true; Array.Sort(elapsed);
        GD.Print($"RIVER PERFORMANCE: 16 residents, median frame {elapsed[60]:0.0}ms, p95 {elapsed[114]:0.0}ms (local rendered sample).");
        ulong measured=Time.GetTicksUsec(); for(int i=0;i<100;i++) UpdateHud(); double hudMs=(Time.GetTicksUsec()-measured)/100000d;
        measured=Time.GetTicksUsec(); for(int i=0;i<100;i++) { RenderActors(0); RenderFoodViews(); } double actorsMs=(Time.GetTicksUsec()-measured)/100000d;
        var simulation=World.LoadJson(_world.SaveJson()); measured=Time.GetTicksUsec(); for(int i=0;i<100;i++) simulation.Tick(.1f); double tickMs=(Time.GetTicksUsec()-measured)/100000d;
        GD.Print($"RIVER CPU: HUD {hudMs:0.00}ms/update, actors {actorsMs:0.00}ms/update, simulation {tickMs:0.00}ms/tick.");
        GD.Print($"RIVER RENDER: {Performance.GetMonitor(Performance.Monitor.RenderTotalDrawCallsInFrame):0} draw calls; {Performance.GetMonitor(Performance.Monitor.RenderTotalObjectsInFrame):0} rendered objects.");
        GD.Print("PASS: sixth campaign map, 960/1440 goals, player-triggered phases, manual assessment restore, recent square participation and persisted river completion.");
    }
}

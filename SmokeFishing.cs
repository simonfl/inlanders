using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;

public partial class Game
{
    private async void RunFishingSmoke()
    {
        try
        {
            _campaignPath="artifacts/fishing-campaign-smoke.json"; _campaignBook=new();
            var lake=World.NewCampaign(7);
            var dock=lake.Place(new(14,0),3,BuildingKind.FishingDock) ?? throw new Exception("Dock placement rejected");
            for(int i=0;i<6000 && !dock.Complete;i++) lake.Tick(.1f);
            if(!dock.Complete) throw new Exception("Dock construction did not finish");
            foreach(var p in lake.People) lake.Assign(p.Id,Role.Unassigned);
            lake.Assign(0,Role.Fisher);
            for(int i=0;i<3000 && dock.Boat?.Phase!=BoatPhase.Fishing;i++) lake.Tick(.1f);
            if(dock.Boat?.Phase!=BoatPhase.Fishing) throw new Exception("Boat did not reach fishing grounds");
            AdoptWorld(lake); _paused=true; CloseDrawer(); FrameMap();
            foreach(int width in new[]{1440,960})
            {
                GetWindow().Size=new(width,width==960?640:900); FrameMap();
                for(int i=0;i<5;i++) await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
                if(_fishGrounds.Count!=2 || !_resourceValues[Inlanders.Simulation.Resource.Fish].GetParent<Control>().Visible) throw new Exception("Lake resources missing");
                await Capture($"artifacts/f26a-lake-{width}.png");
                OpenEconomy();
                for(int i=0;i<5;i++) await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
                if(!_economyStocks.ContainsKey(Inlanders.Simulation.Resource.Fish)) throw new Exception("Fish inventory unavailable");
                CloseDrawer();
            }
            var passenger=_people[0].Body.Position;
            if(passenger.DistanceTo(new Vector3(dock.Boat.Position.X,.19f,dock.Boat.Position.Y))>.01f) throw new Exception("Passenger rendered away from boat");
            var oar=_boatViews[dock.Id].Left.Transform;
            for(int i=0;i<5;i++) await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
            if(oar!=_boatViews[dock.Id].Left.Transform) throw new Exception("Paused oar moved");
            _focus=new(12,0,1); _camera.Size=12; UpdateCamera();
            for(int i=0;i<5;i++) await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
            await Capture("artifacts/f26a-fishing-close.png");
            var frameTimes=new double[120];
            for(int i=0;i<frameTimes.Length;i++)
            {
                ulong start=Time.GetTicksUsec(); await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
                frameTimes[i]=(Time.GetTicksUsec()-start)/1000d;
            }
            Array.Sort(frameTimes);
            GD.Print($"LAKE RENDER: 8 residents, one paused boat, 960x640; median {frameTimes[60]:0.0}ms, p95 {frameTimes[114]:0.0}ms, draws {Performance.GetMonitor(Performance.Monitor.RenderTotalDrawCallsInFrame):0}.");
            for(int i=0;i<3000 && lake.DeliveredFish==0;i++) lake.Tick(.1f);
            lake.Validate();
            if(lake.DeliveredFish==0) throw new Exception("Catch did not reach pantry");
            ToggleDrawer(2);
            for(int i=0;i<5;i++) await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
            if(!_riverAction.Visible || _riverAction.Disabled || !_riverAction.Text.Contains("Prepare")) throw new Exception("Lake phase action unavailable");
            _riverAction.EmitSignal(BaseButton.SignalName.Pressed);
            for(int i=0;i<5;i++) await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
            if(lake.Campaign!.Lake!.Phase!=1 || !_riverAction.Disabled || !_objective.Text.Contains("Residents")) throw new Exception("Lake growth goals not shown");
            await Capture("artifacts/f26a-lake-goals-960.png");
            CloseDrawer(); _focus=new(3,0,4); _camera.Size=16; UpdateCamera();
            BeginPlacement(BuildingKind.FishingDock); _rotation=1;
            var pointer=_camera.UnprojectPosition(new(3,0,4));
            Input.ParseInputEvent(new InputEventMouseMotion { Position=pointer,GlobalPosition=pointer });
            for(int i=0;i<5;i++) await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
            if(!_ghostValid || !_ghost.Visible || _ghostModel.RotationDegrees.Y!=270 || !_buildDescription.Text.Contains("available")) throw new Exception("Dock shore preview/survey failed");
            await Capture("artifacts/f26a-dock-preview-960.png");
            _placing=false; RefreshGhost();
            if(OS.GetCmdlineUserArgs().Contains("--lake-review"))
            {
                AdoptWorld(World.LoadJson(System.IO.File.ReadAllText("artifacts/f11b2-complete.json"))); CloseDrawer();
                if(_world.Population!=12 || _world.Campaign?.Complete!=true) throw new Exception("Lake review needs the completed twelve-person fixture");
                foreach(int width in new[]{1440,960})
                {
                    GetWindow().Size=new(width,width==960?640:900); FrameMap();
                    for(int i=0;i<5;i++) await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
                    await Capture($"artifacts/f11b2-village-{width}.png");
                }
            }
            GD.Print("PASS: constructed dock, visible boat/passenger, paused oars, actual fish delivery, habitat stocks, fish HUD/Economy at 1440/960 and saved lake campaign phase action.");
            GetTree().Quit();
        }
        catch(Exception e) { GD.PrintErr("FISHING SMOKE FAIL: "+e); GetTree().Quit(1); }
    }
}

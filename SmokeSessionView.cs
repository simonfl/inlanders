using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
public partial class Game
{
    private async void RunSessionViewSmoke()
    {
        try
        {
            async System.Threading.Tasks.Task Frames(){for(int i=0;i<6;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
            _creativeSavePath="artifacts/session196/creative.json";_continuePath="artifacts/session196/continue.json";MakeMainMenu();
            foreach(bool relaxed in new[]{false,true})
            {
                EnterFromMenu(World.NewRiverFrontage(relaxed,true,true));await Frames();
                var home=_world.Cottages.First(c=>c.Kind==BuildingKind.Cottage);ShowWorkplaceCard(home.Id);
                _focus=BuildingPosition(home);_angle=2.1f;_camera.Size=19;UpdateCamera();await Frames();
                var focus=_focus;SaveWorld();string saved=_world.SaveJson();ReturnToMainMenu();ContinueFromMenu();await Frames();
                if(!_paused || _workCardSite!=home.Id || _focus.DistanceTo(focus)>.001f || Math.Abs(_angle-2.1f)>.001f || _camera.Size!=19 || _world.SaveJson()!=saved)throw new Exception("Continue lost place/view or changed village");
                ShowDailyLife(0);SaveWorld();ReturnToMainMenu();ContinueFromMenu();await Frames();
                if(_dailyPerson!=0 || !_dailyCard.Visible || !_paused)throw new Exception("Continue lost resident context");
                _world.SessionView=_world.SessionView! with {Person=-1,Site=999999};_world.SaveFile(_continuePath);ShowMainMenu();ContinueFromMenu();await Frames();
                if(_workCardSite>=0 || _dailyPerson>=0 || !_paused)throw new Exception("Missing target reopened stale selection");
                ShowWorkplaceCard(home.Id);SaveWorld();ClearSelection();_focus=Vector3.Zero;LoadWorld();await Frames();
                if(_workCardSite!=home.Id || !_paused)throw new Exception("F9 missed saved work");
                _paused=false;for(int i=0;i<300;i++)_world.Tick(.1f);_paused=true;_world.Validate();
                if(_world.Food.Time<29)throw new Exception("Restored settlement could not resume actual life");
                await Capture($"artifacts/196-return-{relaxed}.png");
            }
            GD.Print("PASS Normal/Relaxed actual save/menu/Continue and F9 restore chosen place/resident/view, missing target safe, paused and actual resumed life.");GetTree().Quit();
        }
        catch(Exception e){GD.PrintErr(e);GetTree().Quit(1);}
    }
}


using Godot;
using Inlanders.Simulation;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
public partial class Game
{
    private async void RunNewcomerSmoke()
    {
        try
        {
            string dir=Path.Combine(Path.GetTempPath(),"inlanders-challenge-"+Guid.NewGuid());Directory.CreateDirectory(dir);
            _creativeSavePath=Path.Combine(dir,"creative.json");_continuePath=Path.Combine(dir,"continue.json");
            async Task Frames(){for(int i=0;i<8;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
            MakeMainMenu();GetWindow().Size=new(960,640);await Frames();await UiClick(_mainButtons["Play challenge"]);await Frames();
            await UiClick(_mainButtons["New challenge"]);await Frames();
            if(_world.Challenge==null || !_challengeGoals.IsVisibleInTree() || !_challengeInvite.Disabled || !CurrentSavePath.Contains("newcomer-challenge"))throw new Exception("Challenge entry/brief failed");
            if(!_challengeProgress.Text.Contains("two meals") || !_challengeProgress.Text.Contains("Everyone must be fed"))throw new Exception("Full rules hidden");
            if(_challengeInvite.GetGlobalRect().End.Y>_hud.Size.Y-75)throw new Exception("Invitation below fold");
            await Capture("artifacts/216-challenge-brief.png");
            if(!SaveSession())throw new Exception("Challenge save failed");ReturnToMainMenu();await Frames();await UiClick(_mainButtons["Continue"]);await Frames();
            if(_world.Challenge==null)throw new Exception("Continue lost challenge");
            AdoptWorld(World.LoadFile("artifacts/215-challenge-False-False.json"));if(!_drawer.Visible || _tabs.CurrentTab!=2)ToggleDrawer(2);await Frames();
            _drawerPages[2].EnsureControlVisible(_challengeContinue);await Frames();await Capture("artifacts/216-challenge-complete.png");
            await UiClick(_challengeContinue);await Frames();if(!_world.Challenge!.Continuing || _drawer.Visible)throw new Exception("Continue building failed");
            if(!SaveSession())throw new Exception("Completed challenge save failed");
            ReturnToMainMenu();await Frames();await UiClick(_mainButtons["Play challenge"]);await Frames();
            await UiClick(_mainButtons["Situation: first settlement · change"]);await Frames();await UiClick(_mainButtons["New challenge"]);await Frames();
            if(_world.Challenge?.Repair!=true || !CurrentSavePath.Contains("far-bank-challenge"))throw new Exception("Repair situation identity");
            await Capture("artifacts/217-repair-brief.png");
            if(!SaveSession() || World.LoadFile(CurrentSavePath).Challenge?.Repair!=true)throw new Exception("Repair save");
            await UiClick(_challengeAccess);await Frames();
            var field=_world.Cottages.Single(b=>b.Id==_workCardSite);
            if(!_workCardMove.IsVisibleInTree() || !_workCardCrossing.IsVisibleInTree() || !_workCardText.Text.Contains("No walking route"))throw new Exception("Direct access recovery hidden");
            if(_workCardMove.GetGlobalRect().End.Y>_hud.Size.Y-100)throw new Exception("Recovery action below fold");
            await Capture("artifacts/218-cut-off-field.png");
            await UiClick(_workCardMove);await Frames();await Press(Key.Escape);await Frames();
            if(field.WorkPaused || _movingSite>=0)throw new Exception("Move cancellation stranded production");
            await UiClick(_workCardCrossing);await Frames();
            var at=new Cell(5,0);var point=_camera.UnprojectPosition(OnGround(at.X,at.Z));
            Input.ParseInputEvent(new InputEventMouseMotion{Position=point,GlobalPosition=point});await Frames();
            if(!_ghostValid)throw new Exception("Crossing preview rejected");await Click(point);await Frames();_placing=false;RefreshGhost();
            var bridge=_world.Cottages.Single(b=>b.Kind==BuildingKind.Bridge);
            for(int i=0;i<2400 && _world.FoodAvailableAt(field.Id,Inlanders.Simulation.Resource.Vegetables)==0;i++){_world.Tick(.1f);if(i%100==0)await Frames();}
            if(!bridge.Complete || _world.ReadWorkplace(field).MissingAccess || _world.FoodAvailableAt(field.Id,Inlanders.Simulation.Resource.Vegetables)==0)throw new Exception("Crossing did not restore real food delivery");
            RenderActors(0);ShowWorkplaceCard(field.Id);await Frames();await Capture("artifacts/218-restored-field.png");_world.Validate();
            ReturnToMainMenu();await Frames();await UiClick(_mainButtons["Free play"]);await Frames();
            if(!_mainButtons.ContainsKey("Food setup: choose your own · change") || _mainButtons.ContainsKey("Tend a river settlement"))throw new Exception("Duplicate free-play starts");
            await Capture("artifacts/219-free-play-menu.png");
            await UiClick(_mainButtons["Food setup: choose your own · change"]);await Frames();await UiClick(_mainButtons["New settlement"]);await Frames();
            if(_world.Challenge!=null || _world.PublicPlace is not {RiverLandscape:true,PlayerFounded:false})throw new Exception("Prepared food setup identity");
            GD.Print("PASS consolidated free-play setup; ordinary inspect, direct pause/move cancel, crossing preview/place and actual construction plus restored field delivery (accelerated model ticks); native repair situation selection and separate save; native new challenge brief, disabled early invitation, save/Continue identity, completed outcome card and continue building at960. Completion fixture uses actual simulation, not UI playthrough.");GetTree().Quit();
        }
        catch(Exception e){GD.PrintErr(e);GetTree().Quit(1);}
    }
}

using Godot;
using Inlanders.Simulation;
using System;
using System.IO;
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
            await Capture("artifacts/215-challenge-brief.png");
            if(!SaveSession())throw new Exception("Challenge save failed");ReturnToMainMenu();await Frames();await UiClick(_mainButtons["Continue"]);await Frames();
            if(_world.Challenge==null)throw new Exception("Continue lost challenge");
            AdoptWorld(World.LoadFile("artifacts/215-challenge-False-False.json"));if(!_drawer.Visible || _tabs.CurrentTab!=2)ToggleDrawer(2);await Frames();
            _drawerPages[2].EnsureControlVisible(_challengeContinue);await Frames();await Capture("artifacts/215-challenge-complete.png");
            await UiClick(_challengeContinue);await Frames();if(!_world.Challenge!.Continuing || _drawer.Visible)throw new Exception("Continue building failed");
            if(!SaveSession())throw new Exception("Completed challenge save failed");
            GD.Print("PASS native new challenge brief, disabled early invitation, save/Continue identity, completed outcome card and continue building at960. Completion fixture uses actual simulation, not UI playthrough.");GetTree().Quit();
        }
        catch(Exception e){GD.PrintErr(e);GetTree().Quit(1);}
    }
}

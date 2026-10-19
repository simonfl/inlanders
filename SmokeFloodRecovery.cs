using Godot;
using Inlanders.Simulation;
using System;
using System.IO;
using System.Linq;
public partial class Game
{
    private async void RunFloodRecoverySmoke()
    {
        try
        {
            string dir=Path.Combine(Path.GetTempPath(),"inlanders-restoration-"+Guid.NewGuid());Directory.CreateDirectory(dir);
            _creativeSavePath=Path.Combine(dir,"creative.json");_continuePath=Path.Combine(dir,"continue.json");
            GetWindow().Size=new(960,640);MakeMainMenu();
            await UiClick(_mainButtons["Play challenge"]);await UiClick(_mainButtons["Situation: first settlement · change"]);await UiClick(_mainButtons["Situation: cut-off fields · change"]);
            if(_challengeMapPanel?.IsVisibleInTree()!=true || !_challengeMapSummary.Text.Contains("8 beds"))throw new Exception("Scenario map/start conditions missing");
            await Capture("artifacts/restoration/menu-960.png");
            GetWindow().Size=new(1440,900);for(int i=0;i<4;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
            await Capture("artifacts/restoration/menu-1440.png");GetWindow().Size=new(960,640);
            await UiClick(_mainButtons["New challenge"]);
            if(_challengeMapPanel.IsVisibleInTree())throw new Exception("Menu map leaked into play");
            for(int i=0;i<8;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
            if(_world.Challenge?.Restoration!=true || _challengeInvite.Visible || !CurrentSavePath.Contains("restoration-challenge") || !_challengeProgress.Text.Contains("No new arrivals"))throw new Exception("Restoration entry unclear");
            _paused=true;await Capture("artifacts/restoration/entry-960.png");
            await UiClick(_challengeRecover);if(_tabs.CurrentTab!=1)throw new Exception("Housing recovery did not open catalogue");
            if(!SaveSession() || World.LoadFile(CurrentSavePath).Challenge?.Restoration!=true)throw new Exception("Restoration save");
            AdoptWorld(World.LoadFile("artifacts/restoration/False-bridge-complete.json"));ToggleDrawer(2);UpdateHud();
            for(int i=0;i<8;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
            _drawerPages[2].EnsureControlVisible(_challengeContinue);
            await UiClick(_challengeContinue);if(!_world.Challenge!.Continuing)throw new Exception("Restoration continue failed");
            GD.Print("PASS native restoration entry, purpose, housing action, separate save and actual simulated completion/continue.");GetTree().Quit();
        }
        catch(Exception e){GD.PrintErr(e);GetTree().Quit(1);}
    }
}

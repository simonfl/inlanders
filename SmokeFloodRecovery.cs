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
            await UiClick(_mainButtons["Play challenge"]);if(!_mainButtons["A place for newcomers · introduction"].Disabled || _mainButtons.ContainsKey("Far-bank newcomer comparison"))throw new Exception("Primary challenge choices unclear");
            await UiClick(_mainButtons["After the flood · restoration"]);
            for(int i=0;i<4;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
            if(_mainButtons["New challenge"].GetGlobalRect().End.Y>GetWindow().Size.Y-40)throw new Exception("Start below compact menu fold");
            await UiClick(_mainButtons["Mode: Normal · change"]);
            if(!_challengeRelaxed || !_challengeRestoration)throw new Exception("Relaxed selection lost situation");
            await UiClick(_mainButtons["Mode: Relaxed · change"]);
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
            ToggleDrawer(0);UpdateHud();for(int i=0;i<8;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
            _drawerPages[0].EnsureControlVisible(_foodFirst);await UiClick(_foodFirst);
            if(_world.Founding?.FoodFirst!=true)throw new Exception("Food focus UI failed");
            CloseDrawer();var crop=_world.Cottages.First(c=>c.Kind==BuildingKind.VegetableField);ShowWorkplaceCard(crop.Id);
            for(int i=0;i<8;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
            _workCardScroll.EnsureControlVisible(_cropOrder);await UiClick(_cropOrder);
            if(!crop.HarvestThenPause || crop.WorkPaused || !_workCardMove.Disabled)throw new Exception("Crop order/land-bound move UI failed");
            if(!SaveSession() || !World.LoadFile(CurrentSavePath).Founding!.FoodFirst || !World.LoadFile(CurrentSavePath).Cottages.Single(c=>c.Id==crop.Id).HarvestThenPause)throw new Exception("Work orders not saved");
            if(!SaveSession() || World.LoadFile(CurrentSavePath).Challenge?.Restoration!=true)throw new Exception("Restoration save");
            AdoptWorld(World.LoadFile("artifacts/restoration/False-bridge-complete.json"));ToggleDrawer(2);UpdateHud();
            for(int i=0;i<8;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
            _drawerPages[2].EnsureControlVisible(_challengeContinue);
            await UiClick(_challengeContinue);if(!_world.Challenge!.Continuing)throw new Exception("Restoration continue failed");
            GD.Print("PASS native food-first and crop orders, land-bound move rejection, restoration entry, purpose, housing action, separate save and actual simulated completion/continue.");GetTree().Quit();
        }
        catch(Exception e){GD.PrintErr(e);GetTree().Quit(1);}
    }
}

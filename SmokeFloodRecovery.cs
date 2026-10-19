using Godot;
using Inlanders.Simulation;
using System;
using System.IO;
using System.Linq;
using Resource=Inlanders.Simulation.Resource;
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
            if(_world.RestorationFoodWarning!=null)throw new Exception("Opening falsely reports a supply trend");
            _world.People[0].Fed=false;UpdateHud();
            if(!_menuButtons[2].Text.Contains("8/12 housed") || !_menuButtons[2].Text.Contains("missed meals"))throw new Exception("Housing conceals missed meals");
            _world.People[0].Fed=true;UpdateHud();
            await UiClick(_recoveryFood);if(_tabs.CurrentTab!=1)throw new Exception("Food recovery action failed");
            ToggleDrawer(2);UpdateHud();for(int i=0;i<4;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
            await UiClick(_challengeRecover);if(_tabs.CurrentTab!=1)throw new Exception("Housing recovery did not open catalogue");
            BeginPlacement(BuildingKind.Cottage);CloseDrawer();UpdateHud();
            RenderHomePlotUi();if(_homePlotPanel.Visible)throw new Exception("Ordinary housing prompts unsolicited yard styling");
            if(!_hint.Text.Contains("new +") || !_hint.Text.Contains("queued"))throw new Exception("Construction budget absent from placement");
            await Capture("artifacts/restoration/construction-budget-960.png");_placing=false;RefreshGhost();
            ToggleDrawer(0);UpdateHud();for(int i=0;i<8;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
            CloseDrawer();var crop=_world.Cottages.First(c=>c.Kind==BuildingKind.VegetableField);ShowWorkplaceCard(crop.Id);
            for(int i=0;i<8;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
            _workCardScroll.EnsureControlVisible(_cropOrder);await UiClick(_cropOrder);
            if(!crop.HarvestThenPause || crop.WorkPaused || !_workCardMove.Disabled)throw new Exception("Crop order/land-bound move UI failed");
            if(!SaveSession() || !World.LoadFile(CurrentSavePath).Cottages.Single(c=>c.Id==crop.Id).HarvestThenPause)throw new Exception("Work orders not saved");
            if(!SaveSession() || World.LoadFile(CurrentSavePath).Challenge?.Restoration!=true)throw new Exception("Restoration save");
            AdoptWorld(World.LoadFile("artifacts/restoration/False-bridge-complete.json"));ToggleDrawer(2);UpdateHud();
            for(int i=0;i<8;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
            _drawerPages[2].EnsureControlVisible(_challengeContinue);
            await UiClick(_challengeContinue);if(!_world.Challenge!.Continuing)throw new Exception("Restoration continue failed");
            foreach(bool relaxed in new[]{false,true})
            {
                var supply=World.NewRestorationChallenge(relaxed);supply.Food.InitialBerries=supply.Food.Berries=12;
                foreach(var field in supply.Cottages.Where(c=>c.Kind==BuildingKind.VegetableField)){field.PantryFood[Array.IndexOf(World.EdibleKinds,Resource.Vegetables)]=24;supply.Food.GrownVegetables+=24;}
                supply.Validate();AdoptWorld(supply);CloseDrawer();_paused=true;UpdateHud();
                if(_placeFoodCount.Text!="12" || _placeFoodTitle.Text!="FOOD · LOW" || !_placeFood.TooltipText.Contains("60 stored in total; 48"))throw new Exception("Stranded food masks HUD warning");
                await Capture($"artifacts/restoration/reachable-food-{relaxed}.png");
                supply.ApplyWoodlandCare(new(-10,3),new(-5,13),WoodlandIntent.Renew,false);supply.Place(new(5,0),0,BuildingKind.Bridge);bool second=false;
                for(int i=0;i<3000 && !(second && supply.Cottages.Count(c=>c.Kind==BuildingKind.Bridge && c.Complete)==2);i++)
                {
                    if(!second && supply.Cottages.Any(c=>c.Kind==BuildingKind.Bridge && c.Complete)){if(supply.Place(new(0,-7),0,BuildingKind.Bridge)==null)throw new Exception("HUD reconnection refused");second=true;}
                    supply.Tick(.1f);
                    if(i%100==0)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
                }
                supply.Validate();UpdateHud();
                if(!second || supply.ReachableEdibleStored!=supply.EdibleStored || _placeFoodCount.Text!=supply.EdibleStored.ToString() || _placeFoodTitle.Text!="FOOD")throw new Exception("Reconnected food HUD failed");
                GD.Print($"PASS native reachable-food count/warning and real crossing recovery relaxed={relaxed}");
            }
            GD.Print("PASS native automatic work and crop orders, land-bound move rejection, restoration entry, purpose, housing action, separate save and actual simulated completion/continue.");GetTree().Quit();
        }
        catch(Exception e){GD.PrintErr(e);GetTree().Quit(1);}
    }
}

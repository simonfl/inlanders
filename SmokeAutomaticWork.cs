using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
public partial class Game
{
    private async void RunAutomaticWorkSmoke()
    {
        try
        {
            foreach(bool relaxed in new[]{false,true})
            {
                AdoptWorld(World.NewRiverFrontage(relaxed,true,true));_paused=true;GetWindow().Size=new(960,640);
                ToggleDrawer(0);UpdateHud();await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
                if(!_staffing.Text.Contains("32 in reserve") || !_resourceValues[Inlanders.Simulation.Resource.Logs].GetParent<Control>().TooltipText.Contains("32 in reserve"))throw new Exception("Timber reserve explanation missing");
                if(_professionControls.Visible || !_staffing.Text.Contains("Food reserves sufficient"))throw new Exception("Public staffing surface wrong");
                _world.Assign(0,Role.Fisher);UpdateHud();await UiClick(_automaticWork);
                if(_world.People.Any(p=>!p.SharedWorker))throw new Exception("Automatic return failed");
                SelectPerson(0);UpdateHud();if(_jobChoice.Visible || _assignButton.Visible || _assignmentChoice.Visible)throw new Exception("Public role controls visible");
                ToggleDrawer(4);UpdateHud();await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
                if(!_reserves.Visible || !_reserveFood.Text.Contains("four per resident") || !_reserveButtons[Inlanders.Simulation.Resource.Planks].Text.Contains("/24"))throw new Exception("Reserve overview missing");
                await Capture($"artifacts/223-reserves-{relaxed}.png");
                await UiClick(_reserveButtons[Inlanders.Simulation.Resource.Planks]);
                if(!_placing || _buildKind!=BuildingKind.Sawmill)throw new Exception("Reserve supply route did not plan first mill");
                _placing=false;ClearSelection();CloseDrawer();
                var dock=_world.Cottages.Single(c=>c.Kind==BuildingKind.FishingDock);ShowWorkplaceCard(dock.Id);UpdateHud();
                for(int i=0;i<8;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
                await Capture($"artifacts/212-food-rest-{relaxed}.png");
            }
            AdoptWorld(World.NewRiverFrontage(true,true,true));_paused=true;
            var millAt=_world.Map.Land.First(c=>_world.PlacementProblem(c,0,BuildingKind.Sawmill)==null);
            var mill=_world.Place(millAt,0,BuildingKind.Sawmill)!;_world.SetOutputTarget(mill.Id,0);
            ShowWorkplaceCard(mill.Id);UpdateHud();RenderWorkplaceCard();
            for(int i=0;i<4;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
            if(_world.ReadWorkplace(mill).State!="Manual stock cap" || !_workCardAutoTarget.Visible)throw new Exception("Manual cap not actionable");
            await UiClick(_workCardAutoTarget);
            if(!mill.AutomaticOutputTarget || _world.EffectiveOutputTarget(mill)!=24)throw new Exception("Automatic target recovery failed");
            GD.Print("PASS ordinary workplace card restores automatic material target.");
            AdoptWorld(World.NewRiverLivelihood());_paused=true;
            var project=_world.Place(new(0,0),0,BuildingKind.Cottage)!;ShowWorkplaceCard(project.Id);RememberSupplyOrigin();
            var originalFocus=_focus;BeginEverydayTimber();
            if(!_careActive || _careIntent!=WoodlandIntent.Renew || _carePlant.ButtonPressed)throw new Exception("Default area timber wrong");
            CancelWoodlandCare();if(_workCardSite!=project.Id || _focus!=originalFocus)throw new Exception("Area cancel lost origin");
            RememberSupplyOrigin();BeginEverydayTimber();
            _careFirst=new(-10,-12);_careLast=new(-5,13);_careSelected=true;RefreshWoodlandCare();
            for(int i=0;i<8;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
            await Capture("artifacts/213-timber-area.png");await UiClick(_careApply);
            if(_careActive || _workCardSite!=project.Id || _world.ManagedWoodland.Count==0 || _watching)throw new Exception("Area apply lost project");
            GD.Print("PASS area harvest-and-renew proposal, pure cancel, actual apply and project return.");
            GD.Print("PASS public automatic staffing, return action, resident controls and stocked dock in Normal/Relaxed at960.");GetTree().Quit();
        }
        catch(Exception e){GD.PrintErr(e);GetTree().Quit(1);}
    }
}

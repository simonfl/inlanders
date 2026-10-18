using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
public partial class Game
{
    private async void RunInputRecoverySmoke()
    {
        try
        {
            async System.Threading.Tasks.Task Frames(){for(int i=0;i<8;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
            AdoptWorld(World.NewRiverLivelihood(true));_paused=true;CloseManagementUi();GetWindow().Size=new(960,640);
            var oven=_world.Place(new(0,0),0,BuildingKind.Bakery)!;ShowWorkplaceCard(oven.Id);await Frames();
            if(!_workCardInput.Visible || _world.ReadWorkplace(oven).MissingInput!=Inlanders.Simulation.Resource.Grain)throw new Exception("Ingredient recovery hidden");
            await Capture("artifacts/204-bakery-input.png");if(_workCard.GetGlobalRect().End.Y>_hud.Size.Y-65)throw new Exception("Input card overflow: "+_workCard.GetGlobalRect());
            await UiClick(_workCardInput);await Frames();if(!_placing || _buildKind!=BuildingKind.Farm)throw new Exception("Missing grain did not plan a field");
            var originalFocus=_supplyFocus;var originalAngle=_supplyAngle;var originalZoom=_supplyZoom;
            await Press(Key.Escape);await Frames();
            if(_workCardSite!=oven.Id || _focus!=originalFocus || _angle!=originalAngle || _camera.Size!=originalZoom)throw new Exception("Cancelled supply lost original project/view");
            await UiClick(_workCardInput);await Frames();
            _focus=OnGround(0,-7);UpdateCamera();await Frames();
            await Click(_camera.UnprojectPosition(OnGround(0,-7)));await Frames();
            var field=_world.Cottages.Single(c=>c.Kind==BuildingKind.Farm);
            if(!_supplyReturn.Visible || _workCardSite!=field.Id)throw new Exception("Placed source lost return context");
            await UiClick(_supplyReturn);await Frames();
            if(_workCardSite!=oven.Id || _focus!=originalFocus)throw new Exception("Placed-source return failed");
            _world.SetWorkplacePaused(field.Id,true);ShowWorkplaceCard(oven.Id);await Frames();await UiClick(_workCardInput);await Frames();
            if(_workCardSite!=field.Id || !field.WorkPaused)throw new Exception("Existing paused source not inspected honestly");
            await Capture("artifacts/207-supply-return.png");
            await UiClick(_supplyReturn);await Frames();if(_workCardSite!=oven.Id)throw new Exception("Existing-source return failed");
            await UiClick(_workCardInput);await Frames();
            await UiClick(_workCardPause);if(field.WorkPaused)throw new Exception("Source did not resume");
            for(int i=0;i<6000 && _world.Food.EatenBread==0;i++)_world.Tick(.1f);
            if(_world.Food.EatenBread==0)throw new Exception("Recovered chain did not feed anyone");_world.Validate();
            ShowWorkplaceCard(oven.Id);await Frames();
            if(_workCardMove.Visible || _workCardDetails.Visible || _workCardSupply.Visible || _workCardTrips.Visible)throw new Exception("Occasional actions crowd everyday life");
            if(_workCard.GetGlobalRect().End.Y>_hud.Size.Y-75)throw new Exception("Recovered card overflow");
            await Capture("artifacts/206-bakery-everyday.png");
            await UiClick(_workOptionsButton);await Frames();
            if(!_workCardMove.Visible || !_workCardDetails.Visible || !_workCardTrips.Visible)throw new Exception("Expanded actions missing");
            await UiClick(_workCardTrips);await Frames();
            if(_placeJourneySite!=oven.Id)throw new Exception("Expanded trip control inaccessible");
            if(_workCard.GetGlobalRect().End.Y>_hud.Size.Y-75)throw new Exception("Expanded card overflow");
            await Capture("artifacts/206-bakery-changes.png");
            await UiClick(_workOptionsButton);await Frames();await UiClick(_workCardWatchPlace);await Frames();
            if(!_watching)throw new Exception("Place-first Watch unavailable");await Press(Key.Escape);await Frames();
            if(_workCardSite!=oven.Id || _workOptions)throw new Exception("Watch return lost simple place");
            GD.Print("PASS ordinary bakery ingredient action, new field planning, existing paused-field inspection/resume, actual grain→bread→eaten meal.");GetTree().Quit();
        }
        catch(Exception e){GD.PrintErr(e);GetTree().Quit(1);}
    }
}

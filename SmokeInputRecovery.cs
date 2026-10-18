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
            if(_workCardMove.Visible || _workCardDetails.Visible || _workCardSupply.Visible || !_workCardTrips.Visible)throw new Exception("Occasional actions crowd everyday life");
            if(_workCard.GetGlobalRect().End.Y>_hud.Size.Y-75)throw new Exception("Recovered card overflow");
            await Capture("artifacts/210-bakery-everyday.png");
            await UiClick(_workOptionsButton);await Frames();
            if(!_workCardMove.Visible || !_workCardDetails.Visible || _workCardTrips.Visible)throw new Exception("Expanded actions missing");
            await UiClick(_workOptionsButton);await Frames();
            var observationFocus=_focus;float observationZoom=_camera.Size,observationAngle=_angle;
            await UiClick(_workCardTrips);await Frames();
            if(_placeJourneySite!=oven.Id || _workOptionsButton.Visible)throw new Exception("Observation grouped with changes");
            if(_placeJourney==null)throw new Exception("Expected actual recovered bread trip");
            await UiClick(_placeTripFrame);await Frames();
            if(_workCard.GetGlobalRect().End.Y>_hud.Size.Y-75)throw new Exception("Expanded card overflow");
            await Capture("artifacts/210-bakery-trip.png");
            await UiClick(_workCardTrips);await Frames();
            if(_focus!=observationFocus || _camera.Size!=observationZoom || _angle!=observationAngle)throw new Exception("Trip return lost original view");
            await UiClick(_workCardTrips);await Frames();await UiClick(_placeTripFrame);await Frames();await UiClick(_placeTripFollow);await Frames();
            await UiClick(_dailyHomeBack);await Frames();
            if(_workCardSite!=oven.Id || _focus!=observationFocus || _camera.Size!=observationZoom || _workOptions)throw new Exception("Follow return lost original place/view");
            GetWindow().Size=new(1440,900);await Frames();await UiClick(_workCardTrips);await Frames();
            if(_workOptionsButton.Visible || _workCard.GetGlobalRect().End.Y>_hud.Size.Y-75)throw new Exception("Wide trip presentation failed");
            await Capture("artifacts/210-bakery-trip-wide.png");await UiClick(_workCardTrips);await Frames();
            await UiClick(_workCardWatchPlace);await Frames();
            if(!_watching)throw new Exception("Place-first Watch unavailable");await Press(Key.Escape);await Frames();
            if(_workCardSite!=oven.Id || _workOptions)throw new Exception("Watch return lost simple place");
            AdoptWorld(World.NewRiverLivelihood(true));_paused=true;CloseManagementUi();
            var consumer=_world.Place(new(0,0),0,BuildingKind.Bakery)!;
            var older=_world.Place(new(0,-7),0,BuildingKind.Farm)!;
            var nearbyCell=_world.Map.Land.Where(c=>_world.PlacementProblem(c,0,BuildingKind.Farm)==null).OrderBy(c=>(c.Point-consumer.Entrance.Point).LengthSquared()).First();
            var nearby=_world.Place(nearbyCell,0,BuildingKind.Farm)!;
            ShowWorkplaceCard(consumer.Id);await Frames();
            if(_workCardInput.Text!="Inspect grain field")throw new Exception("Supply label conceals its actual action");
            await UiClick(_workCardInput);await Frames();
            if(_workCardSite!=nearby.Id)throw new Exception("UI inspected an older distant source");
            await UiClick(_supplyReturn);await Frames();if(_workCardSite!=consumer.Id)throw new Exception("Relevant-source return failed");
            foreach(bool relaxed in new[]{false,true})
            {
                AdoptWorld(World.NewRiverLivelihood(relaxed));_paused=true;CloseManagementUi();
                var squareCell=_world.Map.Land.OrderBy(c=>c.Point.LengthSquared()).First(c=>_world.PlacementProblem(c,0,BuildingKind.Square)==null);
                var square=_world.Place(squareCell,0,BuildingKind.Square)!;
                for(int tick=0;tick<2000 && !square.Complete;tick++)_world.Tick(.1f);
                if(!square.Complete)throw new Exception("Square fixture did not complete");
                ShowWorkplaceCard(square.Id);await Frames();
                await UiClick(_workOptionsButton);await Frames();await UiClick(_workCardDetails);await Frames();if(!_inspectorSecondary.Visible)await UiClick(_inspectorDetails);await Frames();
                if(!_inspectorManual.IsVisibleInTree() || _inspectorManual.Text.Contains("supper",StringComparison.OrdinalIgnoreCase) || _inspectorManual.Text.Contains("Goals") || _inspectorManual.Text.Contains("Creative") || !_inspectorManual.Text.Contains("visits"))throw new Exception($"Square guidance ({relaxed}): visible={_inspectorManual.IsVisibleInTree()}, text={_inspectorManual.Text}");
                _inspectionScroll.EnsureControlVisible(_inspectorManual);await Frames();await Capture($"artifacts/210-square-guidance-{relaxed}.png");
            }
            GD.Print("PASS Normal/Relaxed public square Details → Details & policies describes actual visits without archived supper instructions.");
            GD.Print("PASS ordinary bakery ingredient action, new field planning, existing paused-field inspection/resume, actual grain→bread→eaten meal.");GetTree().Quit();
        }
        catch(Exception e){GD.PrintErr(e);GetTree().Quit(1);}
    }
}

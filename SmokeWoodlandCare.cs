using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
public partial class Game
{
    private async void RunWoodlandCareSmoke()
    {
        try
        {
            async System.Threading.Tasks.Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
            AdoptWorld(World.NewRiverFrontage(false,true,true));_paused=true;GetWindow().Size=new(960,640);CloseManagementUi();
            var tree=_world.Trees.OrderBy(t=>t.Cell.Point.LengthSquared()).First();
            _focus=OnGround(tree.Cell.X,tree.Cell.Z);_camera.Size=18;UpdateCamera();await Frames();
            ToggleDrawer(1);SelectBuildSection(1);await Frames();
            var entry=_buildSections[1].GetChildren().OfType<Button>().Single(b=>b.Text=="Care for woodland");
            _drawerPages[1].EnsureControlVisible(entry);await Frames();await UiClick(entry);await Frames();
            var point=_camera.UnprojectPosition(OnGround(tree.Cell.X,tree.Cell.Z));string original=_world.SaveJson();
            await Click(point);await Frames();
            if(!_careSelected || _careApply.Disabled || _world.SaveJson()!=original)throw new Exception("Care proposal changed village");
            if(_carePanel.GetGlobalRect().End.Y>_hud.Size.Y-75)throw new Exception("Care controls overflow compact view");
            await Capture("artifacts/186-care-proposal-960.png");await Press(Key.Escape);await Frames();
            if(_careActive || !_paused || _world.SaveJson()!=original)throw new Exception("Care cancellation changed state");
            // Reconsider an applied clear order through the same ordinary proposal.
            _world.SetClearing(tree.Cell,true);BeginWoodlandCare();await Frames();await Click(point);
            await UiClick(_careChoices[WoodlandIntent.Keep]);await Frames();
            if(_careApply.Disabled || !tree.ClearRequested)throw new Exception("Reconsideration preview must remain pure and applicable");
            await UiClick(_careApply);_paused=true;await Frames();ExitWatch();
            if(tree.ClearRequested || !tree.Preserved || tree.Felled)throw new Exception("Keep did not cancel pending clearance");
            _careIntent=WoodlandIntent.Renew;
            // Choose an actual wanted addition with ordinary placement, then supply it from selected woodland.
            var at=_world.Map.Land.OrderBy(c=>(c.Point-new Cell(0,7).Point).LengthSquared()).First(c=>_world.PlacementProblem(c,0,BuildingKind.SeatingGarden)==null);
            _focus=OnGround(at.X,at.Z);UpdateCamera();BeginPlacement(BuildingKind.SeatingGarden);await Frames();await Click(_camera.UnprojectPosition(OnGround(at.X,at.Z)));await Press(Key.Escape);await Frames();
            var addition=_world.Cottages.Last();if(addition.Kind!=BuildingKind.SeatingGarden)throw new Exception("Wanted addition placement failed");
            _focus=OnGround(tree.Cell.X,tree.Cell.Z);UpdateCamera();BeginWoodlandCare();await Frames();await Click(_camera.UnprojectPosition(OnGround(tree.Cell.X,tree.Cell.Z)));await UiClick(_careApply);await Frames();
            if(!_watching || _paused || tree.Preserved || !_world.ManagedWoodland.Contains(tree.Cell))throw new Exception("Applied care lost intention/watch");
            _speed=6;double start=_uiTime;
            while(_uiTime-start<85 && (_world.TreesPlanted==0 || tree.Growth<1 || !addition.Complete))await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
            _paused=true;
            if(_world.TreesPlanted==0 || tree.Growth<1 || !addition.Complete || _world.People.Any(p=>!p.Fed))throw new Exception("Care/addition never became supplied renewed village");
            await Capture("artifacts/186-care-renewed-960.png");ExitWatch();_world.Validate();
            GD.Print("PASS native woodland proposal/cancel, chosen addition, apply/watch, real timber/renewal/growth and fed village.");GetTree().Quit();
        }
        catch(Exception e){GD.PrintErr(e);GetTree().Quit(1);}
    }
}

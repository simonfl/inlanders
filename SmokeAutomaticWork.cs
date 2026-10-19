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
                if(_professionControls.Visible || !_staffing.Text.Contains("Food reserves sufficient"))throw new Exception("Public staffing surface wrong");
                _world.Assign(0,Role.Fisher);UpdateHud();await UiClick(_automaticWork);
                if(_world.People.Any(p=>!p.SharedWorker))throw new Exception("Automatic return failed");
                SelectPerson(0);UpdateHud();if(_jobChoice.Visible || _assignButton.Visible || _assignmentChoice.Visible)throw new Exception("Public role controls visible");
                var dock=_world.Cottages.Single(c=>c.Kind==BuildingKind.FishingDock);ShowWorkplaceCard(dock.Id);UpdateHud();
                for(int i=0;i<8;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
                await Capture($"artifacts/212-food-rest-{relaxed}.png");
            }
            GD.Print("PASS public automatic staffing, return action, resident controls and stocked dock in Normal/Relaxed at960.");GetTree().Quit();
        }
        catch(Exception e){GD.PrintErr(e);GetTree().Quit(1);}
    }
}

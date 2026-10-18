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
            await Press(Key.Escape);var field=_world.Place(new(0,-7),0,BuildingKind.Farm)!;
            _world.SetWorkplacePaused(field.Id,true);ShowWorkplaceCard(oven.Id);await Frames();await UiClick(_workCardInput);await Frames();
            if(_workCardSite!=field.Id || !field.WorkPaused)throw new Exception("Existing paused source not inspected honestly");
            await UiClick(_workCardPause);if(field.WorkPaused)throw new Exception("Source did not resume");
            for(int i=0;i<6000 && _world.Food.EatenBread==0;i++)_world.Tick(.1f);
            if(_world.Food.EatenBread==0)throw new Exception("Recovered chain did not feed anyone");_world.Validate();
            ShowWorkplaceCard(oven.Id);await Frames();await Capture("artifacts/204-bakery-recovered.png");
            GD.Print("PASS ordinary bakery ingredient action, new field planning, existing paused-field inspection/resume, actual grain→bread→eaten meal.");GetTree().Quit();
        }
        catch(Exception e){GD.PrintErr(e);GetTree().Quit(1);}
    }
}

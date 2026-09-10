using Godot;
using Inlanders.Simulation;
using System;
using System.Threading.Tasks;

public partial class Game
{
    private async Task CheckHappinessUi()
    {
        var previous=_world; var size=GetWindow().Size;
        async Task Frames() { for(int i=0;i<4;i++) await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame); }
        try
        {
            AdoptWorld(new World()); _paused=true;
            foreach(var width in new[]{1440,960})
            {
                GetWindow().Size=new(width,width==960?640:900); SelectPerson(0); await Frames();
                if(!_happinessButton.Text.Contains("Settling in") || !_happinessReasons.Text.Contains("Village meal variety") || !_happinessButton.Text.Contains("40/100"))
                    throw new Exception("Happiness reasons missing");
                string saved=_world.SaveJson(); await Frames();
                if(saved!=_world.SaveJson()) throw new Exception("Paused mood changed");
                _happinessReasons.Show(); _inspectionScroll.EnsureControlVisible(_happinessButton); await Frames();
                await Capture($"artifacts/f14-inspector-{width}.png"); _happinessReasons.Hide();
                ToggleDrawer(0); await Frames();
                if(!_staffing.Text.Contains("happiness: 40/100")) throw new Exception("Village happiness missing");
                CloseDrawer();
            }
            foreach(var p in _world.People) _world.Assign(p.Id,Role.Unassigned);
            _world.Food.GrownVegetables=_world.Food.Vegetables=20;
            _world.Food.BakedBread=_world.Food.Bread=20;
            _world.Food.GrownGrain=_world.Food.UsedGrain=10;
            _world.Food.MealClock=59.9f; _world.Tick(.2f); _world.Validate();
            foreach(var width in new[]{1440,960})
            {
                GetWindow().Size=new(width,width==960?640:900); SelectPerson(0); await Frames();
                if(!_happinessReasons.Text.Contains("3 berries · 3 vegetables · 2 bread") || !_happinessReasons.Text.Contains("+20/20"))
                    throw new Exception("Actual meal missing from resident explanation");
                _happinessReasons.Show(); _inspectionScroll.EnsureControlVisible(_happinessReasons); await Frames();
                await Capture($"artifacts/f14b-meal-person-{width}.png");
                OpenEconomy(); await Frames();
                if(!_economyFood.Text.Contains("Last meal: 8/8 portions eaten") || !_economyFood.Text.Contains("3 berries · 3 vegetables · 2 bread"))
                    throw new Exception("Actual meal missing from Economy");
                await Capture($"artifacts/f14b-meal-economy-{width}.png"); CloseDrawer();
            }
            GD.Print("PASS: happiness reasons, actual meal portions in inspector/Economy, village average and paused inspection at wide/narrow sizes.");
        }
        finally { GetWindow().Size=size; AdoptWorld(previous); _paused=true; }
    }
}

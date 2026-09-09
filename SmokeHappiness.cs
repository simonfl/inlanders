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
                if(!_happinessButton.Text.Contains("Settling in") || !_happinessReasons.Text.Contains("Food choice") || !_happinessButton.Text.Contains("40/100"))
                    throw new Exception("Happiness reasons missing");
                string saved=_world.SaveJson(); await Frames();
                if(saved!=_world.SaveJson()) throw new Exception("Paused mood changed");
                _happinessReasons.Show(); _inspectionScroll.EnsureControlVisible(_happinessButton); await Frames();
                await Capture($"artifacts/f14-inspector-{width}.png"); _happinessReasons.Hide();
                ToggleDrawer(0); await Frames();
                if(!_staffing.Text.Contains("happiness: 40/100")) throw new Exception("Village happiness missing");
                CloseDrawer();
            }
            GD.Print("PASS: happiness reasons, village average and paused inspection at wide/narrow sizes.");
        }
        finally { GetWindow().Size=size; AdoptWorld(previous); _paused=true; }
    }
}

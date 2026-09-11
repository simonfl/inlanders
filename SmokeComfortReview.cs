using Godot;
using Inlanders.Simulation;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async Task CheckComfortReview()
    {
        async Task Frames() { for(int i=0;i<4;i++) await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame); }
        foreach(string housing in new[]{"cottages","lodges"})
        foreach(string state in new[]{"ordinary","improve"})
        {
            AdoptWorld(World.LoadJson(File.ReadAllText($"artifacts/comfort-compact-{housing}-{state}-world.json")));
            _paused=true;CloseManagementUi();ToggleWatch();ToggleCleanWatch();
            var home=_world.Cottages.First(c=>Buildings.Get(c.Kind).Beds>0);
            _focus=new(home.Cell.X,0,home.Cell.Z);_camera.Size=18;UpdateCamera();
            string saved=_world.SaveJson();
            foreach(int width in new[]{960,1440})
            foreach(int direction in new[]{0,1,2,3})
            {
                GetWindow().Size=new(width,width==960?640:900);_angle=direction*Mathf.Pi/2;UpdateCamera();await Frames();
                await Capture($"artifacts/comfort-review-{housing}-{state}-{width}-{direction}.png");
                if(_world.SaveJson()!=saved) throw new Exception("Comfort camera review changed settlement");
            }
            ExitWatch();
        }
        GD.Print("PASS: ordinary/improved cottage and lodge captures at 960/1440, four directions, paused state unchanged. Aesthetic acceptance remains a player decision.");
    }
}

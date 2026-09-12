using Godot;
using Inlanders.Simulation;
using System;
using System.IO;
using System.Threading.Tasks;

public partial class Game
{
    private async Task CheckRestorationViews()
    {
        GetWindow().Size=new(1440,900);_goldenHour=false;ApplyAtmosphere();_showWorldLabels=false;ApplyWorldLabels();
        foreach(string name in new[]{"start","crossing-stage","paved-detour","crossing-overlap"})
        {
            string json=File.ReadAllText($"artifacts/restoration/{name}.json");
            AdoptWorld(World.LoadJson(json));_paused=true;CloseManagementUi();_noticeUntil=0;
            _focus=new(4,0,0);_camera.Size=32;_angle=.72f;UpdateCamera();
            for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
            if(_world.SaveJson()!=json)throw new Exception("Restoration rendering changed snapshot");
            await Capture($"artifacts/restoration/{name}.png");
        }
        GD.Print("PASS: restoration prototype initial/crossing/paved/completed views with unchanged saved state.");
    }
}

using Godot;
using Inlanders.Simulation;
using System;
using System.Threading.Tasks;

public partial class Game
{
    private async Task CheckFinalePrototype()
    {
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        foreach(string name in new[]{"initial","central-prebuildFalse-earlyFalse-poorFalse","local-prebuildFalse-earlyFalse-poorFalse","poor-bakery","celebrated-local-prebuildFalse-earlyFalse-poorTrue"})
        {
            AdoptWorld(World.LoadFile($"artifacts/finale-{name}.json"));_paused=true;CloseManagementUi();_noticeUntil=0;
            foreach(int width in new[]{960,1440})
            {
                GetWindow().Size=new(width,width==960?640:900);FrameMap();await Frames();
                string saved=_world.SaveJson();await Frames();
                if(_world.SaveJson()!=saved)throw new Exception("Paused finale prototype changed state");
                await Capture($"artifacts/finale-{name}-{width}.png");
            }
            _focus=new(2,0,3);_camera.Size=22;_angle=.72f;UpdateCamera();await Frames();
            await Capture($"artifacts/finale-{name}-center.png");
        }
        GD.Print("PASS: finale map/route/recovery render captures at 960/1440 and paused state isolation. Prototype only; no campaign integration.");
    }
}

using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
public partial class Game
{
    private async void RunWorldPickingSmoke()
    {
        try
        {
            async System.Threading.Tasks.Task Frames(){for(int i=0;i<8;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
            AdoptWorld(World.NewRiverFrontage(false,true,true));_paused=true;CloseManagementUi();
            var home=_world.Cottages.First(c=>c.Kind==BuildingKind.Cottage);_focus=BuildingPosition(home);_angle=.72f+Mathf.Pi;UpdateCamera();string before=_world.SaveJson();
            foreach(int width in new[]{960,1440})foreach(float zoom in new[]{19f,36f})
            {
                GetWindow().Size=new(width,width==960?640:900);_camera.Size=zoom;UpdateCamera();ClearSelection();await Frames();
                var roof=_camera.UnprojectPosition(BuildingPosition(home)+Vector3.Up*2.1f);await Click(roof);await Frames();
                if(_workCardSite!=home.Id || _dailyPerson>=0)throw new Exception($"Roof click missed visible home at {width}/{zoom}: site={_workCardSite}, resident={_dailyPerson}");
                await Capture($"artifacts/195-roof-{width}-{zoom}.png");
                ClearSelection();await Frames();var resident=_world.People.First(p=>p.HomeId==home.Id);var point=_camera.UnprojectPosition(PresentedPerson(resident.Id)+Vector3.Up*.65f);
                await Click(point);await Frames();await Capture($"artifacts/195-resident-{width}-{zoom}.png");if(_dailyPerson!=resident.Id)throw new Exception($"Actual resident click failed {width}/{zoom}");
                if(_world.SaveJson()!=before)throw new Exception("Inspection changed village");
            }
            ClearSelection();_angle=.72f;_camera.Size=19;UpdateCamera();await Frames();
            var hidden=_world.People.First(p=>p.HomeId==home.Id);await Click(_camera.UnprojectPosition(PresentedPerson(hidden.Id)+Vector3.Up*.65f));await Frames();
            if(_workCardSite!=home.Id || _dailyPerson>=0)throw new Exception("Hidden resident stole visible roof click");
            GD.Print("PASS visible roof/home and resident picking at two zooms and960/1440; actual contextual cards, roof occlusion and unchanged world.");GetTree().Quit();
        }
        catch(Exception e){GD.PrintErr(e);GetTree().Quit(1);}
    }
}

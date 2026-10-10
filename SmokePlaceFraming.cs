using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
public partial class Game
{
    private async void RunPlaceFramingSmoke()
    {
        try
        {
            async System.Threading.Tasks.Task Frames(){for(int i=0;i<6;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
            var w=World.NewRiverFrontage(false,true,true);
            var at=(from c in w.Map.Land from r in Enumerable.Range(0,4) where w.PreviewGroundPreparation(c,r,BuildingKind.SeatingGarden)!=null select(c,r)).First();
            w.PlanPreparedAddition(at.c,at.r,BuildingKind.SeatingGarden);
            for(int i=0;i<8000 && !w.People.Any(p=>p.LeisureSiteId is int id && w.Cottages.Any(c=>c.Id==id && c.Cell==at.c && c.Complete));i++)w.Tick(.1f);
            var site=w.Cottages.Single(c=>c.Cell==at.c && c.Kind==BuildingKind.SeatingGarden);
            AdoptWorld(w);_paused=true;_showWorldLabels=false;ApplyWorldLabels();CloseManagementUi();GetWindow().Size=new(960,640);_focus=BuildingPosition(site);_camera.Size=18;UpdateCamera();await Frames();
            ShowWorkplaceCard(site.Id);await Frames();var before=_focus;float zoom=_camera.Size;await UiClick(_workCardWatchPlace);await Frames();
            var center=_camera.UnprojectPosition(BuildingPosition(site));GD.Print($"WATCH place {site.Cell}, screen {center}, viewport {_hud.Size}, zoom {_camera.Size}");
            await Capture("artifacts/199-watch-place-after.png");
            if(center.X<_hud.Size.X*.2f || center.X>_hud.Size.X*.8f || center.Y<_hud.Size.Y*.25f || center.Y>_hud.Size.Y*.7f)throw new Exception("Chosen place pushed to the edge of its Watch view");
            if(!_watching)throw new Exception("Watch action failed");
            _paused=false;_speed=3;float started=_uiTime;while(_uiTime-started<20 && !w.People.Any(p=>p.LastLeisureSiteId==site.Id))await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);_paused=true;
            if(!w.People.Any(p=>p.LastLeisureSiteId==site.Id))throw new Exception("Framed place never actually used");
            await Press(Key.Escape);await Frames();if(_workCardSite!=site.Id || _focus.DistanceTo(before)>.001f || _camera.Size!=zoom)throw new Exception("Return lost inspected place or view");
            GetWindow().Size=new(1440,900);await Frames();await UiClick(_workCardWatchPlace);await Frames();
            center=_camera.UnprojectPosition(BuildingPosition(site));if(center.Y<_hud.Size.Y*.25f || center.Y>_hud.Size.Y*.7f)throw new Exception("Wide Watch loses destination");
            await Capture("artifacts/199-watch-place-1440.png");await Press(Key.Escape);await Frames();
            w.Validate();GD.Print("PASS watch actual place through completed visit and return to original inspection.");GetTree().Quit();
        }
        catch(Exception e){GD.PrintErr(e);GetTree().Quit(1);}
    }
}

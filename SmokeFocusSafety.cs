using Godot;
using Inlanders.Simulation;
using System;
public partial class Game
{
    private async void RunFocusSafetySmoke()
    {
        try
        {
            async System.Threading.Tasks.Task Frames(){for(int i=0;i<12;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
            _atmospherePath=System.IO.Path.Combine(System.IO.Path.GetTempPath(),"inlanders-focus-"+Guid.NewGuid()+".cfg");
            AdoptWorld(World.NewRiverFrontage(false,true,true));_pauseInBackground=true;_paused=false;_speed=6;await Frames();
            _Notification((int)NotificationWMWindowFocusOut);string saved=_world.SaveJson();await Frames();
            if(!_paused || _world.SaveJson()!=saved)throw new Exception("Village advanced while the player was in another app");
            _Notification((int)NotificationWMWindowFocusIn);await Frames();if(!_paused)throw new Exception("Focus return resumed without player action");
            await Press(Key.Space);await Frames();if(_paused || _world.SaveJson()==saved)throw new Exception("Deliberate resume failed");
            BeginWoodlandCare();_careDragging=true;_pathStroke=_woodlandStroke=true;string proposal=_world.SaveJson();
            _Notification((int)NotificationWMWindowFocusOut);await Frames();if(!_paused || _careActive || _pathStroke || _woodlandStroke || _world.SaveJson()!=proposal)throw new Exception("Interrupted proposal changed village or kept gesture");
            _pauseInBackground=false;SaveAtmosphere();_pauseInBackground=true;LoadAtmosphere();if(_pauseInBackground)throw new Exception("Background preference did not persist");
            _paused=false;float time=_world.Food.Time;_Notification((int)NotificationWMWindowFocusOut);await Frames();if(_paused || _world.Food.Time<=time)throw new Exception("Explicit background-running choice ignored");
            _paused=true;_pauseInBackground=true;ApplyAtmosphere();await OpenMenu(3);_drawerPages[3].EnsureControlVisible(_backgroundPauseButton);await Frames();await Capture("artifacts/200-background-option.png");
            GD.Print("PASS real frames stop on focus loss, return stays paused, Space resumes, interrupted woodland/paint gestures cancel without mutation, saved opt-out permits actual background life.");GetTree().Quit();
        }
        catch(Exception e){GD.PrintErr(e);GetTree().Quit(1);}
    }
}

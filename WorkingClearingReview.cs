using Godot;
using System;
using System.Threading.Tasks;
public partial class Game
{
    private async Task ProbeWorkingClearing()
    {
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        ShowMainMenu();await Frames();await UiClick(_mainButtons["Play"]);await Frames();await UiClick(_mainButtons["Tend a working clearing"]);await Frames();await CaptureReviewBundle("working-clearing-menu");
        await UiClick(_mainButtons["New clearing"]);await Frames();if(_world.PublicPlace?.WorkingClearing!=true || _world.Population!=8)throw new Exception("Wrong clearing entry");
        await ProbeHousehold();string before=_world.SaveJson();await Press(Key.F5);await Press(Key.F9);await Frames();if(before!=_world.SaveJson())throw new Exception("Clearing save identity");
        _paused=false;_speed=6;double began=_uiTime;while(_uiTime-began<12)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);_paused=true;_world.Validate();await CaptureReviewBundle("working-clearing-life");
        GD.Print("PASS working clearing actual menu, household controls, save identity and ordinary life");
    }
}

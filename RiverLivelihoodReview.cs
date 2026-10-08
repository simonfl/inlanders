using Godot;
using System;
using System.Threading.Tasks;
public partial class Game
{
    private async Task ProbeRiverLivelihood()
    {
        async Task Frames(){for(int i=0;i<6;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        ShowMainMenu();await Frames();await UiClick(_mainButtons["Play"]);await Frames();await UiClick(_mainButtons["Establish life by the river"]);await Frames();await CaptureReviewBundle("river-livelihood-menu");
        await UiClick(_mainButtons["New settlement"]);await Frames();
        if(_world.PublicPlace is not {RiverLandscape:true,PlayerFounded:true} || _world.Housed!=12 || _world.Cottages.Count!=6)throw new Exception("Wrong livelihood opening");
        await ProbeHousehold();string saved=_world.SaveJson();await Press(Key.F5);await Press(Key.F9);await Frames();if(saved!=_world.SaveJson())throw new Exception("Livelihood save differs");
        await Press(Key.H);await Frames();await CaptureReviewBundle("river-livelihood-watch");await Press(Key.Escape);await Frames();if(_watching)throw new Exception("Watch exit failed");
        GD.Print("PASS river livelihood ordinary menu, homes, current save and Watch.");
    }
}

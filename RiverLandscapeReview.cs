using Godot;
using System;
using System.Threading.Tasks;
public partial class Game
{
    private async Task ProbeRiverLandscape()
    {
        async Task Frames(){for(int i=0;i<6;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        ShowMainMenu();await Frames();await UiClick(_mainButtons["Play"]);await Frames();await CaptureReviewBundle("public-river-beginnings");await UiClick(_mainButtons["Tend a river settlement"]);await Frames();
        await CaptureReviewBundle("river-farmsteads-menu");await UiClick(_mainButtons["New settlement"]);await Frames();
        if(_world.PublicPlace?.RiverLandscape!=true || _world.Housed!=12)throw new Exception("Wrong landscape identity");
        await ProbeHousehold();await Press(Key.F5);string saved=_world.SaveJson();await Press(Key.F9);await Frames();if(saved!=_world.SaveJson())throw new Exception("Landscape save differs");
        await Press(Key.H);await Frames();await CaptureReviewBundle("river-farmsteads-watch");await Press(Key.Escape);await Frames();if(_watching)throw new Exception("Watch exit failed");
        GD.Print("PASS river farmsteads ordinary menu, household controls, current save and Watch.");
    }
}

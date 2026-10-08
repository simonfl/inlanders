using Godot;
using System;
using System.Threading.Tasks;
public partial class Game
{
    private async Task ProbeRiverLandscape()
    {
        async Task Frames(){for(int i=0;i<6;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        ShowMainMenu();await Frames();await UiClick(_mainButtons["Play"]);await Frames();await UiClick(_mainButtons["Shape an inhabited hamlet"]);await Frames();await UiClick(_mainButtons["Landscape: inlet · change"]);await Frames();
        await UiClick(_mainButtons["Layout: scattered frontage · change"]);await Frames();await CaptureReviewBundle("river-farmsteads-menu");await UiClick(_mainButtons["New hamlet"]);await Frames();
        if(_world.PublicPlace?.RiverLandscape!=true || _world.Housed!=12)throw new Exception("Wrong landscape identity");
        await ProbeHousehold();string saved=_world.SaveJson();await Press(Key.F5);await Press(Key.F9);await Frames();if(saved!=_world.SaveJson())throw new Exception("Landscape save differs");
        await Press(Key.H);await Frames();await CaptureReviewBundle("river-farmsteads-watch");await Press(Key.Escape);await Frames();if(_watching)throw new Exception("Watch exit failed");
        GD.Print("PASS river farmsteads ordinary menu, household controls, current save and Watch.");
    }
}

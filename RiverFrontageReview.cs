using Godot;
using Inlanders.Simulation;
using System;
using System.Threading.Tasks;
public partial class Game
{
    private async Task ProbeRiverFrontage()
    {
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        if(_reviewRequest!.RootElement.GetProperty("scenario").GetString()=="river-hamlet")
        {
            ShowMainMenu();await Frames();await UiClick(_mainButtons["Play"]);await Frames();await UiClick(_mainButtons["Shape an inhabited hamlet"]);await Frames();
            await UiClick(_mainButtons["Landscape: inlet · change"]);await Frames();await CaptureReviewBundle("inhabited-frontage-menu");
            await UiClick(_mainButtons["New hamlet"]);await Frames();
            if(_world.PublicPlace is not {RiverFrontage:true,PlayerFounded:false} || _world.Housed!=12)throw new Exception("Inhabited frontage entry failed");
            await ProbeHousehold();
            CloseManagementUi();_paused=true;var origin=_focus;float zoom=_camera.Size;string before=_world.SaveJson();
            await OpenMenu(2);await Frames();await UiClick(_landSurvey);await Frames();
            foreach(var button in _landViewButtons){await UiClick(button);await Frames();if(!_surveying || !_paused || before!=_world.SaveJson())throw new Exception("Land views changed simulation");}
            await CaptureReviewBundle("look-over-the-land");await Press(Key.Escape);await Frames();
            if(_surveying || _focus!=origin || _camera.Size!=zoom || before!=_world.SaveJson())throw new Exception("Land survey return lost view/state");
            GD.Print("PASS: actual land views preserve village/clock and return to original camera.");return;
        }
        _frontageStart=false;ShowMainMenu();await Frames();await UiClick(_mainButtons["Play"]);await Frames();
        await UiClick(_mainButtons["Establish a farmstead"]);await Frames();
        await UiClick(_mainButtons["Landscape: inlet · change"]);await Frames();
        await CaptureReviewBundle("frontage-menu");
        await UiClick(_mainButtons["New farmstead"]);await Frames();
        if(_world.PublicPlace?.RiverFrontage!=true)throw new Exception("Frontage menu opened wrong world");
        string saved=_world.SaveJson();await Press(Key.F5);await Press(Key.F9);await Frames();
        if(_world.SaveJson()!=saved || _world.PublicPlace?.RiverFrontage!=true)throw new Exception("Frontage save identity lost");
        await ProbePlayerFounded();
        GD.Print("PASS: actual river-frontage menu, exact save identity, ordinary home/garden construction, meals and rest.");
    }
}

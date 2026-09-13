using Godot;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
public partial class Game
{
    private async Task ProbeSettlementResume()
    {
        async Task Frames(){for(int i=0;i<4;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        for(int i=0;i<8;i++)
        {
            string before=_world.SaveJson();await OpenMenu(2);await Frames();await UiClick(_nextSettlement);await Frames();
            await UiClick(_mainButtons["Resume settlement"]);await Frames();
            if(_atMainMenu || !_hud.IsVisibleInTree() || !_paused || _world.Neighborhood==null || _world.SaveJson()!=before)
            {await CaptureReviewBundle("resume-failed");throw new Exception("Resume failed on iteration "+i+": "+_menuMessage.Text);}
        }
        await CaptureReviewBundle("resume-confirmed");GD.Print("PASS: eight completed-settlement leave/resume transitions with active HUD and exact state");
    }
    private void TraceMenuClick(string stage,Button? button=null,Vector2? point=null,string? error=null)
    {
        if(_reviewRequest==null)return;
        var focus=GetViewport().GuiGetFocusOwner();
        File.AppendAllText(Path.Combine(_reviewDirectory,"menu-transitions.jsonl"),JsonSerializer.Serialize(new{
            stage,page=_menuPageTitle,atMainMenu=_atMainMenu,hud=_hud.IsVisibleInTree(),paused=_paused,
            message=_menuMessage?.Text,error,button=button?.Text,rect=button?.GetGlobalRect().ToString(),point=point?.ToString(),
            focus=focus is Button b?b.Text:focus?.Name.ToString(),scroll=_mainScroll?.ScrollVertical,time=_reviewTimer.Elapsed.TotalSeconds})+"\n");
    }
}

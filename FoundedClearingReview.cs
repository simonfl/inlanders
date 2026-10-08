using Godot;
using Inlanders.Simulation;
using System;
using System.Threading.Tasks;
public partial class Game
{
    private async Task ProbeFoundedClearing()
    {
        async Task Frames(){for(int i=0;i<6;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        ShowMainMenu();await Frames();await UiClick(_mainButtons["Play"]);await Frames();await UiClick(_mainButtons["Earlier beginnings"]);await Frames();await UiClick(_mainButtons["Tend a working clearing"]);await Frames();
        await UiClick(_mainButtons["Beginning: inhabited · change"]);await Frames();await CaptureReviewBundle("matched-clearing-menu");
        foreach(bool relaxed in new[]{false,true})
        {
            await UiClick(_mainButtons[relaxed?"New relaxed clearing":"New clearing"]);await Frames();
            Check(_world.PublicPlace is {WorkingClearing:true,PlayerFounded:true} && _world.Creative==relaxed && _world.Cottages.Count==0 && _firstPlace.Visible,"Matched open clearing entry wrong");
            Check(_firstPlace.GetGlobalRect().End.Y<_hud.Size.Y-76,"Opening choices overflow");
            string before=_world.SaveJson();await UiClick(_firstPlaceChoices[BuildingKind.Cottage]);await Frames();await Press(Key.Escape);await Frames();Check(before==_world.SaveJson(),"Cancelled opening changed world");
            await Press(Key.F5);await Press(Key.F9);await Frames();Check(before==_world.SaveJson(),"Matched clearing save differs");await CaptureReviewBundle(relaxed?"matched-open-relaxed":"matched-open-normal");
            if(!relaxed){ReturnToMainMenu();await Frames();await UiClick(_mainButtons["Play"]);await Frames();await UiClick(_mainButtons["Earlier beginnings"]);await Frames();await UiClick(_mainButtons["Tend a working clearing"]);await Frames();}
        }
        GD.Print("PASS matched clearing actual menu, both modes, first-choice cancellation and current saves.");
    }
}

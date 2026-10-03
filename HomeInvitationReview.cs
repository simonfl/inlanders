using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;
public partial class Game
{
    private async Task ProbeHomeInvitation()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        var home=_world.Cottages.Single(c=>c.Cell==new Cell(1,-5));int first=_world.Population;
        CloseManagementUi();_focus=BuildingPosition(home);_camera.Size=17;UpdateCamera();await Frames();
        await Click(_camera.UnprojectPosition(OnGround(home.Cell.X,home.Cell.Z)));await Frames();
        await UiClick(_homeOptionsButton);await Frames();
        Check(_workCardSite==home.Id && _homeInvite.IsVisibleInTree() && !_homeInvite.Disabled,"Empty home invitation not accessible by world click");
        Check(_workCard.GetGlobalRect().End.Y<_hud.Size.Y-76,"Invitation card overflows compact view");await CaptureReviewBundle("home-invitation-choice");
        await UiClick(_homeInvite);await Frames();Check(_world.Population==first+2 && _world.People.Skip(first).All(p=>p.HomeId==home.Id) && !_homeInvite.Visible,"Actual invitation failed or remained repeatable");
        await CaptureReviewBundle("chosen-home-arrivals");_paused=false;_speed=6;double start=_uiTime;
        while(_uiTime-start<65 && !_world.People.Skip(first).All(p=>p.RestVisits>0 && _world.Founding!.Settled.Contains(p.Id)))await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
        _paused=true;Check(_world.People.Skip(first).All(p=>p.RestVisits>0 && _world.Founding!.Settled.Contains(p.Id)),"Invited neighbors did not actually eat and rest");
        await CaptureReviewBundle("chosen-home-inhabited");_world.Validate();
        GD.Print("PASS: actual empty-home world selection and optional invitation, correct household, compact card, real meals and home rest at6x.");
    }
}

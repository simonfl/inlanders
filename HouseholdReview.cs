using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;
public partial class Game
{
    private async Task ProbeHousehold()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<6;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        var home=_world.Cottages.First(c=>c.Complete && _world.People.Count(p=>p.HomeId==c.Id)>0);CloseManagementUi();ClearSelection();
        _focus=BuildingPosition(home);_camera.Size=18;UpdateCamera();await Frames();await Click(_camera.UnprojectPosition(BuildingPosition(home)+Vector3.Up*.5f));await Frames();
        Check(_workCardSite==home.Id && _householdPeople.Visible,"Selecting home does not show household");
        var residents=_world.People.Where(p=>p.HomeId==home.Id).ToArray();Check(_householdResidents.Count(b=>b.Visible)==residents.Length,"Household roster wrong");
        Check(_workCard.GetGlobalRect().End.Y<_hud.Size.Y-75,"Household card overflows");string saved=_world.SaveJson();await CaptureReviewBundle("household-at-a-glance");
        await UiClick(_householdResidents[residents.Length-1]);await Frames();Check(_dailyPerson==residents[^1].Id && _dailyExpanded && _followPerson && _dailyHomeBack.Visible,"Cannot follow chosen household resident");
        await CaptureReviewBundle("chosen-household-journey");await UiClick(_dailyHomeBack);await Frames();Check(_workCardSite==home.Id && _world.SaveJson()==saved,"Household return changed simulation");
        await CaptureReviewBundle("back-to-household");GD.Print("PASS: actual home selection, all household residents, chosen resident/current journey and pure return at compact size.");
    }
}

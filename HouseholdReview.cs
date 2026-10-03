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
        if(_reviewRequest!.RootElement.GetProperty("scenario").GetString()=="essentials")
        {
            CloseManagementUi();_fullBuild=false;ToggleDrawer(1);await Frames();string before=_world.SaveJson();
            Check(_essentials.IsVisibleInTree() && !_buildingCategories.Visible,"Small palette not primary");Check(_drawerPages[1].GetGlobalRect().Encloses(_essentialChoices[BuildingKind.Bridge].GetGlobalRect()),"Everyday choices need scrolling");await CaptureReviewBundle("everyday-building-choices");
            await UiClick(_essentialChoices[BuildingKind.VegetableField]);await Frames();Check(_placing && _buildKind==BuildingKind.VegetableField,"Simple field choice failed");await Press(Key.Escape);await Frames();
            ToggleDrawer(1);await Frames();await UiClick(_buildBreadth);await Frames();Check(_buildingCategories.Visible && !_essentials.Visible,"Full catalogue inaccessible");await CaptureReviewBundle("full-catalogue-retained");
            Check(_world.SaveJson()==before,"Palette mutated village");CloseDrawer();
        }
        var home=_world.Cottages.First(c=>c.Complete && _world.People.Count(p=>p.HomeId==c.Id)>0);CloseManagementUi();ClearSelection();
        _focus=BuildingPosition(home);_camera.Size=18;UpdateCamera();await Frames();await Click(_camera.UnprojectPosition(BuildingPosition(home)+Vector3.Up*.5f));await Frames();
        Check(_workCardSite==home.Id && _householdPeople.Visible,"Selecting home does not show household");
        var residents=_world.People.Where(p=>p.HomeId==home.Id).ToArray();Check(_householdResidents.Count(b=>b.Visible)==residents.Length,"Household roster wrong");
        Check(_workCard.GetGlobalRect().End.Y<_hud.Size.Y-75,"Household card overflows");string saved=_world.SaveJson();await CaptureReviewBundle("household-at-a-glance");
        await UiClick(_householdResidents[residents.Length-1]);await Frames();Check(_dailyPerson==residents[^1].Id && _dailyExpanded && _followPerson && _dailyHomeBack.Visible,"Cannot follow chosen household resident");
        await CaptureReviewBundle("chosen-household-journey");await UiClick(_dailyHomeBack);await Frames();Check(_workCardSite==home.Id && _world.SaveJson()==saved,"Household return changed simulation");
        await CaptureReviewBundle("back-to-household");
        if(_reviewRequest!.RootElement.GetProperty("scenario").GetString()=="household-move")
        {
            Check(!_householdChange.Visible && !_workCardDetails.Visible,"Secondary home actions exposed by default");await UiClick(_homeOptionsButton);await Frames();
            var other=_world.Cottages.First(c=>c.Id!=home.Id && c.Complete && Buildings.Get(c.Kind).Beds>0);var others=_world.People.Where(p=>p.HomeId==other.Id).Select(p=>p.Id).ToArray();
            _focus=BuildingPosition(other);_camera.Size=20;UpdateCamera();await Frames();
            await UiClick(_householdChange);await Frames();var point=_camera.UnprojectPosition(BuildingPosition(other));await Click(point);await Frames();Check(_householdTo==other.Id && !_householdConfirm.Disabled,"Home choice not actionable");
            await CaptureReviewBundle("household-exchange-proposal");await Press(Key.Escape);await Frames();Check(_world.SaveJson()==saved,"Cancelled household exchange changed state");
            await UiClick(_homeOptionsButton);await Frames();await UiClick(_householdChange);await Frames();await Click(point);await Frames();await UiClick(_householdConfirm);await Frames();Check(residents.All(p=>p.HomeId==other.Id) && others.All(id=>_world.People[id].HomeId==home.Id),"Households did not exchange");_world.Validate();await CaptureReviewBundle("chosen-households");
        }
        GD.Print("PASS: actual home selection, all household residents, chosen resident/current journey and pure return at compact size.");
    }
}

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
            Check(_essentials.IsVisibleInTree() && !_buildingCategories.Visible,"Small palette not primary");Check(_drawerPages[1].GetGlobalRect().Encloses(_gatherTimber.GetGlobalRect()),"Everyday choices need scrolling");await CaptureReviewBundle("everyday-building-choices");
            await UiClick(_essentialChoices[BuildingKind.VegetableField]);await Frames();Check(_placing && _buildKind==BuildingKind.VegetableField,"Simple field choice failed");await Press(Key.Escape);await Frames();
            ToggleDrawer(1);await Frames();await UiClick(_buildBreadth);await Frames();Check(_buildingCategories.Visible && !_essentials.Visible,"Full catalogue inaccessible");await CaptureReviewBundle("full-catalogue-retained");
            Check(_world.SaveJson()==before,"Palette mutated village");
            await UiClick(_buildBreadth);await Frames();await UiClick(_gatherTimber);await Frames();
            var tree=_world.Trees.Where(t=>t.Logs>0 && _world.ClearingProblem(t.Cell)==null).OrderBy(t=>t.Cell.Point.LengthSquared()).First();
            _focus=OnGround(tree.Cell.X,tree.Cell.Z);_camera.Size=18;UpdateCamera();await Frames();var tp=_camera.UnprojectPosition(OnGround(tree.Cell.X,tree.Cell.Z));Input.ParseInputEvent(new InputEventMouseMotion{Position=tp,GlobalPosition=tp});await Frames();
            Check(_hint.Text.Contains("shared workers"),"Clearing wrongly asks for manual staffing");await Click(tp);await Frames();Check(tree.ClearRequested,"Timber not ordered");await Click(tp);await Frames();Check(!tree.ClearRequested,"Timber order not cancelled");
            await Click(tp);await Press(Key.Escape);_paused=false;_speed=6;double started=_uiTime;
            while(_uiTime-started<45 && _world.Trees.Contains(tree))await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
            _paused=true;Check(!_world.Trees.Contains(tree),"Marked timber never collected and cleared by shared labor");await CaptureReviewBundle("timber-ground-cleared");CloseDrawer();
        }
        CloseManagementUi();string navigationState=_world.SaveJson();
        Check(_menuButtons.Where(b=>b.Visible).Count()==2,"Public navigation is not reduced");
        foreach(int page in new[]{0,4,3}){await OpenMenu(page);await Frames();Check(_drawer.Visible && _tabs.CurrentTab==page,"Village secondary navigation lost page");await Press(Key.Escape);await Frames();}
        Check(_world.SaveJson()==navigationState,"Navigation changed village");
        var home=_world.Cottages.First(c=>c.Complete && _world.People.Count(p=>p.HomeId==c.Id)>0);CloseManagementUi();ClearSelection();
        _focus=BuildingPosition(home);_camera.Size=18;UpdateCamera();await Frames();await Click(_camera.UnprojectPosition(BuildingPosition(home)+Vector3.Up*.5f));await Frames();
        Check(_workCardSite==home.Id && _householdPeople.Visible,"Selecting home does not show household");
        var residents=_world.People.Where(p=>p.HomeId==home.Id).ToArray();Check(_householdResidents.Count(b=>b.Visible)==residents.Length,"Household roster wrong");
        Check(!_workCardMove.Visible && !_workCardYard.Visible && !_workCardFurnish.Visible,"Home alterations compete with everyday use");
        Check(_workCard.GetGlobalRect().End.Y<_hud.Size.Y-75,"Household card overflows");string saved=_world.SaveJson();await CaptureReviewBundle("household-at-a-glance");
        await UiClick(_householdResidents[residents.Length-1]);await Frames();Check(_dailyPerson==residents[^1].Id && _dailyExpanded && _followPerson && _dailyHomeBack.Visible,"Cannot follow chosen household resident");
        await CaptureReviewBundle("chosen-household-journey");await UiClick(_dailyHomeBack);await Frames();Check(_workCardSite==home.Id && _world.SaveJson()==saved,"Household return changed simulation");
        await CaptureReviewBundle("back-to-household");
        var workplace=_world.Cottages.FirstOrDefault(c=>c.Complete && World.ProductionOutput(c.Kind)!=null);
        if(workplace!=null){ShowDailyLife(residents[0].Id,workplace.Id);await Frames();Check(_dailyHomeBack.Visible && _dailyHomeBack.Text.Contains(BuildingName(workplace.Kind)),"Origin workplace return missing");await UiClick(_dailyHomeBack);await Frames();Check(_workCardSite==workplace.Id && !_inspector.Visible && _world.SaveJson()==saved,"Return opened wrong place or mutated world");if(_workCardSupply.Visible){await UiClick(_workCardSupply);await Frames();Check(_showFoodMap && !_workCard.Visible && _foodMapExit.IsVisibleInTree(),"Food view stacks inspector or lacks exit");await CaptureReviewBundle("food-place-and-supply");await UiClick(_foodMapExit);await Frames();Check(!_showFoodMap && _world.SaveJson()==saved,"Supply view mutated state or failed to close");}ShowWorkplaceCard(home.Id);await Frames();}

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

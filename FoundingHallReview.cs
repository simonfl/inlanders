using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
public partial class Game
{
    private async Task ProbeFoundingHall()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        async Task Click(Button b){_drawerPages[2].EnsureControlVisible(b);await Frames();await UiClick(b,6);await Frames();}
        ToggleDrawer(2);await Frames();Check(_hallBegin.IsVisibleInTree(),"Continuation hidden");
        await Click(_hallBegin);Check(_world.Founding!.HallProject==1 && _foundingHallGoals.IsVisibleInTree() && _hallFinish.Disabled,"Begin project failed");
        await CaptureReviewBundle("hall-project-brief");
        var buttons=_foundingHallGoals.GetChildren().OfType<Button>().ToArray();
        await Click(buttons[0]);Check(!_drawer.Visible && Math.Abs(_focus.X-15)<.1,"Stone focus failed");ToggleDrawer(2);await Frames();
        await Click(buttons[1]);Check(_placing && _buildKind==BuildingKind.Quarry,"Quarry shortcut failed");await Press(Key.Escape);
        // Ordinary construction commands; accelerated simulation driver, not human play.
        Cottage Place(BuildingKind kind,Cell at)
        {
            var plan=_world.Map.Land.SelectMany(c=>Enumerable.Range(0,4).Select(r=>new{c,r})).OrderBy(p=>(p.c.Point-at.Point).LengthSquared()).ThenBy(p=>p.c.Z).ThenBy(p=>p.c.X).ThenBy(p=>p.r).First(p=>_world.PlacementProblem(p.c,p.r,kind)==null);
            return _world.Place(plan.c,plan.r,kind)!;
        }
        Place(BuildingKind.Quarry,new(15,1));Place(BuildingKind.Sawmill,new(-6,5));var hall=Place(BuildingKind.GatheringHall,new(-2,3));
        for(int i=0;i<9000 && !_world.People.Any(p=>p.SiteId==hall.Id && p.Task==Work.ToMaterials);i++)_world.Tick(.1f);
        Check(_world.People.Any(p=>p.SiteId==hall.Id && p.Task==Work.ToMaterials),"No hall pickup observed");
        CloseDrawer();SelectBuilding(hall.Id);UpdateHud();await Frames();
        Check(_siteInfo.Text.Contains("Collecting:") && _siteInfo.Text.Contains("central storage"),"Live material source missing");
        await CaptureReviewBundle("hall-material-pickups");ClearSelection();
        SaveWorld();string active=_world.SaveJson();await Press(Key.F9);await Frames();Check(_world.SaveJson()==active && _world.Founding!.HallProject==1,"Active project restore differs");
        for(int i=0;i<18000 && _world.FinishFoundingHallProblem()!=null;i++)_world.Tick(.1f);
        Check(_world.FinishFoundingHallProblem()==null,"Hall never used");_world.Validate();UpdateHud();CloseDrawer();ToggleDrawer(2);await Frames();
        await Click(_hallFinish);Check(_world.Founding!.HallProject==2 && _paused,"Ending failed");
        _focus=OnGround(hall.Cell.X,hall.Cell.Z);UpdateCamera();await CaptureReviewBundle("hall-project-finished");SaveWorld();
        // Completion keeps ordinary growth available, with supply evidence at the decision.
        var home=Place(BuildingKind.Cottage,new(-7,8));
        for(int i=0;i<9000 && (!home.Complete || _world.InvitationProblem()!=null);i++)_world.Tick(.1f);
        UpdateHud();CloseDrawer();ToggleDrawer(2);await Frames();
        Check(_foundingInvite.IsVisibleInTree() && !_foundingInvite.Disabled && _foundingFood.Text.Contains("Recent deliveries:"),"Post-project growth controls missing");
        await Click(_foundingInvite);Check(_world.Population==14 && _world.Founding!.HallProject==2,"Completed project blocks growth");
        await CaptureReviewBundle("hall-continued-growth");
        await Click(_foundingFoodView);Check(_showFoodMap && !_drawer.Visible,"Growth food inspection failed");ToggleFoodMap();
        OpenEconomy();UpdateHud();await Frames();_drawerPages[4].EnsureControlVisible(_foodExpansionLink);await Frames();
        await CaptureReviewBundle("food-recovery-links");await UiClick(_foodWorkplaceLink);await Frames();
        Check(_tabs.CurrentTab==1 && _buildSection==2 && _buildingFilter.Selected==2,"Food workplaces link lost filter");
        var hut=_world.Cottages.First(c=>c.Kind==BuildingKind.ForagerHut);_drawerPages[1].EnsureControlVisible(_queueButtons[hut.Id]);await Frames();await UiClick(_queueButtons[hut.Id]);await Frames();
        Check(_selectedSite==hut.Id && _productionControls.Visible,"Food workplace inspector missing");
        ClearSelection();OpenEconomy();_drawerPages[4].EnsureControlVisible(_foodExpansionLink);await Frames();await UiClick(_foodExpansionLink);await Frames();
        Check(_tabs.CurrentTab==1 && _buildSection==0 && _buildingFilter.Selected==2,"Food building choices missing");
        OpenMealCoverage();_serviceFilter.Select(3);UpdateServiceCoverage();await Frames();
        var diner=_world.People.First(p=>p.Meal is {} m && (m.Reserved || m.Carrying || m.Eaten));int? pickup=diner.Meal!.SourceId;
        var sourceButton=_mealSourceLinks[diner.Id];_drawerPages[0].EnsureControlVisible(sourceButton);await Frames();await UiClick(sourceButton);await Frames();
        Check(pickup is int sourceId?_selectedSite==sourceId:_showFoodMap && !_drawer.Visible,"Actual meal pickup link failed");
        ClearSelection();if(_showFoodMap)ToggleFoodMap();
        SaveWorld();string finished=_world.SaveJson();
        ReturnToMainMenu();await Frames();await UiClick(_mainButtons["Continue"]);await Frames();Check(_world.SaveJson()==finished && CurrentSavePath==FoundingPath,"Project Continue differs");
        File.WriteAllText(Path.Combine(_reviewDirectory,"hall-controls.txt"),"PASS: continuation, stone focus, quarry planning, active F9, real use, ending and Continue; accelerated command-driven construction, not human play");
    }
}

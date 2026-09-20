using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;
public partial class Game
{
    private async Task ProbeCultivatedBank()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        ShowMainMenu();await Frames();await UiClick(_mainButtons["Play"]);await Frames();
        await UiClick(_mainButtons["Compare: cultivated bank"]);await Frames();await CaptureReviewBundle("cultivated-bank-choice");
        await UiClick(_mainButtons["New hamlet"]);await Frames();Check(_world.PublicPlace==new HamletProfile(false,true),"Wrong bank profile");
        var home=_world.Cottages.First(c=>c.Kind==BuildingKind.Cottage);
        await Click(_camera.UnprojectPosition(OnGround(home.Cell.X,home.Cell.Z-1)));await Frames();
        Check(_workCard.Visible && _workCardSite==home.Id && !_inspector.Visible,"Home click did not expose compact actions");await CaptureReviewBundle("home-world-actions");
        await UiClick(_workCardWorker);await Frames();Check(_dailyCard.Visible,"Watch resident failed");ClearSelection();
        var staged=_world.Place(new(1,-9),0,BuildingKind.Cottage);Check(staged!=null,"Construction card fixture unavailable");CreateActors();RenderActors(0);
        await Click(_camera.UnprojectPosition(OnGround(1,-10)));await Frames();Check(_workCardSite==staged!.Id && _workCardCancel.Visible,"Construction click missed actions");
        await UiClick(_workCardPause);await Frames();Check(staged.ConstructionPaused,"Card staging failed");await CaptureReviewBundle("construction-world-actions");
        await UiClick(_workCardCancel);await Frames();Check(!_world.Cottages.Contains(staged),"Card cancellation failed");ClearSelection();
        var plot=_world.Cottages.Single(c=>c.Kind==BuildingKind.VegetableGarden && c.Cell.Z==7);
        ShowWorkplaceCard(plot.Id);await Frames();await UiClick(_workCardPause);await Frames();await UiClick(_workCardMove);await Frames();
        await Click(_camera.UnprojectPosition(OnGround(4,-8)));await Frames();Check(plot.Cell==new Cell(4,-8) && plot.WorkPaused,"Cultivated strip move failed");
        Check(_workCard.Visible && !_inspector.Visible,"Move lost compact actions");await CaptureReviewBundle("cultivated-bank-opened");await UiClick(_workCardPause);await Frames();CloseDrawer();ClearSelection();
        BeginPlacement(BuildingKind.Cottage);await Click(_camera.UnprojectPosition(OnGround(1,-9)));await Frames();
        var added=_world.Cottages.Single(c=>c.Cell==new Cell(1,-9));Check(!added.Complete,"Added footprint fixture not staged");
        string unchanged=_world.SaveJson();await Press(Key.G);await Frames();await UiClick(_hamletCompare);await Frames();
        Check(_courtStartingLayout!.Visible && _hamletComparePanel.Visible && unchanged==_world.SaveJson(),"Opening comparison mutated the village");
        Check(_world.Founding!.StartingBuildings.Single(b=>b.Id==plot.Id).Cell==new Cell(5,7),"Move rewrote opening");Check(_courtStartingLayout.GetChildCount()==30,"Comparison omitted new placement or retained unchanged outlines");await CaptureReviewBundle("opening-versus-current");
        await Press(Key.Escape);await Frames();Check(!_courtStartingLayout.Visible && !_hamletComparePanel.Visible,"Comparison escape failed");
        string saved=_world.SaveJson();await Press(Key.F5);await Press(Key.F9);await Frames();Check(saved==_world.SaveJson(),"Bank save differs");
        Reset();await Frames();Check(_world.PublicPlace==new HamletProfile(false,true),"Bank restart changed place");
        ReturnToMainMenu();await Frames();Check(_atMainMenu && _mainButtons.ContainsKey("Play"),"Bank return to menu/save failed: "+_notice);await UiClick(_mainButtons["Play"]);await Frames();await UiClick(_mainButtons["Compare: cultivated bank"]);await Frames();
        await UiClick(_mainButtons["New relaxed hamlet"]);await Frames();Check(_world.PublicPlace==new HamletProfile(true,true) && CurrentSavePath.EndsWith("cultivated-bank-relaxed.json"),"Relaxed bank slot differs");
        await ProbePublicPlaceContract();
        GD.Print("PASS: public bank comparison, actual garden move/replant, exact save, retained restart and relaxed slot (scripted UI).");
    }
}

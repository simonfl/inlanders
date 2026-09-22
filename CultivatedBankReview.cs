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
        await UiClick(_mainButtons["Other starting layouts"]);await Frames();await UiClick(_mainButtons["Compare: cultivated bank"]);await Frames();await CaptureReviewBundle("cultivated-bank-choice");
        await UiClick(_mainButtons["New hamlet"]);await Frames();Check(_world.PublicPlace==new HamletProfile(false,true),"Wrong bank profile");
        var home=_world.Cottages.First(c=>c.Kind==BuildingKind.Cottage);
        await Click(_camera.UnprojectPosition(OnGround(home.Cell.X,home.Cell.Z-1)));await Frames();
        Check(_workCard.Visible && _workCardSite==home.Id && !_inspector.Visible,"Home click did not expose compact actions");await CaptureReviewBundle("home-world-actions");
        var initialFocus=_focus;float initialAngle=_angle,initialZoom=_camera.Size;
        int oldSide=home.YardSide;string beforeYard=_world.SaveJson();await UiClick(_workCardYard);await Frames();
        int targetSide=Enumerable.Range(1,3).Select(i=>(oldSide+i)%4).First(i=>_world.HomeYardProblem(home.Id,i)==null);
        foreach(int side in Enumerable.Range(0,4))
        {
            await UiClick(_yardSides[side]);await Frames();
            Check(_workCardText.Text.Contains("Preview: "+World.YardSideName(side)),"Preview card reports wrong side");
            foreach(var place in _world.YardPlaces(home,side))
            {
                var point=_camera.UnprojectPosition(OnGround(place.X,place.Z,.35f));
                Check(point.X>20 && point.X<_hud.Size.X-346 && point.Y>85 && point.Y<_hud.Size.Y-70,"Proposed furniture outside unobscured viewport");
            }
            await CaptureReviewBundle("yard-side-"+side);
        }
        await UiClick(_yardSides[0]);await Frames();float groundAngle=_angle;
        var groundChoice=_world.YardPlaces(home,targetSide).First();
        await Click(_camera.UnprojectPosition(OnGround(groundChoice.X,groundChoice.Z,.08f)));await Frames();
        Check(_yardPreviewSide==targetSide && _angle==groundAngle && beforeYard==_world.SaveJson(),"Direct yard ground selection changed camera/world or missed candidate");
        await CaptureReviewBundle("yard-ground-click");
        await UiClick(_yardSides[targetSide]);await Frames();Check(home.YardSide==oldSide && _world.SaveJson()==beforeYard && _yardPreviewGround!.Visible,"Preview changed world or is invisible");
        Check(_workCard.GetGlobalRect().End.Y<_hud.Size.Y-60,"Yard preview overlaps bottom controls");await CaptureReviewBundle("yard-ground-preview");
        await Press(Key.H);await Frames();Check(_watching && _yardPreviewSide<0 && _world.SaveJson()==beforeYard,"Watch retained a proposal or changed simulation");
        await Press(Key.Escape);await Frames();Check(!_watching,"One Escape did not leave Watch after yard proposal");
        await UiClick(_workCardYard);await Frames();await UiClick(_yardSides[targetSide]);await Frames();
        await Press(Key.Escape);await Frames();Check(_yardPreviewSide<0 && _world.SaveJson()==beforeYard,"Preview cancellation changed world");
        await UiClick(_workCardYard);await Frames();await UiClick(_yardSides[targetSide]);await Frames();await UiClick(_yardApply);await Frames();Check(home.YardSide==targetSide,"Confirmed yard-side action failed");
        Check(!home.ImprovementRequested && !home.Improved,"Ground-only confirmation ordered furnishing");
        await UiClick(_workCardYard);await Frames();await UiClick(_yardFurnish);await Frames();
        Check(home.YardSide==targetSide && home.ImprovementRequested && !home.Improved,"Priced furnishing did not order real work");
        await CaptureReviewBundle("chosen-yard-side");
        await UiClick(_workCardFurnish);await Frames();Check(!home.ImprovementRequested,"Combined order cannot cancel");
        string beforeWatch=_world.SaveJson();bool wasPaused=_paused;float wasSpeed=_speed;
        await UiClick(_workCardWatchPlace);await Frames();
        Check(_watching && !_hud.Visible && !_followPerson && _world.SaveJson()==beforeWatch && _paused==wasPaused && _speed==wasSpeed,"Watch place changed life or followed a person");
        await CaptureReviewBundle("watch-domestic-place");await Press(Key.Escape);await Frames();
        ShowWorkplaceCard(home.Id);await Frames();
        await UiClick(_workCardWorker);await Frames();Check(_dailyCard.Visible,"Watch resident failed");ClearSelection();_focus=initialFocus;_angle=initialAngle;_camera.Size=initialZoom;UpdateCamera();
        var staged=_world.Place(new(1,-9),0,BuildingKind.Cottage);Check(staged!=null,"Construction card fixture unavailable");CreateActors();RenderActors(0);
        await Click(_camera.UnprojectPosition(OnGround(1,-10)));await Frames();Check(_workCardSite==staged!.Id && _workCardCancel.Visible,"Construction click missed actions");
        await UiClick(_workCardPause);await Frames();Check(staged.ConstructionPaused,"Card staging failed");await CaptureReviewBundle("construction-world-actions");
        await UiClick(_workCardCancel);await Frames();Check(!_world.Cottages.Contains(staged),"Card cancellation failed");ClearSelection();
        var plot=_world.Cottages.Single(c=>c.Kind==BuildingKind.VegetableField && c.Cell.Z==6);
        ShowWorkplaceCard(plot.Id);await Frames();await UiClick(_workCardPause);await Frames();await UiClick(_workCardMove);await Frames();
        await Click(_camera.UnprojectPosition(OnGround(4,-8)));await Frames();Check(plot.Cell==new Cell(4,-8) && plot.WorkPaused,"Cultivated strip move failed");
        Check(_workCard.Visible && !_inspector.Visible,"Move lost compact actions");await CaptureReviewBundle("cultivated-bank-opened");await UiClick(_workCardPause);await Frames();CloseDrawer();ClearSelection();
        BeginPlacement(BuildingKind.Cottage);await Click(_camera.UnprojectPosition(OnGround(1,-9)));await Frames();
        var added=_world.Cottages.Single(c=>c.Cell==new Cell(1,-9));Check(!added.Complete,"Added footprint fixture not staged");
        string unchanged=_world.SaveJson();await Press(Key.G);await Frames();await UiClick(_hamletCompare);await Frames();
        Check(_courtStartingLayout!.Visible && _hamletComparePanel.Visible && unchanged==_world.SaveJson(),"Opening comparison mutated the village");
        Check(_world.Founding!.StartingBuildings.Single(b=>b.Id==plot.Id).Cell==new Cell(5,6),"Move rewrote opening");Check(_courtStartingLayout.GetChildCount()==42,"Comparison omitted new placement or retained unchanged outlines");await CaptureReviewBundle("opening-versus-current");
        await Press(Key.Escape);await Frames();Check(!_courtStartingLayout.Visible && !_hamletComparePanel.Visible,"Comparison escape failed");
        string saved=_world.SaveJson();await Press(Key.F5);await Press(Key.F9);await Frames();Check(saved==_world.SaveJson(),"Bank save differs");
        Reset();await Frames();Check(_world.PublicPlace==new HamletProfile(false,true),"Bank restart changed place");
        ReturnToMainMenu();await Frames();Check(_atMainMenu && _mainButtons.ContainsKey("Play"),"Bank return to menu/save failed: "+_notice);await UiClick(_mainButtons["Play"]);await Frames();await UiClick(_mainButtons["Other starting layouts"]);await Frames();await UiClick(_mainButtons["Compare: cultivated bank"]);await Frames();
        await UiClick(_mainButtons["New relaxed hamlet"]);await Frames();Check(_world.PublicPlace==new HamletProfile(true,true) && CurrentSavePath.EndsWith("cultivated-bank-relaxed.json"),"Relaxed bank slot differs");
        var relaxedHome=_world.Cottages.First(c=>c.Kind==BuildingKind.Cottage);
        ShowWorkplaceCard(relaxedHome.Id);await Frames();await UiClick(_workCardYard);await Frames();await UiClick(_yardSides[3]);await Frames();
        Check(_yardFurnish.Text.Contains("free"),"Relaxed preview misstates price");await UiClick(_yardFurnish);await Frames();Check(relaxedHome.Improved && relaxedHome.YardSide==3,"Relaxed combined furnishing failed");
        var neighbor=_world.Cottages.Single(c=>c.Cell==new Cell(1,6));ShowWorkplaceCard(neighbor.Id);await Frames();await UiClick(_workCardYard);await Frames();await UiClick(_yardSides[1]);await Frames();
        Check(_yardFurnish.Disabled && _yardApply.Disabled && _yardPreviewInfo.Text.Contains("yard uses"),"Overlapping yard preview lacks blocker");await CaptureReviewBundle("blocked-yard-preview");await Press(Key.Escape);await Frames();ClearSelection();
        await ProbeCommonsCard();
        await ProbePublicPlaceContract();
        GD.Print("PASS: public bank comparison, actual garden move/replant, exact save, retained restart and relaxed slot (scripted UI).");
    }
}

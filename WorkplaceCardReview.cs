using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;
public partial class Game
{
    private async Task ProbeWorkplaceCard()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<6;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        var site=_world.Cottages.First(c=>c.Kind==BuildingKind.Bakery && c.Complete);
        async Task Open()
        {
            ClearSelection();CloseDrawer();_focus=OnGround(site.Cell.X,site.Cell.Z);_camera.Size=22;UpdateCamera();await Frames();
            await Click(_camera.UnprojectPosition(OnGround(site.Cell.X,site.Cell.Z-.5f)));await Frames();
            Check(_workCard.Visible && _workCardSite==site.Id && !_inspector.Visible,"World workplace card did not open");
        }
        await Open();string before=_world.SaveJson();await CaptureReviewBundle("world-workplace-card");
        Check(_world.SaveJson()==before,"Workplace reading changed state");
        await UiClick(_workCardPause,6);await Frames();Check(site.WorkPaused && !_workCardMove.Disabled,"Pause/move recovery absent");
        await UiClick(_workCardPause,6);await Frames();Check(!site.WorkPaused,"Resume failed");
        await UiClick(_workCardDetails);await Frames();Check(_inspector.Visible && !_inspectorSecondary.Visible && !_workCard.Visible,"Details transition failed");
        await Open();
        for(int i=0;i<6000 && CardWorker()==null;i++)_world.Tick(.1f);
        RenderActors(0);_nextWorkCard=0;await Frames();var worker=CardWorker();Check(worker!=null,"No actual worker to follow");
        await UiClick(_workCardWorker);await Frames();Check(_dailyPerson==worker!.Id && _followPerson,"Work link did not follow actual worker");
        await Open();
        for(int i=0;i<6000 && CardDiner()==null;i++)_world.Tick(.1f);
        RenderActors(0);_nextWorkCard=0;await Frames();var diner=CardDiner();Check(diner!=null,"No actual meal collector to follow");
        await UiClick(_workCardDiner);await Frames();Check(_dailyPerson==diner!.Id && _followPerson,"Meal link did not follow actual collector");
        await Press(Key.Escape);await Frames();Check(!_workCard.Visible && !_dailyCard.Visible,"Escape left a card open");
        // Ordinary construction supplies one optional domestic project in this same village.
        foreach(var kind in new[]{BuildingKind.Sawmill,BuildingKind.Carpenter})
        {
            var at=_world.Map.Land.First(c=>_world.PlacementProblem(c,0,kind)==null);Check(_world.Place(at,0,kind)!=null,"Workshop rejected");
        }
        for(int i=0;i<4000;i++)_world.Tick(.1f);
        var home=_world.Cottages.First(c=>c.Kind==BuildingKind.Cottage && _world.ImprovementProblem(c.Id)==null);
        SelectBuilding(home.Id);await Frames();await UiClick(_inspectorDetails);await Frames();await UiClick(_comfortOrder);await Frames();Check(home.ImprovementRequested,"Furnish action failed");
        for(int i=0;i<6000 && !home.Improved;i++)_world.Tick(.1f);
        Check(home.Improved,"Carpenter did not furnish home");RenderActors(0);UpdateHud();await Frames();
        await CaptureReviewBundle("normal-furnished-home");await Press(Key.F5);string saved=_world.SaveJson();await Press(Key.F9);await Frames();Check(_world.SaveJson()==saved,"Integrated save differs");
        GD.Print("PASS: world workplace card, held pause/resume, details, actual worker/collector follow, escape, ordinary furnishing and current save.");
    }
}

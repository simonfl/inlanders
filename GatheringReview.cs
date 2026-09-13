using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async Task ProbeGathering()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<4;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        async Task Until(Func<bool> done,string why)
        {for(int i=0;i<2000 && !done();i++){_world.Tick(.1f);if(i%100==0){_world.Validate();RenderActors(0);UpdateHud();await Frames();}}Check(done(),why);RenderActors(0);UpdateHud();await Frames();}
        var site=_world.Cottages.Where(c=>c.Complete && _world.GatheringProblem(c.Entrance)==null)
            .OrderBy(c=>(c.Cell.Point-new Cell(17,4).Point).LengthSquared()).First();
        SelectBuilding(site.Id);UpdateHud();await Frames();
        Check(_gatherHere.Visible && !_gatherHere.Disabled,"Outdoor meal action unavailable");
        await UiClick(_gatherHere);await Frames();await UiClick(_gatherPlanStart);await Frames();Check(_world.Gathering?.Active==true,"Gather button failed");
        await Until(()=>_world.People.Any(p=>p.Meal is {Gathering:true,Carrying:true}),"No carried shared food");
        SelectBuilding(site.Id);UpdateHud();await Frames();await UiClick(_gatherCancel);await Frames();Check(_world.Gathering!.Cancelled,"Cancel button failed");
        await Until(()=>_world.People.All(p=>p.Task!=Work.ReturnMeal),"Cancellation did not return portions");
        await OpenMenu(2);await UiClick(_gatherPlanEntry);await Frames();Check(_gatherPlanning,"Goals did not open place planning");
        string before=_world.SaveJson();await Press(Key.Escape);Check(!_gatherPlanning && _world.SaveJson()==before,"Plan cancellation changed the world");
        await OpenMenu(2);await UiClick(_gatherPlanEntry);await Frames();
        async Task Choose(Cell cell)
        {
            _focus=OnGround(cell.X,cell.Z);UpdateCamera();await Frames();
            var point=_camera.UnprojectPosition(OnGround(cell.X,cell.Z));Check(!PointerOverHud(point),"Ground selection hidden by HUD");
            await Click(point);await Frames();
        }
        await Choose(new(5,3));Check(_gatherPlanStart.Disabled,"Water accepted as gathering center");
        await Choose(new(22,10));Check(!_gatherPlanStart.Disabled,"Open meadow rejected");
        await UiClick(_gatherLayout);await Frames();Check(!_gatherSpread,"Compact layout unavailable");
        await CaptureReviewBundle("gathering-plan-compact");
        await UiClick(_gatherLayout);await Frames();Check(_gatherSpread,"Circle layout unavailable");
        await CaptureReviewBundle("gathering-plan-circle");
        Check(_world.SaveJson()==before,"Preview changed the world");
        await UiClick(_gatherPlanStart);await Frames();Check(_world.Gathering!.Active && _world.Gathering.Center==new Cell(22,10),"Ground plan not committed");
        await Until(()=>_world.Gathering!.Eating,"No simultaneous gathering");
        _focus=OnGround(_world.Gathering!.Center.X,_world.Gathering.Center.Z);UpdateCamera();CloseManagementUi();await Frames();await CaptureReviewBundle("gathering-seated");
        await Press(Key.F5);string saved=_world.SaveJson();await Press(Key.F9);Check(_world.SaveJson()==saved,"Gathering load differs");
        await Until(()=>_world.Gathering!.Complete,"Shared meal did not finish");await CaptureReviewBundle("gathering-complete");
        GD.Print("PASS: outdoor meal controls, physical gathering, cancel/restart, simultaneous seats, save/load and completion.");
    }
}

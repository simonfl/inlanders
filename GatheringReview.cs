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
        await UiClick(_gatherHere);await Frames();Check(_world.Gathering?.Active==true,"Gather button failed");
        await Until(()=>_world.People.Any(p=>p.Meal is {Gathering:true,Carrying:true}),"No carried shared food");
        await UiClick(_gatherCancel);await Frames();Check(_world.Gathering!.Cancelled,"Cancel button failed");
        await Until(()=>_world.People.All(p=>p.Task!=Work.ReturnMeal),"Cancellation did not return portions");
        await UiClick(_gatherHere);await Frames();Check(_world.Gathering!.Active,"Restart failed");
        await Until(()=>_world.Gathering!.Eating,"No simultaneous gathering");
        _focus=OnGround(site.Entrance.X,site.Entrance.Z);UpdateCamera();CloseManagementUi();await Frames();await CaptureReviewBundle();
        await Press(Key.F5);string saved=_world.SaveJson();await Press(Key.F9);Check(_world.SaveJson()==saved,"Gathering load differs");
        await Until(()=>_world.Gathering!.Complete,"Shared meal did not finish");await CaptureReviewBundle();
        GD.Print("PASS: outdoor meal controls, physical gathering, cancel/restart, simultaneous seats, save/load and completion.");
    }
}

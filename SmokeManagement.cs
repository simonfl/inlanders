using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async Task CheckManagementUi()
    {
        void Check(bool ok,string why) { if(!ok) throw new Exception(why); }
        async Task Frames() { for(int i=0;i<4;i++) await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame); }
        var previous=_world; var size=GetWindow().Size;
        AdoptWorld(World.NewCampaign(2)); _paused=true;
        SelectPerson(6); await Frames();
        _jobChoice.Select((int)Role.Sawyer); await UiClick(_assignButton);
        Check(_world.People[6].Role==Role.Sawyer,"Direct assignment failed");
        _jobChoice.Select((int)Role.Unassigned); await UiClick(_assignButton);
        Check(_world.People[6].Role==Role.Unassigned,"Direct unassignment failed");
        await UiClick(_followButton); await Frames();
        Check(_followPerson && _focus.DistanceTo(_people[6].Body.Position)<.01f,"Follow camera failed");
        Input.ParseInputEvent(new InputEventKey { PhysicalKeycode=Key.D, Pressed=true }); await Frames();
        Input.ParseInputEvent(new InputEventKey { PhysicalKeycode=Key.D, Pressed=false }); await Frames();
        Check(!_followPerson,"Manual pan did not stop following");
        var hut=_world.Cottages.Single(c=>c.Kind==BuildingKind.ForagerHut);
        SelectBuilding(hut.Id); await Frames();
        await UiClick(_staffPlus); Check(_world.People.Count(p=>p.Role==Role.Forager)==3,"Workplace staffing failed");
        await UiClick(_staffMinus); Check(_world.People.Count(p=>p.Role==Role.Forager)==2,"Workplace unstaffing failed");
        for(int i=0;i<10;i++) _world.Tick(.1f);
        await Frames();
        var worker=_world.People.First(p=>p.WorkplaceId==hut.Id);
        Check(_workerLinks[worker.Id].Visible,"Active worker link missing");
        GetWindow().Size=new(960,640); await Frames();
        _noticeUntil=0; await Frames(); await Capture("artifacts/f21c-workplace.png");
        await UiClick(_workerLinks[worker.Id]); await Frames();
        Check(_selectedPerson==worker.Id && !_workplaceButton.Disabled,"Worker inspection failed");
        await UiClick(_workplaceButton); await Frames();
        Check(_selectedSite==hut.Id,"Workplace inspection failed");
        SelectPerson(worker.Id); await Frames(); await UiClick(_followButton);
        await Capture("artifacts/f21c-villager.png");
        ClearSelection(); Check(!_followPerson,"Closing selection left follow active");
        AdoptWorld(previous); GetWindow().Size=size; await Frames();
        GD.Print("SMOKE PASS: direct jobs, workplace staffing, worker/workplace links, following/manual pan, and 960px inspection.");
    }
}

using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;
public partial class Game
{
    private async Task ProbeCourt()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        ShowMainMenu();await Frames();await UiClick(_mainButtons["Settlements"]);await Frames();
        await UiClick(_mainButtons["New Willow court"]);await Frames();
        Check(_world.IsArrangementCourt && !_drawer.Visible && _paused,"Court entry should expose village, not Goals");
        _noticeUntil=0;await CaptureReviewBundle("court-opening");
        var p=_world.People[0];
        var point=_camera.UnprojectPosition(_people[p.Id].Body.Position+new Vector3(0,.5f,0));
        await Click(point);await Frames();
        Check(_dailyPerson>=0 && _dailyCard.Visible && !_inspector.Visible,"Resident click did not expose daily life");
        int selected=_dailyPerson;var home=_world.Cottages.Single(c=>c.Id==_world.People[selected].HomeId);var original=home.Cell;
        await CaptureReviewBundle("court-household");
        await UiClick(_dailyMove);await Frames();Check(_movingSite==home.Id,"Home trial not entered");
        var destination=_world.Map.Land.OrderBy(c=>(c.Point-new Cell(1,6).Point).LengthSquared())
            .First(c=>c!=original && _world.RelocationProblem(home.Id,c,home.Rotation)==null);
        _focus=OnGround(destination.X,destination.Z);UpdateCamera();await Frames();
        point=_camera.UnprojectPosition(OnGround(destination.X,destination.Z));
        Input.ParseInputEvent(new InputEventMouseMotion{Position=point,GlobalPosition=point});await Frames();await Click(point);await Frames();
        Check(home.Cell==destination && _world.Neighborhood!.Arrangement!.BuildingId==home.Id && _paused,"Trial placement failed");
        ShowDailyLife(selected);await Frames();await CaptureReviewBundle("court-trial");
        await Press(Key.F5);string saved=_world.SaveJson();await Press(Key.F9);await Frames();Check(_world.SaveJson()==saved,"Trial save/load failed");
        ShowDailyLife(selected);await Frames();await UiClick(_dailyRestore);await Frames();
        Check(_world.Cottages.Single(c=>c.Id==home.Id).Cell==original && _world.Neighborhood!.Arrangement!.BuildingId==null,"Restore failed");
        // Observe an actual request, collection and consumption with ordinary simulation rules.
        bool claimed=false,ate=false;
        for(int i=0;i<2400 && !ate;i++)
        {
            _world.Tick(.1f);
            if(i%20==0){RenderActors(0);UpdateHud();await Frames();}
            var resident=_world.People[selected];
            if(!claimed && resident.Meal is {Reserved:true}){claimed=true;ShowDailyLife(selected);await Frames();await CaptureReviewBundle("court-claimed-journey");}
            ate=_world.Food.MealConsumptions.Any(m=>m.Person==selected);
        }
        Check(claimed && ate,"No actual collection and eating observed");ShowDailyLife(selected);await Frames();await CaptureReviewBundle("court-after-meal");
        bool longerTrip=false;
        for(int i=0;i<1800 && !longerTrip;i++)
        {
            _world.Tick(.1f);
            var resident=_world.People.FirstOrDefault(p=>p.Meal is {Reserved:true} && p.Route.Count>4);
            if(resident!=null){longerTrip=true;selected=resident.Id;}
            if(i%100==0){RenderActors(0);UpdateHud();await Frames();}
        }
        GD.Print(longerTrip?"Observed an actual longer meal collection; source is not prescribed.":"No longer meal collection appeared in this observation; no stock or behavior forced.");
        _noticeUntil=0;_focus=OnGround(3,3);_camera.Size=29;UpdateCamera();RenderActors(0);ShowDailyLife(selected);await Frames();await CaptureReviewBundle("court-later-journey");
        _world.Validate();SaveWorld();saved=_world.SaveJson();ReturnToMainMenu();await Frames();await UiClick(_mainButtons["Continue"]);await Frames();
        Check(_world.IsArrangementCourt && _world.SaveJson()==saved,"Continue lost court");
        Reset();await Frames();Check(_world.IsArrangementCourt && _world.Food.Time==0,"Court reset changed mode");
        GD.Print("PASS: court entry, resident selection, actual meal journey, reversible home placement, saves, Continue and reset (scripted UI)");
    }
}

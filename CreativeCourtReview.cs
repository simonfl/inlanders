using Godot;
using Inlanders.Simulation;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async Task ProbeCreativeCourt()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        var normal=World.NewArrangementCourt();normal.SaveFile(_neighborhoodPath);string ordinary=normal.SaveJson();
        ShowMainMenu();await Frames();await UiClick(_mainButtons["Creative"]);await Frames();
        await UiClick(_mainButtons["New free arrangement"]);await Frames();
        Check(_world.Creative && _world.IsArrangementCourt && _paused && !_drawer.Visible && CurrentSavePath==CreativeCourtPath,"Free arrangement entry/slot failed");
        _noticeUntil=0;await CaptureReviewBundle("free-opening");
        var homes=_world.Cottages.Where(c=>c.Kind==BuildingKind.Cottage).Take(2).ToArray();
        foreach(var home in homes)
        {
            int resident=_world.People.First(p=>p.HomeId==home.Id).Id;
            ShowDailyLife(resident);await Frames();Check(_dailyMove.Text=="Move home" && !_dailyRestore.Visible,"Free mode offered a locked trial");
            await CaptureReviewBundle("free-compact-resident");
            await UiClick(_dailyMove);await Frames();
            var at=_world.Map.Land.OrderBy(c=>(c.Point-new Cell(-7,3).Point).LengthSquared()).First(c=>c!=home.Cell && _world.RelocationProblem(home.Id,c,_rotation)==null);
            _focus=OnGround(at.X,at.Z);UpdateCamera();await Frames();
            var point=_camera.UnprojectPosition(OnGround(at.X,at.Z));
            Input.ParseInputEvent(new InputEventMouseMotion{Position=point,GlobalPosition=point});await Frames();await Click(point);await Frames();
            Check(home.Cell==at && _paused && _world.Neighborhood!.Arrangement!.BuildingId==null,"Multiple paused home moves failed");
        }
        BeginPlacement(BuildingKind.SeatingGarden);await Frames();
        var plot=_world.Map.Land.OrderBy(c=>(c.Point-new Cell(-1,1).Point).LengthSquared()).First(c=>_world.PlacementProblem(c,_rotation,BuildingKind.SeatingGarden)==null);
        _focus=OnGround(plot.X,plot.Z);UpdateCamera();await Frames();
        var target=_camera.UnprojectPosition(OnGround(plot.X,plot.Z));
        Input.ParseInputEvent(new InputEventMouseMotion{Position=target,GlobalPosition=target});await Frames();await Click(target);await Press(Key.Escape);await Frames();
        var garden=_world.Cottages.Single(c=>c.Cell==plot);
        Check(garden.Complete && garden.Delivered==0,"Free placement wasn't instant");
        SelectBuilding(garden.Id);ShowInspector();await Frames();await UiClick(_removeBuildingButton);await Frames();
        Check(!_world.Cottages.Any(c=>c.Id==garden.Id),"Free removal failed");
        CloseDrawer();_inspector.Hide();ShowDailyLife(0);await Frames();await Press(Key.Escape);await Frames();
        Check(!_dailyCard.Visible && !_dailyRoute.Visible,"Escape left the resident card open");
        bool collected=false;
        for(int i=0;i<1800;i++)
        {
            _world.Tick(.1f);collected|=_world.People.Any(p=>p.Meal is {Carrying:true});
            if(i%100==0){RenderActors(0);UpdateHud();await Frames();}
        }
        Check(collected && _world.Food.EatenBerries+_world.Food.EatenVegetables>0 && _world.Food.Hunger==0,"Free village did not collect and eat real meals");
        _focus=OnGround(3,3);_camera.Size=29;UpdateCamera();_noticeUntil=0;await Frames();Check(!_dailyCard.Visible,"Closed card reopened");await CaptureReviewBundle("free-daily-life-card-closed");
        await Press(Key.F5);string saved=_world.SaveJson();await Press(Key.F9);await Frames();Check(_world.SaveJson()==saved,"Free save/load changed village");
        ReturnToMainMenu();await Frames();await UiClick(_mainButtons["Continue"]);await Frames();Check(_world.SaveJson()==saved,"Continue changed free mode");
        ReturnToMainMenu();await Frames();await UiClick(_mainButtons["Creative"]);await Frames();await UiClick(_mainButtons["Resume free arrangement"]);await Frames();
        Check(_world.SaveJson()==saved && File.ReadAllText(_neighborhoodPath)==ordinary,"Resume overwrote the normal court");
        Reset();await Frames();Check(_world.Creative && _world.IsArrangementCourt && _world.SimulatesMeals && _world.Food.Time==0,"Reset lost free court rules");
        GD.Print("PASS: free court menu, two paused moves, actual placement/removal, compact card, physical meals, F5/F9, Continue/resume, separate normal save and reset (scripted UI)");
    }
}

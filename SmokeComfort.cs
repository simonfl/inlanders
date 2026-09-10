using Godot;
using Inlanders.Simulation;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async Task CheckComfortUi()
    {
        void Check(bool ok,string why) { if(!ok) throw new Exception(why); }
        async Task Frames() { for(int i=0;i<5;i++) await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame); }
        foreach(var size in new[]{new Vector2I(1440,900),new(960,640)})
        {
            GetWindow().Size=size;
            AdoptWorld(World.LoadJson(File.ReadAllText("artifacts/f25b2-ready.json"))); _paused=true;
            var home=_world.Cottages.First(c=>c.Kind==BuildingKind.Cottage);
            SelectBuilding(home.Id);await Frames();
            Check(_comfortOrder.Visible && !_comfortOrder.Disabled,"Home order unavailable");
            await UiClick(_comfortOrder);await Frames();Check(home.ImprovementRequested,"Home order click failed");
            await UiClick(_comfortCancel);await Frames();Check(!home.ImprovementRequested,"Home cancel click failed");
            await Capture($"artifacts/f25b2-ordinary-{size.X}.png");
            AdoptWorld(World.LoadJson(File.ReadAllText("artifacts/f25b2-InstallingComfort.json")));_paused=true;
            var worker=_world.People.Single(p=>p.Task==Work.InstallingComfort);
            SelectBuilding(worker.ComfortHomeId!.Value);await Frames();
            Check(_people[worker.Id].Hammer.Visible && _people[worker.Id].WorkBoard.Visible,"Installation pose missing");
            string saved=_world.SaveJson();await UiClick(_comfortWorker);await Frames();
            Check(_selectedPerson==worker.Id && _world.SaveJson()==saved,"Worker inspection changed order");
            SelectBuilding(worker.WorkplaceId!.Value);await Frames();
            Check(_productionControls.Visible && !_productionTargetToggle.Visible,"Carpenter has irrelevant stock target");
            await UiClick(_productionSource);await Frames();Check(_selectedSite==worker.ComfortHomeId,"Workshop home link failed");
            await Capture($"artifacts/f25b2-installing-{size.X}.png");
            AdoptWorld(World.LoadJson(File.ReadAllText("artifacts/f25b2-improved.json")));_paused=true;
            home=_world.Cottages.First(c=>c.Improved);SelectBuilding(home.Id);await Frames();
            Check(!_comfortOrder.Visible && _comfortInfo.Text.Contains("5m"),"Completed benefit missing");
            foreach(var p in _world.People) _world.Assign(p.Id,Role.Unassigned);
            var resident=_world.People.First(p=>p.HomeId==home.Id);resident.NextRestTime=_world.Food.Time;
            for(int i=0;i<5000 && resident.Task!=Work.Resting;i++) _world.Tick(.1f);
            await Frames();Check(resident.ImprovedRest && _people[resident.Id].ComfortCushion.Visible,"Actual improved rest cushion missing");
            await Capture($"artifacts/f25b2-improved-{size.X}.png");
            saved=_world.SaveJson();await Frames();Check(_world.SaveJson()==saved,"Paused improved rest changed");
        }
    }
}

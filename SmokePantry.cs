using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async void RunPantrySmoke()
    {
        try { if(OS.GetCmdlineUserArgs().Contains("--render-isolation")) await CheckFrameSyncUi(); else await CheckPantryUi(); await ProfilePantries(); GetTree().Quit(); }
        catch(Exception e) { GD.PrintErr("PANTRY SMOKE FAIL: "+e); GetTree().Quit(1); }
    }
    private async Task ProfilePantries()
    {
        var w=World.NewLargeMap(false,false);w.Food.InitialBerries=w.Food.Berries=512;
        foreach(var p in w.People) w.Assign(p.Id,p.Id<2?Role.Logger:p.Id<4?Role.Builder:Role.Unassigned);
        Cottage PlaceNear(Cell target,BuildingKind kind)
        {
            var cell=w.Map.Land.Where(c=>w.PlacementProblem(c,false,kind)==null).OrderBy(c=>(c.Point-target.Point).LengthSquared()).First();
            return w.Place(cell,false,kind)!;
        }
        PlaceNear(new(0,0),BuildingKind.Pantry);PlaceNear(new(5,5),BuildingKind.Pantry);
        for(int i=0;i<8;i++) PlaceNear(new(i%4*3-5,i/4*5),BuildingKind.Cottage);
        PlaceNear(new(-2,3),BuildingKind.SeatingGarden);PlaceNear(new(6,2),BuildingKind.SeatingGarden);
        for(int i=0;i<12000 && w.Cottages.Any(c=>!c.Complete);i++) w.Tick(.1f);
        if(w.Cottages.Any(c=>!c.Complete)) throw new Exception("Pantry profile construction stalled");
        while(w.Population<16) if(!w.InviteNewcomers()) throw new Exception("Pantry profile invitation failed");
        w.Assign(3,Role.Hauler);
        for(int i=0;i<600;i++) w.Tick(.1f);
        foreach(var cell in w.Map.Land.OrderBy(c=>c.Point.LengthSquared()))
        {if(w.Decorations.Count>=36) break;w.PlaceDecoration(cell,(DecorationKind)(w.Decorations.Count%4));}
        w.Validate();AdoptWorld(w);CloseManagementUi();_paused=true;_speed=1;
        GetWindow().Size=new(1440,900);_focus=OnGround(1,2);_camera.Size=26;UpdateCamera();
        if(OS.GetCmdlineUserArgs().Contains("--render-isolation")) { await ProfileRenderIsolation(); w=_world; }
        async Task Sample(string label,int count)
        {
            for(int i=0;i<10;i++) await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
            var times=new double[count];
            for(int i=0;i<count;i++) {ulong start=Time.GetTicksUsec();await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);times[i]=(Time.GetTicksUsec()-start)/1000d;}
            Array.Sort(times);GD.Print($"PANTRY PROFILE {label}: median {times[count/2]:0.0}ms p95 {times[(int)(count*.95)]:0.0}ms nodes {GetTree().GetNodeCount()} draws {Performance.GetMonitor(Performance.Monitor.RenderTotalDrawCallsInFrame):0}.");
        }
        await Sample("16 residents / 36 decorations / paused",120);
        _paused=false;await Sample("16 residents / actual meals and hauling / 1x",240);_paused=true;
        await Capture("artifacts/f07c2-profile-village.png");
        var spot=w.Map.Land.Where(c=>w.PlacementProblem(c,false,BuildingKind.Pantry)==null && !PointerOverHud(_camera.UnprojectPosition(OnGround(c.X,c.Z)))).OrderBy(c=>c.Point.LengthSquared()).First();
        _rotation=0;BeginPlacement(BuildingKind.Pantry);
        _pointerPosition=_camera.UnprojectPosition(OnGround(spot.X,spot.Z));
        Input.ParseInputEvent(new InputEventMouseMotion {Position=_pointerPosition,GlobalPosition=_pointerPosition});
        for(int i=0;i<10;i++) await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
        if(!_ghost.Visible || !_ghostValid) throw new Exception("Pantry profile preview invalid");
        string saved=w.SaveJson();int nodes=GetTree().GetNodeCount();
        await Sample("paused pantry preview / 600 frames",600);
        if(saved!=w.SaveJson() || nodes!=GetTree().GetNodeCount()) throw new Exception("Pantry preview changed state or node count");
        await Capture("artifacts/f07c2-profile-preview.png");
    }
    private async Task CheckPantryUi()
    {
        var previous=_world; var size=GetWindow().Size;
        async Task Frames(int n=5) {for(int i=0;i<n;i++) await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        void Until(World w,Func<bool> done)
        {for(int i=0;i<6000 && !done();i++) {w.Tick(.1f);w.Validate();} if(!done()) throw new Exception("Pantry fixture stalled");}
        try
        {
            var w=new World(10); w.Food.InitialBerries=w.Food.Berries=128;
            var pantry=w.Place(new(3,0),false,BuildingKind.Pantry)!;
            Until(w,()=>pantry.Complete);
            foreach(var p in w.People) w.Assign(p.Id,Role.Unassigned);
            w.Assign(0,Role.Hauler); Until(w,()=>pantry.PantryFood.Sum()>0);
            Until(w,()=>w.People.Any(p=>p.Task==Work.EatingMeal && p.Meal?.SourceId==pantry.Id));
            AdoptWorld(w); _paused=true; _focus=OnGround(3,0); _camera.Size=14; UpdateCamera(); SelectBuilding(pantry.Id);
            foreach(int width in new[]{1440,960})
            {
                GetWindow().Size=new(width,width==960?640:900); await Frames();
                _inspectionScroll.EnsureControlVisible(_pantryMore); await Frames();
                if(!_pantryControls.Visible || !_pantryInfo.Text.Contains("reserved")) throw new Exception("Pantry controls missing");
                int target=pantry.PantryTarget; await UiClick(_pantryMore); await Frames();
                if(pantry.PantryTarget!=target+4) throw new Exception("Pantry target click failed");
                await UiClick(_pantryLess); await Frames();
                string saved=w.SaveJson(); await Frames(30); if(saved!=w.SaveJson()) throw new Exception("Paused pantry advanced");
                await Capture($"artifacts/f07c2-pantry-{width}.png");
                var diner=w.People.First(p=>p.Task==Work.EatingMeal && p.Meal?.SourceId==pantry.Id);
                SelectPerson(diner.Id); await Frames(); _inspectionScroll.EnsureControlVisible(_mealLink); await Frames();
                if(!_mealNeeds.Text.Contains("Eating the collected") || !_mealLink.Visible) throw new Exception("Resident meal explanation/link missing");
                await Capture($"artifacts/f07c2-meal-person-{width}.png");
                await UiClick(_mealLink); await Frames();
                if(_selectedSite!=pantry.Id || w.SaveJson()!=saved) throw new Exception("Meal source link changed simulation or chose wrong pantry");
                OpenEconomy(); await Frames(); UpdateStorageDirectory();
                if(!_storageLinks.ContainsKey(pantry.Id) || !_economyFood.Text.Contains("staggered") || !_foodFlow.Text.Contains("closed/skipped")) throw new Exception("Pantry economy location or physical meal guidance missing");
                _drawerPages[4].EnsureControlVisible(_storageLinks[pantry.Id]); await Frames();
                await UiClick(_storageLinks[pantry.Id]); await Frames();
                if(_selectedSite!=pantry.Id || w.SaveJson()!=saved) throw new Exception("Economy pantry link changed simulation or selected wrong building");
                if(_drawer.Visible) ToggleDrawer(4);
                SelectBuilding(pantry.Id); await Frames();
            }
            var eater=w.People.First(p=>p.Task==Work.EatingMeal);
            if(!_people[eater.Id].RestStool.Visible || !_people[eater.Id].Carry.Visible) throw new Exception("Eating pose or real meal cargo missing");
            string roundtrip=w.SaveJson(); AdoptWorld(World.LoadJson(roundtrip)); _paused=true; await Frames();
            if(_world.SaveJson()!=roundtrip) throw new Exception("Rendered pantry reload changed state");
            GD.Print("PASS: live pantry model/stock, seated meal cargo, target clicks, paused state and reload at 960/1440.");
        }
        finally {GetWindow().Size=size;AdoptWorld(previous);_paused=true;}
    }
}

using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;

public partial class Game
{
    private async void RunWildlifeSmoke()
    {
        try
        {
            async System.Threading.Tasks.Task Frames() { for(int i=0;i<5;i++) await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame); }
            var w=World.NewLargeMap(); foreach(var p in w.People) w.Assign(p.Id,Role.Unassigned);
            var habitat=w.Map.Wildlife[0];
            var cell=w.Map.Land.Where(c=>w.PlacementProblem(c,false,BuildingKind.HuntingLodge)==null && (c.Point-habitat.Cell.Point).LengthSquared()<40).OrderBy(c=>(c.Point-w.YardAccess.Point).LengthSquared()).First();
            var lodge=w.Place(cell,false,BuildingKind.HuntingLodge)!;
            w.Assign(0,Role.Hunter); w.Assign(1,Role.Logger); w.Assign(2,Role.Builder);
            AdoptWorld(w); _paused=true; _noticeUntil=0;
            for(int i=0;i<7000 && w.People[0].Task!=Work.Hunting;i++) { w.Tick(.1f); w.Validate(); }
            if(w.People[0].Task!=Work.Hunting) throw new Exception("Rendered hunter stalled");
            await Frames();
            if(!_people[0].Bow.Visible || _people[0].Carry.Visible || _wildlifeViews.Count!=2) throw new Exception("Hunting presentation missing");
            var head=_wildlifeViews[0].GetChildren().OfType<Node3D>().First(n=>n.Name.ToString().StartsWith("Deer")).GetNode<Node3D>("Head");
            var pose=head.Transform; string saved=w.SaveJson(); await Frames();
            if(head.Transform!=pose || w.SaveJson()!=saved) throw new Exception("Paused habitat moved");
            for(int i=0;i<150 && w.People[0].Carried==0;i++) w.Tick(.1f);
            await Frames(); if(_people[0].Cargo!=Inlanders.Simulation.Resource.Game || !_people[0].Carry.Visible) throw new Exception("Game parcel missing");
            AdoptWorld(World.LoadJson(w.SaveJson())); w=_world; _paused=true; await Frames();
            if(_people[0].Cargo!=Inlanders.Simulation.Resource.Game || _people[0].Count!=2) throw new Exception("Game cargo lost on reload");
            foreach(int width in new[]{1440,960})
            {
                _placing=false; GetWindow().Size=new(width,width==960?640:900); _focus=OnGround(-7,-6); _camera.Size=17; UpdateCamera(); SelectBuilding(lodge.Id); await Frames();
                if(!_siteInfo.Text.Contains("mature trees") || !_workplaceControls.Visible || !_resourceValues[Inlanders.Simulation.Resource.Game].GetParent<Control>().Visible) throw new Exception("Habitat service controls missing");
                await Capture($"artifacts/f26c-hunting-{width}.png");
                OpenEconomy(); await Frames(); if(!_economyStocks.ContainsKey(Inlanders.Simulation.Resource.Game)) throw new Exception("Game inventory missing");
                CloseDrawer(); ToggleDrawer(1); BeginPlacement(BuildingKind.HuntingLodge); _pointerPosition=new(40,40); _hover=cell; RefreshGhost(); await Frames();
                await Capture($"artifacts/f26c-habitat-{width}.png");
            }
            var tree=w.Trees.First(t=>habitat.Contains(t.Cell) && !t.Felled && t.Growth>=1);
            CloseDrawer(); ToggleClearing(); _pointerPosition=_camera.UnprojectPosition(OnGround(tree.Cell.X,tree.Cell.Z)); _hover=tree.Cell; RefreshGhost(); await Frames();
            if(!_hint.Text.Contains("recovery") || !_hint.Text.Contains("capacity")) throw new Exception("Visible clearing habitat loss preview missing: "+_hint.Text);
            await Capture("artifacts/f26c-clearing-960.png");
            CloseManagementUi(); _placing=false; RefreshGhost();
            _camera.Size=22; UpdateCamera(); await Frames(); await Capture("artifacts/f26c-village.png");
            await ProfileWildlife();
            GD.Print("PASS: hunting lodge, wildlife, bow, game parcels, pause/reload, habitat survey, clearing loss, inventory and 960/1440 controls."); GetTree().Quit();
        }
        catch(Exception e) { GD.PrintErr("WILDLIFE SMOKE FAIL: "+e); GetTree().Quit(1); }
    }
    private async System.Threading.Tasks.Task ProfileWildlife()
    {
        var w=World.NewCreative(true); foreach(var p in w.People) w.Assign(p.Id,Role.Unassigned);
        foreach(var cell in w.Map.Land.OrderBy(c=>(c.Point-new Cell(-5,-5).Point).LengthSquared()))
        {
            if(w.Beds>=16) break;
            w.Place(cell,false,BuildingKind.Lodge);
        }
        while(w.Population<16) if(!w.InviteNewcomers()) throw new Exception("Wildlife profile population failed");
        foreach(var cell in w.Map.Land.OrderBy(c=>(c.Point-new Cell(-6,-5).Point).LengthSquared()))
        {
            if(w.Decorations.Count>=36) break;
            w.PlaceDecoration(cell,(DecorationKind)(w.Decorations.Count%4));
        }
        AdoptWorld(w); _paused=true; CloseManagementUi(); _placing=false; _focus=OnGround(-5,-4); _camera.Size=24; GetWindow().Size=new(1440,900); UpdateCamera();
        async System.Threading.Tasks.Task Sample(string label,int count)
        {
            for(int i=0;i<10;i++) await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
            var times=new double[count];
            for(int i=0;i<count;i++) { ulong start=Time.GetTicksUsec(); await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame); times[i]=(Time.GetTicksUsec()-start)/1000d; }
            Array.Sort(times);
            GD.Print($"WILDLIFE PROFILE {label}: median {times[count/2]:0.0}ms; p95 {times[(int)(count*.95)]:0.0}ms; draws {Performance.GetMonitor(Performance.Monitor.RenderTotalDrawCallsInFrame):0}; nodes {GetTree().GetNodeCount()}.");
        }
        await Sample("16 residents / 36 decorations / wildlife",120);
        foreach(var view in _wildlifeViews) view.Visible=false;
        await Sample("same scene / wildlife hidden",120);
        foreach(var view in _wildlifeViews) view.Visible=true;
        BeginPlacement(BuildingKind.HuntingLodge); _pointerPosition=new(40,40); _hover=new(-7,-7); RefreshGhost();
        for(int i=0;i<10;i++) await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
        string saved=w.SaveJson(); int nodes=GetTree().GetNodeCount();
        await Sample("paused survey / 600 frames",600);
        if(w.SaveJson()!=saved || nodes!=GetTree().GetNodeCount()) throw new Exception("Paused wildlife survey changed state or grew nodes");
    }
}

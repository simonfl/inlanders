using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async Task ProfileVillageRendering()
    {
        async Task Sample(string label, int count)
        {
            var times=new double[count];
            for(int i=0;i<count;i++)
            {
                ulong start=Time.GetTicksUsec(); await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
                times[i]=(Time.GetTicksUsec()-start)/1000d;
            }
            Array.Sort(times);
            GD.Print($"RENDER PROFILE {label}: median {times[count/2]:0.0}ms; p95 {times[(int)(count*.95)]:0.0}ms; draws {Performance.GetMonitor(Performance.Monitor.RenderTotalDrawCallsInFrame):0}; objects {Performance.GetMonitor(Performance.Monitor.RenderTotalObjectsInFrame):0}; nodes {GetTree().GetNodeCount()}.");
        }
        _paused=true; CloseDrawer();
        int MeshCount(Node root) => root.GetChildren().Sum(n=>(n is MeshInstance3D ? 1:0)+MeshCount(n));
        GD.Print($"RENDER INVENTORY: building meshes {_cottages.Values.Sum(v=>MeshCount(v.Body))}; crops {_cropViews.Values.Sum(v=>MeshCount(v.Body))}; landscape {MeshCount(_landscape)}; yard {MeshCount(_stored)}; people {_people.Sum(v=>MeshCount(v.Body))}.");
        await Sample("river-paused",120);
        foreach(var cell in _world.Map.Land.Where(c=>c.X>5).OrderBy(c=>c.Z).ThenBy(c=>c.X))
        {
            if(_world.Decorations.Count>=36) break;
            _world.PlaceDecoration(cell,(DecorationKind)(_world.Decorations.Count%4));
        }
        _world.Validate();
        for(int i=0;i<5;i++) await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
        await Capture("artifacts/f23c-decorated.png");
        await Sample("decorated-paused",120);
        var spot=_world.Map.Land.First(c=>_world.PlacementProblem(c,false,BuildingKind.Cottage)==null && !PointerOverHud(_camera.UnprojectPosition(OnGround(c.X,c.Z))));
        _rotation=0; BeginPlacement(BuildingKind.Cottage);
        _pointerPosition=_camera.UnprojectPosition(OnGround(spot.X,spot.Z));
        Input.ParseInputEvent(new InputEventMouseMotion { Position=_pointerPosition,GlobalPosition=_pointerPosition });
        for(int i=0;i<8;i++) await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
        if(!_ghost.Visible || !_ghostValid) throw new Exception("Rendering profile needs a legal visible preview");
        string saved=_world.SaveJson(); int nodes=GetTree().GetNodeCount();
        await Sample("stationary-preview-600-frames",600);
        if(GetTree().GetNodeCount()!=nodes || _world.SaveJson()!=saved) throw new Exception("Stationary paused preview changed nodes or simulation");
        await Capture("artifacts/f23c-preview.png");
        _placing=false; RefreshGhost();
        GD.Print("PASS: decorated river and prolonged stationary placement preserve simulation and node count.");
    }
}

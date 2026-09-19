using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async Task ProbePathConnection()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<6;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        async Task Point(Cell cell,bool click)
        {
            _focus=OnGround(cell.X,cell.Z);_camera.Size=18;UpdateCamera();
            var p=_camera.UnprojectPosition(OnGround(cell.X,cell.Z));
            ReviewInput(new InputEventMouseMotion{Position=p,GlobalPosition=p});await Frames();
            if(!click)return;
            ReviewInput(new InputEventMouseButton{Position=p,GlobalPosition=p,ButtonIndex=MouseButton.Left,Pressed=true});await Frames();
            ReviewInput(new InputEventMouseButton{Position=p,GlobalPosition=p,ButtonIndex=MouseButton.Left,Pressed=false});await Frames();
        }
        ToggleDrawer(1);SelectBuildSection(1);await Frames();
        await UiClick(_connectPathsButton,6);CloseDrawer();await Frames();
        Check(_placing && _pathTool==3,"Connection button did not activate");
        var home=_world.Cottages.First(c=>c.Complete && c.Kind==BuildingKind.Cottage);
        var start=home.Entrance;
        var end=_world.Map.Land.Where(c=>!_world.Paths.Contains(c) && c!=start).OrderBy(c=>(c.Point-start.Point).LengthSquared()).First(c=>_world.PathConnection(start,c,out var route)==null && route.Count>=4);
        string before=_world.SaveJson();await Point(home.Cell,true);await Point(end,false);
        Check(_pathAnchor==start && _ghostValid && _connectionRoute.Count>=4,"Building snap or route preview failed");
        await CaptureReviewBundle("connected-path-preview");await Press(Key.Escape);await Frames();
        Check(!_placing && before==_world.SaveJson(),"Cancelled connection mutated world");
        ToggleDrawer(1);SelectBuildSection(1);await Frames();await UiClick(_connectPathsButton,6);CloseDrawer();await Frames();
        await Point(start,true);await Point(end,true);
        Check(_pathAnchor==null && _world.Paths.Contains(end) && !_pathStroke,"Confirmed connection failed");
        await Press(Key.Escape);SaveWorld();string saved=_world.SaveJson();await Press(Key.F9);await Frames();
        Check(saved==_world.SaveJson(),"Connected path save mismatch");
        await Press(Key.P);Check(_placing && _pathTool==1,"Path brush no longer opens");await Press(Key.Escape);
        GD.Print("PASS: connection button, building entrance snap, route preview, unchanged cancellation, held pointer confirmation, F9 and original brush.");
    }
}

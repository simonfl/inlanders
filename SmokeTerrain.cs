using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async Task CheckTerrainUi()
    {
        void Check(bool ok,string message) { if(!ok) throw new Exception(message); }
        async Task Frames() { for(int i=0;i<4;i++) await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame); }
        AdoptWorld(World.NewCreative(true)); _paused=true; _noticeUntil=0;
        foreach(var t in _world.Trees.ToArray()) _world.SetClearing(t.Cell,true);
        var hill=new Cell(-9,-10);
        GetWindow().Size=new(960,640); _focus=OnGround(-9,-10); _camera.Size=16; UpdateCamera();
        foreach(float angle in new[] { .2f,1.5f,3f,4.7f })
        {
            _angle=angle; UpdateCamera(); await Frames();
            foreach(var cell in new[] { hill,new Cell(-10,-6),new(-6,-8) })
            {
                var expected=OnGround(cell.X,cell.Z); var hit=Ground(_camera.UnprojectPosition(expected));
                Check(hit is Vector3 h && h.DistanceTo(expected)<.002f,"Elevated mouse picking disagrees with visible terrain");
            }
        }
        _angle=.72f; _rotated=false; UpdateCamera(); BeginPlacement(BuildingKind.Bakery); CloseDrawer();
        _pointerPosition=_camera.UnprojectPosition(OnGround(hill.X,hill.Z));
        Input.ParseInputEvent(new InputEventMouseMotion { Position=_pointerPosition,GlobalPosition=_pointerPosition }); await Frames();
        Check(_ghostValid && _ghostModel.Position.Y>1.6f,"Hilltop ghost is not raised");
        await Capture("artifacts/f12d-hilltop-preview.png");
        await Click(_pointerPosition); await Frames();
        var bakery=_world.Cottages.Single(); Check(bakery.Cell==hill && _cottages[bakery.Id].Body.Position.Y==1.6f,"Hilltop click or foundation failed");
        CloseManagementUi();
        var farm=_world.Place(new(-2,10),true,BuildingKind.Farm)!;
        Check(farm != null,"Raised rotated field rejected");
        _world.Assign(4,Role.Farmer); _world.Assign(5,Role.Baker);
        for(int z=-6;z>=-9;z--) Check(_world.SetPath(new(-9,z),true),"Slope path rejected");
        Check(_world.PlantTree(new(-6,-6))!=null,"Slope planting rejected");
        Check(_world.PlaceDecoration(new(-7,-6),DecorationKind.Pebbles),"Slope pebbles rejected");
        Check(_world.PlaceDecoration(new(-8,-6),DecorationKind.Flowers),"Slope flowers rejected");
        bool sawClimb=false, capturedCrops=false;
        for(int i=0;i<3400;i++)
        {
            _world.Tick(.1f); _world.Validate(); RenderActors(.1f); RenderFoodViews();
            foreach(var p in _world.People)
            {
                var body=_people[p.Id].Body;
                Check(Math.Abs(body.Position.Y-Height(body.Position.X,body.Position.Z))<.001f,"Villager sank into slope");
                sawClimb |= p.Carried>0 && body.Position.Y is > .1f and < 1.5f;
            }
            if (!capturedCrops && farm!.Growth > .6f)
            {
                capturedCrops=true; _focus=OnGround(-2,10); _camera.Size=13; UpdateCamera(); await Frames();
                await Capture("artifacts/f12d-raised-crops.png");
            }
            if(i%100==0) await Frames();
        }
        Check(sawClimb && capturedCrops && _world.Food.Bread>0,"Rendered hill economy never carried goods uphill");
        Check(_cropViews.TryGetValue(farm!.Id,out var crops) && crops.Body.Position.Y==1.6f,"Crop mesh stayed below raised field");
        string saved=_world.SaveJson(); AdoptWorld(World.LoadJson(saved)); await Frames();
        Check(_world.SaveJson()==saved,"Terrain save changed the populated village");
        _focus=OnGround(-2,10); _camera.Size=13; _noticeUntil=0; UpdateCamera(); await Frames();
        await Capture("artifacts/f12d-raised-farm.png");
        _focus=OnGround(-8,-8); _camera.Size=16; _angle=.72f; UpdateCamera(); _noticeUntil=0; await Frames();
        await Capture("artifacts/f12d-working-hill.png");
        BeginPlacement(BuildingKind.Cottage); CloseDrawer();
        var slope=new Cell(-9,-5); _pointerPosition=_camera.UnprojectPosition(OnGround(slope.X,slope.Z));
        Input.ParseInputEvent(new InputEventMouseMotion { Position=_pointerPosition,GlobalPosition=_pointerPosition }); await Frames();
        Check(!_ghostValid && _placementProblem.Contains("level ground"),"Slope rejection missing from preview");
        await Capture("artifacts/f12d-slope-rejected-960.png");
        _placing=false; RefreshGhost(); FrameMap(); await Frames(); await Capture("artifacts/f12d-overview.png");
        GD.Print("SMOKE PASS: elevated picking at four angles, hilltop ghost/click, rotated crops, slope paths/planting/decorations, working carriers, saved terrain and 960px slope feedback.");
    }
}

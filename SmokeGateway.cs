using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async Task CheckGatewayUi()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<6;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        System.IO.Directory.CreateDirectory("artifacts/gateways");
        foreach(int width in new[]{960,1440})
        {
            var w=World.NewCreative();foreach(var p in w.People)w.Assign(p.Id,Role.Unassigned);
            var gate=new Cell(4,0);
            for(int x=2;x<=6;x++)for(int z=-4;z<=0;z++)
                if((x==2||x==6||z==-4||z==0) && new Cell(x,z)!=gate)
                    Check(w.PlaceDecoration(new(x,z),DecorationKind.Fence),$"Courtyard fence invalid {x},{z}");
            Check(w.PlaceDecoration(new(3,-2),DecorationKind.Flowers),"Garden flowers invalid");
            Check(w.SetPath(gate,true) && w.SetPath(new(4,-1),true),"Gateway path fixture failed");
            AdoptWorld(w);_paused=true;GetWindow().Size=new(width,width==960?640:900);_focus=new(4,0,-1);_camera.Size=13;_angle=.72f;UpdateCamera();await Frames();
            await Capture($"artifacts/gateways/gap-{width}.png");
            ToggleDrawer(1);SelectBuildSection(1);_decorationChoice.Select((int)DecorationKind.Gateway);await Frames();_drawerPages[1].EnsureControlVisible(_decorateButton);await Frames();await UiClick(_decorateButton);CloseDrawer();await Frames();
            Check(DecorationDescription.Contains("Always open"),"Gateway guidance absent");
            for(int facing=0;facing<4;facing++)
            {
                _rotation=0;for(int turn=0;turn<facing;turn++)await Press(Key.R);Check(_rotation==facing,"Gateway R did not retain full facing");_pointerPosition=_camera.UnprojectPosition(OnGround(gate.X,gate.Z));
                Input.ParseInputEvent(new InputEventMouseMotion{Position=_pointerPosition});await Frames();
                Check(_ghostValid && _fencePreviewCells.Count>0,"Gateway preview or neighboring fence preview missing");
                await Capture($"artifacts/gateways/preview-{width}-{facing}.png");
                await Click(_pointerPosition);await Frames();
                var item=w.Decorations.Single(d=>d.Cell==gate);Check(item.Facing==facing && w.Paths.Contains(gate),"Gateway UI facing or path lost");
                Check(_fenceBodies[gate].GetMeta("facing").AsInt32()==facing,"Rendered facing lost");
                Check(FenceConnections(gate)==(facing%2==0?5:0),"Fences crossed opening");
                _placing=false;RefreshGhost();await Frames();
                await Capture($"artifacts/gateways/placed-{width}-{facing}.png");
                if(width==960 && facing==0){for(int view=0;view<4;view++){_angle=.72f+view*Mathf.Pi/2;UpdateCamera();await Frames();await Capture($"artifacts/gateways/view-{view}.png");}_angle=.72f;UpdateCamera();await Frames();}
                Check(World.LoadJson(w.SaveJson()).SaveJson()==w.SaveJson(),"Gateway UI save failed");
                Check(w.RemoveDecoration(gate) && w.Paths.Contains(gate),"Removing gate lost path");
                BeginDecorating(false);await Frames();
            }
            _placing=false;RefreshGhost();
            Check(w.RemoveDecoration(new(5,0)) && w.PlaceDecoration(new(5,0),DecorationKind.Gateway,false,1),"Neighbor gate fixture failed");
            Check(w.PlaceDecoration(gate,DecorationKind.Gateway,false,0) && FenceConnections(gate)==4,"Incompatible gateway sides joined");
            w.RemoveDecoration(new(5,0));w.PlaceDecoration(new(5,0),DecorationKind.Gateway,false,0);
            Check(FenceConnections(gate)==5 && FenceConnections(new(5,0))==5,"Compatible gateway side link missing");w.Validate();
        }
        var hills=World.NewCreative(true);var slope=hills.Map.Land.First(c=>!hills.Map.LevelGround(new[]{c}) && hills.DecorationProblem(c,DecorationKind.Gateway)==null);
        AdoptWorld(hills);_paused=true;_focus=new(slope.X,0,slope.Z);_camera.Size=10;UpdateCamera();
        for(int facing=0;facing<4;facing++)
        {
            Check(hills.PlaceDecoration(slope,DecorationKind.Gateway,false,facing),"Slope gateway placement failed");await Frames();
            Check(_fenceBodies[slope].Position==OnGround(slope.X,slope.Z),"Slope gate body misplaced");
            await Capture($"artifacts/gateways/slope-{facing}.png");hills.Validate();hills.RemoveDecoration(slope);await Frames();
        }
        GD.Print("PASS: gateway palette, four saved/rendered facings, side-only neighbor previews, courtyard gap comparisons, path preservation and removal at 960/1440.");
    }
}

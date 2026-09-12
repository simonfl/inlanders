using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async Task CheckAreaRemoval()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        async Task Drag(Cell first,Cell last)
        {
            var a=_camera.UnprojectPosition(OnGround(first.X,first.Z));var b=_camera.UnprojectPosition(OnGround(last.X,last.Z));
            Check(!PointerOverHud(a) && !PointerOverHud(b),"Removal fixture overlaps HUD");
            Input.ParseInputEvent(new InputEventMouseButton{Position=a,GlobalPosition=a,ButtonIndex=MouseButton.Left,Pressed=true});
            Input.ParseInputEvent(new InputEventMouseMotion{Position=b,GlobalPosition=b,ButtonMask=MouseButtonMask.Left});await Frames();
            Input.ParseInputEvent(new InputEventMouseButton{Position=b,GlobalPosition=b,ButtonIndex=MouseButton.Left,Pressed=false});await Frames();
        }
        System.IO.Directory.CreateDirectory("artifacts/area-removal");
        foreach(int width in new[]{960,1440})
        {
            var w=World.NewCreative(true);foreach(var p in w.People)w.Assign(p.Id,Role.Unassigned);
            var at=w.Map.Land.Where(c=>w.PlacementProblem(c,0,BuildingKind.Cottage)==null).OrderBy(c=>c.Point.LengthSquared()).First();var site=w.Place(at,0,BuildingKind.Cottage)!;
            var decor=w.Map.Land.Where(c=>w.DecorationProblem(c,DecorationKind.Flowers)==null).OrderBy(c=>(c.Point-at.Point).LengthSquared()).First();w.PlaceDecoration(decor,DecorationKind.Flowers);
            var path=w.Map.Land.Where(c=>w.PathProblem(c)==null).OrderBy(c=>(c.Point-at.Point).LengthSquared()).First();w.SetPath(path,true);
            var cells=World.Footprint(at,0,BuildingKind.Cottage).Append(decor).Append(path).ToArray();
            var first=new Cell(cells.Min(c=>c.X)-1,cells.Min(c=>c.Z)-1);var last=new Cell(cells.Max(c=>c.X)+1,cells.Max(c=>c.Z)+1);
            AdoptWorld(w);_paused=true;_speed=4;GetWindow().Size=new(width,width==960?640:900);_focus=OnGround((first.X+last.X)/2f,(first.Z+last.Z)/2f);_camera.Size=24;_angle=0;UpdateCamera();await Frames();
            ToggleDrawer(1);SelectBuildSection(1);_drawerPages[1].EnsureControlVisible(_areaEntry);await Frames();
            await UiClick(_areaEntry);await Frames();Check(_areaRemoving && _areaPanel.Visible,"Area entry failed");
            string saved=w.SaveJson();var focus=_focus;await Drag(first,last);
            Check(_areaSelection is {Count:3} && _areaSelection.Buildings.Count==1 && _areaSelection.Decorations.Count==1 && _areaSelection.Paths.Count==1,"Mixed selection wrong");
            Check(w.SaveJson()==saved && _focus==focus && _areaMarks.GetChildCount()>0,"Selection mutated world, moved camera or has no highlights");
            Check(_areaPanel.GetGlobalRect().End.Y<=heightLimit(),"Removal panel overflow");
            float heightLimit()=>_bottomBar.Position.Y;
            await Capture($"artifacts/area-removal/review-{width}.png");
            await Press(Key.Escape);Check(!_areaRemoving && w.SaveJson()==saved,"Escape changed selection world");
            BeginAreaRemoval();await Frames();await Drag(last,first);
            w.RemoveDecoration(decor);string stale=w.SaveJson();await UiClick(_areaConfirm);await Frames();
            Check(ReferenceEquals(w,_world) && w.SaveJson()==stale && _areaRemoving,"Stale confirmation partially committed");
            await Capture($"artifacts/area-removal/rejected-{width}.png");
            await Drag(first,last);await UiClick(_areaConfirm);await Frames();
            Check(!_areaRemoving && !_areaPanel.Visible && _world.Cottages.Count==0 && _world.Paths.Count==0 && _world.Decorations.Count==0,"Confirmed selection did not remove objects");
            Check(_paused && _speed==4 && _focus==focus && !_cottages.ContainsKey(site.Id),"Removal reset controls or retained building view");
            _world.Validate();Check(World.LoadJson(_world.SaveJson()).SaveJson()==_world.SaveJson(),"Removed UI world failed save");
            BeginAreaRemoval();await Frames();await Press(Key.B);Check(!_areaRemoving,"Build shortcut retained removal");
            BeginAreaRemoval();await Frames();await UiClick(_menuButtons[0]);Check(!_areaRemoving,"Mouse drawer retained removal");
            BeginAreaRemoval();_Notification((int)NotificationWMWindowFocusOut);Check(!_areaRemoving,"Focus loss retained removal");
            BeginAreaRemoval();Input.ParseInputEvent(new InputEventMouseButton{Position=new(400,300),ButtonIndex=MouseButton.Right,Pressed=true});await Frames();Check(!_areaRemoving,"Right-click did not cancel");
            Input.ParseInputEvent(new InputEventMouseButton{Position=new(400,300),ButtonIndex=MouseButton.Right,Pressed=false});
            BeginAreaRemoval();BeginPlacement(BuildingKind.Bakery);Check(!_areaRemoving,"Placement retained removal");await Press(Key.Escape);
            BeginAreaRemoval();AdoptWorld(World.NewScenario());await Frames();Check(!_areaRemoving && !_areaEntry.Visible,"World replacement retained Creative tool");
        }
        GD.Print("PASS: Creative area entry, drag/reversed selection, whole-object highlights, Escape, stale atomic refusal, confirmation, save/control preservation and mode handoffs at 960/1440.");
    }
}

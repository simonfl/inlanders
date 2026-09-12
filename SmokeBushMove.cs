using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async Task CheckBushMoveUi()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<8;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        async Task Pick(Cell cell)
        {var p=_camera.UnprojectPosition(OnGround(cell.X,cell.Z));Check(!PointerOverHud(p),"Bush fixture overlaps HUD");await Click(p);await Frames();}
        System.IO.Directory.CreateDirectory("artifacts/bush-move");
        foreach(int width in new[]{960,1440})
        {
            var w=World.NewCreative();foreach(var person in w.People)w.Assign(person.Id,Role.Unassigned);
            var bush=w.Bushes.Single(b=>b.Cell==new Cell(0,-2));var source=bush.Cell;var target=new Cell(3,-3);
            Check(w.BushMoveProblem(bush.Id,target)==null,"Bush UI fixture invalid");
            AdoptWorld(w);_paused=true;_speed=4;GetWindow().Size=new(width,width==960?640:900);_focus=new(1,0,-2);_camera.Size=15;_angle=0;UpdateCamera();await Frames();
            ToggleDrawer(1);SelectBuildSection(1);await Frames();await UiClick(_bushMoveEntry);await Frames();
            string saved=w.SaveJson();var focus=_focus;await Pick(source);await Pick(target);
            Check(_bushMoving && _bushMoveId==bush.Id && _bushMoveAt==target && !_bushMoveConfirm.Disabled && _bushMoveMarks.GetChildCount()==4,"Bush selection or preview failed");
            Check(w.SaveJson()==saved && _focus==focus && _bushMovePanel.GetGlobalRect().End.Y<_bottomBar.Position.Y,"Preview mutated world/panned/overflowed");
            await Capture($"artifacts/bush-move/preview-{width}.png");await Press(Key.Escape);Check(!_bushMoving && w.SaveJson()==saved,"Escape changed world");
            BeginBushMove();await Frames();await Pick(source);await Pick(target);
            // Change the live destination after preview. Confirm must recheck, even before the next periodic refresh.
            w.SetPath(target,true);string blocked=w.SaveJson();_bushMoveConfirm.EmitSignal(BaseButton.SignalName.Pressed);await Frames();
            Check(_bushMoving && bush.Cell==source && blocked==w.SaveJson() && _bushMoveConfirm.Disabled,"Stale destination committed");await Capture($"artifacts/bush-move/rejected-{width}.png");
            w.SetPath(target,false);await Pick(target);var body=_bushViews[bush.Id].Body;await UiClick(_bushMoveConfirm);await Frames();
            Check(!_bushMoving && bush.Cell==target && _bushViews[bush.Id].Body==body && body.Position==OnGround(target.X,target.Z),"Bush body did not move with source");
            Check(_paused && _speed==4 && _focus==focus && bush.Ripe==8,"Move changed time/camera/food");w.Validate();Check(World.LoadJson(w.SaveJson()).SaveJson()==w.SaveJson(),"Moved UI world cannot roundtrip");
            await Capture($"artifacts/bush-move/moved-{width}.png");
            BeginBushMove();await Press(Key.B);Check(!_bushMoving,"Build shortcut kept move mode");
            BeginBushMove();await UiClick(_menuButtons[0]);Check(!_bushMoving,"Drawer click kept move mode");
            BeginBushMove();_Notification((int)NotificationWMWindowFocusOut);Check(!_bushMoving,"Focus loss kept move mode");
            BeginBushMove();Input.ParseInputEvent(new InputEventMouseButton{ButtonIndex=MouseButton.Right,Pressed=true});await Frames();Check(!_bushMoving,"Right-click did not cancel");
            Input.ParseInputEvent(new InputEventMouseButton{ButtonIndex=MouseButton.Right,Pressed=false});
            BeginBushMove();BeginPlacement(BuildingKind.Cottage);Check(!_bushMoving,"Placement kept move mode");
            BeginBushMove();AdoptWorld(World.NewScenario());await Frames();Check(!_bushMoving && !_bushMoveEntry.Visible,"Normal/new world kept Creative tool");
        }
        GD.Print("PASS: Creative bush entry, source/destination preview, live rejection, explicit move, same-body relocation, exact saves, camera/time preservation and cancellation/handoffs at 960/1440.");
    }
}

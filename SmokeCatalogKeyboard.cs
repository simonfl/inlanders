using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async Task CheckCatalogKeyboard()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        async Task Focus(Control control)
        {
            for(int i=0;i<22 && GetViewport().GuiGetFocusOwner()!=control;i++){await Press(Key.Tab);await Frames();}
            Check(GetViewport().GuiGetFocusOwner()==control,"Catalog control unreachable by Tab");
        }
        foreach(int width in new[]{960,1440})
        {
            AdoptWorld(World.NewScenario());_paused=true;CloseManagementUi();_placing=false;RefreshGhost();
            GetWindow().Size=new(width,width==960?640:900);_focus=new(3,0,0);_camera.Size=16;UpdateCamera();await Frames();
            string state=_world.SaveJson();await Press(Key.B);await Frames();
            Check(_catalogKeyboard && CatalogOpen && !_placing,"B did not open read-only catalog");
            var camera=_focus;int rotation=_rotation;
            Input.ParseInputEvent(new InputEventKey{Keycode=Key.W,PhysicalKeycode=Key.W,Pressed=true});await Frames();
            Input.ParseInputEvent(new InputEventKey{Keycode=Key.W,PhysicalKeycode=Key.W,Pressed=false});
            await Press(Key.R);await Press(Key.Q);Check(_focus==camera && _rotation==rotation,"Catalog keys reached camera/rotation");
            await Focus(_buildingFilter);
            for(int category=1;category<=5;category++)
            {
                await Press(Key.Right);await Frames();Check(_buildingFilter.Selected==category,"Keyboard category failed");
                Check(_kindButtons.Where(p=>p.Value.IsVisibleInTree()).All(p=>BuildingCategory(p.Key)==category),"Category displays unrelated cards");
            }
            await Press(Key.Right);await Frames();Check(_buildingFilter.Selected==0,"Category wrap failed");
            foreach(var kind in Enum.GetValues<BuildingKind>())
            {
                await Focus(_kindButtons[kind]);await Frames();
                Check(_hint.Text.Contains(BuildingDescription(kind)) && _hint.Text.Contains(Buildings.Get(kind).CostText),"Focus omits actual description/cost for "+kind);
                var rect=_kindButtons[kind].GetGlobalRect();var view=_drawerPages[1].GetGlobalRect();
                Check(rect.Position.Y>=view.Position.Y && rect.End.Y<=view.End.Y,"Focused card clipped: "+kind);
                if(kind==BuildingKind.Quarry)await Capture($"artifacts/catalog-keyboard-card-{width}.png");
                await Press(Key.Enter);await Frames();Check(_placing && _buildKind==kind && !_catalogKeyboard,"Activation did not hand off "+kind);
                Input.ParseInputEvent(new InputEventMouseMotion{Position=_camera.UnprojectPosition(new(3,0,0)),GlobalPosition=_camera.UnprojectPosition(new(3,0,0))});await Frames();
                rotation=_rotation;await Press(Key.R);Check(_rotation==(rotation+1)%4,"Rotation consumed after card activation");
                Check(_ghostModelKey==kind.ToString() && _world.SaveJson()==state,"Preview selected wrong model or edited world");
                await Press(Key.Escape);Check(!_placing,"Escape kept placement active");
                await Press(Key.B);await Frames();Check(GetViewport().GuiGetFocusOwner()==_kindButtons[kind],"Reopening lost current card");
            }
            await Focus(_kindButtons[BuildingKind.Cottage]);_world.Food.Celebrating=true;await Frames();
            await Press(Key.Enter);Check(!_placing && _hint.Text.Contains("supper"),"Disabled supper card activated or lacks reason");_world.Food.Celebrating=false;await Frames();
            await Press(Key.Escape);await Frames();Check(!_catalogKeyboard && !_drawer.Visible,"Escape did not close focused catalog");
            await Press(Key.B);await Frames();await Focus(_kindButtons[BuildingKind.Cottage]);
            await Click(_kindButtons[BuildingKind.Cottage].GetGlobalRect().GetCenter());await Frames();Check(_placing && !_catalogKeyboard,"Mouse takeover failed");
            CloseDrawer();Input.ParseInputEvent(new InputEventMouseMotion{Position=_camera.UnprojectPosition(new(3,0,0)),GlobalPosition=_camera.UnprojectPosition(new(3,0,0))});await Frames();
            _rotation=0;RefreshGhost();await Click(_pointerPosition);await Frames();Check(_world.Cottages.Any(c=>c.Kind==BuildingKind.Cottage && c.Cell==new Cell(3,0)),"Pointer placement failed after catalog handoff");
            await OpenMenu(3);_viewName.GrabFocus();_viewName.Text="Typed village";await Press(Key.B);Check(!_catalogKeyboard && EditingText,"Typing B opened catalog");_viewName.ReleaseFocus();_viewName.Text="";
            CloseManagementUi();await Press(Key.B);await Frames();
            GetWindow().Size=new(width==960?1440:960,width==960?900:640);await Frames();await Press(Key.Tab);await Frames();
            Check(_catalogKeyboard && GetViewport().GuiGetFocusOwner()!=null,"Resize lost keyboard catalog");
            await Press(Key.Escape);
        }
        GD.Print("PASS: catalog keyboard categories/all buildings, actual descriptions/costs, scroll/focus, preview isolation, disabled activation, camera/rotation isolation and handoff, cancellation, typing and mouse placement at 960/1440.");
    }
}

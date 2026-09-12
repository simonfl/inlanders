using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async Task CheckTerrainShapingUi()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<8;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        async Task Select(Cell a,Cell b)
        {
            var p=_camera.UnprojectPosition(OnGround(a.X,a.Z));Check(!PointerOverHud(p),"Start overlaps HUD");
            Input.ParseInputEvent(new InputEventMouseButton{ButtonIndex=MouseButton.Left,Pressed=true,Position=p});await Frames();
            var q=_camera.UnprojectPosition(OnGround(b.X,b.Z));Check(!PointerOverHud(q),"End overlaps HUD");
            Input.ParseInputEvent(new InputEventMouseMotion{Position=q,ButtonMask=MouseButtonMask.Left});await Frames();
            Input.ParseInputEvent(new InputEventMouseButton{ButtonIndex=MouseButton.Left,Pressed=false,Position=q});await Frames();
        }
        System.IO.Directory.CreateDirectory("artifacts/terrain-shaping");
        foreach(int width in new[]{960,1440})
        {
            var w=World.NewCreative();foreach(var p in w.People)w.Assign(p.Id,Role.Unassigned);
            var at=new Cell(4,0);var cells=World.Footprint(at,0).Append(World.Door(at,0)).ToArray();
            var first=new Cell(cells.Min(c=>c.X),cells.Min(c=>c.Z));var last=new Cell(cells.Max(c=>c.X),cells.Max(c=>c.Z));
            Check(w.PreviewTerrain(first,last,.4f).Problem==null,"UI site invalid");
            AdoptWorld(w);_paused=true;_speed=4;GetWindow().Size=new(width,width==960?640:900);_focus=new(4,0,0);_camera.Size=16;_angle=0;UpdateCamera();await Frames();
            ToggleDrawer(1);SelectBuildSection(1);await Frames();await UiClick(_terrainEntry);await Frames();
            var focus=_focus;string before=w.SaveJson();await Select(first,last);
            Check(_terrainEditing && !_terrainApply.Disabled && _terrainRenderMap!=null && _terrainMarks.GetChildCount()==1,"Terrace preview failed");
            Check(w.SaveJson()==before && _focus==focus,"Preview mutated world or panned camera");
            Check(_terrainPanel.GetGlobalRect().End.Y<_bottomBar.Position.Y,"Terrain panel overflow");
            await Capture($"artifacts/terrain-shaping/after-{width}.png");await UiClick(_terrainCompare);await Frames();
            Check(_terrainRenderMap==null && w.SaveJson()==before,"Before view changed world");await Capture($"artifacts/terrain-shaping/before-{width}.png");
            await Press(Key.Escape);Check(!_terrainEditing && w.SaveJson()==before,"Cancel changed terrain");
            BeginTerrain();await Frames();await Select(first,last);
            var blockedCell=_terrainPreview!.ChangedCells.First(c=>c.X<first.X);Check(w.SetPath(blockedCell,true),"Path fixture failed");string blocked=w.SaveJson();
            _terrainApply.EmitSignal(BaseButton.SignalName.Pressed);await Frames();
            Check(w.SaveJson()==blocked && _terrainApply.Disabled && _terrainMarkedBlocker==blockedCell && _terrainText.Text.Contains("Path"),"Stale Apply or blocker failed");
            Check(_terrainClose.GetGlobalRect().End.Y<_bottomBar.Position.Y,"Blocked panel overflow");await Capture($"artifacts/terrain-shaping/blocked-{width}.png");
            w.SetPath(blockedCell,false);_terrainRefresh=0;UpdateTerrainUi();await Frames();await UiClick(_terrainApply);await Frames();
            Check(w.Map.Heights.Length>0 && !_terrainUndoButton.Disabled && _terrainRenderMap==null,"Apply or undo availability failed");
            var point=OnGround(at.X,at.Z);var hit=Ground(_camera.UnprojectPosition(point));
            Check(hit.HasValue && hit.Value.DistanceTo(point)<.01f,"Edited surface picking failed");
            Check(_paused && _speed==4 && _focus==focus,"Apply changed camera or speed");w.Validate();
            var cottage=w.Place(at,0);Check(cottage!=null,"Cannot build on actual terrace");
            CancelTerrain();BeginTerrain();await Frames();
            Check(_terrainUndoButton.Disabled && _terrainUndoText.Text.Contains("Building") && _terrainMarkedBlocker.HasValue,"Reopened blocked Undo lacks reason/marker");
            Check(_terrainClose.GetGlobalRect().End.Y<_bottomBar.Position.Y,"Undo panel overflow");await Capture($"artifacts/terrain-shaping/undo-building-{width}.png");
            w.RemoveBuilding(cottage!.Id);_terrainRefresh=0;UpdateTerrainUi();await Frames();
            Check(!_terrainUndoButton.Disabled,"Cleared building did not restore Undo");
            // Controlled transient route fixture: reservation appears after Apply, then clears.
            var walker=w.People[0];walker.Route.Enqueue(blockedCell);_terrainRefresh=0;UpdateTerrainUi();await Frames();
            Check(_terrainUndoButton.Disabled && _terrainUndoText.Text.Contains("Walking route") && _terrainMarkedBlocker==blockedCell,"Traffic Undo guidance failed");
            await Capture($"artifacts/terrain-shaping/undo-route-{width}.png");walker.Route.Clear();_terrainRefresh=0;UpdateTerrainUi();await Frames();
            Check(!_terrainUndoButton.Disabled && _terrainMarkedBlocker==null,"Traffic clearing did not restore Undo");
            await Capture($"artifacts/terrain-shaping/applied-{width}.png");
            _terrainTarget.Value=0;await Select(first,last);Check(_terrainRenderMap!=null && !_terrainApply.Disabled,"Lowering preview failed");
            await Capture($"artifacts/terrain-shaping/lowering-{width}.png");CancelTerrain();BeginTerrain();_terrainTarget.Value=.4;await Frames();
            await UiClick(_terrainUndoButton);await Frames();
            Check(w.Map.Heights.Length==0 && _terrainUndoButton.Disabled,"Undo did not restore original geometry");
            await Capture($"artifacts/terrain-shaping/undone-{width}.png");
            var access=w.Map.Land.Select(c=>(Cell:c,Preview:w.PreviewTerrain(c,c,.4f))).First(x=>x.Preview.Blocker?.Kind.EndsWith("access")==true);
            _terrainFirst=_terrainLast=access.Cell;_terrainRefresh=0;UpdateTerrainUi();await Frames();
            Check(_terrainApply.Disabled && _terrainMarkedBlocker==access.Preview.Blocker!.Cell,"Invisible access marker failed");
            Check(_terrainClose.GetGlobalRect().End.Y<_bottomBar.Position.Y,"Access panel overflow");await Capture($"artifacts/terrain-shaping/access-{width}.png");
            await UiClick(_terrainClose);Check(!_terrainEditing,"Close unusable after rejection");BeginTerrain();await Frames();
            await Select(first,last);_Notification((int)NotificationWMWindowFocusOut);Check(!_terrainEditing && _terrainRenderMap==null,"Focus loss kept preview");
            BeginTerrain();await Press(Key.B);Check(!_terrainEditing,"Build shortcut kept terrain tool");
            BeginTerrain();BeginPlacement(BuildingKind.Cottage);Check(!_terrainEditing,"Placement kept terrain tool");
            BeginTerrain();AdoptWorld(World.NewScenario());await Frames();Check(!_terrainEditing && !_terrainEntry.Visible,"New normal world kept terrain tool");
        }
        GD.Print("PASS: terrace drag selection, before/after preview, stale Apply rejection, Apply/Undo, placement, picking, cancellation and camera/time preservation at 960/1440.");
    }
}

using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;

public partial class Game
{
    private async void RunWoodlandSmoke()
    {
        try
        {
            async System.Threading.Tasks.Task Frames() { for(int i=0;i<5;i++) await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame); }
            void Tool(string name) => _buildSections[1].GetChildren().OfType<Button>().Single(b=>b.Text==name).EmitSignal(BaseButton.SignalName.Pressed);
            var w=World.NewCreative(); foreach(var p in w.People) w.Assign(p.Id,Role.Unassigned);
            AdoptWorld(w); _paused=true; ToggleDrawer(1); SelectBuildSection(1);
            Tool("Preserve trees");
            var tree=w.Trees[0]; PaintWoodland(tree.Cell); await Frames();
            if(!tree.Preserved || _trees[tree.Id].Stage<20) throw new Exception("Preservation control or tree marker missing");
            Tool("Allow harvesting"); PaintWoodland(tree.Cell);
            if(tree.Preserved) throw new Exception("Allow harvesting control failed");
            Tool("Preserve trees"); PaintWoodland(tree.Cell);
            Tool("Manage grove · replant"); PaintWoodland(new(3,0)); PaintWoodland(new(6,0)); await Frames();
            if(w.ManagedWoodland.Count!=4 || _woodlandView?.GetChildCount()!=4) throw new Exception("Grove stroke/overlay did not cover four spots");
            foreach(int width in new[]{1440,960})
            {
                GetWindow().Size=new(width,width==960?640:900); _focus=new(2,0,1); _camera.Size=18; UpdateCamera(); await Frames();
                _drawerPages[1].ScrollVertical=235; await Frames();
                if(!_buildDescription.Text.Contains("32")) throw new Exception("Grove limit guidance missing");
                await Capture($"artifacts/f02b-woodland-{width}.png");
            }
            Tool("Remove grove spots"); PaintWoodland(new(6,0));
            if(w.ManagedWoodland.Count!=3) throw new Exception("Remove grove control failed");
            BeginPlacement(BuildingKind.Cottage); if(_woodlandTool!=0) throw new Exception("Building tool retained woodland mode");
            string saved=w.SaveJson(); await Frames(); if(w.SaveJson()!=saved) throw new Exception("Paused woodland state changed");
            AdoptWorld(World.LoadJson(saved)); _paused=true; CloseDrawer();
            if(_world.SaveJson()!=saved) throw new Exception("Woodland UI save reload changed state");
            BeginWoodlandTool(3); _focus=new(4,0,6); _camera.Size=12; UpdateCamera(); await Frames();
            var start=_camera.UnprojectPosition(new(3,0,6)); var end=_camera.UnprojectPosition(new(6,0,6));
            Input.ParseInputEvent(new InputEventMouseMotion { Position=start,GlobalPosition=start }); await Frames();
            Input.ParseInputEvent(new InputEventMouseButton { Position=start,GlobalPosition=start,ButtonIndex=MouseButton.Left,Pressed=true }); await Frames();
            Input.ParseInputEvent(new InputEventMouseMotion { Position=end,GlobalPosition=end,ButtonMask=MouseButtonMask.Left }); await Frames();
            Input.ParseInputEvent(new InputEventMouseButton { Position=end,GlobalPosition=end,ButtonIndex=MouseButton.Left,Pressed=false }); await Frames();
            if(_world.ManagedWoodland.Count!=7 || _woodlandStroke) throw new Exception("Real pointer grove drag/release failed");
            _placing=false; RefreshGhost();
            foreach(var t in _world.Trees) _world.SetTreePreserved(t.Cell,true);
            _world.Assign(0,Role.Logger); _world.Assign(1,Role.Logger);
            for(int i=0;i<3500;i++) _world.Tick(.1f);
            _world.Validate(); await Frames();
            if(_world.TreesPlanted<3) throw new Exception("Rendered managed grove did not plant");
            await Capture("artifacts/f02b-grown-grove-960.png");
            GD.Print("PASS: woodland controls, interpolated grove stroke, preservation marker, overlay, tool switching, paused save/reload and growth at 960/1440.");
            GetTree().Quit();
        }
        catch(Exception e) { GD.PrintErr(e); GetTree().Quit(1); }
    }
}

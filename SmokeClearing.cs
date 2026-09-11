using Godot;
using Inlanders.Simulation;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async void RunClearingSmoke()
    {
        _savePath = Path.Combine(Path.GetTempPath(), "inlanders-clearing-" + Guid.NewGuid() + ".json");
        try
        {
            void Check(bool value, string message) { if (!value) throw new Exception(message); }
            async Task Frames() { for (int i = 0; i < 4; i++) await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame); }
            async Task Point(Cell cell)
            {
                var position = _camera.UnprojectPosition(new(cell.X, 0, cell.Z));
                Input.ParseInputEvent(new InputEventMouseMotion { Position = position, GlobalPosition = position }); await Frames();
            }
            _paused = true; foreach (var v in _world.People) _world.Assign(v.Id, Role.Unassigned);
            Cell cell = new(3, -5); var tree = _world.Trees.Single(t => t.Cell == cell);
            await OpenMenu(1); await UiClick(_clearTreeButton); CloseDrawer(); await Point(cell);
            Check(_clearingTrees && _ghostValid && _hint.Text.Contains("assign loggers"), "Unstaffed clearing guidance missing");
            await Click(_camera.UnprojectPosition(new(cell.X, 0, cell.Z))); await Frames();
            Check(tree.ClearRequested && _placing && _trees[tree.Id].Stage >= 10, "Tree order or visible marker missing");
            await Capture("artifacts/f12c-marked.png");
            await Click(_camera.UnprojectPosition(new(cell.X, 0, cell.Z))); Check(!tree.ClearRequested, "Click again did not cancel");
            await Click(_camera.UnprojectPosition(new(cell.X, 0, cell.Z))); await Press(Key.F5);
            _world.SetClearing(cell, false); await Press(Key.F9); await Frames();
            tree = _world.Trees.Single(t => t.Cell == cell); Check(tree.ClearRequested, "Clearing order not saved");
            await Press(Key.C); await Point(new(0, 0));
            Check(!_ghostValid && _hint.Text.Contains("Choose a tree"), "Invalid target feedback missing");
            await Press(Key.Escape); Check(!_placing && !_ghost.Visible, "Esc did not finish clearing mode");
            _world.Assign(0, Role.Logger);
            for (int i = 0; i < 10000 && _world.People[0].Task != Work.ClearingStump; i++) _world.Tick(0.1f);
            await Frames();
            Check(_world.People[0].Task == Work.ClearingStump && _people[0].Spade.Visible && _world.Stored == 8, "Root work/tool or hauling failed");
            GetWindow().Size = new(960, 640); _focus = new(cell.X, 0, cell.Z); _camera.Size = 15; UpdateCamera(); await Frames();
            await Capture("artifacts/f12c-root-work.png");
            for (int i = 0; i < 100 && _world.Trees.Contains(tree); i++) _world.Tick(0.1f);
            await Frames(); Check(!_world.Trees.Contains(tree) && !_trees.ContainsKey(tree.Id), "Cleared tree visual remained");
            await UiClick(_kindButtons[BuildingKind.Cottage]); CloseDrawer();
            if (_rotation==0) await Press(Key.R); await Point(cell);
            Check(_ghostValid && !_clearingTrees, "Cleared ground did not allow building preview");
            await Capture("artifacts/f12c-buildable.png");
            await Click(_camera.UnprojectPosition(new(cell.X, 0, cell.Z)));
            Check(_world.Cottages.Count == 1, "Construction on cleared ground failed");
            await Press(Key.T); Check(_plantingTrees && !_clearingTrees, "Planting mode conflicts with clearing"); await Press(Key.Escape);
            GD.Print("SMOKE PASS: clearing tool/C/Esc, markers, repeat marking/cancel, invalid targets, saved orders, physical hauling, digging animation, and building on reclaimed land at 960px.");
            GetTree().Quit();
        }
        catch (Exception e) { GD.PrintErr("CLEARING SMOKE FAIL: " + e); GetTree().Quit(1); }
        finally { foreach (string suffix in new[] { "", ".bak", ".tmp" }) if (File.Exists(_savePath + suffix)) File.Delete(_savePath + suffix); }
    }
}

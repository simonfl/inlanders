using Godot;
using Inlanders.Simulation;
using System;
using System.Threading.Tasks;

public partial class Game
{
    private async Task CheckPathsUi()
    {
        string original = _world.SaveJson();
        void Check(bool value, string message) { if (!value) throw new Exception(message); }
        async Task Move(Cell cell)
        {
            var pos = _camera.UnprojectPosition(new(cell.X, 0, cell.Z));
            Input.ParseInputEvent(new InputEventMouseMotion { Position = pos, GlobalPosition = pos });
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        }
        await Press(Key.P); await Move(new(3, 0));
        var start = _camera.UnprojectPosition(new(3, 0, 0));
        Input.ParseInputEvent(new InputEventMouseButton { Position = start, ButtonIndex = MouseButton.Left, Pressed = true });
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        await Move(new(6, 0));
        Input.ParseInputEvent(new InputEventMouseButton { Position = _pointerPosition, ButtonIndex = MouseButton.Left, Pressed = false });
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        Check(_world.Paths.Count == 4 && _pathView.GetChildCount() > 4, "Drag skipped tiles or path connections missing");
        await Press(Key.Escape); await Capture("artifacts/f01-paths.png");
        string paved = _world.SaveJson(); _world = World.LoadJson(paved); CreateActors();
        TogglePaths(2); await Move(new(4, 0)); await Click(_camera.UnprojectPosition(new(4, 0, 0)));
        Check(!_world.Paths.Contains(new(4, 0)) && _world.Paths.Count == 3, "Path erasing failed");
        await Press(Key.Escape); await UiClick(_kindButtons[BuildingKind.Cottage]);
        Check(_pathTool == 0, "Building mode retained path brush");
        await Press(Key.Escape);
        _world = World.LoadJson(original); CreateActors(); CloseManagementUi();
        GD.Print("SMOKE PASS: path drag interpolation, connected visuals, erasing, saved paths, and mode switching.");
    }
}

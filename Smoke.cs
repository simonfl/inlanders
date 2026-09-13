using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async Task OpenMenu(int index)
    {
        if (!_drawer.Visible || _tabs.CurrentTab != index) await UiClick(_menuButtons[index]);
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
    }
    private async Task UiClick(Button button)
    {
        if(_mainScroll!=null && _mainScroll.IsAncestorOf(button))
        {
            _mainScroll.EnsureControlVisible(button);
            await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
        }
        if (_inspectionScroll.IsAncestorOf(button))
        {
            _inspectionScroll.EnsureControlVisible(button);
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        }
        for (int section = 0; section < _buildSections.Length; section++)
            if (_buildSections[section].IsAncestorOf(button)) SelectBuildSection(section);
        for (int i = 0; i < _drawerPages.Count; i++)
            if (_drawerPages[i].IsAncestorOf(button))
            {
                await OpenMenu(i); _drawerPages[i].EnsureControlVisible(button);
                await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame); break;
            }
        await ToSignal(RenderingServer.Singleton, RenderingServer.SignalName.FramePostDraw);
        if (!button.IsVisibleInTree()) throw new Exception("Cannot click hidden control: " + button.Text);
        await Click(button.GetGlobalRect().GetCenter());
    }
    private async Task Capture(string path)
    {
        await ToSignal(RenderingServer.Singleton, RenderingServer.SignalName.FramePostDraw);
        System.IO.Directory.CreateDirectory("artifacts");
        var result = GetViewport().GetTexture().GetImage().SavePng(path);
        if (result != Error.Ok) throw new Exception($"Screenshot failed: {result}");
    }
    private async Task Press(Key key)
    {
        Input.ParseInputEvent(new InputEventKey { Keycode = key, Pressed = true });
        Input.ParseInputEvent(new InputEventKey { Keycode = key, Pressed = false });
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
    }
    private async Task Click(Vector2 position)
    {
        Input.ParseInputEvent(new InputEventMouseButton { Position = position, GlobalPosition = position, ButtonIndex = MouseButton.Left, Pressed = true });
        Input.ParseInputEvent(new InputEventMouseButton { Position = position, GlobalPosition = position, ButtonIndex = MouseButton.Left, Pressed = false });
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
    }
}

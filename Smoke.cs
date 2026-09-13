using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    // Target the reviewed viewport synchronously; global ParseInputEvent queues delivery.
    private void ReviewInput(InputEvent input)=>GetViewport().PushInput(input,true);
    private async Task OpenMenu(int index)
    {
        if (!_drawer.Visible || _tabs.CurrentTab != index) await UiClick(_menuButtons[index]);
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
    }
    private async Task UiClick(Button button)
    {
        if(_inspectorSecondary!=null && _inspectorSecondary.IsAncestorOf(button) && !_inspectorSecondary.Visible) await UiClick(_inspectorDetails);
        if(_mainScroll!=null && _mainScroll.IsAncestorOf(button))
        {
            // Wrapped menu text and scrollbar appearance can trigger a second deferred layout.
            for(int frame=0;frame<4;frame++)
            {
                _mainScroll.EnsureControlVisible(button);
                await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
            }
        }
        if (_inspectionScroll.IsAncestorOf(button))
        {
            for(int frame=0;frame<4;frame++){_inspectionScroll.EnsureControlVisible(button);await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);}
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
        if (!button.IsVisibleInTree()){TraceMenuClick("hidden",button);await CaptureReviewBundle("hidden-control");throw new Exception("Cannot click hidden control: " + button.Text);}
        var hit=button.GetGlobalRect();
        for(Node? ancestor=button.GetParent();ancestor!=null;ancestor=ancestor.GetParent())
            if(ancestor is ScrollContainer scroll)hit=hit.Intersection(scroll.GetGlobalRect());
        if(!hit.HasArea())throw new Exception("Control remains clipped after scrolling: "+button.Text);
        if(_reviewRequest!=null)
        {
            var point=hit.GetCenter();
            void Activated()=>TraceMenuClick("pressed-signal",button,point);button.Pressed+=Activated;
            TraceMenuClick("before",button,point);
            ReviewInput(new InputEventMouseButton{Position=point,GlobalPosition=point,ButtonIndex=MouseButton.Left,Pressed=true});TraceMenuClick("down",button,point);
            ReviewInput(new InputEventMouseButton{Position=point,GlobalPosition=point,ButtonIndex=MouseButton.Left,Pressed=false});TraceMenuClick("up",GodotObject.IsInstanceValid(button)?button:null,point);
            await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);TraceMenuClick("next-frame",GodotObject.IsInstanceValid(button)?button:null,point);
            if(GodotObject.IsInstanceValid(button))button.Pressed-=Activated;
        }
        else await Click(hit.GetCenter());
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
        ReviewInput(new InputEventKey { Keycode = key, Pressed = true });
        ReviewInput(new InputEventKey { Keycode = key, Pressed = false });
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
    }
    private async Task Click(Vector2 position)
    {
        ReviewInput(new InputEventMouseButton { Position = position, GlobalPosition = position, ButtonIndex = MouseButton.Left, Pressed = true });
        ReviewInput(new InputEventMouseButton { Position = position, GlobalPosition = position, ButtonIndex = MouseButton.Left, Pressed = false });
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
    }
}



using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async Task CheckWatchUi()
    {
        void Check(bool ok,string message) { if(!ok) throw new Exception(message); }
        async Task Frames() { for(int i=0;i<4;i++) await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame); }
        var size=GetWindow().Size; _paused=true; _noticeUntil=0;
        await OpenMenu(0); SelectPerson(0); await Frames();
        string saved=_world.SaveJson(); int selected=_selectedPerson; _followPerson=true;
        await Press(Key.H); await Frames();
        Check(_watching && !_hud.Visible && _watchRoot.Visible && !_selection.Visible,"Watch did not hide management");
        Check(_world.SaveJson()==saved && _selectedPerson==selected && _followPerson,"Watch entry changed simulation or selection");
        foreach(var viewport in new[]{new Vector2I(1440,900),new(960,640)}) {
            GetWindow().Size=viewport; await Frames();
            var rect=_watchBar.GetGlobalRect();
            Check(rect.Position.X>=0 && rect.End.X<=viewport.X && rect.End.Y<=viewport.Y,"Watch controls overflow");
            await Capture($"artifacts/f21e-watch-{viewport.X}.png");
            bool labels=_showWorldLabels;
            var focusMode=_watchPause.FocusMode; _watchPause.FocusMode=Control.FocusModeEnum.All;
            _watchPause.GrabFocus(); await Press(Key.Tab); await Frames(); _watchPause.FocusMode=focusMode;
            Check(_cleanWatch && !_watchBar.Visible && !_hud.Visible,"Focused control swallowed clean-view shortcut");
            Check(GetTree().GetNodesInGroup("world_labels").OfType<Label3D>().All(l=>!l.Visible),"Clean view left world labels visible");
            var freshLabel=new Label3D { Text="new label" }; _dynamic.AddChild(freshLabel); await Frames();
            Check(!freshLabel.Visible,"New world label escaped clean view"); freshLabel.QueueFree();
            Check(_showWorldLabels==labels && _world.SaveJson()==saved,"Clean view changed preference or village");
            await Capture($"artifacts/f21e2-clean-{viewport.X}.png");
            await Press(Key.Tab); await Frames();
            Check(!_cleanWatch && _watchBar.Visible && WorldLabelsVisible==labels,"Tab did not restore Watch controls and label preference");
            await UiClick(_watchClean); await Frames(); Check(_cleanWatch,"Clean view button failed");
            await Press(Key.Tab); await Frames();
        }
        await Click(_camera.UnprojectPosition(_people[1].Body.Position)); await Frames();
        Check(_selectedPerson==selected && _world.SaveJson()==saved,"Watch click changed selection or village");
        await Click(_watchSpeed.GetGlobalRect().GetCenter()); await Frames();
        Check(_speed==3,"Watch speed failed");
        float beforeTime=_world.Food.Time;
        await Click(_watchPause.GetGlobalRect().GetCenter()); await Frames();
        for(int i=0;i<60 && _world.Food.Time==beforeTime;i++) await Frames();
        Check(!_paused && _world.Food.Time>beforeTime,"Watch did not run simulation");
        await Press(Key.Space); await Frames(); Check(_paused,"Space did not pause watch");
        await Press(Key.Tab); await Frames(); await Press(Key.Escape); await Frames();
        Check(!_watching && _hud.Visible && !_watchRoot.Visible && _selectedPerson==selected,"Watch did not restore management");
        BeginPlacement(BuildingKind.Cottage); await Press(Key.H); await Frames();
        Check(!_placing && !_ghost.Visible && !_pathStroke,"Watch left a placement tool active");
        await Press(Key.B); await Frames();
        Check(!_watching && _drawer.Visible && _tabs.CurrentTab==1 && !_placing,"Build shortcut did not restore controls safely");
        await Press(Key.H); await Frames(); await Click(_watchReturn.GetGlobalRect().GetCenter()); await Frames();
        Check(!_watching,"Visible manage button failed");
        ToggleWatch(); ToggleCleanWatch(); AdoptWorld(_world); await Frames();
        Check(!_watching && !_cleanWatch && !_watchRoot.Visible && _hud.Visible,"World adoption left watch controls active");
        _speed=1; CloseManagementUi(); GetWindow().Size=size; await Frames();
        GD.Print("SMOKE PASS: watch entry/exit, selection preservation, camera-view clicks, pause/speed, live simulation, cancelled placement, management shortcut, and 1440/960 controls.");
    }
}

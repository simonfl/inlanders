using Godot;

public partial class Game
{
    private bool _watching;
    private bool _cleanWatch;
    private bool _watchOrbit;
    private Button _watchOrbitButton = null!;
    private Control _watchRoot = null!;
    private PanelContainer _watchBar = null!;
    private Button _watchPause = null!, _watchSpeed = null!, _watchReturn = null!, _watchClean = null!;

    private void MakeWatchUi()
    {
        var layer=new CanvasLayer { Layer=5 }; AddChild(layer);
        _watchRoot=new Control { MouseFilter=Control.MouseFilterEnum.Ignore, Theme=HudTheme() };
        layer.AddChild(_watchRoot); _watchRoot.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
        _watchBar=HudPanel(_watchRoot);
        var row=new HBoxContainer(); row.AddThemeConstantOverride("separation",8); _watchBar.AddChild(row);
        _watchReturn=Button("Manage [H]",ExitWatch,126); row.AddChild(_watchReturn);
        _watchPause=Button("Pause",TogglePause,90); row.AddChild(_watchPause);
        _watchSpeed=Button("1×",()=>_speed=_speed==1?3:_speed==3?6:1,48); row.AddChild(_watchSpeed);
        row.AddChild(Button("Frame map",FrameMap,100));
        _watchLabelsButton = Button("", ToggleWorldLabels, 100); row.AddChild(_watchLabelsButton); UpdateLabelButtons();
        _watchClean=Button("Clean view [Tab]",ToggleCleanWatch,140); row.AddChild(_watchClean);
        _watchOrbitButton=Button("Orbit [J]",ToggleWatchOrbit,130); row.AddChild(_watchOrbitButton);
        _watchOrbitButton.TooltipText="Slowly circle the current focus, even while paused. Stops following a resident. J stops the orbit; pan, zoom, Q/E, Frame map or a saved view takes over immediately.";
        _watchBar.TooltipText="WASD pan · Wheel zoom · Q/E orbit · Space pause · 1–3 saved views · Ctrl+1–3 saves · Tab hides/restores controls and labels · H or Esc returns to management";
        _watchRoot.Hide();
    }
    private void ToggleWatch()
    {
        if(_watching) { ExitWatch(); return; }
        if(_atMainMenu) return;
        StopResourceSurvey();
        _watching=true;
        // Cancel the placement gesture, but retain the drawer, selection and camera follow.
        _placing=false; _pathStroke=false; _lastPathCell=null;
        RefreshGhost(); _selection.Hide(); _hud.Hide(); _watchRoot.Show();
        UpdateWatchUi();
    }
    private void ExitWatch()
    {
        if(!_watching) return;
        _watching=false; _cleanWatch=false; _watchOrbit=false; _watchBar.Show(); ApplyWorldLabels(); _watchRoot.Hide(); _hud.Show(); _selection.Show();
        RefreshSelection(); LayoutHud();
    }
    private void ToggleCleanWatch()
    {
        if(!_watching) return;
        _cleanWatch=!_cleanWatch; _watchBar.Visible=!_cleanWatch;
        if(_cleanWatch) GetViewport().GuiReleaseFocus();
        ApplyWorldLabels();
    }
    private void ToggleWatchOrbit()
    {
        if(!_watching) return;
        _watchOrbit=!_watchOrbit;
        if(_watchOrbit) _followPerson=false;
    }
    private void AdvanceWatchOrbit(float dt)
    {
        if(!_watching || !_watchOrbit) return;
        _angle=Mathf.PosMod(_angle+dt*Mathf.Tau/120f,Mathf.Tau);
        UpdateCamera();
    }
    private void UpdateWatchUi()
    {
        if(!_watching) return;
        _watchBar.Size=new(840,58);
        _watchBar.Position=new((_watchRoot.Size.X-_watchBar.Size.X)/2,_watchRoot.Size.Y-74);
        _watchPause.Text=_paused?"Resume":"Pause";
        _watchSpeed.Text=$"{_speed}×";
        _watchOrbitButton.Text=_watchOrbit?"Stop orbit [J]":"Orbit [J]";
        _selection.Hide();
    }
}

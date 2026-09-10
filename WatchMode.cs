using Godot;

public partial class Game
{
    private bool _watching;
    private Control _watchRoot = null!;
    private PanelContainer _watchBar = null!;
    private Button _watchPause = null!, _watchSpeed = null!, _watchReturn = null!;

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
        _watchBar.TooltipText="WASD pan · Wheel zoom · Q/E orbit · Space pause · 1–3 saved views · Ctrl+1–3 saves · H or Esc returns to management";
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
        _watching=false; _watchRoot.Hide(); _hud.Show(); _selection.Show();
        RefreshSelection(); LayoutHud();
    }
    private void UpdateWatchUi()
    {
        if(!_watching) return;
        _watchBar.Size=new(520,58);
        _watchBar.Position=new((_watchRoot.Size.X-_watchBar.Size.X)/2,_watchRoot.Size.Y-74);
        _watchPause.Text=_paused?"Resume":"Pause";
        _watchSpeed.Text=$"{_speed}×";
        _selection.Hide();
    }
}

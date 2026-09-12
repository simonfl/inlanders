using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Game
{
    private readonly List<(string Key,Control Control)> _menuControls=new();
    private readonly Dictionary<string,string> _menuFocusMemory=new();
    private string _menuPageTitle="";
    private int _menuPageRevision;
    private Action? _menuBack;
    private Panel _menuFocusRing=null!;

    private void RegisterMenuControl(Control control,string key)
    {
        control.FocusMode=Control.FocusModeEnum.All;
        if(control is BaseButton)control.AddThemeStyleboxOverride("focus",new StyleBoxEmpty());
        _menuControls.Add((key,control));
        control.FocusEntered+=()=>
        {
            _menuFocusMemory[_menuPageTitle]=key;
            _mainScroll.EnsureControlVisible(control);
            RevealMenuFocusAfterLayout(control,_menuPageRevision);
        };
    }
    private async void RevealMenuFocusAfterLayout(Control control,int revision)
    {
        // A remembered late entry can receive focus before the new page has a scroll range.
        await ToSignal(RenderingServer.Singleton,RenderingServer.SignalName.FramePostDraw);
        if(_atMainMenu && revision==_menuPageRevision && GodotObject.IsInstanceValid(control) && control.HasFocus())
            _mainScroll.EnsureControlVisible(control);
    }
    private Control[] EnabledMenuControls()=>_menuControls.Select(c=>c.Control)
        .Where(c=>c.IsVisibleInTree() && !(c is BaseButton b && b.Disabled)).ToArray();
    private void FocusMenuPage(int revision)
    {
        if(!_atMainMenu || revision!=_menuPageRevision)return;
        var enabled=EnabledMenuControls();
        var remembered=_menuFocusMemory.GetValueOrDefault(_menuPageTitle);
        var preferred=_menuControls.FirstOrDefault(c=>c.Key==remembered && enabled.Contains(c.Control)).Control;
        (preferred??enabled.FirstOrDefault())?.GrabFocus();
    }
    private void UpdateMainMenuFocus()
    {
        if(_menuFocusRing==null)return;
        var focus=GetViewport().GuiGetFocusOwner();
        _menuFocusRing.Visible=_atMainMenu && focus!=null && _mainColumn.IsAncestorOf(focus);
        if(!_menuFocusRing.Visible)return;
        var rect=focus!.GetGlobalRect().Intersection(_mainScroll.GetGlobalRect());
        _menuFocusRing.Position=rect.Position-_mainMenu.GlobalPosition;
        _menuFocusRing.Size=rect.Size;
    }
    private void HandleMainMenuKey(InputEvent input)
    {
        if(input is not InputEventKey { Pressed:true } key)return;
        var controls=EnabledMenuControls();var focus=GetViewport().GuiGetFocusOwner();
        if(key.Keycode==Key.Escape)
        {
            if(!key.Echo)_menuBack?.Invoke();
            GetViewport().SetInputAsHandled();return;
        }
        if(key.Keycode is Key.Tab or Key.Up or Key.Down)
        {
            int direction=key.Keycode==Key.Up || key.Keycode==Key.Tab && key.ShiftPressed?-1:1;
            int index=Array.IndexOf(controls,focus);
            if(controls.Length>0)controls[(index<0?(direction>0?0:controls.Length-1):(index+direction+controls.Length)%controls.Length)].GrabFocus();
            GetViewport().SetInputAsHandled();return;
        }
        if(key.Keycode is Key.Left or Key.Right && focus is HSlider slider)
        {
            slider.Value+=key.Keycode==Key.Left?-slider.Step:slider.Step;
            SaveAudioSettings();GetViewport().SetInputAsHandled();
        }
        // Enter/Space activation stays with the focused native button, on release.
    }
    private void ConfirmMenu(string action,string explanation,Action accept,Action back)
    {
        string title=action+"?";
        _menuFocusMemory.Remove(title);MenuPage(title);
        _mainColumn.AddChild(Text(explanation,16,true));
        MenuButton("Cancel",back);MenuButton(action,accept);
    }
}

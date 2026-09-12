using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;

public partial class Game
{
    private bool _catalogKeyboard;
    private BuildingKind _catalogLastKind=BuildingKind.Cottage;
    private bool CatalogOpen=>!_watching && _hud.IsVisibleInTree() && _drawer.Visible && _tabs.CurrentTab==1 && _buildSection==0;
    private Control[] CatalogControls()=>new Control[]{_buildingFilter}.Concat(_kindButtons.Values)
        .Where(c=>c.IsVisibleInTree() && !(c is BaseButton b && b.Disabled)).ToArray();
    private void StopCatalogKeyboard()
    {
        if(!_catalogKeyboard)return;
        _catalogKeyboard=false;
        var focus=GetViewport().GuiGetFocusOwner();
        if(focus==_buildingFilter || _kindButtons.Values.Any(b=>b==focus))focus?.ReleaseFocus();
        _buildingFilter.FocusMode=Control.FocusModeEnum.None;
        foreach(var b in _kindButtons.Values)b.FocusMode=Control.FocusModeEnum.None;
    }
    private async void FocusCatalogControl(Control control)
    {
        control.GrabFocus();_drawerPages[1].EnsureControlVisible(control);
        await ToSignal(RenderingServer.Singleton,RenderingServer.SignalName.FramePostDraw);
        if(_catalogKeyboard && CatalogOpen && control.HasFocus())_drawerPages[1].EnsureControlVisible(control);
    }
    private void OpenKeyboardCatalog()
    {
        if(_watching)ExitWatch();
        CancelCameraDrag();_pathStroke=_woodlandStroke=false;
        ClearSelection();_placing=false;RefreshGhost();
        if(!_drawer.Visible || _tabs.CurrentTab!=1)ToggleDrawer(1);
        SelectBuildSection(0);UpdateVillageDirectory();
        _catalogKeyboard=true;
        foreach(var control in new Control[]{_buildingFilter}.Concat(_kindButtons.Values))
        {
            control.FocusMode=Control.FocusModeEnum.All;
            var style=new StyleBoxFlat { BgColor=new(0,0,0,0),BorderColor=new("efd08c"),BorderWidthLeft=2,BorderWidthRight=2,BorderWidthTop=2,BorderWidthBottom=2,CornerRadiusTopLeft=7,CornerRadiusTopRight=7,CornerRadiusBottomLeft=7,CornerRadiusBottomRight=7 };
            control.AddThemeStyleboxOverride("focus",style);
        }
        var preferred=_kindButtons[_catalogLastKind];
        FocusCatalogControl(preferred.IsVisibleInTree() && !preferred.Disabled?preferred:_buildingFilter);
    }
    private bool HandleCatalogKeyboard(InputEvent input)
    {
        if(_catalogKeyboard && (!CatalogOpen || EditingText))StopCatalogKeyboard();
        if(input is InputEventMouseButton {Pressed:true,ButtonIndex:MouseButton.Left or MouseButton.Right or MouseButton.Middle})StopCatalogKeyboard();
        if(input is not InputEventKey {Pressed:true} key || EditingText)return false;
        if(key.Keycode==Key.B && !key.CtrlPressed && !key.AltPressed)
        {
            if(!key.Echo){if(_catalogKeyboard){StopCatalogKeyboard();CloseDrawer();}else OpenKeyboardCatalog();}
            return true;
        }
        if(!_catalogKeyboard)return false;
        if(key.Keycode is Key.H or Key.V or Key.G or Key.O or Key.I){StopCatalogKeyboard();return false;}
        if(key.Keycode==Key.Escape){StopCatalogKeyboard();CloseDrawer();return true;}
        var controls=CatalogControls();var focus=GetViewport().GuiGetFocusOwner();
        if(key.Keycode is Key.Tab or Key.Up or Key.Down)
        {
            int step=key.Keycode==Key.Up || key.Keycode==Key.Tab && key.ShiftPressed?-1:1;
            int index=Array.IndexOf(controls,focus);
            if(controls.Length>0)FocusCatalogControl(controls[index<0?0:(index+step+controls.Length)%controls.Length]);
        }
        else if(focus==_buildingFilter && key.Keycode is Key.Left or Key.Right or Key.Enter or Key.Space)
        {
            int step=key.Keycode==Key.Left?-1:1;
            _buildingFilter.Select((_buildingFilter.Selected+step+_buildingFilter.ItemCount)%_buildingFilter.ItemCount);
            UpdateVillageDirectory();FocusCatalogControl(_buildingFilter);
        }
        else if(!key.Echo && key.Keycode is Key.Enter or Key.Space && focus is Button button && !button.Disabled && !_world.Food.Celebrating)
            button.EmitSignal(BaseButton.SignalName.Pressed);
        return true; // Focused catalog keys never reach world shortcuts.
    }
    private string CatalogKeyboardHint()
    {
        var focus=GetViewport().GuiGetFocusOwner();
        string keys="Tab / ↑↓ select · Enter place · Esc close";
        if(_world.Food.Celebrating)return "Wait until supper is over to place buildings. Esc closes the catalog.";
        if(focus==_buildingFilter)return "Building categories · ←→ change category · Tab to cards · Esc close";
        foreach(var (kind,button) in _kindButtons)if(button==focus)
        {
            _catalogLastKind=kind;
            return $"{BuildingName(kind)} · {(_world.Creative?"Free · instant":Buildings.Get(kind).CostText)}\n{BuildingDescription(kind)}\n{keys}";
        }
        return _world.Food.Celebrating?"Wait until supper is over to place buildings. Esc closes the catalog.":keys;
    }
}

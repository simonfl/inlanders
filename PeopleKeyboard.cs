using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Game
{
    private bool _peopleKeyboard;
    private int _peopleKeyboardPerson=-1, _peopleLastPerson;
    private Button _peopleBackButton=null!;
    private readonly Dictionary<Control,Control.FocusModeEnum> _peopleFocusModes=new();
    private bool PeopleKeyboardOpen=>!_watching && _hud.IsVisibleInTree() &&
        (_peopleKeyboardPerson>=0 ? _inspector.Visible && _selectedPerson==_peopleKeyboardPerson : _drawer.Visible && _tabs.CurrentTab==0);
    private Control[] PeopleKeyboardControls()=> (_peopleKeyboardPerson>=0
        ? new Control[]{_peopleBackButton,_jobChoice,_assignButton}
        : new Control[]{_rosterFilter}.Concat(_roster).ToArray())
        .Where(c=>c.IsVisibleInTree() && !(c is BaseButton b && b.Disabled)).ToArray();
    private void StopPeopleKeyboard()
    {
        if(!_peopleKeyboard)return;
        _peopleKeyboard=false;_peopleKeyboardPerson=-1;_peopleBackButton.Hide();
        foreach(var (control,mode) in _peopleFocusModes)
            if(IsInstanceValid(control)){if(control.HasFocus())control.ReleaseFocus();control.FocusMode=mode;}
        _peopleFocusModes.Clear();
    }
    private async void FocusPeopleControl(Control control)
    {
        if(!_peopleFocusModes.ContainsKey(control))
        {
            _peopleFocusModes[control]=control.FocusMode;control.FocusMode=Control.FocusModeEnum.All;
            control.AddThemeStyleboxOverride("focus",new StyleBoxFlat{BgColor=new(0,0,0,0),BorderColor=new("efd08c"),BorderWidthLeft=2,BorderWidthRight=2,BorderWidthTop=2,BorderWidthBottom=2,CornerRadiusTopLeft=7,CornerRadiusTopRight=7,CornerRadiusBottomLeft=7,CornerRadiusBottomRight=7});
        }
        control.GrabFocus();var scroll=_peopleKeyboardPerson>=0?_inspectionScroll:_drawerPages[0];
        scroll.EnsureControlVisible(control);
        await ToSignal(RenderingServer.Singleton,RenderingServer.SignalName.FramePostDraw);
        if(_peopleKeyboard && PeopleKeyboardOpen && IsInstanceValid(control) && control.HasFocus())scroll.EnsureControlVisible(control);
    }
    private void OpenPeopleKeyboard()
    {
        StopCatalogKeyboard();StopPeopleKeyboard();if(_watching)ExitWatch();
        CancelCameraDrag();CancelDecorationStroke();_pathStroke=_woodlandStroke=false;_placing=false;RefreshGhost();
        ClearSelection();if(!_drawer.Visible || _tabs.CurrentTab!=0)ToggleDrawer(0);
        UpdateVillageDirectory();_peopleKeyboard=true;
        var preferred=_peopleLastPerson>=0 && _peopleLastPerson<_roster.Count?_roster[_peopleLastPerson]:null;
        FocusPeopleControl(preferred!=null && preferred.IsVisibleInTree()?preferred:_rosterFilter);
    }
    private void UpdatePeopleKeyboard()
    {
        if(!_peopleKeyboard)return;
        if(!PeopleKeyboardOpen || EditingText){StopPeopleKeyboard();return;}
        UpdateManagementControls();
        var controls=PeopleKeyboardControls();var focus=GetViewport().GuiGetFocusOwner();
        if(!controls.Contains(focus) && controls.Length>0)FocusPeopleControl(controls[0]);
    }
    private bool HandlePeopleKeyboard(InputEvent input)
    {
        if(_peopleKeyboard && (!PeopleKeyboardOpen || EditingText))StopPeopleKeyboard();
        if(input is InputEventMouseButton{Pressed:true,ButtonIndex:MouseButton.Left} click && _peopleKeyboard && _peopleBackButton.IsVisibleInTree() && _peopleBackButton.GetGlobalRect().HasPoint(click.Position))
        {OpenPeopleKeyboard();StopPeopleKeyboard();return true;}
        if(input is InputEventMouseButton{Pressed:true,ButtonIndex:MouseButton.Left or MouseButton.Right or MouseButton.Middle})StopPeopleKeyboard();
        if(input is not InputEventKey{Pressed:true} key || EditingText)return false;
        if(key.Keycode==Key.V && !key.CtrlPressed && !key.AltPressed)
        {
            if(!key.Echo){if(_peopleKeyboard){StopPeopleKeyboard();CloseDrawer();ClearSelection();}else OpenPeopleKeyboard();}return true;
        }
        if(!_peopleKeyboard)return false;
        if(key.Keycode is Key.B or Key.H or Key.G or Key.O or Key.I){StopPeopleKeyboard();return false;}
        if(key.Keycode==Key.Escape)
        {
            if(_peopleKeyboardPerson>=0)OpenPeopleKeyboard();else{StopPeopleKeyboard();CloseDrawer();}return true;
        }
        var controls=PeopleKeyboardControls();var focus=GetViewport().GuiGetFocusOwner();
        if(key.Keycode is Key.Tab or Key.Up or Key.Down)
        {
            int step=key.Keycode==Key.Up || key.Keycode==Key.Tab && key.ShiftPressed?-1:1;
            int index=Array.IndexOf(controls,focus);
            if(controls.Length>0)FocusPeopleControl(controls[index<0?0:(index+step+controls.Length)%controls.Length]);
        }
        else if(focus is OptionButton choice && !choice.Disabled && key.Keycode is Key.Left or Key.Right)
        {
            choice.Select((choice.Selected+(key.Keycode==Key.Left?-1:1)+choice.ItemCount)%choice.ItemCount);
            UpdateVillageDirectory();UpdateManagementControls();
        }
        else if(!key.Echo && key.Keycode is Key.Enter or Key.Space && controls.Contains(focus))
        {
            int id=_roster.FindIndex(b=>b==focus);
            if(_peopleKeyboardPerson<0 && id>=0 && _world.People.Any(p=>p.Id==id))
            {
                _peopleLastPerson=id;_peopleKeyboardPerson=id;SelectPerson(id);_peopleBackButton.Show();UpdateHud();
                FocusPeopleControl(_jobChoice.Disabled?_peopleBackButton:_jobChoice);
            }
            else if(focus==_peopleBackButton)OpenPeopleKeyboard();
            else if(focus==_assignButton && !_assignButton.Disabled && _selectedPerson==_peopleKeyboardPerson)
            { _assignButton.EmitSignal(BaseButton.SignalName.Pressed);UpdateManagementControls();FocusPeopleControl(_jobChoice); }
        }
        return true;
    }
    private string PeopleKeyboardHint()
    {
        if(_peopleKeyboardPerson>=0)return "←→ choose role · Tab to Assign · Enter confirms\nEsc returns to residents · V closes";
        var focus=GetViewport().GuiGetFocusOwner();int id=_roster.FindIndex(b=>b==focus);
        return (id>=0?$"{_world.People[id].Name} · {_world.People[id].Role} · {_world.People[id].Status}\n":"←→ filter residents · ")+"Tab / ↑↓ select · Enter inspect · Esc close";
    }
}

using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public partial class Game
{
    private bool _economyKeyboard,_economyInspecting;
    private int _economySite=-1,_economyPerson=-1;
    private string _economyFocusKey="survey",_economyReturnKey="survey";
    private Button _economyBack=null!;
    private readonly Dictionary<Control,Control.FocusModeEnum> _economyFocusModes=new();
    private bool EconomyKeyboardOpen=>!_watching && _hud.IsVisibleInTree() && (_economyInspecting
        ? _inspector.Visible && _selectedSite==_economySite && _selectedPerson==_economyPerson
        : _drawer.Visible && _tabs.CurrentTab==4);
    private (string Key,Button Button)[] EconomyControls()
    {
        var rows=new List<(string,Button)>();
        if(_economyInspecting)rows.Add(("back",_economyBack));
        else
        {
            rows.Add(("survey",_surveyToggle));rows.Add(("meals",_mealAttention));rows.Add(("bread",_breadToggle));
            rows.AddRange(_breadPlaceLinks.Select(p=>($"bread:{p.Key}",p.Value)));
            for(int i=0;i<_economyIssues.Count && i<(_economyReport?.Issues.Length??0);i++)rows.Add(($"issue:{_economyReport!.Issues[i].Id}",_economyIssues[i]));
            rows.Add(("routes",_supplyToggle));
            rows.AddRange(_supplyLinks.GetChildren().OfType<Button>().Where(b=>b.HasMeta("worker")).Select(b=>($"route:{(int)b.GetMeta("worker")}",b)));
            rows.Add(("yard",_yardLink));rows.AddRange(_storageLinks.Select(p=>($"store:{p.Key}",p.Value)));
            for(int id=0;id<_idleLinks.Count;id++)rows.Add(($"idle:{id}",_idleLinks[id]));
        }
        return rows.Where(r=>IsInstanceValid(r.Item2) && r.Item2.IsVisibleInTree() && !r.Item2.Disabled).ToArray();
    }
    private void StopEconomyKeyboard()
    {
        _economyKeyboard=false;_economyInspecting=false;if(_economyBack!=null)_economyBack.Hide();
        foreach(var (control,mode) in _economyFocusModes)if(IsInstanceValid(control))
        {if(control.HasFocus())control.ReleaseFocus();control.FocusMode=mode;}
        _economyFocusModes.Clear();
    }
    private async void FocusEconomy(string key)
    {
        var rows=EconomyControls();if(rows.Length==0)return;
        var row=rows.FirstOrDefault(r=>r.Key==key);if(row.Button==null)row=rows[0];
        _economyFocusKey=row.Key;var control=row.Button;
        if(!_economyFocusModes.ContainsKey(control))
        {
            _economyFocusModes[control]=control.FocusMode;control.FocusMode=Control.FocusModeEnum.All;
            control.AddThemeStyleboxOverride("focus",new StyleBoxFlat{BgColor=new(0,0,0,0),BorderColor=new("efd08c"),BorderWidthLeft=2,BorderWidthRight=2,BorderWidthTop=2,BorderWidthBottom=2,CornerRadiusTopLeft=7,CornerRadiusTopRight=7,CornerRadiusBottomLeft=7,CornerRadiusBottomRight=7});
        }
        control.GrabFocus();var scroll=_economyInspecting?_inspectionScroll:_drawerPages[4];scroll.EnsureControlVisible(control);
        await ToSignal(RenderingServer.Singleton,RenderingServer.SignalName.FramePostDraw);
        if(_economyKeyboard && EconomyKeyboardOpen && IsInstanceValid(control) && control.HasFocus())scroll.EnsureControlVisible(control);
    }
    private void OpenEconomyKeyboard(bool returning=false)
    {
        string preferred=returning?_economyReturnKey:_economyFocusKey;
        StopEconomyKeyboard();StopPeopleKeyboard();StopCatalogKeyboard();if(_watching)ExitWatch();
        CancelCameraDrag();CancelDecorationStroke();_pathStroke=_woodlandStroke=false;_placing=false;RefreshGhost();ClearSelection();
        OpenEconomy();UpdateEconomyUi();_nextSupplyRefresh=0;RenderSupplyRoutes();_economyKeyboard=true;FocusEconomy(preferred);
    }
    private void UpdateEconomyKeyboard()
    {
        if(!_economyKeyboard)return;
        if(!EconomyKeyboardOpen || EditingText){StopEconomyKeyboard();return;}
        var row=EconomyControls().FirstOrDefault(r=>r.Key==_economyFocusKey);
        if(row.Button==null || GetViewport().GuiGetFocusOwner()!=row.Button)FocusEconomy(_economyFocusKey);
    }
    private bool HandleEconomyKeyboard(InputEvent input)
    {
        if(_economyKeyboard && (!EconomyKeyboardOpen || EditingText))StopEconomyKeyboard();
        if(input is InputEventMouseButton{Pressed:true,ButtonIndex:MouseButton.Left} click && _economyKeyboard && _economyInspecting && _economyBack.GetGlobalRect().HasPoint(click.Position))
        {OpenEconomyKeyboard(true);StopEconomyKeyboard();return true;}
        if(input is InputEventMouseButton{Pressed:true})StopEconomyKeyboard();
        if(input is not InputEventKey{Pressed:true} key || EditingText)return false;
        if(key.Keycode==Key.I && !key.CtrlPressed && !key.AltPressed)
        {
            if(!key.Echo){if(_economyKeyboard){StopEconomyKeyboard();CloseDrawer();ClearSelection();}else OpenEconomyKeyboard();}return true;
        }
        if(!_economyKeyboard)return false;
        if(key.Keycode is Key.B or Key.V or Key.G or Key.O or Key.H or Key.U){StopEconomyKeyboard();return false;}
        if(key.Keycode==Key.Escape)
        {if(_economyInspecting)OpenEconomyKeyboard(true);else{StopEconomyKeyboard();CloseDrawer();}return true;}
        if(key.Keycode is Key.Pagedown or Key.Pageup)
        {var scroll=_economyInspecting?_inspectionScroll:_drawerPages[4];scroll.ScrollVertical+=(key.Keycode==Key.Pagedown?1:-1)*(int)(scroll.Size.Y*.75f);return true;}
        string intended=_economyFocusKey;
        // Refresh identity before activation: reused issue/route rows must never
        // activate the new occupant of an old focused slot.
        if(!_economyInspecting){UpdateEconomyUi();_nextSupplyRefresh=0;RenderSupplyRoutes();}
        var rows=EconomyControls();int index=Array.FindIndex(rows,r=>r.Key==intended);
        if(index<0){FocusEconomy("survey");return true;}
        if(key.Keycode is Key.Tab or Key.Up or Key.Down)
        {int step=key.Keycode==Key.Up || key.Keycode==Key.Tab && key.ShiftPressed?-1:1;FocusEconomy(rows[(index+step+rows.Length)%rows.Length].Key);}
        else if(!key.Echo && key.Keycode is Key.Enter or Key.Space)
        {
            if(_economyInspecting){OpenEconomyKeyboard(true);return true;}
            _economyReturnKey=intended;rows[index].Button.EmitSignal(BaseButton.SignalName.Pressed);
            if(_inspector.Visible && (_selectedSite>=0 || _selectedPerson>=0))
            {_economyInspecting=true;_economySite=_selectedSite;_economyPerson=_selectedPerson;_economyBack.Show();FocusEconomy("back");}
            else if(_drawer.Visible && _tabs.CurrentTab==0){StopEconomyKeyboard();OpenPeopleKeyboard();}
            else if(_surveying){StopEconomyKeyboard();OpenSurveyKeyboard();}
            else if(!EconomyKeyboardOpen)StopEconomyKeyboard();
            else FocusEconomy(intended);
        }
        return true;
    }
}

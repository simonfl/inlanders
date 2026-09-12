using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Collections.Generic;

public partial class Game
{
    private bool _surveyKeyboard;
    private int _surveyKeyboardSite=-1;
    private string _surveyFocus="choice";
    private Button _surveyFinish=null!;
    private readonly Dictionary<Control,Control.FocusModeEnum> _surveyFocusModes=new();
    private bool SurveyKeyboardOpen=>_surveying && !_watching && _inspector.IsVisibleInTree() &&
        (_surveyKeyboardSite>=0?_selectedSite==_surveyKeyboardSite:_surveyDetails.IsVisibleInTree());
    private (string Key,Control Control)[] SurveyControls()
    {
        var rows=new List<(string,Control)>();
        if(_surveyKeyboardSite>=0)rows.Add(("back",_surveyBack));
        else
        {
            rows.Add(("choice",_sourceChoice));rows.AddRange(_sourceWorkplaceLinks.Select(p=>($"site:{p.Key}",(Control)p.Value)));rows.Add(("finish",_surveyFinish));
        }
        return rows.Where(r=>IsInstanceValid(r.Item2) && r.Item2.IsVisibleInTree() && !(r.Item2 is BaseButton b && b.Disabled)).ToArray();
    }
    private void StopSurveyKeyboard()
    {
        _surveyKeyboard=false;_surveyKeyboardSite=-1;
        foreach(var (control,mode) in _surveyFocusModes)if(IsInstanceValid(control)){if(control.HasFocus())control.ReleaseFocus();control.FocusMode=mode;}
        _surveyFocusModes.Clear();
    }
    private async void FocusSurvey(string key)
    {
        var rows=SurveyControls();if(rows.Length==0)return;var row=rows.FirstOrDefault(r=>r.Key==key);if(row.Control==null)row=rows[0];
        _surveyFocus=row.Key;var control=row.Control;
        if(!_surveyFocusModes.ContainsKey(control))
        {
            _surveyFocusModes[control]=control.FocusMode;control.FocusMode=Control.FocusModeEnum.All;
            control.AddThemeStyleboxOverride("focus",new StyleBoxFlat{BgColor=new(0,0,0,0),BorderColor=new("efd08c"),BorderWidthLeft=2,BorderWidthRight=2,BorderWidthTop=2,BorderWidthBottom=2,CornerRadiusTopLeft=7,CornerRadiusTopRight=7,CornerRadiusBottomLeft=7,CornerRadiusBottomRight=7});
        }
        control.GrabFocus();_inspectionScroll.EnsureControlVisible(control);
        await ToSignal(RenderingServer.Singleton,RenderingServer.SignalName.FramePostDraw);
        if(_surveyKeyboard && SurveyKeyboardOpen && IsInstanceValid(control) && control.HasFocus())_inspectionScroll.EnsureControlVisible(control);
    }
    private void OpenSurveyKeyboard()
    {
        StopGoalsKeyboard();StopEconomyKeyboard();StopPeopleKeyboard();StopCatalogKeyboard();StopSurveyKeyboard();
        CancelCameraDrag();CancelDecorationStroke();if(_watching)ExitWatch();CloseDrawer();
        if(!_surveying)ToggleResourceSurvey();
        else if(_selectedSource==null){ClearSelection();ShowInspector();}
        _surveyKeyboard=true;_surveyFocus="choice";_nextSourceRefresh=0;UpdateResourceSurvey();
    }
    private void ReturnSurveySource()
    {
        _surveyKeyboardSite=-1;
        if(_lastSurveySource is SourceKey source && _world.ReadResourceSurvey(source)!=null)SelectResourceSource(source,true);
        else{ClearSelection();ShowInspector();_nextSourceRefresh=0;UpdateResourceSurvey();}
        _surveyFocus="choice";
    }
    private void UpdateSurveyKeyboard()
    {
        if(!_surveyKeyboard)return;
        if(!SurveyKeyboardOpen || EditingText){StopSurveyKeyboard();return;}
        if(_surveyKeyboardSite>=0 && (!_world.Cottages.Any(c=>c.Id==_surveyKeyboardSite) || _lastSurveySource==null))ReturnSurveySource();
        var row=SurveyControls().FirstOrDefault(r=>r.Key==_surveyFocus);
        if(row.Control==null || GetViewport().GuiGetFocusOwner()!=row.Control)FocusSurvey(_surveyFocus);
    }
    private bool HandleSurveyKeyboard(InputEvent input)
    {
        if(_surveyKeyboard && (!SurveyKeyboardOpen || EditingText))StopSurveyKeyboard();
        if(input is InputEventMouseButton{Pressed:true})StopSurveyKeyboard();
        if(input is not InputEventKey{Pressed:true} key || EditingText)return false;
        if(key.Keycode==Key.U && !key.CtrlPressed && !key.AltPressed)
        {if(!key.Echo){if(_surveying)StopResourceSurvey();else OpenSurveyKeyboard();}return true;}
        if(!_surveyKeyboard)return false;
        if(key.Keycode is Key.G or Key.I or Key.V or Key.B or Key.O or Key.H){StopSurveyKeyboard();StopResourceSurvey();return false;}
        if(key.Keycode==Key.Escape){if(_surveyKeyboardSite>=0)ReturnSurveySource();else StopResourceSurvey();return true;}
        if(key.Keycode is Key.Pageup or Key.Pagedown)
        {_inspectionScroll.ScrollVertical+=(key.Keycode==Key.Pagedown?1:-1)*(int)(_inspectionScroll.Size.Y*.75f);return true;}
        string intended=_surveyFocus;_nextSourceRefresh=0;UpdateResourceSurvey();
        var rows=SurveyControls();int index=Array.FindIndex(rows,r=>r.Key==intended);
        if(index<0){FocusSurvey("choice");return true;}
        if(key.Keycode is Key.Tab or Key.Up or Key.Down)
        {int step=key.Keycode==Key.Up || key.Keycode==Key.Tab && key.ShiftPressed?-1:1;FocusSurvey(rows[(index+step+rows.Length)%rows.Length].Key);}
        else if(intended=="choice" && key.Keycode is Key.Left or Key.Right && _sourceList.Count>0)
        {
            int current=_sourceList.FindIndex(s=>s.Key==_selectedSource);
            int next=current<0?(key.Keycode==Key.Left?_sourceList.Count-1:0):(current+(key.Keycode==Key.Left?-1:1)+_sourceList.Count)%_sourceList.Count;
            SelectResourceSource(_sourceList[next].Key,true);FocusSurvey("choice");
        }
        else if(!key.Echo && key.Keycode is Key.Enter or Key.Space)
        {
            if(intended=="back")ReturnSurveySource();
            else if(intended=="choice" && _selectedSource==null && _sourceList.Count>0){SelectResourceSource(_sourceList[0].Key,true);FocusSurvey("choice");}
            else if(rows[index].Control is Button button)
            {
                button.EmitSignal(BaseButton.SignalName.Pressed);
                if(_surveying && _selectedSite>=0){_surveyKeyboardSite=_selectedSite;UpdateResourceSurvey();FocusSurvey("back");}
            }
        }
        return true;
    }
}

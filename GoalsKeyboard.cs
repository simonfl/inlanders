using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public partial class Game
{
    private bool _goalsKeyboard;
    private int _goalsPage=2,_goalsInspectPage=-1,_goalsSite=-1,_goalsPerson=-1;
    private string _goalsFocus="overview",_goalsReturn="overview",_goalsInspectReturn="overview",_goalsVersion="";
    private Button _goalsOverview=null!,_goalsBackList=null!,_goalsBackInspect=null!;
    private readonly Dictionary<Control,Control.FocusModeEnum> _goalsFocusModes=new();
    private string GoalsVersion=>$"{_world.Campaign?.Level}:{_world.Campaign?.Complete}:{_world.CampaignPhaseTitle}";
    private bool GoalsKeyboardOpen=>!_watching && _hud.IsVisibleInTree() && (_goalsInspectPage>=0
        ? _inspector.Visible && _selectedSite==_goalsSite && _selectedPerson==_goalsPerson
        : _drawer.Visible && _tabs.CurrentTab==_goalsPage);
    private (string Key,Control Control)[] GoalsControls()
    {
        var rows=new List<(string,Control)>();
        if(_goalsInspectPage>=0)rows.Add(("back",_goalsBackInspect));
        else if(_goalsPage==0)
        {
            rows.Add(("back",_goalsBackList));rows.Add(("filter",_serviceFilter));
            foreach(var (id,r) in _serviceResidents)
            {rows.Add(($"person:{id}",r.Person));rows.Add(($"home:{id}:{_world.People[id].HomeId}",r.Home));rows.Add(($"venue:{id}:{ServiceVenue(_world.People[id])}",r.Venue));}
        }
        else
        {
            rows.Add(("overview",_goalsOverview));rows.Add(("phase:"+GoalsVersion,_riverAction));
            foreach(var (key,item) in _goalItems)
            {
                rows.Add(($"why:{key}",item.Toggle));
                if(_goalTrackButtons.TryGetValue(key,out var track))rows.Add(($"track:{key}",track));
                if(_goalResidentButtons.TryGetValue(key,out var residents))rows.Add(($"residents:{key}",residents));
                if(_goalPlaces.TryGetValue(key,out var places))foreach(var b in places.GetChildren().OfType<Button>())
                    rows.Add(($"place:{key}:"+(b.HasMeta("place_id")?b.GetMeta("place_id").ToString():b.Text),b));
            }
            rows.AddRange(new (string,Control)[]{("meal-why",_goalMealExplain),("meal-people",_goalMealPeople),("meal-economy",_goalMealEconomy),
                ("supper",_supperButton),("bread",_supperBreadLink),("dismiss:"+_world.CurrentCampaignHint()?.Id,_dismissHint),("guidance",_guidance),("hints",_reopenHints),
                ("continue",_keepPlaying),("next",_nextLevel),("replay",_replayLevel),("restore",_restoreReplay),
                ("visitor-accept:"+_world.Gardener,_visitorAccept),("visitor-decline:"+_world.Gardener,_visitorDecline),("visitor-plant",_visitorPlant)});
            for(int i=0;i<_levelButtons.Count;i++)rows.Add(($"level:{i+1}",_levelButtons[i]));
            rows.Add(("standalone",_standaloneLevel));
        }
        return rows.Where(r=>IsInstanceValid(r.Item2) && r.Item2.IsVisibleInTree() && !(r.Item2 is BaseButton b && b.Disabled)).ToArray();
    }
    private void StopGoalsKeyboard()
    {
        _goalsKeyboard=false;_goalsInspectPage=-1;_goalsPage=2;
        _goalsOverview?.Hide();_goalsBackList?.Hide();_goalsBackInspect?.Hide();
        foreach(var (control,mode) in _goalsFocusModes)if(IsInstanceValid(control)){if(control.HasFocus())control.ReleaseFocus();control.FocusMode=mode;}
        _goalsFocusModes.Clear();
    }
    private async void FocusGoal(string key)
    {
        var rows=GoalsControls();if(rows.Length==0)return;
        var row=rows.FirstOrDefault(r=>r.Key==key);if(row.Control==null)row=rows[0];
        _goalsFocus=row.Key;var control=row.Control;
        if(!_goalsFocusModes.ContainsKey(control))
        {
            _goalsFocusModes[control]=control.FocusMode;control.FocusMode=Control.FocusModeEnum.All;
            control.AddThemeStyleboxOverride("focus",new StyleBoxFlat{BgColor=new(0,0,0,0),BorderColor=new("efd08c"),BorderWidthLeft=2,BorderWidthRight=2,BorderWidthTop=2,BorderWidthBottom=2,CornerRadiusTopLeft=7,CornerRadiusTopRight=7,CornerRadiusBottomLeft=7,CornerRadiusBottomRight=7});
        }
        control.GrabFocus();if(control==_goalsBackList)return;
        var scroll=_goalsInspectPage>=0?_inspectionScroll:_drawerPages[_goalsPage];scroll.EnsureControlVisible(control);
        await ToSignal(RenderingServer.Singleton,RenderingServer.SignalName.FramePostDraw);
        if(_goalsKeyboard && GoalsKeyboardOpen && IsInstanceValid(control) && control.HasFocus())scroll.EnsureControlVisible(control);
    }
    private void OpenGoalsKeyboard(string key="overview")
    {
        StopGoalsKeyboard();StopEconomyKeyboard();StopPeopleKeyboard();StopCatalogKeyboard();if(_watching)ExitWatch();
        StopResourceSurvey();CancelCameraDrag();CancelDecorationStroke();_placing=false;_pathStroke=_woodlandStroke=false;RefreshGhost();ClearSelection();
        if(!_drawer.Visible || _tabs.CurrentTab!=2)ToggleDrawer(2);
        UpdateCampaignUi();_goalsKeyboard=true;_goalsOverview.Show();_goalsVersion=GoalsVersion;_goalsFocus=key;
    }
    private void BackFromGoalsLink()
    {
        if(_goalsInspectPage==0)
        {
            _goalsInspectPage=-1;_goalsBackInspect.Hide();ClearSelection();
            if(!_drawer.Visible || _tabs.CurrentTab!=0)ToggleDrawer(0);
            _goalsPage=0;_goalsBackList.Show();UpdateServiceCoverage();_goalsFocus=_goalsInspectReturn;
        }
        else OpenGoalsKeyboard(_goalsReturn);
    }
    private void UpdateGoalsKeyboard()
    {
        if(!_goalsKeyboard)return;
        if(!GoalsKeyboardOpen || EditingText){StopGoalsKeyboard();return;}
        if(_goalsVersion!=GoalsVersion){OpenGoalsKeyboard();return;}
        var row=GoalsControls().FirstOrDefault(r=>r.Key==_goalsFocus);
        if(row.Control==null || GetViewport().GuiGetFocusOwner()!=row.Control)FocusGoal(_goalsFocus);
    }
    private bool HandleGoalsKeyboard(InputEvent input)
    {
        if(_goalsKeyboard && (!GoalsKeyboardOpen || EditingText))StopGoalsKeyboard();
        if(input is InputEventMouseButton{Pressed:true,ButtonIndex:MouseButton.Left} click && _goalsKeyboard &&
            new[]{_goalsBackList,_goalsBackInspect}.Any(b=>b.IsVisibleInTree() && b.GetGlobalRect().HasPoint(click.Position)))
        {BackFromGoalsLink();StopGoalsKeyboard();return true;}
        if(input is InputEventMouseButton{Pressed:true})StopGoalsKeyboard();
        if(input is not InputEventKey{Pressed:true} key || EditingText)return false;
        if(key.Keycode==Key.G && !key.CtrlPressed && !key.AltPressed)
        {if(!key.Echo){if(_goalsKeyboard){StopGoalsKeyboard();CloseDrawer();ClearSelection();}else OpenGoalsKeyboard();}return true;}
        if(!_goalsKeyboard)return false;
        if(key.Keycode is Key.I or Key.V or Key.B or Key.O or Key.H or Key.U){StopGoalsKeyboard();return false;}
        if(key.Keycode==Key.Escape)
        {if(_goalsInspectPage>=0 || _goalsPage!=2)BackFromGoalsLink();else{StopGoalsKeyboard();CloseDrawer();}return true;}
        if(key.Keycode is Key.Pageup or Key.Pagedown)
        {var scroll=_goalsInspectPage>=0?_inspectionScroll:_drawerPages[_goalsPage];scroll.ScrollVertical+=(key.Keycode==Key.Pagedown?1:-1)*(int)(scroll.Size.Y*.75f);return true;}
        string intended=_goalsFocus,version=_goalsVersion;UpdateCampaignUi();if(_goalsPage==0)UpdateServiceCoverage();
        if(version!=GoalsVersion){OpenGoalsKeyboard();return true;}
        var rows=GoalsControls();int index=Array.FindIndex(rows,r=>r.Key==intended);
        if(index<0){FocusGoal(_goalsPage==2?"overview":"back");return true;}
        if(key.Keycode is Key.Tab or Key.Up or Key.Down)
        {int step=key.Keycode==Key.Up || key.Keycode==Key.Tab && key.ShiftPressed?-1:1;FocusGoal(rows[(index+step+rows.Length)%rows.Length].Key);}
        else if(rows[index].Control is OptionButton choice && key.Keycode is Key.Left or Key.Right)
        {choice.Select((choice.Selected+(key.Keycode==Key.Left?-1:1)+choice.ItemCount)%choice.ItemCount);UpdateServiceCoverage();}
        else if(!key.Echo && key.Keycode is Key.Enter or Key.Space && rows[index].Control is Button button)
        {
            if(intended=="back"){BackFromGoalsLink();return true;}
            if(_goalsPage==2)_goalsReturn=intended;
            int origin=_goalsPage;button.EmitSignal(BaseButton.SignalName.Pressed);
            if(!_goalsKeyboard)return true; // Settlement replacement resets the context.
            UpdateCampaignUi();
            if(version!=GoalsVersion){OpenGoalsKeyboard();return true;}
            if(_inspector.Visible && (_selectedSite>=0 || _selectedPerson>=0))
            {_goalsInspectPage=origin;_goalsInspectReturn=intended;_goalsSite=_selectedSite;_goalsPerson=_selectedPerson;_goalsBackList.Hide();_goalsBackInspect.Show();FocusGoal("back");}
            else if(_drawer.Visible && _tabs.CurrentTab==0)
            {_goalsPage=0;_goalsBackList.Show();UpdateServiceCoverage();_goalsFocus="filter";}
            else if(_drawer.Visible && _tabs.CurrentTab==4){StopGoalsKeyboard();OpenEconomyKeyboard();}
            else if(!GoalsKeyboardOpen)StopGoalsKeyboard();else FocusGoal(intended);
        }
        return true;
    }
}

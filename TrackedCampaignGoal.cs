using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class Game
{
    private string? _trackedGoalKey;
    private PanelContainer _trackedGoalPanel=null!;
    private Label _trackedGoalText=null!;
    private Button _trackedGoalOpen=null!,_trackedGoalClose=null!;
    private readonly Dictionary<string,Button> _goalTrackButtons=new();
    private void MakeTrackedGoal()
    {
        _trackedGoalPanel=HudPanel(_hud);_trackedGoalPanel.Position=new(16,92);_trackedGoalPanel.CustomMinimumSize=new(285,0);
        var column=new VBoxContainer();_trackedGoalPanel.AddChild(column);
        _trackedGoalText=Text("",13,true);column.AddChild(_trackedGoalText);
        var row=new HBoxContainer();column.AddChild(row);
        _trackedGoalOpen=Button("Open goal",()=>
        {
            if(!_drawer.Visible || _tabs.CurrentTab!=2)ToggleDrawer(2);
            if(_trackedGoalKey is string key && _goalItems.TryGetValue(key,out var item))
            {item.Help.Show();_drawerPages[2].EnsureControlVisible(item.Root);}
        });_trackedGoalOpen.SizeFlagsHorizontal=Control.SizeFlags.ExpandFill;row.AddChild(_trackedGoalOpen);
        _trackedGoalClose=Button("×",()=>_trackedGoalKey=null,34);_trackedGoalClose.TooltipText="Stop tracking this goal";row.AddChild(_trackedGoalClose);
        _trackedGoalPanel.Hide();
    }
    private void UpdateTrackedGoal()
    {
        var condition=_world.ReadCampaignConditions().FirstOrDefault(c=>c.Key==_trackedGoalKey);
        if(condition==null)_trackedGoalKey=null;
        _trackedGoalPanel.Visible=condition!=null && !_atMainMenu && !_watching && !_drawer.Visible && !_placing;
        if(condition!=null)_trackedGoalText.Text=$"{(condition.Met?"✓":"○")} {condition.Label}\n{condition.Current}/{condition.Required} · {_world.CampaignPhaseTitle}";
        foreach(var entry in _goalTrackButtons)entry.Value.Text=entry.Key==_trackedGoalKey?"Stop tracking":"Track while playing";
    }
}

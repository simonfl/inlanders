using Godot;
using Inlanders.Simulation;
using System.Collections.Generic;
using System.Linq;

public partial class Game
{
    private VBoxContainer _goalDashboard=null!,_goalCards=null!;
    private Label _goalPhase=null!,_goalNext=null!;
    private readonly Dictionary<string,(VBoxContainer Root,Label Count,Button Toggle,Label Help)> _goalItems=new();
    private World? _goalWorld;
    private string _goalKeys="";
    private readonly Dictionary<string,Button> _goalResidentButtons=new();
    private void MakeGoalDashboard(VBoxContainer column)
    {
        _goalDashboard=new();column.AddChild(_goalDashboard);
        _goalPhase=Text("",16,true);_goalDashboard.AddChild(_goalPhase);
        _goalNext=Text("",13,true);_goalDashboard.AddChild(_goalNext);
        _goalCards=new();_goalCards.AddThemeConstantOverride("separation",8);_goalDashboard.AddChild(_goalCards);
        MakeGoalMeals();
    }
    private void UpdateGoalDashboard()
    {
        bool active=(_world.IsRiverCampaign || _world.IsLakeCampaign) && _world.Campaign?.Complete!=true;
        _goalDashboard.Visible=active;_goalArrival.Visible=_objective.Visible=!active;
        if(!active)return;
        var conditions=_world.ReadCampaignConditions();string keys=string.Join(",",conditions.Select(c=>c.Key));
        if(_goalWorld!=_world || _goalKeys!=keys)
        {
            _goalWorld=_world;_goalKeys=keys;
            _goalResidentButtons.Clear();
            _goalPlaces.Clear();_goalPlaceKeys.Clear();
            _goalMealHelp.Hide();
            foreach(var item in _goalItems.Values){_goalCards.RemoveChild(item.Root);item.Root.QueueFree();}_goalItems.Clear();
            foreach(var c in conditions)
            {
                var root=new VBoxContainer();_goalCards.AddChild(root);var row=new HBoxContainer();root.AddChild(row);
                var count=Text("",14,true);count.SizeFlagsHorizontal=Control.SizeFlags.ExpandFill;row.AddChild(count);
                var help=Text(c.Explanation,13,true);help.Hide();
                var toggle=Button("Why?",()=>help.Visible=!help.Visible,48);row.AddChild(toggle);root.AddChild(help);
                _goalItems[c.Key]=(root,count,toggle,help);
                if(c.Key is "housing" or "rest" or "recreation" or "east-recreation")
                {
                    string key=c.Key;var residents=Button("Inspect residents",()=>OpenCampaignResidents(key));root.AddChild(residents);_goalResidentButtons[key]=residents;
                }
            }
        }
        _goalPhase.Text=_world.CampaignPhaseTitle;
        var missing=conditions.FirstOrDefault(c=>!c.Met);
        _goalNext.Text=missing!=null?$"Next: {missing.Label.ToLowerInvariant()} ({missing.Current}/{missing.Required}).":_riverAction.Visible?(_riverAction.Disabled?_riverAction.TooltipText:"Ready for the next phase when you are."):"Keep services running during assessment.";
        foreach(var c in conditions) _goalItems[c.Key].Count.Text=$"{(c.Met?"✓":"○")} {c.Label}\n{c.Current}/{c.Required}";
        foreach(var entry in _goalResidentButtons)entry.Value.Visible=_goalItems[entry.Key].Help.Visible;
        UpdateGoalPlaces();
        UpdateGoalMeals();
    }
}

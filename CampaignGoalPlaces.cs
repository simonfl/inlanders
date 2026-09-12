using Godot;
using Inlanders.Simulation;
using System.Collections.Generic;
using System.Linq;

public partial class Game
{
    private readonly Dictionary<string,VBoxContainer> _goalPlaces=new();
    private readonly Dictionary<string,string> _goalPlaceKeys=new();
    private IEnumerable<Cottage> GoalPlaces(string key) => _world.Cottages.Where(c=>key switch
    {
        "housing" or "rest"=>Buildings.Get(c.Kind).Beds>0,
        "east-beds"=>Buildings.Get(c.Kind).Beds>0 && c.Cell.X>5,
        "east-recreation"=>c.Kind==BuildingKind.Square && c.Cell.X>5,
        "recreation"=>Buildings.Get(c.Kind).RecreationSlots>0,
        "fish"=>c.Kind==BuildingKind.FishingDock,
        "hall" or "hall-visits" or "hall-planks" or "hall-stone"=>c.Kind==BuildingKind.GatheringHall,
        _=>false
    }).OrderBy(c=>c.Id);
    private void PlanGoalBuilding(string key)
    {
        var kind=key switch {"hall" or "hall-visits" or "hall-planks" or "hall-stone"=>BuildingKind.GatheringHall,"fish"=>BuildingKind.FishingDock,"recreation" or "east-recreation"=>BuildingKind.Square,_=>BuildingKind.Cottage};
        CloseDrawer();_followPerson=false;_watchOrbit=false;
        if(key.StartsWith("east-"))
        {
            var land=_world.Map.Land.Where(c=>c.X>5).ToArray();
            if(land.Length>0)_focus=new((float)land.Average(c=>c.X),0,(float)land.Average(c=>c.Z));
            UpdateCamera();
        }
        BeginPlacement(kind);
    }
    private void UpdateGoalPlaces()
    {
        foreach(var entry in _goalItems)
        {
            string key=entry.Key;if(key=="population")continue;
            if(!_goalPlaces.TryGetValue(key,out var panel))
            {
                panel=new VBoxContainer();entry.Value.Root.AddChild(panel);_goalPlaces[key]=panel;
            }
            panel.Visible=entry.Value.Help.Visible;if(!panel.Visible)continue;
            var sites=GoalPlaces(key).ToArray();string signature=string.Join(",",sites.Select(c=>$"{c.Id}:{c.Complete}:{c.DemolitionRequested}"));
            if(_goalPlaceKeys.TryGetValue(key,out var old) && old==signature)continue;
            _goalPlaceKeys[key]=signature;
            foreach(var child in panel.GetChildren()){panel.RemoveChild(child);child.QueueFree();}
            panel.AddChild(Text(sites.Length==0?"No matching places yet.":"RELEVANT PLACES",12,true));
            foreach(var site in sites)
            {
                int id=site.Id;string status=site.DemolitionRequested?" · removing":!site.Complete?" · building":"";
                panel.AddChild(Button($"Show {BuildingName(site.Kind)} {id}{status}",()=>ShowServicePlace(id)));
            }
            panel.AddChild(Button(key.StartsWith("hall")?"Plan a gathering hall":key=="fish"?"Plan a fishing dock":key.Contains("recreation")?"Plan a Square":"Plan a cottage",()=>PlanGoalBuilding(key)));
            if(key=="hall-stone")foreach(var source in _world.ResourceSources().Where(s=>s.Key.Kind==SourceKind.Stone))
            {
                var sourceKey=source.Key;
                panel.AddChild(Button($"Survey {source.Name}",()=>{CloseDrawer();if(!_surveying)ToggleResourceSurvey();SelectResourceSource(sourceKey,true);}));
            }
        }
    }
}

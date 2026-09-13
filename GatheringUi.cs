using Godot;
using Inlanders.Simulation;
using System.Linq;

public partial class Game
{
    private VBoxContainer _gatheringControls=null!;
    private Label _gatheringInfo=null!;
    private Button _gatherHere=null!,_gatherCancel=null!;
    private void MakeGatheringControls()
    {
        _gatheringControls=new();_buildingDetails.AddChild(_gatheringControls);
        _buildingDetails.MoveChild(_gatheringControls,1);
        _gatheringInfo=Text("",14,true);_gatheringControls.AddChild(_gatheringInfo);
        _gatherHere=Button("Share an outdoor meal here",()=>{
            var site=_world.Cottages.FirstOrDefault(c=>c.Id==_selectedSite);
            if(site!=null && _world.BeginGathering(site.Entrance)){SaveWorld();UpdateHud();}
        });_gatheringControls.AddChild(_gatherHere);
        _gatherCancel=Button("Cancel outdoor meal",()=>{_world.CancelGathering();SaveWorld();UpdateHud();});_gatheringControls.AddChild(_gatherCancel);
    }
    private void UpdateGatheringControls()
    {
        var site=_world.Cottages.FirstOrDefault(c=>c.Id==_selectedSite && c.Complete && !c.DemolitionRequested);
        _gatheringControls.Visible=_world.Neighborhood?.Complete==true && site!=null;
        if(!_gatheringControls.Visible)return;
        bool active=_world.Gathering?.Active==true;
        string? problem=active?null:_world.GatheringProblem(site!.Entrance);
        _gatheringInfo.Text=active?(_world.Gathering!.Eating?"Everyone is seated. Sharing the meal together.":
            $"{_world.People.Count(p=>p.Meal?.Gathering==true && p.Task==Work.EatingMeal)}/{_world.Population} seated. People bring their next meal and wait together. Cancel anytime to return carried food."):
            problem??"Gather in the open space outside this building. Everyone brings their next meal, eats together, then returns to village life. No new building is needed.";
        _gatherHere.Visible=!active;_gatherHere.Disabled=problem!=null;
        _gatherCancel.Visible=active;
    }
}

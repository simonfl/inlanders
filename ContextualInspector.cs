using Godot;
using Inlanders.Simulation;
using System.Linq;

public partial class Game
{
    private VBoxContainer _inspectorSecondary=null!;
    private Button _inspectorDetails=null!;
    private Label _inspectorManual=null!,_localFoodSummary=null!;
    private int _contextSite=-1;
    private void MakeContextualInspector()
    {
        _inspectorSecondary=new(){Visible=false};
        var primary=new Node[]{_siteInfo,_productionControls,_workplaceControls,_welcomeControls,_moveButton,_restoreTrialSite,_cancelButton,_priorityButtons[0].GetParent()};
        var secondary=_buildingDetails.GetChildren().Where(n=>!primary.Contains(n)).ToArray();
        _localFoodSummary=Text("",14,true);_buildingDetails.AddChild(_localFoodSummary);
        _buildingDetails.MoveChild(_productionControls,1);_buildingDetails.MoveChild(_localFoodSummary,2);_buildingDetails.MoveChild(_workplaceControls,3);
        _inspectorDetails=Button("Details & policies ▾",()=>{_inspectorSecondary.Visible=!_inspectorSecondary.Visible;_inspectorDetails.Text=_inspectorSecondary.Visible?"Details & policies ▴":"Details & policies ▾";});
        _buildingDetails.AddChild(_inspectorDetails);_buildingDetails.AddChild(_inspectorSecondary);
        _inspectorManual=Text("",13,true);_inspectorSecondary.AddChild(_inspectorManual);
        foreach(var child in secondary)child.Reparent(_inspectorSecondary);
        _productionTargetToggle.Reparent(_inspectorSecondary);_productionTargetControls.Reparent(_inspectorSecondary);
    }
    private void UpdateContextualInspector(Cottage? site)
    {
        if(site==null){_contextSite=-1;return;}
        if(_contextSite!=site.Id){_contextSite=site.Id;_inspectorSecondary.Hide();_inspectorDetails.Text="Details & policies ▾";_inspectionScroll.ScrollVertical=0;}
        _inspectorManual.Text=_siteInfo.Text;
        if(site.WorkPaused && _productionControls.Visible){_productionState.TooltipText=_productionState.Text;_productionState.Text="PAUSED\nCurrent deliveries finish. Resume to start new work.";}
        if(site.Complete)
        {
            string detail=site.Kind switch {
                BuildingKind.Cottage or BuildingKind.Lodge=>$"{_world.People.Count(p=>p.HomeId==site.Id)}/{Buildings.Get(site.Kind).Beds} beds occupied",
                BuildingKind.Square or BuildingKind.SeatingGarden or BuildingKind.GatheringHall=>$"{_world.People.Count(p=>p.LeisureSiteId==site.Id)} visiting · Open for recreation",
                BuildingKind.Bridge=>"Crossing open",
                BuildingKind.Pantry=>"Food collection · No serving staff needed",
                BuildingKind.Stockpile=>$"{_world.MaterialAt(site.Id,site.StorageMaterial)} {site.StorageMaterial.ToString().ToLowerInvariant()} here",
                _=>""};
            _siteInfo.Text=BuildingName(site.Kind).ToUpperInvariant()+"\n"+detail;
            if(site.DemolitionRequested)_siteInfo.Text+="\nDismantling ordered · See Details to cancel";
        }
        bool food=site.Complete && (site.Kind==BuildingKind.Pantry || _world.IsWorkplaceFoodStore(site) || site.Kind==BuildingKind.Square && _world.Neighborhood?.VenueId==site.Id);
        _localFoodSummary.Visible=food;
        if(food)_localFoodSummary.Text="FOOD HERE\n"+string.Join(" · ",World.EdibleKinds.Where(k=>_world.FoodAt(site.Id,k)>0).Select(k=>$"{_world.FoodAt(site.Id,k)} {k.ToString().ToLowerInvariant()}"))+"\n"+$"{World.EdibleKinds.Sum(k=>_world.FoodAvailableAt(site.Id,k))} available · {World.EdibleKinds.Sum(k=>_world.FoodReservedAt(site.Id,k))} claimed";
        if(!_productionControls.Visible){_productionTargetToggle.Hide();_productionTargetControls.Hide();}
        if(_world.SharedWork && _workplaceControls.Visible)
        {
            _workplaceStaff.TooltipText=_workplaceStaff.Text;
            _workplaceStaff.Text=_workplaceStaff.Text.Split('\n')[0]+"\nShared workers fill open jobs automatically.";
        }
    }
}



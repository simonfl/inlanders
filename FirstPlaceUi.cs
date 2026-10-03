using Godot;
using Inlanders.Simulation;
using System.Collections.Generic;
public partial class Game
{
    private PanelContainer _firstPlace=null!;
    private World? _firstPlaceWorld;
    private bool _firstPlaceDismissed;
    private readonly Dictionary<BuildingKind,Button> _firstPlaceChoices=new();
    private Button _firstPlaceBrowse=null!,_firstPlaceLook=null!;
    private Label _firstPlaceGrain=null!;
    private void MakeFirstPlaceUi()
    {
        _firstPlace=HudPanel(_hud);var column=new VBoxContainer();column.AddThemeConstantOverride("separation",6);_firstPlace.AddChild(column);
        var heading=Text("MAKE A FIRST PLACE",16);column.AddChild(heading);
        var intro=Text("Choose your ground. Shared workers build; Space starts daily life. Supplies give you time.",14,true);intro.CustomMinimumSize=new(276,0);column.AddChild(intro);
        foreach(var kind in new[]{BuildingKind.Cottage,BuildingKind.VegetableGarden,BuildingKind.Farm,BuildingKind.FishingDock})
        {
            var choice=kind;var button=Button("",()=>{CloseManagementUi();BeginPlacement(choice);_noticeUntil=0;});
            button.TooltipText=BuildingDescription(kind);column.AddChild(button);_firstPlaceChoices[kind]=button;
        }
        _firstPlaceGrain=Text("",13,true);column.AddChild(_firstPlaceGrain);
        _firstPlaceBrowse=Button("All building choices [B]",()=>{_fullBuild=true;ToggleDrawer(1);SelectBuildSection(0);_buildingFilter.Select(0);UpdateVillageDirectory();});column.AddChild(_firstPlaceBrowse);
        _firstPlaceLook=Button("Look around first [Esc]",()=>_firstPlaceDismissed=true);column.AddChild(_firstPlaceLook);_firstPlace.Hide();
    }
    private void RenderFirstPlaceUi()
    {
        if(_firstPlaceWorld!=_world){_firstPlaceWorld=_world;_firstPlaceDismissed=false;}
        _firstPlace.Visible=_world.PublicPlace?.PlayerFounded==true && !_world.Founding!.Finished && _world.Cottages.Count==0 && !_firstPlaceDismissed && !_atMainMenu && !_watching && !_placing && !_drawer.Visible && !_inspector.Visible && _selectedSite<0 && _selectedPerson<0;
        if(!_firstPlace.Visible)return;
        _firstPlace.Position=new(16,92);_firstPlace.Size=new(300,0);
        foreach(var pair in _firstPlaceChoices)
        {
            string title=pair.Key switch{BuildingKind.Cottage=>"Home for 2",BuildingKind.VegetableGarden=>"Vegetables",BuildingKind.Farm=>"Grain field",_=>"River landing"};
            pair.Value.Text=title+" · "+BuildCost(pair.Key);pair.Value.TooltipText=BuildingDescription(pair.Key);
        }
        _firstPlaceGrain.Text="Grain also needs an oven · "+BuildCost(BuildingKind.Bakery)+". Vegetables and fish are eaten directly.";
    }
}

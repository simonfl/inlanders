using Godot;
using Inlanders.Simulation;
using System.Collections.Generic;
using System.Linq;
public partial class Game
{
    private PanelContainer _firstPlace=null!;
    private World? _firstPlaceWorld;
    private bool _firstPlaceDismissed;
    private readonly Dictionary<BuildingKind,Button> _firstPlaceChoices=new();
    private Button _firstPlaceBrowse=null!,_firstPlaceLook=null!;
    private Button _firstPlaceTimber=null!;
    private Label _firstPlaceHeading=null!,_firstPlaceIntro=null!,_firstPlaceDetail=null!;
    private bool FirstLivelihoodArrival=>_world.PublicPlace is {RiverLandscape:true,PlayerFounded:true} && !_world.Cottages.Any(c=>World.ProductionOutput(c.Kind)!=null);
    private void MakeFirstPlaceUi()
    {
        _firstPlace=HudPanel(_hud);var column=new VBoxContainer();column.AddThemeConstantOverride("separation",6);_firstPlace.AddChild(column);
        _firstPlaceHeading=Text("MAKE A FIRST PLACE",16);column.AddChild(_firstPlaceHeading);
        _firstPlaceIntro=Text("Choose your ground. Shared workers build; Space starts daily life. Supplies give you time.",14,true);_firstPlaceIntro.CustomMinimumSize=new(276,0);column.AddChild(_firstPlaceIntro);
        foreach(var kind in EverydayBuildings.Append(BuildingKind.ForagerHut))
        {
            var choice=kind;var button=Button("",()=>{CloseManagementUi();BeginPlacement(choice);_noticeUntil=0;});
            button.TooltipText=BuildingDescription(kind);column.AddChild(button);_firstPlaceChoices[kind]=button;
        }
        _firstPlaceTimber=Button("Gather timber · choose trees",BeginEverydayTimber);column.AddChild(_firstPlaceTimber);
        _firstPlaceDetail=Text("",13,true);column.AddChild(_firstPlaceDetail);
        _firstPlaceBrowse=Button("All building choices [B]",()=>{_fullBuild=true;ToggleDrawer(1);SelectBuildSection(0);_buildingFilter.Select(0);UpdateVillageDirectory();});column.AddChild(_firstPlaceBrowse);
        _firstPlaceLook=Button("Look around first [Esc]",()=>_firstPlaceDismissed=true);column.AddChild(_firstPlaceLook);_firstPlace.Hide();
    }
    private void RenderFirstPlaceUi()
    {
        if(_firstPlaceWorld!=_world){_firstPlaceWorld=_world;_firstPlaceDismissed=false;}
        _firstPlace.Visible=!_showFoodMap && _world.PublicPlace?.PlayerFounded==true && !_world.Founding!.Finished && (_world.Cottages.Count==0 || FirstLivelihoodArrival) && !_firstPlaceDismissed && !_atMainMenu && !_watching && !_placing && !_drawer.Visible && !_inspector.Visible && _selectedSite<0 && _selectedPerson<0;
        if(!_firstPlace.Visible)return;
        if(!_placing && _uiTime<_noticeUntil)_hintPanel.Hide();
        _firstPlace.Position=new(16,92);_firstPlace.Size=new(300,0);
        bool livelihood=FirstLivelihoodArrival;
        _firstPlaceHeading.Text=livelihood?"MAKE A LIVING HERE":"MAKE A FIRST PLACE";
        _firstPlaceIntro.Text=livelihood?"Homes are ready. Choose a food source and its ground. Shared workers build and collect; Space starts life.":"Choose your ground. Shared workers build; Space starts daily life. Supplies give you time.";
        _firstPlaceDetail.Text=livelihood?"Crops need time; fishing needs a shore. Berries supplement other food. Provisions cover your first steps; no required recipe.":"Draw a cultivated strip to choose its size. Kitchen gardens and grain are also in All building choices.";
        foreach(var pair in _firstPlaceChoices)
        {
            pair.Value.Visible=pair.Key==BuildingKind.Cottage?!livelihood:pair.Key!=BuildingKind.ForagerHut || livelihood;
            string title=pair.Key switch{BuildingKind.Cottage=>"Home for 2",BuildingKind.VegetableField=>"Cultivated strip",BuildingKind.ForagerHut=>"Gather berries",_=>"River landing"};
            pair.Value.Text=title+" · "+BuildCost(pair.Key);pair.Value.TooltipText=BuildingDescription(pair.Key);
        }
    }
}

using Godot;
using Inlanders.Simulation;
using System.Collections.Generic;
public partial class Game
{
    private static readonly BuildingKind[] EverydayBuildings={BuildingKind.Cottage,BuildingKind.VegetableField,BuildingKind.FishingDock};
    private void BeginEverydayTimber(){BeginWoodlandTool(2);CloseDrawer();}
    private bool _fullBuild;
    private VBoxContainer _essentials=null!;
    private Button _buildBreadth=null!,_gatherTimber=null!;
    private readonly Dictionary<BuildingKind,Button> _essentialChoices=new();
    private bool SmallBuild=>_world.PublicPlace!=null && !_fullBuild && !_catalogKeyboard;
    private void MakeBuildEssentials(VBoxContainer parent)
    {
        _buildBreadth=Button("Browse all buildings",()=>{_fullBuild=!_fullBuild;UpdateVillageDirectory();_drawerPages[1].ScrollVertical=0;});parent.AddChild(_buildBreadth);
        _essentials=new();parent.AddChild(_essentials);
        _essentials.AddChild(Text("All buildings remain available.",13,true));
        foreach(var kind in EverydayBuildings)
        {
            var choice=kind;var b=Button("",()=>{BeginPlacement(choice);CloseDrawer();});b.CustomMinimumSize=new(0,50);b.AutowrapMode=TextServer.AutowrapMode.WordSmart;_essentials.AddChild(b);_essentialChoices[kind]=b;
        }
        _gatherTimber=Button("Harvest timber · choose trees",BeginEverydayTimber);_gatherTimber.CustomMinimumSize=new(0,50);_gatherTimber.TooltipText="Allow selected trees to supply shared timber work when needed. Stumps remain for replanting; use Clear trees & stumps [C] when you need building ground. Preserve a tree to stop a cut before it falls.";_essentials.AddChild(_gatherTimber);
    }
    private void RenderBuildEssentials()
    {
        bool publicPlace=_world.PublicPlace!=null && _buildSection==0;
        _buildBreadth.Visible=publicPlace;_buildBreadth.Text=SmallBuild?"Browse all buildings":"Everyday choices";
        _essentials.Visible=publicPlace && SmallBuild;_gatherTimber.Disabled=_world.Food.Celebrating;
        foreach(var (kind,b) in _essentialChoices){b.Text=BuildingName(kind)+" · "+BuildCost(kind)+"\n"+(kind==BuildingKind.VegetableField?"Draw the growing ground":BuildingPurpose(kind));b.TooltipText=BuildingDescription(kind);b.Disabled=_world.Food.Celebrating;}
    }
}

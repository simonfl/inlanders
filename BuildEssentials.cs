using Godot;
using Inlanders.Simulation;
using System.Collections.Generic;
public partial class Game
{
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
        foreach(var kind in new[]{BuildingKind.Cottage,BuildingKind.VegetableField,BuildingKind.FishingDock})
        {
            var choice=kind;var b=Button("",()=>{BeginPlacement(choice);CloseDrawer();});b.CustomMinimumSize=new(0,50);b.AutowrapMode=TextServer.AutowrapMode.WordSmart;_essentials.AddChild(b);_essentialChoices[kind]=b;
        }
        _gatherTimber=Button("Gather timber · choose trees",()=>{ToggleClearing();CloseDrawer();});_gatherTimber.CustomMinimumSize=new(0,50);_gatherTimber.TooltipText="Shared workers collect the marked trees' timber and clear roots. Mark again to cancel before work; cutting cannot be undone.";_essentials.AddChild(_gatherTimber);
    }
    private void RenderBuildEssentials()
    {
        bool publicPlace=_world.PublicPlace!=null && _buildSection==0;
        _buildBreadth.Visible=publicPlace;_buildBreadth.Text=SmallBuild?"Browse all buildings":"Everyday choices";
        _essentials.Visible=publicPlace && SmallBuild;_gatherTimber.Disabled=_world.Food.Celebrating;
        foreach(var (kind,b) in _essentialChoices){b.Text=BuildingName(kind)+" · "+BuildCost(kind)+"\n"+(kind==BuildingKind.VegetableField?"Draw the growing ground":BuildingPurpose(kind));b.TooltipText=BuildingDescription(kind);b.Disabled=_world.Food.Celebrating;}
    }
}

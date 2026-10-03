using Godot;
using Inlanders.Simulation;
using System;
public partial class Game
{
    private OptionButton _homeAppearance=null!;
    private void MakeHomeAppearance(VBoxContainer column)
    {
        _homeAppearance=new(){CustomMinimumSize=new(0,38),SizeFlagsHorizontal=Control.SizeFlags.ExpandFill};
        foreach(var finish in Enum.GetValues<CottageFinish>())_homeAppearance.AddItem("Appearance · "+(finish==CottageFinish.Automatic?"Weathered wood":finish.ToString()),(int)finish);
        _homeAppearance.TooltipText="Choose this home's roof and plaster colors. Free and reversible; no effect on work, meals or comfort.";
        _homeAppearance.ItemSelected+=index=>{if(_world.SetCottageFinish(_workCardSite,(CottageFinish)_homeAppearance.GetItemId((int)index))){CreateActors();RenderActors(0);_nextWorkCard=0;}};
        column.AddChild(_homeAppearance);
    }
    private void RenderHomeAppearance(Cottage site)
    {
        _homeAppearance.Visible=_homeOptions && _yardPreviewSide<0 && site.Complete && site.Kind==BuildingKind.Cottage && _world.PublicPlace!=null;
        _homeAppearance.Select((int)site.Finish);_homeAppearance.Disabled=site.DemolitionRequested;
    }
}

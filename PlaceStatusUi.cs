using Godot;
using Inlanders.Simulation;
using System.Linq;
using Resource=Inlanders.Simulation.Resource;
public partial class Game
{
    private VBoxContainer _placeFood=null!;
    private Label _placeFoodCount=null!;
    private void MakePlaceStatus(HBoxContainer top)
    {
        _placeFood=new(){CustomMinimumSize=new(72,0),SizeFlagsHorizontal=Control.SizeFlags.ExpandFill,MouseFilter=Control.MouseFilterEnum.Stop,MouseDefaultCursorShape=Control.CursorShape.PointingHand};
        _placeFood.AddThemeConstantOverride("separation",0);top.AddChild(_placeFood);
        var title=Text("FOOD",11);title.Modulate=new("a8bcb0");_placeFood.AddChild(title);_placeFoodCount=Text("",20);_placeFood.AddChild(_placeFoodCount);
        _placeFood.GuiInput+=input=>{if(input is InputEventMouseButton{ButtonIndex:MouseButton.Left,Pressed:true}){OpenEconomy();_placeFood.AcceptEvent();}};
    }
    private void UpdatePlaceStatus()
    {
        bool place=_world.PublicPlace!=null;_placeFood.Visible=place;_brand.Visible=place || _hud.Size.X>=1200;
        // Reset every base column too, so leaving public play restores archived controls.
        foreach(var resource in new[]{Resource.Logs,Resource.Planks,Resource.Berries,Resource.Grain,Resource.Bread,Resource.Vegetables})_resourceValues[resource].GetParent<Control>().Visible=true;
        if(!place)return;
        int food=World.EdibleKinds.Sum(_world.StoredFood);_placeFoodCount.Text=food.ToString();
        _placeFood.TooltipText=$"{food} stored meal portions across the village, including reserved portions. Grain needs baking. Click for individual foods, carrying and shortages in Economy [I].";
        var site=_world.Cottages.FirstOrDefault(c=>c.Id==_selectedSite);
        BuildingKind? kind=_placing && !_plantingTrees && !_clearingTrees && _pathTool==0 && !_decorating && _woodlandTool==0?_buildKind:site?.Kind;
        var output=kind is {} k?World.ProductionOutput(k):null;
        foreach(var item in _resourceValues)
        {
            bool relevant=item.Key is Resource.Logs or Resource.Planks || item.Key==output ||
                kind==BuildingKind.Bakery && item.Key==Resource.Grain ||
                kind is {} building && Buildings.Get(building).StoneCost>0 && item.Key==Resource.Stone;
            item.Value.GetParent<Control>().Visible=relevant;
        }
    }
}


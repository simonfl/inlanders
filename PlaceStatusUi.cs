using Godot;
using Inlanders.Simulation;
using System.Linq;
using Resource=Inlanders.Simulation.Resource;
public partial class Game
{
    private VBoxContainer _placeFood=null!;
    private Label _placeFoodCount=null!,_placeFoodTitle=null!;
    private void MakePlaceStatus(HBoxContainer top)
    {
        _placeFood=new(){CustomMinimumSize=new(72,0),SizeFlagsHorizontal=Control.SizeFlags.ExpandFill,MouseFilter=Control.MouseFilterEnum.Stop,MouseDefaultCursorShape=Control.CursorShape.PointingHand};
        _placeFood.AddThemeConstantOverride("separation",0);top.AddChild(_placeFood);
        _placeFoodTitle=Text("FOOD",11);_placeFoodTitle.Modulate=new("a8bcb0");_placeFood.AddChild(_placeFoodTitle);_placeFoodCount=Text("",20);_placeFood.AddChild(_placeFoodCount);
        _placeFood.GuiInput+=input=>{if(input is InputEventMouseButton{ButtonIndex:MouseButton.Left,Pressed:true}){OpenEconomy();_placeFood.AcceptEvent();}};
    }
    private void UpdatePlaceStatus()
    {
        RenderPublicNavigation();
        bool place=_world.PublicPlace!=null;_placeFood.Visible=place;_brand.Visible=place || _hud.Size.X>=1200;
        // Commit each column's final visibility once. Public play previously showed
        // legacy columns here and immediately hid them again, invalidating layout.
        if(!place)
        {
            foreach(var item in _resourceValues)
            {
                bool visible=item.Key switch {
                    Resource.Game=>_world.Map.Wildlife.Count>0 || _world.Food.HuntedGame>0 || _world.CreativeAdded(Resource.Game)>0,
                    Resource.Fruit=>_world.Cottages.Any(c=>c.Kind==BuildingKind.Orchard) || _world.Food.GrownFruit>0 || _world.CreativeAdded(Resource.Fruit)>0,
                    Resource.Fish=>_world.Map.FishingGrounds.Count>0 || _world.Food.CaughtFish>0 || _world.CreativeAdded(Resource.Fish)>0,
                    Resource.Stone=>_world.Map.StoneDeposits.Count>0 || _world.Stone>0,
                    _=>true};
                var column=item.Value.GetParent<Control>();if(column.Visible!=visible)column.Visible=visible;
            }
            return;
        }
        int food=World.EdibleKinds.Sum(_world.StoredFood);_placeFoodCount.Text=food.ToString();
        bool low=_world.SimulatesMeals && food<_world.Population*2;
        _placeFoodTitle.Text=low?"FOOD · LOW":"FOOD";
        _placeFoodCount.Modulate=low?new("ffd39b"):Colors.White;
        _placeFood.TooltipText=$"{food} stored meal portions across the village, including reserved portions. Grain needs baking. Click for individual foods, carrying and shortages in Economy [I].";
        if(low)_placeFood.TooltipText="Less than two stored portions per resident. Click to inspect existing production, meal access or compare another food source. This is a reserve warning, not a forecast.\n"+_placeFood.TooltipText;
        var site=_world.Cottages.FirstOrDefault(c=>c.Id==_selectedSite);
        BuildingKind? kind=_placing && !_plantingTrees && !_clearingTrees && _pathTool==0 && !_decorating && _woodlandTool==0?_buildKind:site?.Kind;
        var output=kind is {} k?World.ProductionOutput(k):null;
        foreach(var item in _resourceValues)
        {
            bool relevant=item.Key is Resource.Logs or Resource.Planks || item.Key==output ||
                kind==BuildingKind.Bakery && item.Key==Resource.Grain ||
                kind is {} building && Buildings.Get(building).StoneCost>0 && item.Key==Resource.Stone;
            var column=item.Value.GetParent<Control>();if(column.Visible!=relevant)column.Visible=relevant;
        }
    }
}


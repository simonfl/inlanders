using Godot;
using Inlanders.Simulation;
using System.Collections.Generic;
using System.Linq;
using Resource=Inlanders.Simulation.Resource;
public partial class Game
{
    private VBoxContainer _reserves=null!;
    private Label _reserveFood=null!;
    private readonly Dictionary<Resource,Button> _reserveButtons=new();
    private void MakeReserveUi(VBoxContainer column)
    {
        _reserves=new();column.AddChild(_reserves);
        _reserves.AddChild(Text("AUTOMATIC RESERVES",12));
        _reserveFood=Text("",14,true);_reserves.AddChild(_reserveFood);
        foreach(var material in new[]{Resource.Logs,Resource.Planks,Resource.Stone,Resource.Grain})
        {
            var item=material;
            var button=Button("",()=>OpenReserveSupply(item));
            button.AutowrapMode=TextServer.AutowrapMode.WordSmart;
            _reserveButtons[item]=button;_reserves.AddChild(button);
        }
        _reserves.AddChild(Text("Material targets include queued construction. Stock is not necessarily unreserved or nearby. Select a material to inspect its supply; current jobs finish even when the target is reached.",13,true));
    }
    private void OpenReserveSupply(Resource material)
    {
        var kind=World.SupplyProducerKind(material);
        var existing=_world.Cottages.Where(c=>c.Kind==kind && !c.DemolitionRequested).OrderByDescending(c=>c.Complete).ThenBy(c=>c.WorkPaused).ThenBy(c=>c.Id).FirstOrDefault();
        if(existing!=null){_focus=BuildingPosition(existing);UpdateCamera();ShowWorkplaceCard(existing.Id);return;}
        ClearSelection();OpenMaterialSupply(material);
    }
    private void UpdateReserveUi()
    {
        _reserves.Visible=_world.ProvisionedLife;if(!_reserves.Visible)return;
        _reserveFood.Text=$"Food: {_world.EdibleStored}/{_world.Population*4} portions stored · four per resident. Priority below {_world.Population*3}. Grain is an oven ingredient, not a meal.";
        foreach(var (material,button) in _reserveButtons)
        {
            int stored=material==Resource.Logs?_world.Stored:material==Resource.Planks?_world.Planks:material==Resource.Grain?_world.StoredGrain:_world.Stone;
            button.Text=$"{material}: {stored}/{(material==Resource.Grain?_world.GrainReserveTarget:_world.MaterialStockTarget(material))} stored · inspect supply";
            button.TooltipText=material==Resource.Grain?$"Ingredient buffer: one crop minimum, or two bread portions per resident. Each grain makes {_world.BreadPerGrain} bread. A bakery must process it; grain cannot feed residents directly.":$"{_world.ProductionCommitted(material)} stored or committed. "+(material==Resource.Logs?"32 logs in reserve plus construction. Harvest & renew permits selected trees.":"24 in reserve plus construction and applicable improvements. Manual workplace caps can stop production earlier.");
        }
    }
}

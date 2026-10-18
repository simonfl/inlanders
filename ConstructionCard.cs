using Godot;
using Inlanders.Simulation;
using System.Linq;
public partial class Game
{
    private Button _constructionSupply=null!,_constructionPriority=null!;
    private void MakeConstructionCard(VBoxContainer column)
    {
        _constructionSupply=Button("",()=>
        {
            var site=_world.Cottages.FirstOrDefault(c=>c.Id==_workCardSite);
            if(site==null || _world.ReadConstruction(site).Missing is not {} material)return;
            OpenMaterialSupply(material);
        });column.AddChild(_constructionSupply);
        _constructionPriority=Button("",()=>{var site=_world.Cottages.FirstOrDefault(c=>c.Id==_workCardSite);if(site!=null)_world.SetPriority(site.Id,site.Priority==2?1:2);_nextWorkCard=0;});column.AddChild(_constructionPriority);
        _constructionPriority.TooltipText="Prefer this construction when workers choose their next job. Existing deliveries and urgent meals continue. Click again for ordinary priority.";
    }
    private void OpenMaterialSupply(Inlanders.Simulation.Resource material)
    {
        RememberSupplyOrigin();
        if(material==Inlanders.Simulation.Resource.Logs){BeginEverydayTimber();return;}
        var kind=World.SupplyProducerKind(material);if(kind==null)return;
        var producer=_world.ReadSupplyProducer(_workCardSite,material);
        if(producer!=null){_focus=BuildingPosition(producer);UpdateCamera();ShowWorkplaceCard(producer.Id);}
        else {BeginPlacement(kind.Value);CloseDrawer();}
    }
    private string SupplyAction(Cottage site,Inlanders.Simulation.Resource material)
    {
        if(material==Inlanders.Simulation.Resource.Logs)return "Choose timber to harvest";
        string name=material==Inlanders.Simulation.Resource.Grain?"grain field":material==Inlanders.Simulation.Resource.Planks?"sawmill":"quarry";
        return (_world.ReadSupplyProducer(site.Id,material)!=null?"Inspect ":"Plan a ")+name;
    }
    private void RenderConstructionCard(Cottage site)
    {
        bool show=!site.Complete && !site.DemolitionRequested;
        _constructionPriority.Visible=show;_constructionPriority.Text=site.Priority==2?"Build first · on":"Build first";
        _constructionPriority.Disabled=_world.Food.Celebrating;
        var missing=show?_world.ReadConstruction(site).Missing:null;_constructionSupply.Visible=missing!=null;
        _constructionSupply.Text=missing is {} material?SupplyAction(site,material):"";
    }
}

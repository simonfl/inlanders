using Godot;
using Inlanders.Simulation;
public partial class Game
{
    private Button _cropOrder=null!;
    private void MakeCropOrder(VBoxContainer column)
    {
        _cropOrder=Button("Harvest, then pause",()=>{var site=_world.Cottages.Find(c=>c.Id==_workCardSite);if(site!=null)_world.SetHarvestThenPause(site.Id,!site.HarvestThenPause);_nextWorkCard=0;});column.AddChild(_cropOrder);
        _cropOrder.TooltipText="Finish the planted crop and its deliveries, then stop before sowing again. Useful before moving or changing the field. No worker assignment needed.";
    }
    private void RenderCropOrder(Cottage site)
    {
        _cropOrder.Visible=_world.PublicPlace!=null && site.Complete && !site.DemolitionRequested && (site.Kind==BuildingKind.Farm || World.IsVegetablePlot(site.Kind));
        _cropOrder.Text=site.HarvestThenPause?(site.WorkPaused?"Resume cropping":"Harvest, then pause · on"):"Harvest, then pause";
        _cropOrder.Disabled=_world.Food.Celebrating || !site.HarvestThenPause && !site.Planted && site.Harvest==0;
    }
}

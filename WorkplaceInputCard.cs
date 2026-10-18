using Godot;
using Inlanders.Simulation;
using System.Linq;
public partial class Game
{
    private Button _workCardInput=null!;
    private void MakeWorkplaceInputCard(VBoxContainer column)
    {
        _workCardInput=Button("",()=>
        {
            var site=_world.Cottages.FirstOrDefault(c=>c.Id==_workCardSite);
            if(site!=null && _world.ReadWorkplace(site).MissingInput is {} input)OpenMaterialSupply(input);
        });column.AddChild(_workCardInput);
    }
    private void RenderWorkplaceInputCard(Cottage site)
    {
        var input=site.Complete && _placeJourneySite!=site.Id?_world.ReadWorkplace(site).MissingInput:null;
        _workCardInput.Visible=input!=null;
        _workCardInput.Text=input is {} material?SupplyAction(site,material):"";
        _workCardInput.TooltipText=input==Inlanders.Simulation.Resource.Grain?"Grain fields supply the oven. Inspect an existing field, including a paused or unfinished one, or choose where to build the first. Vegetables do not supply grain.":"Select real timber to harvest; preserved trees remain until you choose them.";
    }
}

using Godot;
using Inlanders.Simulation;
using System.Linq;

public partial class Game
{
    private VBoxContainer _comfortControls=null!;
    private Label _comfortInfo=null!;
    private Button _comfortOrder=null!, _comfortCancel=null!, _comfortWorker=null!;
    private void MakeComfortControls()
    {
        _comfortControls=new();_buildingDetails.AddChild(_comfortControls);
        _comfortInfo=Text("",14,true);_comfortControls.AddChild(_comfortInfo);
        _comfortOrder=Button("Improve home",()=>_world.RequestImprovement(_selectedSite));_comfortControls.AddChild(_comfortOrder);
        _comfortCancel=Button("Cancel improvement",()=>_world.CancelImprovement(_selectedSite));_comfortControls.AddChild(_comfortCancel);
        _comfortCancel.TooltipText="Uncollected claims release; carried planks return physically and builders recover delivered planks. The home stays occupied.";
        _comfortWorker=Button("Inspect carpenter",()=> {var p=_world.People.FirstOrDefault(p=>p.ComfortHomeId==_selectedSite);if(p!=null) SelectPerson(p.Id);});_comfortControls.AddChild(_comfortWorker);
    }
    private void UpdateComfortControls()
    {
        var home=_world.Cottages.FirstOrDefault(c=>c.Id==_selectedSite && Buildings.Get(c.Kind).Beds>0 && c.Complete);
        _comfortControls.Visible=home!=null;if(home==null)return;
        _comfortInfo.Text=$"HOME COMFORT · {_world.People.Count(p=>p.HomeId==home.Id)}/{Buildings.Get(home.Kind).Beds} residents\n{_world.ComfortSummary(home)}";
        if(!home.Improved && !home.ImprovementRequested && !_world.Creative) _comfortInfo.Text+="\nNeeds an open carpenter workshop, an assigned carpenter and available planks. Orders can wait for supply.";
        _comfortOrder.Visible=!home.Improved && !home.ImprovementRequested && !home.DemolitionRequested;
        _comfortOrder.Text=_world.Creative?"Improve home · free":$"Improve home · {World.ComfortCost(home)} planks";
        _comfortOrder.Disabled=_world.ImprovementProblem(home.Id)!=null;_comfortOrder.TooltipText=_world.ImprovementProblem(home.Id)??"Install furnishings while residents continue using their home.";
        _comfortCancel.Visible=home.ImprovementRequested;_comfortCancel.Disabled=_world.Food.Celebrating || home.DemolitionRequested;
        var worker=_world.People.FirstOrDefault(p=>p.ComfortHomeId==home.Id);_comfortWorker.Visible=worker!=null;
        if(worker!=null) _comfortWorker.Text=$"Inspect {worker.Name} · {worker.Role}";
    }
}

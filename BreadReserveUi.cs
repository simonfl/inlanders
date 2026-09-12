using Godot;
using Inlanders.Simulation;
using System.Collections.Generic;
using System.Linq;
using Resource = Inlanders.Simulation.Resource;

public partial class Game
{
    private Button _breadToggle=null!, _supperBreadLink=null!;
    private VBoxContainer _breadDetails=null!, _breadPlaces=null!;
    private Label _breadSummary=null!;
    private readonly Dictionary<int,Button> _breadPlaceLinks=new();

    private void MakeBreadReserveUi(VBoxContainer column)
    {
        _breadToggle=Button("Inspect bread supply",()=>_breadDetails.Visible=!_breadDetails.Visible);
        column.AddChild(_breadToggle);
        _breadDetails=new(){Visible=false};column.AddChild(_breadDetails);
        _breadSummary=Text("",14,true);_breadDetails.AddChild(_breadSummary);
        _breadPlaces=new();_breadDetails.AddChild(_breadPlaces);
    }
    private async void OpenBreadReserve()
    {
        OpenEconomy();_breadDetails.Show();
        await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
        await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
        _drawerPages[4].ScrollVertical+=(int)(_breadToggle.GetGlobalRect().Position.Y-_drawerPages[4].GetGlobalRect().Position.Y);
    }
    private void UpdateBreadReserveUi()
    {
        _breadSummary.Text=_world.BreadSupplySummary();
        _breadToggle.Text=$"Bread supply · {_world.CentralFoodAvailable(Resource.Bread)} available centrally";
        var sites=_world.Cottages.Where(c=>c.Kind is BuildingKind.Bakery or BuildingKind.Pantry).ToArray();
        foreach(int id in _breadPlaceLinks.Keys.Where(id=>!sites.Any(c=>c.Id==id)).ToArray())
        {
            var old=_breadPlaceLinks[id];_breadPlaces.RemoveChild(old);old.QueueFree();_breadPlaceLinks.Remove(id);
        }
        foreach(var site in sites)
        {
            if(!_breadPlaceLinks.TryGetValue(site.Id,out var link))
            {
                int id=site.Id;link=Button("",()=> {if(_world.Cottages.Any(c=>c.Id==id))SelectBuilding(id);});
                link.AutowrapMode=TextServer.AutowrapMode.WordSmart;
                _breadPlaces.AddChild(link);_breadPlaceLinks.Add(id,link);
            }
            link.Text=$"Inspect {(site.Kind==BuildingKind.Bakery?"bakery":"pantry")} {site.Id} · "+
                (site.DemolitionRequested?"being removed":!site.Complete?"under construction":
                site.Kind==BuildingKind.Bakery?_world.ReadWorkplace(site).State:
                $"{_world.FoodAt(site.Id,Resource.Bread)} bread stored");
        }
    }
}

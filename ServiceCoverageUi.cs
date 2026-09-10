using Godot;
using Inlanders.Simulation;
using System.Collections.Generic;
using System.Linq;

public partial class Game
{
    private Button _serviceToggle=null!;
    private VBoxContainer _servicePanel=null!, _serviceRows=null!;
    private OptionButton _serviceFilter=null!;
    private Label _serviceCount=null!;
    private World? _serviceWorld;
    private readonly Dictionary<int,(VBoxContainer Row,Button Person,Label Reason,Button Home,Button Venue)> _serviceResidents=new();
    private bool HasRecreation(Villager p) => p.LastLeisureTime is float t && _world.Food.Time-t<p.LastLeisureWindow;
    private int? ServiceVenue(Villager p) => p.LeisureSiteId ?? (HasRecreation(p)?p.LastLeisureSiteId:null);
    private void ShowServicePlace(int? id)
    {
        var site=_world.Cottages.FirstOrDefault(c=>c.Id==id);
        if(site==null) return;
        _followPerson=false; SelectBuilding(site.Id); _focus=OnGround(site.Cell.X,site.Cell.Z); UpdateCamera();
    }
    private void MakeServiceCoverage(VBoxContainer column)
    {
        _serviceToggle=Button("Rest & recreation",()=>_servicePanel.Visible=!_servicePanel.Visible);
        column.AddChild(_serviceToggle);
        _servicePanel=new() { Visible=false }; column.AddChild(_servicePanel);
        var guide=Text("Completed visits · rest lasts 4m; recreation lasts 2m from a square or 4m from a hall.",13,true);
        guide.TooltipText="Square: 6-second visit, at least 1 minute between outings. Hall: 12-second visit, at least 2 minutes between outings. Residents go between jobs. Travel or attendance alone does not count as a completed visit.";
        _servicePanel.AddChild(guide);
        _serviceFilter=DirectoryFilter(_servicePanel,"Find residents without recent completed visits. New arrivals may still be waiting for their first outing.");
        foreach(string name in new[]{"Missing either visit","Missing recent rest","Missing recreation","All residents"}) _serviceFilter.AddItem(name);
        _serviceCount=Text("",13,true); _servicePanel.AddChild(_serviceCount);
        _serviceRows=new(); _serviceRows.AddThemeConstantOverride("separation",12); _servicePanel.AddChild(_serviceRows);
    }
    private void UpdateServiceCoverage()
    {
        if(_serviceWorld!=_world)
        {
            _serviceWorld=_world; _serviceFilter.Select(0); _servicePanel.Hide();
            foreach(var row in _serviceResidents.Values) { _serviceRows.RemoveChild(row.Row); row.Row.QueueFree(); }
            _serviceResidents.Clear();
        }
        _serviceToggle.Text=$"{(_servicePanel.Visible?"▾":"▸")} Rest {_world.ResidentsRested}/{_world.Population} · recreation {_world.People.Count(HasRecreation)}/{_world.Population}";
        if(!_servicePanel.Visible) return;
        int shown=0;
        foreach(var p in _world.People)
        {
            if(!_serviceResidents.TryGetValue(p.Id,out var item))
            {
                int id=p.Id;
                var row=new VBoxContainer(); _serviceRows.AddChild(row);
                var person=Button(p.Name,()=>SelectPerson(id)); row.AddChild(person);
                var reason=Text("",13,true); row.AddChild(reason);
                var home=Button("Show home",()=>ShowServicePlace(_world.People[id].HomeId)); row.AddChild(home);
                var venue=Button("Show venue",()=>ShowServicePlace(ServiceVenue(_world.People[id]))); row.AddChild(venue);
                item=(row,person,reason,home,venue); _serviceResidents[id]=item;
            }
            bool rest=_world.RecentlyRested(p), recreation=HasRecreation(p);
            item.Row.Visible=_serviceFilter.Selected switch { 0=>!rest || !recreation, 1=>!rest, 2=>!recreation, _=>true };
            if(!item.Row.Visible) continue;
            shown++;
            item.Person.Text=$"{p.Name} · inspect";
            item.Reason.Text=$"REST · {_world.RestSummary(p)}\nRECREATION · {_world.RecreationSummary(p)}";
            var homeSite=_world.Cottages.FirstOrDefault(c=>c.Id==p.HomeId);
            item.Home.Visible=homeSite!=null;
            item.Home.Text=homeSite==null?"Show home":$"Home · {BuildingName(homeSite.Kind)} {homeSite.Id}";
            var venueSite=_world.Cottages.FirstOrDefault(c=>c.Id==ServiceVenue(p));
            item.Venue.Visible=venueSite!=null;
            item.Venue.Text=venueSite==null?"Show venue":$"{(p.LeisureSiteId!=null?"Current outing":"Last visit")} · {BuildingName(venueSite.Kind)} {venueSite.Id}";
        }
        _serviceCount.Text=shown==0?"No residents match. Choose All residents to see their routines.":$"{shown} of {_world.Population} residents";
    }
}

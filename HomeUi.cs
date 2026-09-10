using Godot;
using Inlanders.Simulation;
using System.Linq;

public partial class Game
{
    private Label _homeNeeds=null!;
    private Button _homeLink=null!, _moveHome=null!, _previewHome=null!;
    private OptionButton _homeChoice=null!;
    private string _homeChoiceKey="";
    private World? _homeUiWorld;
    private void MakeHomeUi()
    {
        _homeNeeds=Text("",14,true); _personDetails.AddChild(_homeNeeds);
        _homeLink=Button("Show home",()=>
        {
            if(_selectedPerson<0 || _world.People[_selectedPerson].HomeId is not int id) return;
            var home=_world.Cottages.FirstOrDefault(c=>c.Id==id); if(home==null) return;
            SelectBuilding(id); _focus=OnGround(home.Cell.X,home.Cell.Z); UpdateCamera();
        });
        _personDetails.AddChild(_homeLink);
        _homeChoice=DirectoryFilter(_personDetails,"Choose a completed home with a spare bed.");
        _previewHome=Button("Preview selected home",()=>
        {
            var home=_world.Cottages.FirstOrDefault(c=>c.Id==_homeChoice.GetSelectedId()); if(home==null) return;
            _followPerson=false; _focus=OnGround(home.Cell.X,home.Cell.Z); UpdateCamera();
        });
        _personDetails.AddChild(_previewHome);
        _moveHome=Button("Move to selected home",()=>
        {
            if(_selectedPerson<0 || _homeChoice.Selected<0) return;
            int id=_homeChoice.GetSelectedId();
            if(!_world.AssignHome(_selectedPerson,id)) Notice(_world.HomeAssignmentProblem(_selectedPerson,id) ?? "Home unchanged.");
            else Notice("Home assigned. The next rest visit uses this home.");
        });
        _personDetails.AddChild(_moveHome);
    }
    private void UpdateHomeUi(Villager person)
    {
        if(_homeUiWorld!=_world) { _homeUiWorld=_world; _homeChoiceKey=""; }
        var homes=_world.Cottages.Where(h=>h.Complete && !h.DemolitionRequested && Buildings.Get(h.Kind).Beds>0).ToArray();
        string key=person.Id+":"+string.Join(";",homes.Select(h=>$"{h.Id}:{_world.People.Count(p=>p.HomeId==h.Id)}"))+":"+person.HomeId;
        if(key!=_homeChoiceKey)
        {
            _homeChoiceKey=key; _homeChoice.Clear();
            foreach(var home in homes)
            {
                int occupied=_world.People.Count(p=>p.HomeId==home.Id);
                _homeChoice.AddItem($"{BuildingName(home.Kind)} {home.Id} · {occupied}/{Buildings.Get(home.Kind).Beds}",home.Id);
                int index=_homeChoice.ItemCount-1;
                _homeChoice.SetItemDisabled(index,occupied>=Buildings.Get(home.Kind).Beds && person.HomeId!=home.Id);
                if(person.HomeId==home.Id) _homeChoice.Select(index);
            }
            if(homes.Length==0) _homeChoice.AddItem("No completed homes",-1);
        }
        _homeNeeds.Text=$"HOME · {(person.HomeId is int id ? homes.FirstOrDefault(h=>h.Id==id)?.Kind+" "+id : "Not assigned")}\n{_world.RestSummary(person)}\n\nRECREATION\n{_world.RecreationSummary(person)}";
        _homeLink.Disabled=person.HomeId==null;
        int chosen=_homeChoice.Selected>=0 ? _homeChoice.GetSelectedId() : -1;
        _previewHome.Disabled=!homes.Any(h=>h.Id==chosen);
        _moveHome.Disabled=chosen==person.HomeId || _world.HomeAssignmentProblem(person.Id,chosen)!=null;
        _moveHome.TooltipText=_world.HomeAssignmentProblem(person.Id,chosen) ?? "Move the resident's next home visit; their current work continues.";
    }
}

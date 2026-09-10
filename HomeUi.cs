using Godot;
using Inlanders.Simulation;
using System.Linq;

public partial class Game
{
    private Label _homeNeeds=null!;
    private Label _mealNeeds=null!;
    private Button _mealLink=null!;
    private Button _homeLink=null!, _moveHome=null!, _previewHome=null!, _recreationLink=null!;
    private OptionButton _homeChoice=null!;
    private string _homeChoiceKey="";
    private World? _homeUiWorld;
    private void MakeHomeUi()
    {
        _mealNeeds=Text("",14,true); _personDetails.AddChild(_mealNeeds);
        _mealLink=Button("Show meal supply",()=>
        {
            if(_selectedPerson<0 || _world.People[_selectedPerson].Meal is not {} meal) return;
            if(_world.People[_selectedPerson].Task!=Work.ReturnMeal && meal.SourceId is int id)
            {
                var pantry=_world.Cottages.FirstOrDefault(c=>c.Id==id); if(pantry==null) return;
                SelectBuilding(id); _focus=OnGround(pantry.Cell.X,pantry.Cell.Z);
            }
            else { _followPerson=false; _focus=OnGround(_world.Stockpile.X,_world.Stockpile.Z); }
            UpdateCamera();
        });
        _personDetails.AddChild(_mealLink);
        _homeNeeds=Text("",14,true); _personDetails.AddChild(_homeNeeds);
        _homeLink=Button("Show home",()=>
        {
            if(_selectedPerson<0 || _world.People[_selectedPerson].HomeId is not int id) return;
            var home=_world.Cottages.FirstOrDefault(c=>c.Id==id); if(home==null) return;
            SelectBuilding(id); _focus=OnGround(home.Cell.X,home.Cell.Z); UpdateCamera();
        });
        _personDetails.AddChild(_homeLink);
        _recreationLink=Button("Show recreation venue",()=> { if(_selectedPerson>=0) ShowServicePlace(ServiceVenue(_world.People[_selectedPerson])); });
        _personDetails.AddChild(_recreationLink);
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
        _mealNeeds.Visible=!_world.Creative;
        _mealNeeds.Text="MEALS\n"+_world.MealSummary(person);
        _mealLink.Visible=!_world.Creative && person.Meal is { } meal && (meal.Reserved || meal.Carrying) &&
            (person.Task==Work.ReturnMeal || meal.SourceId==null || _world.Cottages.Any(c=>c.Id==meal.SourceId));
        _mealLink.Text=person.Task==Work.ReturnMeal?"Show return destination":"Show meal supply";
        if(_homeUiWorld!=_world) { _homeUiWorld=_world; _homeChoiceKey=""; }
        var homes=_world.Cottages.Where(h=>h.Complete && !h.DemolitionRequested && Buildings.Get(h.Kind).Beds>0).ToArray();
        string key=person.Id+":"+string.Join(";",homes.Select(h=>$"{h.Id}:{h.Improved}:{_world.People.Count(p=>p.HomeId==h.Id)}"))+":"+person.HomeId;
        if(key!=_homeChoiceKey)
        {
            _homeChoiceKey=key; _homeChoice.Clear();
            foreach(var home in homes)
            {
                int occupied=_world.People.Count(p=>p.HomeId==home.Id);
                _homeChoice.AddItem($"{BuildingName(home.Kind)} {home.Id}{(home.Improved?" · improved":"")} · {occupied}/{Buildings.Get(home.Kind).Beds}",home.Id);
                int index=_homeChoice.ItemCount-1;
                _homeChoice.SetItemDisabled(index,occupied>=Buildings.Get(home.Kind).Beds && person.HomeId!=home.Id);
                if(person.HomeId==home.Id) _homeChoice.Select(index);
            }
            if(homes.Length==0) _homeChoice.AddItem("No completed homes",-1);
        }
        _homeNeeds.Text=$"HOME · {(person.HomeId is int id ? homes.FirstOrDefault(h=>h.Id==id)?.Kind+" "+id : "Not assigned")}\n{_world.RestSummary(person)}\n\nRECREATION\n{_world.RecreationSummary(person)}";
        _homeLink.Disabled=person.HomeId==null;
        var venue=_world.Cottages.FirstOrDefault(c=>c.Id==ServiceVenue(person));
        _recreationLink.Visible=venue!=null;
        _recreationLink.Text=venue==null?"Show recreation venue":$"{(person.LeisureSiteId!=null?"Current outing":"Last visit")} · {BuildingName(venue.Kind)} {venue.Id}";
        int chosen=_homeChoice.Selected>=0 ? _homeChoice.GetSelectedId() : -1;
        _previewHome.Disabled=!homes.Any(h=>h.Id==chosen);
        _moveHome.Disabled=chosen==person.HomeId || _world.HomeAssignmentProblem(person.Id,chosen)!=null;
        _moveHome.TooltipText=_world.HomeAssignmentProblem(person.Id,chosen) ?? "Move the resident's next home visit; their current work continues.";
    }
}

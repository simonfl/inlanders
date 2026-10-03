using Godot;
using Inlanders.Simulation;
using System.Linq;
public partial class Game
{
    private bool _selectedCommons;
    private Cell? _selectedCommonsCenter;
    private SharedCommons? SelectedSharedPlace=>_world.SharedPlaces.FirstOrDefault(c=>c.Center==_selectedCommonsCenter);
    private PanelContainer _commonsCard=null!;
    private Label _commonsCardText=null!;
    private Button _commonsCardMove=null!,_commonsCardWatch=null!,_commonsCardRemove=null!;
    private void MakeCommonsCard()
    {
        _commonsCard=HudPanel(_hud);_commonsCard.Hide();var column=new VBoxContainer();_commonsCard.AddChild(column);
        _commonsCardText=Text("",14,true);_commonsCardText.CustomMinimumSize=new(282,0);column.AddChild(_commonsCardText);
        _commonsCardWatch=Button("Watch this place",WatchSharedPlace);column.AddChild(_commonsCardWatch);
        _commonsCardMove=Button("Arrange · preview",()=>BeginGatheringPlan(SelectedSharedPlace?.Center,true));column.AddChild(_commonsCardMove);
        _commonsCardRemove=Button("Remove shared place",()=>{if(_selectedCommonsCenter is Cell center)_world.RemoveCommons(center);ClearSelection();});column.AddChild(_commonsCardRemove);
        _commonsCardRemove.TooltipText="Remove the meal place. Carried meals remain physical and residents find another place to eat.";
        column.AddChild(Button("Close [Esc]",ClearSelection));
    }
    private void ShowCommonsCard(Cell? center=null)
    {
        ClearSelection();CloseDrawer();_selectedCommons=true;_selectedCommonsCenter=center??_world.Commons?.Center;UpdateCommonsCard();
    }
    private void UpdateCommonsCard()
    {
        if(_commonsCard==null)return;
        bool show=_selectedCommons && _world.PublicPlace!=null && SelectedSharedPlace!=null && !_atMainMenu && !_watching && !_drawer.Visible && !_placing && !_gatherPlanning;
        _commonsCard.Visible=show;if(!show)return;
        var place=SelectedSharedPlace!;
        _commonsCard.Position=new(_hud.Size.X-322,92);_commonsCard.Size=new(306,0);
        int eating=_world.People.Count(p=>p.Task==Work.EatingMeal && p.Meal?.Commons==true && place.Places.Contains(p.Meal.Seat));
        int arriving=_world.People.Count(p=>p.Meal?.Commons==true && p.Task!=Work.EatingMeal && place.Places.Contains(p.Meal.Seat));
        int quiet=_world.People.Count(p=>_world.QuietSharedPlace(p)==place);
        _commonsCardText.Text=$"SHARED MEAL PLACE · {place.Places.Length} seats\n{(place.Layout==SharedPlaceLayout.Line?"Seating in a line":"Gathered seating")}\n{eating} eating here · {arriving} on the way\n{quiet} visiting between jobs\n"+
            (_world.CommonsFoodNearby(place.Center)?"Food is available nearby. Neighbors bring their ordinary meals.":"Meals need nearby food. Quiet neighbors can still visit between jobs.")+"\nMoving or removing this place is free.";
    }
}

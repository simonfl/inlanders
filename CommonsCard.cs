using Godot;
using Inlanders.Simulation;
using System.Linq;
public partial class Game
{
    private bool _selectedCommons;
    private Cell? _selectedCommonsCenter;
    private SharedCommons? SelectedSharedPlace=>_world.SharedPlaces.FirstOrDefault(c=>c.Center==_selectedCommonsCenter);
    private PanelContainer _commonsCard=null!;
    private bool _showCommonsVisitors;
    private Button _commonsPeople=null!,_commonsVisitorNext=null!;
    private int _commonsVisitorOffset;
    private GridContainer _commonsVisitorGrid=null!;
    private readonly Button[] _commonsVisitors=new Button[6];
    private Label _commonsCardText=null!;
    private Button _commonsCardMove=null!,_commonsCardWatch=null!,_commonsCardRemove=null!,_commonsCardPath=null!;
    private void MakeCommonsCard()
    {
        _commonsCard=HudPanel(_hud);_commonsCard.Hide();var column=new VBoxContainer();_commonsCard.AddChild(column);
        _commonsCardText=Text("",14,true);_commonsCardText.CustomMinimumSize=new(282,0);column.AddChild(_commonsCardText);
        _commonsPeople=Button("People here",()=>_showCommonsVisitors=!_showCommonsVisitors);column.AddChild(_commonsPeople);
        var visitors=_commonsVisitorGrid=new GridContainer{Columns=2};column.AddChild(visitors);
        for(int i=0;i<_commonsVisitors.Length;i++)
        {
            var button=Button("",()=>{});button.AddThemeFontSizeOverride("font_size",12);_commonsVisitors[i]=button;visitors.AddChild(button);
            button.Pressed+=()=>{
                if(_selectedCommonsCenter is not Cell center || !button.HasMeta("person"))return;
                int person=(int)button.GetMeta("person");ClearSelection();ShowDailyLife(person);_dailySharedOrigin=center;_dailyExpanded=true;_followPerson=true;
            };
        }
        _commonsVisitorNext=Button("More visitors",()=>_commonsVisitorOffset+=6);column.AddChild(_commonsVisitorNext);
        _commonsCardWatch=Button("Watch this place",WatchSharedPlace);column.AddChild(_commonsCardWatch);
        _commonsCardMove=Button("Arrange · preview",()=>BeginGatheringPlan(SelectedSharedPlace?.Center,true));column.AddChild(_commonsCardMove);
        _commonsCardPath=Button("Path from here",BeginSharedPlacePath);column.AddChild(_commonsCardPath);
        _commonsCardPath.TooltipText="Preview a walking path to a home, workplace or another shared place. Choose the destination and Apply; Esc returns here without changing the ground.";
        _commonsCardRemove=Button("Remove shared place",()=>{if(_selectedCommonsCenter is Cell center)_world.RemoveCommons(center);ClearSelection();});column.AddChild(_commonsCardRemove);
        _commonsCardRemove.TooltipText="Remove the meal place. Carried meals remain physical and residents find another place to eat.";
        column.AddChild(Button("Close [Esc]",ClearSelection));
    }
    private void ShowCommonsCard(Cell? center=null)
    {
        ClearSelection();CloseDrawer();_showCommonsVisitors=false;_commonsVisitorOffset=0;_selectedCommons=true;_selectedCommonsCenter=center??_world.Commons?.Center;UpdateCommonsCard();
    }
    private void UpdateCommonsCard()
    {
        if(_commonsCard==null)return;
        bool show=_selectedCommons && _world.PublicPlace!=null && SelectedSharedPlace!=null && !_atMainMenu && !_watching && !_drawer.Visible && !_placing && !_gatherPlanning;
        _commonsCard.Visible=show;if(!show)return;
        var place=SelectedSharedPlace!;
        _commonsCard.Position=new(_hud.Size.X-322,92);_commonsCard.Size=new(306,0);
        _commonsCardPath.Disabled=_world.PathProblem(place.Center)!=null;
        _commonsVisitorGrid.Visible=_showCommonsVisitors;
        _commonsPeople.Text=_showCommonsVisitors?"Back to shared place":"People here";
        foreach(var control in new[]{_commonsCardWatch,_commonsCardMove,_commonsCardPath,_commonsCardRemove})control.Visible=!_showCommonsVisitors;
        var visitors=_world.ReadSharedVisitors(place.Center);
        _commonsVisitorNext.Visible=_showCommonsVisitors && visitors.Length>6;
        _commonsVisitorOffset=visitors.Length==0?0:_commonsVisitorOffset%visitors.Length;
        for(int i=0;i<_commonsVisitors.Length;i++)
        {
            var button=_commonsVisitors[i];button.Visible=i<visitors.Length;if(i>=visitors.Length)continue;
            var visitor=visitors[(i+_commonsVisitorOffset)%visitors.Length];button.SetMeta("person",visitor.Person);button.Text=_world.People[visitor.Person].Name+"\n"+visitor.Activity;
            button.TooltipText="Follow this actual visitor, then return to this shared place.";
        }
        int eating=_world.People.Count(p=>p.Task==Work.EatingMeal && p.Meal?.Commons==true && place.Places.Contains(p.Meal.Seat));
        int arriving=_world.People.Count(p=>p.Meal?.Commons==true && p.Task!=Work.EatingMeal && place.Places.Contains(p.Meal.Seat));
        int quiet=_world.People.Count(p=>_world.QuietSharedPlace(p)==place);
        _commonsCardText.Text=$"SHARED MEAL PLACE · {place.Places.Length} seats\n{(place.Layout==SharedPlaceLayout.Line?"Seating in a line":"Gathered seating")}\n{eating} eating here · {arriving} on the way\n{quiet} visiting between jobs\n"+
            (_world.CommonsFoodNearby(place.Center)?"Food is available nearby. Neighbors bring their ordinary meals.":"Meals need nearby food. Quiet neighbors can still visit between jobs.")+"\nMoving or removing this place is free.";
        if(_showCommonsVisitors)_commonsCardText.Text=$"SHARED PLACE · {visitors.Length} current visitors\n"+(visitors.Length==0?"No one is visiting right now.":"Select a neighbor to follow their actual activity.");
    }
}

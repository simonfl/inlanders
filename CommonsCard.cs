using Godot;
using Inlanders.Simulation;
using System.Linq;
public partial class Game
{
    private bool _selectedCommons;
    private PanelContainer _commonsCard=null!;
    private Label _commonsCardText=null!;
    private Button _commonsCardMove=null!,_commonsCardWatch=null!,_commonsCardRemove=null!;
    private void MakeCommonsCard()
    {
        _commonsCard=HudPanel(_hud);_commonsCard.Hide();var column=new VBoxContainer();_commonsCard.AddChild(column);
        _commonsCardText=Text("",14,true);_commonsCardText.CustomMinimumSize=new(282,0);column.AddChild(_commonsCardText);
        _commonsCardWatch=Button("Watch this place",()=>{
            if(_world.Commons is not {} place)return;
            ClearSelection();_focus=OnGround(place.Center.X,place.Center.Z);_camera.Size=14;UpdateCamera();ToggleWatch();
        });column.AddChild(_commonsCardWatch);
        _commonsCardMove=Button("Move · preview",()=>BeginGatheringPlan(_world.Commons?.Center,true));column.AddChild(_commonsCardMove);
        _commonsCardRemove=Button("Remove shared place",()=>{_world.RemoveCommons();ClearSelection();});column.AddChild(_commonsCardRemove);
        _commonsCardRemove.TooltipText="Remove the meal place. Carried meals remain physical and residents find another place to eat.";
        column.AddChild(Button("Close [Esc]",ClearSelection));
    }
    private void ShowCommonsCard()
    {
        ClearSelection();CloseDrawer();_selectedCommons=true;UpdateCommonsCard();
    }
    private void UpdateCommonsCard()
    {
        if(_commonsCard==null)return;
        bool show=_selectedCommons && _world.PublicPlace!=null && _world.Commons!=null && !_atMainMenu && !_watching && !_drawer.Visible && !_placing && !_gatherPlanning;
        _commonsCard.Visible=show;if(!show)return;
        var place=_world.Commons!;
        _commonsCard.Position=new(_hud.Size.X-322,92);_commonsCard.Size=new(306,0);
        int eating=_world.People.Count(p=>p.Task==Work.EatingMeal && p.Meal?.Commons==true);
        int arriving=_world.People.Count(p=>p.Meal?.Commons==true && p.Task!=Work.EatingMeal);
        _commonsCardText.Text=$"SHARED MEAL PLACE\n{eating} eating here · {arriving} on the way\n"+
            (_world.CommonsFoodNearby(place.Center)?"Food is available nearby. Neighbors bring their ordinary meals.":"No food available nearby now. Move closer to food or restore the nearby supply.")+"\nMoving or removing this place is free.";
    }
}

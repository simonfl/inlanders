using Godot;
using Inlanders.Simulation;
using System.Linq;
public partial class Game
{
    private int _householdFrom=-1,_householdTo=-1;
    private World? _householdMoveWorld;
    private PanelContainer _householdMovePanel=null!;
    private Label _householdMoveInfo=null!;
    private Button _householdChange=null!,_householdConfirm=null!;
    private void MakeHouseholdMoveUi()
    {
        _householdMovePanel=HudPanel(_hud);var column=new VBoxContainer();_householdMovePanel.AddChild(column);
        _householdMoveInfo=Text("",14,true);_householdMoveInfo.CustomMinimumSize=new(286,0);column.AddChild(_householdMoveInfo);
        _householdConfirm=Button("Use these homes",()=>{int target=_householdTo;if(_world.MoveHousehold(_householdFrom,target)){_householdFrom=_householdTo=-1;_householdMovePanel.Hide();ShowWorkplaceCard(target);Notice("Homes exchanged. Neighbors finish their current activity, then use their new home.");}});column.AddChild(_householdConfirm);
        column.AddChild(Button("Cancel [Esc]",CancelHouseholdMove));_householdMovePanel.Hide();
    }
    private void CancelHouseholdMove(){int home=_householdFrom;_householdFrom=_householdTo=-1;_householdMovePanel.Hide();if(_householdMoveWorld==_world && _world.Cottages.Any(c=>c.Id==home))ShowWorkplaceCard(home);}
    private void RenderHouseholdMove()
    {
        if(_householdMoveWorld!=_world || _atMainMenu)_householdFrom=-1;
        _householdMovePanel.Visible=_householdFrom>=0;if(_householdFrom<0)return;
        _householdMovePanel.Position=new(16,92);_householdMovePanel.Size=new(310,0);
        string Names(int home)=>string.Join(", ",_world.People.Where(p=>p.HomeId==home).Select(p=>p.Name));
        var target=_world.Cottages.FirstOrDefault(c=>c.Id==_householdTo);
        string? problem=_world.HouseholdMoveProblem(_householdFrom,_householdTo);
        _householdMoveInfo.Text="CHOOSE A HOME\n"+Names(_householdFrom)+"\n"+(target==null?"Click another house in the village. If occupied, its neighbors exchange homes with these residents.":"To "+BuildingName(target.Kind)+" "+target.Id+".\n"+(Names(target.Id)==""?"The destination is empty.":Names(target.Id)+" move into the first home.")+"\n"+(problem??"Current work and meals continue. Their next home visit uses this arrangement."));
        _householdConfirm.Disabled=problem!=null;
    }
    private bool HandleHouseholdMove(InputEvent input)
    {
        if(_householdFrom<0)return false;
        if(input is InputEventKey{Pressed:true,Keycode:Key.Escape} || input is InputEventMouseButton{Pressed:true,ButtonIndex:MouseButton.Right}){CancelHouseholdMove();return true;}
        if(input is InputEventMouseButton{Pressed:true,ButtonIndex:MouseButton.Left} click && !PointerOverHud(click.Position))
        {
            if(Ground(click.Position) is Vector3 point){var cell=new Cell(Mathf.RoundToInt(point.X),Mathf.RoundToInt(point.Z));_householdTo=_world.Cottages.FirstOrDefault(c=>World.Footprint(c).Contains(cell))?.Id??-1;_selectedSite=_householdTo;RefreshSelection();}return true;
        }
        return false;
    }
}

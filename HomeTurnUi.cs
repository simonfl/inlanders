using Godot;
using Inlanders.Simulation;
using System.Linq;
public partial class Game
{
    private int _turnHome=-1,_homeTurn;
    private World? _turnWorld;
    private Button _turnHomeButton=null!,_turnLeft=null!,_turnRight=null!,_turnApply=null!;
    private PanelContainer _turnPanel=null!;
    private Label _turnInfo=null!;
    private Node3D? _turnGround;
    private string _turnKey="";
    private void MakeHomeTurn(VBoxContainer column)
    {
        _turnHomeButton=Button("Turn home · preview",()=>{var home=_world.Cottages.Single(c=>c.Id==_workCardSite);_turnHome=home.Id;_turnWorld=_world;_homeTurn=home.Rotation;_turnKey="";});column.AddChild(_turnHomeButton);
        _turnPanel=HudPanel(_hud);var box=new VBoxContainer();_turnPanel.AddChild(box);box.AddChild(Text("TURN THIS HOME",15));
        _turnInfo=Text("",14,true);_turnInfo.CustomMinimumSize=new(280,0);box.AddChild(_turnInfo);
        var row=new HBoxContainer();box.AddChild(row);_turnLeft=Button("Turn left",()=>_homeTurn=(_homeTurn+3)%4);row.AddChild(_turnLeft);_turnRight=Button("Turn right",()=>_homeTurn=(_homeTurn+1)%4);row.AddChild(_turnRight);
        _turnApply=Button("Use this direction",()=>{var home=_world.Cottages.FirstOrDefault(c=>c.Id==_turnHome);if(home!=null && _world.MoveBuilding(home.Id,home.Cell,_homeTurn)){EndHomeTurn();CreateActors();RenderActors(0);RefreshSelection();}});box.AddChild(_turnApply);
        box.AddChild(Button("Cancel [Esc]",EndHomeTurn));_turnPanel.Hide();
    }
    private void EndHomeTurn(){if(_turnHome<0)return;_turnHome=-1;_turnPanel.Hide();_turnGround?.Hide();_nextWorkCard=0;}
    private void RenderHomeTurn()
    {
        var home=_world.Cottages.FirstOrDefault(c=>c.Id==_turnHome);
        if(home==null || _turnWorld!=_world || _selectedSite!=_turnHome || _atMainMenu || _placing || _drawer.Visible || _watching){EndHomeTurn();return;}
        _turnPanel.Show();_turnPanel.Position=new(_workCardRight?_hud.Size.X-326:16,92);_turnPanel.Size=new(310,0);
        var problem=_world.RelocationProblem(home.Id,home.Cell,_homeTurn);
        _turnInfo.Text="Turn around the same ground anchor. Blue marks the proposed entrance. Household and furnishings stay.\n"+(problem??"Clear approach. No material cost.");
        _turnApply.Disabled=problem!=null || _homeTurn==home.Rotation;
        _turnGround??=new();if(_turnGround.GetParent()==null)AddChild(_turnGround);_turnGround.Show();
        string key=$"{home.Id}/{_homeTurn}/{problem}";if(key==_turnKey)return;_turnKey=key;Clear(_turnGround);
        foreach(var c in World.Footprint(home.Cell,_homeTurn,home.Kind))GroundPatch(_turnGround,c.X,c.Z,.88f,.88f,problem==null?new("c9b56d"):new Color("bc7463"),.11f);
        var door=World.Door(home.Cell,_homeTurn);GroundPatch(_turnGround,door.X,door.Z,.85f,.85f,new("75c7d0"),.12f);
    }
}

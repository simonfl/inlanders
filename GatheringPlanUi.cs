using Godot;
using Inlanders.Simulation;
using System.Linq;

public partial class Game
{
    private bool _gatherPlanning,_gatherSpread=true,_planningCommons;
    private int _commonsSeats=6,_commonsDirection;
    private SharedPlaceLayout _commonsLayout;
    private HBoxContainer _commonsArrangement=null!;
    private Button _commonsShape=null!,_commonsTurn=null!;
    private Cell? _gatherReplaceAt;
    private HBoxContainer _commonsSizes=null!;
    private readonly Button[] _commonsSizeButtons=new Button[3];
    private Button _commonsEntry=null!,_commonsRemove=null!;
    private Label _gatherPlanTitle=null!;
    private Cell? _gatherPlanAt;
    private World? _gatherPlanWorld;
    private PanelContainer _gatherPlanPanel=null!;
    private Label _gatherPlanInfo=null!;
    private Button _gatherPlanStart=null!,_gatherLayout=null!,_gatherPlanEntry=null!,_gatherGoalsCancel=null!;
    private Node3D _gatherPlanMarks=null!;
    private string _gatherPlanKey="";
    private float _gatherPlanRefresh;
    private void MakeGatheringPlanUi()
    {
        MakeCommonsCard();
        _gatherPlanPanel=HudPanel(_hud);_gatherPlanPanel.Hide();var column=new VBoxContainer();_gatherPlanPanel.AddChild(column);
        _gatherPlanTitle=Text("OUTDOOR MEAL",16);column.AddChild(_gatherPlanTitle);_gatherPlanInfo=Text("",14,true);column.AddChild(_gatherPlanInfo);
        _commonsSizes=new();column.AddChild(_commonsSizes);
        for(int i=0;i<3;i++){int seats=(i+1)*2;var button=Button($"{seats} seats",()=>{_commonsSeats=seats;_gatherPlanRefresh=0;UpdateGatheringPlan();});button.SizeFlagsHorizontal=Control.SizeFlags.ExpandFill;_commonsSizeButtons[i]=button;_commonsSizes.AddChild(button);}
        _commonsArrangement=new();column.AddChild(_commonsArrangement);
        _commonsShape=Button("Gathered",()=>{_commonsLayout=_commonsLayout==SharedPlaceLayout.Gathered?SharedPlaceLayout.Line:SharedPlaceLayout.Gathered;_gatherPlanRefresh=0;UpdateGatheringPlan();});_commonsArrangement.AddChild(_commonsShape);
        _commonsTurn=Button("Turn [R]",TurnCommonsProposal);_commonsArrangement.AddChild(_commonsTurn);
        foreach(var button in new[]{_commonsShape,_commonsTurn})button.SizeFlagsHorizontal=Control.SizeFlags.ExpandFill;
        _gatherLayout=Button("Seating: circle",()=>{_gatherSpread=!_gatherSpread;_gatherPlanRefresh=0;UpdateGatheringPlan();});column.AddChild(_gatherLayout);
        _gatherPlanStart=Button("Gather at these places",ConfirmGatheringPlan);column.AddChild(_gatherPlanStart);
        column.AddChild(Button("Cancel [Esc]",CancelGatheringPlan));_gatherPlanMarks=new();AddChild(_gatherPlanMarks);
    }
    private void BeginGatheringPlan(Cell? center=null,bool commons=false)
    {
        if(commons?!_world.CanArrangeCommons:_world.Neighborhood?.Complete!=true || _world.Gathering?.Active==true)return;
        CloseManagementUi();_placing=false;RefreshGhost();CancelCameraDrag();
        _commonsArrangement.Visible=_commonsSizes.Visible=commons;_gatherReplaceAt=commons && center!=null && _world.SharedPlaces.Any(c=>c.Center==center)?center:null;
        _commonsSeats=_gatherReplaceAt!=null?_world.SharedPlaces.Single(c=>c.Center==center).Places.Length:6;
        var prior=_world.SharedPlaces.FirstOrDefault(c=>c.Center==_gatherReplaceAt);_commonsLayout=prior?.Layout??SharedPlaceLayout.Gathered;_commonsDirection=prior?.Rotation??0;
        _planningCommons=commons;_gatherPlanTitle.Text=commons?"SHARED PLACE":"OUTDOOR MEAL";_gatherPlanStart.Text=commons?"Keep this shared place":"Gather at these places";_gatherLayout.Visible=!commons;
        _gatherPlanning=true;_gatherPlanWorld=_world;_gatherPlanAt=center;_gatherPlanPanel.Show();_gatherPlanRefresh=0;UpdateGatheringPlan();
    }
    private void CancelGatheringPlan()
    {
        if(!_gatherPlanning)return;
        _gatherPlanning=false;_gatherPlanAt=null;_gatherPlanWorld=null;_gatherPlanPanel.Hide();Clear(_gatherPlanMarks);_gatherPlanKey="";CancelCameraDrag();
    }
    private void UpdateGatheringPlan()
    {
        if(_gatherPlanEntry!=null){_gatherPlanEntry.Visible=_world.Neighborhood?.Complete==true;_gatherPlanEntry.Text=_world.Gathering?.Active==true?"Show outdoor meal":"Plan an outdoor meal";_gatherGoalsCancel.Visible=_world.Gathering?.Active==true;}
        UpdateCommonsView();UpdateCommonsCard();
        if(!_gatherPlanning)return;
        if(_gatherPlanWorld!=_world || _placing || _watching || _atMainMenu){CancelGatheringPlan();return;}
        _gatherPlanPanel.Position=new(_hud.Size.X-310,92);_gatherPlanPanel.Size=new(294,0);
        if(_uiTime<_gatherPlanRefresh)return;_gatherPlanRefresh=_uiTime+.25f;
        var seats=_gatherPlanAt is Cell at?(_planningCommons?_world.CommonsPlaces(at,_commonsSeats,_gatherReplaceAt,_commonsLayout,_commonsDirection):_world.GatheringPlaces(at,_gatherSpread)):System.Array.Empty<Cell>();
        string? problem=_gatherPlanAt is Cell target?(_planningCommons?_world.CommonsProblem(target,_commonsSeats,_gatherReplaceAt,_commonsLayout,_commonsDirection):_world.GatheringProblem(target,_gatherSpread)):"Click open ground to preview real places. No building is required.";
        for(int i=0;i<3;i++)_commonsSizeButtons[i].Modulate=_commonsSeats==(i+1)*2?_cream:Colors.White;
        _commonsShape.Text=_commonsLayout==SharedPlaceLayout.Gathered?"Gathered":"In a line";
        _gatherLayout.Text=_gatherSpread?"Seating: circle":"Seating: compact";
        _gatherPlanStart.Disabled=problem!=null;
        _gatherPlanInfo.Text=$"{seats.Length}/{_world.Population} reachable places\n"+(problem??"Each marker is a real place. Everyone brings their next meal, waits together, then eats.")+"\n\nChoose another spot by clicking ground. Middle-drag or WASD moves the camera. No food is committed until you confirm.";
        if(_planningCommons)_gatherPlanInfo.Text=$"{seats.Length}/{_commonsSeats} reachable places\n"+(problem??"People bring ordinary meals here; no waiting for the whole village.")+"\n"+(_gatherPlanAt is Cell foodAt && _world.CommonsFoodNearby(foodAt)?"Food is available nearby now.":"Meals need nearby food. Quiet neighbors can still visit between jobs.")+"\n\nClick another spot; R turns the proposal. Select finished ground to change that place.";
        string key=$"{_gatherPlanAt}:{_gatherSpread}:{_commonsLayout}:{_commonsDirection}:{problem}:"+string.Join(';',seats);if(key==_gatherPlanKey)return;
        Clear(_gatherPlanMarks);_gatherPlanKey=key;
        foreach(var cell in seats)
        {
            Cylinder(_gatherPlanMarks,OnGround(cell.X,cell.Z,.04f),.30f,.05f,new(problem==null?"d7bf83":"c48170"));
            if(_planningCommons && _commonsLayout==SharedPlaceLayout.Line)
            {
                var forward=World.RotateOffset(cell,0,1,_commonsDirection);var d=(forward.Point-cell.Point)*.37f;
                Box(_gatherPlanMarks,OnGround(cell.X+d.X,cell.Z+d.Y,.06f),new(.10f,.06f,.10f),new("f1dfae"));
            }
        }
        if(_gatherPlanAt is Cell center)Box(_gatherPlanMarks,OnGround(center.X,center.Z,.07f),new(.18f,.08f,.18f),new("f1dfae"));
    }
    private void TurnCommonsProposal(){_commonsDirection=(_commonsDirection+1)%4;_gatherPlanRefresh=0;UpdateGatheringPlan();}
    private bool CommitSharedPlace(Cell at)=>_gatherReplaceAt is Cell original?_world.MoveCommons(original,at,_commonsSeats,_commonsLayout,_commonsDirection):_world.AddCommons(at,_commonsSeats,_commonsLayout,_commonsDirection);
    private void ConfirmGatheringPlan()
    {
        if(_gatherPlanAt is not Cell at)return;
        if(!(_planningCommons?CommitSharedPlace(at):_world.BeginGathering(at,_gatherSpread))){_gatherPlanRefresh=0;UpdateGatheringPlan();Notice((_planningCommons?_world.CommonsProblem(at,_commonsSeats,_gatherReplaceAt,_commonsLayout,_commonsDirection):_world.GatheringProblem(at,_gatherSpread))??"Choose another spot.");return;}
        bool commons=_planningCommons;CancelGatheringPlan();SaveWorld();UpdateHud();if(commons){if(_world.PublicPlace!=null)ShowCommonsCard(at);Notice("A shared place for meals and quiet visits. Click its ground to watch, move or remove it.");return;}Notice("People will bring their next meal here. Goals shows the gathering and lets you cancel.");
    }
    private bool HandleGatheringPlanInput(InputEvent input)
    {
        if(!_gatherPlanning)return false;
        if(_planningCommons && input is InputEventKey{Pressed:true,Echo:false,Keycode:Key.R}){TurnCommonsProposal();return true;}
        if(input is InputEventKey{Pressed:true,Keycode:Key.Escape} || input is InputEventMouseButton{Pressed:true,ButtonIndex:MouseButton.Right}){CancelGatheringPlan();return true;}
        if(input is InputEventKey{Pressed:true} key && key.Keycode is Key.B or Key.V or Key.G or Key.I or Key.O or Key.H or Key.U or Key.T or Key.C or Key.P){CancelGatheringPlan();return false;}
        if(input is not InputEventMouseButton{ButtonIndex:MouseButton.Left} button || PointerOverHud(button.Position))return false;
        if(button.Pressed && Ground(button.Position) is Vector3 point){_gatherPlanAt=new(Mathf.RoundToInt(point.X),Mathf.RoundToInt(point.Z));_gatherPlanRefresh=0;UpdateGatheringPlan();}
        return true;
    }
}

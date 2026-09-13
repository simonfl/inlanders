using Godot;
using Inlanders.Simulation;
using System.Linq;

public partial class Game
{
    private bool _gatherPlanning,_gatherSpread=true,_planningCommons;
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
        _gatherPlanPanel=HudPanel(_hud);_gatherPlanPanel.Hide();var column=new VBoxContainer();_gatherPlanPanel.AddChild(column);
        _gatherPlanTitle=Text("OUTDOOR MEAL",16);column.AddChild(_gatherPlanTitle);_gatherPlanInfo=Text("",14,true);column.AddChild(_gatherPlanInfo);
        _gatherLayout=Button("Seating: circle",()=>{_gatherSpread=!_gatherSpread;_gatherPlanRefresh=0;UpdateGatheringPlan();});column.AddChild(_gatherLayout);
        _gatherPlanStart=Button("Gather at these places",ConfirmGatheringPlan);column.AddChild(_gatherPlanStart);
        column.AddChild(Button("Cancel [Esc]",CancelGatheringPlan));_gatherPlanMarks=new();AddChild(_gatherPlanMarks);
    }
    private void BeginGatheringPlan(Cell? center=null,bool commons=false)
    {
        if(_world.Neighborhood?.Complete!=true || !commons && _world.Gathering?.Active==true)return;
        CloseManagementUi();_placing=false;RefreshGhost();CancelCameraDrag();
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
        UpdateCommonsView();
        if(!_gatherPlanning)return;
        if(_gatherPlanWorld!=_world || _placing || _watching || _atMainMenu){CancelGatheringPlan();return;}
        _gatherPlanPanel.Position=new(_hud.Size.X-310,92);_gatherPlanPanel.Size=new(294,0);
        if(_uiTime<_gatherPlanRefresh)return;_gatherPlanRefresh=_uiTime+.25f;
        var seats=_gatherPlanAt is Cell at?(_planningCommons?_world.CommonsPlaces(at):_world.GatheringPlaces(at,_gatherSpread)):System.Array.Empty<Cell>();
        string? problem=_gatherPlanAt is Cell target?(_planningCommons?_world.CommonsProblem(target):_world.GatheringProblem(target,_gatherSpread)):"Click open ground to preview real places. No building is required.";
        _gatherLayout.Text=_gatherSpread?"Seating: circle":"Seating: compact";
        _gatherPlanStart.Disabled=problem!=null;
        _gatherPlanInfo.Text=$"{seats.Length}/{_world.Population} reachable places\n"+(problem??"Each marker is a real place. Everyone brings their next meal, waits together, then eats.")+"\n\nChoose another spot by clicking ground. Middle-drag or WASD moves the camera. No food is committed until you confirm.";
        if(_planningCommons)_gatherPlanInfo.Text=$"{seats.Length}/6 reachable places\n"+(problem??"People with nearby food bring their ordinary meals here. No waiting for the whole village.")+"\n\nClick another spot to rearrange. Routes stay open; move or remove this place from Goals.";
        string key=$"{_gatherPlanAt}:{_gatherSpread}:{problem}:"+string.Join(';',seats);if(key==_gatherPlanKey)return;
        Clear(_gatherPlanMarks);_gatherPlanKey=key;
        foreach(var cell in seats)Cylinder(_gatherPlanMarks,OnGround(cell.X,cell.Z,.04f),.30f,.05f,new(problem==null?"d7bf83":"c48170"));
        if(_gatherPlanAt is Cell center)Box(_gatherPlanMarks,OnGround(center.X,center.Z,.07f),new(.18f,.08f,.18f),new("f1dfae"));
    }
    private void ConfirmGatheringPlan()
    {
        if(_gatherPlanAt is not Cell at)return;
        if(!(_planningCommons?_world.SetCommons(at):_world.BeginGathering(at,_gatherSpread))){_gatherPlanRefresh=0;UpdateGatheringPlan();Notice(_world.GatheringProblem(at,_gatherSpread)??"Choose another spot.");return;}
        bool commons=_planningCommons;CancelGatheringPlan();SaveWorld();UpdateHud();if(commons){Notice("A shared place for ordinary meals. Nearby food matters; move or remove it in Goals.");return;}Notice("People will bring their next meal here. Goals shows the gathering and lets you cancel.");
    }
    private bool HandleGatheringPlanInput(InputEvent input)
    {
        if(!_gatherPlanning)return false;
        if(input is InputEventKey{Pressed:true,Keycode:Key.Escape} || input is InputEventMouseButton{Pressed:true,ButtonIndex:MouseButton.Right}){CancelGatheringPlan();return true;}
        if(input is InputEventKey{Pressed:true} key && key.Keycode is Key.B or Key.V or Key.G or Key.I or Key.O or Key.H or Key.U or Key.T or Key.C or Key.P){CancelGatheringPlan();return false;}
        if(input is not InputEventMouseButton{ButtonIndex:MouseButton.Left} button || PointerOverHud(button.Position))return false;
        if(button.Pressed && Ground(button.Position) is Vector3 point){_gatherPlanAt=new(Mathf.RoundToInt(point.X),Mathf.RoundToInt(point.Z));_gatherPlanRefresh=0;UpdateGatheringPlan();}
        return true;
    }
}

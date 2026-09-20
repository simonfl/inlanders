using Godot;
using Inlanders.Simulation;
using System.Linq;
public partial class Game
{
    private VBoxContainer _foundingHallGoals=null!;
    private Button _hallBegin=null!,_hallFinish=null!;
    private void MakeFoundingHallUi(VBoxContainer column)
    {
        _hallBegin=Button("Optional project · Lakeside hall",()=>{if(_world.BeginFoundingHall()){SaveWorld();UpdateHud();_drawerPages[2].ScrollVertical=0;}});
        _foundingGoals.AddChild(_hallBegin);
        _foundingHallGoals=new();column.AddChild(_foundingHallGoals);
        _foundingHallGoals.AddChild(Button("Find far-shore stone",()=>{var stone=_world.Map.StoneDeposits[0];_focus=OnGround(stone.Cell.X,stone.Cell.Z);UpdateCamera();CloseDrawer();Notice("Build a quarry near this outcrop. Workers can walk around the lake.");}));
        _foundingHallGoals.AddChild(Button("Plan quarry · 6 logs",()=>BeginPlacement(BuildingKind.Quarry)));
        _foundingHallGoals.AddChild(Button("Plan sawmill · 6 logs",()=>BeginPlacement(BuildingKind.Sawmill)));
        _foundingHallGoals.AddChild(Button("Plan hall · 8 planks + 12 stone",()=>BeginPlacement(BuildingKind.GatheringHall)));
        _hallFinish=Button("This hall is ready",()=>{if(_world.FinishFoundingHall()){_paused=true;SaveWorld();UpdateHud();_drawerPages[2].ScrollVertical=0;}});_foundingHallGoals.AddChild(_hallFinish);
        _foundingHallGoals.AddChild(Button("Return to main menu",ReturnToMainMenu));
        _foundingHallGoals.MoveChild(_hallFinish,0);
        _foundingHallGoals.Hide();
    }
    private void UpdateFoundingHallUi()
    {
        var f=_world.Founding!;_hallBegin.Visible=f.Finished && f.HallProject==0 && !f.RiverFarmstead;
        if(f.HallProject==0)return;
        _goalTitle.Text=f.HallProject==2?"A hall for your village":"A lakeside hall";
        _goalArrival.Text=f.HallProject==2?"Neighbors have begun using the hall. Keep shaping its surroundings, or leave the village here.":"Build a place to gather. Quarry the far shore; saw timber into planks.";
        _goalArrival.TooltipText="A hall near homes favors short visits. Stone goes through storage; a hall near the quarry does not automatically shorten deliveries. All buildings remain available.";
        var hall=_world.FoundingHall;
        _objective.Text=f.HallProject==2?"Project finished. Ordinary village life continues.":hall==null?"Plan the hall now; workers gather its materials.":
            !hall.Complete?$"Hall {hall.Id}: {hall.Delivered}/8 planks · {hall.DeliveredStone}/12 stone\n"+_world.FinishFoundingHallProblem():
            f.HallVisitors.TryGetValue(hall.Id,out int person)?$"{_world.People[person].Name} took a break at the hall. Finish when satisfied.":"The hall is built. Play to let a neighbor finish an ordinary break there.";
        _foundingGoals.Visible=f.HallProject==2;_foundingHallGoals.Visible=f.HallProject==1;
        _hallFinish.Disabled=_world.FinishFoundingHallProblem()!=null;_hallFinish.TooltipText=_world.FinishFoundingHallProblem()??"An optional ending after actual use.";
        _menuButtons[2].Text=f.HallProject==2?"Village · Hall finished":"Village · Hall";
    }
}

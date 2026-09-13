using Godot;
using Inlanders.Simulation;
using System.IO;

public partial class Game
{
    private VBoxContainer _foundingGoals=null!;
    private Button _foundingInvite=null!,_foundingFinish=null!,_foundingContinue=null!,_foundingLeave=null!;
    private Label _foundingFood=null!;
    private Button _foundingFoodView=null!;
    private string FoundingPath=>Path.Combine(Path.GetDirectoryName(_creativeSavePath)!,"founding.json");
    private void PlaySettlementsMenu()
    {
        MenuPage("Play");
        _mainColumn.AddChild(Text("Choose a place to begin. Each village has its own save.",16,true));
        MenuButton("Found a village · A home by the water",FoundingMenu);
        MenuButton("Short introduction · A place to gather",()=>CourtStartMenu(true));
        MenuButton("Back",ShowMainMenu);
    }
    private void FoundingMenu()
    {
        MenuPage("A home by the water");
        _mainColumn.AddChild(Text("Eight founders, one home and a lake. Build homes and choose how to feed your village: fish along the shore, preserve woodland for hunting, or give land to gardens. Workers share the jobs.",16,true));
        _mainColumn.AddChild(Text("Invite two households when you are ready to grow. Let the newcomers settle in, then finish here or keep building. Construction uses real timber and labor; arrivals never happen on a timer.",16,true));
        if(File.Exists(FoundingPath))MenuButton("Resume · A home by the water",()=>MenuAttempt(()=>EnterFromMenu(World.LoadFile(FoundingPath))));
        void Start()=>MenuAttempt(()=>{var w=World.NewFoundingSettlement();w.SaveFile(FoundingPath);EnterFromMenu(w);});
        MenuButton("New · A home by the water",()=>{if(File.Exists(FoundingPath))ConfirmMenu("Start a new village?","Replace this founding village?",Start,FoundingMenu);else Start();});
        MenuButton("Back",PlaySettlementsMenu);
    }
    private void MakeFoundingUi(VBoxContainer column)
    {
        _foundingGoals=new();column.AddChild(_foundingGoals);
        _foundingGoals.AddChild(Button("Build homes and workplaces",()=>{if(!_drawer.Visible || _tabs.CurrentTab!=1)ToggleDrawer(1);SelectBuildSection(0);_buildingFilter.Select(0);UpdateVillageDirectory();}));
        _foundingFood=Text("",14,true);_foundingGoals.AddChild(_foundingFood);
        _foundingFoodView=Button("Inspect food in the village",()=>{if(!_showFoodMap)ToggleFoodMap();else CloseDrawer();});_foundingGoals.AddChild(_foundingFoodView);
        _foundingInvite=Button("Invite two neighbors",()=>{if(_world.InviteNewcomers()){SaveWorld();UpdateHud();}else Notice(_world.InvitationProblem()??"Not ready.");});_foundingGoals.AddChild(_foundingInvite);
        _foundingFinish=Button("This village is ready",()=>{if(_world.FinishFounding()){_paused=true;SaveWorld();UpdateHud();_drawerPages[2].ScrollVertical=0;}});_foundingGoals.AddChild(_foundingFinish);
        _foundingContinue=Button("Keep building",()=>{CloseDrawer();_paused=false;});_foundingGoals.AddChild(_foundingContinue);
        _foundingLeave=Button("Finish here · main menu",ReturnToMainMenu);_foundingGoals.AddChild(_foundingLeave);
        MakeFoundingHallUi(column);
        _foundingGoals.Hide();
    }
    private void UpdateFoundingUi()
    {
        var f=_world.Founding!;
        _foundingHallGoals.Hide();
        _courtExperienceGoals.Hide();_neighborhoodGoals.Hide();_journeyAction.Hide();_visitorPanel.Hide();
        _campaignSelection.Hide();_campaignControls.Hide();_standaloneGuide.Hide();_supperButton.Hide();_supperBreadLink.Hide();_riverAction.Hide();_progress.Hide();
        _goalDashboard.Hide();_trackedGoalPanel.Hide();_goalArrival.Show();
        _foundingGoals.Show();_goalTitle.Text=f.Finished?"A village you founded":"A home by the water";
        _goalArrival.Text=f.Finished?"Keep growing when you want. More neighbors need more food; their first meal does not prove lasting supply.":"Build homes for eight founders. Choose a food source, then invite two households when ready.";
        _objective.Show();_objective.Text=$"Homes: {_world.Housed}/{_world.Population} residents · New neighbors settled: {f.Settled.Count}/{System.Math.Max(4,_world.Population-World.InitialPopulation)}\n"+(_world.FinishFoundingProblem()??"Everyone has settled in. Finish when satisfied, or keep building.");
        _foundingInvite.Disabled=_world.InvitationProblem()!=null;_foundingInvite.TooltipText=_world.InvitationProblem()??"Two neighbors join now. Their first meals will increase demand.";
        _foundingInvite.Visible=true;
        var flow=_world.ReadFoodFlow();
        float delivered=flow.Seconds>0?flow.Delivered*60f/flow.Seconds:0;
        _foundingFood.Text=$"FOOD FOR GROWTH\n{_world.EdibleStored} stored · {_world.Population} portions needed per minute\n"+
            (flow.Seconds>=60?$"Recent deliveries: {delivered:0.0} / minute"+(delivered<_world.Population?" · compare with demand":""):"Gathering a minute of delivery history.");
        _foundingFood.TooltipText=$"Observed over the last {flow.Seconds:0} simulated seconds. New producer deliveries only; transfers between stores are excluded. This is history, not a forecast or a guarantee that meals arrive on time. Two newcomers add two portions per minute. Inspect food to check locations and routes.";
        _foundingFinish.Visible=!f.Finished && _world.Population>=12;_foundingFinish.Disabled=_world.FinishFoundingProblem()!=null;
        _foundingFinish.TooltipText=_world.FinishFoundingProblem()??"An optional ending, not an economy assessment.";
        _foundingContinue.Visible=f.Finished;_foundingLeave.Visible=f.Finished;
        _menuButtons[2].Text=f.Finished?"Village · Finished":"Village";_menuButtons[2].TooltipText="Your founding village [G]";
        if(_tabs.CurrentTab==2)_drawerTitle.Text="Your village";
        UpdateFoundingHallUi();
    }
}

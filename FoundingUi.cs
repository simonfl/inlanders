using Godot;
using Inlanders.Simulation;
using System.IO;

public partial class Game
{
    private VBoxContainer _foundingGoals=null!;
    private Button _foundingInvite=null!,_foundingFinish=null!,_foundingContinue=null!,_foundingLeave=null!;
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
        _foundingGoals.AddChild(Button("Build homes and workplaces",()=>ToggleDrawer(0)));
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
        _goalArrival.Text=f.Finished?"Your new neighbors have homes and have eaten here. Stay and shape the village, or leave it here.":"Build homes for eight founders. Choose a food source, then invite two households when ready.";
        _objective.Show();_objective.Text=$"Homes: {_world.Housed}/{_world.Population} residents · New neighbors settled: {f.Settled.Count}/{System.Math.Max(4,_world.Population-World.InitialPopulation)}\n"+(_world.FinishFoundingProblem()??"Everyone has settled in. Finish when satisfied, or keep building.");
        _foundingInvite.Disabled=_world.InvitationProblem()!=null;_foundingInvite.TooltipText=_world.InvitationProblem()??"Two neighbors join now. Their first meals will increase demand.";
        _foundingInvite.Visible=!f.Finished;
        _foundingFinish.Visible=!f.Finished && _world.Population>=12;_foundingFinish.Disabled=_world.FinishFoundingProblem()!=null;
        _foundingFinish.TooltipText=_world.FinishFoundingProblem()??"An optional ending, not an economy assessment.";
        _foundingContinue.Visible=f.Finished;_foundingLeave.Visible=f.Finished;
        _menuButtons[2].Text=f.Finished?"Village · Finished":"Village";_menuButtons[2].TooltipText="Your founding village [G]";
        if(_tabs.CurrentTab==2)_drawerTitle.Text="Your village";
        UpdateFoundingHallUi();
    }
}

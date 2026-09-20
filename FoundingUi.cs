using Godot;
using Inlanders.Simulation;
using System.IO;

public partial class Game
{
    private VBoxContainer _foundingGoals=null!;
    private Button _foundingInvite=null!,_foundingFinish=null!,_foundingContinue=null!,_foundingLeave=null!;
    private Button _foundingCommons=null!,_foundingCommonsRemove=null!;
    private Label _foundingFood=null!;
    private Button _foundingFoodView=null!;
    private string FoundingPath=>Path.Combine(Path.GetDirectoryName(_creativeSavePath)!,"founding.json");
    private void FoundingMenu()
    {
        MenuPage("A home by the water");
        _mainColumn.AddChild(Text("Eight founders, one home and a lake. Give everyone a home. The existing foragers feed this small start; choose more food as you grow. Workers share jobs automatically.",16,true));
        _mainColumn.AddChild(Text("Invite two households when you are ready to grow. Let the newcomers settle in, then finish here or keep building. Construction uses real timber and labor; arrivals never happen on a timer.",16,true));
        if(File.Exists(FoundingPath))MenuButton("Resume · A home by the water",()=>MenuAttempt(()=>EnterFromMenu(World.LoadFile(FoundingPath))));
        void Start()=>MenuAttempt(()=>{var w=World.NewFoundingSettlement();w.SaveFile(FoundingPath);EnterFromMenu(w);});
        MenuButton("New · A home by the water",()=>{if(File.Exists(FoundingPath))ConfirmMenu("Start a new village?","Replace this founding village?",Start,FoundingMenu);else Start();});
        MenuButton("Back",ComparisonMenu);
    }
    private void MakeFoundingUi(VBoxContainer column)
    {
        _foundingGoals=new();column.AddChild(_foundingGoals);
        _foundingGoals.AddChild(Button("Build homes and workplaces",()=>{if(!_drawer.Visible || _tabs.CurrentTab!=1)ToggleDrawer(1);SelectBuildSection(0);_buildingFilter.Select(_world.Founding?.WorkingVillage==true?0:_world.Founding?.RiverFarmstead==true && !_world.FoundingHasNewFood?2:1);UpdateVillageDirectory();}));
        _foundingCommons=Button("Make a shared place",()=>BeginGatheringPlan(_world.Commons?.Center,true));_foundingGoals.AddChild(_foundingCommons);
        _foundingCommonsRemove=Button("Remove shared place",()=>{_world.RemoveCommons();SaveWorld();UpdateHud();});_foundingGoals.AddChild(_foundingCommonsRemove);
        _foundingFood=Text("",14,true);_foundingGoals.AddChild(_foundingFood);
        _foundingFoodView=Button("Inspect food in the village",()=>{if(!_showFoodMap)ToggleFoodMap();else CloseDrawer();});_foundingGoals.AddChild(_foundingFoodView);
        _foundingInvite=Button("Invite two neighbors",()=>{if(_world.InviteNewcomers()){SaveWorld();UpdateHud();}else Notice(_world.InvitationProblem()??"Not ready.");});_foundingGoals.AddChild(_foundingInvite);
        _foundingFinish=Button("This village is ready",()=>{if(_world.FinishFounding()){_paused=true;SaveWorld();UpdateHud();_drawerPages[2].ScrollVertical=0;}});_foundingGoals.AddChild(_foundingFinish);
        _foundingContinue=Button("Keep shaping the village",()=>{CloseDrawer();_paused=false;});_foundingGoals.AddChild(_foundingContinue);
        _foundingLeave=Button("Finish here · main menu",ReturnToMainMenu);_foundingGoals.AddChild(_foundingLeave);
        MakeFoundingHallUi(column);
        _foundingGoals.MoveChild(_foundingContinue,0);
        _foundingGoals.MoveChild(_hallBegin,1);
        _foundingGoals.MoveChild(_foundingLeave,2);
        _foundingGoals.MoveChild(_foundingFinish,3);
        _foundingGoals.MoveChild(_foundingInvite,4);
        _foundingGoals.MoveChild(_foundingFood,_foundingGoals.GetChildCount()-2);
        _foundingGoals.MoveChild(_foundingFoodView,_foundingGoals.GetChildCount()-1);
        _foundingGoals.Hide();
    }
    private void UpdateFoundingUi()
    {
        var f=_world.Founding!;
        _foundingCommons.Visible=f.RiverFarmstead;_foundingCommonsRemove.Visible=_world.Commons!=null;
        _foundingCommons.Text=_world.Commons==null?"Make a shared place":"Rearrange shared place";
        _foundingCommons.TooltipText="Choose outdoor ground near food. Residents bring their ordinary meals here; no ceremony or new building required.";
        // Keep active controls visible across mouse-down and mouse-up frames.
        _foundingHallGoals.Visible=f.HallProject==1;
        _courtExperienceGoals.Hide();_neighborhoodGoals.Hide();_journeyAction.Hide();_visitorPanel.Hide();
        _campaignSelection.Hide();_campaignControls.Hide();_standaloneGuide.Hide();_supperButton.Hide();_supperBreadLink.Hide();_riverAction.Hide();_progress.Hide();
        _goalDashboard.Hide();_trackedGoalPanel.Hide();_goalArrival.Hide();
        _foundingGoals.Visible=f.HallProject!=1;_goalTitle.Text=f.Finished?"A village you founded":f.WorkingVillage?"The long way home":f.RiverFarmstead?"A place of our own":"A home by the water";
        _goalArrival.Text=f.Finished?"Keep growing when you want. More neighbors need more food; their first meal does not prove lasting supply.":"Build homes for eight founders. Choose a food source, then invite two households when ready.";
        _objective.Show();_objective.Text=$"Homes: {_world.Housed}/{_world.Population} residents · New neighbors settled: {f.Settled.Count}/{System.Math.Max(4,_world.Population-World.InitialPopulation)}\n"+(f.Finished?"Finished. Shape this village, grow, or leave it here.":_world.FinishFoundingProblem()??"Everyone has settled in. Finish whenever you like.");
        if(f.RiverFarmstead)_objective.Text=$"Homes: {_world.Housed}/{_world.Population} residents\n"+(_world.FoundingHasNewFood?"First food delivered.\n":"Choose food before provisions run out.\n")+(f.Finished?"Finished. Stay, improve, or leave this village here.":_world.FinishFoundingProblem()??"Homes and the first food supply are working. Finish when satisfied; continued supply still needs your care.");
        if(f.WorkingVillage && !f.Finished)_objective.Text="An inhabited village across an inlet. Watch a resident and choose what to improve.\nHomes: "+_world.Housed+"/"+_world.Population+"\n"+(_world.FoundingHasNewFood?"Food is flowing. Finish when satisfied, or stay and reshape this place.":"The field and oven are already working. Watch their first delivery, then choose what to change.");
        _foundingInvite.Disabled=_world.InvitationProblem()!=null;_foundingInvite.TooltipText=_world.InvitationProblem()??"Two neighbors join now. Their first meals will increase demand.";
        _foundingInvite.Visible=true;
        var flow=_world.ReadFoodFlow();
        float delivered=flow.Seconds>0?flow.Delivered*60f/flow.Seconds:0;
        _foundingFood.Text=$"FOOD FOR GROWTH\n{_world.EdibleStored} stored · {_world.Population} portions needed per minute\n"+
            (flow.Seconds>=60?$"Recent deliveries: {delivered:0.0} / minute"+(delivered<_world.Population?" · compare with demand":""):"Gathering a minute of delivery history.");
        _foundingFood.TooltipText=$"Observed over the last {flow.Seconds:0} simulated seconds. New producer deliveries only; transfers between stores are excluded. This is history, not a forecast or a guarantee that meals arrive on time. Two newcomers add two portions per minute. Inspect food to check locations and routes.";
        if(f.ProvisionedLife)_foundingFood.Text+="\nShared workers provision about four meals each, then return home or take building work. Food work resumes as stores fall; dedicated workers keep their own workplace routine.";
        if(f.RiverFarmstead)_foundingFood.Text=_foundingFood.Text.Replace("FOOD FOR GROWTH","DAILY FOOD");
        _foundingFinish.Visible=!f.Finished && (f.RiverFarmstead || _world.Population>=12);_foundingFinish.Disabled=_world.FinishFoundingProblem()!=null;
        _foundingFinish.TooltipText=_world.FinishFoundingProblem()??"An optional ending, not an economy assessment.";
        _foundingContinue.Visible=f.Finished;_foundingLeave.Visible=f.Finished;
        _menuButtons[2].Text=f.Finished?"Village · Finished":"Village";_menuButtons[2].TooltipText="Your founding village [G]";
        if(_tabs.CurrentTab==2)_drawerTitle.Text="Your village";
        UpdateFoundingHallUi();
    }
}

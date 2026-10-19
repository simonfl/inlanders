using Godot;
using Inlanders.Simulation;
using System;
using System.IO;
public partial class Game
{
    private VBoxContainer _challengeGoals=null!;
    private Label _challengeProgress=null!;
    private Button _challengeInvite=null!,_challengeContinue=null!,_challengeLeave=null!;
    private bool _challengeRelaxed;
    private void ChallengeMenu()
    {
        MenuPage("A place for newcomers");
        _mainColumn.AddChild(Text("Four neighbors seek a home here. Choose a livelihood, prepare homes, then invite them. Help the larger village feed itself. Different food sources and arrangements can work.",17,true));
        MenuButton(_challengeRelaxed?"Mode: Relaxed · change":"Mode: Normal · change",()=>{_challengeRelaxed=!_challengeRelaxed;ChallengeMenu();});
        _mainColumn.AddChild(Text(_challengeRelaxed?"Free construction; real work and meals, with no hunger penalty.":"Real building materials, shared labor and recoverable food shortages. No deadline until you choose to invite.",14,true));
        var profile=new HamletProfile(_challengeRelaxed,Newcomers:true);
        string path=Path.Combine(Path.GetDirectoryName(_creativeSavePath)!,profile.SaveName);
        if(File.Exists(path))MenuButton("Resume challenge",()=>MenuAttempt(()=>{EnterFromMenu(World.LoadFile(path));ToggleDrawer(2);}));
        void Start()=>MenuAttempt(()=>{var w=profile.Create();w.SaveFile(path);EnterFromMenu(w);ToggleDrawer(2);});
        MenuButton("New challenge",()=>{if(File.Exists(path))ConfirmMenu("Begin again?","Replace this challenge?",Start,ChallengeMenu);else Start();});
        MenuButton("Back",ShowMainMenu);
    }
    private void MakeChallengeUi(VBoxContainer column)
    {
        _challengeGoals=new();column.AddChild(_challengeGoals);
        _challengeProgress=Text("",16,true);_challengeGoals.AddChild(_challengeProgress);
        _challengeInvite=Button("Invite four neighbors",()=>{if(_world.InviteChallengeNeighbors()){SaveWorld();UpdateHud();}else Notice(_world.ChallengeInvitationProblem()??"Not ready.");});_challengeGoals.AddChild(_challengeInvite);
        _challengeGoals.AddChild(Button("Choose buildings",()=>{ToggleDrawer(1);SelectBuildSection(0);}));
        _challengeContinue=Button("Challenge complete · continue building",()=>{_world.Challenge!.Continuing=true;SaveWorld();CloseDrawer();_paused=false;});_challengeGoals.AddChild(_challengeContinue);
        _challengeLeave=Button("Save and leave",ReturnToMainMenu);_challengeGoals.AddChild(_challengeLeave);_challengeGoals.Hide();
    }
    private void UpdateChallengeUi()
    {
        var c=_world.Challenge;_challengeGoals.Visible=c!=null;if(c==null)return;
        _foundingGoals.Hide();_foundingHallGoals.Hide();_objective.Hide();_goalArrival.Hide();
        _goalTitle.Text="A PLACE FOR NEWCOMERS";
        string progress=c.InvitedAt==null?$"Prepare for four neighbors\nSpare beds: {Math.Min(4,_world.SpareBeds)}/4\nFresh food delivered: {Math.Min(16,_world.ChallengeDeliveries)}/16\nFood ready: {Math.Min(32,_world.EdibleStored)}/32\n\nChoose your own food sources and home arrangement. Starting provisions do not count as production.":!c.Arrived?$"Four neighbors are on their way\nArrival in {Math.Max(0,30-(_world.Food.Time-c.InvitedAt.Value)):0} seconds. Keep homes accessible and food available.":
            $"Help the newcomers settle\nResidents housed: {_world.Housed}/16\nNewcomers with two meals: {_world.ChallengeNeighborsFed}/4\nFresh food since arrival: {Math.Min(32,_world.ChallengeDeliveries)}/32\n\nOrdinary meals count automatically. Food deliveries and completed meals keep their progress.";
        _challengeProgress.Text=c.Complete?"CHALLENGE COMPLETE\nFour newcomers have homes and two meals each. Your village delivered 32 fresh portions after their arrival.\nStay and build, or save this village and leave.":progress;
        _challengeInvite.Visible=c.InvitedAt==null;_challengeInvite.Disabled=_world.ChallengeInvitationProblem()!=null;_challengeInvite.TooltipText=_world.ChallengeInvitationProblem()??"They arrive 30 seconds after you invite them.";
        _challengeContinue.Visible=c.Complete;_challengeLeave.Visible=c.Complete;
        _menuButtons[2].Text=c.Complete?"Challenge · complete":c.Arrived?"Challenge · settling":"Challenge";
        _menuButtons[2].TooltipText="Your objective and progress [G]";
        if(_tabs.CurrentTab==2)_drawerTitle.Text="Challenge";
    }
}

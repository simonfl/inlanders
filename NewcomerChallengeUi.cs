using Godot;
using Inlanders.Simulation;
using System;
using System.IO;
using System.Linq;
public partial class Game
{
    private VBoxContainer _challengeGoals=null!;
    private Label _challengeProgress=null!;
    private Button _challengeInvite=null!,_challengeContinue=null!,_challengeLeave=null!;
    private bool _challengeRelaxed;
    private void ChallengeMenu()
    {
        MenuPage("A place for newcomers");
        _mainColumn.AddChild(Text("Four neighbors seek a home beside a crowded inlet. A small kitchen garden is not enough for the larger village. Expand toward the northern meadow, improve a crossing, or rearrange the home bank. Choose your own food sources.",17,true));
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
        _challengeProgress=Text("",14,true);_challengeGoals.AddChild(_challengeProgress);
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
        string contract="Goal: welcome four neighbors into reachable homes, let each eat two meals, and deliver 32 fresh portions after arrival. Everyone must be fed when you finish.\n";
        string progress=c.InvitedAt==null?$"PREPARE\nSpare beds {Math.Min(4,_world.SpareBeds)}/4 · fresh food {Math.Min(16,_world.ChallengeDeliveries)}/16\nStored food {Math.Min(32,_world.EdibleStored)}/32\n\n{contract}\nInvite when ready; they arrive 30 seconds later. Choose any food sources and layout.":!c.Arrived?$"ON THEIR WAY · {Math.Max(0,30-(_world.Food.Time-c.InvitedAt.Value)):0}s\n\n{contract}":
            $"SETTLING IN\nReachable homes: {_world.ChallengeHoused}/16\nNewcomers with two meals: {_world.ChallengeNeighborsFed}/4\nFresh deliveries: {Math.Min(32,_world.ChallengeDeliveries)}/32\nEveryone fed: {_world.People.Count(p=>p.Fed)}/16\n\n{_world.ChallengeSettlingProblem()??"Ready to finish."}\nMeals and deliveries keep their progress.";
        _challengeProgress.Text=c.Complete?"CHALLENGE COMPLETE\nFour newcomers have homes and two meals each. Your village delivered 32 fresh portions after their arrival.\nStay and build, or save this village and leave.":progress;
        _challengeInvite.Visible=c.InvitedAt==null;_challengeInvite.Disabled=_world.ChallengeInvitationProblem()!=null;_challengeInvite.TooltipText=_world.ChallengeInvitationProblem()??"They arrive 30 seconds after you invite them.";
        _challengeContinue.Visible=c.Complete;_challengeLeave.Visible=c.Complete;
        _menuButtons[2].Text=c.Complete?"Challenge · complete":c.Arrived?"Challenge · settling":"Challenge";
        _menuButtons[2].TooltipText="Your objective and progress [G]";
        if(_tabs.CurrentTab==2)_drawerTitle.Text="Challenge";
    }
}

using Godot;
using Inlanders.Simulation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
public partial class Game
{
    private VBoxContainer _challengeGoals=null!;
    private Label _challengeProgress=null!;
    private Button _challengeAccess=null!,_challengeWatch=null!;
    private World? _challengeObservedWorld;
    private bool _challengeSawArrival,_challengeSawCompletion;
    private Button _challengeInvite=null!,_challengeContinue=null!,_challengeLeave=null!;
    private bool _challengeRelaxed,_challengeRepair;
    private void ChallengeMenu()
    {
        MenuPage(_challengeRepair?"The far-bank fields":"A place for newcomers");
        MenuButton(_challengeRepair?"Situation: cut-off fields · change":"Situation: first settlement · change",()=>{_challengeRepair=!_challengeRepair;ChallengeMenu();});
        _mainColumn.AddChild(Text(_challengeRepair?"Two existing fields are cut off by a river channel. Reconnect them, move cultivation onto the crowded home bank, or build a different food supply. Prepare homes and invite four newcomers when ready.":"Four neighbors seek a home beside a crowded inlet. A small kitchen garden is not enough for the larger village. Expand toward the northern meadow, improve a crossing, or rearrange the home bank. Choose your own food sources.",17,true));
        MenuButton(_challengeRelaxed?"Mode: Relaxed · change":"Mode: Normal · change",()=>{_challengeRelaxed=!_challengeRelaxed;ChallengeMenu();});
        _mainColumn.AddChild(Text(_challengeRelaxed?"Free construction; real work and meals, with no hunger penalty.":"Real building materials, shared labor and recoverable food shortages. No deadline. Invitation starts a 30-second arrival journey.",14,true));
        var profile=new HamletProfile(_challengeRelaxed,Newcomers:true,Repair:_challengeRepair);
        string path=Path.Combine(Path.GetDirectoryName(_creativeSavePath)!,profile.SaveName);
        if(File.Exists(path))MenuButton("Resume challenge",()=>MenuAttempt(()=>{EnterFromMenu(World.LoadFile(path));ToggleDrawer(2);}));
        void Start()=>MenuAttempt(()=>{var w=profile.Create();w.SaveFile(path);EnterFromMenu(w);ToggleDrawer(2);});
        MenuButton("New challenge",()=>{if(File.Exists(path))ConfirmMenu("Begin again?","Replace this challenge?",Start,ChallengeMenu);else Start();});
        MenuButton("Back",ShowMainMenu);
    }
    private Villager? ChallengePerson()=>_world.People.FirstOrDefault(p=>!p.Fed) ??
        _world.People.FirstOrDefault(p=>p.Id is >=12 and <16 && (_world.Challenge?.Meals.GetValueOrDefault(p.Id)??0)<2) ??
        _world.People.FirstOrDefault(p=>p.Id==12);
    private void MakeChallengeUi(VBoxContainer column)
    {
        _challengeGoals=new();column.AddChild(_challengeGoals);
        _challengeAccess=Button("Inspect a cut-off field",()=>{
            var field=_world.Cottages.FirstOrDefault(b=>b.Kind==BuildingKind.VegetableField && _world.ReadWorkplace(b).MissingAccess);
            if(field==null)return;_noticeUntil=0;CloseDrawer();_focus=OnGround(field.Cell.X,field.Cell.Z);UpdateCamera();ShowWorkplaceCard(field.Id);
        });_challengeGoals.AddChild(_challengeAccess);
        _challengeWatch=Button("Find newcomers",()=>{var person=ChallengePerson();if(person==null)return;_noticeUntil=0;ShowDailyLife(person.Id);_followPerson=true;});_challengeGoals.AddChild(_challengeWatch);
        _challengeProgress=Text("",14,true);_challengeGoals.AddChild(_challengeProgress);
        _challengeInvite=Button("Invite four neighbors",()=>{if(_world.InviteChallengeNeighbors()){SaveWorld();UpdateHud();}else Notice(_world.ChallengeInvitationProblem()??"Not ready.");});_challengeGoals.AddChild(_challengeInvite);
        _challengeGoals.AddChild(Button("Choose buildings",()=>{ToggleDrawer(1);SelectBuildSection(0);}));
        _challengeContinue=Button("Challenge complete · continue building",()=>{_world.Challenge!.Continuing=true;SaveWorld();CloseDrawer();_paused=false;});_challengeGoals.AddChild(_challengeContinue);
        _challengeLeave=Button("Save and leave",ReturnToMainMenu);_challengeGoals.AddChild(_challengeLeave);_challengeGoals.Hide();
    }
    private void UpdateChallengeUi()
    {
        var c=_world.Challenge;_challengeGoals.Visible=c!=null;if(c==null)return;
        if(_challengeObservedWorld!=_world){_challengeObservedWorld=_world;_challengeSawArrival=c.Arrived;_challengeSawCompletion=c.Complete;}
        else
        {
            if(c.Arrived && !_challengeSawArrival)Notice("Jeanne, Pierre, Marguerite and Louis have arrived. Challenge [G] can take you to them.");
            if(c.Complete && !_challengeSawCompletion)Notice("The newcomers have settled. Challenge complete — keep building or save and leave through Challenge [G].");
            _challengeSawArrival=c.Arrived;_challengeSawCompletion=c.Complete;
        }
        _challengeWatch.Visible=c.Arrived && !c.Continuing;
        if(ChallengePerson() is {} person)_challengeWatch.Text="Find "+person.Name+(person.Fed?c.Complete?" · settled":" · settling in":" · missed a meal");
        _foundingGoals.Hide();_foundingHallGoals.Hide();_objective.Hide();_goalArrival.Hide();
        _challengeAccess.Visible=c.Repair && !c.Complete && _world.Cottages.Any(b=>b.Kind==BuildingKind.VegetableField && _world.ReadWorkplace(b).MissingAccess);
        _goalTitle.Text=c.Repair?"THE FAR-BANK FIELDS":"A PLACE FOR NEWCOMERS";
        string contract="Goal: four newcomers need reachable homes and two meals each. Deliver 32 fresh portions after arrival. Everyone must be fed when you finish.\n";
        string progress=c.InvitedAt==null?$"PREPARE\nSpare beds {Math.Min(4,_world.SpareBeds)}/4 · fresh food {Math.Min(16,_world.ChallengeDeliveries)}/16\nStored food {Math.Min(32,_world.EdibleStored)}/32\n\n{contract}\nInvite when ready; they arrive 30 seconds later. Choose any food sources and layout.":!c.Arrived?$"ON THEIR WAY · {Math.Max(0,30-(_world.Food.Time-c.InvitedAt.Value)):0}s\n\n{contract}":
            $"SETTLING IN\nReachable homes: {_world.ChallengeHoused}/16\nNewcomers with two meals: {_world.ChallengeNeighborsFed}/4\nFresh deliveries: {Math.Min(32,_world.ChallengeDeliveries)}/32\nEveryone fed: {_world.People.Count(p=>p.Fed)}/16\n\n{_world.ChallengeSettlingProblem()??"Ready to finish."}\nMeals and deliveries keep their progress.";
        _challengeProgress.Text=c.Complete?"CHALLENGE COMPLETE\nFour newcomers have homes and two meals each. Your village delivered 32 fresh portions after their arrival.\nStay and build, or save this village and leave. After continuing, use People to welcome more neighbors when you choose.":progress;
        _challengeInvite.Visible=c.InvitedAt==null;_challengeInvite.Disabled=_world.ChallengeInvitationProblem()!=null;_challengeInvite.TooltipText=_world.ChallengeInvitationProblem()??"They arrive 30 seconds after you invite them.";
        _challengeContinue.Visible=c.Complete && !c.Continuing;_challengeLeave.Visible=c.Complete;
        _menuButtons[2].Text=c.Complete?"Challenge · complete":c.Arrived?$"Settling · {_world.ChallengeNeighborsFed}/4":c.InvitedAt!=null?"Neighbors on their way":_world.ChallengeInvitationProblem()==null?"Ready to invite four":"Challenge";
        _menuButtons[2].TooltipText="Your objective and progress [G]";
        if(_tabs.CurrentTab==2)_drawerTitle.Text="Challenge";
    }
}

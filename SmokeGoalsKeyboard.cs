using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async Task CheckGoalsKeyboard()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        async Task Focus(string target)
        {
            for(int i=0;i<140 && _goalsFocus!=target;i++){await Press(Key.Tab);await Frames();}
            Check(_goalsFocus==target,"Goal control not reachable: "+target);
        }
        _campaignPath="artifacts/goals-keyboard-book.json";_campaignBook=new();
        foreach(int width in new[]{960,1440})
        {
            GetWindow().Size=new(width,width==960?640:900);AdoptWorld(World.NewCampaign(1));_paused=true;await Frames();
            string saved=_world.SaveJson();await Press(Key.G);await Frames();
            Check(_goalsKeyboard && _goalsFocus=="overview","Intro Goals entry failed");
            await Press(Key.Pagedown);await Press(Key.Pageup);await Frames();Check(saved==_world.SaveJson(),"Reading intro mutated world");
            await Focus("guidance");await Press(Key.Enter);await Frames();Check(!_world.Campaign!.Guidance,"Guidance action did not activate");
            await Press(Key.Enter);Check(_world.Campaign.Guidance,"Guidance did not restore");
            await Focus("standalone");await Frames();Check(GetViewport().GuiGetFocusOwner()==_standaloneLevel,"Standalone link lacks keyboard focus");await Focus("guidance");
            await Capture($"artifacts/goals-keyboard-intro-{width}.png");await Press(Key.Escape);Check(!_goalsKeyboard,"Intro did not close");

            var w=World.LoadFile("artifacts/finale-campaign-prepared-second.json");AdoptWorld(w);_paused=true;await Frames();
            saved=w.SaveJson();await Press(Key.G);await Focus("why:housing");await Press(Key.Enter);await Frames();
            Check(_goalItems["housing"].Help.Visible,"Goal explanation failed");
            await Focus("residents:housing");await Press(Key.Enter);await Frames();
            Check(_goalsPage==0 && _campaignServiceKey=="housing" && _serviceFilter.Selected==5,"Evidence filter lost goal");
            await Press(Key.Right);await Frames();Check(_serviceFilter.Selected==6,$"Counted resident filter failed: selected {_serviceFilter.Selected}, focus {_goalsFocus}, page {_goalsPage}, active {_goalsKeyboard}, visible {_serviceFilter.IsVisibleInTree()}, disabled {_serviceFilter.Disabled}, controls {string.Join(',',GoalsControls().Select(r=>r.Key))}");
            int id=w.People.Last().Id;await Focus($"person:{id}");await Capture($"artifacts/goals-keyboard-evidence-{width}.png");
            await Press(Key.Space);await Frames();Check(_selectedPerson==id && _goalsInspectPage==0 && _paused,"Wrong evidence resident inspected");
            await Press(Key.Escape);await Frames();Check(_goalsFocus==$"person:{id}","Evidence return lost resident");
            await Press(Key.Escape);await Frames();Check(_goalsPage==2 && _goalsFocus=="residents:housing","Return lost goal");
            int site=GoalPlaces("housing").First().Id;string place=$"place:housing:{site}";await Focus(place);
            _goalPlaceKeys.Clear();UpdateGoalPlaces();await Frames();Check(_goalsFocus==place,"Place rebuild lost stable ID");
            await Press(Key.Enter);await Frames();Check(_selectedSite==site && _goalsInspectPage==2,"Wrong goal place inspected");
            await UiClick(_goalsBackInspect);await Frames();Check(!_goalsKeyboard && _tabs.CurrentTab==2,"Mouse Back failed");
            Check(saved==w.SaveJson(),"Read-only goal evidence changed simulation");
            await Press(Key.G);await Frames();string action="phase:"+GoalsVersion;await Focus(action);
            // Readiness can change between focusing an action and pressing Enter.
            var homes=w.Cottages.Where(c=>Buildings.Get(c.Kind).Beds>0).ToArray();w.Cottages.RemoveAll(c=>homes.Contains(c));
            await Press(Key.Enter);await Frames();Check(w.Campaign!.Finale!.Phase==2 && _riverAction.Disabled,"Stale ready action advanced");
            w.Cottages.AddRange(homes);w.Validate();await Frames();await Focus(action);await Press(Key.Enter);await Frames();
            Check(w.Campaign.Finale.Phase==3 && _goalsFocus=="overview","Assessment activation/phase reset failed");
            saved=w.SaveJson();await Press(Key.Enter);await Frames();Check(saved==w.SaveJson(),"Phase replacement activated another action");
            await Capture($"artifacts/goals-keyboard-assessment-{width}.png");
            await Press(Key.I);await Frames();Check(_economyKeyboard && !_goalsKeyboard,"Economy handoff failed");
            await Press(Key.G);await Frames();Check(_goalsKeyboard && !_economyKeyboard,"Goals handoff failed");
            await Press(Key.O);await Frames();_viewName.GrabFocus();await Press(Key.G);Check(!_goalsKeyboard,"Typing opened Goals");_viewName.ReleaseFocus();
            await Press(Key.G);AdoptWorld(World.LoadJson(saved));await Frames();Check(!_goalsKeyboard && !_goalsBackList.Visible && !_goalsBackInspect.Visible,"Reload retained Goals context");
        }
        var finale=World.LoadFile("artifacts/finale-campaign-ready-supper.json");AdoptWorld(finale);_paused=true;await Frames();await Press(Key.G);await Focus("supper");await Press(Key.Enter);await Frames();
        Check(finale.Food.Celebrating && !_goalsKeyboard,"Supper action did not begin existing flow");
        await Press(Key.G);await Focus("guidance");
        for(int i=0;i<2000 && !finale.Campaign!.Complete;i++)finale.Tick(.1f);
        await Frames();Check(finale.Campaign!.Complete && _goalsFocus=="overview","Live completion did not reset action focus");
        string complete=finale.SaveJson();await Press(Key.Enter);Check(complete==finale.SaveJson(),"Completion auto-activated replay/continue");
        finale.Validate();Check(World.LoadJson(complete).SaveJson()==complete,"Goal completion save differs");
        GD.Print("PASS: Goals keyboard intro, counted resident/place evidence and returns, 960/1440, stale readiness, real assessment/supper/completion, handoffs, typing and saves.");
    }
}

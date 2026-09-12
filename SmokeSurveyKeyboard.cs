using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async Task CheckSurveyKeyboard()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        async Task Focus(string key)
        {
            for(int i=0;i<35 && _surveyFocus!=key;i++){await Press(Key.Tab);await Frames();}
            Check(_surveyFocus==key,"Survey control not reachable: "+key);
        }
        async Task Source(SourceKey key)
        {
            await Focus("choice");for(int i=0;i<=_sourceList.Count && _selectedSource!=key;i++){await Press(Key.Right);await Frames();}
            Check(_selectedSource==key,"Picker did not select source identity");
        }
        foreach(int width in new[]{960,1440})foreach(bool lake in new[]{false,true})
        {
            var w=lake?World.NewLakeMap():World.NewCreative(true);
            if(lake)Check(w.Place(new(3,4),true,BuildingKind.FishingDock)!=null,"Dock fixture failed");
            else
            {
                Check(w.Place(new(-9,2),false,BuildingKind.Quarry)!=null,"Quarry fixture failed");
                var habitat=w.Map.Wildlife.First();var cell=w.Map.Land.First(c=>w.PlacementProblem(c,0,BuildingKind.HuntingLodge)==null && (c.Point-habitat.Cell.Point).LengthSquared()<=64);
                Check(w.Place(cell,0,BuildingKind.HuntingLodge)!=null,"Hunting fixture failed");
            }
            AdoptWorld(w);_paused=true;GetWindow().Size=new(width,width==960?640:900);await Frames();string saved=w.SaveJson();
            BeginPlacement(BuildingKind.Cottage);await Press(Key.U);await Frames();
            Check(_surveyKeyboard && _surveying && !_placing,"Survey did not replace placement");
            foreach(var source in w.ResourceSources().ToArray())
            {
                await Source(source.Key);Check(_sourceInfo.Text.Contains("available"),"Missing source evidence");
                Check(_focus.X==source.Cell.X && _focus.Z==source.Cell.Z,"Keyboard camera differs from source");
                await Capture($"artifacts/survey-keyboard-{source.Key.Kind}-{source.Key.Id}-{width}.png");
                if(_sourceWorkplaceLinks.Count>0)
                {
                    int id=_sourceWorkplaceLinks.Keys.First();await Focus($"site:{id}");await Press(Key.Enter);await Frames();
                    Check(_surveyKeyboardSite==id && _selectedSite==id,"Wrong workplace inspected");
                    var camera=_focus;var angle=_angle;await Press(Key.Q);await Press(Key.R);await Press(Key.Pagedown);await Frames();Check(_focus==camera && _angle==angle && _paused,"Survey leaked world shortcuts");
                    await Press(Key.Escape);await Frames();Check(_selectedSource==source.Key && _surveyKeyboardSite<0,"Back lost source identity");
                }
                Check(saved==w.SaveJson(),"Read-only survey changed world");
            }
            await Press(Key.Escape);Check(!_surveying && !_surveyKeyboard,"Survey did not finish");
            await Press(Key.U);await Source(w.ResourceSources().First(s=>w.ReadResourceSurvey(s.Key)!.Workplaces.Length>0).Key);
            int mouseSite=_sourceWorkplaceLinks.Keys.First();await Focus($"site:{mouseSite}");await Press(Key.Enter);await Frames();
            var mouseSource=_lastSurveySource;await UiClick(_surveyBack);await Frames();
            Check(!_surveyKeyboard && _selectedSource==mouseSource,"Mouse Back lost source");
            await Press(Key.U);Check(!_surveying,"U did not finish a mouse-operated survey");
            await Press(Key.I);await Frames();await Press(Key.Enter);await Frames(); // Economy opens on Survey.
            Check(_surveyKeyboard && !_economyKeyboard,"Economy survey handoff failed");
            await Press(Key.H);await Frames();Check(_watching && !_surveyKeyboard && !_surveying,"Watch handoff failed");
            await Press(Key.U);await Frames();Check(_surveyKeyboard && !_watching,"Survey did not exit Watch");
            await Press(Key.O);await Frames();_viewName.GrabFocus();await Press(Key.U);Check(!_surveyKeyboard,"Typing opened survey");_viewName.ReleaseFocus();
            await Press(Key.U);await Source(w.ResourceSources().First().Key);
            var chosen=_selectedSource;w.Map.StoneDeposits.Reverse();_nextSourceRefresh=0;UpdateResourceSurvey();await Frames();
            Check(_selectedSource==chosen && _sourceList[_sourceChoice.Selected-1].Key==chosen,"Reordered picker changed selection");
            if(_sourceWorkplaceLinks.Count>0)
            {
                int id=_sourceWorkplaceLinks.Keys.First();await Focus($"site:{id}");w.Cottages.RemoveAll(c=>c.Id==id);
                await Press(Key.Enter);await Frames();Check(_selectedSite<0 && _surveyKeyboardSite<0,"Vanished workplace activated replacement");
            }
            w.Validate();saved=w.SaveJson();Check(World.LoadJson(saved).SaveJson()==saved,"Survey fixture save failed");
            AdoptWorld(World.LoadJson(saved));await Frames();Check(!_surveyKeyboard && !_surveying,"Reload retained survey focus");
        }
        var woods=World.NewCampaign(9);woods.Campaign!.Woods!.Phase=1;AdoptWorld(woods);_paused=true;await Frames();await Press(Key.G);
        string goal="wood-trees-"+woods.Map.Wildlife.First().Id;
        for(int i=0;i<60 && _goalsFocus!="why:"+goal;i++){await Press(Key.Tab);await Frames();}
        Check(_goalsFocus=="why:"+goal,"Woodland goal not reachable");await Press(Key.Enter);await Frames();
        string link="place:"+goal+":Inspect woodland and hunting lodges";
        for(int i=0;i<20 && _goalsFocus!=link;i++){await Press(Key.Tab);await Frames();}
        Check(_goalsFocus==link,"Goal survey link missing");await Press(Key.Enter);await Frames();
        Check(_surveyKeyboard && !_goalsKeyboard && _selectedSource?.Kind==SourceKind.Woodland,"Goals survey handoff failed");
        woods.Campaign!.Woods!.Phase=0; // Restore the isolated disclosure fixture before validating real simulation ticks.
        var recoveringHabitat=woods.Map.Wildlife.First();recoveringHabitat.Stock=0;_nextSourceRefresh=0;UpdateResourceSurvey();string prior=_sourceInfo.Text;
        for(int i=0;i<120;i++)woods.Tick(.1f);_nextSourceRefresh=0;UpdateResourceSurvey();await Frames();
        Check(_sourceInfo.Text!=prior && _sourceInfo.Text.Contains(woods.ReadResourceSurvey(new(SourceKind.Woodland,recoveringHabitat.Id))!.Detail),"Simulation stock change did not refresh evidence");
        woods.Validate();
        woods.Map.Wildlife.RemoveAll(h=>h.Id==recoveringHabitat.Id);_nextSourceRefresh=0;UpdateResourceSurvey();await Frames();
        Check(_selectedSource==null && _sourceChoice.Selected==0,"Removed source silently selected a replacement");
        var empty=World.NewScenario();AdoptWorld(empty);await Frames();string unchanged=empty.SaveJson();await Press(Key.U);await Frames();
        Check(_sourceChoice.Disabled && _surveyFocus=="finish" && _sourceInfo.Text.Contains("No fish grounds"),"Empty map focus failed");
        await Press(Key.Enter);Check(!_surveying && unchanged==empty.SaveJson(),"Empty survey exit mutated state");
        GD.Print("PASS: survey keyboard all source kinds, workplace/back, reordered picker, removed workplace, empty maps, 960/1440, Goals/Economy/Watch/typing handoffs and exact saves.");
    }
}

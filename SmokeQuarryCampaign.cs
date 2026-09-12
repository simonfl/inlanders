using Godot;
using Inlanders.Simulation;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async Task CheckQuarryCampaignUi()
    {
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        void Check(bool ok,string message){if(!ok)throw new Exception(message);}
        void Widths()
        {
            Check(_speedButton.GetGlobalRect().End.X<=GetWindow().Size.X-16 && _pauseButton.GetGlobalRect().End.X<=GetWindow().Size.X-16,"Quarry resource bar hides time controls");
            var page=_drawerPages[2];var bounds=page.GetGlobalRect();
            float right=bounds.End.X-(page.GetVScrollBar().Visible?page.GetVScrollBar().Size.X:0);
            void Visit(Node n)
            {
                if(n is Control c && c.IsVisibleInTree())Check(c.GetGlobalRect().End.X<=right+1 && c.GetGlobalRect().Position.X>=bounds.Position.X-1,$"Quarry Goals overflow: {c.Name}");
                foreach(var child in n.GetChildren())Visit(child);
            }
            Visit(page.GetChild(0));
        }
        _campaignPath=Path.Combine(Path.GetTempPath(),"inlanders-quarry-ui-"+Guid.NewGuid()+".json");_campaignBook=null;
        AdoptWorld(World.NewScenario());await OpenMenu(2);await UiClick(_levelButtons[7]);await Frames();
        Check(_world.IsQuarryCampaign && _world.Campaign!.Quarry!.Phase==0,"Campaign picker did not start level eight");
        foreach(int width in new[]{960,1440})
        {
            GetWindow().Size=new(width,width==960?640:900);
            AdoptWorld(World.NewCampaign(8));await OpenMenu(2);await Frames();Widths();
            Check(_levelButtons.Count==World.CampaignLevels.Length && _riverAction.Visible && _riverAction.Disabled,"Quarry entry/phase controls missing");
            Check(_kindButtons.Values.All(b=>!b.Disabled),"Quarry locks building types");
            await UiClick(_goalItems["hall-stone"].Toggle);await Frames();Widths();
            string saved=_world.SaveJson();
            var survey=_goalPlaces["hall-stone"].GetChildren().OfType<Button>().Last();
            await UiClick(survey);await Frames();
            Check(_selectedSource==new SourceKey(SourceKind.Stone,1) && _sourceInfo.Text.Contains("36"),"Distant source link failed");
            Check(_world.SaveJson()==saved,"Survey navigation mutated state");StopResourceSurvey();
            await OpenMenu(2);_drawerPages[2].ScrollVertical=0;await Frames();
            await Capture($"artifacts/quarry-opening-{width}.png");
            AdoptWorld(World.LoadFile("artifacts/quarry-prepared.json"));await OpenMenu(2);await Frames();
            Check(!_riverAction.Disabled,"Finished hall phase action disabled");
            await UiClick(_riverAction);await Frames();
            Check(_world.Campaign!.Quarry!.Phase==1 && !_riverAction.Visible && _goalMeals.Visible,"Assessment button did not advance/save the phase");
            AdoptWorld(World.LoadFile("artifacts/quarry-rough.json"));await OpenMenu(2);await Frames();Widths();
            Check(_goalItems.ContainsKey("hall-visits") && _goalMeals.Visible && !_goalMealEvidence.Text.Contains("25%"),"Hall service UI rules wrong");
            await UiClick(_goalItems["hall-visits"].Toggle);await Frames();Widths();saved=_world.SaveJson();
            await UiClick(_goalResidentButtons["hall-visits"]);await Frames();
            Check(_campaignServiceKey=="hall-visits" && _serviceResidents.Values.Count(r=>r.Row.Visible)==_world.Population,"Missing hall resident inspection");
            Check(_world.SaveJson()==saved,"Resident navigation mutated state");
            await OpenMenu(2);_drawerPages[2].ScrollVertical=0;await Frames();
            await Capture($"artifacts/quarry-assessment-{width}.png");
            AdoptWorld(World.LoadFile("artifacts/quarry-complete.json"));SaveCampaign();await OpenMenu(2);await Frames();
            Check(_nextLevel.Visible && _keepPlaying.Visible,"Quarry continuation controls wrong");
            string complete=_world.SaveJson();await UiClick(_replayLevel);await Frames();
            Check(!_world.Campaign!.Complete && _world.Campaign.Quarry!.Phase==0,"Quarry replay did not reset");
            await UiClick(_restoreReplay);await Frames();Check(_world.SaveJson()==complete,"Quarry replay restore differed");
            CloseManagementUi();_camera.Size=22;_focus=new(-1,0,0);UpdateCamera();await Frames();
            await Capture($"artifacts/quarry-complete-{width}.png");
        }
        GD.Print("PASS: quarry entry, source links, hall residents, food evidence, 960/1440 widths, completion/replay/restore and read-only navigation.");
    }
}

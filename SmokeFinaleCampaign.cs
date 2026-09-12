using Godot;
using Inlanders.Simulation;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async Task CheckFinaleCampaignUi()
    {
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        void Widths()
        {
            var page=_drawerPages[2];var bounds=page.GetGlobalRect();
            float right=bounds.End.X-(page.GetVScrollBar().Visible?page.GetVScrollBar().Size.X:0);
            void Visit(Node n)
            {
                if(n is Control c && c.IsVisibleInTree())Check(c.GetGlobalRect().End.X<=right+1 && c.GetGlobalRect().Position.X>=bounds.Position.X-1,$"Finale Goals overflow: {c.Name}");
                foreach(var child in n.GetChildren())Visit(child);
            }
            Visit(page.GetChild(0));
        }
        _campaignPath=Path.Combine(Path.GetTempPath(),"inlanders-finale-ui-"+Guid.NewGuid()+".json");_campaignBook=null;
        AdoptWorld(World.NewScenario());await OpenMenu(2);await UiClick(_levelButtons[9]);await Frames();
        Check(_world.IsFinaleCampaign && _drawerPages[2].ScrollVertical==0,"Picker did not start level ten at its objectives");
        foreach(int width in new[]{960,1440})
        {
            GetWindow().Size=new(width,width==960?640:900);
            AdoptWorld(World.NewCampaign(10));await OpenMenu(2);_drawerPages[2].ScrollVertical=0;await Frames();Widths();
            Check(_riverAction.Visible && _riverAction.Disabled && !_supperButton.Visible,"Opening gates wrong");
            Check(_kindButtons.Values.All(b=>!b.Disabled),"Finale locks buildings");
            await Capture($"artifacts/finale-campaign-opening-{width}.png");
            foreach(string step in new[]{"first","second"})
            {
                AdoptWorld(World.LoadFile($"artifacts/finale-campaign-prepared-{step}.json"));await OpenMenu(2);await Frames();
                Check(!_riverAction.Disabled,"Prepared assessment unavailable");await UiClick(_riverAction);await Frames();Widths();
                Check(_goalMeals.Visible && !_riverAction.Visible && !_supperButton.Visible,"Assessment UI failed");
                Check(!_goalMealEvidence.Text.Contains("25%"),"Finale adds unintended diet requirement");
                if(step=="second")
                {
                    string saved=_world.SaveJson();await UiClick(_goalItems["rest"].Toggle);await Frames();Widths();
                    await UiClick(_goalResidentButtons["rest"]);await Frames();Check(_tabs.CurrentTab==0,"Rest investigation failed");
                    Check(_world.SaveJson()==saved,"Rest investigation changes simulation");
                }
                await OpenMenu(2);_drawerPages[2].ScrollVertical=0;await Frames();await Capture($"artifacts/finale-campaign-{step}-{width}.png");
            }
            AdoptWorld(World.LoadFile("artifacts/finale-campaign-poor.json"));await OpenMenu(2);await Frames();Widths();
            Check(_supperButton.Visible && _supperButton.Disabled && _goalPhase.Text.Contains("earned"),"Poor allocation lost earned milestones");
            string poor=_world.SaveJson();await UiClick(_supperBreadLink);await Frames();
            Check(_breadDetails.Visible && _world.SaveJson()==poor,"Finale bread recovery link failed");
            AdoptWorld(World.LoadFile("artifacts/finale-campaign-ready-supper.json"));await OpenMenu(2);await Frames();Widths();
            Check(!_supperButton.Disabled && _supperButton.Text.Contains("44"),"Additional residents did not change supper control");
            await UiClick(_supperButton);await Frames();Check(_world.Food.Celebrating,"Finale supper action failed");
            await OpenMenu(2);await Frames();Check(_goalItems.ContainsKey("supper-gathering") && !_goalItems.ContainsKey("supper-bread"),"Gathering still asks for consumed bread");
            _paused=true;
            for(int i=0;i<2400 && !_world.Campaign!.Complete;i++){_world.Tick(.1f);if(i%10==0)await Frames();}
            _paused=true;Check(_world.Campaign!.Complete,"Rendered supper failed to complete");SaveCampaign();await OpenMenu(2);await Frames();
            Check(!_nextLevel.Visible && _keepPlaying.Visible,"Final level completion controls wrong");
            string complete=_world.SaveJson();await UiClick(_replayLevel);await Frames();
            Check(_world.Campaign!.Finale!.Phase==0 && !_world.Campaign.Complete,"Finale replay failed");
            await UiClick(_restoreReplay);await Frames();Check(_world.SaveJson()==complete,"Finale replay restore differs");
            await Capture($"artifacts/finale-campaign-complete-{width}.png");
        }
        GD.Print("PASS: level-ten picker, phase actions, all buildings, service/bread links, extra-resident supper, 960/1440 Goals and real completion/replay/restore.");
    }
}

using Godot;
using Inlanders.Simulation;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async Task CheckWoodsCampaignUi()
    {
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        void Check(bool ok,string message){if(!ok)throw new Exception(message);}
        void Widths()
        {
            Check(_speedButton.GetGlobalRect().End.X<=GetWindow().Size.X-16,"Woodland resource bar hides speed controls");
            var page=_drawerPages[2];var bounds=page.GetGlobalRect();
            float right=bounds.End.X-(page.GetVScrollBar().Visible?page.GetVScrollBar().Size.X:0);
            void Visit(Node n)
            {
                if(n is Control c && c.IsVisibleInTree())Check(c.GetGlobalRect().End.X<=right+1 && c.GetGlobalRect().Position.X>=bounds.Position.X-1,$"Woodland Goals overflow: {c.Name}");
                foreach(var child in n.GetChildren())Visit(child);
            }
            Visit(page.GetChild(0));
        }
        _campaignPath=Path.Combine(Path.GetTempPath(),"inlanders-woods-ui-"+Guid.NewGuid()+".json");_campaignBook=null;
        AdoptWorld(World.NewScenario());await OpenMenu(2);await UiClick(_levelButtons[8]);await Frames();
        Check(_world.IsWoodsCampaign && _world.Campaign!.Woods!.Phase==0,"Picker did not start Living woods");
        foreach(int width in new[]{960,1440})
        {
            GetWindow().Size=new(width,width==960?640:900);
            AdoptWorld(World.NewCampaign(9));await OpenMenu(2);await Frames();Widths();
            Check(_levelButtons.Count==World.CampaignLevels.Length && _riverAction.Visible && _riverAction.Disabled,"Opening controls missing");
            Check(_kindButtons.Values.All(b=>!b.Disabled),"Woodland locks building types");
            await UiClick(_goalItems["game"].Toggle);await Frames();Widths();
            string saved=_world.SaveJson();
            await UiClick(_goalPlaces["game"].GetChildren().OfType<Button>().Last());await Frames();
            Check(_selectedSource==new SourceKey(SourceKind.Woodland,1) && _sourceInfo.Text.Contains("mature trees"),"East wood source link failed");
            Check(_world.SaveJson()==saved,"Source inspection mutated state");StopResourceSurvey();
            await OpenMenu(2);_drawerPages[2].ScrollVertical=0;await Frames();await Capture($"artifacts/woods-opening-{width}.png");
            AdoptWorld(World.LoadFile("artifacts/woods-catch.json"));await OpenMenu(2);await Frames();
            await UiClick(_riverAction);await Frames();
            Check(_world.Campaign!.Woods!.Phase==1 && _goalItems.ContainsKey("wood-trees-0"),"First catch phase action failed");
            AdoptWorld(World.LoadFile("artifacts/woods-prepared.json"));await OpenMenu(2);await Frames();Widths();
            Check(!_riverAction.Disabled,"Prepared village assessment disabled");
            await UiClick(_riverAction);await Frames();
            Check(_world.Campaign!.Woods!.Phase==2 && !_riverAction.Visible && _goalMeals.Visible,"Assessment action failed");
            Check(!_goalMealEvidence.Text.Contains("25%"),"Woodland imposes unintended diet mix");
            foreach(string fixture in new[]{"depleted","cleared","restoring"})
            {
                AdoptWorld(World.LoadFile($"artifacts/woods-{fixture}.json"));await OpenMenu(2);await Frames();Widths();
                string key=fixture=="depleted"?"wood-stock-0":"wood-trees-0";
                await UiClick(_goalItems[key].Toggle);await Frames();Widths();
                Check(_goalItems[key].Help.Text.Contains(fixture=="depleted"?"pause hunting":"preserve"),"Recovery explanation wrong");
                saved=_world.SaveJson();
                await UiClick(_goalPlaces[key].GetChildren().OfType<Button>().First());await Frames();
                Check(_selectedSource==new SourceKey(SourceKind.Woodland,0),"West recovery link failed");
                Check(_world.SaveJson()==saved,"Recovery navigation mutated state");
                await Capture($"artifacts/woods-{fixture}-survey-{width}.png");StopResourceSurvey();
                await OpenMenu(2);_drawerPages[2].ScrollVertical=0;await Frames();
                await Capture($"artifacts/woods-{fixture}-goals-{width}.png");
                if(fixture=="restoring")
                {
                    await UiClick(_goalPlaces[key].GetChildren().OfType<Button>().ElementAt(1));await Frames();
                    Check(_placing && _woodlandTool==1 && _world.SaveJson()==saved,"Preserve recovery tool link failed");
                    await Press(Key.Escape);
                    await OpenMenu(2);await Frames();
                    await UiClick(_goalPlaces[key].GetChildren().OfType<Button>().ElementAt(2));await Frames();
                    Check(_placing && _plantingTrees && _world.SaveJson()==saved,"Plant recovery tool link failed");
                    await Press(Key.Escape);await OpenMenu(2);await Frames();
                    await UiClick(_goalPlaces[key].GetChildren().OfType<Button>().ElementAt(3));await Frames();
                    Check(_placing && _clearingTrees && _world.SaveJson()==saved,"Root clearing tool link failed");
                    await Press(Key.Escape);
                }
            }
            AdoptWorld(World.LoadFile("artifacts/woods-complete.json"));SaveCampaign();await OpenMenu(2);await Frames();
            Check(_nextLevel.Visible && _keepPlaying.Visible,"Woodland next settlement controls wrong");
            string complete=_world.SaveJson();await UiClick(_replayLevel);await Frames();
            Check(!_world.Campaign!.Complete && _world.Campaign.Woods!.Phase==0,"Woodland replay failed");
            await UiClick(_restoreReplay);await Frames();Check(_world.SaveJson()==complete,"Woodland replay restore differs");
            CloseManagementUi();_camera.Size=25;_focus=new(2,0,-1);UpdateCamera();await Frames();
            await Capture($"artifacts/woods-complete-{width}.png");
            await OpenMenu(2);await UiClick(_nextLevel);await Frames();
            Check(_world.IsFinaleCampaign,"Woodland next-settlement action did not reach finale");
        }
        GD.Print("PASS: woodland picker, phase action, habitat recovery links, food evidence, 960/1440 widths and completion/replay/restore.");
    }
}

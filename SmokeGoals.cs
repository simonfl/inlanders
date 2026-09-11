using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async Task CheckGoalDashboard()
    {
        async Task Frames(){for(int i=0;i<4;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        foreach(int width in new[]{960,1440})
        foreach(int level in new[]{6,7})
        {
            GetWindow().Size=new(width,width==960?640:900);AdoptWorld(World.NewCampaign(level));
            if(level==6)_world.Campaign!.River!.Phase=2;else _world.Campaign!.Lake!.Phase=1;
            await OpenMenu(2);await Frames();string saved=_world.SaveJson();
            Check(_goalDashboard.Visible && !_objective.Visible && !_goalArrival.Visible,"Long objective still displaces condition cards");
            Check(_riverAction.IsVisibleInTree() && _riverAction.GetGlobalRect().End.Y<GetWindow().Size.Y-80,"Phase action is not near top");
            string key=level==6?"east-recreation":"rest";
            await UiClick(_goalItems[key].Toggle);await Frames();Check(_goalItems[key].Help.Visible,"Condition help failed");
            await UiClick(_goalResidentButtons[key]);await Frames();
            Check(_serviceFilter.Selected==5 && _campaignServiceKey==key,"Goal resident filter failed");
            Check(_serviceResidents.Values.Count(r=>r.Row.Visible)==_world.People.Count(p=>!_world.ResidentMeetsCampaignCondition(p,key)),"Goal resident count disagrees");
            _serviceFilter.Select(6);UpdateServiceCoverage();await Frames();
            Check(_serviceResidents.Values.Count(r=>r.Row.Visible)==_world.People.Count(p=>_world.ResidentMeetsCampaignCondition(p,key)),"Counted resident filter disagrees");
            await OpenMenu(2);await Frames();
            Check(_goalPlaces[key].Visible,"Goal places hidden with help open");
            string placementState=_world.SaveJson();
            var planButton=_goalPlaces[key].GetChildren().OfType<Button>().Last();
            await UiClick(planButton);await Frames();Check(_placing && _world.SaveJson()==placementState,"Planning a goal building mutated village");
            await Press(Key.Escape);await OpenMenu(2);await Frames();
            await UiClick(_goalItems[key].Toggle);await Frames();Check(!_goalItems[key].Help.Visible,"Condition help did not collapse");
            Check(_world.SaveJson()==saved,"Goal navigation changed village");
            _drawerPages[2].ScrollVertical=0;await Frames();await Capture($"artifacts/goals-{level}-{width}.png");
            if(level==6)_world.Campaign!.River!.Phase=3;else _world.Campaign!.Lake!.Phase=2;
            await Frames();saved=_world.SaveJson();
            Check(_goalMeals.Visible && _goalMealEvidence.Text.Contains("Fresh deliveries"),"Food assessment evidence missing");
            await UiClick(_goalMealExplain);await Frames();Check(_goalMealHelp.Visible,"Meal explanation failed");
            await UiClick(_goalMealPeople);await Frames();Check(_serviceFilter.Selected==4,"Meal inspection link failed");
            await OpenMenu(2);await Frames();await UiClick(_goalMealEconomy);await Frames();Check(_tabs.CurrentTab==4,"Economy link failed");
            Check(_world.SaveJson()==saved,"Meal assessment navigation changed world");
            await OpenMenu(2);await Frames();
        }
        AdoptWorld(World.NewCreative());await Frames();Check(!_goalDashboard.Visible,"Campaign cards leaked into Creative");
        GD.Print("PASS: compact river/lake goals, top phase action, collapsible explanations, exact read-only navigation and 960/1440 layouts.");
    }
}

using Godot;
using System;
using System.Linq;

public partial class Game
{
    private VBoxContainer _goalMeals=null!;
    private Label _goalMealStatus=null!,_goalMealEvidence=null!,_goalMealHelp=null!;
    private Button _goalMealExplain=null!,_goalMealPeople=null!,_goalMealEconomy=null!;
    private void MakeGoalMeals()
    {
        _goalMeals=new();_goalDashboard.AddChild(_goalMeals);
        _goalMealStatus=Text("",14,true);_goalMeals.AddChild(_goalMealStatus);
        _goalMealEvidence=Text("",13,true);_goalMeals.AddChild(_goalMealEvidence);
        _goalMealHelp=Text("The assessment uses a moving three-minute window starting when you begin it. Everyone needs two closed meal requests, no missed/skipped requests, at least a quarter of eaten portions outside the dominant food, and fresh deliveries covering eating and demand. Late eating restores nourishment but does not erase a missed request; let it age out while improving service. This is not three synchronized village meals or a stockpile target.",13,true);_goalMealHelp.Hide();
        _goalMealExplain=Button("How food assessment works",()=>_goalMealHelp.Visible=!_goalMealHelp.Visible);_goalMeals.AddChild(_goalMealExplain);_goalMeals.AddChild(_goalMealHelp);
        _goalMealPeople=Button("Inspect meal trips",OpenMealCoverage);_goalMeals.AddChild(_goalMealPeople);
        _goalMealPeople.TooltipText="People shows current hunger and missed/skipped meals in the last three minutes; newly started assessments may cover a shorter window.";
        _goalMealEconomy=Button("Open food economy",()=>{if(!_drawer.Visible || _tabs.CurrentTab!=4)ToggleDrawer(4);});_goalMeals.AddChild(_goalMealEconomy);
    }
    private void UpdateGoalMeals()
    {
        bool river=_world.IsRiverCampaign;
        int phase=river?_world.Campaign!.River!.Phase:_world.Campaign!.Lake!.Phase;
        bool assessing=river?phase is 1 or 3:phase==2;
        _goalMeals.Visible=assessing;if(!assessing)return;
        bool proved=river && phase==1 && _world.Campaign!.River!.Meals>=2;
        if(proved){_goalMealStatus.Text="✓ First food assessment proved. This milestone is kept.";_goalMealEvidence.Text="Prepare the next expansion when ready.";return;}
        float since=river?_world.Campaign!.River!.AssessmentStarted:_world.Campaign!.Lake!.AssessmentStarted;
        var food=_world.ReadMealAssessment(since);
        string Mark(bool ok)=>ok?"✓":"○";
        _goalMealStatus.Text=$"FOOD ASSESSMENT · last {Math.Min(180,(int)(_world.Food.Time-since))}s / up to 3m";
        _goalMealEvidence.Text=$"{Mark(food.ResidentsWithHistory==food.Residents)} Two closed requests: {food.ResidentsWithHistory}/{food.Residents} residents\n{Mark(food.Missed==0 && food.Skipped==0)} Missed / skipped: {food.Missed} / {food.Skipped}\n{Mark(food.Varied)} Other food portions: {food.NonDominant}/{food.Eaten} eaten (need 25%)\n{Mark(food.FreshSupply)} Fresh deliveries: {food.Delivered}/{Math.Max(food.Closed+food.Skipped,food.Eaten)} needed";
        if(_world.ReadCampaignConditions().All(c=>c.Met)) _goalNext.Text=food.Problem??"Meal evidence ready; keep the village running for the assessment check.";
    }
}

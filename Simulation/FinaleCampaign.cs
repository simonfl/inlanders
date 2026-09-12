using System;
using System.Linq;

namespace Inlanders.Simulation;

public sealed class FinaleProgress
{
    public int Phase { get; set; }
    public float AssessmentStarted { get; set; }
}
public sealed partial class World
{
    private System.Collections.Generic.IReadOnlyList<CampaignCondition> ReadFinaleConditions()
    {
        int phase=Campaign!.Finale!.Phase;
        var rows=new System.Collections.Generic.List<CampaignCondition>
        {
            new("population","Residents",Population,phase<2?12:20,"Invite pairs from People after preparing homes and food. All buildings remain available. After assessment milestones are earned they stay kept; later arrivals increase current housing and supper needs."),
            new("housing","Residents housed",Housed,Population,"Every current resident needs a completed home. Central homes can be moved by building replacements and demolishing the old homes; builders physically recover their materials.")
        };
        if(phase is 2 or 3)
        {
            rows.Add(new("rest","Residents recently rested",LakeRested,(Population*3+3)/4,"Three quarters need completed home rest, lasting four minutes or five after an improved visit. Inspect homes and everyday trips."));
            rows.Add(new("recreation","Recreation visits · 2m",LakeRecreation,(Population+1)/2,"Half need a completed visit within two minutes. Squares, gardens and halls count. Consider central services or local services near eastern homes."));
        }
        if(phase==4 && Food.Celebrating)
            rows.Add(new("supper-gathering","Residents at supper",People.Count(p=>p.Task==Work.Supper),Population,"Everyone must arrive, then share six seconds together. Food has already been set aside."));
        else if(phase==4)
            rows.Add(new("supper-bread","Bread available centrally",CentralFoodAvailable(Resource.Bread),SupperCost,"Both assessments stay earned. Supper requires two unreserved central loaves per current resident, homes for everyone and clear gathering space. Meals keep using bread. Inspect bread supply below to compare deliveries, eating, local stock and bakery work."));
        return rows;
    }
    public bool IsFinaleCampaign => Campaign?.Level==10;
    public string FinaleActionLabel => Campaign?.Finale?.Phase==0?"Assess the first neighborhood":"Assess the expanded village";
    private string? FinaleHomesProblem(int minimum) => Population<minimum?$"Prepare homes and food, then invite pairs from People to reach {minimum} residents.":
        Housed<Population?"Finish homes for everyone, including extra residents.":null;
    public string? FinaleActionProblem() => !IsFinaleCampaign?"No lasting-village campaign is active.":Campaign!.Finale!.Phase switch
    {
        0=>FinaleHomesProblem(12),
        2=>FinaleHomesProblem(20),
        _=>"Keep the current phase working; earned milestones are preserved."
    };
    public bool AdvanceFinalePhase()
    {
        if(FinaleActionProblem()!=null)return false;
        Campaign!.Finale!.Phase++;Campaign.Finale.AssessmentStarted=Food.Time;return true;
    }
    public string? FinaleServiceProblem()
    {
        bool expanded=Campaign!.Finale!.Phase>=2;
        string? homes=FinaleHomesProblem(expanded?20:12);if(homes!=null)return homes;
        var food=ReadMealAssessment(Campaign.Finale.AssessmentStarted);
        if(!food.Reliable)return food.Missed>0 || food.Skipped>0?"Restore food work or shorten meal trips. Missed requests must leave the three-minute assessment window.":"Keep food working until everyone has two closed meal requests since this assessment began.";
        if(!food.FreshSupply)return "Fresh deliveries must cover eating and meal demand. Stored reserves alone do not prove supply.";
        if(expanded && LakeRested<(Population*3+3)/4)return "Three quarters of residents need a recent completed home rest. Inspect homes and travel.";
        if(expanded && LakeRecreation<(Population+1)/2)return "Half the residents need a completed recreation visit within two minutes. Squares, gardens and halls count; inspect their access and travel.";
        return null;
    }
    public string? FinaleSupperProblem() => !IsFinaleCampaign?null:Campaign!.Finale!.Phase<4?"Earn both neighborhood assessments before hosting the finale supper.":
        FinaleHomesProblem(20);
    private void RecordFinaleProgress()
    {
        if(!IsFinaleCampaign)return;
        var progress=Campaign!.Finale!;
        if(progress.Phase is 1 or 3 && FinaleServiceProblem()==null)progress.Phase++;
        if(progress.Phase==4 && Food.SupperComplete){progress.Phase=5;Campaign.Complete=true;}
    }
    private void ValidateFinaleCampaign()
    {
        var finale=Campaign?.Finale;
        if(IsFinaleCampaign!=(finale!=null))throw new InvalidOperationException("Missing or misplaced finale state");
        if(finale!=null && (finale.Phase is <0 or >5 || !float.IsFinite(finale.AssessmentStarted) || finale.AssessmentStarted<0 || finale.AssessmentStarted>Food.Time ||
            finale.Phase==0 && finale.AssessmentStarted!=0 || Campaign!.Complete!=(finale.Phase==5) ||
            finale.Phase<4 && (Food.Celebrating || Food.SupperComplete) || finale.Phase==5 && !Food.SupperComplete))
            throw new InvalidOperationException("Invalid finale progress");
    }
    private double FinaleCompletion => Campaign!.Finale!.Phase/5d;
    public string FinaleObjective => Campaign!.Finale!.Phase switch
    {
        0=>"Support twelve housed residents, then twenty. Central land can hold homes or production; the eastern bank has more space. Finish with a shared supper. All buildings remain available.",
        1=>FinaleServiceProblem()??"The first neighborhood is supported.",
        2=>"First neighborhood earned. Expand to twenty with continuing meals, recent home rest and recreation. Relocate central homes or serve the east locally.",
        3=>FinaleServiceProblem()??"The expanded village is supported.",
        4=>$"Both assessments earned. Prepare {SupperCost} unreserved central bread for {Population} residents, keep everyone housed and leave gathering space. Ordinary meals still use bread. Extra residents increase the supper cost; earned assessments stay kept.",
        _=>"A lasting village. The neighborhoods are established and everyone has shared supper. Keep enjoying the village."
    };
}

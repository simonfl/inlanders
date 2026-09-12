using System;
using System.Linq;

namespace Inlanders.Simulation;

public sealed class QuarryProgress
{
    public int Phase { get; set; }
    public float AssessmentStarted { get; set; }
}
public sealed partial class World
{
    public bool IsQuarryCampaign => Campaign?.Level==8;
    public int RecentHallVisitors => People.Count(p=>ResidentMeetsCampaignCondition(p,"hall-visits"));
    public Cottage? CampaignHall => Cottages.Where(c=>c.Kind==BuildingKind.GatheringHall && !c.DemolitionRequested)
        .OrderByDescending(c=>c.Complete).ThenByDescending(c=>c.Delivered+c.DeliveredStone).ThenBy(c=>c.Id).FirstOrDefault();
    public string? QuarryServiceProblem()
    {
        if(CampaignHall?.Complete!=true)return "Finish an available gathering hall. Plans and halls being dismantled do not count.";
        if(Housed<Population)return "Finish homes for every resident.";
        if(RecentHallVisitors<(Population+1)/2)return $"{RecentHallVisitors}/{(Population+1)/2} residents recently used a gathering hall. Their latest completed break must be at an available hall within four in-game minutes. Keep it near work or homes; a closer square or garden may attract visits instead.";
        var food=ReadMealAssessment(Campaign!.Quarry!.AssessmentStarted);
        if(!food.Reliable)return food.Missed>0 || food.Skipped>0 ? "Meals were missed or skipped. Restore food work or shorten meal trips; the last three minutes must recover without misses." : "Waiting for two closed meal requests per resident since assessment began.";
        return food.FreshSupply?null:"Fresh deliveries must cover actual eating and closed or skipped meal demand. Keep food producers working.";
    }
    public string? QuarryActionProblem()=>!IsQuarryCampaign?"No quarry campaign is active.":Campaign!.Quarry!.Phase!=0?"Assessment is underway. Keep the hall and food service working.":CampaignHall?.Complete==true?null:"Build a gathering hall: 8 planks and 12 stone. Nearby stone supplies only 8; the distant outcrop can supply the whole project.";
    public bool AdvanceQuarryPhase()
    {
        if(QuarryActionProblem()!=null)return false;
        Campaign!.Quarry!.Phase=1;Campaign.Quarry.AssessmentStarted=Food.Time;return true;
    }
    private void RecordQuarryMeal()
    {
        if(IsQuarryCampaign && Campaign!.Quarry!.Phase==1 && QuarryServiceProblem()==null)
        {Campaign.Quarry.Phase=2;Campaign.Complete=true;}
    }
    private void ValidateQuarryCampaign()
    {
        var q=Campaign?.Quarry;
        if(IsQuarryCampaign!=(q!=null))throw new InvalidOperationException("Missing or misplaced quarry campaign state");
        if(q!=null && (q.Phase is <0 or >2 || !float.IsFinite(q.AssessmentStarted) || q.AssessmentStarted<0 || q.AssessmentStarted>Food.Time || q.Phase==0 && q.AssessmentStarted!=0 || Campaign!.Complete!=(q.Phase==2)))
            throw new InvalidOperationException("Invalid quarry assessment state");
    }
    private double QuarryCompletion=>Campaign!.Quarry!.Phase==2?1:Campaign.Quarry.Phase==1?.5:CampaignHall?.Complete==true?.4:0;
    public string QuarryObjective=>Campaign!.Quarry!.Phase==0?"Build a gathering hall with 8 planks and 12 stone. Nearby stone holds 8; use two camps for shorter hauling or only the distant camp to save infrastructure. Keep food workers supplied.":$"Hall visits within 4m: {RecentHallVisitors}/{(Population+1)/2}\nHoused: {Housed}/{Population}\n{QuarryServiceProblem()??"Hall and food service are working."}";
}

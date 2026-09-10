using System;
using System.Linq;

namespace Inlanders.Simulation;

public sealed class LakeProgress
{
    public int Phase { get; set; }
    public int Meals { get; set; }
    public int DeliveredBaseline { get; set; }
    public int Required { get; set; }
    public string LastResult { get; set; } = "";
}

public sealed partial class World
{
    public bool IsLakeCampaign => Campaign?.Level==7;
    public int LakeRested => People.Count(RecentlyRested);
    public int LakeRecreation => People.Count(p=>p.LastLeisureTime is float t && Food.Time-t<120);
    private string? LakeVillageProblem()
    {
        if(Population<12) return "Prepare food and spare beds, then invite newcomers from People to reach twelve residents.";
        if(Housed<Population) return "Finish homes for everyone, including extra newcomers.";
        if(LakeRested<(Population*3+3)/4) return $"At least {(Population*3+3)/4} residents need a completed home rest in the last four minutes; currently {LakeRested}. Inspect home access and travel.";
        if(LakeRecreation<(Population+1)/2) return $"At least {(Population+1)/2} residents need a completed square visit in the last two minutes; currently {LakeRecreation}. Leave space and time for breaks.";
        return null;
    }
    public string LakeActionLabel => Campaign?.Lake?.Phase==0 ? "Prepare the lakeside village" : "Assess the lakeside village";
    public string? LakeActionProblem() => !IsLakeCampaign || Campaign!.Lake==null ? "This settlement has no lake assessment." :
        Campaign.Lake.Phase==0 ? DeliveredFish<4 ? "Bring the first four fish all the way to the pantry. Catches still aboard do not count." : null :
        Campaign.Lake.Phase==1 ? LakeVillageProblem() : "Assessment is underway: serve three full mixed meals with fresh deliveries covering consumption.";
    public bool AdvanceLakePhase()
    {
        if(LakeActionProblem()!=null) return false;
        var lake=Campaign!.Lake!; lake.Phase++; lake.Meals=0; lake.Required=0; lake.DeliveredBaseline=DeliveredEdible;
        lake.LastResult=lake.Phase==1 ? "First catch delivered. Prepare homes, food and places to meet before growing to twelve." : "Assessment started. Keep meals, rest and recreation working together.";
        return true;
    }
    private void RecordLakeMeal()
    {
        if(!IsLakeCampaign || Campaign!.Lake is not { Phase:2 } lake) return;
        lake.Required+=Population;
        string? problem=LakeVillageProblem();
        problem ??= Food.LastMealServed<Population ? "A meal did not feed everyone." :
            Food.LastMealNonDominant<(Population+3)/4 ? "At least a quarter of meal portions must be outside the dominant food." :
            DeliveredEdible-lake.DeliveredBaseline<lake.Required ? $"Fresh pantry deliveries: {DeliveredEdible-lake.DeliveredBaseline}/{lake.Required} consumed portions. Check Economy: add food capacity or shorten workers' trips, including trips to homes and squares." : null;
        if(problem!=null)
        {
            lake.Meals=0; lake.Required=0; lake.DeliveredBaseline=DeliveredEdible;
            lake.LastResult=problem+" Adjust the village; the next meal can start a new streak."; return;
        }
        lake.Meals++; lake.LastResult="A full mixed meal, fresh supply, home rest and recreation are all working.";
        if(lake.Meals==3) { lake.Phase=3; Campaign.Complete=true; }
    }
    private double LakeCompletion => Campaign!.Lake!.Phase>=3 ? 1 : Campaign.Lake.Phase/3d+
        (Campaign.Lake.Phase==2 ? Campaign.Lake.Meals/3d : LakeActionProblem()==null ? 1 : 0)/3d;
    public string LakeObjective
    {
        get
        {
            var lake=Campaign!.Lake!;
            if(lake.Phase==0) return $"Open a fishing route\n\nFish delivered to pantry: {Math.Min(4,DeliveredFish)}/4\nBuild a dock on accessible shore and assign a fisher. Grounds replenish {Map.FishingGrounds.Sum(g=>g.RegrowthPerSecond)*60:0.#} fish/minute in total, shared by all docks. Delivered supply can be lower because of travel.\n\nNext: support twelve residents with mixed meals, home rest and square visits. Every resident eats once a minute. Plan cultivation and reserve useful space near the village before expanding.";
            return $"{(lake.Phase==1?"Prepare a lakeside community":"Prove the village works")}\n\nResidents: {Population}/12 minimum\nHoused: {Housed}/{Population}\nRested in last 4 minutes: {LakeRested}/{(Population*3+3)/4}\nSquare visit in last 2 minutes: {LakeRecreation}/{(Population+1)/2}\n\n"+
                (lake.Phase==1 ? "Prepare production before each newcomer pair. Fish, berries, vegetables and bread can share the work. Begin assessment when ready." :
                $"Full mixed meals: {lake.Meals}/3\nAt least {(Population+3)/4} portions outside the dominant food per meal.\nFresh deliveries: {DeliveredEdible-lake.DeliveredBaseline}; consumed portions: {lake.Required}.\n{lake.LastResult}");
        }
    }
    private void ValidateLakeCampaign()
    {
        var lake=Campaign?.Lake;
        if(IsLakeCampaign!=(lake!=null)) throw new InvalidOperationException("Missing or misplaced lake campaign state");
        if(lake!=null && (lake.Phase is <0 or >3 || lake.Meals is <0 or >3 || lake.Required<0 || lake.DeliveredBaseline<0 || lake.DeliveredBaseline>DeliveredEdible ||
            lake.LastResult==null || Campaign!.Complete!=(lake.Phase==3) || lake.Phase==3 && lake.Meals!=3 || lake.Phase<2 && lake.Meals!=0))
            throw new InvalidOperationException("Invalid lake assessment state");
    }
}

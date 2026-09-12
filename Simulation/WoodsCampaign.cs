using System;
using System.Linq;

namespace Inlanders.Simulation;

public sealed class WoodsProgress
{
    public int Phase { get; set; }
    public float AssessmentStarted { get; set; }
}
public sealed partial class World
{
    public bool IsWoodsCampaign => Campaign?.Level==9;
    public int DeliveredGame => DeliveredFood(Resource.Game);
    public string WoodsName(WoodlandHabitat h) => IsWoodsCampaign ? h.Id==0?"West wood":"East wood" : $"Woodland {h.Id}";
    public string? WoodsHabitatProblem()
    {
        foreach(var h in Map.Wildlife)
            if(HabitatTrees(h)<4)return $"{WoodsName(h)} has {HabitatTrees(h)}/4 mature trees. Collect felled timber, clear roots, plant replacements and preserve those planting orders. Saplings take three in-game minutes after planting to mature; unprotected trees may be harvested again. Waiting alone cannot restore cleared habitat.";
        foreach(var h in Map.Wildlife)
            if(AvailableGame(h)<2)return $"{WoodsName(h)} has {AvailableGame(h)}/2 unclaimed game. Pause hunting lodges to let stock recover; existing outings still hold their catch. Trees support +{HabitatRecovery(h):0.#} game/minute here. Keep other food working.";
        return null;
    }
    private string? WoodsVillageProblem() => Population<12?"Prepare spare homes and food, then invite newcomer pairs from People to reach twelve residents.":
        Housed<Population?"Finish homes for everyone, including extra newcomers.":WoodsHabitatProblem();
    public string? WoodsServiceProblem()
    {
        var problem=WoodsVillageProblem();if(problem!=null)return problem;
        var food=ReadMealAssessment(Campaign!.Woods!.AssessmentStarted);
        if(!food.Reliable)return food.Missed>0 || food.Skipped>0?"Meals were missed or skipped. Restore food work or shorten meal trips; service recovers as misses leave the three-minute window.":"Waiting for two closed meal requests per resident since assessment began. Keep food workers supplied.";
        return food.FreshSupply?null:"Fresh food deliveries must cover eating and closed or skipped meal demand. Keep production working; stored reserves alone do not prove supply.";
    }
    public string WoodsActionLabel => Campaign?.Woods?.Phase==0?"Prepare the woodland village":"Assess the woodland village";
    public string? WoodsActionProblem() => !IsWoodsCampaign?"No woodland campaign is active.":Campaign!.Woods!.Phase switch
    {
        0=>DeliveredGame<4?"Deliver four game to food storage. Assign a hunter to a finished lodge near either wood. Carried catches do not count.":null,
        1=>WoodsVillageProblem(),
        _=>"Assessment is underway. Keep the village fed and both woods supported."
    };
    public bool AdvanceWoodsPhase()
    {
        if(WoodsActionProblem()!=null)return false;
        var woods=Campaign!.Woods!;woods.Phase++;
        if(woods.Phase==2)woods.AssessmentStarted=Food.Time;
        return true;
    }
    private void RecordWoodsMeal()
    {
        if(IsWoodsCampaign && Campaign!.Woods!.Phase==2 && WoodsServiceProblem()==null)
        {Campaign.Woods.Phase=3;Campaign.Complete=true;}
    }
    private void ValidateWoodsCampaign()
    {
        var woods=Campaign?.Woods;
        if(IsWoodsCampaign!=(woods!=null))throw new InvalidOperationException("Missing or misplaced woodland campaign state");
        if(woods!=null && (woods.Phase is <0 or >3 || !float.IsFinite(woods.AssessmentStarted) || woods.AssessmentStarted<0 || woods.AssessmentStarted>Food.Time ||
            woods.Phase<2 && woods.AssessmentStarted!=0 || Campaign!.Complete!=(woods.Phase==3) || woods.Phase>0 && DeliveredGame<4 ||
            Map.Wildlife.Count!=2 || !Map.Wildlife.Select(h=>h.Id).Order().SequenceEqual(new[]{0,1})))
            throw new InvalidOperationException("Invalid woodland assessment state");
    }
    private double WoodsCompletion => Campaign!.Woods!.Phase==3?1:Campaign.Woods.Phase/3d+(Campaign.Woods.Phase<2 && WoodsActionProblem()==null?1d/6:0);
    public string WoodsObjective => Campaign!.Woods!.Phase==0?$"Game delivered: {Math.Min(4,DeliveredGame)}/4. Preserve habitat or selectively harvest for cultivation. Next: twelve housed residents, four mature trees and two unclaimed game in each wood, plus continuing meals.":
        $"Residents: {Population}/12 minimum · Housed: {Housed}/{Population}\n"+string.Join("\n",Map.Wildlife.Select(h=>$"{WoodsName(h)}: {HabitatTrees(h)}/4 mature trees · {AvailableGame(h)}/2 unclaimed game"))+"\n"+(Campaign.Woods.Phase==1?WoodsActionProblem()??"Begin assessment when ready.":WoodsServiceProblem()??"Village and woodland are supported.");
}

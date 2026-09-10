using System;
using System.Linq;

namespace Inlanders.Simulation;

public sealed class RiverProgress
{
    public float AssessmentStarted { get; set; }
    public int Phase { get; set; }
    public int Meals { get; set; }
    public int DeliveredBaseline { get; set; }
    public int Required { get; set; }
    public string LastResult { get; set; } = "";
}

public sealed partial class World
{
    public bool IsRiverCampaign => Campaign?.Level == 6;
    public int EastBankBeds => Cottages.Where(c=>c.Complete && !c.DemolitionRequested && c.Cell.X>5).Sum(c=>Buildings.Get(c.Kind).Beds);
    public int EastBankRecreation => People.Count(p=>p.LastLeisureTime is float last && Food.Time-last<120 && Cottages.Any(c=>c.Id==p.LastLeisureSiteId && c.Kind==BuildingKind.Square && c.Cell.X>5 && c.Complete && !c.DemolitionRequested));
    public int DeliveredFish => DeliveredFood(Resource.Fish);
    private int DeliveredEdible => EdibleKinds.Sum(DeliveredFood);
    public string? RiverPreparationProblem(bool final)
    {
        if(EastBankBeds<(final?8:4)) return $"Finish {(final?8:4)} beds on the east bank, across the river.";
        if(Population<(final?16:12)) return $"Invite newcomers from People to reach {(final?16:12)} residents; prepare spare beds and food before each pair.";
        if(Housed<Population) return "Finish homes for everyone, including any extra newcomers.";
        if(final && !Cottages.Any(c=>c.Cell.X>5 && c.Kind==BuildingKind.Square && c.Complete && !c.DemolitionRequested)) return "Finish an east-bank square for the expanded village.";
        if(final && EastBankRecreation<(Population+1)/2) return $"At least {(Population+1)/2} residents need a completed east-bank square visit within the last two minutes; currently {EastBankRecreation}.";
        return null;
    }
    public string RiverActionLabel => Campaign?.River?.Phase switch { 0=>"Assess first neighborhood",1=>"Begin final expansion",2=>"Assess the expanded village",_=>"Assessment in progress" };
    public string? RiverActionProblem()
    {
        if(!IsRiverCampaign || Campaign!.River is not { } river) return "This settlement has no staged assessment.";
        if(river.Phase>=3) return "Maintain reliable mixed meals with fresh deliveries covering demand and consumption.";
        if(RiverPreparationProblem(river.Phase>=2) is string problem) return problem;
        return river.Phase==1 && river.Meals<2 ? "Prove reliable meal service, actual variety and fresh supply for the current residents." : null;
    }
    public bool AdvanceRiverPhase()
    {
        if(RiverActionProblem()!=null) return false;
        var river=Campaign!.River!; river.Phase++; river.Meals=0; river.Required=0; river.DeliveredBaseline=DeliveredEdible;
        river.AssessmentStarted=Food.Time;
        river.LastResult=river.Phase==2 ? "First neighborhood proven. Prepare the final expansion at your own pace." : "Assessment started. Build reliable closed meal requests for everyone; at least a quarter of food actually eaten must be outside the dominant food.";
        return true;
    }
    private void RecordRiverMeal()
    {
        if(!IsRiverCampaign || Campaign!.River is not { Phase: 1 or 3 } river) return;
        if(river.Phase==1 && river.Meals>=2) return;
        var service=ReadMealAssessment(river.AssessmentStarted);
        river.Required=Math.Max(service.Closed+service.Skipped,service.Eaten);
        string? problem=RiverPreparationProblem(river.Phase==3);
        problem ??= service.Problem;
        if(problem!=null)
        {
            river.Meals=0;
            river.LastResult=problem+" Improve the village; the rolling window recovers as service improves."; return;
        }
        river.Meals=river.Phase==1?2:3; river.LastResult="Reliable meal service, actual variety and fresh supply demonstrated.";
        if(river.Phase==3 && river.Meals>=3) { river.Phase=4; Campaign.Complete=true; }
    }
    public string RiverObjective
    {
        get
        {
            var river=Campaign!.River!; bool final=river.Phase>=2;
            string title=river.Phase switch { 0=>"First neighborhood",1=>"Prove the first expansion",2=>"Village on both banks",3=>"Prove the final village",_=>"A village on both banks" };
            string needs=$"Residents: {Population}/{(final?16:12)} minimum\nHoused: {Housed}/{Population}\nEast-bank beds: {EastBankBeds}/{(final?8:4)}";
            if(final) needs+="\nEast-bank square: "+(Cottages.Any(c=>c.Cell.X>5 && c.Kind==BuildingKind.Square && c.Complete && !c.DemolitionRequested)?"Ready":"Needed");
            if(final) needs+=$"\nRecent east-bank recreation: {EastBankRecreation}/{(Population+1)/2} residents (last 2 minutes)";
            string assessment=river.Phase is 1 or 3 ? $"\n\nMeal service · last 3 minutes since assessment began\n{ReadMealAssessment(river.AssessmentStarted).Summary}\n{river.LastResult}" : "";
            return title+"\n\n"+needs+assessment;
        }
    }
    private double RiverCompletion => Campaign!.River!.Phase>=4 ? 1 : Campaign.River.Phase/4d+
        (Campaign.River.Phase is 1 or 3 ? Math.Min(1,Campaign.River.Meals/(Campaign.River.Phase==1?2d:3d)) : RiverPreparationProblem(Campaign.River.Phase>=2)==null?1:0)/4d;
    private void ValidateRiverCampaign()
    {
        var river=Campaign?.River;
        if(IsRiverCampaign != (river!=null)) throw new InvalidOperationException("Missing or misplaced river campaign state");
        if(river!=null && (!float.IsFinite(river.AssessmentStarted) || river.AssessmentStarted<0 || river.AssessmentStarted>Food.Time || river.Phase<0 || river.Phase>4 || river.Meals<0 || river.Meals>(river.Phase is 3 or 4?3:2) || river.Required<0 || river.DeliveredBaseline<0 || river.DeliveredBaseline>DeliveredEdible || river.LastResult==null || Campaign!.Complete!=(river.Phase==4)))
            throw new InvalidOperationException("Invalid river assessment state");
    }
    private static World NewRiverSettlement()
    {
        var w = new World(0);
        w.Trees.Clear(); w.Bushes.Clear(); w._nextTree = 0;
        w.Map = new() { Name = "Across the river", MinX = -10, MinZ = -12, Width = 31, Depth = 27 };
        for (int x=w.Map.MinX;x<=w.Map.MaxX;x++) for (int z=w.Map.MinZ;z<=w.Map.MaxZ;z++)
        {
            bool west = x < 5 && x >= -8 && z >= -6 && z <= 8;
            bool east = x > 5 && x <= 18 && Math.Abs(x-12)+Math.Abs(z-1) <= 17;
            bool river = x == 5 && z >= -6 && z <= 8;
            if (!west && !east && !river) w.Map.Excluded.Add(new(x,z));
            if (river) w.Map.Water.Add(new(x,z));
        }
        foreach(var cell in new[]{new Cell(-7,-4),new(-4,-4),new(2,-4),new(-8,6),new(8,-8),new(11,-10),new(15,-9),new(9,-4),new(14,-3),new(17,0),new(11,3),new(15,5),new(9,9),new(13,12)})
            w.Trees.Add(new() { Id=w._nextTree++, Cell=cell, Logs=8 });
        foreach(var cell in new[]{new Cell(-8,0),new(2,6),new(12,-7),new(16,8)}) w.Bushes.Add(new() { Id=w.Bushes.Count, Cell=cell });
        w.InitialLogs = w.Trees.Sum(t=>t.Logs);
        void Ready(Cell cell, BuildingKind kind)
        {
            var site=w.Place(cell,false,kind) ?? throw new InvalidOperationException("Invalid river starting building");
            site.Delivered=site.Required; site.Construction=1; w.InitialLogs+=site.Required;
        }
        Ready(new(-6,3),BuildingKind.ForagerHut);
        foreach(var cell in new[]{new Cell(-5,0),new(-2,0),new(1,0),new(-5,6)}) Ready(cell,BuildingKind.Cottage);
        w.Food.InitialBerries=w.Food.Berries=40;
        var roles=new[]{Role.Logger,Role.Logger,Role.Builder,Role.Builder,Role.Forager,Role.Forager,Role.Unassigned,Role.Unassigned};
        for(int i=0;i<w.Population;i++) w.Assign(i,roles[i]);
        w.ReconcileHomes(); w.Validate(); w.ValidateMapOccupancy(); return w;
    }
}

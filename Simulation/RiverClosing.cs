using System;
using System.Linq;

namespace Inlanders.Simulation;

public sealed class RiverClosingProgress
{
    public int Boats { get; set; }
    public bool Closed { get; set; }
    public float StableSeconds { get; set; }
    public bool Complete { get; set; }
    public bool AdvanceRequested { get; set; }
    public bool AdvancedSchedule { get; set; }
    public int TrialDeliveries { get; set; }
    public int TrialMeals { get; set; }
}

public sealed partial class World
{
    private int RiverMealsEaten => Food.EatenBerries + Food.EatenVegetables + Food.EatenBread + Food.EatenFish + Food.EatenGame + Food.EatenFruit;

    public static World NewRiverClosing()
    {
        var w=NewCrossingRecovery();
        w.Founding!.FirstHome!.RiverClosing=new();
        w.Map.Name="Avant la fermeture du fleuve · Before the river closes";
        w.History.Clear();
        w.History.Add("Recover food, then prepare for two boats: four settlers at minute 8 with 24 portions, four more at minute 16 without the promised food. The river closes at minute 24. House and feed all twelve, then sustain the village for four minutes without outside supplies. Hunger is recoverable; there is no sudden loss screen.");
        return w;
    }

    // Browser comparison: begin with working access, so the first decision is
    // how to grow the settlement rather than repeating the crossing tutorial.
    public static World NewBrowserRiverClosing()
    {
        var w = NewRiverClosing();
        var crossing = w.Place(RecoveryCrossing, 0, BuildingKind.Bridge)!;
        crossing.Delivered = crossing.Required; crossing.Construction = 1;
        var field = w.Cottages.Single(c => c.Kind == BuildingKind.VegetableField);
        field.Harvest = 0; field.Growth = .15f;
        w.Food.GrownVegetables = 0;
        w._yardLogs = 12;
        w.InitialLogs = w.Trees.Sum(t => t.Logs) + w.Cottages.Sum(c => c.Delivered) + w._yardLogs;
        w.History.Clear();
        w.History.Add("The crossing stands and the first field is growing. Eight portions remain. Prepare homes and food for four neighbors at minute 8 (+24 food), then four more at minute 16 (no food). The river closes at minute 24. A shortage slows work but can be recovered.");
        w.Validate(); w.ValidateMapOccupancy();
        return w;
    }

    private void AdvanceRiverClosing(float dt)
    {
        if(Founding?.FirstHome?.RiverClosing is not {} river || river.Complete)return;
        while(river.Boats<2 && (river.AdvanceRequested || Food.Time>=(river.Boats+1)*480))
        {
            river.AdvanceRequested=false;
            string[] names=river.Boats==0?new[]{"Anne","Jacques","Madeleine","Étienne"}:new[]{"Marguerite","François","Catherine","Jean"};
            foreach(var name in names)People.Add(new Villager{Id=Population,Name=name,Position=YardAccess.Point,Role=Role.Unassigned,SharedWorker=true,NextMealTime=Food.Time+20,Status="Arrived by boat — finding a home and useful work"});
            if(river.Boats==0){Food.InitialBerries+=24;Food.Berries+=24;}
            river.Boats++;
            History.Add(river.Boats==1?"The first boat arrived: four neighbors and 24 portions. The last four neighbors arrive at minute 16, without food. Prepare their homes and expand production.":"The last boat arrived with four neighbors, but no food. Twelve people now depend on this land. At minute 24 the river closes; prove the village can support everyone.");
            ReconcileHomes();_retry=0;
        }
        if(!river.Closed && (Food.Time>=1440 || river.AdvanceRequested))
        {river.Closed=true;Founding.FirstHome.Served.Clear();History.Add("The river is closed. Feed every resident fresh food and sustain housing, meals, production and a one-meal reserve for four continuous minutes.");}
        river.AdvanceRequested=false;
        if(!river.Closed)return;
        if(RiverLivingProblem()==null)
        {
            if(river.StableSeconds==0){river.TrialDeliveries=DeliveredEdible;river.TrialMeals=RiverMealsEaten;}
            river.StableSeconds=Math.Min(240,river.StableSeconds+dt);
        }
        else river.StableSeconds=0;
        if(river.StableSeconds>=240 && DeliveredEdible-river.TrialDeliveries>=RiverMealsEaten-river.TrialMeals){river.Complete=true;History.Add("The village stands on its own. All twelve neighbors have homes, fresh meals and a working food supply after the river closed.");}
    }

    public string? AdvanceRiverScheduleProblem()
    {
        var river=Founding?.FirstHome?.RiverClosing;
        if(river==null || river.Closed)return "The supply season is already over.";
        int target=river.Boats<2?Population+4:Population;
        if(Beds<target)return $"Prepare {target} beds first.";
        if(People.Any(p=>!p.Fed) || ReachableEdibleStored<target*2)return $"Feed everyone and store {target*2} portions first.";
        var flow=ReadFoodFlow();
        if(flow.Seconds<180 || flow.Delivered<target*3)return $"Demonstrate {target*3} fresh deliveries over the last three minutes.";
        return null;
    }
    public bool AdvanceRiverSchedule()
    {
        if(AdvanceRiverScheduleProblem()!=null)return false;
        Founding!.FirstHome!.RiverClosing!.AdvanceRequested=true;
        Founding.FirstHome.RiverClosing.AdvancedSchedule=true;
        return true;
    }

    public string? RiverStabilityProblem()
    {
        if(RiverLivingProblem() is {} problem)return problem;
        var river=Founding!.FirstHome!.RiverClosing!;
        return river.StableSeconds>=240 && DeliveredEdible-river.TrialDeliveries<RiverMealsEaten-river.TrialMeals?
            $"Still need {RiverMealsEaten-river.TrialMeals-(DeliveredEdible-river.TrialDeliveries)} food delivered to match meals eaten. Keep production running.":null;
    }

    private string RiverTrialGoalText()
    {
        var river=Founding!.FirstHome!.RiverClosing!;
        if(river.Complete)return "✓ Self-sufficient\n✓ Four-minute trial passed\n✓ Food production covered meals";
        if(!river.Closed)return "○ Self-sufficiency trial\nStarts when the river closes";
        int seconds=(int)MathF.Floor(river.StableSeconds);
        string stability=$"{(seconds>=240?"✓":"○")} Stable · {seconds/60}:{seconds%60:00} / 4:00";
        // A reset ends the old measurement window; do not show its stale totals.
        string food=river.StableSeconds<=0?"○ Food balance · trial not started":
            $"{(DeliveredEdible-river.TrialDeliveries>=RiverMealsEaten-river.TrialMeals?"✓":"○")} Food delivered {DeliveredEdible-river.TrialDeliveries} / {RiverMealsEaten-river.TrialMeals} eaten";
        return "○ Self-sufficiency trial\n"+stability+"\n"+food;
    }

    public string RiverTrialSupplyText()
    {
        var story=Founding!.FirstHome!;
        return $"Fed · {People.Count(p=>p.Fed)}/{Population} neighbors\nReserve · {ReachableEdibleStored}/{Population} food minimum\nFresh meals · {story.Served.Count}/{Population} neighbors";
    }
    private string? RiverLivingProblem()
    {
        if(People.Any(p=>p.HomeId is not int id || !Cottages.Any(c=>c.Id==id && IsHome(c) && Accessible(c.Entrance))))return "Some neighbors still need a reachable home.";
        if(People.Any(p=>!p.Fed))return "Restore meals for hungry neighbors.";
        if(ReachableEdibleStored<Population)return $"Keep one meal for everyone: {ReachableEdibleStored}/{Population} portions.";
        if(Founding!.FirstHome!.Served.Count<Population)return $"Fresh meals since closure: {Founding.FirstHome.Served.Count}/{Population} neighbors.";
        return null;
    }

    private FirstHomeObjectives ReadRiverClosingObjectives()
    {
        var story=Founding!.FirstHome!;var river=story.RiverClosing!;
        var recovery=ReadCrossingRecoveryObjectives();
        bool supplied=DeliveredEdible>0;
        int requiredBeds=river.Boats==0?8:12;
        int housed=People.Count(p=>p.HomeId is int id && Cottages.Any(c=>c.Id==id && IsHome(c) && Accessible(c.Entrance)));
        string timetable=river.Boats<2?$"Boat +4 in {Math.Max(0,(river.Boats+1)*480-Food.Time)/60:0.0} min · {(river.Boats==0?"24 food":"no food")}":!river.Closed?$"Closure in {(1440-Food.Time)/60:0.0} min · no more shipments":"River closed · stand on your own";
        string action;string hint;int? site=null;
        if(river.Complete){action="done";hint="All twelve neighbors are established and the village feeds itself. Stay and enjoy what you built.";}
        else if(!supplied){action=recovery.Action;hint=action=="recover"?"Restore the crossing or grow food here.":recovery.Hint;site=recovery.Site;}
        else if(Beds<requiredBeds)
        {
            var plans=Cottages.Where(c=>c.Kind==BuildingKind.Cottage && !c.Complete && !c.DemolitionRequested).ToArray();
            action="home";hint=$"Prepare {requiredBeds} beds. New neighbors also need more food.";
            if(Beds+plans.Length*2>=requiredBeds)
            {
                site=plans[0].Id;action="inspect";hint="Enough homes are planned. Finish their construction.";
                if(Available<plans.Sum(c=>c.Remaining(Resource.Logs)) && !Trees.Any(t=>!t.Preserved && t.Logs>0 && Accessible(t.Access)))
                {action="grove";hint="The planned homes need timber. Work an accessible grove.";}
            }
        }
        else if(!river.Closed && AdvanceRiverScheduleProblem()==null){action="advance";hint="Ready early. Advance the next event now, or keep preparing.";}
        else if(river.Closed)
        {
            hint=RiverStabilityProblem()??$"Keep everyone housed and fed for {Math.Max(0,240-(int)MathF.Floor(river.StableSeconds))} more seconds. Shortages reset the timer.";
            action=People.All(p=>p.Fed) && ReachableEdibleStored>=Population && river.StableSeconds<240?"watch":"food";
        }
        else {action="food";hint="Expand food capacity before the next arrival.";}
        var flow=ReadFoodFlow();
        return new($"{(supplied?"✓":"○")} Restore fresh food",
            $"{(Beds>=requiredBeds?"✓":"○")} Homes · {Beds}/{requiredBeds} beds · {housed}/{Population} housed",
            RiverTrialGoalText(),
            river.Closed?hint:timetable+"\n"+hint+$"\nLast {flow.Seconds:0}s: {flow.Delivered} food / {flow.Required} needed.",action,site,(supplied?1:0)+(Beds>=requiredBeds?1:0)+(river.Complete?1:0));
    }
}

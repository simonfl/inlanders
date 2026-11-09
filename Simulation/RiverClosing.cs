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
            "Waiting for production to cover meals eaten during the trial.":null;
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
            hint=RiverStabilityProblem()??"Holding steady. Keep the supply working.";
            action=People.All(p=>p.Fed) && ReachableEdibleStored>=Population && river.StableSeconds<240?"watch":"food";
        }
        else {action="food";hint="Expand food capacity before the next arrival.";}
        var flow=ReadFoodFlow();
        return new($"{(supplied?"✓":"○")} Restore fresh food",
            $"{(Beds>=requiredBeds?"✓":"○")} Homes · {Beds}/{requiredBeds} beds · {housed}/{Population} housed",
            $"{(river.Complete?"✓":"○")} Self-sufficient · {river.StableSeconds/60:0.0}/4 min",
            timetable+"\n"+hint+$"\nLast {flow.Seconds:0}s: {flow.Delivered} food / {flow.Required} needed.",action,site,(supplied?1:0)+(Beds>=requiredBeds?1:0)+(river.Complete?1:0));
    }
}

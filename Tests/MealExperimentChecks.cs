using MealPrototype;
using Inlanders.Simulation;

public static class MealExperimentChecks
{
    static void Check(bool ok,string why) { if(!ok) throw new Exception(why); }
    static Experiment New(int count=1,int route=10)
    {
        var e=new Experiment();
        for(int i=0;i<count;i++) e.People.Add(new() { Id=i,NextDue=i*600/count,RouteTicks=route });
        return e;
    }
    static void Roundtrip(Experiment e)
    {
        string saved=e.Save(); var copy=Experiment.Load(saved);
        Check(copy.Save()==saved,"Experiment changed on reload");
        var twin=Experiment.Load(saved); copy.Step(1200); twin.Step(1200);
        Check(copy.Save()==twin.Save(),"Experiment continuation diverged");
    }
    public static void Run()
    {
        var e=New(); e.Supply(0,4); Roundtrip(e);
        e.Step(); Check(e.Available(0)==3 && e.Stock[0]==4,"Claim withdrew food too early"); Roundtrip(e);
        e.Interrupt(0); Check(e.Available(0)==4,"Walking interruption leaked claim");
        e.Step(11); Check(e.People[0].Request!.Phase==Phase.Eating && e.Stock[0]==3 && e.Eaten[0]==0,"Collection was not physical"); Roundtrip(e);
        e.Interrupt(0); Check(e.People[0].Request!.Phase==Phase.Returning && e.Stock[0]==3,"Interruption teleported food"); Roundtrip(e);
        e.Step(10); Check(e.Stock[0]==4,"Return lost carried portion");
        e.Step(51); Check(e.Eaten.Sum()==1 && e.Outcomes.Count==0,"Eating/count boundary wrong"); Roundtrip(e);
        e.Step(600-e.Time+1); Check(e.Outcomes.Count==1 && e.Outcomes[0].Timely,"Timely outcome missing");

        var late=New(route:5); late.Supply(2,5); late.People[0].BusyUntil=580;
        late.Step(601); Check(late.Outcomes.Count==1 && !late.Outcomes[0].Timely && late.Hunger==1,"Deadline failed to record delayed eating");
        late.Step(40); Check(late.Meals.Count==1 && late.Meals[0].Late && late.Hunger==0 && !late.Outcomes[0].Timely,"Late nourishment rewrote reliability or stayed hungry");
        Check(late.Skipped.Count==1 && late.People[0].NextDue==1200,"Late completion created catch-up debt"); Roundtrip(late);

        var blocked=New(route:5); blocked.Supply(4,1); blocked.Step(6); blocked.People[0].HoldUntil=950;
        blocked.Step(895); Check(blocked.People[0].Request!.Phase==Phase.Returning && blocked.Stock[4]==0,"Late bound did not protect carried food");
        blocked.Step(60); Check(blocked.Stock[4]==1 && blocked.Eaten[4]==0 && blocked.Outcomes.Count==1,"Timed-out meal was consumed or lost");
        var tooFar=New(route:700); tooFar.Supply(3,1); tooFar.Step(601);
        Check(tooFar.Available(3)==1 && tooFar.Eaten[3]==0 && tooFar.Outcomes.Count==1 && tooFar.Hunger==1,"Expired walking request kept food reserved");
        var empty=New(8); empty.Step(1201); Check(empty.Hunger==1 && empty.Outcomes.Count>=8,"Shortages did not close requests");
        empty.Supply(1,32); empty.Step(1200); Check(empty.Hunger==0,"Supplied village failed to recover");

        for(int k=0;k<5;k++)
        {
            var diet=New(); diet.Supply(k,3); diet.Step(1300);
            Check(diet.Eaten[k]==3,"Edible type could not satisfy meals");
        }
        var fairness=New(4,1); foreach(var p in fairness.People) p.NextDue=0;
        for(int round=0;round<4;round++) { fairness.Supply(0,1); fairness.Step(round==0?601:600); }
        Check(fairness.Meals.Select(m=>m.Resident).Distinct().Count()==4,"Low IDs monopolized scarce meals");
        Baselines();
        Console.WriteLine("PASS: meal experiment stock/claims, physical return, all five foods, timed/late/missed/skipped demand, recovery, fair scarcity and exact phase saves.");
    }
    static void Baselines()
    {
        foreach(bool remote in new[]{false,true})
        {
            var w=World.NewLargeMap(); string world=w.SaveJson();
            var e=New(8);
            var far=w.Map.Land.Where(c=>c.X>8 && w.PathProblem(c)==null).OrderByDescending(c=>c.Z).First();
            foreach(var p in e.People)
                p.RouteTicks=w.MealExperimentRouteTicks(remote && p.Id>=4?far:w.YardAccess,w.YardAccess) ?? throw new Exception("Baseline source unreachable");
            e.Supply(0,200); e.Step(6000);
            Check(world==w.SaveJson(),"Route probe changed village");
            Console.WriteLine($"MEAL baseline {(remote?"dispersed":"central")}: {e.Meals.Count} consumed, {e.Outcomes.Count(o=>o.Timely)} timely / {e.Outcomes.Count} closed, {e.Skipped.Count} skipped; {e.People.Sum(p=>p.Travel)/10f:0.0}s outbound travel + {e.Meals.Count*4}s eating / 600s, {e.People.Count(p=>!p.Fed)} currently unfed; route ticks [{string.Join(',',e.People.Select(p=>p.RouteTicks))}].");
        }
    }
}

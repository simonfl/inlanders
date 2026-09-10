using Inlanders.Simulation;

public static class MealServiceChecks
{
    static void Check(bool ok,string why) { if(!ok) throw new Exception(why); }
    static void Step(World w,int ticks=1) { for(int i=0;i<ticks;i++) { w.Tick(.1f); w.Validate(); } }
    public static void Run()
    {
        var scarce=new World();
        foreach(var p in scarce.People) { scarce.Assign(p.Id,Role.Unassigned); p.NextMealTime=0; }
        scarce.Food.InitialBerries=scarce.Food.Berries=1;
        var winners=new List<int>();
        for(int cycle=0;cycle<3;cycle++)
        {
            while(scarce.Food.Time<cycle*60+55) Step(scarce);
            winners.Add(scarce.Food.MealConsumptions.Last().Person);
            if(cycle<2)
            {
                while(scarce.Food.Time<(cycle+1)*60+1) Step(scarce);
                scarce.Food.InitialBerries++; scarce.Food.Berries++;
            }
        }
        Check(winners.SequenceEqual(new[]{0,1,2}),"Simultaneous scarce meal claims did not rotate fairly");
        foreach(var kind in World.EdibleKinds)
        {
            var single=new World(); foreach(var p in single.People) single.Assign(p.Id,Role.Unassigned);
            single.Food.InitialBerries=single.Food.Berries=0;
            switch(kind)
            {
                case Resource.Berries: single.Food.InitialBerries=single.Food.Berries=40; break;
                case Resource.Vegetables: single.Food.GrownVegetables=single.Food.Vegetables=40; break;
                case Resource.Bread:
                    single.Food.BakedBread=single.Food.Bread=40;
                    single.Food.GrownGrain=single.Food.UsedGrain=20; break;
                case Resource.Fish: single.Food.CaughtFish=single.Food.Fish=40; break;
                case Resource.Game: single.Food.HuntedGame=single.Food.Game=40; break;
            }
            Step(single,1250);
            Check(single.Food.MealConsumptions.Count>=16 && single.Food.MealConsumptions.All(m=>m.Kind==kind),$"Actual meals failed for {kind}");
            Check(World.LoadJson(single.SaveJson()).SaveJson()==single.SaveJson(),$"{kind} meal history failed save");
        }
        var delayed=new World(); foreach(var p in delayed.People) delayed.Assign(p.Id,Role.Unassigned);
        delayed.Food.InitialBerries=delayed.Food.Berries=100;
        var diner=delayed.People[0];
        for(int i=0;i<500 && diner.Task!=Work.EatingMeal;i++) Step(delayed);
        Check(diner.Task==Work.EatingMeal,"Late-service fixture never collected its meal");
        int request=diner.Meal!.Id;
        // Hold eating progress to exercise a delayed physical portion across its deadline.
        while(delayed.Food.Time<diner.Meal.Due+61) { diner.Timer=0; Step(delayed); }
        Check(!diner.Fed && delayed.Food.MealOutcomes.Any(m=>m.Request==request && !m.Timely),"Missed deadline was not recorded");
        string lateSave=delayed.SaveJson(); Check(World.LoadJson(lateSave).SaveJson()==lateSave,"Late carried meal failed save");
        Step(delayed,60);
        Check(diner.Fed && delayed.Food.MealConsumptions.Any(m=>m.Request==request && m.Late),"Late eating did not restore nourishment");
        Check(delayed.Food.MealOutcomes.Any(m=>m.Person==diner.Id && m.Skipped) && delayed.Food.MealOutcomes.Single(m=>m.Request==request).Timely==false,"Late eating erased missed or skipped service");
        Check(delayed.ReadFoodFlow().Required==delayed.Food.MealOutcomes.Count,"Skipped demand is absent from food-flow accounting");
        var delayedAssessment=delayed.ReadMealAssessment();
        Check(!delayedAssessment.Reliable && delayedAssessment.Missed>0 && delayedAssessment.Skipped>0,"Late recovery incorrectly proved reliable service");
        string inspected=delayed.SaveJson();
        Check(delayed.NeedsMealAttention(diner),"Nourished resident with recent missed service disappeared from investigation");
        Check(delayed.SaveJson()==inspected,"Meal attention query changed state");
        Step(delayed,1900);
        Check(diner.Fed && !delayed.NeedsMealAttention(diner),"Recovered resident stayed flagged after missed history expired");
        var creative=World.NewCreative();
        Check(!creative.People.Any(creative.NeedsMealAttention),"Creative meal needs were enabled by inspection");
        var w=new World(); foreach(var p in w.People) w.Assign(p.Id,Role.Unassigned);
        w.Food.InitialBerries=w.Food.Berries=100;
        var phases=new HashSet<Work>();
        for(int i=0;i<1500;i++)
        {
            Step(w);
            Check(w.DeliveredBerries==0,"Collecting or returning initial meal stock changed fresh deliveries");
            foreach(var p in w.People.Where(p=>p.Task is Work.ToMealSupply or Work.ToMealSeat or Work.EatingMeal))
            {
                if(!phases.Add(p.Task)) continue;
                string saved=w.SaveJson(); var copy=World.LoadJson(saved); Check(copy.SaveJson()==saved,"Physical meal phase save changed");
                string explanation=w.MealSummary(p);
                Check(w.SaveJson()==saved && copy.MealSummary(copy.People[p.Id])==explanation,"Meal inspection mutated state or changed after reload");
                var twin=World.LoadJson(saved); Step(copy,200); Step(twin,200); Check(copy.SaveJson()==twin.SaveJson(),"Meal continuation diverged");
                Check(w.PlacementProblem(p.Meal!.Seat,false,BuildingKind.SeatingGarden)!=null,"Meal seat could be built over");
                copy=World.LoadJson(saved); var before=copy.Food.EatenBerries; copy.Assign(p.Id,Role.Logger);
                if(p.Carried>0)
                {
                    Check(copy.People[p.Id].Task==Work.ReturnMeal && copy.People[p.Id].Carried==1,"Interrupted meal did not return physically");
                    string returning=copy.SaveJson(); Check(World.LoadJson(returning).SaveJson()==returning,"Returning meal failed save");
                }
                Step(copy,30);
            }
        }
        Check(phases.Count==3 && w.People.All(p=>p.Fed) && w.Food.EatenBerries>=16,$"Residents did not repeatedly eat: phases {string.Join(',',phases)}, eaten {w.Food.EatenBerries}, time {w.Food.Time}; "+string.Join("; ",w.People.Select(p=>$"{p.Id} {p.Task} fed={p.Fed} due={p.Meal?.Due} next={p.NextMealTime} {p.Status}")));
        Check(w.Food.MealOutcomes.Any(m=>m.Timely) && w.Food.MealConsumptions.Count>0,"Actual service events missing");
        Check(!w.ReadMealAssessment().Reliable,"Residents with pending second deadlines counted as proven service");
        Step(w,350);
        string assessmentSave=w.SaveJson(); var assessment=w.ReadMealAssessment();
        Check(assessment.Reliable && !assessment.Varied && !assessment.FreshSupply,"Service, variety and fresh supply were conflated");
        Check(w.SaveJson()==assessmentSave && World.LoadJson(assessmentSave).ReadMealAssessment()==assessment,"Assessment mutated or failed to restore");
        Check(!w.ReadMealAssessment(w.Food.Time).Reliable,"Restarted assessment inherited old service proof");
        var hungry=new World(); foreach(var p in hungry.People) hungry.Assign(p.Id,Role.Unassigned);
        hungry.Food.InitialBerries=hungry.Food.Berries=0; Step(hungry,1300);
        Check(hungry.Food.Hunger==1,"No-food residents did not become hungry");
        hungry.Food.InitialBerries+=40; hungry.Food.Berries+=40; Step(hungry,700);
        Check(hungry.People.All(p=>p.Fed),"Actual meals failed to recover hunger");
        Console.WriteLine($"PASS: live central meal routes/cargo/seats, three phase saves and interruption, {w.Food.EatenBerries} consumed portions, shortage/recovery and food conservation.");
    }
}

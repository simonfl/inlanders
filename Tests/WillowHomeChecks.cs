using Inlanders.Simulation;
using System.Text.Json;

static class WillowHomeChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    public static void Run()
    {
        foreach(string plan in new[]{"crossing","shore","gardens","recovery"})
        {
            var w=World.NewWillowHome();
            void Advance(int ticks){for(int i=0;i<ticks;i++)w.Tick(.1f);w.Validate();}
            void SaveCheck(){var copy=World.LoadJson(w.SaveJson());for(int i=0;i<10;i++){w.Tick(.1f);copy.Tick(.1f);}Check(w.SaveJson()==copy.SaveJson(),"Willow continuation diverged");w.Validate();}
            void Restore(BuildingKind kind){foreach(var site in w.Cottages.Where(c=>!c.Complete && c.ConstructionPaused && c.Kind==kind))Check(w.RestoreWillow(site.Id)==null,"Restoration rejected");}
            Check(w.Housed==4 && w.Population==8 && w.ReadWillowHomeProgress()!.Restored==0,"Wrong opening");
            Check(w.BeginWillowSupper()!=null && w.OrderStoryShipment("food")!=null && w.WelcomeStory()!=null,"Opening bypass");
            if(plan=="recovery")
            {
                Restore(BuildingKind.Cottage);Advance(400);
                var unfinished=w.Cottages.FirstOrDefault(c=>w.IsWillowRuin(c) && !c.ConstructionPaused);
                if(unfinished!=null){Check(w.PauseWillow(unfinished.Id)==null,"Pause failed");SaveCheck();Check(w.RestoreWillow(unfinished.Id)==null,"Resume failed");}
                Advance(1800);Restore(BuildingKind.Bridge);
            }
            else if(plan=="crossing")Restore(BuildingKind.Bridge);
            else if(plan=="shore"){Restore(BuildingKind.FishingDock);Check(w.PlanRiverside(new(4,4),0,BuildingKind.VegetableGarden),"Shore garden invalid");}
            else
            {
                Check(w.PlanRiverside(new(4,4),0,BuildingKind.VegetableGarden),"East garden invalid: "+w.RiversideSiteProblem(new(4,4),0,BuildingKind.VegetableGarden));
                Check(w.PlanRiverside(new(-1,8),0,BuildingKind.VegetableGarden),"West garden invalid: "+w.RiversideSiteProblem(new(-1,8),0,BuildingKind.VegetableGarden));
                var third=new Cell(-4,8);for(int retry=0;retry<100 && w.RiversideSiteProblem(third,0,BuildingKind.VegetableGarden)!=null;retry++)Advance(10);Check(w.PlanRiverside(third,0,BuildingKind.VegetableGarden),"Third garden invalid: "+w.RiversideSiteProblem(third,0,BuildingKind.VegetableGarden));
                w.ApplyWoodlandCare(new(5,7),new(5,7),WoodlandIntent.Renew,false);
            }
            Advance(350);SaveCheck();
            if(plan is "shore" or "gardens")
            {
                for(int retry=0;retry<200 && w.RiversideSiteProblem(new(-6,5),0,BuildingKind.Cottage)?.Contains("standing")==true;retry++)Advance(10);Check(w.PlanRiverside(new(-6,5),0,BuildingKind.Cottage),"Near home west invalid: "+w.RiversideSiteProblem(new(-6,5),0,BuildingKind.Cottage));
                Check(w.PlanRiverside(new(2,7),0,BuildingKind.Cottage),"Near home east invalid");
            }
            else Restore(BuildingKind.Cottage);
            w.ApplyWoodlandCare(new(5,7),new(5,7),WoodlandIntent.Renew,false);
            bool routeUsed=false;float hungry=0;
            for(int i=0;i<12000 && w.WillowSupperProblem()!=null;i++)
            {
                w.Tick(.1f);if(w.People.Any(p=>!p.Fed))hungry+=.1f;if(w.People.Any(p=>p.Position.Y<-1 && p.Carried>0))routeUsed=true;
                if(plan is "crossing" or "recovery" && w.VillageStory!.SalvageExhausted && w.Cottages.Single(c=>c.Id==w.VillageStory.SalvageField).ConstructionPaused)
                {Check(w.RestoreWillow(w.VillageStory.SalvageField)==null,"Field restoration rejected");w.ApplyWoodlandCare(new(-6,-8),new(-6,-8),WoodlandIntent.Renew,false);Check(w.PlanRiverside(new(4,4),0,BuildingKind.VegetableGarden),"Support garden invalid");}
            }
            w.Validate();SaveCheck();Check(w.WillowSupperProblem()==null,$"{plan}: not ready at {w.Food.Time}: {w.WillowSupperProblem()}");
            int before=w.ReachableEdibleStored;
            Check(w.BeginWillowSupper()==null,"Ready supper refused");Check(w.ReachableEdibleStored==before-8,"Supper didn't consume exactly eight");
            Check(w.BeginWillowSupper()!=null,"Duplicate supper accepted");SaveCheck();
            for(int i=0;i<2400 && !w.VillageStory!.Complete;i++)w.Tick(.1f);
            Check(w.VillageStory!.Complete && w.Food.SupperComplete && !w.Food.Celebrating,"Supper failed to finish");SaveCheck();
            Console.WriteLine(JsonSerializer.Serialize(new{plan,seconds=w.Food.Time,hungry,routeUsed,food=w.ReachableEdibleStored,homes=w.Housed,restored=w.ReadWillowHomeProgress()!.Restored}));
        }
        Console.WriteLine("PASS alternative livelihoods, shortage recovery, repairs, actual supper consumption, exact save continuation and gathering.");
    }
}

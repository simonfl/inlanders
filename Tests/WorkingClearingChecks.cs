using Inlanders.Simulation;
static class WorkingClearingChecks
{
    public static void Run()
    {
        Ensemble();
        CheckCourtChoice();
        PrepareCourtComparison("growing");PrepareCourtComparison("shore");PrepareCourtComparison("combined");
        foreach(bool relaxed in new[]{false,true})foreach(string change in new[]{"keep","near-field","landing"})
        {
            void Check(bool ok,string why){if(!ok)throw new Exception(why);}
            var w=World.NewWorkingClearing(relaxed);Check(w.Population==8 && w.Housed==8 && w.Cottages.Count==6 && w.PublicPlace!.Create().SaveJson()==w.SaveJson(),"Clearing inventory/profile");
            var f=w.Cottages.First(c=>c.Kind==BuildingKind.VegetableField);
            if(change=="near-field")
            {
                w.SetWorkplacePaused(f.Id,true);var at=new Cell(0,7);Check(w.RelocationProblem(f.Id,at,1)==null,"Central court accepts growing ground");
                Check(w.MoveBuilding(f.Id,at,1),"Nearby field move");w.SetWorkplacePaused(f.Id,false);
            }
            if(change=="landing")
            {var at=w.Map.Land.First(c=>w.PlacementProblem(c,1,BuildingKind.FishingDock)==null);Check(w.Place(at,1,BuildingKind.FishingDock)!=null,"Landing branch");}
            int hungry=0;for(int i=0;i<36000;i++){w.Tick(.1f);if(w.People.Any(p=>!p.Fed))hungry++;if(i%100==0)w.Validate();}
            Check(w.Food.EatenVegetables>0 && w.Housed==8,"Working clearing life");if(change=="landing")Check(w.Food.EatenFish>0,"Landing not used");
            Check(relaxed || hungry==0,$"Clearing {change} cannot remain modest: {hungry} hungry ticks");
            var copy=World.LoadJson(w.SaveJson());for(int i=0;i<100;i++){w.Tick(.1f);copy.Tick(.1f);}Check(w.SaveJson()==copy.SaveJson(),"Clearing continuation");
            Console.WriteLine($"PASS clearing {relaxed}/{change}: 60min actual life, food {w.EdibleStored}, grown {w.Food.GrownVegetables}, hungry ticks {hungry}, exact continuation");
        }
    }
    public static void Ensemble()
    {
        foreach(bool relaxed in new[]{false,true})
        {
            var old=World.NewWorkingClearing(relaxed,true);var current=World.NewWorkingClearing(relaxed);
            void Check(bool ok,string why){if(!ok)throw new Exception(why);}
            Check(old.Population==current.Population && old.Housed==current.Housed && old.InitialLogs==current.InitialLogs && old.Stored==current.Stored && old.Planks==current.Planks && old.EdibleStored==current.EdibleStored,"Ensemble changed initial supply");
            Check(old.Cottages.Select(c=>(c.Kind,c.Required,c.Depth,c.Growth)).SequenceEqual(current.Cottages.Select(c=>(c.Kind,c.Required,c.Depth,c.Growth))),"Ensemble changed productive capacity/investment");
            foreach(var (w,label) in new[]{(old,"former"),(current,"recomposed")})
            {
                int hungry=0;for(int i=0;i<6000;i++){w.Tick(.1f);if(w.People.Any(p=>!p.Fed))hungry++;if(i%100==0)w.Validate();}
                Check(w.Food.EatenVegetables>0 && (relaxed || hungry==0),"Ensemble failed ordinary life");
                var copy=World.LoadJson(w.SaveJson());for(int i=0;i<50;i++){w.Tick(.1f);copy.Tick(.1f);}Check(w.SaveJson()==copy.SaveJson(),"Ensemble save continuation");
                Directory.CreateDirectory("artifacts/clearing-ensemble");w.SaveFile($"artifacts/clearing-ensemble/{label}-{relaxed}.json");
                Console.WriteLine($"PASS ensemble {label}/{relaxed}: equal initial supply/crop capacity; ten minutes, vegetables eaten {w.Food.EatenVegetables}, grown {w.Food.GrownVegetables}, hungry {hungry}, exact continuation");
            }
        }
    }
    private static void CheckCourtChoice()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        var w=World.NewWorkingClearing();var home=w.Cottages.Single(c=>c.Cell==new Cell(-3,9));
        Check(w.Map.Water.Contains(new(5,1)) && !w.Map.Water.Contains(new(3,1)) && !w.Map.LevelGround(new[]{new Cell(0,12)}),"Court geography missing");
        Check(w.SetCommons(new(0,7)),"Court shared ground unavailable");
        var f=w.Cottages.First(c=>c.Kind==BuildingKind.VegetableField);w.SetWorkplacePaused(f.Id,true);
        Check(w.RelocationProblem(f.Id,new(0,7),1)!=null,"Field can occupy shared court");
        w.RemoveCommons();Check(w.FurnishHomeYard(home.Id,3),"Court domestic ground unavailable");
        Check(w.RelocationProblem(f.Id,new(0,7),1)!=null,"Field can occupy ordered domestic ground");
        Check(w.PlacementProblem(new(5,7),1,BuildingKind.FishingDock)==null,"Court alternative lacks shore livelihood");
        w.Validate();Console.WriteLine("PASS clearing court: actual water/slope, competing field/shared/domestic claims, nearby landing");
    }

    public static World PrepareCourtComparison(string arm)
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        var w=World.NewWorkingClearing();
        if(arm is "growing" or "combined")
        {
            var f=w.Cottages.First(c=>c.Kind==BuildingKind.VegetableField);w.SetWorkplacePaused(f.Id,true);
            if(arm=="combined")Check(w.SetCommons(new(0,7)),"Combined commons refused");
            var target=arm=="combined"?new Cell(0,2):new Cell(0,7);
            Check(w.MoveBuilding(f.Id,target,1),"Comparison field refused: "+w.RelocationProblem(f.Id,target,1));w.SetWorkplacePaused(f.Id,false);
        }
        else
        {
            Check(w.SetCommons(new(0,7)),"Comparison commons refused");
            Check(w.Place(new(5,7),1,BuildingKind.FishingDock)!=null,"Comparison landing refused");
        }
        var home=w.Cottages.Single(c=>c.Cell==new Cell(arm=="growing"?3:-3,9));
        Check(w.FurnishHomeYard(home.Id,0),"Comparison yard refused");
        int domestic=0,shared=0,hungry=0;
        for(int i=0;i<6000;i++)
        {
            w.Tick(.1f);if(i%100==0)w.Validate();
            domestic+=w.People.Count(p=>w.AtFurnishedHome(p));
            shared+=w.People.Count(p=>p.Task==Work.EatingMeal && p.Meal?.Commons==true);
            if(w.People.Any(p=>!p.Fed))hungry++;
        }
        Check(home.Improved && domestic>0 && hungry==0,"Comparison lacked furnished domestic use or food");
        Check(arm=="shore"?w.Food.EatenFish>0 && shared>0:w.Food.EatenVegetables>0 && (arm!="combined" || shared>0),"Comparison livelihood/shared ground unused");
        w.Validate();Check(World.LoadJson(w.SaveJson()).SaveJson()==w.SaveJson(),"Comparison save");
        Console.WriteLine($"PASS court {arm}: 10min actual life; domestic ticks {domestic}, shared meal ticks {shared}, fish eaten {w.Food.EatenFish}, vegetables eaten {w.Food.EatenVegetables}, hungry {hungry}");
        return w;
    }

}

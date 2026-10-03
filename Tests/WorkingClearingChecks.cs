using Inlanders.Simulation;
static class WorkingClearingChecks
{
    public static void Run()
    {
        CheckCourtChoice();
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
    private static void CheckCourtChoice()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        var w=World.NewWorkingClearing();var home=w.Cottages.Single(c=>c.Cell==new Cell(-3,9));
        Check(w.Map.Water.Contains(new(3,1)) && !w.Map.LevelGround(new[]{new Cell(0,12)}),"Court geography missing");
        Check(w.SetCommons(new(0,7)),"Court shared ground unavailable");
        var f=w.Cottages.First(c=>c.Kind==BuildingKind.VegetableField);w.SetWorkplacePaused(f.Id,true);
        Check(w.RelocationProblem(f.Id,new(0,7),1)!=null,"Field can occupy shared court");
        w.RemoveCommons();Check(w.FurnishHomeYard(home.Id,3),"Court domestic ground unavailable");
        Check(w.RelocationProblem(f.Id,new(0,7),1)!=null,"Field can occupy ordered domestic ground");
        Check(w.PlacementProblem(new(5,7),1,BuildingKind.FishingDock)==null,"Court alternative lacks shore livelihood");
        w.Validate();Console.WriteLine("PASS clearing court: actual water/slope, competing field/shared/domestic claims, nearby landing");
    }

}

using Inlanders.Simulation;
static class RiverLivelihoodChecks
{
    public static void Run()
    {
        Directory.CreateDirectory("artifacts/river-livelihood");
        foreach(bool relaxed in new[]{false,true})foreach(bool shore in new[]{false,true})
        {
            var ready=World.NewRiverFrontage(relaxed,true,true);var w=World.NewRiverLivelihood(relaxed);
            void Check(bool ok,string why){if(!ok)throw new Exception(why);}
            Check(w.PublicPlace!.Create().SaveJson()==w.SaveJson(),"Restart differs");
            Check(w.Housed==12 && w.Population==12 && w.Cottages.Count==6 && w.InitialLogs==ready.InitialLogs && w.EdibleStored==ready.EdibleStored && w.Stored==ready.Stored+ready.Cottages.Where(c=>World.ProductionOutput(c.Kind)!=null).Sum(c=>c.Delivered),"Arrival resources differ");
            var kinds=shore?new[]{BuildingKind.VegetableField,BuildingKind.FishingDock}:new[]{BuildingKind.VegetableField};
            foreach(var site in ready.Cottages.Where(c=>kinds.Contains(c.Kind) && (!shore || c.Kind==BuildingKind.FishingDock || c.Depth==2)))
                Check(w.Place(site.Cell,site.Rotation,site.Kind,site.PlotRows)!=null,"Chosen livelihood refused");
            int hunger=0;float first=0;
            for(int i=0;i<12000;i++){w.Tick(.1f);hunger+=w.People.Count(p=>!p.Fed);if(first==0 && w.FoundingHasNewFood)first=w.Food.Time;}
            Check(w.FoundingHasNewFood && w.Food.MealConsumptions.Count>0 && w.Cottages.All(c=>c.Complete),"Livelihood did not operate");
            Check(hunger==0,"Chosen livelihood starved");
            w.Validate();w.SaveFile($"artifacts/river-livelihood/{relaxed}-{shore}.json");
            var copy=World.LoadJson(w.SaveJson());for(int i=0;i<600;i++){w.Tick(.1f);copy.Tick(.1f);}Check(w.SaveJson()==copy.SaveJson(),"Continuation differs");
            Console.WriteLine($"PASS river livelihood relaxed={relaxed} shore={shore}: first food {first:0}s, twenty-minute meals, no hunger, exact continuation.");
        }
    }
}

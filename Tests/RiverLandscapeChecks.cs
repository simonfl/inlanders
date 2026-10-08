using Inlanders.Simulation;
static class RiverLandscapeChecks
{
    public static void Run()
    {
        Directory.CreateDirectory("artifacts/river-landscape");
        foreach(bool relaxed in new[]{false,true})
        {
            var baseline=World.NewRiverFrontage(relaxed,true);var w=World.NewRiverFrontage(relaxed,true,true);
            void Check(bool ok,string why){if(!ok)throw new Exception(why);}
            Check(w.Population==baseline.Population && w.InitialLogs==baseline.InitialLogs && w.Stored==baseline.Stored && w.EdibleStored==baseline.EdibleStored && w.Planks==baseline.Planks,"Landscape changed matched resources");
            Check(w.Cottages.Select(c=>c.Kind).SequenceEqual(baseline.Cottages.Select(c=>c.Kind)) && w.Cottages.Where(c=>c.Kind==BuildingKind.VegetableField).Sum(c=>c.Depth)==baseline.Cottages.Where(c=>c.Kind==BuildingKind.VegetableField).Sum(c=>c.Depth),"Landscape changed production/building capacity");
            Check(w.PublicPlace!.Create().SaveJson()==w.SaveJson(),"Landscape restart differs");
            int hunger=0;for(int i=0;i<12000;i++){w.Tick(.1f);baseline.Tick(.1f);hunger+=w.People.Count(p=>!p.Fed);}
            Check(w.FoundingHasNewFood && w.Food.MealConsumptions.Count>0 && hunger==0,"Landscape did not support actual meals");w.Validate();w.SaveFile($"artifacts/river-landscape/{relaxed}.json");baseline.SaveFile($"artifacts/river-landscape/baseline-{relaxed}.json");
            var copy=World.LoadJson(w.SaveJson());for(int i=0;i<600;i++){w.Tick(.1f);copy.Tick(.1f);}Check(w.SaveJson()==copy.SaveJson(),"Landscape continuation differs");
            Console.WriteLine($"PASS river landscape {relaxed}: equal population/materials/buildings/crop capacity, twenty-minute meals, exact continuation.");
        }
    }
}

using Inlanders.Simulation;
static class CultivatedBankChecks
{
    public static void Run()
    {
        Directory.CreateDirectory("artifacts/cultivated-bank");
        foreach(bool relaxed in new[]{false,true})
        {
            var baseline=World.NewTransformationHamlet(relaxed);var w=new HamletProfile(relaxed,true).Create();
            if(w.Population!=baseline.Population || w.Housed!=baseline.Housed || w.EdibleStored!=baseline.EdibleStored || !w.Cottages.Select(c=>c.Kind).SequenceEqual(baseline.Cottages.Select(c=>c.Kind)))throw new Exception("Comparison changed inventory/population");
            if(w.Cottages.Any(c=>!w.Paths.Contains(c.Entrance)))throw new Exception("Missing real approach");
            w.Validate();
            int hungry=0;for(int i=0;i<6000;i++){w.Tick(.1f);if(w.People.Any(p=>!p.Fed))hungry++;if(i%100==0)w.Validate();}
            if(!w.FoundingHasNewFood)throw new Exception("Actual cultivation never delivered food");
            var copy=World.LoadJson(w.SaveJson());for(int i=0;i<100;i++){w.Tick(.1f);copy.Tick(.1f);}
            if(w.SaveJson()!=copy.SaveJson() || copy.PublicPlace!=new HamletProfile(relaxed,true) || !copy.PublicPlace.Create().Founding!.CultivatedBank)throw new Exception("Layout identity/restart/reload differs");
            w.SaveFile($"artifacts/cultivated-bank/{(relaxed?"relaxed":"normal")}.json");
            Console.WriteLine($"PASS: cultivated bank {(relaxed?"relaxed":"normal")}: actual food, paths, exact continuation; hungry ticks {hungry}, food {w.EdibleStored}. No enjoyment/ranking claim.");
        }
    }
}


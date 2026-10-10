using Inlanders.Simulation;
static class ArrivalRecoveryChecks
{
    public static void Run()
    {
        Directory.CreateDirectory("artifacts/arrival-recovery");
        foreach(bool relaxed in new[]{false,true})
        {
            var w=World.NewRiverLivelihood(relaxed);
            if(w.Place(new(8,5),1,BuildingKind.FishingDock)==null)throw new Exception("No landing");
            for(int i=0;i<18000;i++)w.Tick(.1f);
            if(w.EdibleStored>=w.Population*2)throw new Exception("Inadequate arrival not exposed");
            w.SaveFile($"artifacts/arrival-recovery/{relaxed}-low.json");
            bool longField=false,smallField=false;int lateMisses=0;
            for(int i=0;i<12000;i++)
            {
                if(!longField)longField=w.Place(new(2,-7),1,BuildingKind.VegetableField,8)!=null;
                if(!smallField)smallField=w.Place(new(0,0),1,BuildingKind.VegetableField,2)!=null;
                w.Tick(.1f);if(i>6000)lateMisses+=w.People.Count(p=>!p.Fed);
            }
            if(!longField || !smallField || lateMisses!=0 || w.Food.EatenVegetables==0)throw new Exception("Arrival recovery failed");
            w.Validate();w.SaveFile($"artifacts/arrival-recovery/{relaxed}-recovered.json");
            var copy=World.LoadJson(w.SaveJson());for(int i=0;i<600;i++){w.Tick(.1f);copy.Tick(.1f);}if(w.SaveJson()!=copy.SaveJson())throw new Exception("Recovery continuation differs");
            Console.WriteLine($"PASS arrival recovery relaxed={relaxed}: inadequate landing, ordinary cultivation, final ten minutes without missed meals and exact continuation.");
        }
    }
}

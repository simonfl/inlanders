using Inlanders.Simulation;
static class HamletLayoutChecks
{
    public static void Run()
    {
        var w=World.NewTransformationHamlet();
        if(w.Cottages.Any(c=>!w.Paths.Contains(c.Entrance)) || w.Paths.Any(c=>w.PathProblem(c)!=null))throw new Exception("Authored working approaches miss entrances or cross blocked ground");
        var camp=w.Place(new(-4,-12),0,BuildingKind.Quarry) ?? throw new Exception("Hamlet quarry unavailable: "+w.PlacementProblem(new(-4,-12),0,BuildingKind.Quarry));
        w.Assign(0,Role.Builder);w.Assign(1,Role.Quarrier);
        for(int i=0;i<6000 && w.Stone==0;i++){w.Tick(.1f);if(i%100==0)w.Validate();}
        if(!camp.Complete || w.Stone==0)throw new Exception("Hamlet quarry failed to construct and deliver actual stone");
        var copy=World.LoadJson(w.SaveJson());for(int i=0;i<100;i++){w.Tick(.1f);copy.Tick(.1f);}
        if(w.SaveJson()!=copy.SaveJson())throw new Exception("Hamlet quarry continuation differs");w.Validate();
        Console.WriteLine("PASS: authored hamlet has a legal quarry, actual construction/extraction/delivery and exact save continuation.");
    }
}

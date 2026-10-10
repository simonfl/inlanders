using Inlanders.Simulation;
static class GroveRenewalChecks
{
    public static void Run()
    {
        foreach(bool relaxed in new[]{false,true})
        {
            var w=World.NewRiverLivelihood(relaxed);int timber=w.Stored;
            var at=w.Map.Land.OrderBy(c=>(c.Point-new Cell(-6,-6).Point).LengthSquared()).First(c=>w.PlantingProblem(c)==null);
            if(!w.SetManagedWoodland(at,true))throw new Exception("Grove rejected");
            for(int i=0;i<900 && w.TreesPlanted==0;i++)w.Tick(.1f);
            if(w.TreesPlanted==0 || w.Stored!=timber)throw new Exception("Grove waits for timber shortage");
            var copy=World.LoadJson(w.SaveJson());for(int i=0;i<600;i++){w.Tick(.1f);copy.Tick(.1f);}if(w.SaveJson()!=copy.SaveJson())throw new Exception("Grove continuation differs");
            w.Validate();Console.WriteLine($"PASS proactive grove relaxed={relaxed}: actual planting with {timber} logs already stored, unchanged reserves, exact continuation.");
        }
    }
}

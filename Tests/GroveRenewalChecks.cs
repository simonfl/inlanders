using Inlanders.Simulation;
static class GroveRenewalChecks
{
    public static void Run()
    {
        var chosen=World.NewRiverLivelihood();var kept=chosen.Trees[0];var working=chosen.Trees[1];
        if(!chosen.SetHarvestGrove(working.Cell) || working.Preserved || !kept.Preserved || !chosen.ManagedWoodland.Contains(working.Cell))throw new Exception("Grove intent did not release only chosen timber");
        string before=chosen.SaveJson();
        if(chosen.SetHarvestGrove(chosen.Cottages[0].Cell) || chosen.SaveJson()!=before)throw new Exception("Rejected grove changed land");
        chosen.SetTreePreserved(working.Cell,true);
        if(!working.Preserved || !chosen.ManagedWoodland.Contains(working.Cell))throw new Exception("Preserve cannot override managed harvest");
        chosen.Validate();
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

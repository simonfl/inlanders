using Inlanders.Simulation;
static class GrainReserveChecks
{
    public static void Run()
    {
        var w=World.NewRiverLivelihood(true);
        Cottage Place(BuildingKind kind){var p=ReviewPlacement.Find(w,kind,new(0,0),(_,_)=>true,"grain")!;return w.Place(p.Actual,p.Rotation,kind)!;}
        var farm=Place(BuildingKind.Farm);
        for(int i=0;i<1800;i++)w.Tick(.1f);
        if(w.Food.GrownGrain!=6 || w.StoredGrain!=6)throw new Exception($"Unbounded grain: {w.Food.GrownGrain}/{w.StoredGrain}");
        var bakery=Place(BuildingKind.Bakery);
        for(int i=0;i<1800 && w.Food.BakedBread==0;i++)w.Tick(.1f);
        if(w.Food.BakedBread==0)throw new Exception("Grain did not supply real bread");
        w.SetOutputTarget(farm.Id,0);if(w.EffectiveOutputTarget(farm)!=0)throw new Exception("Manual grain cap lost");
        w.UseAutomaticOutputTarget(farm.Id);if(w.EffectiveOutputTarget(farm)!=w.GrainReserveTarget)throw new Exception("Auto grain restore");
        var copy=World.LoadJson(w.SaveJson());for(int i=0;i<30;i++){w.Tick(.1f);copy.Tick(.1f);}if(w.SaveJson()!=copy.SaveJson())throw new Exception("Grain continuation differs");
        w.Validate();Console.WriteLine("PASS grain buffers one crop without an oven; oven makes actual bread; manual/automatic cap and exact saved continuation.");
    }
}

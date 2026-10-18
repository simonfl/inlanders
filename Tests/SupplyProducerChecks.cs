using Inlanders.Simulation;
static class SupplyProducerChecks
{
    public static void Run()
    {
        var w=World.NewRiverLivelihood(true);
        var oven=w.Place(new(0,0),0,BuildingKind.Bakery)!;
        int Cost(Cottage c)=>w.PathConnection(c.Entrance,oven.Entrance,out var route)==null?route.Skip(1).Sum(s=>w.Paths.Contains(s)?4:5):int.MaxValue;
        var far=w.Place(new(0,-7),0,BuildingKind.Farm)!;
        var cell=w.Map.Land.Where(c=>w.PlacementProblem(c,0,BuildingKind.Farm)==null)
            .OrderBy(c=>(c.Point-oven.Entrance.Point).LengthSquared()).First();
        var near=w.Place(cell,0,BuildingKind.Farm)!;
        if(Cost(near)>=Cost(far))throw new Exception($"Not a locality counterexample: {cell}, {Cost(near)}/{Cost(far)}");
        var legacy=w.Cottages.First(c=>c.Kind==BuildingKind.Farm);
        if(legacy.Id!=far.Id)throw new Exception("Legacy fixture differs");
        string before=w.SaveJson();
        if(w.ReadSupplyProducer(oven.Id,Resource.Grain)?.Id!=near.Id)throw new Exception("Supply action prefers older distant field");
        if(w.SaveJson()!=before)throw new Exception("Inspection mutated simulation");
        w.SetWorkplacePaused(near.Id,true);
        if(w.ReadSupplyProducer(oven.Id,Resource.Grain)?.Id!=far.Id)throw new Exception("Active supply ignored");
        w.SetWorkplacePaused(far.Id,true);
        if(w.ReadSupplyProducer(oven.Id,Resource.Grain)?.Id!=near.Id)throw new Exception("No honest paused fallback");
        if(w.ReadSupplyProducer(-1,Resource.Grain)!=null || w.ReadSupplyProducer(oven.Id,Resource.Logs)!=null)throw new Exception("Invented supply");
        w.Validate();Console.WriteLine($"PASS supply navigation: older {far.Cell} cost {Cost(far)} → {near.Cell} cost {Cost(near)}; active/paused fallback and pure reads.");
    }
}

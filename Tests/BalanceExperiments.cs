using Inlanders.Simulation;

public static class BalanceExperiments
{
    static Cottage Place(World w, Cell cell, BuildingKind kind) => w.Place(cell, false, kind) ?? throw new Exception($"Balance fixture rejected {kind} {cell}: {w.PlacementProblem(cell, false, kind)}");
    static void Tick(World w) { w.Tick(.1f); }
    public static void Run()
    {
        CampaignChecks.Run();
        foreach (bool lodges in new[] { false, true })
        {
            var w = new World(); w.Food.InitialBerries = w.Food.Berries = 1000;
            if (lodges)
            {
                Place(w, new(3, 0), BuildingKind.Sawmill); Place(w, new(6, 0), BuildingKind.Lodge); Place(w, new(3, 6), BuildingKind.Lodge);
                w.Assign(0, Role.Sawyer);
            }
            else foreach (var c in new[] { new Cell(3, 0), new(6, 0), new(3, 6), new(-5, 6) }) Place(w, c, BuildingKind.Cottage);
            float builder = 0, sawyer = 0;
            for (int i = 0; i < 12000 && w.Beds < 8; i++)
            {
                builder += .1f * w.People.Count(p => p.Role == Role.Builder && p.Task != Work.Waiting);
                sawyer += .1f * w.People.Count(p => p.Role == Role.Sawyer && p.Task != Work.Waiting); Tick(w);
            }
            w.Validate(); if (w.Beds != 8) throw new Exception("Housing investment stalled");
            Console.WriteLine($"HOUSING {(lodges ? "mill+2lodges" : "4cottages")}: ready={w.Food.Time:F1}s committedRawLogs={w.Cottages.Where(c=>c.Material==Resource.Logs).Sum(c=>c.Delivered)+w.SawnLogs} sparePlanks={w.Planks+w.Cottages.Sum(c=>c.OutputPlanks)+w.People.Where(p=>p.Cargo==Resource.Planks).Sum(p=>p.Carried)} tiles={w.Cottages.Sum(c=>World.Footprint(c.Cell,c.Rotation,c.Kind).Count())} builderActive={builder:F1}s sawyerActive={sawyer:F1}s");
        }
        foreach (string food in new[] { "berries", "gardens", "gardens3", "bread", "bread2fields", "bread-near", "gardens-near", "bread2fields-near", "gardens3-near" })
        {
            var w = World.NewCampaign(2);
            foreach (var p in w.People) w.Assign(p.Id, Role.Unassigned);
            w.Assign(0, Role.Logger); w.Assign(1, Role.Logger); w.Assign(2, Role.Builder); w.Assign(3, Role.Builder);
            if (food == "berries") { w.Assign(4, Role.Forager); w.Assign(5, Role.Forager); }
            else
            {
                Place(w, new(3, -3), food.StartsWith("bread") ? BuildingKind.Farm : BuildingKind.VegetableGarden);
                Place(w, food.EndsWith("near") ? new(-4, 2) : new(6, -3), food.StartsWith("bread") ? BuildingKind.Bakery : BuildingKind.VegetableGarden);
                if (food.StartsWith("gardens3") || food.StartsWith("bread2fields")) Place(w, new(0, 6), food.StartsWith("bread") ? BuildingKind.Farm : BuildingKind.VegetableGarden);
                w.Assign(4, Role.Farmer); w.Assign(5, food.StartsWith("bread") ? Role.Baker : Role.Farmer);
            }
            int Delivered() => w.DeliveredBerries + w.DeliveredVegetables + w.DeliveredBread;
            int Reserve() => w.Food.Berries + w.Food.Vegetables + w.Food.Bread;
            float first = -1, hungry = 0; int atFive = 0, deliveredFive = 0, min = Reserve();
            var bakerTime = new Dictionary<Work, int>();
            int missingGrain = 0;
            for (int i = 0; i < 9000; i++)
            {
                if (i >= 3000 && food.StartsWith("bread"))
                {
                    var baker = w.People[5];
                    bakerTime[baker.Task] = bakerTime.GetValueOrDefault(baker.Task) + 1;
                    if (baker.Task == Work.Waiting && w.Food.Grain - w.ReservedGrain < 2) missingGrain++;
                }
                Tick(w); if (first < 0 && Delivered() > 0) first = w.Food.Time;
                if (w.Food.Hunger > 0) hungry += .1f; min = Math.Min(min, Reserve());
                if (i == 2999) { atFive = Reserve(); deliveredFive = Delivered(); }
            }
            w.Validate();
            Console.WriteLine($"FOOD {food}: first={first:F1}s delivered={Delivered()} reserve300={atFive} reserve900={Reserve()} minReserve={min} hungry={hungry:F1}s (2 food workers, existing homes/hut, 96 starting food)");
            Console.WriteLine($"  minutes5-15 delivered={Delivered()-deliveredFive} bakerMissingGrain={missingGrain/10f:F1}s " + string.Join(" ", bakerTime.Select(kv => $"{kv.Key}={kv.Value/10f:F1}s")));
        }
        foreach (bool far in new[] { false, true }) foreach (int storage in new[] { 0, 1, 2 })
        {
            var w = World.NewLargeMap(false, false); w.Food.InitialBerries = w.Food.Berries = 1000;
            foreach (var p in w.People) w.Assign(p.Id, Role.Unassigned);
            w.Assign(0, Role.Logger); w.Assign(1, Role.Logger); w.Assign(2, Role.Builder); w.Assign(3, Role.Builder);
            if (storage > 0) Place(w, far ? new(9, 2) : new(0, 0), BuildingKind.Stockpile);
            if (storage == 2) w.Assign(4, Role.Hauler);
            foreach (var c in far ? new[] { new Cell(9, 5), new(12, 9), new(5, 10) } : new[] { new Cell(3, 0), new(6, 0), new(3, 6) }) Place(w, c, BuildingKind.Cottage);
            float travel = 0, hauler = 0;
            for (int i = 0; i < 12000 && w.Beds < 6; i++)
            {
                var before = w.People.Select(p => p.Position).ToArray(); Tick(w);
                foreach (var p in w.People) if (p.Role == Role.Builder) travel += (p.Position - before[p.Id]).Length();
                hauler += .1f * w.People.Count(p => p.Role == Role.Hauler && p.Task != Work.Waiting);
            }
            w.Validate(); if (w.Beds != 6) throw new Exception("Logistics investment stalled");
            Console.WriteLine($"STORAGE {(far ? "far" : "near")} {(storage==0 ? "yard-only" : storage==1 ? "pile-no-hauler" : "pile+hauler")}: ready={w.Food.Time:F1}s builderTravel={travel:F1} haulerActive={hauler:F1}s (includes stockpile construction)");
        }
        RemoteNeighborhood(false);
        RemoteNeighborhood(true);
    }

    private static void RemoteNeighborhood(bool woodland)
    {
        // Choose the same legal neighborhood for every variant, reserving the depot footprint too.
        var layout = World.NewLargeMap(false, false);
        var depotCell = woodland ? new Cell(-9, -3) : new Cell(9, 2);
        Place(layout, depotCell, BuildingKind.Stockpile);
        var homes = new List<Cell>();
        foreach (int z in woodland ? new[] { -5, -8, -11, -14 } : new[] { 5, 8, 11, 14 })
        foreach (int x in woodland ? new[] { -12, -9, -6, -3, 0, 3, 6 } : new[] { 12, 9, 6, 3, 0 })
        {
            var cell = new Cell(x, z);
            if (homes.Count < 12 && layout.Place(cell) != null) homes.Add(cell);
        }
        if (homes.Count != 12) throw new Exception($"Remote fixture fits only {homes.Count} homes");
        Console.WriteLine($"REMOTE {(woodland ? "woodland" : "east")} home cells: " + string.Join("; ", homes));
        foreach (int storage in new[] { 0, 1, 2 })
        {
            var w = World.NewLargeMap(false, false); w.Food.InitialBerries = w.Food.Berries = 1000;
            foreach (var p in w.People) w.Assign(p.Id, Role.Unassigned);
            w.Assign(0, Role.Logger); w.Assign(1, Role.Logger); w.Assign(2, Role.Builder); w.Assign(3, Role.Builder);
            if (storage > 0) Place(w, depotCell, BuildingKind.Stockpile);
            if (storage == 2) w.Assign(4, Role.Hauler);
            double logger = 0, builder = 0, hauler = 0;
            // Expand in three waves. Include all work from starting the depot through the twelfth home.
            for (int wave = 0; wave < 3; wave++)
            {
                var pending = homes.Skip(wave * 4).Take(4).ToList();
                for (int i = 0; i < 18000 && w.Beds < (wave + 1) * 8; i++)
                {
                    // Placement protects moving workers; retry occupied footprints as a player would.
                    pending.RemoveAll(cell => w.Place(cell) != null);
                    // Stop harvesting at equal useful output: construction plus twelve spare logs.
                    // Returned cargo still counts as labor, even after the logger is reassigned.
                    int timberGoal = 12 * Buildings.Get(BuildingKind.Cottage).Cost + 12 + (storage > 0 ? Buildings.Get(BuildingKind.Stockpile).Cost : 0);
                    if (w.InitialLogs - w.Trees.Sum(t => t.Logs) >= timberGoal)
                        foreach (var p in w.People.Take(2).Where(p => p.Role == Role.Logger)) w.Assign(p.Id, Role.Unassigned);
                    logger += .1 * w.People.Take(2).Count(p => p.Task != Work.Waiting);
                    builder += .1 * w.People.Count(p => p.Role == Role.Builder && p.Task != Work.Waiting);
                    hauler += .1 * w.People.Count(p => p.Role == Role.Hauler && p.Task != Work.Waiting);
                    Tick(w);
                }
                w.Validate(); if (w.Beds != (wave + 1) * 8) throw new Exception("Remote expansion stalled");
                Console.WriteLine($"REMOTE {(woodland ? "woodland" : "east")} storage={storage} homes={(wave+1)*4} ready={w.Food.Time:F1}s logger={logger:F1}s builder={builder:F1}s hauler={hauler:F1}s totalActive={logger+builder+hauler:F1}s storedLogs={w.Stored} cargoLogs={w.People.Where(p=>p.Cargo==Resource.Logs).Sum(p=>p.Carried)}");
            }
        }
    }
}

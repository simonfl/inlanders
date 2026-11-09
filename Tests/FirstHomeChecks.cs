using Inlanders.Simulation;

static class FirstHomeChecks
{
    public static void Run()
    {
        Directory.CreateDirectory("artifacts/first-home");
        foreach (var kind in new[] { BuildingKind.ForagerHut, BuildingKind.VegetableGarden })
        {
            var world = World.NewFirstHome();
            if(world.ReadFirstHomeObjectives() is not { Action: "home", Done: 0 }) throw new Exception("Opening objective is not actionable");
            world.SaveFile("artifacts/first-home/start.json");
            if (world.Population != 4 || world.PublicPlace?.FirstHome != true || world.InviteNewcomers())
                throw new Exception("Commission identity or population changed");
            var home = ReviewPlacement.Find(world, BuildingKind.Cottage, new(1,1), (cell,rotation) => true, "home")!;
            var cancelled = world.Place(home.Actual, home.Rotation, BuildingKind.Cottage)!;
            if (!world.Cancel(cancelled.Id)) throw new Exception("Plan cannot be cancelled");
            if(world.ReadFirstHomeObjectives().Action != "home") throw new Exception("Cancelled home leaves stale guidance");
            world.Place(home.Actual, home.Rotation, BuildingKind.Cottage);
            if(world.ReadFirstHomeObjectives().Action != "grove") throw new Exception("Timber blocker not explained");
            world.SaveFile("artifacts/first-home/timber-needed.json");
            var food = ReviewPlacement.Find(world, kind, new(-5,3), (cell,rotation) => true, "livelihood")!;
            world.Place(food.Actual, food.Rotation, kind);
            // Preserved trees keep the no-order branch genuinely inactive.
            for (int i = 0; i < 100; i++) world.Tick(.1f);
            if (world.Founding!.FirstHome!.Complete || world.Cottages.Any(c => c.Id != 1 && c.Complete))
                throw new Exception("Unordered timber bypassed the opening");
            world.ApplyWoodlandCare(new(-10,-9), new(-2,-4), WoodlandIntent.Renew, false);
            for (int i = 0; i < 4500 && !world.Founding.FirstHome.Complete; i++) world.Tick(.1f);
            world.Validate();
            string beforeRead = world.SaveJson();
            if(world.ReadFirstHomeObjectives() is not { Done: 3, Action: "done" } || world.SaveJson() != beforeRead)
                throw new Exception("Completed goals missing or progress query mutated world");
            if (!world.Founding.FirstHome.Complete || world.ReadFoodFlow().Delivered == 0 || world.Housed != 4)
                throw new Exception($"First home never completed for {kind}: {world.Food.Time}s, {world.Housed} housed, {world.ReadFoodFlow().Delivered} delivered");
            var copy = World.LoadJson(world.SaveJson());
            for (int i = 0; i < 50; i++) { world.Tick(.1f); copy.Tick(.1f); }
            if (world.SaveJson() != copy.SaveJson()) throw new Exception("Commission save differs");
            world.SaveFile($"artifacts/first-home/{kind}-complete.json");
            Console.WriteLine($"PASS {kind}: actual timber, home, food and household meals at {world.Food.Time:0}s, cancellation and exact save");
        }
    }
}

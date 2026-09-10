using Inlanders.Simulation;

public static class CreativeChecks
{
    static void Check(bool ok, string message) { if (!ok) throw new Exception(message); }
    static void Step(World w, int count) { for (int i = 0; i < count; i++) { w.Tick(.1f); w.Validate(); } }
    public static void Run()
    {
        foreach (var kind in Enum.GetValues<BuildingKind>().Where(k => k != BuildingKind.Bridge))
        {
            var w = World.NewCreative();
            if(kind==BuildingKind.Quarry) w.Map.StoneDeposits.Add(new() { Id=0,Cell=new(5,-2),Capacity=16,Remaining=16 });
            if(kind==BuildingKind.FishingDock)
            {
                w.Map.Water.Add(new(3,-1));
                w.Map.FishingGrounds.Add(new FishHabitat { Id=0,Cell=new(3,-1) });
            }
            w.Food.InitialBerries = w.Food.Berries = 0;
            var site = w.Place(new(3, 0), false, kind)!;
            Check(site != null && site.Complete && site.Delivered == 0 && site.DeliveredStone==0 && w.Stone==0 && w.Stored == 0 && w.Planks == 0, $"Free placement failed: {kind}");
            Check(w.Place(new(3, 0)) == null, "Creative bypassed collisions");
            Check(w.RemoveBuilding(site!.Id), "Creative removal rejected");
            w.Validate();
            Check(w.Stored == 0 && w.Planks == 0, "Free building created demolition materials");
            Step(w, 1900);
            Check(w.Food.WorkEfficiency == 1 && w.Food.Hunger == 0 && w.Food.EatenBerries == 0 && !w.CanCelebrate, "Creative still needs food");
            Check(w.ReadEconomy().Issues.All(i => i.Id != "food-low") && w.Gardener == VisitorState.NotArrived, "Creative economy has irrelevant warnings");
            Check(w.SunflowersUnlocked && w.ReadHappiness(w.People[0]).Reasons.Contains("disabled"), "Creative needs feedback missing");
            Check(World.LoadJson(w.SaveJson()).SaveJson() == w.SaveJson(), "Creative save roundtrip changed");
        }
        var arrival = World.NewCreative();
        arrival.Food.InitialBerries = arrival.Food.Berries = 0;
        foreach (var cell in new[] { new Cell(3,0), new(6,0), new(3,6) })
            Check(arrival.Place(cell, false, BuildingKind.Lodge) != null, "Arrival lodge rejected");
        Check(arrival.InviteNewcomers(), "Creative invitations require food");
        arrival.Validate();

        var bridgeWorld = World.NewCreative(true);
        for (int z = bridgeWorld.Map.MinZ; z <= bridgeWorld.Map.MaxZ; z++)
            if (bridgeWorld.Map.Contains(new(7,z))) bridgeWorld.Map.Water.Add(new(7,z));
        var bridge = bridgeWorld.Place(new(7,3), true, BuildingKind.Bridge)!;
        Check(bridge.Complete && !bridgeWorld.RemoveBuilding(bridge.Id), "Sole crossing removed despite reachable far-bank resources");
        Check(bridgeWorld.Place(new(7,4), true, BuildingKind.Bridge) != null && bridgeWorld.RemoveBuilding(bridge.Id), "Alternate crossing did not permit removal");
        World.LoadJson(bridgeWorld.SaveJson());

        foreach (var kind in new[] { BuildingKind.ForagerHut, BuildingKind.Farm, BuildingKind.VegetableGarden, BuildingKind.Bakery, BuildingKind.Sawmill, BuildingKind.Stockpile, BuildingKind.Square })
        for (int seconds = 1; seconds <= 80; seconds += 13)
        {
            var w = World.NewCreative();
            w.Place(new(3,0), false, BuildingKind.Farm);
            var target = w.Place(new(6,0), true, kind)!;
            Check(target != null, "Removal fixture rejected");
            var roles = new[] { Role.Logger, Role.Logger, Role.Farmer, Role.Farmer, Role.Forager, Role.Baker, Role.Sawyer, Role.Hauler };
            for (int i = 0; i < roles.Length; i++) w.Assign(i, roles[i]);
            Step(w, seconds * 10);
            var cargo = w.People.Select(p => p.Carried).ToArray();
            Check(w.RemoveBuilding(target!.Id), "Active workplace removal rejected");
            Check(w.People.Select(p => p.Carried).SequenceEqual(cargo), "Removal lost carried goods");
            w.Validate();
            w = World.LoadJson(w.SaveJson());
            Step(w, 400);
            var tree = w.Trees.FirstOrDefault(t => t.Owner != null) ?? w.Trees.First();
            Check(w.SetClearing(tree.Cell, true) && !w.Trees.Contains(tree), "Immediate clearing failed");
            Step(w, 100);
        }
        Check(!World.NewScenario().RemoveBuilding(1), "Normal mode allowed demolition");
        Console.WriteLine("PASS: Creative free placement, foodless days, invitations, exact saves, bridge removal safety, active workplace cargo and immediate clearing.");
    }
}

using Inlanders.Simulation;
using System.Text.Json.Nodes;

public static class FoodChecks
{
    static void Check(bool value, string message) { if (!value) throw new Exception(message); }
    static void Step(World w, int count = 1) { for (int i = 0; i < count; i++) { w.Tick(0.1f); w.Validate(); } }
    static void Until(World w, Func<bool> condition, string error, int max = 20000)
    {
        for (int i = 0; i < max && !condition(); i++) Step(w);
        Check(condition(), error);
    }
    public static World Scenario()
    {
        var w = World.NewScenario();
        foreach (var (cell, kind, rotated) in new[] {
            (new Cell(0,0), BuildingKind.ForagerHut, false), (new Cell(3,-3), BuildingKind.Farm, false), (new Cell(6,-3), BuildingKind.Bakery, false),
            (new Cell(3,0), BuildingKind.Cottage, false), (new Cell(6,0), BuildingKind.Cottage, true),
            (new Cell(3,6), BuildingKind.Cottage, false), (new Cell(-5,6), BuildingKind.Cottage, false) })
            Check(w.Place(cell, rotated, kind) != null, $"Food fixture rejected {cell}/{kind}");
        return w;
    }
    public static void Run()
    {
        var w = Scenario();
        var phases = new[] { Work.ToBush, Work.Foraging, Work.ToFarm, Work.Planting, Work.Harvesting, Work.ToGrain, Work.ToOven, Work.Baking, Work.ToBread, Work.ToPantry };
        var snapshots = new Dictionary<Work, string>();
        string? fullBasket = null;
        string? fullGrain = null;
        for (int tick = 0; tick < 20000 && !w.CanCelebrate; tick++)
        {
            Step(w);
            if (fullBasket == null && w.People.Any(v => v.Task == Work.ToPantry && v.Cargo == Resource.Bread && v.Carried == 4)) fullBasket = w.SaveJson();
            if (fullGrain == null && w.People.Any(v => v.Task == Work.ToPantry && v.Cargo == Resource.Grain && v.Carried == 4)) fullGrain = w.SaveJson();
            foreach (var phase in phases)
                if (!snapshots.ContainsKey(phase) && w.People.Any(v => v.Task == phase)) snapshots[phase] = w.SaveJson();
        }
        Check(w.CanCelebrate, $"Supper unreachable: day {w.Food.Day}, bread {w.Food.Bread}, grain {w.Food.Grain}");
        Check(w.Housed == 8 && w.Food.GatheredBerries > 0 && w.Food.GrownGrain >= 8 && w.Food.BakedBread >= 16 && w.Food.EatenBerries > 0 && w.Food.EatenBread > 0 && w.Food.EatenBerries + w.Food.EatenBread >= 32, $"Food chain did not sustain the scenario: housed {w.Housed}, gathered {w.Food.GatheredBerries}, grain {w.Food.GrownGrain}, baked {w.Food.BakedBread}, eaten berries {w.Food.EatenBerries}, eaten bread {w.Food.EatenBread}");
        Check(phases.All(snapshots.ContainsKey), "Missing production phase");
        Console.WriteLine($"PASS: complete food economy reaches supper on day {w.Food.Day}; berries regrow, crops ripen, grain is hauled/baked, and meals are consumed.");
        foreach (var (phase, json) in snapshots)
        {
            var a = World.LoadJson(json); var b = World.LoadJson(a.SaveJson());
            Check(a.SaveJson() == json, $"Save lost state at {phase}");
            Step(a, 200); Step(b, 200); Check(a.SaveJson() == b.SaveJson(), $"Load diverged at {phase}");
            var interrupted = World.LoadJson(json); var worker = interrupted.People.First(v => v.Task == phase);
            var role = worker.Role; int cargo = worker.Carried;
            interrupted.Assign(worker.Id, Role.Unassigned); interrupted.Validate();
            Check(worker.WorkplaceId == null && worker.BushId == null && worker.FoodReserved == 0, $"Leaked food reservation at {phase}");
            if (cargo > 0) Check(worker.Carried == cargo && worker.Task == Work.ToPantry, "Food cargo teleported on reassignment");
            Until(interrupted, () => worker.Carried == 0, "Returned food never reached pantry");
            interrupted.Assign(worker.Id, role); Step(interrupted, 500);
        }
        Console.WriteLine("PASS: exact save/load continuation and safe reassignment in all ten food work phases.");
        Check(fullBasket != null, "Baker never carried the full four-loaf batch");
        var batch = World.LoadJson(fullBasket!); var continued = World.LoadJson(fullBasket!);
        Check(batch.SaveJson() == fullBasket, "Four-loaf basket changed on load");
        Step(batch, 800); Step(continued, 800);
        Check(batch.SaveJson() == continued.SaveJson(), "Full-batch delivery continuation diverged");
        var returned = World.LoadJson(fullBasket!);
        var carrier = returned.People.Single(v => v.Cargo == Resource.Bread && v.Carried == 4);
        int deliveredBefore = returned.DeliveredBread;
        returned.Assign(carrier.Id, Role.Unassigned); returned.Validate();
        Check(carrier.Carried == 4 && carrier.Task == Work.ToPantry && returned.DeliveredBread == deliveredBefore, "Reassignment teleported a bread batch");
        Until(returned, () => carrier.Carried == 0, "Four-loaf basket was stranded");
        Check(returned.DeliveredBread == deliveredBefore + 4, "Returned batch lost or duplicated bread");
        Console.WriteLine("PASS: full four-loaf basket, exact continuation, physical return after reassignment and bread conservation.");
        Check(fullGrain != null, "Farmer never carried four grain");
        var grainLoad = World.LoadJson(fullGrain!); var grainClone = World.LoadJson(fullGrain!);
        Check(grainLoad.SaveJson() == fullGrain, "Grain load changed on save/load");
        Step(grainLoad, 800); Step(grainClone, 800);
        Check(grainLoad.SaveJson() == grainClone.SaveJson(), "Four-grain delivery continuation diverged");
        grainLoad = World.LoadJson(fullGrain!);
        var farmer = grainLoad.People.Single(v => v.Cargo == Resource.Grain && v.Carried == 4);
        grainLoad.Assign(farmer.Id, Role.Unassigned); grainLoad.Validate();
        Check(farmer.Carried == 4 && farmer.Task == Work.ToPantry, "Reassignment lost the grain load");
        Until(grainLoad, () => farmer.Carried == 0, "Four-grain load was stranded");
        Check(grainLoad.Food.Grain >= 4, "Grain did not reach the pantry");
        Console.WriteLine("PASS: four-grain harvest load, exact continuation and physical return with grain conservation.");
        Check(!World.NewScenario().BeginSupper(), "Unqualified settlement hosted supper");
        int before = w.Food.Bread; Check(w.BeginSupper(), "Supper failed to start");
        Check(w.Food.Bread == before - 16 && !w.BeginSupper(), "Supper charged incorrectly or started twice");
        Until(w, () => w.People.Any(v => v.Task == Work.ToSupper), "Nobody joined supper");
        w = World.LoadJson(w.SaveJson());
        Until(w, () => w.Food.SupperComplete, "Gathering did not finish after load");
        Check(w.Food.SupperBread == 16 && !w.BeginSupper(), "Celebration repeated"); Step(w, 200);
        Console.WriteLine("PASS: qualifying objective, physical gathering, one-time bread cost, save during gathering, and continued play after supper.");

        var hungry = World.NewScenario(); hungry.Food.Berries = 0; hungry.Food.EatenBerries = 24;
        hungry.Food.Grain = 10; hungry.Food.GrownGrain = 10; // Accounting-balanced fixture: raw grain is not edible.
        Step(hungry, 610); Check(hungry.Food.Hunger == 1 && hungry.Food.WorkEfficiency == 0.5f && hungry.Food.Grain == 10, "Hunger/inedible grain rule failed");
        Check(hungry.Place(new(0,0), false, BuildingKind.ForagerHut) != null, "Recovery hut rejected");
        Until(hungry, () => hungry.Food.EatenBerries > 24 && hungry.Food.Hunger < 1, "Hungry settlement could not recover");
        Check(hungry.People.Count == 8, "Hunger killed a villager");
        Console.WriteLine("PASS: shortages halve work speed without deaths, raw grain is inedible, and foraging restores meals.");

        var stock = World.LoadJson(snapshots[Work.ToGrain]);
        var node = JsonNode.Parse(stock.SaveJson())!; node["Version"] = 999;
        bool refused = false; try { World.LoadJson(node.ToJsonString()); } catch (InvalidDataException) { refused = true; }
        Check(refused, "Unsupported save accepted");
        node["Version"] = 27; node["Food"]!["Bread"] = -1;
        refused = false; try { World.LoadJson(node.ToJsonString()); } catch (InvalidOperationException) { refused = true; }
        Check(refused, "Corrupt inventory save accepted");
        string path = Path.Combine(Path.GetTempPath(), "inlanders-save-" + Guid.NewGuid() + ".json");
        try
        {
            stock.SaveFile(path); string first = File.ReadAllText(path); Step(stock, 10); stock.SaveFile(path);
            Check(File.ReadAllText(path + ".bak") == first && World.LoadFile(path).SaveJson() == stock.SaveJson(), "File replacement/backup failed");
        }
        finally { foreach (string suffix in new[] { "", ".bak", ".tmp" }) if (File.Exists(path + suffix)) File.Delete(path + suffix); }
        Console.WriteLine("PASS: version/corruption rejection, disk save/load, and previous-save backup.");
    }
}

using Inlanders.Simulation;
using System.Text.Json.Nodes;

public static class ProductionChecks
{
    static void Check(bool ok, string reason) { if (!ok) throw new Exception(reason); }
    static void Step(World w, int count = 1) { for (int i = 0; i < count; i++) { w.Tick(.1f); w.Validate(); } }
    static void Until(World w, Func<bool> done, string reason) { for (int i = 0; i < 8000 && !done(); i++) Step(w); Check(done(), reason); }
    static World Empty()
    {
        var w = World.NewCreative(); foreach (var p in w.People) w.Assign(p.Id, Role.Unassigned); return w;
    }
    static Cottage Place(World w, Cell cell, BuildingKind kind) => w.Place(cell, false, kind) ?? throw new Exception("Production fixture blocked");
    public static void Run()
    {
        var w = Empty(); var field = Place(w, new(3,-3), BuildingKind.Farm); var garden = Place(w, new(6,-3), BuildingKind.VegetableGarden);
        Check(w.ReadWorkplace(field).State == "No staff", "Unstaffed field not explained");
        w.Assign(0, Role.Farmer);
        Until(w, () => w.People[0].Cargo == Resource.Grain && w.People[0].Carried == 4, "No grain delivery");
        Check(w.ReadWorkplace(field).State == "Delivering output", "Grain travel not diagnosed");
        Check(w.SetWorkplacePaused(field.Id, true), "Pause rejected");
        Check(w.People[0].Carried == 4 && w.People[0].Role == Role.Farmer && w.ReadWorkplace(field).State.StartsWith("Pausing"), "Pause changed cargo or role");
        var clone = World.LoadJson(w.SaveJson()); Step(w, 300); Step(clone, 300);
        Check(w.SaveJson() == clone.SaveJson(), "Paused delivery save diverged");
        Until(w, () => w.DeliveredVegetables >= 8, "Pausing grain stopped the garden");
        Check(field.Harvest == 2 && w.Food.Grain == 4 && w.People[0].Role == Role.Farmer, "Paused crop was harvested or lost");
        Check(w.ReadWorkplace(field).State == "Paused", "Completed pause not explained");
        w.SetOutputTarget(field.Id, 0); w.SetWorkplacePaused(field.Id, false);
        Until(w, () => w.Food.Grain == 6, "Resume did not collect remaining grain above target");
        Step(w, 1000); Check(w.Food.GrownGrain == 6 && w.ReadWorkplace(field).State == "Target met", "Zero target started another grain crop");

        // One shared four-loaf commitment: two bakeries must not both reserve a fresh batch.
        w = Empty(); var first = Place(w, new(3,-3), BuildingKind.Bakery); var second = Place(w, new(6,-3), BuildingKind.Bakery);
        w.Assign(0, Role.Baker); w.Assign(1, Role.Baker);
        Check(w.ReadWorkplace(first).State == "Missing grain", "Input shortage not diagnosed");
        w.Food.Grain = w.Food.GrownGrain = 20;
        w.SetOutputTarget(first.Id, 4); w.SetOutputTarget(second.Id, 4);
        var phases = new Dictionary<Work, string>();
        for (int i = 0; i < 3000 && w.DeliveredBread < 4; i++)
        {
            Step(w); Check(w.ProductionCommitted(Resource.Bread) <= 4, "Concurrent batches exceeded a shared target");
            foreach (var p in w.People.Where(p => p.WorkplaceId != null)) phases.TryAdd(p.Task, w.SaveJson());
        }
        Check(w.DeliveredBread == 4 && w.Food.UsedGrain == 2, "Targeted bakery did not deliver one batch");
        Step(w, 600); Check(w.Food.BakedBread == 4, "Target met kept baking");
        foreach (var phase in new[] { Work.ToGrain, Work.ToOven, Work.Baking, Work.ToBread, Work.ToPantry })
        {
            Check(phases.ContainsKey(phase), "Missed bakery phase " + phase);
            var paused = World.LoadJson(phases[phase]); var worker = paused.People.First(p => p.Task == phase);
            int workplace = worker.WorkplaceId!.Value;
            paused.SetWorkplacePaused(workplace, true);
            string saved = paused.SaveJson(); var replay = World.LoadJson(saved);
            Check(replay.SaveJson() == saved, "Paused batch state changed on load");
            Step(paused, 1000); Step(replay, 1000);
            Check(paused.SaveJson() == replay.SaveJson() && paused.DeliveredBread == 4, "Paused batch failed to finish exactly once at " + phase);
        }
        Check(!w.SetOutputTarget(first.Id, 201) && !w.SetOutputTarget(first.Id, -2) && !w.SetWorkplacePaused(999, true), "Invalid control accepted");
        Check(w.ReadFoodFlow().Delivered == 4 && w.ReadFoodFlow().Eaten == 0, "Flow counted baking instead of pantry arrivals");
        w.SetOutputTarget(first.Id, 5); // A whole four-loaf batch may cross an odd threshold.
        Until(w, () => w.DeliveredBread == 8, "Target did not restart production");
        Step(w, 500); Check(w.DeliveredBread == 8, "Overshoot exceeded one batch");

        // Growing crops count before another farmer claims a new sowing job.
        w = Empty(); first = Place(w, new(3,-3), BuildingKind.Farm); second = Place(w, new(6,-3), BuildingKind.Farm);
        w.SetOutputTarget(first.Id, 6); w.SetOutputTarget(second.Id, 6);
        w.Assign(0, Role.Farmer); w.Assign(1, Role.Farmer);
        Until(w, () => w.Food.Grain == 6, "Committed crop was not collected");
        Step(w, 1000); Check(w.Food.GrownGrain == 6, "Multiple farmers duplicated target headroom");
        // Berries use picking claims too; existing starting food is part of their stock target.
        w = Empty(); first = Place(w, new(0,0), BuildingKind.ForagerHut);
        w.SetOutputTarget(first.Id, 26); w.Assign(0, Role.Forager); w.Assign(1, Role.Forager);
        Until(w, () => w.Food.Berries == 26, "Forager target not supplied");
        Step(w, 500); Check(w.Food.GatheredBerries == 2, "Foragers duplicated target headroom");

        // Pausing a mill keeps the partial batch and collects the other half after resuming.
        w = Empty(); first = Place(w, new(3,0), BuildingKind.Sawmill);
        w.SetOutputTarget(first.Id, 4); w.Assign(0, Role.Logger); w.Assign(1, Role.Sawyer);
        Until(w, () => w.People[1].Task == Work.Sawing, "No active mill batch");
        w.SetWorkplacePaused(first.Id, true);
        var millReplay = World.LoadJson(w.SaveJson()); Step(w, 500); Step(millReplay, 500);
        Check(w.SaveJson() == millReplay.SaveJson() && w.Planks == 2 && first.OutputPlanks == 2, "Paused mill lost its batch/output");
        w.SetWorkplacePaused(first.Id, false);
        Until(w, () => w.Planks == 4, "Resumed mill did not collect remaining output");
        Step(w, 500); Check(w.SawnLogs == 2, "Mill target started another batch");

        // Pausing a growing field preserves growth and leaves the ripe crop for resume.
        w = Empty(); first = Place(w, new(3,-3), BuildingKind.VegetableGarden); w.Assign(0, Role.Farmer);
        Until(w, () => first.Planted, "Garden was not planted"); w.SetWorkplacePaused(first.Id, true);
        Step(w, 700); Check(first.Harvest == 8 && w.DeliveredVegetables == 0, "Pause froze growth or collected a paused crop");
        Check(w.SetOutputTarget(first.Id, 0), "Zero target rejected"); w.SetWorkplacePaused(first.Id, false);
        Until(w, () => w.DeliveredVegetables == 8, "Existing crop stranded at zero target");
        Step(w, 700); Check(w.Food.GrownVegetables == 8, "Zero target replanted garden");

        // Actual meals, shortages, expiry, read-only history and exact restore.
        w = new World(); foreach (var p in w.People) w.Assign(p.Id, Role.Unassigned);
        Step(w, 1210); var flow = w.ReadFoodFlow();
        Check(flow.Delivered == 0 && flow.Eaten == w.Food.EatenBerries && flow.Eaten >= 16 && flow.Required == 9,
            "Initial stock counted as production, actual eating omitted, or staggered closed demand miscounted");
        string state = w.SaveJson(); Check(World.LoadJson(state).ReadFoodFlow() == flow && w.SaveJson() == state, "Reading/restoring food flow changed history");
        Step(w, 2500); flow = w.ReadFoodFlow();
        Check(flow.Required > flow.Eaten && w.RecentFood.All(e => e.Time > w.Food.Time - World.FoodFlowWindow), "Expired history retained or shortages hidden");
        var bad = JsonNode.Parse(state)!; bad["Buildings"] = new JsonArray(); bad["RecentFood"]![0]!["Eaten"] = -1;
        bool rejected = false; try { World.LoadJson(bad.ToJsonString()); } catch (InvalidOperationException) { rejected = true; }
        Check(rejected, "Invalid flow history accepted");
        bad = JsonNode.Parse(state)!; bad["Version"] = 19;
        rejected = false; try { World.LoadJson(bad.ToJsonString()); } catch (InvalidDataException) { rejected = true; }
        Check(rejected, "Old development save accepted");
        w = World.NewCampaign(2);
        var hut = w.Cottages.First(c => c.Kind == BuildingKind.ForagerHut);
        w.SetWorkplacePaused(hut.Id, true);
        foreach (var p in w.People) w.Assign(p.Id, Role.Unassigned);
        Until(w, () => w.Food.EdibleStored < w.Population * 2, "Low reserve fixture did not drain");
        Check(w.ReadEconomy().Issues.Any(i => i.Id == "food-paused" && i.Workplace == hut.Id) && !w.ReadEconomy().Issues.Any(i => i.Id == "staff-Forager"), "Paused food shortage recommended staffing instead of resume");
        Console.WriteLine("PASS: workplace pause/resume, active batch/delivery saves, shared crop/bakery/forager targets, input/travel diagnostics, actual food-flow history and old-save rejection.");
    }
}

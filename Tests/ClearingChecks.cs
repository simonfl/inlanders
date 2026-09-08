using System;
using System.Linq;
using System.Text.Json.Nodes;
using Inlanders.Simulation;

public static class ClearingChecks
{
    static void Check(bool value, string message) { if (!value) throw new Exception(message); }
    static void Step(World w, int count = 1) { for (int i = 0; i < count; i++) { w.Tick(0.1f); w.Validate(); } }
    static void Until(World w, Func<bool> done, string message)
    {
        for (int i = 0; i < 10000 && !done(); i++) Step(w);
        Check(done(), message);
    }
    public static void Run()
    {
        var w = new World(); foreach (var p in w.People) w.Assign(p.Id, Role.Unassigned);
        var target = w.Trees.Last(); var cell = target.Cell;
        Check(!w.SetClearing(w.Stockpile, true) && !w.SetClearing(w.Bushes[0].Cell, true) && !w.SetClearing(new(40, 40), true), "Non-tree clearing accepted");
        Check(w.SetClearing(cell, true), "Tree marking rejected"); Step(w, 50);
        Check(target.ClearRequested && target.Logs == 8 && !target.Felled && w.Stored == 0, "Unstaffed clearing mutated timber");
        Check(!w.CanPlace(cell, true), "Marked tree became buildable early");
        w.Assign(0, Role.Logger);
        foreach (var phase in new[] { Work.ToTree, Work.Chopping, Work.ToStockpile, Work.ToClearStump, Work.ClearingStump })
        {
            Until(w, () => w.People[0].Task == phase, "Missing clearing phase " + phase);
            Check(w.Trees.Where(t => t.Id != target.Id).All(t => t.Logs == 8), "Clear order did not take priority over ordinary logging");
            string json = w.SaveJson(); var a = World.LoadJson(json); var b = World.LoadJson(json);
            Step(a, 10); Step(b, 10); Check(a.SaveJson() == b.SaveJson(), "Clearing save continuation changed");
            var interrupted = World.LoadJson(json); interrupted.Assign(0, Role.Unassigned); interrupted.Validate();
            Check(interrupted.Trees.Single(t => t.Id == target.Id).Owner == null, "Reassignment leaked clearing ownership");
            interrupted.Assign(0, Role.Logger);
            Until(interrupted, () => interrupted.Trees.All(t => t.Id != target.Id), "Interrupted clearing never resumed");
        }
        var cancelled = World.LoadJson(w.SaveJson());
        Check(cancelled.SetClearing(cell, false), "Cancel rejected"); Step(cancelled, 100);
        Check(cancelled.Trees.Any(t => t.Id == target.Id && !t.ClearRequested && t.Felled), "Cancel removed the stump");
        Check(cancelled.PlantTree(cell) != null, "Cancelled clearing prevented replanting");
        Until(w, () => w.Trees.All(t => t.Id != target.Id), "Roots were not removed");
        Check(w.Stored == 8 && w.People.All(p => p.Carried == 0), "Timber was lost or teleported during clearing");
        Check(w.Place(cell, true) != null, "Cleared footprint not reusable for construction");
        var saplings = new World(); foreach (var p in saplings.People) saplings.Assign(p.Id, Role.Unassigned);
        var sapling = saplings.PlantTree(new(3, 0))!; saplings.Assign(0, Role.Logger);
        Until(saplings, () => saplings.People[0].Task == Work.PlantingTree, "Planting fixture failed");
        Check(saplings.SetClearing(sapling.Cell, true) && sapling.Owner == null, "Clearing did not interrupt conflicting planting");
        Check(!saplings.CanPlantTree(sapling.Cell), "Planting over a clearing order accepted");
        Until(saplings, () => !saplings.Trees.Contains(sapling), "Planting marker not cleared");
        Check(saplings.GrownLogs == 0 && saplings.Stored == 0, "Clearing immature tree created timber");
        var growing = saplings.PlantTree(new(5, 0))!;
        Until(saplings, () => !growing.NeedsPlanting, "Sapling did not grow");
        saplings.SetClearing(growing.Cell, true);
        Until(saplings, () => !saplings.Trees.Contains(growing), "Growing sapling not cleared");
        Check(saplings.GrownLogs == 0, "Growing sapling produced mature timber");
        var busy = World.NewLargeMap();
        foreach (var p in busy.People) busy.Assign(p.Id, Role.Logger);
        var targets = busy.Trees.Where(t => t.Cell.X > 8).ToArray();
        foreach (var t in targets) busy.SetClearing(t.Cell, true);
        Until(busy, () => targets.All(t => !busy.Trees.Contains(t)), "Concurrent clearing on larger map failed");
        Check(World.LoadJson(busy.SaveJson()).SaveJson() == busy.SaveJson(), "Cleared larger map did not load");
        var salvage = new World(); var plan = salvage.Place(new(3, 0))!;
        Until(salvage, () => plan.Delivered > 0, "Salvage fixture failed"); salvage.Cancel(plan.Id);
        Check(!salvage.SetClearing(salvage.Trees.First(t => t.Salvage).Cell, true), "Salvage accepted as a tree");
        var legacy = JsonNode.Parse(new World().SaveJson())!; legacy["Version"] = 5;
        foreach (var t in legacy["Trees"]!.AsArray()) t!.AsObject().Remove("ClearRequested");
        Check(World.LoadJson(legacy.ToJsonString()).Trees.All(t => !t.ClearRequested), "Legacy trees became clearing orders");
        Console.WriteLine("PASS: clearing priority, physical timber recovery, root removal, reusable land, five saved/interrupted phases, cancel/replant, saplings, and old saves.");
    }
}

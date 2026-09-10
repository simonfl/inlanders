using Inlanders.Simulation;
using System.Text.Json.Nodes;

public static class WoodlandChecks
{
    static void Check(bool value, string message) { if (!value) throw new Exception(message); }
    static void Step(World w, int count = 1) { for (int i = 0; i < count; i++) { w.Tick(0.1f); w.Validate(); } }
    static void Until(World w, Func<bool> done, string message, int max = 12000)
    {
        for (int i = 0; i < max && !done(); i++) Step(w);
        Check(done(), message);
    }
    public static void Run()
    {
        var w = new World(0);
        foreach (var v in w.People) w.Assign(v.Id, Role.Unassigned);
        foreach (var cell in new[] { new Cell(-9,0), new(8,0), w.Stockpile, w.YardAccess, w.Trees[0].Cell, w.Trees[0].Access, World.At(w.People[0]), w.Bushes[0].Cell })
            Check(w.PlantTree(cell) == null, $"Invalid planting accepted: {cell}");
        var tree = w.PlantTree(new(0,0)) ?? throw new Exception("Open-ground planting failed");
        Check(w.PlantTree(tree.Cell) == null && !w.CanPlace(tree.Cell, false), "Planting overlap accepted");
        Step(w, 2000); Check(tree.NeedsPlanting && tree.Growth == 0 && w.GrownLogs == 0, "Unstaffed planting grew timber");
        w.Assign(0, Role.Logger);
        Until(w, () => w.People[0].Task == Work.ToSapling, "Logger did not claim planting");
        foreach (var phase in new[] { Work.ToSapling, Work.PlantingTree })
        {
            Until(w, () => w.People[0].Task == phase, $"Missing {phase}");
            string json = w.SaveJson(); var copy = World.LoadJson(json);
            Check(copy.SaveJson() == json, $"Planting save lost {phase}");
            w.Assign(0, Role.Unassigned); Check(tree.Owner == null && tree.NeedsPlanting, "Interrupted planting leaked claim");
            w.Assign(0, Role.Logger);
        }
        Until(w, () => !tree.NeedsPlanting, "Logger never planted");
        w.Assign(0, Role.Unassigned);
        Step(w, 900); Check(tree.Growth > 0.49f && tree.Growth < 0.52f && tree.Logs == 0, "Sapling grew early or followed hunger slowdown");
        string growing = w.SaveJson(); var restored = World.LoadJson(growing);
        Check(restored.SaveJson() == growing, "Growth save mismatch");
        Step(w, 910); Step(restored, 910);
        Check(w.SaveJson() == restored.SaveJson() && tree.Logs == 8 && w.GrownLogs == 8, "Growth continuation or yield incorrect");
        Step(w, 200); Check(w.GrownLogs == 8, "Mature tree produced timber twice");
        w.Assign(0, Role.Logger);
        Until(w, () => w.Stored == 8, "New timber was not hauled");
        Check(tree.Felled && tree.Logs == 0, "Harvest left no reusable stump");
        int count = w.Trees.Count;
        Check(w.PlantTree(tree.Cell) == tree && w.Trees.Count == count, "Stump was not replanted in place");
        Until(w, () => w.Stored == 16, "Second timber cycle did not reach storage");
        Check(w.GrownLogs == 16, "Repeated growth accounting failed");
        var cottage = w.Place(new(3,0)) ?? throw new Exception("Expansion site rejected");
        w.Assign(1, Role.Builder); Until(w, () => cottage.Complete, "Renewed timber could not fund construction");
        Console.WriteLine("PASS: planting restrictions, logger claims/interruption, three-day growth, exact saves, two harvest cycles, and construction with renewed timber.");

        var bad = JsonNode.Parse(w.SaveJson())!; bad["Trees"]![0]!["Growth"] = -1;
        bool refused = false; try { World.LoadJson(bad.ToJsonString()); } catch (InvalidOperationException) { refused = true; }
        Check(refused, "Invalid tree growth save accepted");
        Console.WriteLine("PASS: invalid growth is rejected.");

        // Planting while workers travel must protect the next waypoint and replan later route cells.
        var moving = new World(); Step(moving, 10);
        var walker = moving.People.First(v => v.Route.Count > 0);
        Check(!moving.CanPlantTree(walker.Route.Peek()), "Planting blocked next waypoint");
        int planted = 0;
        for (int x = -8; x <= 8; x++) for (int z = -7; z <= 7; z++)
        {
            if (moving.PlantTree(new(x,z)) != null) planted++;
            moving.Validate();
        }
        Check(planted > 0, "No reachable planting sites"); Step(moving, 2500);
        Console.WriteLine("PASS: dense live planting preserves worker routes and resource access.");
    }
}

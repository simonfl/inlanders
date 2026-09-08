using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using Inlanders.Simulation;

public static class MapChecks
{
    public static void Run()
    {
        void Check(bool value, string message) { if (!value) throw new Exception(message); }
        var w = World.NewLargeMap();
        Check(w.Map.Width == 32 && w.Map.Excluded.Count > 0 && w.Trees.Count == 20 && w.Bushes.Count == 6, "Large layout missing resources or outline");
        Check(!w.CanPlace(new(15, 15), false) && !w.CanPlantTree(new(-16, -16)), "Missing terrain accepts building or planting");
        foreach (var cell in new[] { new Cell(-10, -3), new(9, 3), new(2, 10) })
            Check(w.Place(cell) != null, $"Distant clearing rejects building at {cell}: {w.PlacementProblem(cell, false)}");
        for (int i = 0; i < 8; i++) w.Assign(i, i < 4 ? Role.Logger : Role.Builder);
        var timer = Stopwatch.StartNew();
        for (int i = 0; i < 10000 && w.Cottages.Any(c => !c.Complete); i++) { w.Tick(0.1f); w.Validate(); }
        Check(w.Cottages.All(c => c.Complete), "Distant construction failed");
        string saved = w.SaveJson(); var restored = World.LoadJson(saved);
        Check(restored.SaveJson() == saved, "Map save changed authored terrain or resources");
        foreach (var p in w.People) { w.Assign(p.Id, Role.Logger); restored.Assign(p.Id, Role.Logger); }
        for (int i = 0; i < 15000 && w.Trees.Any(t => t.Logs > 0); i++)
        { w.Tick(0.1f); restored.Tick(0.1f); w.Validate(); restored.Validate(); }
        Check(w.Trees.All(t => t.Logs == 0) && w.SaveJson() == restored.SaveJson(), "Outer groves unreachable or continuation changed");
        Check(w.PlantTree(new(10, 10)) != null, "Planting outside old limits failed");
        var old = JsonNode.Parse(new World().SaveJson())!; old["Version"] = 4; old.AsObject().Remove("Map");
        Check(World.LoadJson(old.ToJsonString()).Map.OriginalOutline, "Legacy save lost original map");
        // A valid rectangular scenario uses the same model without any excluded cells.
        var rectangle = JsonNode.Parse(new World().SaveJson())!; rectangle["Map"]!["Width"] = 25;
        var rect = World.LoadJson(rectangle.ToJsonString()); Check(rect.CanPlace(new(12, 0), false), "Configurable rectangle failed");
        var invalid = JsonNode.Parse(saved)!; invalid["Map"]!["Width"] = 0;
        bool rejected = false; try { World.LoadJson(invalid.ToJsonString()); } catch (InvalidDataException) { rejected = true; }
        Check(rejected, "Malformed terrain loaded");
        invalid = JsonNode.Parse(saved)!; invalid["Map"]!["Excluded"]!.AsArray().Add(new JsonObject { ["X"] = -2, ["Z"] = 3 });
        rejected = false; try { World.LoadJson(invalid.ToJsonString()); } catch (Exception e) when (e is InvalidDataException or InvalidOperationException) { rejected = true; }
        Check(rejected, "Map that removes yard access loaded");
        Console.WriteLine($"PASS: expanded/irregular maps, three distant construction areas, all outer groves, exact map saves, legacy bounds, invalid layouts; simulation checks {timer.Elapsed.TotalSeconds:F2}s.");
    }
}

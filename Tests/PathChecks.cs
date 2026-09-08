using System;
using System.Linq;
using System.Text.Json.Nodes;
using Inlanders.Simulation;

public static class PathChecks
{
    public static void Run()
    {
        void Check(bool value, string message) { if (!value) throw new Exception(message); }
        var w = new World(); foreach (var p in w.People) w.Assign(p.Id, Role.Unassigned);
        Check(!w.SetPath(w.Stockpile, true) && !w.SetPath(w.Trees[0].Cell, true) && !w.SetPath(new(9, 9), true), "Blocked path accepted");
        w.People[0].Position = new(4, 6);
        for (int z = -1; z <= 6; z++) Check(w.SetPath(new(5, z), true), "Path corridor rejected");
        for (int x = -2; x <= 4; x++) Check(w.SetPath(new(x, -1), true), "Path connection rejected");
        w.Assign(0, Role.Logger); w.Tick(0.1f);
        Check(w.People[0].Route.Count(c => w.Paths.Contains(c)) >= 6, "Worker ignored path route");
        string saved = w.SaveJson(); var restored = World.LoadJson(saved);
        for (int i = 0; i < 100; i++) { w.Tick(0.1f); restored.Tick(0.1f); w.Validate(); restored.Validate(); }
        Check(w.SaveJson() == restored.SaveJson(), "Path save changed active route");
        var plain = World.LoadJson(saved);
        foreach (var cell in plain.Paths.ToArray()) plain.SetPath(cell, false);
        var fast = World.LoadJson(saved);
        int Arrival(World a)
        {
            int ticks = 0; while (a.People[0].Route.Count > 0 && ticks < 1000) { a.Tick(0.1f); a.Validate(); ticks++; }
            return ticks;
        }
        Check(Arrival(fast) < Arrival(plain), "Path travel did not beat the grass route");
        int id = w.People[0].TreeId ?? -1;
        foreach (var cell in w.Paths.ToArray()) w.SetPath(cell, false);
        Check((w.People[0].TreeId ?? -1) == id, "Editing paths lost work claim"); w.Validate();
        var building = new World(); building.SetPath(new(3, 0), true);
        Check(building.Place(new(3, 0)) != null && !building.Paths.Contains(new(3, 0)), "Building did not replace path");
        building.SetPath(new(6, 4), true); Check(building.PlantTree(new(6, 4)) != null && !building.Paths.Contains(new(6, 4)), "Planting did not replace path");
        var legacy = JsonNode.Parse(new World().SaveJson())!; legacy["Version"] = 6; legacy.AsObject().Remove("Paths");
        Check(World.LoadJson(legacy.ToJsonString()).Paths.Count == 0, "Old save gained paths");
        Console.WriteLine("PASS: path legality, weighted route preference and faster travel, live edits/claims, exact saves, building/planting replacement, and legacy saves.");
    }
}

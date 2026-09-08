using System;
using Inlanders.Simulation;

public static class PlacementChecks
{
    public static void Run()
    {
        var w = new World(0);
        void Reason(Cell cell, string expected)
        {
            string reason = w.PlacementProblem(cell, false) ?? "";
            if (!reason.Contains(expected, StringComparison.OrdinalIgnoreCase) || w.Place(cell) != null)
                throw new Exception($"Expected {expected} rejection at {cell}: {reason}");
        }
        Reason(new(9, 0), "map");
        Reason(new(-3, 3), "yard");
        Reason(new(-3, -4), "tree");
        Reason(new(0, -2), "bush");
        Reason(new(-3, -5), "entrance");
        if (w.PlacementProblem(new(3, 0), false) != null || w.Place(new(3, 0)) == null)
            throw new Exception("Empty storage must still allow a valid plan");
        Reason(new(3, 0), "overlaps");
        if (w.PlantTree(new(5, 4)) == null || !w.PlantingProblem(new(5, 4))!.Contains("already marked"))
            throw new Exception("Pending tree explanation missing");
        Console.WriteLine("PASS: placement explanations match rejected commands; empty storage permits planning; duplicate planting is explained.");
    }
}

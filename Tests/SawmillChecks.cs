using Inlanders.Simulation;
using System.Text.Json.Nodes;

public static class SawmillChecks
{
    static void Check(bool value, string message) { if (!value) throw new Exception(message); }
    static void Step(World w, int ticks = 1) { for (int i = 0; i < ticks; i++) { w.Tick(0.1f); w.Validate(); } }
    static void Until(World w, Func<bool> done, string message, int max = 20000)
    { for (int i = 0; i < max && !done(); i++) Step(w); Check(done(), message); }
    public static void Run()
    {
        var w = new World();
        var mill = w.Place(new(3,0), false, BuildingKind.Sawmill)!;
        var lodge = w.Place(new(6,0), true, BuildingKind.Lodge)!;
        w.Assign(0, Role.Sawyer); w.Assign(1, Role.Sawyer);
        var phases = new[] { Work.ToSawLogs, Work.ToSawmill, Work.Sawing, Work.ToPlanks, Work.ToStockpile };
        var saves = new Dictionary<Work, string>();
        var deliveries = new Dictionary<Work, string>();
        for (int tick = 0; tick < 20000 && !lodge.Complete; tick++)
        {
            Step(w);
            foreach (var phase in phases)
                if (!saves.ContainsKey(phase) && w.People.Any(v => v.Role == Role.Sawyer && v.Task == phase)) saves[phase] = w.SaveJson();
            foreach (var phase in new[] { Work.ToMaterials, Work.ToCottage, Work.ToBuild, Work.Building })
                if (!deliveries.ContainsKey(phase) && w.People.Any(v => v.SiteId == lodge.Id && v.Task == phase)) deliveries[phase] = w.SaveJson();
        }
        Check(lodge.Complete && lodge.Delivered == lodge.Required && w.Housed == 4 && saves.Count == phases.Length && deliveries.Count == 4, "Sawmill/lodge chain did not finish all phases");
        Until(w, () => w.Planks == World.PlankStockTarget && w.People.All(v => v.Role != Role.Sawyer || v.Task == Work.Waiting), "Mill never reached stock target");
        int used = w.SawnLogs; Step(w, 1200); Check(w.SawnLogs == used, "Idle sawmill kept consuming timber");
        Check(w.Place(new(3,6), false, BuildingKind.Lodge) != null, "Second lodge rejected");
        Until(w, () => w.Housed == 8, "Two lodges did not house the village");
        w.Food.Bread += 16; w.Food.BakedBread += 16; w.Food.UsedGrain += 8; w.Food.GrownGrain += 8;
        w.Validate(); Check(w.BeginSupper(), "Lodges did not qualify for supper");
        Until(w, () => w.Food.SupperComplete, "Supper interrupted sawmill claims incorrectly");
        foreach (var (phase, json) in saves)
        {
            var a = World.LoadJson(json); var b = World.LoadJson(json);
            Check(a.SaveJson() == json, $"Sawmill save mismatch at {phase}"); Step(a, 300); Step(b, 300);
            Check(a.SaveJson() == b.SaveJson(), $"Sawmill continuation diverged at {phase}");
            var c = World.LoadJson(json); var worker = c.People.First(v => v.Role == Role.Sawyer && v.Task == phase);
            int carried = worker.Carried; c.Assign(worker.Id, Role.Unassigned); c.Validate();
            Check(worker.WorkplaceId == null && worker.Reserved == 0 && worker.Carried == carried, "Sawyer reassignment lost cargo or claims");
            Until(c, () => c.Cottages.Single(s => s.Id == lodge.Id).Complete && worker.Carried == 0, $"Reassignment stalled {phase}");
        }
        foreach (var (phase, json) in deliveries)
        {
            var c = World.LoadJson(json); Check(c.Cancel(lodge.Id), "Lodge cancellation failed"); c.Validate();
            foreach (var person in c.People) c.Assign(person.Id, Role.Logger);
            Until(c, () => c.Trees.All(t => !t.Salvage) && c.People.All(v => v.Carried == 0), $"Plank salvage stalled {phase}");
            Check(c.CanPlace(lodge.Cell, lodge.Rotation), "Salvaged lodge site not reusable");
        }
        // Two mills and several builders compete for the same limited timber.
        var scarce = new World(3);
        scarce.Place(new(3,0), false, BuildingKind.Sawmill); scarce.Place(new(6,0), true, BuildingKind.Sawmill);
        scarce.Place(new(3,6), false, BuildingKind.Lodge);
        scarce.Assign(0, Role.Sawyer); scarce.Assign(1, Role.Sawyer);
        Until(scarce, () => scarce.Housed == 4, "Competing mills/builders stalled with sufficient timber");
        var bad = JsonNode.Parse(w.SaveJson())!; bad["Planks"] = -1;
        bool rejected = false; try { World.LoadJson(bad.ToJsonString()); } catch (InvalidOperationException) { rejected = true; }
        Check(rejected, "Invalid plank save accepted");
        Console.WriteLine("PASS: sawmill/lodge chain, physical cargo, stock target, five saved/interrupted sawyer phases, four cancellation phases with plank salvage and competing mills.");
    }
}

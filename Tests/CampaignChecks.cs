using System;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using Inlanders.Simulation;

public static class CampaignChecks
{
    static void Check(bool condition, string message) { if (!condition) throw new Exception(message); }
    static void Steps(World w, int count) { for (int i = 0; i < count; i++) { w.Tick(0.1f); w.Validate(); } }
    public static void Run()
    {
        var w = World.NewCampaign(1);
        Check(w.HasForagerHut && w.Housed == 0 && w.Food.Berries == 64, "Opening setup wrong");
        Check(w.CurrentCampaignHint()?.Id == "welcome", "Arrival hint missing");
        foreach (var cell in new[] { new Cell(3, 0), new(6, 0), new(3, 6), new(-5, 6) }) Check(w.Place(cell) != null, "Housing layout rejected");
        Check(w.CurrentCampaignHint()?.Id == "construction", "Building ahead left stale tutorial");
        for (int i = 0; i < 10000 && !w.Campaign!.Complete; i++) Steps(w, 1);
        Check(w.Campaign!.Complete && w.Housed == 8 && w.Food.Hunger == 0, "Housing level did not finish comfortably");
        var book = new CampaignBook(); book.Capture(w);
        string first = w.SaveJson();
        var alternative = World.NewCampaign(1);
        alternative.Place(new(3, 0), false, BuildingKind.Sawmill);
        alternative.Place(new(6, 0), false, BuildingKind.Lodge); alternative.Place(new(3, 6), false, BuildingKind.Lodge);
        alternative.Assign(6, Role.Sawyer);
        for (int i = 0; i < 10000 && !alternative.Campaign!.Complete; i++) Steps(alternative, 1);
        Check(alternative.Campaign!.Complete, "Lodges did not qualify as alternative housing");
        w = World.NewCampaign(2);
        Check(w.Housed == 8 && !w.HasForagerHut && w.DeliveredBerries == 0, "Berry setup wrong");
        Check(w.Place(new(0, 0), false, BuildingKind.ForagerHut) != null, "Hut site rejected");
        for (int i = 0; i < 3000 && !w.HasForagerHut; i++) Steps(w, 1);
        Check(w.CurrentCampaignHint()?.Id == "foragers", "Unstaffed hut not explained");
        w.Assign(4, Role.Forager); w.Assign(5, Role.Forager);
        for (int i = 0; i < 3000 && !w.People.Any(p => p.Cargo == Resource.Berries && p.Carried > 0); i++) Steps(w, 1);
        Check(w.DeliveredBerries < w.Food.GatheredBerries, "Carried berries counted as delivered");
        w.Campaign!.Dismissed.Add("deliveries"); w.Campaign.Guidance = false;
        string inFlight = w.SaveJson(); var copy = World.LoadJson(inFlight);
        Check(copy.SaveJson() == inFlight && copy.CurrentCampaignHint() == null, "Campaign save lost tutorial or cargo state");
        // Interrupt a carrying forager; the returned berries count once, even across saves.
        var carrier = w.People.First(p => p.Cargo == Resource.Berries && p.Carried > 0); w.Assign(carrier.Id, Role.Unassigned); copy.Assign(carrier.Id, Role.Unassigned);
        for (int i = 0; i < 5000 && !w.Campaign.Complete; i++) { Steps(w, 1); Steps(copy, 1); }
        Check(w.Campaign.Complete && w.SaveJson() == copy.SaveJson(), "Berry level or exact continuation failed");
        int delivered = w.DeliveredBerries;
        foreach (var p in w.People) w.Assign(p.Id, Role.Unassigned);
        Steps(w, 1000);
        Check(w.DeliveredBerries >= delivered && w.Campaign.Complete, "Meals undid delivery progress");
        book.Capture(w); book.BeforeReplay[2] = w.SaveJson(); book.Capture(World.NewCampaign(2));
        string path = Path.Combine(Path.GetTempPath(), "inlanders-campaign-" + Guid.NewGuid() + ".json");
        try
        {
            book.SaveFile(path); var restored = CampaignBook.LoadFile(path);
            Check(restored.Completed.SetEquals(new[] { 1, 2 }) && restored.Settlements[1] == first && World.LoadJson(restored.BeforeReplay[2]).Campaign!.Complete,
                "Replay or switching lost completed village/progress");
            restored.SaveFile(path); Check(File.Exists(path + ".bak"), "Campaign backup missing");
        }
        finally { foreach (string suffix in new[] { "", ".bak", ".tmp" }) if (File.Exists(path + suffix)) File.Delete(path + suffix); }
        // Legacy saves retain the original food budget and standalone objective.
        var legacy = JsonNode.Parse(new World().SaveJson())!; legacy["Version"] = 3; legacy.AsObject().Remove("Campaign"); legacy["Food"]!.AsObject().Remove("InitialBerries");
        var old = World.LoadJson(legacy.ToJsonString()); Check(old.Campaign == null && old.Food.InitialBerries == 24, "Standalone compatibility failed");
        Console.WriteLine("PASS: both campaign levels, action-aware/disabled hints, delivered food accounting, exact interrupted-cargo saves, meal persistence, replay archives, and legacy standalone saves.");
    }
}

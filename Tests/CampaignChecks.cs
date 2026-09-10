using System;
using System.IO;
using System.Linq;
using Inlanders.Simulation;

public static class CampaignChecks
{
    static void Check(bool condition, string message) { if (!condition) throw new Exception(message); }
    static void Until(World w, Func<bool> done)
    {
        for (int i = 0; i < 20000 && !done(); i++) { w.Tick(.1f); if (i % 50 == 0) w.Validate(); }
        w.Validate(); Check(done(), $"Level {w.Campaign!.Level} stalled: {w.CampaignObjective}");
    }
    static void Place(World w, Cell cell, BuildingKind kind = BuildingKind.Cottage) =>
        Check(w.Place(cell, false, kind) != null, $"Rejected {kind} at {cell}");
    public static void Run()
    {
        var book = new CampaignBook();
        Cell[] supperSpots = Array.Empty<Cell>();
        for (int level = 1; level <= World.CampaignLevels.Length; level++)
        {
            var w = World.NewCampaign(level);
            Check(!w.Campaign!.Complete && w.DeliveredBread == 0, "New level already credited");
            if (level == 1)
            {
                Check(w.CurrentCampaignHint()?.Id == "welcome", "Opening guidance missing");
                Place(w, new(0,0), BuildingKind.ForagerHut);
                foreach (var c in new[] { new Cell(3,0), new(6,0), new(3,6), new(-5,6) }) Place(w,c);
            }
            if (level == 2)
            {
                Place(w, new(3,-3), BuildingKind.Farm); Place(w, new(6,-3), BuildingKind.Bakery);
                w.Assign(6,Role.Farmer); w.Assign(7,Role.Baker);
            }
            if (level == 3)
            {
                Place(w,new(3,-3),BuildingKind.Sawmill); Place(w,new(6,-3),BuildingKind.Lodge);
                w.Assign(6,Role.Sawyer);
                foreach (var c in new[] { new Cell(0,6), new(0,8), new(2,8), new(4,8) })
                    Check(w.PlantTree(c) != null, "Planting rejected");
                Check(w.TreesPlanted == 0, "Marks counted before planting");
            }
            if (level == 4)
            {
                Check(w.Housed==8 && w.HasBuilding(BuildingKind.Farm) && w.HasBuilding(BuildingKind.Bakery),"Supper start repeats earlier building lessons");
                Place(w,new(7,3),BuildingKind.Square);
                Until(w,() => w.CanCelebrate);
                Check(!w.Campaign.Complete, "Finale completed before supper");
                Check(w.BeginSupper() && !w.BeginSupper(), "Supper charged twice");
                var square = w.Cottages.Single(c => c.Kind == BuildingKind.Square);
                Check(w.MeetingSpots.Count == 8 && w.MeetingSpots.All(c => (c.Point-square.Entrance.Point).LengthSquared() <= 16), "Guests not near square");
                supperSpots = w.MeetingSpots.ToArray();
                Check(!w.PlaceDecoration(supperSpots[0], DecorationKind.Flowers), "Active supper space could be blocked");
            }
            if (level == 5)
            {
                Check(w.Housed==8 && w.Food.Berries==48 && w.CurrentCampaignHint()?.Id=="garden","Garden opening incorrect");
                Place(w,new(3,-3),BuildingKind.VegetableGarden);
            }
            // Exact continuation, including planting jobs and guests walking to supper.
            w.Campaign.Dismissed.Add("welcome"); w.Campaign.Guidance = false;
            var copy = World.LoadJson(w.SaveJson());
            Check(copy.CurrentCampaignHint() == null, "Disabled hints returned");
            for (int i=0;i<50;i++) { w.Tick(.1f); copy.Tick(.1f); }
            Check(w.SaveJson() == copy.SaveJson(), "Save continuation diverged");
            Until(w, () => w.Campaign.Complete);
            float completionTime=w.Food.Time;
            if (level == 4)
            {
                var buildingCopy = World.LoadJson(w.SaveJson());
                bool CanBuildOnOldSpot(Cell c) => World.Footprint(c, false).Any(supperSpots.Contains) && buildingCopy.PlacementProblem(c, false) == null;
                Until(buildingCopy, () => buildingCopy.Map.Land.Any(CanBuildOnOldSpot));
                Place(buildingCopy, buildingCopy.Map.Land.First(CanBuildOnOldSpot));
                string built = buildingCopy.SaveJson();
                Check(World.LoadJson(built).SaveJson() == built, "Post-supper construction broke save roundtrip");
                Until(w, () => supperSpots.Any(c => w.DecorationProblem(c, DecorationKind.Flowers) == null));
                var vacated = supperSpots.First(c => w.DecorationProblem(c, DecorationKind.Flowers) == null);
                Check(w.PlaceDecoration(vacated, DecorationKind.Flowers), "Vacated supper space could not be decorated");
                string decorated = w.SaveJson();
                Check(World.LoadJson(decorated).SaveJson() == decorated, "Post-supper decoration broke save roundtrip");
            }
            if(level==5) Check(w.DeliveredVegetables>=16 && w.Food.VegetableChoiceMeals>=2 && !w.SunflowersUnlocked && w.Cottages.Count(c=>c.Kind==BuildingKind.VegetableGarden)==1 && !w.HasBuilding(BuildingKind.Bakery),"Garden lesson required unrelated systems");
            int vegetables=w.DeliveredVegetables, choices=w.Food.VegetableChoiceMeals;
            int delivered = w.DeliveredBread;
            foreach (var p in w.People) w.Assign(p.Id,Role.Unassigned);
            for (int i=0;i<700;i++) w.Tick(.1f);
            Check(w.Campaign.Complete && w.DeliveredBread >= delivered, "Meals erased progress");
            Check(w.DeliveredVegetables>=vegetables && w.Food.VegetableChoiceMeals>=choices,"Meals erased vegetable progress");
            if(level == 4) Check(w.Food.SupperBread == 16 && w.Food.SupperComplete, "Supper did not finish once");
            book.Capture(w); book.BeforeReplay[level] = w.SaveJson();
            Console.WriteLine($"PASS: campaign {level}, contextual guidance, exact saves, completion and persistent milestones; completed in {completionTime:F1}s (day {1+(int)(completionTime/60)}).");
        }
        book.Capture(World.NewCampaign(5));
        string path = Path.Combine(Path.GetTempPath(), "inlanders-campaign-" + Guid.NewGuid() + ".json");
        try
        {
            book.SaveFile(path); var restored = CampaignBook.LoadFile(path);
            Check(restored.Completed.SetEquals(new[]{1,2,3,4,5}) && World.LoadJson(restored.BeforeReplay[5]).Campaign!.Complete, "Replay lost campaign progress");
        }
        finally { foreach (string suffix in new[]{"",".bak",".tmp"}) if(File.Exists(path+suffix)) File.Delete(path+suffix); }
    }
}

using Inlanders.Simulation;
using System.Text.Json.Nodes;

public static class DemolitionChecks
{
    static void Check(bool ok, string why) { if (!ok) throw new Exception(why); }
    static void Step(World w) { w.Tick(.1f); w.Validate(); }
    static World Built(BuildingKind kind)
    {
        var w = World.NewCreative(); foreach (var p in w.People) w.Assign(p.Id, Role.Unassigned);
        if(kind is BuildingKind.Quarry or BuildingKind.GatheringHall) w.Map.StoneDeposits.Add(new() { Id=0,Cell=new(5,-2),Capacity=16,Remaining=16 });
        if(kind==BuildingKind.HuntingLodge) w.Map.Wildlife.Add(new() { Id=0,Cell=new(-3,-3),Stock=10 });
        if(kind==BuildingKind.FishingDock)
        {
            w.Map.Water.Add(new(3,-1));
            w.Map.FishingGrounds.Add(new FishHabitat { Id=0,Cell=new(3,-1) });
        }
        w.Place(new(3,0), false, kind);
        var json = JsonNode.Parse(w.SaveJson())!; json["Creative"] = false;
        var building = json["Buildings"]![0]!; int cost = Buildings.Get(kind).Cost;
        building["Delivered"] = cost;
        int logs = Buildings.Get(kind).Material==Resource.Planks ? cost / 2 : cost;
        if (Buildings.Get(kind).Material==Resource.Planks) json["SawnLogs"] = logs;
        if(kind==BuildingKind.GatheringHall)
        {
            building["DeliveredStone"]=12; json["QuarriedStone"]=12;
            json["Map"]!["StoneDeposits"]![0]!["Remaining"]=4;
        }
        json["Food"]!["InitialBerries"] = 1000; json["Food"]!["Berries"] = 1000;
        if (kind == BuildingKind.Stockpile) { building["StoredLogs"] = 8; logs += 8; }
        if (kind == BuildingKind.Sawmill) { building["InputLogs"] = 2; building["OutputPlanks"] = 4; json["SawnLogs"] = 2; logs += 4; }
        if (kind == BuildingKind.Bakery) { building["InputGrain"] = 2; building["OutputBread"] = 4; json["Food"]!["GrownGrain"] = 4; json["Food"]!["UsedGrain"] = 2; json["Food"]!["BakedBread"] = 4; }
        if (kind is BuildingKind.Farm or BuildingKind.VegetableGarden)
        {
            int crop = kind == BuildingKind.Farm ? 6 : 8;
            building["Harvest"] = crop; building["Planted"] = true; building["Growth"] = 1;
            json["Food"]![kind == BuildingKind.Farm ? "GrownGrain" : "GrownVegetables"] = crop;
        }
        json["InitialLogs"] = w.InitialLogs + logs;
        return World.LoadJson(json.ToJsonString());
    }
    public static void Run()
    {
        foreach (var kind in Enum.GetValues<BuildingKind>().Where(k => k != BuildingKind.Bridge))
        {
            var w = Built(kind); var site = w.Cottages.Single();
            int beds = w.Beds;
            Check(!w.RemoveBuilding(site.Id), "Normal game used instant Creative removal");
            Check(w.RequestDemolition(site.Id) && w.Beds == 0, "Demolition did not stop housing");
            Check(w.CancelDemolition(site.Id) && w.Beds == beds && !site.WorkPaused, "Pending cancellation did not restore service");
            Check(w.RequestDemolition(site.Id), "Second demolition order rejected");
            Check(!w.SetWorkplacePaused(site.Id, false), "Demolition workplace resumed production");
            string queued = w.SaveJson(); w = World.LoadJson(queued); Check(w.SaveJson() == queued, "Queued demolition lost state");
            w.Assign(0, Role.Builder); bool cargo = false, dismantling = false, resumed = false;
            for (int i=0;i<6000 && (w.Cottages.Count > 0 || w.People.Any(p => p.Carried > 0));i++)
            {
                Step(w); cargo |= w.People.Any(p => p.Carried > 0);
                if (!resumed && w.Cottages.FirstOrDefault()?.DemolitionProgress > 0)
                {
                    dismantling = true; resumed = true;
                    Check(!w.CancelDemolition(site.Id), "Started dismantling was cancelled");
                    w.Assign(0,Role.Unassigned); Step(w);
                    string saved = w.SaveJson(); w = World.LoadJson(saved); Check(w.SaveJson() == saved, "Partial demolition did not roundtrip");
                    w.Assign(0,Role.Builder);
                }
            }
            Check(w.Cottages.Count == 0 && cargo && dismantling, $"Demolition stalled or teleported goods: {kind}");
            int expectedLogs = Buildings.Get(kind).Material==Resource.Planks ? 0 : Buildings.Get(kind).Cost;
            expectedLogs += kind == BuildingKind.Stockpile ? 8 : kind == BuildingKind.Sawmill ? 2 : 0;
            Check(w.Stored == expectedLogs && w.Stone==Buildings.Get(kind).StoneCost && w.Planks == (Buildings.Get(kind).Material==Resource.Planks ? Buildings.Get(kind).Cost : kind == BuildingKind.Sawmill ? 4 : 0), $"Wrong recovery: {kind}");
            Check(w.Place(new(3,0), false, kind==BuildingKind.FishingDock ? kind : BuildingKind.Cottage) != null, "Demolished plot cannot be rebuilt");
        }
        Console.WriteLine("PASS: normal demolition of all land building types, physical buffer/material recovery, bed/service changes, cancellation, reassignment, partial saves and reuse of land.");
        var river = World.NewCreative(true);
        foreach (var p in river.People) river.Assign(p.Id,Role.Unassigned);
        for (int z=river.Map.MinZ;z<=river.Map.MaxZ;z++) if (river.Map.Contains(new(7,z))) river.Map.Water.Add(new(7,z));
        var bridge = river.Place(new(7,3),true,BuildingKind.Bridge)!;
        var paid = JsonNode.Parse(river.SaveJson())!; paid["Creative"] = false;
        paid["Buildings"]![0]!["Delivered"] = 6; paid["InitialLogs"] = river.InitialLogs + 6;
        river = World.LoadJson(paid.ToJsonString());
        Check(!river.RequestDemolition(bridge.Id), "Only crossing was queued for demolition");
        // Build the alternative with real deliveries before allowing removal.
        var alternative = river.Place(new(7,5),true,BuildingKind.Bridge)!;
        river.Assign(0,Role.Logger); river.Assign(1,Role.Builder);
        for(int i=0;i<10000 && !alternative.Complete;i++) Step(river);
        Check(alternative.Complete && river.RequestDemolition(bridge.Id), "Alternative crossing did not enable demolition");
        Check(!river.RequestDemolition(alternative.Id), "Both mutually dependent crossings could be removed");
        for(int i=0;i<10000 && river.Cottages.Any(c=>c.Id==bridge.Id);i++) Step(river);
        Check(!river.Cottages.Any(c=>c.Id==bridge.Id) && river.Cottages.Any(c=>c.Id==alternative.Id), "Safe bridge demolition stalled");
        Console.WriteLine("PASS: bridge access protection, real alternative crossing and mutually dependent demolition orders.");
        var working = FoodChecks.Scenario();
        var snapshots = new Dictionary<Work,string>();
        foreach (int tick in Enumerable.Range(0,5000))
        {
            Step(working);
            foreach (var person in working.People.Where(p => p.WorkplaceId is int id && working.Cottages.Any(c => c.Id == id && c.Kind == BuildingKind.Bakery) &&
                p.Task is Work.ToOven or Work.Baking or Work.ToBread or Work.ToPantry))
                snapshots.TryAdd(person.Task,working.SaveJson());
            if(snapshots.Count==4) break;
        }
        Check(snapshots.Count==4,"Missing live bakery demolition fixtures");
        foreach(var (phase,json) in snapshots)
        {
            var w=World.LoadJson(json); var bakery=w.Cottages.Single(c=>c.Kind==BuildingKind.Bakery);
            var baker=w.People.First(p=>p.WorkplaceId==bakery.Id); int carried=baker.Carried;
            Check(w.RequestDemolition(bakery.Id) && baker.Carried==carried && baker.Role==Role.Baker,"Demolition lost cargo or changed role at "+phase);
            w.Validate(); string interrupted=w.SaveJson(); w=World.LoadJson(interrupted);
            Check(w.SaveJson()==interrupted,"Interrupted demolition did not save exactly");
            for(int i=0;i<10000 && w.Cottages.Any(c=>c.Id==bakery.Id);i++) Step(w);
            Check(!w.Cottages.Any(c=>c.Id==bakery.Id),"Active bakery demolition stalled at "+phase);
        }
        Console.WriteLine("PASS: live bakery grain delivery, active batch, bread pickup and carried delivery interrupted safely with exact saves.");
        var garden=Built(BuildingKind.VegetableGarden); var plot=garden.Cottages.Single();
        garden.RequestDemolition(plot.Id); garden.Assign(0,Role.Builder);
        for(int i=0;i<3000 && plot.Harvest>0;i++) Step(garden);
        Check(plot.Harvest==0 && plot.DemolitionProgress==0 && garden.CancelDemolition(plot.Id),"Cannot cancel after crop evacuation");
        Check(!plot.Planted && plot.Growth==0,"Evacuated garden stuck in harvested state");
        garden.Assign(0,Role.Farmer);
        for(int i=0;i<2000 && !plot.Planted;i++) Step(garden);
        Check(plot.Planted,"Garden did not resume after cancelled demolition");
        Console.WriteLine("PASS: cancelled crop evacuation returns carried food and allows replanting.");
    }
}

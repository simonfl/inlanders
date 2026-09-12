using Inlanders.Simulation;
using System.Text.Json;

// Existing-system budget probes, not implementations of learning or reflection.
static class CivicBudgetChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    public static void Run()
    {
        Directory.CreateDirectory("artifacts");var reports=new List<object>();
        foreach(string route in new[]{"central","scenic-local","food-diversion-recovery","restoration-envelope"})
        {
            var w=World.NewQuarryMap();
            bool scenic=route=="scenic-local",poor=route=="food-diversion-recovery",learning=route=="restoration-envelope";
            var orders=new List<Cottage>();
            Cottage Build(Cell at,BuildingKind kind)
            {
                var site=w.Place(at,0,kind)??throw new Exception($"{route}: {kind} at {at}: {w.PlacementProblem(at,0,kind)}");
                orders.Add(site);return site;
            }
            Build(new(3,-5),BuildingKind.Sawmill);Build(new(11,-4),BuildingKind.Quarry);
            var hall=Build(scenic?new(12,1):new(0,-3),BuildingKind.GatheringHall);
            if(scenic)Build(new(-3,0),BuildingKind.Square);
            if(learning){Build(new(12,1),BuildingKind.GatheringHall);Build(new(-3,0),BuildingKind.Carpenter);}
            int rawLogs=orders.Where(c=>c.Material==Resource.Logs).Sum(c=>c.Required);
            int planks=orders.Where(c=>c.Material==Resource.Planks).Sum(c=>c.Required),stone=orders.Sum(c=>c.RequiredStone);
            Check(rawLogs==(learning?18:scenic?18:12) && planks==(learning?16:8) && stone==(learning?24:12),"Budget proxy changed");
            if(poor){w.Assign(3,Role.Quarrier);w.Assign(4,Role.Sawyer);w.Assign(5,Role.Builder);}
            else {w.Assign(6,Role.Quarrier);w.Assign(7,Role.Sawyer);}
            float completed=-1,recovered=-1;bool sawMiss=false;double routedPersonSeconds=0;int missesBeforeRepair=0;
            for(int tick=0;tick<24000;tick++)
            {
                w.Tick(.1f);w.Validate();routedPersonSeconds+=w.People.Count(p=>p.Route.Count>0)*.1;
                if(completed<0 && orders.All(c=>c.Complete))completed=w.Food.Time;
                var meals=w.ReadMealAssessment();sawMiss|=meals.Missed>0 || meals.Skipped>0;
                if(poor && tick==8999)
                {
                    missesBeforeRepair=meals.Missed+meals.Skipped;Check(sawMiss,"Food diversion did not expose shortage");
                    File.WriteAllText("artifacts/civic-diversion-before-repair.json",w.SaveJson());
                    string saved=w.SaveJson();w=World.LoadJson(saved);Check(w.SaveJson()==saved,"Poor allocation reload differs");
                    w.Assign(3,Role.Forager);w.Assign(4,Role.Forager);w.Assign(5,Role.Farmer);w.Assign(6,Role.Quarrier);w.Assign(7,Role.Sawyer);
                }
                // Find completion by IDs because recovery deliberately reloads the world.
                bool built=orders.All(c=>w.Cottages.Any(s=>s.Id==c.Id && s.Complete));
                int recent=w.People.Count(p=>p.LastLeisureTime is float t && w.Food.Time-t<p.LastLeisureWindow);
                if(built && (!poor || tick>8999) && meals.Reliable && meals.FreshSupply && recent>=4)
                {recovered=w.Food.Time;break;}
            }
            Check(recovered>0,$"{route} did not support its settlement: {w.ReadMealAssessment().Summary}");
            Check(orders.All(c=>w.Cottages.Single(s=>s.Id==c.Id).Complete),"Budget invoice not physically delivered");
            string snapshot=w.SaveJson();var copy=World.LoadJson(snapshot);Check(copy.SaveJson()==snapshot,"Completed probe reload differs");
            for(int tick=0;tick<100;tick++){w.Tick(.1f);copy.Tick(.1f);}Check(w.SaveJson()==copy.SaveJson(),"Probe continuation differs");
            var final=World.LoadJson(snapshot);var meal=final.ReadMealAssessment();
            var report=new {route,rawLogs,planks,stone,minimumLogsIncludingSawing=rawLogs+planks/2,constructionSeconds=completed,supportedSeconds=recovered,
                routedPersonSeconds=Math.Round(routedPersonSeconds,1),missesBeforeRepair,sawAnyMiss=sawMiss,
                recentProjectVisitors=final.People.Count(p=>p.LastLeisureSiteId==hall.Id && p.LastLeisureTime is float t && final.Food.Time-t<p.LastLeisureWindow),
                meals=meal,remainingStone=final.Map.StoneDeposits.Sum(d=>d.Remaining),yardLogs=final.YardLogs};
            reports.Add(report);Console.WriteLine(JsonSerializer.Serialize(report));File.WriteAllText($"artifacts/civic-{route}.json",snapshot);
        }
        File.WriteAllText("artifacts/civic-budget-results.json",JsonSerializer.Serialize(reports,new JsonSerializerOptions{WriteIndented=true}));
        var recovery=World.NewQuarryMap();
        recovery.Place(new(3,-5),0,BuildingKind.Sawmill);recovery.Place(new(11,-4),0,BuildingKind.Quarry);
        var project=recovery.Place(new(0,-3),0,BuildingKind.GatheringHall)!;
        recovery.Assign(6,Role.Quarrier);recovery.Assign(7,Role.Sawyer);
        for(int i=0;i<12000 && (project.Delivered==0 || project.DeliveredStone==0);i++){recovery.Tick(.1f);recovery.Validate();}
        Check(!project.Complete && project.Delivered>0 && project.DeliveredStone>0,"No mixed-material partial project");
        int deliveredPlanks=project.Delivered,deliveredStone=project.DeliveredStone;
        recovery.Assign(1,Role.Unassigned);recovery.Assign(2,Role.Unassigned);float progress=project.Construction;
        for(int i=0;i<600;i++){recovery.Tick(.1f);recovery.Validate();}
        Check(project.Construction==progress,"Diverted builders did not pause project");
        Check(recovery.Cancel(project.Id),"Cannot cancel partial civic envelope");
        Check(recovery.Trees.Where(t=>t.Salvage && t.Material==Resource.Planks).Sum(t=>t.Logs)==deliveredPlanks && recovery.Trees.Where(t=>t.Salvage && t.Material==Resource.Stone).Sum(t=>t.Logs)==deliveredStone,"Cancel lost delivered material");
        recovery=World.LoadJson(recovery.SaveJson());recovery.Assign(1,Role.Builder);recovery.Assign(2,Role.Builder);
        for(int i=0;i<12000 && recovery.Trees.Any(t=>t.Salvage);i++){recovery.Tick(.1f);recovery.Validate();}
        Check(!recovery.Trees.Any(t=>t.Salvage),"Canceled materials not collected");
        // The salvage collector may still stand on the cleared footprint for a tick.
        for(int i=0;i<1200 && recovery.PlacementProblem(new(0,-3),0,BuildingKind.GatheringHall)!=null;i++){recovery.Tick(.1f);recovery.Validate();}
        var rebuilt=recovery.Place(new(0,-3),0,BuildingKind.GatheringHall);Check(rebuilt!=null,"Recovered plot still blocked: "+recovery.PlacementProblem(new(0,-3),0,BuildingKind.GatheringHall));
        for(int i=0;i<12000 && !rebuilt!.Complete;i++){recovery.Tick(.1f);recovery.Validate();}
        Check(rebuilt!.Complete,"Canceled project could not be rebuilt");
        Console.WriteLine($"PASS: paused mixed-material project, canceled {deliveredPlanks} planks/{deliveredStone} stone, saved physical salvage, collected and rebuilt at {recovery.Food.Time:F0}s.");
        Console.WriteLine("PASS: four existing-system civic budget probes, actual construction and food/service recovery, exact saves and continuation. Learning/identity mechanics are not implemented by these probes.");
    }
}

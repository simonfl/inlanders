using Inlanders.Simulation;
using System.Text.Json;

// A route experiment using ordinary bridge construction, not a new restoration system.
static class RestorationChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    static World Start()
    {
        var w=World.NewQuarryMap();w.Map.Name="Northern crossing prototype";
        // Keep a downstream causeway at z=7, beyond the yard-to-quarry route.
        for(int x=6;x<=9;x++)for(int z=2;z<=7;z++)w.Map.Excluded.Remove(new(x,z));
        for(int z=-3;z<=6;z++)w.Map.Water.Add(new(6,z));
        w.Validate();return World.LoadJson(w.SaveJson());
    }
    static void Roundtrip(World w)
    {
        string saved=w.SaveJson();var copy=World.LoadJson(saved);Check(copy.SaveJson()==saved,"Roundtrip differs");
        var control=World.LoadJson(saved);
        for(int i=0;i<200;i++){copy.Tick(.1f);control.Tick(.1f);}
        Check(copy.SaveJson()==control.SaveJson(),"Continuation differs");
    }
    public static void Run()
    {
        Directory.CreateDirectory("artifacts/restoration");
        var reports=new List<object>();File.WriteAllText("artifacts/restoration/start.json",Start().SaveJson());
        foreach(string route in new[]{"existing-route","paved-detour","crossing-first","crossing-overlap","food-diversion"})
        {
            var w=Start();bool bridge=route.StartsWith("crossing") || route=="food-diversion",poor=route=="food-diversion";
            var orders=new List<int>();int logs=0,planks=0,stone=0;
            Cottage Build(Cell cell,BuildingKind kind,int rotation=0)
            {
                var site=w.Place(cell,rotation,kind)??throw new Exception($"{route}: {kind}: {w.PlacementProblem(cell,rotation,kind)}");
                orders.Add(site.Id);if(site.Material==Resource.Logs)logs+=site.Required;else planks+=site.Required;stone+=site.RequiredStone;return site;
            }
            var target=new Cell(11,-3); // Entrance of the remote quarry at (11,-4).
            int Distance()=>w.RestorationProbePath(target)?.Count??throw new Exception("Existing route not reachable");
            int distanceBefore=Distance();float crossingComplete=-1;int crossings=0;double travel=0;
            void Tick()
            {
                w.Tick(.1f);w.Validate();travel+=w.People.Count(p=>p.Route.Count>0)*.1;
                crossings+=w.People.Count(p=>World.At(p)==new Cell(6,-3));
            }
            if(route=="paved-detour")
            {
                var path=w.RestorationProbePath(target)!;
                foreach(var cell in path)Check(w.SetPath(cell,true),"Path refused");
            }
            Cottage? crossing=bridge?Build(new(6,-3),BuildingKind.Bridge,1):null;
            if(route=="crossing-first")
            {
                for(int i=0;i<6000 && !crossing!.Complete;i++)Tick();Check(crossing!.Complete,"Crossing stalled");
                crossingComplete=w.Food.Time;Roundtrip(w);File.WriteAllText("artifacts/restoration/crossing-stage.json",w.SaveJson());
            }
            Build(new(3,-5),BuildingKind.Sawmill);Build(new(11,-4),BuildingKind.Quarry);
            var hall=Build(new(0,-3),BuildingKind.GatheringHall);
            Check(logs==(bridge?18:12) && planks==8 && stone==12,"Budget changed");
            if(poor){w.Assign(3,Role.Quarrier);w.Assign(4,Role.Sawyer);w.Assign(5,Role.Builder);}
            else{w.Assign(6,Role.Quarrier);w.Assign(7,Role.Sawyer);}
            float builtAt=-1,supportedAt=-1;int missesBeforeRepair=0;
            for(int i=0;i<24000;i++)
            {
                Tick();
                if(crossingComplete<0 && crossing?.Complete==true)crossingComplete=w.Food.Time;
                bool built=orders.All(id=>w.Cottages.Single(c=>c.Id==id).Complete);
                if(builtAt<0 && built)builtAt=w.Food.Time;
                if(poor && i==8999)
                {
                    var assessment=w.ReadMealAssessment();missesBeforeRepair=assessment.Missed+assessment.Skipped;
                    Check(missesBeforeRepair>0,"Diversion did not expose food cost");
                    Roundtrip(w);File.WriteAllText("artifacts/restoration/food-before-repair.json",w.SaveJson());
                    w=World.LoadJson(w.SaveJson());w.Assign(3,Role.Forager);w.Assign(4,Role.Forager);w.Assign(5,Role.Farmer);w.Assign(6,Role.Quarrier);w.Assign(7,Role.Sawyer);
                }
                var meals=w.ReadMealAssessment();
                if(built && (!poor || i>8999) && meals.Reliable && meals.FreshSupply && w.People.Any(p=>p.LastLeisureSiteId==hall.Id && p.LastLeisureTime is float t && w.Food.Time-t<p.LastLeisureWindow))
                {supportedAt=w.Food.Time;break;}
            }
            Check(supportedAt>0,"Route failed to support attended project: "+route);Roundtrip(w);
            Check(!bridge || crossings>0,"Built crossing never used");
            Check(!bridge || Distance()<distanceBefore,"Crossing does not shorten the actual route");
            var report=new{route,logs,planks,stone,minimumRawLogs=logs+planks/2,pathTiles=w.Paths.Count,distanceBefore,distanceAfter=Distance(),crossingComplete,builtAt,supportedAt,travelPersonSeconds=Math.Round(travel,1),crossingOccupancySamples=crossings,missesBeforeRepair,meals=w.ReadMealAssessment()};
            reports.Add(report);Console.WriteLine(JsonSerializer.Serialize(report));File.WriteAllText($"artifacts/restoration/{route}.json",w.SaveJson());
        }
        // Pausing/canceling the incomplete access investment must preserve its materials.
        var recovery=Start();var partial=recovery.Place(new(6,-3),1,BuildingKind.Bridge)!;
        for(int i=0;i<5000 && partial.Delivered==0;i++)recovery.Tick(.1f);
        Check(partial.Delivered>0 && !partial.Complete,"No partial crossing");
        int delivered=partial.Delivered;recovery.Assign(1,Role.Unassigned);recovery.Assign(2,Role.Unassigned);
        float progress=partial.Construction;
        for(int i=0;i<300;i++)recovery.Tick(.1f);
        Check(partial.Construction==progress,"Unstaffed crossing advanced");
        Roundtrip(recovery);Check(recovery.Cancel(partial.Id),"Partial crossing cancellation failed");
        Check(recovery.Trees.Where(t=>t.Salvage && t.Material==Resource.Logs).Sum(t=>t.Logs)==delivered,"Crossing cancellation lost logs");
        recovery=World.LoadJson(recovery.SaveJson());recovery.Assign(1,Role.Builder);recovery.Assign(2,Role.Builder);
        for(int i=0;i<6000 && recovery.Trees.Any(t=>t.Salvage);i++)recovery.Tick(.1f);
        Check(!recovery.Trees.Any(t=>t.Salvage),"Crossing salvage stranded");
        for(int i=0;i<1000 && recovery.PlacementProblem(new(6,-3),1,BuildingKind.Bridge)!=null;i++)recovery.Tick(.1f);
        var rebuilt=recovery.Place(new(6,-3),1,BuildingKind.Bridge);Check(rebuilt!=null,"Crossing cannot be rebuilt");
        for(int i=0;i<6000 && !rebuilt!.Complete;i++)recovery.Tick(.1f);
        Check(rebuilt!.Complete,"Rebuilt crossing stalled");recovery.Validate();Roundtrip(recovery);
        reports.Add(new{route="cancel-rebuild",deliveredLogsRecovered=delivered,completedAt=recovery.Food.Time});
        File.WriteAllText("artifacts/restoration/results.json",JsonSerializer.Serialize(reports,new JsonSerializerOptions{WriteIndented=true}));
        Console.WriteLine("PASS: restoration proxy route alternatives, food diversion/recovery, actual crossing/hall use, budget, exact saves and partial cancellation/rebuild.");
    }
}

namespace Inlanders.Simulation
{
    public sealed partial class World
    {
        // Expose the actual pathfinder to this test assembly, not the game API.
        public List<Cell>? RestorationProbePath(Cell destination)=>FindPath(YardAccess,destination,Blocked);
    }
}
using Inlanders.Simulation;

static class QuarryBriefChecks
{
    public static void Run()
    {
        var w=World.NewQuarryMap();w.Validate();
        if(w.YardLogs!=16 || w.YardPlanks!=0 || w.Stone!=0 || w.Trees.Sum(t=>t.Logs)!=64 || w.Housed!=8)throw new Exception("Quarry brief starting budget changed");
        if(w.Map.StoneDeposits[0].Remaining>=Buildings.Get(BuildingKind.GatheringHall).StoneCost)throw new Exception("Near source must require expansion");
        foreach(var (cell,kind) in new[]{(new Cell(-4,-4),BuildingKind.Quarry),(new Cell(11,-4),BuildingKind.Quarry),(new Cell(3,-5),BuildingKind.Sawmill),(new Cell(0,-3),BuildingKind.GatheringHall)})
            if(w.Place(cell,0,kind)==null)throw new Exception($"Proposed plot unavailable: {kind} {cell}: {w.PlacementProblem(cell,0,kind)}");
        string saved=w.SaveJson();if(World.LoadJson(saved).SaveJson()!=saved)throw new Exception("Quarry prototype roundtrip differs");
        Console.WriteLine("PASS: quarry brief map occupancy, simultaneous proposed plots, starter budget, two sources and exact saved layout. Progression/routes remain to implement.");
        foreach(bool nearby in new[]{true,false})
        {
            w=World.NewQuarryMap();
            w.Place(new(3,-5),0,BuildingKind.Sawmill);
            if(nearby)w.Place(new(-4,-4),0,BuildingKind.Quarry);
            w.Place(new(11,-4),0,BuildingKind.Quarry);
            var hall=w.Place(new(0,-3),0,BuildingKind.GatheringHall)!;
            w.Assign(6,Role.Quarrier);w.Assign(7,Role.Sawyer);
            for(int i=0;i<18000;i++)
            {
                w.Tick(.1f);w.Validate();
                if(hall.Complete && w.People.Count(p=>p.LastLeisureSiteId==hall.Id && p.LastLeisureTime is float t && w.Food.Time-t<240)>=4)break;
            }
            int visits=w.People.Count(p=>p.LastLeisureSiteId==hall.Id && p.LastLeisureTime is float t && w.Food.Time-t<240);
            if(!hall.Complete || visits<4)throw new Exception($"Quarry brief {(nearby?"two-camp":"remote-only")} route failed: {w.Food.Time}s, stone {w.Stone}, planks {w.Planks}, hall {hall.Construction}, visits {visits}");
            Console.WriteLine($"Quarry brief {(nearby?"two-camp":"remote-only")}: hall + {visits} recent visitors at {w.Food.Time:F0}s; deposits {string.Join('/',w.Map.StoneDeposits.Select(d=>d.Remaining))}, yard logs {w.YardLogs}.");
        }
    }
}

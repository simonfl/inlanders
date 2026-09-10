using Inlanders.Simulation;

public static class LocalStorageExperiments
{
    public static void Run()
    {
        var food=World.NewCampaign(2);
        SupplyRoute? delivery=null;
        for(int i=0;i<3000 && delivery==null;i++) { food.Tick(.1f); delivery=food.ReadSupplyRoutes().FirstOrDefault(r=>r.Cargo==Resource.Berries && r.Destination=="Central pantry"); }
        if(delivery==null) throw new Exception("Food relay comparison missed a berry delivery");
        // Best-case on-route relay, equal two-unit loads, each worker returning to its source.
        // Splitting one round trip cannot reduce the sum of its two round trips; detours/setup only add work.
        float direct=delivery.Distance*2, relay=(delivery.Distance/2*2)+(delivery.Distance/2*2);
        Console.WriteLine($"FOOD relay lower bound: direct {direct:0.0} vs split {relay:0.0} round-trip worker-tiles per {delivery.Amount} berries; excludes relay setup/handling and job scheduling. Larger loads or local meals would be a different design.");
        foreach(bool remote in new[]{false,true})
        foreach(int variant in new[]{0,1,2})
        {
            var w=World.NewCampaign(3); w.Food.InitialBerries=w.Food.Berries=1000;
            var millCell=remote?new Cell(9,0):new(-4,2);
            var depotCell=remote?new Cell(9,3):new(3,-3);
            var mill=w.Place(millCell,false,BuildingKind.Sawmill) ?? throw new Exception($"Mill {millCell}: {w.PlacementProblem(millCell,false,BuildingKind.Sawmill)}");
            Cottage? depot=variant==0?null:w.Place(variant==2?new(0,6):depotCell,false,BuildingKind.Stockpile) ?? throw new Exception("Depot rejected");
            if(depot!=null && !w.SetStorageMaterial(depot.Id,Resource.Planks)) throw new Exception("Planned material selection rejected");
            w.Assign(6,Role.Sawyer);
            var homes=remote?new[]{new Cell(12,0),new(12,3),new(12,6)}:new[]{new Cell(0,6),new(3,6),new(6,6)};
            // Reserve the depot footprint in all comparisons, including the awkward variant.
            if(!remote) homes=new[]{new Cell(3,6),new(6,6),new(-5,6)};
            foreach(var tree in w.Trees.Where(t=>homes.Any(c=>World.Footprint(c,false,BuildingKind.Lodge).Append(World.Door(c,false)).Contains(t.Cell))).ToArray()) w.SetClearing(tree.Cell,true);
            int initialTimber=w.Trees.Sum(t=>t.Logs);
            int timberGoal=Buildings.Get(BuildingKind.Sawmill).Cost+18+(depot==null?0:Buildings.Get(BuildingKind.Stockpile).Cost)+12;
            var workers=new[]{0,1,2,3,6};
            double travel=0,active=0; int wave=0; Cottage? lodge=null; float completed=0;
            for(int i=0;i<18000;i++)
            {
                if(depot?.Complete==true && depot.StorageMaterial!=Resource.Planks && !w.SetStorageMaterial(depot.Id,Resource.Planks))
                {
                    // Configure as soon as construction finishes, before the next claim tick.
                    throw new Exception("New depot already committed to log traffic");
                }
                if(mill.Complete) w.SetOutputTarget(mill.Id,w.SawnLogs>=18?0:36-w.Cottages.Where(c=>c.Kind==BuildingKind.Lodge).Sum(c=>c.Delivered));
                if(initialTimber-w.Trees.Sum(t=>t.Logs)>=timberGoal)
                    foreach(var p in w.People.Take(2).Where(p=>p.Role==Role.Logger)) w.Assign(p.Id,Role.Unassigned);
                if(lodge==null && (depot==null || depot.Complete)) lodge=w.Place(homes[wave],false,BuildingKind.Lodge);
                w.Tick(.1f); w.Validate();
                travel+=.1*w.People.Where(p=>workers.Contains(p.Id)).Count(p=>p.Route.Count>0);
                active+=.1*w.People.Where(p=>workers.Contains(p.Id)).Count(p=>p.Task!=Work.Waiting);
                if(lodge?.Complete==true) { wave++; if(wave==3) { completed=w.Food.Time; break; } lodge=null; }
            }
            if(completed==0) throw new Exception($"Local storage experiment stalled: wave {wave}, placement {w.PlacementProblem(homes[wave],false,BuildingKind.Lodge)}, logs {w.Stored}, planks {w.Planks}; "+string.Join("; ",w.People.Select(p=>$"{p.Role}: {p.Status}")));
            CheckOutput(w);
            Console.WriteLine($"PLANK {(remote?"remote":"compact")} variant={variant}: complete {completed:0.0}s, travel {travel:0.0} person-s, active {active:0.0} person-s, sawn {w.SawnLogs}, stored {w.Planks}, logs {w.Stored}, rest {w.People.Sum(p=>p.RestVisits)}");
        }
    }
    static void CheckOutput(World w) { if(w.SawnLogs!=18 || w.Planks!=0 || w.Cottages.Where(c=>c.Kind==BuildingKind.Lodge).Sum(c=>c.Delivered)!=36) throw new Exception("Unequal useful plank output"); }
}

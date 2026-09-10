using Inlanders.Simulation;

public static class FishingBalance
{
    public static void Run()
    {
        foreach(var (name,cell,kind) in new[]{("near landing",new Cell(3,4),BuildingKind.FishingDock),("far landing",new Cell(14,0),BuildingKind.FishingDock),("near garden",new Cell(0,-3),BuildingKind.VegetableGarden)})
        {
            var w=World.NewLakeMap();
            var dock=w.Place(cell,kind==BuildingKind.FishingDock,kind) ?? throw new Exception(w.PlacementProblem(cell,kind==BuildingKind.FishingDock,kind));
            while(!dock.Complete && w.Food.Time<600) w.Tick(.1f);
            if(!dock.Complete) throw new Exception("Dock construction stalled");
            w.Assign(6,kind==BuildingKind.FishingDock?Role.Fisher:Role.Farmer);
            float start=w.Food.Time,boatTravel=0,landTravel=0; int rests=w.People[6].RestVisits;
            for(int i=0;i<7200;i++)
            {
                var p=w.People[6];
                if(p.Task==Work.Aboard && dock.Boat!.Route.Count>0) boatTravel+=.1f;
                if(p.Route.Count>0) landTravel+=.1f;
                w.Tick(.1f); w.Validate();
            }
            Console.WriteLine($"FISH BALANCE {name}: cost {dock.Required} logs; built {start:0}s; 12 minutes: {(kind==BuildingKind.FishingDock?w.DeliveredFish:w.DeliveredVegetables)} food delivered, boat travel {boatTravel:0}s, land travel {landTravel:0}s, {w.People[6].RestVisits-rests} rests; stocks {string.Join(", ",w.Map.FishingGrounds.Select(g=>$"{g.Name} {g.Stock:0.0}"))}.");
        }
    }
}

using Inlanders.Simulation;

public static class SupplyRouteChecks
{
    public static void Run()
    {
        var w=World.NewCampaign(2);
        var mill=w.Place(new(3,-3),false,BuildingKind.Sawmill) ?? throw new Exception("Route mill fixture rejected");
        var store=w.Place(new(6,-3),false,BuildingKind.Stockpile) ?? throw new Exception("Route store fixture rejected");
        w.Assign(6,Role.Sawyer); w.Assign(7,Role.Hauler);
        var observed=new HashSet<Work>();
        for(int i=0;i<6000;i++)
        {
            w.Tick(.1f);
            foreach(var route in w.ReadSupplyRoutes())
            {
                var p=w.People[route.WorkerId]; observed.Add(p.Task);
                if(!route.Steps.SequenceEqual(p.Route) || route.Start!=p.Position || route.Amount!=p.Carried || route.Distance<0)
                    throw new Exception("Supply report invented a route or cargo");
                if(p.Task==Work.ToPantry && route.Destination!="Central pantry") throw new Exception("Food route does not explain central delivery");
                if(p.Task==Work.ToStockpile && p.Cargo==Resource.Planks && route.Destination!="Central plank store") throw new Exception("Planks falsely described as local storage");
            }
            if(i%100==0)
            {
                string before=w.SaveJson(); var report=w.ReadSupplyRoutes();
                if(report.Length>0) report[0].Steps[0]=new(999,999);
                if(w.SaveJson()!=before) throw new Exception("Route observation changed simulation state");
            }
        }
        foreach(var task in new[]{Work.ToTree,Work.ToStockpile,Work.ToPantry,Work.ToSawLogs,Work.ToHaulPickup,Work.ToHaulDrop})
            if(!observed.Contains(task)) throw new Exception($"Route fixture missed {task}");
        w.Validate();
        var lake=World.NewCampaign(7); var dock=lake.Place(new(14,0),true,BuildingKind.FishingDock)!;
        for(int i=0;i<6000 && !dock.Complete;i++) lake.Tick(.1f);
        if(!dock.Complete) throw new Exception("Route dock fixture stalled");
        foreach(var p in lake.People) lake.Assign(p.Id,Role.Unassigned);
        lake.Assign(0,Role.Fisher); bool outbound=false,returning=false;
        for(int i=0;i<3000 && !returning;i++)
        {
            lake.Tick(.1f);
            foreach(var route in lake.ReadSupplyRoutes().Where(r=>r.OnWater))
            {
                if(!route.Steps.SequenceEqual(dock.Boat!.Route) || route.Start!=dock.Boat.Position || route.Amount!=dock.Boat.Fish)
                    throw new Exception("Boat route does not follow actual vessel/catch");
                outbound|=route.Destination=="Fishing ground" && route.Amount==0;
                returning|=route.Destination=="Fishing dock" && route.Amount>0;
            }
        }
        if(!outbound || !returning) throw new Exception("Boat route fixture missed outbound/return legs");
        Console.WriteLine("PASS: live supply route/cargo snapshots, local logs versus central food/planks, hauling and read-only detached paths.");
    }
}

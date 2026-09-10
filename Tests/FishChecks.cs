using Inlanders.Simulation;

public static class FishChecks
{
    static void Check(bool ok,string why) { if(!ok) throw new Exception(why); }
    public static void Run()
    {
        var lake=World.NewLakeMap();
        Check(lake.Map.FishingGrounds.Count==2 && lake.Housed==8 && World.LoadJson(lake.SaveJson()).SaveJson()==lake.SaveJson(),"Authored lake opening failed");
        var west = new Cell(3,4); var east = new Cell(14,0);
        Check(lake.DockLaunch(west,true)==new Cell(4,4) && lake.DockEntrance(west,true)==new Cell(2,4),"West shore dock faces away from water");
        Check(lake.DockLaunch(east,true)==new Cell(13,0) && lake.DockEntrance(east,true)==new Cell(15,0),"East shore dock faces away from water");
        Check(lake.DockProblem(east,true)==null,"East shore landing rejected");
        Check(lake.DockProblem(east,false)!=null,"Inland-facing dock accepted");
        var dock=lake.Place(east,true,BuildingKind.FishingDock);
        Check(dock!=null && dock.Entrance==new Cell(15,0) && dock.Launch==new Cell(13,0),"Placed dock lost shore orientation");
        for(int i=0;i<6000 && !dock!.Complete;i++) lake.Tick(.1f);
        Check(dock!.Complete,"Normal builders did not construct the dock");
        Check(World.LoadJson(lake.SaveJson()).SaveJson()==lake.SaveJson(),"Constructed dock save changed");
        var cancelled=World.LoadJson(lake.SaveJson());
        var unfinished=cancelled.Place(new(3,4),true,BuildingKind.FishingDock);
        Check(unfinished!=null,"West dock cancellation fixture rejected");
        for(int i=0;i<3000 && unfinished!.Delivered==0;i++) cancelled.Tick(.1f);
        Check(unfinished!.Delivered>0 && cancelled.Cancel(unfinished.Id),"Part-delivered shore dock could not be cancelled");
        Check(World.LoadJson(cancelled.SaveJson()).SaveJson()==cancelled.SaveJson(),"Cancelled dock left salvage with underwater access");
        var fishing=World.LoadJson(lake.SaveJson());
        foreach(var p in fishing.People) fishing.Assign(p.Id,Role.Unassigned);
        fishing.Assign(0,Role.Fisher);
        var phases=new HashSet<string>();
        for(int i=0;i<4000 && fishing.DeliveredFish==0;i++)
        {
            fishing.Tick(.1f); fishing.Validate();
            var boat=fishing.Cottages.Single(c=>c.Id==dock.Id).Boat;
            if(boat?.FisherId==null) continue;
            string phase=fishing.People[0].Task==Work.ToDock ? "Walking" : boat.Phase.ToString();
            if(!phases.Add(phase)) continue;
            string snapshot=fishing.SaveJson(); var replay=World.LoadJson(snapshot);
            Check(replay.SaveJson()==snapshot,$"Boat save changed in {phase}");
            var interrupted=World.LoadJson(snapshot); var aboard=interrupted.People[0].Task==Work.Aboard;
            interrupted.Assign(0,Role.Builder);
            Check(!aboard || interrupted.People[0].Task==Work.Aboard,$"Reassignment teleported fisher in {phase}");
            for(int step=0;step<1500 && interrupted.Cottages.Single(c=>c.Id==dock.Id).Boat?.FisherId!=null;step++)
            { interrupted.Tick(.1f); interrupted.Validate(); }
            Check(interrupted.Cottages.Single(c=>c.Id==dock.Id).Boat?.FisherId==null,$"Reassigned fisher stranded in {phase}");
            var paused=World.LoadJson(snapshot);
            Check(!paused.RequestDemolition(dock.Id),"Occupied dock demolished");
            Check(paused.SetWorkplacePaused(dock.Id,true),"Dock pause rejected");
            for(int step=0;step<1500 && paused.Cottages.Single(c=>c.Id==dock.Id).Boat?.FisherId!=null;step++)
            { paused.Tick(.1f); paused.Validate(); }
            Check(paused.RequestDemolition(dock.Id),$"Returned dock could not be demolished in {phase}");
            if(aboard)
            {
                var supper=World.LoadJson(snapshot);
                supper.Food.Bread+=supper.SupperCost; supper.Food.BakedBread+=supper.SupperCost;
                supper.Food.UsedGrain+=supper.SupperCost/2; supper.Food.GrownGrain+=supper.SupperCost/2;
                Check(supper.BeginSupper(),"Supper with fisher at sea rejected");
                Check(supper.People[0].Task==Work.Aboard,"Supper teleported fisher ashore");
                for(int step=0;step<3000 && !supper.Food.SupperComplete;step++) { supper.Tick(.1f); supper.Validate(); }
                Check(supper.Food.SupperComplete,"Returning fisher prevented supper");
            }
            var continued=World.LoadJson(snapshot);
            for(int step=0;step<100;step++) { continued.Tick(.1f); replay.Tick(.1f); }
            Check(continued.SaveJson()==replay.SaveJson(),$"Boat continuation diverged in {phase}");
        }
        Check(fishing.DeliveredFish>0 && phases.IsSupersetOf(new[]{"Walking","Outbound","Fishing","Returning"}),"Complete fishing trip phases were not exercised");
        var shared=World.LoadJson(lake.SaveJson());
        var second=shared.Place(new(14,2),true,BuildingKind.FishingDock);
        Check(second!=null,"Second shore dock rejected");
        for(int i=0;i<6000 && !second!.Complete;i++) shared.Tick(.1f);
        Check(second!.Complete,"Second dock construction stalled");
        foreach(var p in shared.People) { shared.Assign(p.Id,Role.Unassigned); p.NextRestTime=shared.Food.Time+1000; }
        foreach(var g in shared.Map.FishingGrounds) g.Stock=0;
        shared.Map.FishingGrounds[1].Stock=2;
        shared.Assign(0,Role.Fisher); shared.Assign(1,Role.Fisher);
        shared.Tick(.1f); shared.Validate();
        Check(shared.Cottages.Sum(c=>c.Boat?.ReservedCatch??0)==2 && shared.Cottages.Count(c=>c.Boat?.FisherId!=null)==1,"Competing docks claimed the same catch");
        int owner=shared.Cottages.Single(c=>c.Boat?.FisherId!=null).Boat!.FisherId!.Value;
        shared.Assign(owner,Role.Unassigned); shared.Validate();
        Check(shared.AvailableFish(shared.Map.FishingGrounds[1])==2,"Cancelled walk did not release fish claim");
        foreach(var c in shared.Cottages.Where(c=>c.Kind==BuildingKind.FishingDock)) Check(shared.SetOutputTarget(c.Id,0),"Fish target unavailable");
        for(int i=0;i<20;i++) shared.Tick(.1f);
        Check(shared.Cottages.All(c=>c.Boat?.FisherId==null),"Fish target allowed another trip");
        var route=lake.FindBoatRoute(new(4,4),new(11,1));
        Check(route!=null && route.Count>0 && route.All(lake.Map.Water.Contains),"Boat cannot reach lake habitat over water");
        Check(lake.FindBoatRoute(east,new(11,1))==null,"Boat launched on dry land");
        var channel=new World(0);
        channel.Map.Water.UnionWith(new[] { new Cell(5,0),new(5,1),new(5,2) });
        Check(channel.FindBoatRoute(new(5,0),new(5,2))?.Count==2,"Open channel is not navigable");
        channel.Cottages.Add(new Cottage { Id=100,Kind=BuildingKind.Bridge,Cell=new(5,1) });
        Check(channel.FindBoatRoute(new(5,0),new(5,2))==null,"Boat route crosses a planned bridge");
        Check(channel.FindBoatRoute(new(5,1),new(5,1))==null,"Blocked launch accepted as an empty route");
        var crossing=World.NewCreative();
        crossing.Map.Water.UnionWith(new[]{new Cell(5,0),new(5,1),new(5,2)});
        crossing.Map.FishingGrounds.Add(new FishHabitat { Id=0,Cell=new(5,2) });
        var landing=crossing.Place(new(4,0),true,BuildingKind.FishingDock)!;
        Check(landing!=null,"Channel dock rejected");
        foreach(var p in crossing.People) crossing.Assign(p.Id,Role.Unassigned);
        crossing.Assign(0,Role.Fisher);
        for(int i=0;i<1000 && landing!.Boat?.Phase!=BoatPhase.Fishing;i++) crossing.Tick(.1f);
        Check(landing!.Boat?.Phase==BoatPhase.Fishing,"Channel trip stalled");
        Check(crossing.Place(new(5,1),true,BuildingKind.Bridge)==null,"Bridge stranded fisher behind crossing");
        crossing.SetWorkplacePaused(landing.Id,true);
        for(int i=0;i<1000 && landing.Boat!.FisherId!=null;i++) { crossing.Tick(.1f); crossing.Validate(); }
        Check(crossing.Place(new(5,1),true,BuildingKind.Bridge)!=null,"Returned boat unnecessarily blocked crossing");
        crossing.SetWorkplacePaused(landing.Id,false); crossing.Tick(1); crossing.Validate();
        Check(crossing.ReadWorkplace(landing).State=="No reachable fishing ground","Blocked fishing route not explained");
        var water=World.NewLargeMap(true,false);
        var ground=new FishHabitat { Id=0,Name="Test shallows",Cell=new(7,0),Capacity=8,Stock=8,RegrowthPerSecond=.1f };
        water.Map.FishingGrounds.Add(ground); water.Map.Validate();
        Check(ground.Take(6)==6 && water.Map.FishingGrounds.Single().Take(6)==2 && ground.Take(1)==0,"Catch overdrew shared stock");
        ground.Advance(30); Check(ground.Available==3,"Fish did not replenish at habitat rate");
        ground.Advance(1000); Check(ground.Stock==8,"Fish exceeded habitat capacity");
        string saved=water.SaveJson(); var restored=World.LoadJson(saved);
        Check(restored.SaveJson()==saved,"Habitat save changed");
        ground.Stock=-1;
        try { water.Map.Validate(); throw new Exception("Negative habitat loaded"); } catch(System.IO.InvalidDataException) { }
        World Meal(int berries,int veg,int bread,int fish)
        {
            var w=new World(0); foreach(var p in w.People) w.Assign(p.Id,Role.Unassigned);
            w.Food.Berries=w.Food.InitialBerries=berries;
            w.Food.Vegetables=w.Food.GrownVegetables=veg;
            w.Food.Bread=w.Food.BakedBread=bread; w.Food.UsedGrain=w.Food.GrownGrain=bread/2;
            w.Food.Fish=w.Food.CaughtFish=fish; w.Food.MealClock=59.9f; w.Tick(.2f); w.Validate(); return w;
        }
        var pair=Meal(4,0,0,4);
        Check(pair.Food.Hunger==0 && pair.Food.LastMealFish==4 && pair.MealVarietyScore==16,"Fish did not serve as a mixed meal");
        Check(Meal(3,3,2,0).MealVarietyScore==20,"Fourth food weakened an existing balanced diet");
        var all=Meal(2,2,2,2);
        Check(all.Food.LastMealChoices==4 && all.MealVarietyScore==20 && all.LastMealSummary.Contains("2 fish"),"Four-food meal not explained");
        Check(Meal(7,0,0,1).MealVarietyScore==4,"A token fish earned excessive variety");
        Check(Meal(0,0,0,8).Food.Hunger==0 && Meal(0,0,0,4).Food.Hunger==.5f,"Fish portions did not match hunger");
        var delivery=new World(0); foreach(var p in delivery.People) delivery.Assign(p.Id,Role.Unassigned);
        delivery.Food.CaughtFish=4; delivery.People[0].Cargo=Resource.Fish; delivery.People[0].Carried=4;
        delivery.Assign(0,Role.Builder);
        Check(delivery.DeliveredFish==0 && delivery.ReadEconomy().Stocks.Single(s=>s.Resource==Resource.Fish).Carried==4,"Carried fish counted as pantry stock");
        saved=delivery.SaveJson(); Check(World.LoadJson(saved).SaveJson()==saved,"Carried fish save changed");
        for(int i=0;i<500 && delivery.People[0].Carried>0;i++) delivery.Tick(.1f);
        delivery.Validate(); Check(delivery.DeliveredFish==4 && delivery.ReadFoodFlow().Fish==4,"Returned fish missing from actual deliveries");
        Console.WriteLine("PASS: shared habitat and catch claims, dock construction, complete boat trips, phase saves/reassignment/recall/supper, bridge protection, targets, fish meals and actual pantry deliveries.");
    }
}

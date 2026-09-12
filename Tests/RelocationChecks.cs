using Inlanders.Simulation;
using System.Text.Json;
using System.Text.Json.Nodes;

static class RelocationChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    static void Step(World w,int ticks=1){for(int i=0;i<ticks;i++){w.Tick(.1f);w.Validate();}}
    static Cell Destination(World w,Cottage site,int rotation)
    {
        string saved=w.SaveJson();
        foreach(var at in w.Map.Land.Where(c=>c!=site.Cell))
        {
            var problem=w.RelocationProblem(site.Id,at,rotation);Check(w.SaveJson()==saved,"Destination query changed world");
            if(problem==null)return at;
        }
        throw new Exception("No move destination for "+site.Kind);
    }
    static void Roundtrip(World w)
    {
        string saved=w.SaveJson();var copy=World.LoadJson(saved);Check(copy.SaveJson()==saved,"Moved save changed on load");
        var twin=World.LoadJson(saved);Step(copy,150);Step(twin,150);Check(copy.SaveJson()==twin.SaveJson(),"Moved save continuation differs");
    }
    public static void Run()
    {
        var ordinary=World.NewScenario();Check(!ordinary.MoveBuilding(1,new(3,0),0),"Normal mode allowed relocation");
        foreach(var kind in new[]{BuildingKind.Cottage,BuildingKind.Orchard,BuildingKind.Pantry,BuildingKind.Stockpile,BuildingKind.GatheringHall,BuildingKind.Bakery,BuildingKind.Carpenter})
        {
            var w=World.NewCreative(true);foreach(var p in w.People)w.Assign(p.Id,Role.Unassigned);
            var at=w.Map.Land.First(c=>w.PlacementProblem(c,0,kind)==null);var site=w.Place(at,0,kind)!;
            if(kind==BuildingKind.Orchard){site.Planted=true;site.OrchardMature=true;site.Growth=1;site.Harvest=8;w.Food.GrownFruit=8;}
            if(kind==BuildingKind.Pantry){site.PantryFood[5]=8;w.Food.GrownFruit=8;}
            if(kind==BuildingKind.Stockpile){var timber=w.Trees.First(t=>t.Logs>=6);timber.Logs-=6;if(timber.Logs==0)timber.Felled=true;site.StoredLogs=6;}
            if(kind==BuildingKind.Bakery){site.InputGrain=2;w.Food.GrownGrain=2;}
            int targetStock=World.ProductionOutput(kind)!=null?24:-1;var finish=kind==BuildingKind.Cottage?(CottageFinish)2:CottageFinish.Automatic;
            site.Priority=2;site.OutputTarget=targetStock;site.StorageTarget=10;site.Finish=finish;
            w.Validate();
            for(int rotation=0;rotation<4;rotation++)
            {
                string before=w.SaveJson();Check(w.MoveBuilding(site.Id,site.Cell,site.Rotation) && w.SaveJson()==before,"No-op move changed state");
                Check(!w.MoveBuilding(site.Id,new(999,999),rotation) && w.SaveJson()==before,"Failed move changed state");
                var target=Destination(w,site,rotation);
                int fruit=site.Harvest,localFruit=site.PantryFood[5],logs=site.StoredLogs,grain=site.InputGrain;
                Check(w.MoveBuilding(site.Id,target,rotation),"Move rejected checked destination");w.Validate();
                Check(ReferenceEquals(site,w.Cottages.Single(c=>c.Id==site.Id)) && site.Cell==target && site.Rotation==rotation,"Move replaced identity");
                Check(site.Harvest==fruit && site.PantryFood[5]==localFruit && site.StoredLogs==logs && site.InputGrain==grain && site.Priority==2 && site.OutputTarget==targetStock && site.StorageTarget==10 && site.Finish==finish,"Move changed stored state");
                Roundtrip(w);
            }
        }
        var live=World.NewCreative(true);var plot=live.Map.Land.First(c=>live.PlacementProblem(c,0,BuildingKind.Orchard)==null);var grove=live.Place(plot,0,BuildingKind.Orchard)!;
        live.Assign(6,Role.Farmer);
        for(int i=0;i<8000 && !live.People.Any(p=>p.Cargo==Resource.Fruit && p.Carried>0);i++)Step(live);
        Check(live.People.Any(p=>p.Cargo==Resource.Fruit && p.Carried>0),"No live fruit carrier");
        int produced=live.Food.GrownFruit;live.SetWorkplacePaused(grove.Id,true);
        Check(live.MoveBuilding(grove.Id,Destination(live,grove,1),1),"Cannot move active grove");Step(live,600);
        Check(live.Food.GrownFruit==produced && grove.OrchardMature,"Move restarted or lost mature grove");Roundtrip(live);
        var hauling=World.NewCreative(true);var depot=hauling.Place(hauling.Map.Land.First(c=>hauling.PlacementProblem(c,0,BuildingKind.Stockpile)==null),0,BuildingKind.Stockpile)!;
        hauling.Assign(6,Role.Hauler);
        for(int i=0;i<10000 && !hauling.People.Any(p=>p.StorageId==depot.Id && p.Task==Work.ToStockpile && p.Carried>0);i++)Step(hauling);
        var hauler=hauling.People.FirstOrDefault(p=>p.StorageId==depot.Id && p.Task==Work.ToStockpile && p.Carried>0);Check(hauler!=null,"No stockpile delivery fixture");
        Check(hauling.MoveBuilding(depot.Id,Destination(hauling,depot,2),2),"Stockpile delivery move failed");
        Check(hauler!.StorageId!=depot.Id || hauler.Destination==depot.Entrance,"Carrier still targets old store entrance");Step(hauling,400);Roundtrip(hauling);
        var visiting=World.NewCreative(true);var venue=visiting.Place(visiting.Map.Land.First(c=>visiting.PlacementProblem(c,0,BuildingKind.GatheringHall)==null),0,BuildingKind.GatheringHall)!;
        for(int i=0;i<3000 && !visiting.People.Any(p=>p.LeisureSiteId==venue.Id);i++)Step(visiting);
        Check(visiting.People.Any(p=>p.LeisureSiteId==venue.Id),"No civic visit fixture");
        Check(visiting.MoveBuilding(venue.Id,Destination(visiting,venue,3),3),"Active civic move failed");
        Check(visiting.People.All(p=>p.LeisureSiteId!=venue.Id),"Moved venue retained old visit reservations");Step(visiting,600);Roundtrip(visiting);
        var river=World.NewCreative(true);foreach(var p in river.People)river.Assign(p.Id,Role.Unassigned);
        var crossing=river.Map.Water.SelectMany(c=>Enumerable.Range(0,4).Select(r=>(Cell:c,Rotation:r))).First(p=>river.PlacementProblem(p.Cell,p.Rotation,BuildingKind.Bridge)==null);
        var bridge=river.Place(crossing.Cell,crossing.Rotation,BuildingKind.Bridge)!;
        var alternate=river.Map.Water.SelectMany(c=>Enumerable.Range(0,4).Select(r=>(Cell:c,Rotation:r))).First(p=>p.Cell!=bridge.Cell && river.RelocationProblem(bridge.Id,p.Cell,p.Rotation)==null);
        river.People[0].Position=bridge.Cell.Point;river.Validate();string occupied=river.SaveJson();
        Check(river.RelocationProblem(bridge.Id,alternate.Cell,alternate.Rotation)?.Contains("disconnect")==true && !river.MoveBuilding(bridge.Id,alternate.Cell,alternate.Rotation) && river.SaveJson()==occupied,"Bridge move stranded its occupant");
        river.People[0].Position=river.YardAccess.Point;
        Check(river.MoveBuilding(bridge.Id,alternate.Cell,alternate.Rotation),"Empty replacement crossing failed");river.Validate();Roundtrip(river);
        // Use the authored lake, changing only its mode in this isolated fixture.
        var lakeJson=JsonNode.Parse(World.NewLakeMap().SaveJson())!;lakeJson["Creative"]=true;
        var lake=World.LoadJson(lakeJson.ToJsonString());Check(lake.Creative,"Lake fixture mode failed");
        var dock=lake.Place(new(3,4),1,BuildingKind.FishingDock)!;Check(dock!=null,"Dock fixture failed");lake.Assign(6,Role.Fisher);
        for(int i=0;i<2000 && dock!.Boat?.FisherId==null;i++)Step(lake);
        string active=lake.SaveJson();Check(lake.RelocationProblem(dock!.Id)!=null && !lake.MoveBuilding(dock.Id,dock.Cell,2) && lake.SaveJson()==active,"Active boat allowed relocation");
        lake.SetWorkplacePaused(dock.Id,true);for(int i=0;i<8000 && dock.Boat?.FisherId!=null;i++)Step(lake);
        Check(dock.Boat?.FisherId==null,"Boat did not return");
        var shore=lake.Map.Land.Concat(lake.Map.Water).SelectMany(c=>Enumerable.Range(0,4).Select(r=>(Cell:c,Rotation:r))).First(p=>p.Cell!=dock.Cell && lake.RelocationProblem(dock.Id,p.Cell,p.Rotation)==null);
        Check(lake.MoveBuilding(dock.Id,shore.Cell,shore.Rotation),"Moored dock refused move");lake.Validate();Check(dock.Boat!.Position==dock.Launch.Point,"Boat remained at old dock");Roundtrip(lake);
        Console.WriteLine("PASS: relocation core preserves identity, four-way state/goods, read-only failures, mature orchard/cargo, active/moored dock safety and exact continuation. Player preview remains pending.");
    }
}

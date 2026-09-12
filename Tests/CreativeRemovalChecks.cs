using Inlanders.Simulation;
using System.Text.Json.Nodes;

static class CreativeRemovalChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    static void Step(World w,int ticks){for(int i=0;i<ticks;i++){w.Tick(.1f);w.Validate();}}
    static CreativeRemovalSelection Selection(World w,params int[] ids)=>new(
        w.Cottages.Where(c=>ids.Contains(c.Id)).Select(c=>new BuildingRemovalTarget(c.Id,c.Cell,c.Rotation,c.Kind)).ToArray(),
        Array.Empty<Decoration>(),Array.Empty<Cell>());
    static World Commit(World w,CreativeRemovalSelection selection)
    {
        string before=w.SaveJson();var result=w.PrepareCreativeRemoval(selection);
        Check(w.SaveJson()==before,"Preparation mutated live world");Check(result.World!=null,result.Problem??"No result");
        var after=result.World!;after.Validate();string saved=after.SaveJson();var twin=World.LoadJson(saved);
        Check(twin.SaveJson()==saved,"Batch removal save changed");Step(after,200);Step(twin,200);Check(after.SaveJson()==twin.SaveJson(),"Removal continuation differs");
        return after;
    }
    public static void Run()
    {
        var normal=World.NewScenario();Check(normal.PrepareCreativeRemoval(normal.SelectCreativeRemoval(new(-100,-100),new(100,100))).World==null,"Normal economy allowed batch removal");
        var w=World.NewCreative(true);foreach(var p in w.People)w.Assign(p.Id,Role.Unassigned);
        foreach(var kind in new[]{BuildingKind.Cottage,BuildingKind.Orchard,BuildingKind.Pantry,BuildingKind.Stockpile,BuildingKind.Square})
        {
            var at=w.Map.Land.First(c=>w.PlacementProblem(c,0,kind)==null);w.Place(at,0,kind);
        }
        var orchard=w.Cottages.Single(c=>c.Kind==BuildingKind.Orchard);
        orchard.Planted=orchard.OrchardMature=true;orchard.Harvest=8;orchard.Growth=1;w.Food.GrownFruit=8;
        var pantry=w.Cottages.Single(c=>c.Kind==BuildingKind.Pantry);pantry.PantryFood[5]=4;w.Food.GrownFruit+=4;
        var stock=w.Cottages.Single(c=>c.Kind==BuildingKind.Stockpile);var timber=w.Trees.First(t=>t.Logs>=4);timber.Logs-=4;stock.StoredLogs=4;
        var ornament=w.Map.Land.First(c=>w.DecorationProblem(c,DecorationKind.Flowers)==null);Check(w.PlaceDecoration(ornament,DecorationKind.Flowers),"Decoration fixture failed");
        var path=w.Map.Land.First(c=>w.PathProblem(c)==null);Check(w.SetPath(path,true),"Path fixture failed");w.Validate();
        var selection=w.SelectCreativeRemoval(new(-100,-100),new(100,100));
        Check(selection.Buildings.Count==5 && selection.Decorations.Count==1 && selection.Paths.Count==1,"Area selection lost objects");
        var reverse=w.SelectCreativeRemoval(new(100,100),new(-100,-100));Check(reverse.Count==selection.Count,"Reversed area differs");
        Check(w.SelectCreativeRemoval(orchard.Cell,orchard.Cell).Buildings.Any(b=>b.Id==orchard.Id),"Partial footprint did not select whole building");
        var removed=Commit(w,selection);Check(removed.Cottages.Count==0 && removed.Decorations.Count==0 && removed.Paths.Count==0 && removed.Food.Fruit==12,"Mixed removal lost objects or fruit recovery");
        w.RemoveDecoration(ornament);string stale=w.SaveJson();Check(w.PrepareCreativeRemoval(selection).World==null && w.SaveJson()==stale,"Stale selection partially applied");
        var live=World.NewCreative(true);var atLive=live.Map.Land.First(c=>live.PlacementProblem(c,0,BuildingKind.Orchard)==null);var liveOrchard=live.Place(atLive,0,BuildingKind.Orchard)!;live.Assign(6,Role.Farmer);
        for(int i=0;i<8000 && !live.People.Any(p=>p.Cargo==Resource.Fruit && p.Carried>0);i++)Step(live,1);
        Check(live.People.Any(p=>p.Cargo==Resource.Fruit && p.Carried>0),"No actual orchard cargo");Commit(live,Selection(live,liveOrchard.Id));
        var lakeJson=JsonNode.Parse(World.NewLakeMap().SaveJson())!;lakeJson["Creative"]=true;var lake=World.LoadJson(lakeJson.ToJsonString());
        var dock=lake.Place(new(3,4),1,BuildingKind.FishingDock)!;lake.Assign(6,Role.Fisher);
        var extra=lake.Place(lake.Map.Land.First(c=>lake.PlacementProblem(c,0,BuildingKind.Cottage)==null),0,BuildingKind.Cottage)!;
        for(int i=0;i<2000 && dock.Boat?.FisherId==null;i++)Step(lake,1);
        Check(dock.Boat?.FisherId!=null,"No active boat");var both=Selection(lake,extra.Id,dock.Id);string active=lake.SaveJson();
        Check(lake.PrepareCreativeRemoval(both).World==null && lake.SaveJson()==active,"Rejected dock batch partly removed another building");
        lake.SetWorkplacePaused(dock.Id,true);for(int i=0;i<8000 && dock.Boat?.FisherId!=null;i++)Step(lake,1);
        Check(dock.Boat?.FisherId==null,"Dock did not return");Commit(lake,both);
        var riverJson=JsonNode.Parse(World.NewCampaign(6).SaveJson())!;riverJson["Creative"]=true;riverJson["Campaign"]=null;
        var river=World.LoadJson(riverJson.ToJsonString());foreach(var p in river.People)river.Assign(p.Id,Role.Unassigned);
        for(int i=0;i<2;i++)
        {
            var crossing=river.Map.Water.SelectMany(c=>Enumerable.Range(0,4).Select(r=>(Cell:c,Rotation:r))).First(p=>river.PlacementProblem(p.Cell,p.Rotation,BuildingKind.Bridge)==null);
            river.Place(crossing.Cell,crossing.Rotation,BuildingKind.Bridge);
        }
        var bridges=river.Cottages.Where(c=>c.Kind==BuildingKind.Bridge).ToArray();
        Check(bridges.Length==2 && bridges.All(b=>river.RemovalProblem(b.Id)==null),"Alternative crossing fixture not individually removable");
        string connected=river.SaveJson();var crossingBatch=river.PrepareCreativeRemoval(Selection(river,bridges.Select(b=>b.Id).ToArray()));
        Check(crossingBatch.World==null && river.SaveJson()==connected,"Joint bridge removal disconnected village or partly applied");
        Console.WriteLine("PASS: Creative removal transaction, mixed objects, source isolation, stale refusal, goods recovery, live fruit cargo, active/moored docks and exact continuation.");
    }
}

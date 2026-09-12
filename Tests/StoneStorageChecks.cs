using Inlanders.Simulation;
using System.Text.Json.Nodes;

static class StoneStorageChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    static void Step(World w,int ticks=1){for(int i=0;i<ticks;i++){w.Tick(.1f);w.Validate();}}
    static void Until(World w,Func<bool> done,string why){for(int i=0;i<12000 && !done();i++)Step(w);Check(done(),why+" · "+string.Join("; ",w.People.Select(p=>p.Status)));}
    static void Roundtrip(World w)
    {
        string save=w.SaveJson();var a=World.LoadJson(save);var b=World.LoadJson(save);
        Check(a.SaveJson()==save,"Stone save changed on load");Step(a,100);Step(b,100);Check(a.SaveJson()==b.SaveJson(),"Stone continuation diverged");
    }
    public static void Run()
    {
        // Ordinary construction and finite extraction: no injected stone or free pile.
        var w=StorageChecks.Ready();var pile=w.Cottages.Single();
        Check(w.SetStorageMaterial(pile.Id,Resource.Stone),"Cannot select stone");
        w.Map.StoneDeposits.Add(new(){Id=0,Cell=new(5,-4),Capacity=24,Remaining=24});
        var campCell=w.Map.Land.Where(c=>w.PlacementProblem(c,0,BuildingKind.Quarry)==null).OrderBy(c=>(c.Point-new Cell(5,-1).Point).LengthSquared()).First();
        var camp=w.Place(campCell,0,BuildingKind.Quarry)!;
        w.Assign(2,Role.Builder);Until(w,()=>camp.Complete,"Quarry not built");w.Assign(2,Role.Unassigned);w.Assign(0,Role.Quarrier);
        Until(w,()=>w.People[0].Task==Work.ToStockpile && w.People[0].StorageId==pile.Id,"Quarrier skipped nearer stone pile");
        Check(!w.SetStorageMaterial(pile.Id,Resource.Logs),"Incoming stone allowed material change");Roundtrip(w);
        Until(w,()=>pile.StoredStone==12,"Pile never filled");
        Until(w,()=>w.YardStone>=2,"Full pile did not fall back to central storage");
        w.Assign(0,Role.Unassigned);Until(w,()=>w.People.All(p=>p.Carried==0),"Extraction interruption stranded cargo");
        int total=w.Stone;Check(total==w.QuarriedStone && w.YardStone+pile.StoredStone==total,"Stone totals wrong");
        var hall=w.Place(new(6,3),0,BuildingKind.GatheringHall)??throw new Exception("Hall fixture placement");
        w.Assign(2,Role.Builder);
        Until(w,()=>w.People[2].Task==Work.ToMaterials && w.People[2].Cargo==Resource.Stone,"No builder stone claim");
        Check(w.People[2].StorageId==pile.Id && w.ReservedStone==2 && w.ReadEconomy().Stocks.Single(s=>s.Resource==Resource.Stone).Reserved==2,"Local builder reservation absent from economy");Roundtrip(w);
        var cancelled=World.LoadJson(w.SaveJson());Check(cancelled.Cancel(hall.Id),"Hall cancel rejected");
        Until(cancelled,()=>cancelled.People.All(p=>p.Cargo!=Resource.Stone || p.Carried==0),"Cancelled stone claim stranded");cancelled.Validate();
        Until(w,()=>hall.DeliveredStone==12,"Local stone never reached hall");
        w.Assign(2,Role.Unassigned);Check(w.Cancel(hall.Id),"Partial hall cancel rejected");
        // Demolition recovers local stock and reroutes any existing claims.
        Check(w.RequestDemolition(pile.Id),"Stone pile demolition rejected");w.Assign(2,Role.Builder);
        Until(w,()=>w.Cottages.All(c=>c.Id!=pile.Id),"Stone pile demolition stalled");
        Until(w,()=>w.People.All(p=>p.Carried==0),"Demolition cargo stranded");Roundtrip(w);

        foreach(int rotation in Enumerable.Range(0,4)) CreativeTrips(rotation);
        Console.WriteLine("PASS: normal quarry deposits, capacity fallback, local builder reservations/delivery, save continuation, cancellation, demolition, four-way hauling, interruption, relocation and Creative removal.");
    }
    static void CreativeTrips(int rotation)
    {
        var w=World.NewCreative();foreach(var p in w.People)w.Assign(p.Id,Role.Unassigned);
        var pile=w.Place(new(3,0),rotation,BuildingKind.Stockpile)!;
        Check(w.SetStorageMaterial(pile.Id,Resource.Stone) && w.SetStorageTarget(pile.Id,12) && w.SetCreativeCentralStock(Resource.Stone,20),"Creative setup failed");
        w.Assign(0,Role.Hauler);w.Assign(1,Role.Hauler);
        foreach(var phase in new[]{Work.ToHaulPickup,Work.ToHaulDrop})
        {
            Until(w,()=>w.People.Any(p=>p.Task==phase),"Missing stone hauling phase");Roundtrip(w);
            var interrupted=World.LoadJson(w.SaveJson());foreach(var p in interrupted.People)interrupted.Assign(p.Id,Role.Unassigned);
            Until(interrupted,()=>interrupted.People.All(p=>p.Carried==0),"Hauler interruption stranded stone");Check(interrupted.Stone==20,"Interrupted hauling lost stone");
            var removed=World.LoadJson(w.SaveJson());Check(removed.RemoveBuilding(pile.Id),"Active stone pile removal rejected");Until(removed,()=>removed.People.All(p=>p.Carried==0),"Removed pile left cargo");Check(removed.Stone==20 && removed.YardStone==20,"Removal lost stone");
        }
        Until(w,()=>pile.StoredStone==12 && w.People.All(p=>p.Task is not (Work.ToHaulPickup or Work.ToHaulDrop)),"Stone target not filled");
        Check(w.YardStone==8 && !w.SetStorageMaterial(pile.Id,Resource.Planks),"Occupied pile switch or aggregate failed");
        var relocated=World.LoadJson(w.SaveJson());Check(relocated.MoveBuilding(pile.Id,new(6,0),rotation),"Stone relocation rejected");
        Check(relocated.Cottages.Single(c=>c.Id==pile.Id).StoredStone==12 && relocated.Stone==20,"Relocation lost stone");Roundtrip(relocated);
        var selection=w.SelectCreativeRemoval(pile.Cell,pile.Cell);var cleared=w.PrepareCreativeRemoval(selection);
        Check(cleared.World!=null && cleared.World.Stone==20 && cleared.World.YardStone==20,"Area removal lost stone");
        var bad=JsonNode.Parse(w.SaveJson())!;bad["Buildings"]![0]!["StorageMaterial"]=(int)Resource.Logs;
        bool rejected=false;try{World.LoadJson(bad.ToJsonString());}catch(InvalidOperationException){rejected=true;}Check(rejected,"Wrong-material stone inventory loaded");
        w.SetStorageTarget(pile.Id,0);Until(w,()=>w.StorageMaterialProblem(pile.Id)==null && w.People.All(p=>p.Carried==0),"Stone target zero failed to drain");
        Check(w.YardStone==20 && w.SetStorageMaterial(pile.Id,Resource.Planks),"Drained stone pile cannot switch");
    }
}

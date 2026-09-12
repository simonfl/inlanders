using Inlanders.Simulation;
using System.Text.Json.Nodes;

static class WorkplaceAssignmentChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    static void Step(World w,int count=1){for(int i=0;i<count;i++){w.Tick(.1f);w.Validate();}}
    static void Until(World w,Func<bool> done,string why){for(int i=0;i<16000 && !done();i++)Step(w);Check(done(),why+" · "+string.Join("; ",w.People.Select(p=>p.Status)));}
    static Cottage Place(World w,BuildingKind kind,Cell near)
    {
        var cell=w.Map.Land.Where(c=>w.PlacementProblem(c,0,kind)==null).OrderBy(c=>(c.Point-near.Point).LengthSquared()).ThenBy(c=>c.X).ThenBy(c=>c.Z).First();
        return w.Place(cell,0,kind)!;
    }
    static void Roundtrip(World w)
    {
        string saved=w.SaveJson();var loaded=World.LoadJson(saved);Check(loaded.SaveJson()==saved,"Assignment save changed");
        for(int i=0;i<100;i++){Step(w);Step(loaded);Check(w.SaveJson()==loaded.SaveJson(),$"Original assignment continuation diverged at tick {i+1}");}
    }
    public static void Run()
    {
        foreach(var kind in new[]{BuildingKind.ForagerHut,BuildingKind.Farm,BuildingKind.VegetableGarden,BuildingKind.Orchard,BuildingKind.Bakery,BuildingKind.Sawmill,BuildingKind.Quarry,BuildingKind.HuntingLodge}) BoundRole(kind);
        BoundCarpenter();BoundFishing();CapacityAndChanges();GrowingAndRemoval();OrchardGuidance();
        Console.WriteLine("PASS: all supported workplace kinds, reserved slots, strict waiting, next-job changes, boat return, role/removal cleanup, growing fields, automatic fallback by choice, exact saves and invalid assignments.");
    }
    static void BoundRole(BuildingKind kind)
    {
        var w=World.NewCreative(true);foreach(var resident in w.People)w.Assign(resident.Id,Role.Unassigned);
        var first=Place(w,kind,new(0,0));var second=Place(w,kind,new(6,0));
        w.SetCreativeCentralStock(Resource.Grain,20);w.SetCreativeCentralStock(Resource.Logs,30);
        var role=Buildings.Get(kind).Worker!.Value;w.Assign(0,role);
        Check(w.SetWorkplaceAssignment(0,second.Id),"Cannot bind "+kind);
        Until(w,()=>w.People[0].WorkplaceId!=null,"No bound work: "+kind);
        Check(w.People[0].WorkplaceId==second.Id,"Assigned worker chose first workplace: "+kind);Roundtrip(w);
        Check(w.SetWorkplacePaused(second.Id,true),"Pause failed");
        Until(w,()=>w.People[0].WorkplaceId==null && w.People[0].Carried==0,"Current job failed to finish: "+kind);
        Step(w,200);Check(w.People[0].AssignedWorkplaceId==second.Id && w.People[0].WorkplaceId==null && w.ReadWorkplaceAssignment(w.People[0]).Contains("Paused"),"Paused worker silently worked elsewhere: "+kind);
        Check(w.SetWorkplaceAssignment(0,null),"Cannot choose automatic");
        if(kind!=BuildingKind.Carpenter)Until(w,()=>w.People[0].WorkplaceId==first.Id,"Automatic did not find other site: "+kind);
        w.Assign(0,Role.Builder);Check(w.People[0].AssignedWorkplaceId==null,"Role change kept binding");
        Check(!w.SetWorkplaceAssignment(0,first.Id),"Roaming builder bound");w.Validate();
    }
    static void BoundCarpenter()
    {
        var w=new World(60);w.Food.InitialBerries=w.Food.Berries=512;
        foreach(var resident in w.People)w.Assign(resident.Id,Role.Unassigned);
        var home=Place(w,BuildingKind.Cottage,new(0,0));var first=Place(w,BuildingKind.Carpenter,new(3,0));var second=Place(w,BuildingKind.Carpenter,new(6,0));Place(w,BuildingKind.Sawmill,new(3,-3));
        w.Assign(1,Role.Builder);w.Assign(2,Role.Builder);w.Assign(3,Role.Sawyer);w.Assign(4,Role.Logger);
        Until(w,()=>w.Cottages.All(c=>c.Complete) && w.AvailablePlanks>=4,"Normal carpenter setup failed");
        Check(w.RequestImprovement(home.Id),"Improvement order failed");w.Assign(0,Role.Carpenter);Check(w.SetWorkplaceAssignment(0,second.Id),"Carpenter binding failed");
        Until(w,()=>w.People[0].WorkplaceId!=null,"No bound carpenter job");Check(w.People[0].WorkplaceId==second.Id,"Carpenter ignored binding");Roundtrip(w);
        w.SetWorkplacePaused(second.Id,true);Until(w,()=>w.People[0].WorkplaceId==null,"Carpenter failed to finish current delivery");Step(w,200);
        Check(w.People[0].WorkplaceId==null && w.People[0].AssignedWorkplaceId==second.Id,"Paused carpenter moved to first shop");
        w.SetWorkplacePaused(second.Id,false);Until(w,()=>home.Improved,"Bound carpenter did not finish improvement");
    }
    static void CapacityAndChanges()
    {
        var w=World.NewCreative();foreach(var resident in w.People)w.Assign(resident.Id,Role.Unassigned);
        var first=Place(w,BuildingKind.Sawmill,new(3,0));var second=Place(w,BuildingKind.Sawmill,new(6,0));w.SetCreativeCentralStock(Resource.Logs,40);
        w.Assign(0,Role.Sawyer);w.Assign(1,Role.Sawyer);w.Assign(2,Role.Sawyer);
        Until(w,()=>w.People[0].WorkplaceId==first.Id && w.People[1].WorkplaceId==second.Id,"Automatic mill jobs missing");
        var p=w.People[0];var phase=p.Task;var reserved=p.Reserved;var cargo=p.Carried;var route=p.Route.ToArray();
        Check(w.SetWorkplaceAssignment(0,second.Id),"Cannot assign occupied mill");
        Check(p.Task==phase && p.WorkplaceId==first.Id && p.Reserved==reserved && p.Carried==cargo && p.Route.SequenceEqual(route),"Assignment interrupted current job");
        string before=w.SaveJson();Check(!w.SetWorkplaceAssignment(2,second.Id) && before==w.SaveJson(),"Full assignment accepted or mutated world");
        Until(w,()=>p.WorkplaceId==second.Id,"Next job did not use assignment");
        for(int i=0;i<1000;i++){Step(w);Check(w.People[1].WorkplaceId!=second.Id && w.People[2].WorkplaceId!=second.Id,"Automatic worker stole reserved slot");}
        Roundtrip(w);
        var bad=JsonNode.Parse(w.SaveJson())!;bad["People"]![2]!["AssignedWorkplaceId"]=second.Id;
        bool rejected=false;try{World.LoadJson(bad.ToJsonString());}catch(InvalidOperationException){rejected=true;}Check(rejected,"Overbooked save accepted");
        Check(w.MoveBuilding(second.Id,new(6,3),2) && p.AssignedWorkplaceId==second.Id,"Relocation lost persistent assignment");
        Check(w.RemoveBuilding(second.Id) && p.AssignedWorkplaceId==null,"Removal kept binding");Roundtrip(w);
    }
    static void GrowingAndRemoval()
    {
        var w=World.NewCreative();foreach(var resident in w.People)w.Assign(resident.Id,Role.Unassigned);
        var garden=Place(w,BuildingKind.VegetableGarden,new(3,0));var other=Place(w,BuildingKind.Farm,new(6,0));w.Assign(0,Role.Farmer);w.SetWorkplaceAssignment(0,garden.Id);
        Until(w,()=>garden.Planted && w.People[0].WorkplaceId==null,"Bound crop not sown");Step(w,100);
        Check(!other.Planted && w.People[0].AssignedWorkplaceId==garden.Id,"Growing crop triggered silent fallback");
        Check(w.SetWorkplaceAssignment(0,null),"Could not release farmer");Until(w,()=>other.Planted,"Automatic farmer did not use another field");
        // Normal demolition clears assignments immediately; cancellation does not silently reassign workers.
        var normal=StorageChecks.Ready();foreach(var p in normal.People)normal.Assign(p.Id,Role.Unassigned);
        var mill=Place(normal,BuildingKind.Sawmill,new(6,0));normal.Assign(0,Role.Builder);Until(normal,()=>mill.Complete,"Normal mill incomplete");normal.Assign(0,Role.Sawyer);normal.SetWorkplaceAssignment(0,mill.Id);
        Check(normal.RequestDemolition(mill.Id) && normal.People[0].AssignedWorkplaceId==null,"Demolition kept assignment");
        Check(normal.CancelDemolition(mill.Id) && normal.People[0].AssignedWorkplaceId==null,"Demolition cancellation restored old assignment");Roundtrip(normal);
    }
    static void OrchardGuidance()
    {
        Directory.CreateDirectory("artifacts/workplace-assignment");
        foreach(bool mature in new[]{false,true})
        {
            var w=World.NewCreative(true);foreach(var resident in w.People)w.Assign(resident.Id,Role.Unassigned);
            var orchard=Place(w,BuildingKind.Orchard,new(0,0));w.Assign(0,Role.Farmer);w.SetWorkplaceAssignment(0,orchard.Id);
            Until(w,()=>orchard.Planted && orchard.Harvest==0 && orchard.OrchardMature==mature && w.People[0].WorkplaceId==null,"Orchard growth stage not reached");
            var field=Place(w,BuildingKind.Farm,new(6,0));Step(w,100);
            Check(!field.Planted && w.People[0].WorkplaceId==null && w.People[0].AssignedWorkplaceId==orchard.Id,"Bound orchard farmer worked elsewhere during growth");
            var report=w.ReadWorkplace(orchard);
            Check(report.State==(mature?"Fruit growing":"Trees establishing") && report.Detail.Contains("Automatic farmers can work elsewhere") && report.Detail.Contains("Assigned farmers wait") && report.Detail.Contains("People"),"Orchard guidance hides assignment rule or recovery");
            w.SaveFile($"artifacts/workplace-assignment/orchard-{(mature?"growing":"establishing")}.json");Roundtrip(w);
            Check(w.SetWorkplaceAssignment(0,null),"Cannot release orchard farmer");
            Until(w,()=>field.Planted,"Automatic orchard farmer did not sow available field");
            Check(orchard.Planted && orchard.Harvest==0,"Other field was not sown during orchard growth");Roundtrip(w);
        }
    }
    static void BoundFishing()
    {
        var w=World.NewLakeMap();var first=w.Place(new(3,4),true,BuildingKind.FishingDock)!;var second=w.Place(new(14,0),true,BuildingKind.FishingDock)!;
        Until(w,()=>first.Complete && second.Complete,"Two docks not built");foreach(var resident in w.People)w.Assign(resident.Id,Role.Unassigned);
        w.Assign(0,Role.Fisher);Check(w.SetWorkplaceAssignment(0,second.Id),"Cannot assign dock");
        Until(w,()=>w.People[0].Task==Work.Aboard,"Assigned fisher never sailed");Check(w.People[0].WorkplaceId==second.Id,"Fisher chose nearer dock");
        var boat=second.Boat!;var phase=boat.Phase;var at=boat.Position;
        Check(w.SetWorkplaceAssignment(0,first.Id) && w.People[0].WorkplaceId==second.Id && boat.Phase==phase && boat.Position==at,"Assignment teleported boat");Roundtrip(w);
        Until(w,()=>w.People[0].WorkplaceId==first.Id,"Fisher did not finish return/delivery before next dock");Check(second.Boat!.FisherId==null,"Old boat retained fisher");
    }
}

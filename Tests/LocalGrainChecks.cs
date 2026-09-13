using Inlanders.Simulation;
using System.Text.Json.Nodes;

static class LocalGrainChecks
{
    static void Check(bool value,string why){if(!value)throw new Exception(why);}
    static void Until(World world,Func<bool> done,string why,int ticks=18000)
    {
        for(int i=0;i<ticks && !done();i++){world.Tick(.1f);if(i%20==0)world.Validate();}
        Check(done(),$"{why}: {world.Food.Time:F1}s, grain {world.StoredGrain}, bread {world.Food.BakedBread}");world.Validate();
    }
    static Cottage Place(World w,Cell cell,BuildingKind kind,int rotation=0)=>w.Place(cell,rotation,kind)??throw new Exception($"Place {kind}: {w.PlacementProblem(cell,rotation,kind)}");
    static (World world,int farm,int bakery) Prepare(bool local)
    {
        var w=local?World.NewLocalSupplyExperiment():World.NewCampaign(6);
        var bridge=Place(w,new(5,2),BuildingKind.Bridge,1);Until(w,()=>bridge.Complete,"crossing");
        var farm=Place(w,new(8,7),BuildingKind.Farm);var bakery=Place(w,new(11,0),BuildingKind.Bakery);
        Until(w,()=>farm.Complete && bakery.Complete,"food workplaces");w.Assign(6,Role.Farmer);w.Assign(7,Role.Baker);
        return(w,farm.Id,bakery.Id);
    }
    static void Continuation(World w)
    {
        string saved=w.SaveJson();var copy=World.LoadJson(saved);Check(copy.SaveJson()==saved,"Snapshot differs");
        for(int i=0;i<100;i++){w.Tick(.1f);copy.Tick(.1f);Check(w.SaveJson()==copy.SaveJson(),"Original/reload continuation differs");}
    }
    public static void Run()
    {
        Directory.CreateDirectory("artifacts/local-grain");
        var (w,farmId,bakeryId)=Prepare(true);
        Until(w,()=>w.People.Any(p=>p.GrainDestinationId==farmId),"farmer carries to own store");
        string delivery=w.SaveJson();Continuation(w);
        Until(w,()=>w.People.Any(p=>p.GrainSourceId==farmId && p.Task==Work.ToGrain),"baker reserves farm grain");
        Check(w.Food.Grain==0,"Local chain secretly requires central grain");
        var report=w.ReadWorkplace(w.Cottages.Single(c=>c.Id==bakeryId));
        Check(report.SourceBuilding==farmId && report.Source==w.GrainAccess(farmId),"Bakery source feedback points elsewhere");
        var grainStock=w.ReadEconomy().Stocks.Single(s=>s.Resource==Resource.Grain);
        Check(grainStock.Stored==w.StoredGrain && grainStock.Reserved==w.People.Sum(p=>p.FoodReserved),"Economy omits local stock/claims");
        Check(w.ReadSupplyRoutes().Single(r=>r.WorkerId==7).Destination==$"Farm {farmId} grain store","Route calls local pickup central");
        string pickup=w.SaveJson();File.WriteAllText("artifacts/local-grain/pickup.json",pickup);Continuation(w);
        Until(w,()=>w.Food.BakedBread>=4,"local baking");
        Check(w.Food.UsedGrain>=2,"No grain consumed");

        var interrupted=World.LoadJson(pickup);interrupted.Assign(7,Role.Unassigned);interrupted.Validate();
        Check(interrupted.GrainReservedAt(farmId)==0 && interrupted.People[7].GrainSourceId==null,"Assignment leaves grain reserved");Continuation(interrupted);
        var returned=World.LoadJson(delivery);returned.Assign(6,Role.Unassigned);returned.Validate();
        Check(returned.People[6].GrainDestinationId==null && returned.People[6].Carried>0,"Farmer interruption lost cargo");
        Until(returned,()=>returned.People[6].Carried==0,"interrupted grain returned");Continuation(returned);

        var demolish=World.LoadJson(pickup);Check(demolish.RequestDemolition(farmId),"Farm demolition rejected");demolish.Validate();
        Check(demolish.GrainReservedAt(farmId)==0 && demolish.People.All(p=>p.GrainSourceId!=farmId && p.GrainDestinationId!=farmId),"Demolition retained grain claim");
        Check(demolish.CancelDemolition(farmId),"Demolition cancellation failed");Continuation(demolish);
        Check(demolish.RequestDemolition(farmId),"Second demolition rejected");
        Until(demolish,()=>demolish.Cottages.All(c=>c.Id!=farmId),"stored grain physical demolition recovery");Continuation(demolish);

        var capped=World.LoadJson(delivery);capped.Assign(7,Role.Unassigned);
        Until(capped,()=>capped.Food.Grain>0,"full farm store overflow reaches central pantry");
        Check(capped.Cottages.Single(c=>c.Id==farmId).StoredGrain<=World.FarmGrainCapacity,"Store over capacity");Continuation(capped);

        var competing=Prepare(true);var two=competing.world;
        Until(two,()=>two.PlacementProblem(new(8,0),0,BuildingKind.Bakery)==null,"second oven site");
        var second=Place(two,new(8,0),BuildingKind.Bakery);Until(two,()=>second.Complete,"second oven construction");two.Assign(5,Role.Baker);
        Until(two,()=>two.People.Count(p=>p.GrainSourceId==competing.farm && p.Task==Work.ToGrain)==2,"two bakers share a farm reservation");
        Check(two.GrainReservedAt(competing.farm)==4 && two.GrainAvailableAt(competing.farm)>=0,"Two bakers overreserved farm grain");Continuation(two);

        var invalid=JsonNode.Parse(pickup)!;invalid["Buildings"]!.AsArray().Single(b=>b!["Id"]!.GetValue<int>()==farmId)!["StoredGrain"]=0;
        bool rejected=false;try{World.LoadJson(invalid.ToJsonString());}catch{rejected=true;}Check(rejected,"Overreserved/corrupt grain accepted");
        var baseline=Prepare(false).world;Until(baseline,()=>baseline.Food.BakedBread>=4,"baseline central baking");
        Check(baseline.Cottages.All(c=>c.StoredGrain==0) && baseline.People.All(p=>p.GrainSourceId==null && p.GrainDestinationId==null),"Baseline gained local grain");
        File.WriteAllText("artifacts/local-grain/baked.json",w.SaveJson());
        Console.WriteLine("PASS: physical farm grain, bakery pickup without central stock, competing bakers, accurate source, interruption, overflow, demolition/cancel, corrupt-save rejection and original/reload continuation; baseline central route retained.");
    }
}

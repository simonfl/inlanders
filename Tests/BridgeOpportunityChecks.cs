using Inlanders.Simulation;
static class BridgeOpportunityChecks
{
    public static void Run()
    {
        var w=World.NewRestorationChallenge(true);string before=w.SaveJson();
        var near=w.ReadBridgeOpportunity(new(5,0),0)!;
        if(near.GroundTiles<=0 || near.Workplaces!=0 || w.SaveJson()!=before)throw new Exception("Island preview wrong or impure");
        w.Place(new(5,0),0,BuildingKind.Bridge);before=w.SaveJson();var far=w.ReadBridgeOpportunity(new(0,-7),0)!;
        if(far.Workplaces!=2 || far.RipeFood!=40 || w.SaveJson()!=before)throw new Exception("Salvage preview wrong or impure");
        w.Place(new(0,-7),0,BuildingKind.Bridge);
        if(w.Cottages.Where(c=>c.Kind==BuildingKind.VegetableField).Any(c=>w.ReadWorkplace(c).MissingAccess))throw new Exception("Actual crossing disagrees with preview");
        var water=World.NewLargeMap(true,false);water.Map.FishingGrounds.Add(new(){Id=0,Name="Upstream",Cell=new(7,-7),Capacity=16,Stock=16});
        var choice=ReviewPlacement.Find(water,BuildingKind.FishingDock,new(6,4),(c,r)=>water.DockLaunch(c,r).Z>1,"boat route")!;
        var dock=water.Place(choice.Actual,choice.Rotation,BuildingKind.FishingDock)!;
        before=water.SaveJson();var loss=water.ReadBridgeOpportunity(new(7,0),1)??throw new Exception("Crossing unavailable");
        if(loss.LostFishingConnections!=1 || water.SaveJson()!=before)throw new Exception("Boat-route cost absent or impure");
        var crossing=water.Place(new(7,0),1,BuildingKind.Bridge)!;
        if(water.FindBoatRoute(dock.Launch,new(7,-7))!=null || !water.Cancel(crossing.Id) || water.FindBoatRoute(dock.Launch,new(7,-7))==null)throw new Exception("Real boat route/cancel disagrees with preview");
        water.Validate();w.Validate();Console.WriteLine("PASS pure crossing benefits, actual restored fields, fishing-route loss and cancellation recovery");
    }
}

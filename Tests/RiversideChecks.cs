using Inlanders.Simulation;
using System.Text.Json;

static class RiversideChecks
{
    public static void Run()
    {
        var rows=new List<object>();
        foreach(bool farGarden in new[]{false,true})foreach(bool farHomes in new[]{false,true})foreach(bool shortcut in new[]{false,true})
        {
            var w=World.NewRiverside(farGarden);var report=w.ReadRiverside();
            if(report.WalkNow<=report.WalkWithShortcut)throw new Exception("Shortcut does not shorten ordinary route");
            void Plan(BuildingKind kind,Cell cell,int rotation=0){if(!w.PlanRiverside(cell,rotation,kind))throw new Exception($"Plan {kind} {cell}: {w.RiversideSiteProblem(cell,rotation,kind)}");}
            Plan(BuildingKind.Cottage,farHomes?new(1,-3):new(2,6));
            Plan(BuildingKind.Cottage,farHomes?new(5,-3):new(5,6));
            Plan(BuildingKind.VegetableField,farHomes?new(3,-5):new(-2,4),farHomes?0:2);
            if(shortcut)Plan(BuildingKind.Bridge,World.RiversideShortcut);
            bool saved=false;float hungry=0,homesAt=-1;int initialGrove=w.ManagedWoodland.Count;
            while(w.Food.Time<1200 && !w.Riverside!.Complete)
            {
                w.Tick(.1f);
                if(w.People.Any(p=>!p.Fed))hungry+=.1f;
                if(homesAt<0 && w.Beds>=8)homesAt=w.Food.Time;
                if(w.RiversideWelcomeProblem()==null)w.WelcomeRiverside();
                if(!saved && w.Food.Time>45)
                {
                    var copy=World.LoadJson(w.SaveJson());
                    for(int i=0;i<40;i++){w.Tick(.1f);copy.Tick(.1f);}
                    if(w.SaveJson()!=copy.SaveJson())throw new Exception("Riverside continuation diverged");saved=true;
                }
            }
            w.Validate();
            var row=new{farGarden,farHomes,shortcut,complete=w.Riverside!.Complete,time=w.Food.Time,homesAt,hungry,food=w.ReachableEdibleStored,logs=w.Available,initialGrove,grove=w.ManagedWoodland.Count,walkBefore=report.WalkNow,walkAfter=w.ReadRiverside().WalkNow};rows.Add(row);Console.WriteLine(JsonSerializer.Serialize(row));
            if(!w.Riverside.Complete)throw new Exception("Riverside branch failed: "+JsonSerializer.Serialize(row));
        }
        var cancel=World.NewRiverside();
        if(!cancel.PlanRiverside(new(2,6),0,BuildingKind.Cottage))throw new Exception("Clearing setup failed");
        var pending=cancel.Riverside!.Plans.Single();
        if(cancel.PlanRiverside(pending.Cell,0,pending.Kind))throw new Exception("Duplicate clearing plan accepted");
        if(!cancel.CancelRiversidePlan(pending.Id) || cancel.Riverside.Plans.Count!=0 || cancel.Trees.Any(t=>t.ClearRequested) || !cancel.ManagedWoodland.Contains(new(2,5)))throw new Exception("Clearing cancellation failed");
        cancel.Validate();
        var fishing=World.NewRiverside();
        if(!fishing.PlanRiverside(new(7,4),1,BuildingKind.FishingDock))throw new Exception("Riverside shore fishing rejected");
        var bridge=fishing.ReadBridgeOpportunity(World.RiversideShortcut,0);
        if(bridge==null || bridge.LostFishingConnections!=0)throw new Exception("Shortcut severs fishing grounds");
        var recovery=World.NewRiverside(true);
        while(recovery.Food.Time<300)recovery.Tick(.1f);
        foreach(var (at,kind) in new[]{(new Cell(1,-3),BuildingKind.Cottage),(new Cell(5,-3),BuildingKind.Cottage),(new Cell(3,-5),BuildingKind.VegetableField),(World.RiversideShortcut,BuildingKind.Bridge)})
        {
            for(int attempt=0;!recovery.PlanRiverside(at,0,kind);attempt++)
            {if(attempt>=20)throw new Exception("Recovery order blocked");for(int i=0;i<30;i++)recovery.Tick(.1f);}
        }
        while(recovery.Food.Time<1500 && !recovery.Riverside!.Complete)recovery.Tick(.1f);
        if(!recovery.Riverside!.Complete)throw new Exception("Neglected village cannot recover");
        Console.WriteLine($"PASS late recovery at {recovery.Food.Time:0.0}s; shortcut preserves river fishing");
        Directory.CreateDirectory("artifacts/browser/riverside263");File.WriteAllText("artifacts/browser/riverside263/comparisons.json",JsonSerializer.Serialize(rows,new JsonSerializerOptions{WriteIndented=true}));
        Console.WriteLine("PASS both banks, both garden starts, optional shortcut, queued clearing, cancellation, conservation and exact saves");
    }
}

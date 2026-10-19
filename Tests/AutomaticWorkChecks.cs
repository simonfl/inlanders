using Inlanders.Simulation;
static class AutomaticWorkChecks
{
    public static void Run()
    {
        var w=World.NewRiverFrontage(false,true,true);var dock=w.Cottages.Single(c=>c.Kind==BuildingKind.FishingDock);
        if(!w.ReadWorkplace(dock).RestingForFood)throw new Exception("Stocked initial dock should rest");
        var copy=World.LoadJson(w.SaveJson());w.TraceSharedWork=true;
        for(int i=0;i<100;i++){w.Tick(.1f);copy.Tick(.1f);}
        if(w.SaveJson()!=copy.SaveJson())throw new Exception("Tracing changed dispatch");
        if(!w.WorkDecisions.Values.Any(a=>a.Contains("Food: reserve sufficient")))throw new Exception("Reserve gate not captured");
        w.Assign(0,Role.Fisher);
        if(w.ReadWorkplace(dock).RestingForFood)throw new Exception("Fixed fisher falsely reported reserve-resting");
        for(int i=0;i<500 && dock.Boat?.FisherId==null;i++)w.Tick(.1f);
        if(dock.Boat?.FisherId==null)throw new Exception("Fixed fisher could not claim dock");
        w.UseAutomaticVillageWork();
        if(w.People.Any(p=>!p.SharedWorker))throw new Exception("Automatic pool not restored");
        w.Validate();Console.WriteLine("PASS stocked dock rests; trace explains reserve gate without changing simulation; fixed fisher status/dispatch agree; automatic restore retains trip.");
    }
}

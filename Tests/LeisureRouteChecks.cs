using Inlanders.Simulation;
static class LeisureRouteChecks
{
    public static void Run()
    {
        var w=World.NewCreative(true);
        var across=w.Place(new(9,0),0,BuildingKind.SeatingGarden)??throw new Exception("Across: "+w.PlacementProblem(new(9,0),0,BuildingKind.SeatingGarden));
        var local=w.Place(new(1,-7),0,BuildingKind.SeatingGarden)??throw new Exception("Local: "+w.PlacementProblem(new(1,-7),0,BuildingKind.SeatingGarden));
        foreach(var p in w.People){w.Assign(p.Id,Role.Unassigned);p.NextRestTime=10000;p.NextLeisureTime=10000;}
        var visitor=w.People[0];visitor.Position=new Cell(5,0).Point;visitor.NextLeisureTime=0;w.Food.Time=30;
        w.Tick(.1f);
        if(visitor.LeisureSiteId!=local.Id)throw new Exception($"Chose apparent-near across river ({visitor.LeisureSiteId}), expected actual-near {local.Id}");
        var copy=World.LoadJson(w.SaveJson());for(int i=0;i<100;i++){w.Tick(.1f);copy.Tick(.1f);}if(w.SaveJson()!=copy.SaveJson())throw new Exception("Leisure journey save differs");
        for(int i=0;i<1000 && visitor.LeisureVisits==0;i++)w.Tick(.1f);
        if(visitor.LastLeisureSiteId!=local.Id)throw new Exception("Local visit never completed");
        if(w.Place(new(7,0),1,BuildingKind.Bridge)==null)throw new Exception("Bridge: "+w.PlacementProblem(new(7,0),1,BuildingKind.Bridge));
        visitor.Position=new Cell(5,0).Point;visitor.NextLeisureTime=0;w.Assign(visitor.Id,Role.Unassigned);visitor.NextLeisureTime=0;
        for(int i=0;i<10 && visitor.LeisureSiteId==null;i++)w.Tick(.1f);
        if(visitor.LeisureSiteId!=across.Id)throw new Exception("Actual new crossing failed to change destination");
        for(int i=0;i<1000 && visitor.LastLeisureSiteId!=across.Id;i++)w.Tick(.1f);
        if(visitor.LastLeisureSiteId!=across.Id)throw new Exception("Crossing visit never completed");w.Validate();
        Console.WriteLine("PASS actual recreation walking choice, saved journey, completed local visit, new bridge changes journey and completed visit.");
    }
}

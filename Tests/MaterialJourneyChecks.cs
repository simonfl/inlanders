using Inlanders.Simulation;
static class MaterialJourneyChecks
{
    public static void Run()
    {
        var w=StorageChecks.Ready();var pile=w.Cottages.Single();w.Assign(1,Role.Hauler);
        for(int i=0;i<3000 && (pile.StoredLogs<6 || w.People[1].Carried>0 || w.People[1].Reserved>0);i++)w.Tick(.1f);
        w.Assign(1,Role.Unassigned);if(pile.StoredLogs<6)throw new Exception("Supply fixture");
        var site=w.Place(new(5,3),0,BuildingKind.SeatingGarden)??throw new Exception("Addition");
        var builder=w.People[2];builder.Position=w.YardAccess.Point;builder.NextMealTime=w.Food.Time+1000;w.Assign(builder.Id,Role.Builder);
        for(int i=0;i<10 && builder.Task!=Work.ToMaterials;i++)w.Tick(.1f);
        if(builder.Task!=Work.ToMaterials || builder.StorageId!=null)throw new Exception("Builder walks past available source to a longer whole material trip");
        var copy=World.LoadJson(w.SaveJson());for(int i=0;i<100;i++){w.Tick(.1f);copy.Tick(.1f);}if(w.SaveJson()!=copy.SaveJson())throw new Exception("Material journey save");
        for(int i=0;i<2000 && !site.Complete;i++)w.Tick(.1f);
        if(!site.Complete)throw new Exception("Whole-trip supplied addition did not complete");
        for(int i=0;i<1000 && !w.People.Any(p=>p.LastLeisureSiteId==site.Id);i++)w.Tick(.1f);
        if(!w.People.Any(p=>p.LastLeisureSiteId==site.Id))throw new Exception("Addition unused");w.Validate();
        Console.WriteLine("PASS whole material pickup/delivery route, exact active save, actual supplied construction and completed visit.");
    }
}

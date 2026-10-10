using Inlanders.Simulation;
static class ForagingSitingChecks
{
    public static void Run()
    {
        var w=World.NewRiverLivelihood(true);
        var far=w.Place(new(0,-7),0,BuildingKind.ForagerHut)!;
        var near=w.Place(new(-4,10),2,BuildingKind.ForagerHut)!;
        if(far==null || near==null)throw new Exception("Foraging arrangement unavailable");
        for(int i=0;i<100 && !w.People.Any(p=>p.BushId!=null);i++)w.Tick(.1f);
        var picker=w.People.Single(p=>p.BushId!=null);
        if(picker.WorkplaceId!=near.Id)throw new Exception("Old hut steals nearer source despite longer delivery");
        var copy=World.LoadJson(w.SaveJson());for(int i=0;i<600;i++){w.Tick(.1f);copy.Tick(.1f);}if(w.SaveJson()!=copy.SaveJson())throw new Exception("Gathering continuation differs");
        w.Validate();if(w.Food.GatheredBerries==0)throw new Exception("Sited gathering never delivered");
        Console.WriteLine("PASS foraging chooses complete source-to-hut walking trip, actual gathering, exact active save.");
    }
}

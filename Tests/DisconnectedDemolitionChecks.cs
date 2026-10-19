using Inlanders.Simulation;
static class DisconnectedDemolitionChecks
{
    public static void Run()
    {
        var w=World.NewRestorationChallenge();var field=w.Cottages.First(c=>c.Kind==BuildingKind.VegetableField);int id=field.Id;
        if(!w.RequestDemolition(id))throw new Exception("Demolition order refused");
        for(int i=0;i<50;i++)w.Tick(.1f);
        if(field.Builder!=null || !w.ReadWorkplace(field).MissingAccess)throw new Exception("Disconnected demolition claimed or unexplained");
        if(!w.CancelDemolition(id) || field.WorkPaused || !w.RequestDemolition(id))throw new Exception("Pending demolition not recoverable");
        w.ApplyWoodlandCare(new(-10,3),new(-5,13),WoodlandIntent.Renew,false);w.Place(new(5,0),0,BuildingKind.Bridge);
        bool second=false;
        for(int i=0;i<6000 && w.Cottages.Any(c=>c.Id==id);i++)
        {
            if(!second && w.Cottages.Any(c=>c.Kind==BuildingKind.Bridge && c.Complete)){if(w.Place(new(0,-7),0,BuildingKind.Bridge)==null)throw new Exception("Recovery crossing refused");second=true;}
            w.Tick(.1f);
        }
        if(w.Cottages.Any(c=>c.Id==id) || !second)throw new Exception("Reconnected demolition never recovered materials");
        w.Validate();var copy=World.LoadJson(w.SaveJson());for(int i=0;i<20;i++){w.Tick(.1f);copy.Tick(.1f);}if(w.SaveJson()!=copy.SaveJson())throw new Exception("Demolition save differs");
        Console.WriteLine("PASS disconnected demolition waits, cancels, reconnects, recovers crops/materials and saves exactly");
    }
}

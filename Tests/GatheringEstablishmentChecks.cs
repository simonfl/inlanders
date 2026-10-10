using Inlanders.Simulation;
static class GatheringEstablishmentChecks
{
    public static void Run()
    {
        foreach(bool relaxed in new[]{false,true})foreach(var kind in new[]{BuildingKind.ForagerHut,BuildingKind.HuntingLodge,BuildingKind.Orchard})
        {
            var w=World.NewRiverLivelihood(relaxed);Cottage? site=null;
            foreach(var at in w.Map.Land.OrderBy(c=>(c.Point-w.Stockpile.Point).LengthSquared()))
            {for(int r=0;r<4 && site==null;r++)site=w.Place(at,r,kind);if(site!=null)break;}
            if(site==null)throw new Exception("No gathering site");
            bool saved=false;float first=0;
            for(int i=0;i<4500 && site.EstablishmentPending;i++)
            {
                w.Tick(.1f);
                if(!saved && site.Complete && w.People.Any(p=>p.WorkplaceId==site.Id))
                {
                    var copy=World.LoadJson(w.SaveJson());for(int j=0;j<100;j++){w.Tick(.1f);copy.Tick(.1f);}if(w.SaveJson()!=copy.SaveJson())throw new Exception("Active gathering continuation differs");saved=true;
                }
                if(!site.EstablishmentPending)first=w.Food.Time;
            }
            if(site.EstablishmentPending || !saved || !w.FoundingHasNewFood)throw new Exception("First gathering did not deliver/close");
            w.Validate();Console.WriteLine($"PASS gathering first cycle {kind} relaxed={relaxed} at {first:0}s, exact active continuation.");
        }
    }
}

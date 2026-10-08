using Inlanders.Simulation;
static class FieldCollectionChecks
{
    public static void Run()
    {
        foreach(bool relaxed in new[]{false,true})foreach(int rows in new[]{2,5,8})
        {
            var w=World.NewRiverLivelihood(relaxed);var f=w.Place(new(2,-7),1,BuildingKind.VegetableField,rows)??throw new Exception("Field refused");
            World? continuation=null;bool started=false,saved=false;float ripe=0;int trips=0,last=0;
            for(int i=0;i<10000;i++)
            {
                w.Tick(.1f);continuation?.Tick(.1f);
                if(!started && f.Harvest>0){started=true;ripe=w.Food.Time;last=f.Harvest;}
                if(started && f.Harvest<last){trips++;last=f.Harvest;}
                if(started && !saved && w.People.Any(p=>p.WorkplaceId==f.Id && p.Task==Work.Harvesting))
                {
                    continuation=World.LoadJson(w.SaveJson());saved=true;
                }
                if(started && f.Harvest==0)break;
            }
            w.Validate();if(continuation?.SaveJson()!=w.SaveJson())throw new Exception("Active harvest continuation differs");if(trips!=rows)throw new Exception("Not one row per collection");if(!started || f.Harvest!=0 || !saved)throw new Exception("Crop not collected");
            Console.WriteLine($"FIELD COLLECTION relaxed={relaxed} rows={rows}: {trips} trips, ripe-to-collected {w.Food.Time-ripe:0.0}s, vegetables {w.Food.GrownVegetables}");
        }
    }
}

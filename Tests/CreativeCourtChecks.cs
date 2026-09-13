using Inlanders.Simulation;

static class CreativeCourtChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    static void Step(World w,int ticks){for(int i=0;i<ticks;i++){w.Tick(.1f);if(i%100==0)w.Validate();}w.Validate();}
    public static World Arranged(World? source=null)
    {
        var w=World.CreativeCourtFrom(source??CourtChecks.Expanded());
        // A self-chosen example: open the middle of the court and face the houses
        // toward a western lane. No extra producer or stock supplied to this arm.
        var homes=w.Cottages.Where(c=>c.Kind==BuildingKind.Cottage).Take(2).ToArray();
        foreach(var (home,target) in homes.Zip(new[]{new Cell(-7,1),new(-7,5)}))
        {
            var at=w.Map.Land.OrderBy(c=>(c.Point-target.Point).LengthSquared()).First(c=>c!=home.Cell && w.RelocationProblem(home.Id,c,1)==null);
            Check(w.MoveBuilding(home.Id,at,1),"Free home move failed");
        }
        var venue=w.Cottages.First(c=>c.Kind==BuildingKind.SeatingGarden);
        var center=w.Map.Land.OrderBy(c=>(c.Point-new Cell(-2,1).Point).LengthSquared()).First(c=>c!=venue.Cell && w.RelocationProblem(venue.Id,c,0)==null);
        Check(w.MoveBuilding(venue.Id,center,0),"Free venue move failed");
        w.Validate();return w;
    }
    public static void Run()
    {
        var normal=CourtChecks.Expanded();string baseline=normal.SaveJson();
        Check(normal.Population==16,"Comparison needs an inhabited village");
        var free=Arranged(normal);
        Check(normal.SaveJson()==baseline && free.Food.Time==normal.Food.Time && free.EdibleStored==normal.EdibleStored && free.Population==normal.Population,"Arrangement changed baseline/time/food/residents");
        Check(free.Neighborhood!.Arrangement!.BuildingId==null && free.People.All(p=>p.SharedWorker),"Free mode retained trial lock or lost shared work");
        var site=free.Place(free.Map.Land.First(c=>free.PlacementProblem(c,0,BuildingKind.Cottage)==null))!;
        Check(site.Complete && site.Delivered==0 && free.RemoveBuilding(site.Id),"Free build/remove failed");
        foreach(var w in new[]{normal,free})
        {
            int eaten=w.Food.EatenBerries+w.Food.EatenVegetables,rest=w.People.Sum(p=>p.RestVisits),leisure=w.People.Sum(p=>p.LeisureVisits);
            Step(w,1800);
            Check(w.Food.EatenBerries+w.Food.EatenVegetables>eaten && w.People.Sum(p=>p.RestVisits)>rest && w.People.Sum(p=>p.LeisureVisits)>leisure,"Daily life did not continue");
            string save=w.SaveJson();var loaded=World.LoadJson(save);Check(loaded.SaveJson()==save,"Save roundtrip differs");
            Step(w,300);Step(loaded,300);Check(w.SaveJson()==loaded.SaveJson(),"Save continuation differs");
            Console.WriteLine($"{(w.Creative?"Free lane":"Constrained court")}: food {w.EdibleStored}, hungry {w.Food.Hunger}, home rests {w.People.Sum(p=>p.RestVisits)}, recreation {w.People.Sum(p=>p.LeisureVisits)}");
        }
        var quiet=World.NewCreativeCourt();foreach(var c in quiet.Cottages.Where(c=>Buildings.Get(c.Kind).Worker!=null))quiet.SetWorkplacePaused(c.Id,true);
        foreach(var t in quiet.Trees)t.Preserved=true;
        quiet.SetCreativeCentralStock(Resource.Logs,24);
        Step(quiet,1500);
        Check(quiet.People.Any(p=>p.Status=="At home — available for work"),"Idle workers did not disperse home");
        var walker=quiet.People.First(p=>p.HomeId!=null);var home=quiet.Cottages.Single(c=>c.Id==walker.HomeId);
        Check(quiet.RemoveBuilding(home.Id),"Idle home removal failed");Step(quiet,100);
        foreach(var kind in new[]{Resource.Berries,Resource.Vegetables})quiet.SetCreativeCentralStock(kind,quiet.CreativeProtectedStock(kind));
        Step(quiet,2000);Check(quiet.People.Any(p=>!p.Fed),"Shortage fixture did not miss meals");Check(quiet.Food.Hunger==0 && quiet.Food.WorkEfficiency==1 && quiet.ReadHappiness(quiet.People[0]).Meals==30,"Creative hunger penalties returned");
        var producer=quiet.Cottages.First(c=>c.Kind==BuildingKind.VegetableGarden);quiet.SetWorkplacePaused(producer.Id,false);int grown=quiet.Food.GrownVegetables;Step(quiet,1200);Check(quiet.Food.GrownVegetables>grown,"Available workers did not resume production");
        var venue=quiet.Cottages.First(c=>c.Kind==BuildingKind.SeatingGarden);Check(quiet.ChooseWelcomeVenue(venue.Id) && quiet.RemoveBuilding(venue.Id) && quiet.Neighborhood!.VenueId==null,"Removed welcome table retained its selection");quiet.Validate();
        for(int i=0;i<1200 && !quiet.People.Any(p=>p.Meal is {Reserved:true,SourceId:not null});i++)quiet.Tick(.1f);
        var claim=quiet.People.FirstOrDefault(p=>p.Meal is {Reserved:true,SourceId:not null});Check(claim!=null,"No local meal claim to test removal");
        int source=claim!.Meal!.SourceId!.Value;Check(quiet.RemoveBuilding(source),"Live producer removal failed");Step(quiet,500);
        Check(World.NewCreativeCourt().Creative && World.NewCreative().SimulatesMeals==false,"New or historical rules differ");
        Console.WriteLine("PASS: matched sixteen-resident free arrangement, multiple moves, free placement/removal, real meals/rest/recreation, quiet home waiting, home removal, forgiving shortage and exact continuation.");
    }
}

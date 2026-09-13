using Inlanders.Simulation;

static class WelcomeMealChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    static void Until(World w,Func<bool> done,string why,int ticks=24000)
    {
        for(int i=0;i<ticks && !done();i++){w.Tick(.1f);if(i%20==0)w.Validate();}
        Check(done(),why+" at "+w.Food.Time+"; "+w.WelcomeStatus+"; "+string.Join("; ",w.People.Select(p=>p.Status)));w.Validate();
    }
    static Cottage PlaceEast(World w,BuildingKind kind)=>w.Map.Land.Where(c=>c.X>6 && w.PlacementProblem(c,0,kind)==null)
        .OrderBy(c=>(c.Point-new Cell(8,2).Point).LengthSquared()).Select(c=>w.Place(c,0,kind)!).First();
    static void Continuation(World w)
    {
        var copy=World.LoadJson(w.SaveJson());
        for(int i=0;i<100;i++){w.Tick(.1f);copy.Tick(.1f);Check(w.SaveJson()==copy.SaveJson(),"Welcome continuation differs");}
    }
    public static World PrepareReview()
    {
        var w=World.NewNeighborhoodExperiment();var bridge=w.Place(new(5,2),1,BuildingKind.Bridge)!;
        Until(w,()=>bridge.Complete,"Review crossing");var venue=PlaceEast(w,BuildingKind.SeatingGarden);
        Until(w,()=>venue.Complete,"Review venue");return w;
    }
    public static void Run()
    {
        Directory.CreateDirectory("artifacts/welcome-meal");
        foreach(var kind in new[]{BuildingKind.SeatingGarden,BuildingKind.Square})
        {
            var w=World.NewNeighborhoodExperiment();var bridge=w.Place(new(5,2),1,BuildingKind.Bridge)!;
            Until(w,()=>bridge.Complete,"Bridge");var venue=PlaceEast(w,kind);Until(w,()=>venue.Complete,"Venue");
            Check(w.ChooseWelcomeVenue(venue.Id),"Venue refused");
            Until(w,()=>w.People.Any(p=>p.Task==Work.ToFoodPickup && p.FoodDestinationId==venue.Id),"Welcome pickup");
            string pickup=w.SaveJson();Continuation(w);
            Until(w,()=>w.People.Any(p=>p.Task==Work.ToPantry && p.FoodDestinationId==venue.Id && p.Carried>0),"Carried welcome delivery");
            string delivery=w.SaveJson();Continuation(w);
            Until(w,()=>venue.PantryFood.Sum()==12,"Prepared portions before arrival");
            Check(w.Neighborhood!.Welcomed.Count==0,"Attendance before arrivals");
            string prepared=w.SaveJson();
            for(int i=0;i<300;i++)w.Tick(.1f);
            Check(venue.PantryFood.Sum()==12,"Prepared food vanished while waiting");
            Check(w.InviteNewcomers(),"Arrival commitment");
            Until(w,()=>w.People.Any(p=>p.Meal is {Welcome:true,Carrying:true}),"Actual welcome meal journey");string eating=w.SaveJson();Continuation(w);
            Until(w,()=>w.Neighborhood.Welcomed.Count==12,"All attendees eat");
            Check(!w.Neighborhood.Complete && w.Housed==8,"Meal bypassed newcomer homes");
            for(int homes=0;homes<2;homes++){var home=PlaceEast(w,BuildingKind.Cottage);Until(w,()=>home.Complete,"New home");}
            Until(w,()=>w.Neighborhood.Complete,"Neighborhood completion");
            Check(!w.Food.SupperComplete && !w.Food.Celebrating,"Old global supper used");Continuation(w);
            File.WriteAllText($"artifacts/welcome-meal/{kind}-complete.json",w.SaveJson());

            var interruption=World.LoadJson(pickup);var helper=interruption.People.First(p=>p.FoodDestinationId==venue.Id);
            interruption.Assign(helper.Id,Role.Builder);interruption.Validate();Check(helper.PantryReserved==0,"Interrupted pickup kept reservation");Continuation(interruption);
            var returning=World.LoadJson(delivery);var carrier=returning.People.First(p=>p.Task==Work.ToPantry && p.FoodDestinationId==venue.Id);
            int cargo=carrier.Carried;returning.Assign(carrier.Id,Role.Builder);returning.Validate();
            Check(carrier.Carried==cargo && carrier.FoodDestinationId==null,"Carried welcome food lost on assignment");Continuation(returning);
            var activeRemoval=World.LoadJson(eating);Check(activeRemoval.RequestDemolition(venue.Id),"Active welcome demolition refused");activeRemoval.Validate();Continuation(activeRemoval);
            var recovery=World.LoadJson(prepared);int stored=recovery.EdibleStored;
            Check(recovery.RequestDemolition(venue.Id),"Venue demolition refused");Check(recovery.Neighborhood!.VenueId==null,"Demolition left active venue");
            Check(recovery.EdibleStored==stored,"Demolition removed food immediately");
            Until(recovery,()=>recovery.Cottages.All(c=>c.Id!=venue.Id),"Physical demolition recovery");Continuation(recovery);
            var replacement=PlaceEast(recovery,kind);Until(recovery,()=>replacement.Complete,"Replacement venue");
            Check(recovery.ChooseWelcomeVenue(replacement.Id),"Replacement selection");Until(recovery,()=>replacement.PantryFood.Sum()==12,"Replacement resupply");
        }
        Console.WriteLine("PASS: two venue kinds, physical preparation/visits/eating, persistent stock, interruption, demolition/replacement, housing-gated completion and exact active saves.");
    }
}

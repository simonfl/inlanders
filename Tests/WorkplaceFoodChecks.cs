using Inlanders.Simulation;

static class WorkplaceFoodChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    static void Until(World w,Func<bool> done,string why,int ticks=24000)
    {
        for(int i=0;i<ticks && !done();i++){w.Tick(.1f);if(i%20==0)w.Validate();}
        Check(done(),why+" at "+w.Food.Time);w.Validate();
    }
    static void Continuation(World w)
    {
        var copy=World.LoadJson(w.SaveJson());
        for(int i=0;i<100;i++){w.Tick(.1f);copy.Tick(.1f);Check(w.SaveJson()==copy.SaveJson(),"Workplace food continuation differs");}
        w.Validate();copy.Validate();
    }
    public static void Run()
    {
        var w=World.NewWorkplaceFoodExperiment();var hut=w.Cottages.Single(c=>c.Kind==BuildingKind.ForagerHut);
        Check(w.HasWorkplaceFood && !World.NewNeighborhoodExperiment().HasWorkplaceFood,"Food workflow leaked into control");
        Until(w,()=>w.People.Any(p=>p.Task==Work.ToPantry && p.FoodDestinationId==hut.Id && p.Carried>0),"Producer never carried output to its workplace");
        string producing=w.SaveJson();Continuation(w);
        Until(w,()=>w.FoodAt(hut.Id,Resource.Berries)>0,"Workplace stock never arrived");
        Until(w,()=>w.People.Any(p=>p.Task==Work.ToFoodPickup && p.FoodSourceId==hut.Id),"Surplus distribution never reserved workplace food");
        string reserved=w.SaveJson();Continuation(w);
        Until(w,()=>w.People.Any(p=>p.FoodTransfer && p.Task==Work.ToPantry && p.Carried>0),"Distribution never carried physical food");
        string carrying=w.SaveJson();Continuation(w);
        Until(w,()=>w.People.Any(p=>p.Meal is {Carrying:true} meal && meal.SourceId==hut.Id),"Residents never collected workplace meals");
        Continuation(w);
        var bridge=w.Place(new(5,2),1,BuildingKind.Bridge)!;Until(w,()=>bridge.Complete,"Food review crossing");
        Cottage Build(BuildingKind kind)
        {
            var cell=w.Map.Land.Where(c=>c.X>6).OrderBy(c=>(c.Point-new Cell(17,6).Point).LengthSquared()).First(c=>w.PlacementProblem(c,0,kind)==null);
            var site=w.Place(cell,0,kind)!;Until(w,()=>site.Complete,"Food review building "+kind);return site;
        }
        var garden=Build(BuildingKind.VegetableGarden);var farm=Build(BuildingKind.Farm);var bakery=Build(BuildingKind.Bakery);
        var pantry=Build(BuildingKind.Pantry);Check(w.SetPantryTarget(pantry.Id,16),"Pantry target refused");
        Until(w,()=>w.FoodAt(garden.Id,Resource.Vegetables)>0,"Garden did not store vegetables locally");
        Until(w,()=>w.FoodAt(bakery.Id,Resource.Bread)>0,"Bakery did not store bread locally");
        Until(w,()=>w.People.Any(p=>p.Task==Work.ToFoodPickup && p.FoodDestinationId==pantry.Id && p.FoodSourceId is int id && w.IsWorkplaceFoodStore(w.Cottages.Single(c=>c.Id==id))),"Pantry was not supplied directly from a workplace");
        Continuation(w);
        Check(w.SetWorkplacePaused(bakery.Id,true),"Producer pause refused");
        Until(w,()=>w.People.All(p=>p.WorkplaceId!=bakery.Id),"Paused bakery work did not finish");
        Check(w.FoodStoreName(bakery.Id).Contains("Bakery"),"Workplace source is mislabeled as a pantry");
        Continuation(w);
        foreach(string state in new[]{producing,reserved,carrying})
        {
            var removal=World.LoadJson(state);
            Check(removal.RequestDemolition(hut.Id),"Workplace storage demolition refused");
            removal.Validate();Continuation(removal);
            Until(removal,()=>removal.Cottages.All(c=>c.Id!=hut.Id),"Workplace food demolition recovery stalled");
            removal.Validate();
        }
        Console.WriteLine("PASS: workplace output, physical meals/distribution, local pantry replenishment, paused storage, interruption/demolition conservation and exact active saves; control remains unchanged.");
    }
}

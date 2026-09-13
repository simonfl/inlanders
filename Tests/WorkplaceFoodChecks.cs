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
    public static World PrepareVillage()
    {
        var w=World.NewWorkplaceFoodExperiment();
        var bridge=w.Place(new(5,2),1,BuildingKind.Bridge)!;Until(w,()=>bridge.Complete,"Village crossing");
        Cottage Build(BuildingKind kind,Cell center)
        {
            var choice=w.Map.Land.Where(c=>c.X>5).SelectMany(cell=>Enumerable.Range(0,4).Select(rotation=>new{cell,rotation}))
                .Where(p=>w.PlacementProblem(p.cell,p.rotation,kind)==null)
                .OrderBy(p=>(p.cell.Point-center.Point).LengthSquared()).ThenBy(p=>p.cell.Z).ThenBy(p=>p.cell.X).ThenBy(p=>p.rotation).First();
            var site=w.Place(choice.cell,choice.rotation,kind)!;Until(w,()=>site.Complete,"Village "+kind);return site;
        }
        Build(BuildingKind.Cottage,new(8,2));Build(BuildingKind.Cottage,new(10,4));
        var venue=Build(BuildingKind.SeatingGarden,new(8,3));
        Build(BuildingKind.Farm,new(17,3));Build(BuildingKind.Farm,new(21,3));
        Build(BuildingKind.Bakery,new(17,7));Build(BuildingKind.Bakery,new(21,7));
        var pantry=Build(BuildingKind.Pantry,new(14,5));w.SetPantryTarget(pantry.Id,16);
        Check(w.ChooseWelcomeVenue(venue.Id) && w.InviteNewcomers(),"Village welcome refused");
        Until(w,()=>w.Neighborhood!.Complete,"Workplace village welcome");
        Check(w.SetWorkplacePaused(w.Cottages.Single(c=>c.Kind==BuildingKind.ForagerHut).Id,true),"Village foraging pause refused");
        for(int i=0;i<6000;i++){w.Tick(.1f);if(i%100==0)w.Validate();}
        Check(w.Food.BakedBread>0 && w.Cottages.Where(c=>c.Kind==BuildingKind.Bakery).Any(c=>c.PantryFood.Sum()>0),"Working village has no local bread");
        Continuation(w);return w;
    }
    public static void OtherProducers()
    {
        foreach(var pair in new[]{(BuildingKind.Orchard,Resource.Fruit),(BuildingKind.FishingDock,Resource.Fish),(BuildingKind.HuntingLodge,Resource.Game)})
        {
            var (kind,food)=pair;var w=World.NewWorkplaceFoodExperiment();
            w.Food.InitialBerries=w.Food.Berries=1000;
            w.Map.FishingGrounds.Add(new(){Id=0,Cell=new(5,-3),Capacity=16,Stock=16});
            var habitat=new WoodlandHabitat{Id=0,Cell=new(-5,-3)};
            habitat.Stock=w.HabitatCapacity(habitat);w.Map.Wildlife.Add(habitat);
            if(kind==BuildingKind.HuntingLodge)foreach(var tree in w.Trees.Where(t=>habitat.Contains(t.Cell)))tree.Preserved=true;
            foreach(var hut in w.Cottages.Where(c=>c.Kind==BuildingKind.ForagerHut))w.SetWorkplacePaused(hut.Id,true);
            var choice=w.Map.Land.Where(c=>c.X<5).SelectMany(cell=>Enumerable.Range(0,4).Select(rotation=>new{cell,rotation}))
                .Where(p=>w.PlacementProblem(p.cell,p.rotation,kind)==null)
                .OrderBy(p=>(p.cell.Point-new Cell(2,0).Point).LengthSquared()).ThenBy(p=>p.cell.Z).ThenBy(p=>p.cell.X).ThenBy(p=>p.rotation).First();
            var site=w.Place(choice.cell,choice.rotation,kind)!;Until(w,()=>site.Complete,kind+" construction");
            Check(w.DedicateWorker(site.Id),kind+" dedicated worker");
            Until(w,()=>w.People.Any(p=>p.Task==Work.ToPantry && p.FoodDestinationId==site.Id && p.Cargo==food && p.Carried>0),kind+" never returned output locally");
            Continuation(w);
            Until(w,()=>w.FoodAt(site.Id,food)>0,kind+" never deposited output");
            string stocked=w.SaveJson();
            Check(w.SetWorkplacePaused(site.Id,true),kind+" pause");
            // Remove only fixture starting berries; production and meal accounting remain conserved.
            int removed=w.FoodAvailableAt(null,Resource.Berries);w.Food.InitialBerries-=removed;w.Food.Berries-=removed;
            Until(w,()=>w.People.Any(p=>p.Meal is {Carrying:true} meal && meal.SourceId==site.Id && meal.Kind==food),kind+" paused store did not serve a physical meal");
            Continuation(w);
            var removal=World.LoadJson(stocked);Check(removal.RequestDemolition(site.Id),kind+" demolition");
            Continuation(removal);Until(removal,()=>removal.Cottages.All(c=>c.Id!=site.Id),kind+" stored food recovery");
            Console.WriteLine($"PASS: {kind} physical local output, paused meal pickup, active saves and demolition conservation.");
        }
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
        Until(w,()=>w.People.Any(p=>p.Meal is {Reserved:true} meal && meal.SourceId==hut.Id),"No active workplace meal reservation to pause");
        Check(w.SetWorkplacePaused(hut.Id,true),"Forager pause refused");w.Validate();
        Continuation(w);
        Until(w,()=>w.People.Any(p=>p.Meal is {Carrying:true} meal && meal.SourceId==hut.Id),"Paused workplace stopped meal collection");
        Check(w.SetWorkplacePaused(hut.Id,false),"Forager resume refused");
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

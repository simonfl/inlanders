using Inlanders.Simulation;
static class WorkFocusChecks
{
    public static void Run()
    {
        foreach(bool relaxed in new[]{false,true})
        {
            var w=World.NewNewcomerChallenge(relaxed);w.Food.InitialBerries=w.Food.Berries=42;
            var garden=w.Cottages.First(c=>World.IsVegetablePlot(c.Kind));garden.Planted=true;garden.Growth=1;garden.Harvest=8;w.Food.GrownVegetables+=8;
            foreach(var person in w.People)person.NextMealTime=100;
            if(!relaxed){var at=ReviewPlacement.Find(w,BuildingKind.Cottage,new(-2,10),(c,r)=>c.Z>0,"focus construction")!;if(w.Place(at.Actual,at.Rotation,BuildingKind.Cottage)==null)throw new Exception("No building order");}
            var balanced=World.LoadJson(w.SaveJson());w.SetFoodFirst(true);w.Tick(.1f);balanced.Tick(.1f);
            if(!w.People.Any(p=>p.Role==Role.Farmer && p.WorkplaceId==garden.Id))throw new Exception("Food focus did not claim harvest");
            if(!relaxed && (w.People[0].Role!=Role.Farmer || balanced.People[0].Role!=Role.Builder))throw new Exception($"Food/build tradeoff absent: food={w.People[0].Role}/{w.People[0].Status}; balanced={balanced.People[0].Role}/{balanced.People[0].Status}; sites={balanced.Cottages.Count(c=>!c.Complete)}");
            var jobs=w.People.Select(p=>(p.Task,p.WorkplaceId,p.Carried)).ToArray();w.SetFoodFirst(false);
            if(!jobs.SequenceEqual(w.People.Select(p=>(p.Task,p.WorkplaceId,p.Carried))))throw new Exception("Switch interrupted current work");
            w.SetFoodFirst(true);var copy=World.LoadJson(w.SaveJson());for(int i=0;i<500;i++){w.Tick(.1f);copy.Tick(.1f);}
            if(w.SaveJson()!=copy.SaveJson() || w.StoredFood(Resource.Vegetables)==0)throw new Exception("Food focus delivery/save failure");
            w.Validate();Console.WriteLine($"PASS food focus scheduling, no interruption, real harvest delivery and exact save relaxed={relaxed}");
        }
    }
}

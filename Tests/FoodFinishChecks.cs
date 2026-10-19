using Inlanders.Simulation;
static class FoodFinishChecks
{
    public static void Run()
    {
        foreach(bool relaxed in new[]{false,true})
        {
            var w=World.NewRiverFrontage(relaxed,true,true);
            if(w.FoodWorkNeeded)throw new Exception("Fixture not provisioned");
            for(int i=0;i<3000 && (w.Food.GrownVegetables<40 || w.Cottages.Any(c=>c.Kind==BuildingKind.VegetableField && (c.Harvest>0 || c.Planted)));i++)w.Tick(.1f);
            if(w.StoredFood(Resource.Vegetables)==0 || w.Cottages.Any(c=>c.Kind==BuildingKind.VegetableField && (c.Harvest>0 || c.Planted)) || w.Food.GrownVegetables!=40)
                throw new Exception($"Committed crop stranded/replanted: delivered {w.StoredFood(Resource.Vegetables)}, grown {w.Food.GrownVegetables}");
            var copy=World.LoadJson(w.SaveJson());for(int i=0;i<30;i++){w.Tick(.1f);copy.Tick(.1f);}
            if(w.SaveJson()!=copy.SaveJson())throw new Exception("Food completion save mismatch");
            w.Validate();Console.WriteLine($"PASS relaxed={relaxed}: both existing crops collected above reserve, no replacement crop; exact save.");
        }
    }
}

using Inlanders.Simulation;
static class ReachableFoodChecks
{
    public static void Run()
    {
        foreach(bool relaxed in new[]{false,true})
        {
            var w=World.NewRestorationChallenge(relaxed);
            foreach(var field in w.Cottages.Where(c=>c.Kind==BuildingKind.VegetableField)){field.PantryFood[Array.IndexOf(World.EdibleKinds,Resource.Vegetables)]=24;w.Food.GrownVegetables+=24;}
            w.Validate();string before=w.SaveJson();
            if(w.EdibleStored!=72 || w.ReachableEdibleStored!=24 || !w.FoodWorkNeeded || before!=w.SaveJson())throw new Exception("Disconnected food falsely suppresses production or query mutates state");
            w.ApplyWoodlandCare(new(-10,3),new(-5,13),WoodlandIntent.Renew,false);w.Place(new(5,0),0,BuildingKind.Bridge);bool second=false;
            for(int i=0;i<3000 && !(second && w.Cottages.Count(c=>c.Kind==BuildingKind.Bridge && c.Complete)==2);i++)
            {
                if(!second && w.Cottages.Any(c=>c.Kind==BuildingKind.Bridge && c.Complete)){if(w.Place(new(0,-7),0,BuildingKind.Bridge)==null)throw new Exception("Second crossing refused");second=true;}
                w.Tick(.1f);
            }
            if(!second || w.ReachableEdibleStored!=w.EdibleStored)throw new Exception("Food did not become usable after reconnecting");
            var copy=World.LoadJson(w.SaveJson());for(int i=0;i<20;i++){w.Tick(.1f);copy.Tick(.1f);}if(w.SaveJson()!=copy.SaveJson())throw new Exception("Reconnected food save differs");
            w.Validate();Console.WriteLine($"PASS stranded food excluded, real crossings restore availability, pure query and exact save relaxed={relaxed}");
        }
    }
}

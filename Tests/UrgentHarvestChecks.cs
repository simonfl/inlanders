using Inlanders.Simulation;
static class UrgentHarvestChecks
{
    public static void Run()
    {
        foreach(bool relaxed in new[]{false,true})
        {
            var w=World.NewRiverFrontage(false,true,true);
            var at=ReviewPlacement.Find(w,BuildingKind.Farm,new(-3,-5),(c,r)=>true,"grain comparison")!;
            var grain=w.Place(at.Actual,at.Rotation,BuildingKind.Farm)!;
            // Synthetic ready farm, with its construction timber accounted for.
            var data=System.Text.Json.Nodes.JsonNode.Parse(w.SaveJson())!;
            var built=data["Buildings"]!.AsArray().Single(b=>(int)b!["Id"]! ==grain.Id)!;
            built["Delivered"]=grain.Required;built["Construction"]=1;data["InitialLogs"]=w.InitialLogs+grain.Required;
            int grainId=grain.Id;w=World.LoadJson(data.ToJsonString());grain=w.Cottages.Single(c=>c.Id==grainId);
            grain.Planted=true;grain.Growth=1;grain.Harvest=6;w.Food.GrownGrain+=6;
            var vegetables=w.Cottages.First(c=>c.Kind==BuildingKind.VegetableField);vegetables.Planted=true;vegetables.Growth=1;vegetables.Harvest=World.VegetableYield(vegetables);w.Food.GrownVegetables+=vegetables.Harvest;
            foreach(var f in w.Cottages.Where(c=>World.IsVegetablePlot(c.Kind) && c.Id!=vegetables.Id))w.SetWorkplacePaused(f.Id,true);
            w.Food.InitialBerries=w.Food.Berries=0;foreach(var person in w.People)person.NextMealTime=100;
            w.Assign(0,Role.Farmer);w.People[0].Position=grain.Entrance.Point;
            w.Validate();
            if(relaxed)w=World.RelaxedHamletFrom(w);
            var stocked=World.LoadJson(w.SaveJson());stocked.Food.InitialBerries=stocked.Food.Berries=72;
            w.Tick(.1f);stocked.Tick(.1f);
            if(w.People[0].WorkplaceId!=vegetables.Id || stocked.People[0].WorkplaceId!=grain.Id)throw new Exception($"Urgent edible harvest ignored: urgent={w.People[0].WorkplaceId}, stocked={stocked.People[0].WorkplaceId}, grain={grain.Id}, vegetables={vegetables.Id}");
            var copy=World.LoadJson(w.SaveJson());for(int i=0;i<600;i++){w.Tick(.1f);copy.Tick(.1f);}
            if(w.StoredFood(Resource.Vegetables)==0 || w.SaveJson()!=copy.SaveJson())throw new Exception("Urgent harvest delivery/save failure");w.Validate();
            Console.WriteLine($"PASS scarce meals choose edible crop; stocked village chooses nearer grain; real delivery and exact save relaxed={relaxed}");
        }
    }
}

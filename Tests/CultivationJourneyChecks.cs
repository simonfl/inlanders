using Inlanders.Simulation;
static class CultivationJourneyChecks
{
    public static void Run()
    {
        foreach(bool priority in new[]{false,true})
        {
            var w=World.NewRiverLivelihood(true);
            var far=w.Place(new(0,-7),0,BuildingKind.VegetableGarden)??throw new Exception("Older garden");
            var near=w.Place(new(-4,4),2,BuildingKind.VegetableGarden)??throw new Exception("Local garden");
            foreach(var p in w.People){w.Assign(p.Id,Role.Builder);p.NextMealTime=p.NextRestTime=p.NextLeisureTime=10000;}
            var farmer=w.People[0];farmer.Position=near.Entrance.Point;w.Assign(farmer.Id,Role.Farmer);
            if(priority)far.Priority=2; // Retained construction priority on a completed building.
            w.Tick(.1f);int expected=near.Id;
            if(farmer.WorkplaceId!=expected)throw new Exception($"Farmer ignored {(priority?"explicit priority":"local growing ground")}: {farmer.WorkplaceId} != {expected}");
            var copy=World.LoadJson(w.SaveJson());for(int i=0;i<100;i++){w.Tick(.1f);copy.Tick(.1f);}if(copy.SaveJson()!=w.SaveJson())throw new Exception("Cultivation journey continuation differs");
            for(int i=0;i<3500 && w.ReadFoodFlow().Vegetables==0;i++)w.Tick(.1f);
            if(w.ReadFoodFlow().Vegetables==0)throw new Exception("Crop never delivered");w.Validate();
        }
        Console.WriteLine("PASS nearby plot sowing and real harvest/delivery, old construction priority does not bias ongoing work, exact active save.");
    }
}

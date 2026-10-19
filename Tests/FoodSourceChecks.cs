using Inlanders.Simulation;
static class FoodSourceChecks
{
    public static void Run()
    {
        foreach(bool relaxed in new[]{false,true})
        {
            var w=World.NewRestorationChallenge(relaxed);
            var p=ReviewPlacement.Find(w,BuildingKind.FishingDock,new(8,4),(c,r)=>c.Z>2,"source check")!;
            var dock=w.Place(p.Actual,p.Rotation,BuildingKind.FishingDock)!;w.SetLocalFoodReserve(dock.Id,0);
            for(int i=0;i<1200;i++)w.Tick(.1f);
            if(w.DeliveredFish==0 || w.WorkplaceFoodDelivered(dock.Id)!=w.DeliveredFish || w.RecentFood.Where(e=>e.Fish>0).Any(e=>e.SourceWorkplaceId!=dock.Id))throw new Exception("Fish not attributed to actual producer, or transfers counted twice");
            if(w.Cottages.Sum(c=>w.WorkplaceFoodDelivered(c.Id))!=w.ReadFoodFlow().Delivered)throw new Exception("Producer contributions do not cover actual deliveries");
            string before=w.SaveJson();w.WorkplaceFoodDelivered(dock.Id);if(w.SaveJson()!=before)throw new Exception("Read changed world");
            var copy=World.LoadJson(before);for(int i=0;i<30;i++){w.Tick(.1f);copy.Tick(.1f);}if(w.SaveJson()!=copy.SaveJson())throw new Exception("Source history save differs");
            w.Validate();Console.WriteLine($"PASS producer-attributed deliveries, zero local reserve/distribution, pure query and exact save relaxed={relaxed}");
        }
    }
}

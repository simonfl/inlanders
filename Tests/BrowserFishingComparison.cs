using Inlanders.Simulation;
using System.Text.Json;

static class BrowserFishingComparison
{
    public static void Run()
    {
        var results = new List<object>();
        foreach(bool depleted in new[]{false,true}) foreach(var kind in new[]{BuildingKind.VegetableField,BuildingKind.FishingDock})
        {
            var w=World.NewTimberTrade();w.DecideTimberTrade(true);
            int Place(BuildingKind type,Cell target)
            {
                var p=ReviewPlacement.Find(w,type,target,(c,r)=>c.Z>=0,"food comparison") ?? throw new Exception("No site for "+type);
                return w.Place(p.Actual,p.Rotation,type)!.Id;
            }
            w.ApplyWoodlandCare(new(-11,5),new(6,12),WoodlandIntent.Renew,false);
            Place(BuildingKind.Cottage,new(1,6));Place(BuildingKind.Cottage,new(5,6));
            while(w.Food.Time<120)w.Tick(.1f);
            // Controlled comparison of the river stock at the expansion decision.
            // This is a fixture, not a claim of ordinary play or an agent shortcut.
            if(depleted)foreach(var g in w.Map.FishingGrounds)g.Stock=0;
            int id=Place(kind,kind==BuildingKind.FishingDock?new(8,3):new(-6,5));
            var seen=new HashSet<FoodFlowEvent>();int delivered=0,at240=0;float first=-1,completion=-1;bool saved=false;
            while(w.Food.Time<600)
            {
                w.Tick(.1f);
                foreach(var e in w.RecentFood.Where(e=>kind==BuildingKind.FishingDock ? e.Fish>0 : e.SourceWorkplaceId==id))if(seen.Add(e))
                {int food=e.Berries+e.Vegetables+e.Fish+e.Bread+e.Game+e.Fruit;delivered+=food;if(food>0 && first<0)first=w.Food.Time;}
                if(w.Food.Time<240)at240=delivered;
                if(completion<0 && w.Founding!.FirstHome!.Complete)completion=w.Food.Time;
                if(!saved && w.Cottages.Single(c=>c.Id==id).Boat is {Phase:BoatPhase.Returning,Fish:>0})
                {
                    saved=true;var clone=World.LoadJson(w.SaveJson());
                    for(int i=0;i<10;i++){w.Tick(.1f);clone.Tick(.1f);}
                    if(w.SaveJson()!=clone.SaveJson())throw new Exception("Loaded fishing trip diverged");
                }
            }
            w.Validate();
            if(first<0 || completion<0 || kind==BuildingKind.FishingDock && !saved)throw new Exception($"No completion/delivery/loaded trip: completion={completion}, first={first}, saved={saved}, caught={w.Food.CaughtFish}");
            var result=new{depleted,kind=kind.ToString(),first,at240,delivered,completion,food=w.ReachableEdibleStored,fishStock=w.Map.FishingGrounds.Sum(g=>g.Available)};
            results.Add(result);Console.WriteLine(JsonSerializer.Serialize(result));
        }
        Directory.CreateDirectory("artifacts/browser/fishing261");
        File.WriteAllText("artifacts/browser/fishing261/food-comparison.json",JsonSerializer.Serialize(results,new JsonSerializerOptions{WriteIndented=true}));
    }
}

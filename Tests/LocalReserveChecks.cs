using Inlanders.Simulation;
using System.Text.Json;
static class LocalReserveChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    static void Continue(World w)
    {
        var copy=World.LoadJson(w.SaveJson());
        for(int i=0;i<40;i++){w.Tick(.1f);copy.Tick(.1f);Check(w.SaveJson()==copy.SaveJson(),"Reserve saved continuation differs");}
        w.Validate();
    }
    public static void Run()
    {
        var original=WorkplaceFoodChecks.PrepareVillage();
        foreach(var site in original.Cottages.Where(original.IsWorkplaceFoodStore))Check(original.SetLocalFoodReserve(site.Id,24),"Reserve setting refused");
        for(int i=0;i<1200;i++)original.Tick(.1f);original.Validate();
        string baseline=original.SaveJson();Directory.CreateDirectory("artifacts/local-reserve");original.SaveFile("artifacts/local-reserve/start.json");
        var results=new List<object>();
        foreach(int reserve in new[]{0,4,24})
        {
            var w=World.LoadJson(baseline);var producers=w.Cottages.Where(w.IsWorkplaceFoodStore).Select(c=>c.Id).ToHashSet();
            foreach(int id in producers)Check(w.SetLocalFoodReserve(id,reserve),"Reserve change refused");
            Check(!w.SetLocalFoodReserve(-999,4) && !w.SetLocalFoodReserve(producers.First(),25),"Invalid reserve accepted");
            Continue(w);int shipments=0,portions=0,localMeals=0,otherMeals=0;double hunger=0;var meals=new HashSet<int>();
            var previous=new HashSet<int>();string? active=null;
            for(int i=0;i<3000;i++)
            {
                w.Tick(.1f);
                var current=w.People.Where(p=>p.Task==Work.ToFoodPickup && p.FoodSourceId is int id && producers.Contains(id)).Select(p=>p.Id).ToHashSet();
                foreach(int id in current.Except(previous)){var p=w.People.Single(p=>p.Id==id);shipments++;portions+=p.PantryReserved;active??=w.SaveJson();}
                previous=current;
                foreach(var p in w.People.Where(p=>p.Task==Work.EatingMeal && p.Meal!=null))if(meals.Add(p.Meal!.Id))
                    {if(p.Meal.SourceId is int source && producers.Contains(source))localMeals++;else otherMeals++;}
                hunger+=w.Food.Hunger*.1;
                if(i%100==0)w.Validate();
            }
            if(reserve==24)Check(shipments==0,"Full retention allowed new producer shipments");
            if(reserve==0)Check(shipments>0 && portions>0,"Release policy caused no physical distribution");
            Check(localMeals+otherMeals>0,"No meals during policy comparison");
            if(active!=null)
            {
                var claim=World.LoadJson(active);var p=claim.People.First(p=>p.Task==Work.ToFoodPickup && p.FoodSourceId is int id && producers.Contains(id));int source=p.FoodSourceId!.Value,amount=p.PantryReserved;
                Check(claim.SetLocalFoodReserve(source,24) && p.PantryReserved==amount && p.FoodSourceId==source,"Raising retention cancelled an existing shipment");Continue(claim);
                var removal=World.LoadJson(active);Check(removal.RequestDemolition(source),"Producer demolition refused");
                for(int i=0;i<6000 && removal.Cottages.Any(c=>c.Id==source);i++){removal.Tick(.1f);if(i%50==0)removal.Validate();}
                if(removal.Cottages.Any(c=>c.Id==source))removal.SaveFile($"artifacts/local-reserve/removal-stalled-{reserve}.json");
                Check(removal.Cottages.All(c=>c.Id!=source),"Producer demolition did not complete within 600s; inspect saved jobs before attributing to reservations");removal.Validate();
            }
            Continue(w);w.SaveFile($"artifacts/local-reserve/end-{reserve}.json");
            results.Add(new{reserve,shipments,portions,localMeals,otherMeals,hungryPersonSeconds=hunger,stored=w.EdibleStored});
            Console.WriteLine($"PASS reserve {reserve}: {shipments} new shipments/{portions} portions; {localMeals} local/{otherMeals} other meals; {hunger:F1} hungry person-seconds; active save and removal valid.");
        }
        File.WriteAllText("artifacts/local-reserve/comparison.json",JsonSerializer.Serialize(results,new JsonSerializerOptions{WriteIndented=true}));
    }
}

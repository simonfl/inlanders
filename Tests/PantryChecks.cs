using Inlanders.Simulation;

public static class PantryChecks
{
    static void Check(bool ok,string why) {if(!ok) throw new Exception(why);}
    static void Step(World w,int n=1) {for(int i=0;i<n;i++) {w.Tick(.1f); w.Validate(); Check(w.DeliveredBerries==0,"Stored/transferred initial food changed campaign delivery progress");}}
    static void Until(World w,Func<bool> ready,string why)
    { for(int i=0;i<6000 && !ready();i++) Step(w); Check(ready(),why+" "+string.Join("; ",w.People.Select(p=>p.Status))); }
    static void Roundtrip(World w)
    {
        var saved=w.SaveJson(); var copy=World.LoadJson(saved); Check(copy.SaveJson()==saved,"Pantry save changed");
        var twin=World.LoadJson(saved); Step(copy,100); Step(twin,100); Check(copy.SaveJson()==twin.SaveJson(),"Pantry continuation diverged");
    }
    public static void Run()
    {
        foreach(var kind in World.EdibleKinds) CheckFood(kind);
    }
    static void CheckFood(Resource kind)
    {
        var w=new World(40); w.Food.InitialBerries=w.Food.Berries=0;
        switch(kind)
        {
            case Resource.Berries: w.Food.InitialBerries=w.Food.Berries=1000; break;
            case Resource.Vegetables: w.Food.GrownVegetables=w.Food.Vegetables=1000; break;
            case Resource.Bread: w.Food.BakedBread=w.Food.Bread=1000; w.Food.GrownGrain=w.Food.UsedGrain=500; break;
            case Resource.Fish: w.Food.CaughtFish=w.Food.Fish=1000; break;
            case Resource.Game: w.Food.HuntedGame=w.Food.Game=1000; break;
            case Resource.Fruit: w.Food.GrownFruit=w.Food.Fruit=1000; break;
        }
        int Eaten()=>w.Food.EatenBerries+w.Food.EatenVegetables+w.Food.EatenBread+w.Food.EatenFish+w.Food.EatenGame+w.Food.EatenFruit;
        var pantry=w.Place(new(3,0),false,BuildingKind.Pantry)!;
        Until(w,()=>pantry.Complete,"Pantry not built");
        Check(pantry.Delivered==6,"Pantry construction cost wrong");
        foreach(var p in w.People) w.Assign(p.Id,Role.Unassigned);
        w.Assign(0,Role.Hauler);
        Until(w,()=>w.People[0].Task==Work.ToFoodPickup,"No pantry supply pickup"); Roundtrip(w);
        Check(w.FoodIncoming(pantry.Id)>0 && w.FoodAt(pantry.Id,kind)==0,"Incoming food credited before arrival");
        Until(w,()=>w.People[0].Task==Work.ToPantry,"No physical supply cargo"); Roundtrip(w);
        Until(w,()=>pantry.PantryFood.Sum()>0,"Pantry never received food");
        Check(w.ReadFoodFlow().Delivered==0,"Transferred food counted as new production");
        Until(w,()=>w.People.Any(p=>p.Meal is {SourceId:not null,Reserved:true}),"No resident chose stocked local pantry"); Roundtrip(w);
        Until(w,()=>w.People.Any(p=>p.Meal is {SourceId:not null,Carrying:true}),"Local pantry meal not collected"); Roundtrip(w);
        int eaten=Eaten();
        Until(w,()=>Eaten()>eaten,"Local meal not eaten");
        w.SetPantryTarget(pantry.Id,0);
        Until(w,()=>w.People[0].Task==Work.ToFoodPickup && w.People[0].FoodSourceId==pantry.Id,"No return of surplus pantry food"); Roundtrip(w);
        Until(w,()=>pantry.PantryFood.Sum()==0 && w.FoodIncoming(pantry.Id)==0,"Target zero did not drain pantry");
        Check(w.ReadFoodFlow().Delivered==0,"Returning surplus counted as fresh food");
        w.SetPantryTarget(pantry.Id,24); Step(w,200);
        Check(pantry.PantryFood.Sum()+w.FoodIncoming(pantry.Id)<=24,"Pantry overfilled");
        Check(w.RequestDemolition(pantry.Id),"Pantry demolition rejected"); Roundtrip(w);
        Check(w.People.All(p=>p.FoodDestinationId!=pantry.Id && !(p.Meal is {Reserved:true} r && r.SourceId==pantry.Id)),"Closing pantry retained incoming/meal claims");
        w.Assign(1,Role.Builder);
        Until(w,()=>!w.Cottages.Contains(pantry),"Pantry goods not recovered");
        Check(w.ReadFoodFlow().Delivered==0,"Recovered stored food counted as new production");
        Console.WriteLine($"PASS: {kind} pantry construction, physical supply/meals, capacity, exact phase saves, closure, recovery and no transfer inflation.");
    }
}

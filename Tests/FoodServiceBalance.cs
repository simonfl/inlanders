using Inlanders.Simulation;

public static class FoodServiceBalance
{
    public static void Run()
    {
        foreach(int variant in new[]{0,1,2})
        {
            var w=FoodChecks.Scenario();
            var pantry=variant==0?null:w.Place(new(6,-6),false,BuildingKind.Pantry) ?? throw new Exception("Balance pantry rejected");
            float travel=0,eating=0,rest=0,hunger=0; float? built=null,supper=null; bool hauler=false;
            var roles=new Dictionary<Role,float>();
            for(int i=0;i<6000;i++)
            {
                if(w.Cottages.All(c=>c.Complete) && built==null) built=w.Food.Time;
                if(variant==2 && built!=null && !hauler) {w.Assign(3,Role.Hauler); w.SetPantryTarget(pantry!.Id,0); hauler=true;}
                if(w.CanCelebrate && supper==null) supper=w.Food.Time;
                foreach(var p in w.People)
                {
                    if(p.Task is Work.ToMealSupply or Work.ToMealSeat or Work.ReturnMeal)
                    {travel+=.1f; roles[p.Role]=roles.GetValueOrDefault(p.Role)+.1f;}
                    if(p.Task==Work.EatingMeal) eating+=.1f;
                    if(p.Task is Work.ToRest or Work.Resting or Work.ToLeisure or Work.Leisure) rest+=.1f;
                }
                hunger+=w.Food.Hunger*.1f; w.Tick(.1f); w.Validate();
            }
            Console.WriteLine($"FOOD SERVICE variant {variant}: built={built:0.0}s supper={supper:0.0}s, produced berries={w.Food.GatheredBerries} bread={w.Food.BakedBread}, stored bread central={w.Food.Bread} local={w.StoredFood(Resource.Bread)-w.Food.Bread}, meal travel={travel:0.0}s eating={eating:0.0}s rest/recreation={rest:0.0}s hunger-integral={hunger:0.0}s, timely={w.Food.MealOutcomes.Count(m=>m.Timely)} missed={w.Food.MealOutcomes.Count(m=>!m.Timely)}, travel by role [{string.Join(';',roles.Select(k=>$"{k.Key}:{k.Value:0.0}s"))}]");
        }
    }
}

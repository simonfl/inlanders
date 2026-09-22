using Inlanders.Simulation;
static class OvenWorkyardChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    public static void Run()
    {
        Directory.CreateDirectory("artifacts/oven-workyard");
        foreach(bool relaxed in new[]{false,true})
        {
            var w=new HamletProfile(relaxed,true,false,true).Create();
            var field=w.Place(new(-3,-5),0,BuildingKind.Farm);
            var oven=w.Place(new(-1,-3),0,BuildingKind.Bakery);
            Check(field!=null && oven!=null,"Grain/oven investment unavailable");
            var seen=new HashSet<string>();
            void Mark(string phase,bool condition){if(condition && seen.Add(phase))w.SaveFile($"artifacts/oven-workyard/{relaxed}-{phase}.json");}
            for(int i=0;i<12000;i++)
            {
                w.Tick(.1f);if(i%100==0)w.Validate();
                Mark("grain",oven!.InputGrain>0);
                Mark("baking",w.People.Any(p=>p.WorkplaceId==oven.Id && p.Task==Work.Baking));
                Mark("batch",oven.OutputBread>0);
                Mark("stored",oven.PantryFood.Sum()>0);
                Mark("meal",w.People.Any(p=>p.Meal is {Carrying:true,Kind:Resource.Bread} && p.Meal.SourceId==oven.Id));
                if(seen.Count==5)break;
            }
            Check(seen.Count==5 && field!.Complete && oven!.Complete,"Grain/bake/bread/meal sequence incomplete");
            var copy=World.LoadJson(w.SaveJson());for(int i=0;i<100;i++){w.Tick(.1f);copy.Tick(.1f);}Check(copy.SaveJson()==w.SaveJson(),"Oven continuation differs");
        }
        Console.WriteLine("PASS: same-inlet paid/free grain/oven investment, real input/baking/batch/storage/meal sequence and exact continuation.");
    }
}

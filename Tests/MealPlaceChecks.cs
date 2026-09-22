using Inlanders.Simulation;
static class MealPlaceChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    public static void Run()
    {
        Directory.CreateDirectory("artifacts/meal-place");
        foreach(bool relaxed in new[]{false,true})
        {
            var w=World.NewTransformationHamlet(relaxed,true);
            var h=w.Cottages.Single(c=>c.Cell==new Cell(-3,6));
            Check(w.FurnishHomeYard(h.Id,0),"Home order failed");
            for(int i=0;i<6000 && !h.Improved;i++)w.Tick(.1f);
            Check(h.Improved && w.SetCommons(new(3,9)),"Meal alternatives unavailable");
            bool home=false,shared=false;
            for(int i=0;i<6000;i++)
            {
                w.Tick(.1f);if(i%100==0)w.Validate();
                if(!home && w.People.Any(p=>p.Task==Work.EatingMeal && w.AtFurnishedHome(p)))
                {home=true;w.SaveFile($"artifacts/meal-place/{relaxed}-home.json");}
                shared|=w.People.Any(p=>p.Task==Work.EatingMeal && p.Meal?.Commons==true);
                if(home && shared)break;
            }
            Check(home && shared,$"Shared ground still displaced every home meal, or shared meals disappeared: home={home}, shared={shared}");
            var copy=World.LoadJson(w.SaveJson());for(int i=0;i<100;i++){w.Tick(.1f);copy.Tick(.1f);}
            Check(w.SaveJson()==copy.SaveJson(),"Meal choice continuation differs");
            Check(w.RemoveCommons(),"Removal failed");w.Validate();
        }
        Console.WriteLine("PASS: home and shared meals coexist through real furnishing and supplies in both modes; active saves and removal remain correct.");
    }
}


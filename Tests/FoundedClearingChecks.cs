using Inlanders.Simulation;
static class FoundedClearingChecks
{
    public static void Run()
    {
        foreach(bool relaxed in new[]{false,true})
        {
            var ready=World.NewWorkingClearing(relaxed);var w=World.NewWorkingClearing(relaxed,founded:true);
            void Check(bool ok,string why){if(!ok)throw new Exception(why);}
            Check(w.Population==ready.Population && w.EdibleStored==ready.EdibleStored && w.InitialLogs==ready.InitialLogs && w.Stored==ready.Stored+ready.Cottages.Sum(c=>c.Delivered),"Unmatched starting resources");
            Check(w.Map.Land.SequenceEqual(ready.Map.Land) && w.Cottages.Count==0 && w.PublicPlace!.PlayerFounded,"Not the same open land");
            Check(w.PublicPlace!.Create().SaveJson()==w.SaveJson(),"Restart differs");
            foreach(var site in ready.Cottages)Check(w.Place(site.Cell,site.Rotation,site.Kind,site.PlotRows)!=null,"Original layout cannot be authored");
            for(int i=0;i<6000;i++)w.Tick(.1f);
            Check(w.Housed==8 && w.FoundingHasNewFood && w.Cottages.All(c=>c.Complete),"Actual founding did not establish homes and food");
            var copy=World.LoadJson(w.SaveJson());for(int i=0;i<600;i++){w.Tick(.1f);copy.Tick(.1f);}w.Validate();Check(w.SaveJson()==copy.SaveJson(),"Continuation differs");
            Console.WriteLine($"PASS matched founded clearing {relaxed}: equal resources/land, actual construction/food and exact continuation.");
        }
    }
}

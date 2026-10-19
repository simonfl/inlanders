using Inlanders.Simulation;
static class CropOrderChecks
{
    public static void Run()
    {
        foreach(bool relaxed in new[]{false,true})
        {
            var w=World.NewRiverFrontage(relaxed,true,true);var field=w.Cottages.First(c=>c.Kind==BuildingKind.VegetableField);
            if(!w.SetHarvestThenPause(field.Id,true))throw new Exception("Crop order refused");
            var copy=World.LoadJson(w.SaveJson());
            for(int i=0;i<3000 && !field.WorkPaused;i++){w.Tick(.1f);copy.Tick(.1f);}
            if(!field.WorkPaused || field.Harvest>0 || field.Planted || w.SaveJson()!=copy.SaveJson())throw new Exception("Crop order lost output/replanted/save differs");
            for(int i=0;i<100;i++)w.Tick(.1f);
            if(field.Planted || w.StoredFood(Resource.Vegetables)==0)throw new Exception("Paused crop restarted or delivery lost");
            if(!w.SetHarvestThenPause(field.Id,false) || field.WorkPaused || field.HarvestThenPause)throw new Exception("Resume failed");
            w.Validate();Console.WriteLine($"PASS crop completion, pause, actual delivery, resume and exact save relaxed={relaxed}");
        }
    }
}

using Inlanders.Simulation;
static class BatchBreadChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    public static void Run()
    {
        var w=World.NewWorkingVillage();var oven=w.Cottages.Single(c=>c.Kind==BuildingKind.Bakery);
        for(int i=0;i<5000 && oven.OutputBread!=8;i++)w.Tick(.1f);
        Check(oven.OutputBread==8 && w.Food.BakedBread==w.Food.UsedGrain*4,"Normal batch recipe incorrect");
        Directory.CreateDirectory("artifacts/batch-bread");w.SaveFile("artifacts/batch-bread/full-oven.json");
        var copy=World.LoadJson(w.SaveJson());for(int i=0;i<100;i++){w.Tick(.1f);copy.Tick(.1f);}Check(copy.SaveJson()==w.SaveJson(),"Batch continuation differs");
        Check(w.SetWorkplacePaused(oven.Id,true),"Pause rejected");int bread=oven.OutputBread;
        var at=w.Map.Land.First(c=>w.RelocationProblem(oven.Id,c,0)==null);
        Check(w.MoveBuilding(oven.Id,at,0) && oven.OutputBread==bread,"Move lost bread");w.Validate();
        Check(new World().BreadPerGrain==2,"Archived recipe changed");
        Console.WriteLine("PASS: full eight-portion batch, recipe accounting, exact active save, relocation and archived recipe.");
    }
}

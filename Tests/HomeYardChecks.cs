using Inlanders.Simulation;
static class HomeYardChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    public static void Run()
    {
        Directory.CreateDirectory("artifacts/home-yards");var w=World.NewWorkingVillage();
        Check(w.Place(new(7,8),1,BuildingKind.FishingDock)!=null,"Dock rejected");
        foreach(var kind in new[]{BuildingKind.Sawmill,BuildingKind.Carpenter}){var p=ReviewPlacement.Find(w,kind,new(-3,7),(c,r)=>true,"furnishing")!;w.Place(p.Actual,p.Rotation,kind);}
        foreach(var h in w.Cottages.Where(c=>c.Kind==BuildingKind.Cottage))Check(w.RequestImprovement(h.Id),"Order rejected: "+w.ImprovementProblem(h.Id));
        bool quiet=false,meal=false;int furnished=0;
        for(int i=0;i<18000;i++)
        {
            w.Tick(.1f);if(i%200==0)w.Validate();
            furnished=w.Cottages.Count(c=>c.Improved);
            if(!quiet && w.People.Any(w.QuietAtFurnishedHome)) {quiet=true;w.SaveFile("artifacts/home-yards/quiet.json");}
            if(!meal && w.People.Any(p=>p.Task==Work.EatingMeal && w.AtFurnishedHome(p))){meal=true;w.SaveFile("artifacts/home-yards/meal.json");}
            if(furnished==4 && quiet && meal)break;
        }
        Check(furnished==4 && quiet && meal,"Furnishing did not produce domestic use");
        var copy=World.LoadJson(w.SaveJson());for(int i=0;i<100;i++){w.Tick(.1f);copy.Tick(.1f);}Check(copy.SaveJson()==w.SaveJson(),"Domestic continuation differs");
        for(int i=0;i<2400;i++)w.Tick(.1f);w.SaveFile("artifacts/home-yards/inhabited.json");
        var home=w.Cottages.First(c=>c.Improved);var before=w.HomeYardPlaces(home);
        var at=w.Map.Land.First(c=>w.RelocationProblem(home.Id,c,1)==null);Check(w.MoveBuilding(home.Id,at,1),"Furnished move failed");
        Check(!before.SequenceEqual(w.HomeYardPlaces(home)),"Yard did not follow rotated home");w.Validate();
        Console.WriteLine($"PASS: {furnished} ordinarily furnished homes, quiet mending, home meals, exact saves and rotated relocation at {w.Food.Time:0}s.");
    }
}

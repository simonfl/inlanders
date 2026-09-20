using Inlanders.Simulation;
static class RelaxedHamletChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    public static void Run()
    {
        var normal=World.NewTransformationHamlet();var relaxed=World.RelaxedHamletFrom(normal);
        Check(relaxed.Creative && relaxed.SimulatesMeals && relaxed.ProvisionedLife && relaxed.BreadPerGrain==normal.BreadPerGrain,"Rules diverged");
        for(int i=0;i<1200;i++){normal.Tick(.1f);relaxed.Tick(.1f);}
        Check(normal.Food.EatenBerries==relaxed.Food.EatenBerries && normal.People.Select(p=>p.Position).SequenceEqual(relaxed.People.Select(p=>p.Position)),"Unedited resident life diverged before shortage");
        var plan=ReviewPlacement.Find(relaxed,BuildingKind.Bakery,new(4,3),(c,r)=>true,"relaxed")!;
        int timber=relaxed.Stored;var bakery=relaxed.Place(plan.Actual,plan.Rotation,BuildingKind.Bakery)!;
        Check(bakery.Complete && relaxed.Stored==timber,"Relaxed building charged timber");
        var home=relaxed.Cottages.First(c=>c.Kind==BuildingKind.Cottage);Check(relaxed.RequestImprovement(home.Id) && home.Improved,"Relaxed furnishing absent");
        var copy=World.LoadJson(relaxed.SaveJson());for(int i=0;i<500;i++){relaxed.Tick(.1f);copy.Tick(.1f);}Check(copy.SaveJson()==relaxed.SaveJson(),"Relaxed continuation differs");
        foreach(var c in relaxed.Cottages.Where(c=>World.ProductionOutput(c.Kind)!=null))relaxed.SetWorkplacePaused(c.Id,true);
        for(int i=0;i<6000;i++)relaxed.Tick(.1f);
        Check(relaxed.Food.Hunger==0 && relaxed.People.Any(p=>!p.Fed),"Relaxed hunger penalties not disabled or no real missing meals");
        relaxed.Validate();Directory.CreateDirectory("artifacts/transformation");relaxed.SaveFile("artifacts/transformation/relaxed-shortage.json");
        Console.WriteLine("PASS: identical initial daily life, current recipe, free construction/furnishing, actual shortage without penalties and exact current saves.");
    }
}

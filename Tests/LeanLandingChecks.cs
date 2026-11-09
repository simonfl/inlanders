using Inlanders.Simulation;

static class LeanLandingChecks
{
    public static void Run()
    {
        Directory.CreateDirectory("artifacts/lean-landing");
        foreach (var route in new[] { (BuildingKind.FishingDock, 0), (BuildingKind.ForagerHut, 0),
            (BuildingKind.VegetableGarden, 0), (BuildingKind.FishingDock, 300) })
        {
            var (kind, delay) = route;
            var w = World.NewFirstHome(true);
            if(w.PublicPlace?.SaveName != "lean-landing.json" || w.PublicPlace.Create().Map.Name != w.Map.Name ||
                w.ReadFirstHomeObjectives().Action != "food" || w.Food.Berries != 12 || w.Stored != 10)
                throw new Exception("Landing identity or opening pressure differs");
            w.SaveFile("artifacts/lean-landing/start.json");
            for(int i=0;i<delay*10;i++)w.Tick(.1f);
            if(delay>0)
            {
                if(w.People.All(p=>p.Fed) || w.ReachableEdibleStored != 0 || w.Food.WorkEfficiency >= 1)
                    throw new Exception("Ignoring provisions has no consequence");
                w.SaveFile("artifacts/lean-landing/shortage.json");
            }
            Cell target = kind == BuildingKind.FishingDock ? new(8,4) : kind == BuildingKind.ForagerHut ? new(-7,-5) : new(0,3);
            var food = ReviewPlacement.Find(w,kind,target,(c,r)=>true,"landing livelihood")!;
            var site=w.Place(food.Actual,food.Rotation,kind)!;
            int hungryPeak=0; float first=0;
            for(int i=0;i<5000 && w.WorkplaceFoodDelivered(site.Id)==0;i++)
            {w.Tick(.1f);hungryPeak=Math.Max(hungryPeak,w.People.Count(p=>!p.Fed));}
            first=w.Food.Time;
            if(w.WorkplaceFoodDelivered(site.Id)==0)throw new Exception("No first delivery: "+kind);
            if(w.ReadFirstHomeObjectives().Action != "home")throw new Exception("Food-first guidance did not advance to housing");
            var home=ReviewPlacement.Find(w,BuildingKind.Cottage,new(1,6),(c,r)=>true,"landing home")!;
            w.Place(home.Actual,home.Rotation,BuildingKind.Cottage);
            w.ApplyWoodlandCare(new(-10,-9),new(-2,-4),WoodlandIntent.Renew,false);
            for(int i=0;i<9000 && (!w.Founding!.FirstHome!.Complete || w.People.Any(p=>!p.Fed));i++)
            {w.Tick(.1f);hungryPeak=Math.Max(hungryPeak,w.People.Count(p=>!p.Fed));}
            if(!w.Founding!.FirstHome!.Complete || w.People.Any(p=>!p.Fed))throw new Exception("Landing cannot finish/recover: "+kind);
            w.Validate();
            var copy=World.LoadJson(w.SaveJson());
            for(int i=0;i<50;i++){w.Tick(.1f);copy.Tick(.1f);}
            if(w.SaveJson()!=copy.SaveJson())throw new Exception("Landing continuation differs");
            w.SaveFile($"artifacts/lean-landing/{kind}-{delay}-complete.json");
            Console.WriteLine($"PASS {kind}, delay {delay}s: first food {first:0}s, recovered finish {w.Food.Time:0}s, peak hungry {hungryPeak}/4, exact save");
        }
    }
}

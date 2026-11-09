using Inlanders.Simulation;

static class RiverClosingChecks
{
    public static void Run()
    {
        Directory.CreateDirectory("artifacts/river-closing");
        foreach(var route in new[]{(false,false),(true,false),(true,true)})
        {
            var (expand,early)=route;
            var w=World.NewRiverClosing();w.SaveFile("artifacts/river-closing/start.json");
            w.Place(World.RecoveryCrossing,0,BuildingKind.Bridge);
            if(!w.ApplyWoodlandCare(new(-11,5),new(6,12),WoodlandIntent.Renew,false))throw new Exception("Grove order rejected");
            void Add(BuildingKind kind,Cell target)
            {var p=ReviewPlacement.Find(w,kind,target,(c,r)=>c.Z>0,"river preparation") ?? throw new Exception("No room for "+kind);w.Place(p.Actual,p.Rotation,kind);}
            bool stage1=false,stage2=false,saved=false,trialSaved=false;
            for(int i=0;i<(expand?24000:15600) && !w.Founding!.FirstHome!.Complete;i++)
            {
                if(expand && !stage1 && w.Food.Time>=(early?0:180))
                {stage1=true;Add(BuildingKind.Cottage,new(1,6));Add(BuildingKind.Cottage,new(5,6));Add(BuildingKind.VegetableField,new(-6,5));Add(BuildingKind.VegetableGarden,new(2,10));}
                if(expand && !stage2 && w.Food.Time>=(early?0:600))
                {stage2=true;Add(BuildingKind.Cottage,new(6,2));Add(BuildingKind.Cottage,new(-8,2));Add(BuildingKind.VegetableField,new(4,10));Add(BuildingKind.VegetableGarden,new(-6,10));}
                if(early)w.AdvanceRiverSchedule();
                w.Tick(.1f);
                if(!trialSaved && w.Founding.FirstHome.RiverClosing!.StableSeconds>=120)
                {trialSaved=true;var clone=World.LoadJson(w.SaveJson());for(int j=0;j<50;j++){w.Tick(.1f);clone.Tick(.1f);}if(w.SaveJson()!=clone.SaveJson())throw new Exception("Trial save differs");}
                if(!saved && w.Food.Time>=970)
                {saved=true;w.SaveFile($"artifacts/river-closing/{expand}-arrived.json");var clone=World.LoadJson(w.SaveJson());for(int j=0;j<50;j++){w.Tick(.1f);clone.Tick(.1f);}if(w.SaveJson()!=clone.SaveJson())throw new Exception("Arrival save differs");}
            }
            var river=w.Founding!.FirstHome!.RiverClosing!;
            if(w.Population!=12 || river.Boats!=2 || !river.Closed || w.Food.InitialBerries!=32)throw new Exception("Boat schedule or supply conservation wrong");
            w.SaveFile($"artifacts/river-closing/{expand}-diagnostic.json");
            if(expand && (!river.Complete || !early && w.Food.Time<1680))throw new Exception($"Expanded village not proven: {w.Food.Time}, {w.RiverStabilityProblem()}");
            if(!expand && river.Complete)throw new Exception("Opening-only solution won the longer scenario");
            w.Validate();w.SaveFile($"artifacts/river-closing/{expand}-{early}-end.json");
            Console.WriteLine($"PASS expand={expand} early={early} time={w.Food.Time:0}s population={w.Population} beds={w.Beds} complete={river.Complete} stability={river.StableSeconds:0}s reason={w.RiverStabilityProblem()}");
        }
    }
}

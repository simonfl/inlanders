using Inlanders.Simulation;
static class DirectDomesticChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    static void Until(World w,Func<bool> done,string why,int limit=18000)
    {for(int i=0;i<limit && !done();i++){w.Tick(.1f);w.Validate();}Check(done(),why);}
    public static void Run()
    {
        Directory.CreateDirectory("artifacts/direct-domestic");
        foreach(bool bank in new[]{false,true})
        {
            var w=World.NewTransformationHamlet(false,bank);
            var placement=ReviewPlacement.Find(w,BuildingKind.Sawmill,new(-5,5),(c,r)=>true,"domestic planks")!;
            Check(w.Place(placement.Actual,placement.Rotation,BuildingKind.Sawmill)!=null,"Sawmill rejected");
            var home=w.Cottages.First(c=>c.Kind==BuildingKind.Cottage);
            Check(w.RequestImprovement(home.Id),"Direct furnishing rejected");
            var phases=new Dictionary<Work,string>();
            Until(w,()=>{var p=w.People.FirstOrDefault(p=>p.ComfortHomeId==home.Id);if(p!=null){Check(p.WorkplaceId==null,"Direct job acquired a workshop");phases.TryAdd(p.Task,w.SaveJson());}return home.Improved;},"Direct furnishing stalled");
            Check(!w.Cottages.Any(c=>c.Kind==BuildingKind.Carpenter) && home.ImprovementPlanks==4,"Hidden workshop or free planks");
            foreach(var phase in new[]{Work.ToComfortPlanks,Work.ToComfortHome,Work.ToComfortInstall,Work.InstallingComfort})
            {
                Check(phases.ContainsKey(phase),"Missing phase "+phase);
                var continued=World.LoadJson(phases[phase]);
                Until(continued,()=>continued.Cottages.Single(c=>c.Id==home.Id).Improved,"Uncancelled reload stalled "+phase);
                Check(continued.SaveJson()==w.SaveJson(),"Active phase continuation diverged "+phase);
                var copy=World.LoadJson(phases[phase]);Check(copy.SaveJson()==phases[phase],"Phase save changed");
                Check(copy.CancelImprovement(home.Id),"Cannot cancel "+phase);
                var cancelled=copy.Cottages.Single(c=>c.Id==home.Id);
                if(cancelled.ImprovementPlanks>0)Check(copy.ComfortSummary(cancelled).Contains("recovering"),"Recovery feedback missing");
                Until(copy,()=>copy.Cottages.Single(c=>c.Id==home.Id).ImprovementPlanks==0 && !copy.People.Any(p=>p.ComfortHomeId==home.Id || p.Cargo==Resource.Planks && p.Carried>0),"Cancellation stranded planks");
                Check(copy.RequestImprovement(home.Id),"Cannot reorder");
                Until(copy,()=>copy.Cottages.Single(c=>c.Id==home.Id).Improved,"Reorder stalled");
            }
            var moved=World.LoadJson(phases[Work.InstallingComfort]);var moving=moved.Cottages.Single(c=>c.Id==home.Id);
            var destination=moved.Map.Land.First(c=>c!=moving.Cell && moved.RelocationProblem(home.Id,c,1)==null);
            Check(moved.MoveBuilding(home.Id,destination,1),"Active domestic relocation rejected");moved.Validate();
            Until(moved,()=>moving.Improved,"Moved installation stalled");
            var removed=World.LoadJson(phases[Work.ToComfortHome]);Check(removed.RequestDemolition(home.Id),"In-transit domestic demolition rejected");removed.Validate();
            Until(removed,()=>!removed.Cottages.Any(c=>c.Id==home.Id),"Domestic demolition stalled");
            Until(w,()=>w.People.Any(w.QuietAtFurnishedHome),"No lived domestic use");
            w.SaveFile($"artifacts/direct-domestic/{(bank?"bank":"compact")}.json");
            var saved=World.LoadJson(w.SaveJson());for(int i=0;i<100;i++){w.Tick(.1f);saved.Tick(.1f);}Check(saved.SaveJson()==w.SaveJson(),"Direct job continuation diverged");
            var relaxed=World.NewTransformationHamlet(true,bank);var rh=relaxed.Cottages.First(c=>c.Kind==BuildingKind.Cottage);
            Check(relaxed.RequestImprovement(rh.Id) && rh.Improved && rh.ImprovementPlanks==0,"Relaxed furnishing changed");relaxed.Validate();
        }
        Console.WriteLine("PASS: direct shared domestic work in both layouts, four physical phases, cancel/recovery/reorder, exact saves, occupied yard use and relaxed furnishing.");
    }
}

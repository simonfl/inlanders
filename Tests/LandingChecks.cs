using Inlanders.Simulation;
static class LandingChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    public static void Run()
    {
        Directory.CreateDirectory("artifacts/landing");
        foreach(bool relaxed in new[]{false,true})
        {
            var w=new HamletProfile(relaxed,true,false,true).Create();
            var at=new Cell(8,8);var dock=w.Place(at,1,BuildingKind.FishingDock);
            Check(dock!=null,"Inlet landing rejected: "+w.PlacementProblem(at,1,BuildingKind.FishingDock));
            Check(World.Footprint(at,1,BuildingKind.FishingDock).Count()==3,"Landing has no real shore area");
            foreach(var cell in World.Footprint(at,1,BuildingKind.FishingDock))Check(w.PlacementProblem(cell,0,BuildingKind.SeatingGarden)!=null,"Landing bay permits overlap");
            bool stored=false,carried=false;var phases=new HashSet<BoatPhase>();
            for(int i=0;i<9000;i++)
            {
                w.Tick(.1f);if(i%100==0)w.Validate();
                if(dock!.Boat is {} boat && boat.FisherId!=null && phases.Add(boat.Phase))w.SaveFile($"artifacts/landing/{relaxed}-{boat.Phase}.json");
                stored|=dock.PantryFood.Sum()>0;
                carried|=w.People.Any(p=>p.Cargo==Resource.Fish && p.Carried>0);
                if(stored && carried && phases.Contains(BoatPhase.Returning))break;
            }
            Check(dock!.Complete && stored && carried,"Landing did not support actual catch and onward food");
            w.SaveFile($"artifacts/landing/{relaxed}-working.json");
            var copy=World.LoadJson(w.SaveJson());for(int i=0;i<100;i++){w.Tick(.1f);copy.Tick(.1f);}Check(w.SaveJson()==copy.SaveJson(),"Landing continuation differs");
            Check(w.SetWorkplacePaused(dock.Id,true),"Pause failed");
            for(int i=0;i<1500 && dock.Boat?.FisherId!=null;i++)w.Tick(.1f);
            Check(dock.Boat?.FisherId==null && w.RelocationProblem(dock.Id)==null,"Fisher failed to return for rearrangement");w.Validate();
        }
        Console.WriteLine("PASS: real three-cell landing, overlap protection, construction, boat/catch/onward food, exact saves and safe return in both modes.");
    }
}

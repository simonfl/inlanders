using Inlanders.Simulation;
static class GardenRelocationChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    public static void Run()
    {
        foreach(bool relaxed in new[]{false,true})foreach(bool ripe in new[]{false,true})
        {
            var w=World.NewTransformationHamlet(relaxed);var garden=w.Cottages.Single(c=>c.Cell==new Cell(1,3));
            if(ripe){for(int i=0;i<1200 && garden.Harvest==0;i++)w.Tick(.1f);Check(garden.Harvest>0,"No ripe crop to retain");}
            Check(w.RelocationProblem(garden.Id)!=null,"Working garden moved without pausing");
            Check(w.SetWorkplacePaused(garden.Id,true),"Garden pause rejected");
            int crop=garden.Harvest,food=w.EdibleStored;float growth=garden.Growth;string paused=w.SaveJson();
            Check(w.RelocationProblem(garden.Id,new(0,-9),1)==null && w.SaveJson()==paused,"Destination query mutates garden");
            Check(!w.MoveBuilding(garden.Id,w.Stockpile,0) && w.SaveJson()==paused,"Invalid move consumes crops");
            Check(w.MoveBuilding(garden.Id,garden.Cell,garden.Rotation) && garden.Growth==growth,"No-op replanted crop");
            Check(w.MoveBuilding(garden.Id,new(0,-9),1),"Garden move rejected");
            Check(garden.Harvest==crop && w.EdibleStored==food && garden.WorkPaused,"Move lost goods or resumed work");
            Check(ripe?garden.Growth==1:!garden.Planted && garden.Growth==0,"Crop restart/ripe state wrong");
            Check(w.SetCommons(new(1,3)),"Vacated home ground unusable");w.Validate();
            Check(w.SetWorkplacePaused(garden.Id,false),"Resume rejected");
            w.Assign(0,Role.Farmer);Check(w.SetWorkplaceAssignment(0,garden.Id),"Dedicated farmer refused");
            int grown=w.Food.GrownVegetables;bool worked=false;var copy=World.LoadJson(w.SaveJson());
            for(int i=0;i<1500;i++){w.Tick(.1f);copy.Tick(.1f);worked|=w.People.Any(p=>p.WorkplaceId==garden.Id && p.Task is Work.Planting or Work.Harvesting);if(i%100==0)w.Validate();}
            Check(w.SaveJson()==copy.SaveJson(),"Moved garden active continuation differs");
            Check(worked && w.Food.GrownVegetables>grown,"Resumed garden world ceased producing");
        }
        Console.WriteLine("PASS: normal/relaxed garden moves, growing crop restart, ripe goods conservation, invalid/no-op purity, shared-ground reuse and exact resumed saves.");
    }
}

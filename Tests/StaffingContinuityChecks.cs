using Inlanders.Simulation;
using System.Text.Json;
static class StaffingContinuityChecks
{
    public static void Run()
    {
        foreach(bool relaxed in new[]{false,true})
        {
            var w=World.NewRiverLivelihood(relaxed);
            var dock=w.Place(new(8,5),1,BuildingKind.FishingDock)??throw new Exception("Dock site");
            for(int i=0;i<4000 && dock.Boat?.Phase!=BoatPhase.Fishing;i++)w.Tick(.1f);
            var boat=dock.Boat??throw new Exception("No working boat");
            if(boat.Phase!=BoatPhase.Fishing || boat.FisherId is not int id)throw new Exception("Never fished");
            var worker=w.People[id];string before=JsonSerializer.Serialize(boat,new JsonSerializerOptions{IncludeFields=true});var task=worker.Task;
            if(!w.DedicateWorker(dock.Id) || worker.AssignedWorkplaceId!=dock.Id || worker.SharedWorker)throw new Exception("Did not dedicate actual fisher");
            if(JsonSerializer.Serialize(boat,new JsonSerializerOptions{IncludeFields=true})!=before || worker.Task!=task)throw new Exception("Dedication interrupted the catch");
            w.UseAutomaticVillageWork();
            if(!worker.SharedWorker || worker.AssignedWorkplaceId!=null)throw new Exception("Automatic work rejected");
            if(JsonSerializer.Serialize(boat,new JsonSerializerOptions{IncludeFields=true})!=before || worker.Task!=task)throw new Exception("Release interrupted the catch");
            var copy=World.LoadJson(w.SaveJson());for(int i=0;i<100;i++){w.Tick(.1f);copy.Tick(.1f);}if(w.SaveJson()!=copy.SaveJson())throw new Exception("Saved released worker diverges");
            for(int i=0;i<3000 && w.Food.EatenFish==0;i++)w.Tick(.1f);
            if(w.Food.EatenFish==0)throw new Exception("Preserved trip never fed a resident");w.Validate();
            Console.WriteLine($"PASS staffing relaxed={relaxed}: actual at-sea dedication/release retains boat, claim and work; saved continuation, catch/delivery/meal.");
        }
    }
}

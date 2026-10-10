using Inlanders.Simulation;
static class PreparedAdditionChecks
{
    public static void Run()
    {
        foreach(bool relaxed in new[]{false,true})
        {
            var w=World.NewRiverFrontage(relaxed,true,true);
            var at=w.Map.Land.OrderBy(c=>c.Point.LengthSquared()).First(c=>w.PreviewGroundPreparation(c,0,BuildingKind.SeatingGarden)!=null);
            string before=w.SaveJson();w.PreviewGroundPreparation(at,0,BuildingKind.SeatingGarden);if(before!=w.SaveJson())throw new Exception("Plan preview mutates");
            if(!w.PlanPreparedAddition(at,0,BuildingKind.SeatingGarden))throw new Exception("Plan rejected");
            var copy=World.LoadJson(w.SaveJson());for(int i=0;i<100;i++){w.Tick(.1f);copy.Tick(.1f);}if(w.SaveJson()!=copy.SaveJson())throw new Exception("Prepared addition continuation");
            for(int i=0;i<7000 && !w.People.Any(p=>p.LastLeisureSiteId is int id && w.Cottages.Any(c=>c.Id==id && c.Cell==at));i++)w.Tick(.1f);
            var site=w.Cottages.SingleOrDefault(c=>c.Cell==at && c.Kind==BuildingKind.SeatingGarden);
            if(w.PendingAddition!=null || site?.Complete!=true || !w.People.Any(p=>p.LastLeisureSiteId==site.Id))throw new Exception("Planned addition never became a used place");w.Validate();
            Console.WriteLine($"PASS prepared addition relaxed={relaxed}: pure choice, exact saved preparation, real clearance/construction and completed resident visit.");
        }
        foreach(bool relaxed in new[]{false})foreach(bool resume in new[]{false,true})
        {
            var w=World.NewRiverFrontage(relaxed,true,true);var at=w.Map.Land.OrderBy(c=>c.Point.LengthSquared()).First(c=>w.PreviewGroundPreparation(c,0,BuildingKind.SeatingGarden)!=null);
            w.PlanPreparedAddition(at,0,BuildingKind.SeatingGarden);var cell=w.PendingAddition!.Clearing.First();
            if(!w.ApplyWoodlandCare(cell,cell,WoodlandIntent.Renew) || !w.PreparedAdditionConflict())throw new Exception("New woodland intent not reconciled");
            string changed=w.SaveJson();var copy=World.LoadJson(changed);if(copy.SaveJson()!=changed || !copy.PreparedAdditionConflict())throw new Exception("Saved conflict lost");
            if(!resume)
            {
                w.CancelPreparedAddition();if(!w.ManagedWoodland.Contains(cell) || w.Trees.Single(t=>t.Cell==cell).Preserved || w.Trees.Single(t=>t.Cell==cell).ClearRequested)throw new Exception("Cancel overwrote later renewal");
            }
            else
            {
                if(!w.ResumePreparedAddition())throw new Exception("Explicit clearance could not resume");
                for(int i=0;i<7000 && !w.People.Any(p=>p.LastLeisureSiteId is int id && w.Cottages.Any(c=>c.Id==id && c.Cell==at));i++)w.Tick(.1f);
                var site=w.Cottages.SingleOrDefault(c=>c.Cell==at && c.Kind==BuildingKind.SeatingGarden);
                if(site?.Complete!=true || !w.People.Any(p=>p.LastLeisureSiteId==site.Id))throw new Exception("Reconsidered addition never used");
            }
            w.Validate();
        }
        Console.WriteLine("PASS later woodland renewal survives plan cancellation, explicit renewed preparation reaches actual use, saved conflicts in Normal; Relaxed instant completion tested above.");
        foreach(bool fallen in new[]{false,true})
        {
            var w=World.NewRiverFrontage(false,true,true);var at=w.Map.Land.OrderBy(c=>c.Point.LengthSquared()).First(c=>w.PreviewGroundPreparation(c,0,BuildingKind.SeatingGarden)!=null);
            w.PlanPreparedAddition(at,0,BuildingKind.SeatingGarden);var cells=w.PendingAddition!.Clearing;
            if(fallen)for(int i=0;i<2000 && !w.Trees.Any(t=>cells.Contains(t.Cell) && t.Felled);i++)w.Tick(.1f);
            string blocked=w.SaveJson();if(w.PlanPreparedAddition(at,0,BuildingKind.SeatingGarden) || w.SaveJson()!=blocked)throw new Exception("Second intention overwrites pending");
            var logs=w.Trees.Where(t=>cells.Contains(t.Cell)).Sum(t=>t.Logs);
            if(!w.CancelPreparedAddition() || w.PendingAddition!=null || logs!=w.Trees.Where(t=>cells.Contains(t.Cell)).Sum(t=>t.Logs) || w.Trees.Any(t=>cells.Contains(t.Cell) && (t.ClearRequested || !t.Felled && !t.Preserved)))throw new Exception("Cancellation did not retain physical progress");
            for(int i=0;i<300;i++)w.Tick(.1f);w.Validate();if(w.Cottages.Any(c=>c.Cell==at && c.Kind==BuildingKind.SeatingGarden))throw new Exception("Cancelled plan built anyway");
        }
        Console.WriteLine("PASS prepared addition cancellation before/after felling, no resurrection, no overwritten intention.");
    }
}

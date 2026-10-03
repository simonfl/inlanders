using Inlanders.Simulation;
static class RelocationLivelihoodChecks
{
    public static void Run()
    {
        foreach(bool relaxed in new[]{false,true})foreach(bool field in new[]{false,true})
        {
            var w=World.NewWorkingClearing(relaxed);var site=w.Cottages.First(c=>c.Kind==(field?BuildingKind.VegetableField:BuildingKind.Cottage));
            if(field)w.SetWorkplacePaused(site.Id,true);
            var at=w.Map.Land.OrderBy(c=>(c.Point-new Cell(2,1).Point).LengthSquared()).First(c=>w.RelocationProblem(site.Id,c,site.Rotation)==null);
            string before=w.SaveJson();var proposal=w.ReadRelocationLivelihood(site.Id,at,site.Rotation);
            if(proposal.Route.Length==0 || before!=w.SaveJson())throw new Exception("Move connection missing or mutates live state");
            if(w.ReadRelocationLivelihood(site.Id,new(999,999),0).Route.Length!=0 || before!=w.SaveJson())throw new Exception("Invalid move connection changed world");
            if(!w.MoveBuilding(site.Id,at,site.Rotation))throw new Exception("Previewed move refused");if(field)w.SetWorkplacePaused(site.Id,false);
            for(int i=0;i<9000;i++)w.Tick(.1f);w.Validate();if(w.Food.EatenVegetables==0)throw new Exception("Moved place lost ordinary life");
            var copy=World.LoadJson(w.SaveJson());for(int i=0;i<100;i++){w.Tick(.1f);copy.Tick(.1f);}if(w.SaveJson()!=copy.SaveJson())throw new Exception("Moved connection save differs");
            Console.WriteLine($"PASS relocation connection {relaxed}/{field}: pure valid/invalid proposal, actual move/food and exact continuation");
        }
    }
}


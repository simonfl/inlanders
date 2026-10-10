using Inlanders.Simulation;
static class WoodlandCareChecks
{
    public static void Run()
    {
        foreach(bool relaxed in new[]{false,true})foreach(var intent in Enum.GetValues<WoodlandIntent>())
        {
            var w=World.NewRiverFrontage(relaxed,true,true);var target=w.Trees.OrderBy(t=>t.Cell.Point.LengthSquared()).First();
            string before=w.SaveJson();var plan=w.PreviewWoodlandCare(target.Cell,target.Cell,intent);
            if(plan.Problem!=null || plan.Cells.Length!=1 || w.SaveJson()!=before)throw new Exception("Care preview impure/invalid");
            if(!w.ApplyWoodlandCare(target.Cell,target.Cell,intent))throw new Exception("Care refused");
            if(intent==WoodlandIntent.Renew)
            {
                var at=w.Map.Land.First(c=>w.PlacementProblem(c,0,BuildingKind.SeatingGarden)==null);w.Place(at,0,BuildingKind.SeatingGarden);
            }
            for(int i=0;i<4000;i++)w.Tick(.1f);
            if(intent==WoodlandIntent.Clear && w.Trees.Contains(target))throw new Exception("Clearing did not yield usable ground");
            if(intent==WoodlandIntent.Keep && (target.Felled || !target.Preserved))throw new Exception("Retained tree lost");
            if(intent==WoodlandIntent.Renew && !relaxed && (w.TreesPlanted==0 || !w.ManagedWoodland.Contains(target.Cell)))throw new Exception("Renewal did not complete physical cycle");
            var copy=World.LoadJson(w.SaveJson());for(int i=0;i<100;i++){w.Tick(.1f);copy.Tick(.1f);}if(w.SaveJson()!=copy.SaveJson())throw new Exception("Care continuation differs");w.Validate();
            Console.WriteLine($"PASS whole woodland care {intent} relaxed={relaxed}: real consequence, pure proposal, exact continuation.");
        }
        var region=World.NewRiverLivelihood();var one=region.Trees[0].Cell;var two=region.Trees[3].Cell;string untouched=region.SaveJson();
        var selection=region.PreviewWoodlandCare(one,two,WoodlandIntent.Renew);
        if(selection.Cells.Length<2 || region.SaveJson()!=untouched || !region.ApplyWoodlandCare(one,two,WoodlandIntent.Renew))throw new Exception("Multi-tree proposal failed");
        if(region.Trees.Any(t=>t.Preserved==selection.Cells.Contains(t.Cell)))throw new Exception("Care selected wrong trees");
        var invalid=World.NewRiverLivelihood();string original=invalid.SaveJson();
        if(invalid.ApplyWoodlandCare(new(99,99),new(99,99),WoodlandIntent.Renew) || original!=invalid.SaveJson())throw new Exception("Invalid care mutated land");
    }
}

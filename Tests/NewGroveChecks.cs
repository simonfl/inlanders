using Inlanders.Simulation;
static class NewGroveChecks
{
    public static void Run()
    {
        foreach(bool relaxed in new[]{false,true})
        {
            var w=World.NewRiverFrontage(relaxed,true,true);var first=new Cell(-8,0);var last=new Cell(-4,4);
            string before=w.SaveJson();var plan=w.PreviewWoodlandCare(first,last,WoodlandIntent.Renew);
            var fresh=plan.Cells.Where(c=>!w.Trees.Any(t=>t.Cell==c)).ToArray();
            if(plan.Problem!=null || fresh.Length<3 || w.SaveJson()!=before)throw new Exception($"Grove proposal: {plan.Problem}, {fresh.Length} new spots");
            if(fresh.Any(c=>fresh.Any(o=>o!=c && (o.Point-c.Point).LengthSquared()<4)))throw new Exception("No access between saplings");
            if(!w.ApplyWoodlandCare(first,last,WoodlandIntent.Renew))throw new Exception("Grove apply");
            for(int i=0;i<300;i++)w.Tick(.1f);
            var copy=World.LoadJson(w.SaveJson());for(int i=0;i<100;i++){w.Tick(.1f);copy.Tick(.1f);}if(w.SaveJson()!=copy.SaveJson())throw new Exception("Grove active save");
            for(int i=0;i<5000 && fresh.Any(c=>!w.Trees.Any(t=>t.Cell==c && !t.NeedsPlanting && t.Growth>=1));i++)w.Tick(.1f);
            if(fresh.Any(c=>!w.Trees.Any(t=>t.Cell==c && !t.NeedsPlanting && t.Growth>=1)))throw new Exception("Grove never established");
            w.Validate();Directory.CreateDirectory("artifacts/new-grove");w.SaveFile($"artifacts/new-grove/{relaxed}.json");
            Console.WriteLine($"PASS new grove relaxed={relaxed}: {fresh.Length} spaced trees, pure proposal, actual planting/maturity, exact active save.");
        }
    }
}

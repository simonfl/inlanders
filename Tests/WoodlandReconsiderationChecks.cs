using Inlanders.Simulation;
static class WoodlandReconsiderationChecks
{
    public static void Run()
    {
        foreach(bool fallen in new[]{false,true})
        {
            var w=World.NewRiverFrontage(false,true,true);var t=w.Trees.OrderBy(t=>t.Cell.Point.LengthSquared()).First();
            if(!w.SetClearing(t.Cell,true))throw new Exception("Initial clearing");
            for(int i=0;i<5000 && !(fallen?t.Felled && t.Logs==0:t.Owner is int owner && w.People[owner].Task==Work.Chopping);i++)w.Tick(.1f);
            if(fallen? !t.Felled || t.Logs!=0:t.Owner==null)throw new Exception("Physical work not reached");
            var intent=fallen?WoodlandIntent.Renew:WoodlandIntent.Keep;string before=w.SaveJson();int remaining=t.Logs;
            if(w.PreviewWoodlandCare(t.Cell,t.Cell,intent).Problem!=null || w.SaveJson()!=before)throw new Exception("Reconsideration preview failed");
            if(fallen && (w.ApplyWoodlandCare(t.Cell,t.Cell,WoodlandIntent.Keep) || w.SaveJson()!=before))throw new Exception("Keep resurrects fallen tree");
            if(!w.ApplyWoodlandCare(t.Cell,t.Cell,intent) || t.ClearRequested || t.Logs!=remaining)throw new Exception("Reconsideration rewound wood or retained clearing");
            var copy=World.LoadJson(w.SaveJson());for(int i=0;i<100;i++){w.Tick(.1f);copy.Tick(.1f);}if(w.SaveJson()!=copy.SaveJson())throw new Exception("Reconsidered save differs");
            for(int i=0;i<2500;i++)w.Tick(.1f);
            if(fallen? t.Growth<1 || w.TreesPlanted==0:t.Felled || !t.Preserved)throw new Exception("Changed land intention did not complete");w.Validate();
            Console.WriteLine($"PASS reconsider clearing fallen={fallen}: pure preview, real-progress preservation, exact save, retained tree or mature renewal.");
        }
    }
}

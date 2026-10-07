using Inlanders.Simulation;
using System.Diagnostics;
static class PathStrokeChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    public static void Run()
    {
        var source=World.NewWorkingClearing();for(int i=0;i<100;i++)source.Tick(.1f);
        Check(source.People.Any(p=>p.Route.Count>0),"No active travelers");
        var cells=source.Map.Land.Where(c=>source.PathProblem(c)==null).Take(90).ToArray();
        var single=World.LoadJson(source.SaveJson());var batch=World.LoadJson(source.SaveJson());
        var timer=Stopwatch.StartNew();foreach(var cell in cells)single.SetPath(cell,true);double serial=timer.Elapsed.TotalMilliseconds;
        int revision=batch.PathsRevision;timer.Restart();Check(batch.SetPaths(cells,true),"Batch rejected");double batched=timer.Elapsed.TotalMilliseconds;
        Check(batch.PathsRevision==revision+1,"Multiple batch revisions");
        Check(single.SaveJson()==batch.SaveJson(),"Final path geometry/routes/claims differ");
        string saved=batch.SaveJson();Check(!batch.SetPaths(new[]{batch.Stockpile,new Cell(999,999)},true) && batch.SaveJson()==saved,"Invalid stroke mutated");
        revision=batch.PathsRevision;Check(batch.SetPaths(cells,true) && batch.PathsRevision==revision,"Repeated stroke changed revision");
        Check(batch.SetPaths(cells,false),"Erase rejected");batch.Validate();
        var copy=World.LoadJson(batch.SaveJson());for(int i=0;i<100;i++){batch.Tick(.1f);copy.Tick(.1f);}Check(batch.SaveJson()==copy.SaveJson(),"Stroke continuation differs");
        Console.WriteLine($"PASS path stroke: {cells.Length} cells, serial {serial:F2}ms, batch {batched:F2}ms; equal final routes/claims, one revision, invalid/idempotent/erase and exact continuation.");
    }
}

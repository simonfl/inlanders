using Inlanders.Simulation;
static class TimberJourneyChecks
{
    public static void Run()
    {
        var w=World.NewRiverLivelihood();
        foreach(var p in w.People){w.Assign(p.Id,Role.Builder);p.NextMealTime=p.NextRestTime=p.NextLeisureTime=10000;}
        int Cost(Cell a,Cell b){return w.PathConnection(a,b,out var path)==null?path.Skip(1).Sum(c=>w.Paths.Contains(c)?4:5):int.MaxValue;}
        var choices=(from start in w.Map.Land where w.PathProblem(start)==null
            let trees=w.Trees.Where(t=>t.Logs>0 && w.PathProblem(t.Access)==null).ToArray()
            let near=trees.OrderBy(t=>(start.Point-t.Access.Point).LengthSquared()).ThenBy(t=>t.Id).First()
            let best=trees.OrderBy(t=>(long)Cost(start,t.Access)+Cost(t.Access,w.YardAccess)).ThenBy(t=>t.Id).First()
            where near.Id!=best.Id && (long)Cost(start,near.Access)+Cost(near.Access,w.YardAccess)>(long)Cost(start,best.Access)+Cost(best.Access,w.YardAccess)
            select(start,near,best)).First();
        foreach(var tree in w.Trees)w.SetTreePreserved(tree.Cell,false);
        var worker=w.People[0];worker.Position=choices.start.Point;w.Assign(worker.Id,Role.Logger);
        w.Tick(.1f);
        if(worker.TreeId!=choices.best.Id)throw new Exception($"Forestry chose {worker.TreeId}; shortest whole journey is {choices.best.Id}, apparent-near {choices.near.Id}");
        int initial=w.Stored;var copy=World.LoadJson(w.SaveJson());
        for(int i=0;i<100;i++){w.Tick(.1f);copy.Tick(.1f);}if(w.SaveJson()!=copy.SaveJson())throw new Exception("Forestry save diverges");
        for(int i=0;i<2000 && w.Stored==initial;i++)w.Tick(.1f);
        if(w.Stored<=initial)throw new Exception("Selected timber never delivered");w.Validate();
        Console.WriteLine($"PASS forestry full walking trip {choices.start}: {choices.near.Id} → {choices.best.Id}, actual timber delivered and exact active save.");
    }
}

using Inlanders.Simulation;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;

static class RoutePerformanceChecks
{
    public static void Run(string[] args)
    {
        foreach(var kind in Enum.GetValues<BuildingKind>())for(int rotation=0;rotation<4;rotation++)
            for(int x=-4;x<=4;x++)for(int z=-4;z<=4;z++)
                if(World.OccupiesFootprint(new(1,-2),rotation,kind,new(x,z))!=World.Footprint(new(1,-2),rotation,kind).Contains(new(x,z)))
                    throw new Exception("Footprint differs for "+kind+" facing "+rotation);
        int index=Array.IndexOf(args,"--route-perf");string input=args[index+1],output=args[index+2];Directory.CreateDirectory(output);
        var w=World.LoadFile(input);w.Validate();
        var blocked=(Func<Cell,bool>)typeof(World).GetMethod("Blocked",BindingFlags.Instance|BindingFlags.NonPublic)!.CreateDelegate(typeof(Func<Cell,bool>),w);
        bool Reference(Cell c)=>w.Map.StoneDeposits.Any(d=>d.Cell==c) || w.Decorations.Any(d=>d.Cell==c && d.Solid) || !w.Map.Contains(c) ||
            (w.Map.Water.Contains(c) && !w.Cottages.Any(b=>b.Kind==BuildingKind.Bridge && b.Cell==c && b.Complete)) || c==w.Stockpile || w.Trees.Any(t=>t.Cell==c) || w.Bushes.Any(b=>b.Cell==c) ||
            w.Cottages.Any(h=>h.Kind!=BuildingKind.Bridge && World.Footprint(h.Cell,h.Rotation,h.Kind).Contains(c));
        for(int x=w.Map.MinX-2;x<=w.Map.MaxX+2;x++)for(int z=w.Map.MinZ-2;z<=w.Map.MaxZ+2;z++)
            if(blocked(new(x,z))!=Reference(new(x,z)))throw new Exception("Obstacle result differs");
        // Warm runtime/JIT without advancing the measured input.
        var warm=World.LoadFile(input);for(int i=0;i<10;i++)warm.Tick(.1f);
        var times=new List<double>();long allocated=GC.GetAllocatedBytesForCurrentThread();var total=Stopwatch.StartNew();
        for(int i=0;i<300;i++){var sw=Stopwatch.StartNew();w.Tick(.1f);times.Add(sw.Elapsed.TotalMilliseconds);}
        total.Stop();allocated=GC.GetAllocatedBytesForCurrentThread()-allocated;w.Validate();
        string json=w.SaveJson();if(World.LoadJson(json).SaveJson()!=json)throw new Exception("Performance state roundtrip differs");
        File.WriteAllText(output+"/after.json",json);times.Sort();
        var report=new{input,population=w.Population,ticks=300,wallMs=total.Elapsed.TotalMilliseconds,allocatedBytes=allocated,median=times[150],p95=times[285],max=times[^1]};
        File.WriteAllText(output+"/report.json",JsonSerializer.Serialize(report,new JsonSerializerOptions{WriteIndented=true}));Console.WriteLine(JsonSerializer.Serialize(report));
    }
}

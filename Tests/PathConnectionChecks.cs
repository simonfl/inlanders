using Inlanders.Simulation;
using System.Text.Json;

static class PathConnectionChecks
{
    public static void Run()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        var river=World.NewCreative(true);
        for(int z=river.Map.MinZ;z<=river.Map.MaxZ;z++)if(river.Map.Contains(new(7,z)))river.Map.Water.Add(new(7,z));
        string before=river.SaveJson();
        Check(!river.ConnectPaths(new(6,3),new(8,3)) && river.SaveJson()==before,"Connection crossed unbridged water or mutated on rejection");
        Check(river.Place(new(7,3),true,BuildingKind.Bridge)!=null,"Bridge setup failed");
        Check(river.ConnectPaths(new(6,3),new(8,3)) && !river.Paths.Contains(new(7,3)),"Existing bridge connection failed");
        const string dir="artifacts/hamlet-paths";Directory.CreateDirectory(dir);
        var original=World.LoadFile("artifacts/hamlet-rearrange/open-center-late.json");
        foreach(bool connected in new[]{false,true})
        {
            var w=World.LoadJson(original.SaveJson());var routes=new List<object>();
            if(connected)foreach(var pair in new[]{(6,14),(9,14),(14,16)})
            {
                var start=w.Cottages.Single(c=>c.Id==pair.Item1).Entrance;var end=w.Cottages.Single(c=>c.Id==pair.Item2).Entrance;
                before=w.SaveJson();Check(w.PathConnection(start,end,out var route)==null && before==w.SaveJson(),"Preview mutated or rejected route");
                for(int i=1;i<route.Count;i++)Check(Math.Abs(route[i].X-route[i-1].X)+Math.Abs(route[i].Z-route[i-1].Z)==1,"Disconnected route");
                Check(w.ConnectPaths(start,end),"Route rejected");int revision=w.PathsRevision;
                Check(w.ConnectPaths(start,end) && revision==w.PathsRevision,"Repeated connection changed paths");routes.Add(new{start,end,tiles=route.Count});
            }
            string arm=connected?"connected":"unchanged";w.Validate();w.SaveFile(dir+"/"+arm+"-start.json");
            var restored=World.LoadJson(w.SaveJson());double hungry=0;
            for(int i=0;i<3000;i++){w.Tick(.1f);restored.Tick(.1f);hungry+=w.People.Count(p=>!p.Fed)*.1;}
            w.Validate();Check(w.SaveJson()==restored.SaveJson(),"Path continuation diverged");w.SaveFile(dir+"/"+arm+"-late.json");
            var report=new{arm,w.Population,w.Housed,hungry,paths=w.Paths.Count,routes};File.WriteAllText(dir+"/"+arm+"-report.json",JsonSerializer.Serialize(report,new JsonSerializerOptions{WriteIndented=true}));Console.WriteLine(JsonSerializer.Serialize(report));
        }
        PathChecks.Run();Console.WriteLine("PASS: atomic route connections, bridge crossing, readonly previews, idempotence, connected steps and exact continuation.");
    }
}

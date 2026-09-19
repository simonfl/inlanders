using Inlanders.Simulation;
using System.Numerics;
using System.Text.Json;

static class HamletRearrangementChecks
{
    public static void Run(string[] args)
    {
        int index=Array.IndexOf(args,"--hamlet-rearrange");string input=args[index+1];
        const string dir="artifacts/hamlet-rearrange";Directory.CreateDirectory(dir);
        var original=World.LoadFile(input);original.Validate();
        foreach(string arm in new[]{"unchanged","open-center"})
        {
            var w=World.LoadJson(original.SaveJson());var moves=new List<object>();
            if(arm=="open-center")
            {
                string before=w.SaveJson();
                var producer=w.Cottages.First(c=>c.Kind==BuildingKind.ForagerHut);
                if(w.MoveBuilding(producer.Id,new(-3,-7),0) || before!=w.SaveJson())throw new Exception("Founding moved production");
                foreach(var request in new[]{(Id:6,At:new Cell(-3,-5)),(Id:9,At:new Cell(0,9)),(Id:14,At:new Cell(-3,0))})
                {
                    var site=w.Cottages.Single(c=>c.Id==request.Id);var was=site.Cell;
                    var residents=w.People.Where(p=>p.HomeId==site.Id).Select(p=>p.Id).ToArray();
                    int logs=w.Stored,planks=w.Planks;
                    before=w.SaveJson();
                    if(w.MoveBuilding(site.Id,new(w.Map.MaxX+10,0),0) || w.SaveJson()!=before)throw new Exception("Rejected move changed village");
                    var choice=w.Map.Land.SelectMany(c=>Enumerable.Range(0,4).Select(r=>new{Cell=c,Rotation=r}))
                        .OrderBy(p=>(p.Cell.Point-request.At.Point).LengthSquared()).ThenBy(p=>p.Cell.Z).ThenBy(p=>p.Cell.X).ThenBy(p=>p.Rotation)
                        .First(p=>w.RelocationProblem(site.Id,p.Cell,p.Rotation)==null);
                    if(w.SaveJson()!=before)throw new Exception("Move query mutated state");
                    int interrupted=w.People.Count(p=>(p.HomeId==site.Id && p.Task is Work.ToRest or Work.Resting) || p.LeisureSiteId==site.Id);
                    if(!w.MoveBuilding(site.Id,choice.Cell,choice.Rotation))throw new Exception("Move failed");
                    if(w.Stored!=logs || w.Planks!=planks || residents.Any(id=>w.People[id].HomeId!=site.Id))throw new Exception("Move lost materials/residents");
                    w.Validate();moves.Add(new{site.Id,was,requested=request.At,site.Cell,site.Rotation,interrupted});
                }
            }
            w.SaveFile(dir+"/"+arm+"-start.json");
            double hungry=0,restTravel=0;int visits=w.People.Sum(p=>p.LeisureVisits);
            for(int i=0;i<6000;i++)
            {
                var positions=w.People.Select(p=>p.Position).ToArray();var tasks=w.People.Select(p=>p.Task).ToArray();w.Tick(.1f);
                for(int n=0;n<w.Population;n++)if(tasks[n]==Work.ToRest)restTravel+=Vector2.Distance(positions[n],w.People[n].Position);
                hungry+=w.People.Count(p=>!p.Fed)*.1;if(i%1000==0)w.Validate();
            }
            w.Validate();string json=w.SaveJson();if(World.LoadJson(json).SaveJson()!=json)throw new Exception("Rearranged save mismatch");
            w.SaveFile(dir+"/"+arm+"-late.json");
            var report=new{arm,w.Population,w.Housed,hungry,restTravel,visits=w.People.Sum(p=>p.LeisureVisits)-visits,moves};
            File.WriteAllText(dir+"/"+arm+"-report.json",JsonSerializer.Serialize(report,new JsonSerializerOptions{WriteIndented=true}));Console.WriteLine(JsonSerializer.Serialize(report));
        }
    }
}

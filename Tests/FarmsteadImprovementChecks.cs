using Inlanders.Simulation;
using System.Numerics;
using System.Text.Json;

static class FarmsteadImprovementChecks
{
    public static void Run()
    {
        const string folder="artifacts/farmstead-improvements";Directory.CreateDirectory(folder);
        var baseline=World.LoadFile("artifacts/river-farmstead/mixed.json");var results=new List<object>();
        foreach(var arm in new[]{"unchanged","common-place","shore-homes"})
        {
            var w=World.LoadJson(baseline.SaveJson());
            if(arm=="common-place")
            {
                var p=ReviewPlacement.Find(w,BuildingKind.Square,new(3,-3),(c,r)=>true,"common place")!;
                if(w.Place(p.Actual,p.Rotation,BuildingKind.Square)==null)throw new Exception("Square refused");
            }
            if(arm=="shore-homes")
            {
                var homes=w.Cottages.Where(c=>c.Kind==BuildingKind.Cottage).Skip(2).ToArray();
                var targets=new[]{new Cell(5,0),new(5,7)};
                for(int i=0;i<homes.Length;i++)
                {
                    var h=homes[i];var options=w.Map.Land.SelectMany(c=>Enumerable.Range(0,4).Select(r=>new{c,r})).OrderBy(p=>(p.c.Point-targets[i].Point).LengthSquared());
                    var p=options.First(p=>w.RelocationProblem(h.Id,p.c,p.r)==null);
                    if(!w.MoveBuilding(h.Id,p.c,p.r))throw new Exception("Move refused");
                }
                var dock=w.Cottages.Single(c=>c.Kind==BuildingKind.FishingDock);
                foreach(var home in w.Cottages.Where(c=>c.Kind==BuildingKind.Cottage))
                    if(!w.ConnectPaths(home.Entrance,dock.Entrance))throw new Exception("Path refused");
            }
            w.SaveFile(folder+"/"+arm+"-start.json");
            int visits=w.People.Sum(p=>p.LeisureVisits);double hungry=0,mealTravel=0,restTravel=0;
            for(int i=0;i<6000;i++)
            {
                var pos=w.People.Select(p=>p.Position).ToArray();var task=w.People.Select(p=>p.Task).ToArray();w.Tick(.1f);
                for(int j=0;j<w.Population;j++)
                {
                    double distance=Vector2.Distance(pos[j],w.People[j].Position);
                    if(task[j] is Work.ToMealSupply or Work.ToMealSeat)mealTravel+=distance;
                    if(task[j]==Work.ToRest)restTravel+=distance;
                }
                hungry+=w.People.Count(p=>!p.Fed)*.1;
                if(i%600==0)w.Validate();
            }
            w.Validate();if(World.LoadJson(w.SaveJson()).SaveJson()!=w.SaveJson())throw new Exception("Save mismatch");
            w.SaveFile(folder+"/"+arm+".json");
            var row=new{arm,hungry,mealTravel,restTravel,visits=w.People.Sum(p=>p.LeisureVisits)-visits,w.Population,w.Housed,paths=w.Paths.Count};
            results.Add(row);Console.WriteLine(JsonSerializer.Serialize(row));
        }
        File.WriteAllText(folder+"/report.json",JsonSerializer.Serialize(results,new JsonSerializerOptions{WriteIndented=true}));
    }
}

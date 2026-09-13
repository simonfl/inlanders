using Inlanders.Simulation;
using System.Text.Json;

static class CourtChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    public static void Reader()
    {
        var w=World.NewArrangementCourt();var p=w.People[0];
        // A carried meal may outlive its producer; the reader must not dereference a removed site.
        for(int i=0;i<1000 && p.Meal?.Carrying!=true;i++)w.Tick(.1f);
        Check(p.Meal?.Carrying==true,"Reader fixture never collected a meal");p.Meal!.SourceId=999;
        string state=w.SaveJson();var read=w.ReadDailyJourney(p.Id);
        Check(!read.CanInspect && read.Detail.Contains("removed") && w.SaveJson()==state,"Removed-source report failed or mutated state");
        p.Task=Work.ReturnMeal;read=w.ReadDailyJourney(p.Id);Check(read.Source==null && read.CanInspect,"Returned meal must identify central destination");
        w=World.NewArrangementCourt();p=w.People[0];
        for(int i=0;i<1000 && p.Meal?.Eaten!=true;i++)w.Tick(.1f);
        Check(p.Meal?.Eaten==true && !w.ReadDailyJourney(p.Id).Detail.Contains("Next request in 0s"),"Eaten meal reports an immediate next request");
        Console.WriteLine("PASS: daily reader preserves state, handles removed sources/returns and the next meal window");
    }
    public static World Expanded()
    {
        var w=World.NewArrangementCourt();
        foreach(var target in new[]{new Cell(-5,-3),new(-1,-3),new(1,6),new(10,6)})
        {
            var plan=ReviewPlacement.Find(w,BuildingKind.Cottage,target,(_,_)=>true,"expanded court")!;
            var home=w.Place(plan.Actual,plan.Rotation,plan.Kind)!;
            for(int i=0;i<12000 && !home.Complete;i++)w.Tick(.1f);
            Check(home.Complete,"Expanded court construction stalled");
        }
        Check(w.InviteNewcomers(),"Expanded court invitation failed");
        for(int i=0;i<2400;i++)w.Tick(.1f);
        w.Validate();return w;
    }
    public static void Run(bool includeRecovery=true)
    {
        Directory.CreateDirectory("artifacts/court");
        var control=World.NewInheritedShoreline();var w=World.NewArrangementCourt();
        Check(w.Population==control.Population && w.EdibleStored==control.EdibleStored && w.Stored==control.Stored && w.InitialLogs==control.InitialLogs,"Unmatched starting resources");
        Check(w.Cottages.Select(c=>c.Kind).SequenceEqual(control.Cottages.Select(c=>c.Kind)),"Unmatched building capability");
        var home=w.Cottages.First(c=>Buildings.Get(c.Kind).Beds>0);var original=home.Cell;int facing=home.Rotation;
        var plan=w.Map.Land.SelectMany(c=>Enumerable.Range(0,4).Select(r=>(c,r))).First(p=>p.c!=original && w.RelocationProblem(home.Id,p.c,p.r)==null);
        string before=w.SaveJson();Check(w.RelocationProblem(home.Id,new(999,999),0)!=null && w.SaveJson()==before,"Rejected preview mutates world");
        Check(w.MoveBuilding(home.Id,plan.c,plan.r),"Trial move failed");
        var moved=w.SaveJson();w=World.LoadJson(moved);Check(w.SaveJson()==moved,"Trial save mismatch");
        Check(w.RelocationProblem(w.Cottages.Last().Id)!=null,"Second building bypassed trial limit");
        Check(w.RemovalProblem(home.Id)!=null,"Trial demolition bypassed restore");
        Check(w.RestoreArrangement(),"Trial restore failed");home=w.Cottages.Single(c=>c.Id==home.Id);
        Check(home.Cell==original && home.Rotation==facing && w.Neighborhood!.Arrangement!.BuildingId==null,"Wrong restored location");
        w.Validate();
        // Advice must not change just because an automatic worker temporarily has a role.
        foreach(var p in w.People)p.Role=Role.Hauler;
        Check(!w.ReadEconomy().Issues.Any(i=>i.Id=="stockpile" || i.Staff!=null),"Legacy role advice survived");
        foreach(var p in w.People)p.Role=Role.Farmer;
        var oldGarden=w.Cottages.First(c=>c.Kind==BuildingKind.VegetableGarden);
        w.Cottages.RemoveAll(c=>c.Kind==BuildingKind.VegetableGarden);
        Check(w.Place(oldGarden.Cell,0,BuildingKind.Orchard)!=null,"Orchard advice fixture failed");
        Check(!w.ReadEconomy().Issues.Any(i=>i.Build==BuildingKind.Farm),"Transient farmer demands a farm");
        var results=new List<object>();
        foreach(string arm in new[]{"dispersed","court","court-local-garden","court-paused"})
        {
            w=arm=="dispersed"?World.NewInheritedShoreline():World.NewArrangementCourt();
            if(arm=="court-local-garden")
            {
                var garden=w.Cottages.First(c=>c.Kind==BuildingKind.VegetableGarden);
                var at=w.Map.Land.Where(c=>c.X<5).OrderBy(c=>(c.Point-new Cell(1,6).Point).LengthSquared())
                    .SelectMany(c=>Enumerable.Range(0,4).Select(r=>(c,r))).First(p=>w.RelocationProblem(garden.Id,p.c,p.r)==null);
                Check(w.MoveBuilding(garden.Id,at.c,at.r),"Garden trial failed");
            }
            if(arm=="court-paused")foreach(var c in w.Cottages.Where(c=>w.IsWorkplaceFoodStore(c)))w.SetWorkplacePaused(c.Id,true);
            results.Add(Observe(w,arm,600));w.SaveFile("artifacts/court/"+arm+".json");
            var save=w.SaveJson();var copy=World.LoadJson(save);for(int i=0;i<10;i++){w.Tick(.1f);copy.Tick(.1f);}Check(w.SaveJson()==copy.SaveJson(),"Continuation diverged");
        }
        File.WriteAllText("artifacts/court/results.json",JsonSerializer.Serialize(results,new JsonSerializerOptions{WriteIndented=true}));
        // Same-age weak-layout control: no comparison with its earlier observation window.
        if(includeRecovery && File.Exists("artifacts/inherited/east-homes.json"))
        {
            var baseline=World.LoadFile("artifacts/inherited/east-homes.json");var repaired=World.LoadJson(baseline.SaveJson());
            var plan2=ReviewPlacement.Find(repaired,BuildingKind.VegetableGarden,new(0,6),(c,r)=>c.X<5,"recovery")!;
            var garden=repaired.Place(plan2.Actual,plan2.Rotation,plan2.Kind)!;
            int ticks=0;while(!garden.Complete && ticks++<24000){repaired.Tick(.1f);baseline.Tick(.1f);}Check(garden.Complete,"Recovery construction failed");
            for(int i=0;i<3000;i++){repaired.Tick(.1f);baseline.Tick(.1f);}
            var paired=new[]{Observe(baseline,"weak-unchanged",600),Observe(repaired,"weak-garden",600)};
            File.WriteAllText("artifacts/court/recovery.json",JsonSerializer.Serialize(paired,new JsonSerializerOptions{WriteIndented=true}));
        }
        Console.WriteLine("PASS: matched court capability, reversible relocation, current saves, role-independent advice and observed comparison");
    }
    static object Observe(World w,string arm,int seconds)
    {
        float start=w.Food.Time;double hungry=0,walking=0;var trace=new List<object>();var previous=new Dictionary<int,string>();
        for(int i=0;i<seconds*10;i++)
        {
            foreach(var p in w.People)
            {
                if(!p.Fed)hungry+=.1;
                if(p.Route.Count>0 && p.Task is Work.ToMealSupply or Work.ToMealSeat)walking+=.1;
                if(i%5!=0)continue;
                string state=$"{p.Meal?.Id}/{p.Task}/{p.Meal?.Reserved}/{p.Meal?.Carrying}/{p.Meal?.Eaten}/{p.Fed}";
                if(previous.GetValueOrDefault(p.Id)==state)continue;previous[p.Id]=state;
                var j=w.ReadDailyJourney(p.Id);
                trace.Add(new{time=w.Food.Time,person=p.Id,request=p.Meal?.Id,due=p.Meal?.Due,source=p.Meal?.SourceId,task=p.Task.ToString(),remainingRoute=p.Route.Count,carried=p.Carried,fed=p.Fed,diagnosis=j.Heading});
            }
            w.Tick(.1f);if(i%100==0)w.Validate();
        }
        File.WriteAllText($"artifacts/court/{arm}-trace.json",JsonSerializer.Serialize(trace));
        Console.WriteLine($"{arm}: {hungry:F1} hungry person-s, {walking:F1} meal-walking person-s, {w.EdibleStored} food");
        return new{arm,start,end=w.Food.Time,hungry,walking,food=w.EdibleStored};
    }
}


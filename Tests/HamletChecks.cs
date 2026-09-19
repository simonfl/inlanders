using Inlanders.Simulation;
using System.Numerics;
using System.Text.Json;

// Same founded village and ordinary commands; layout is a hypothesis, not a required solution.
static class HamletChecks
{
    public static World Observed(string arm)
    {
        const string dir="artifacts/hamlet";Directory.CreateDirectory(dir);
        var w=FoundingHallChecks.Ready();float start=w.Food.Time;
        var orders=new List<ReviewPlacement>();
        void Place(BuildingKind kind,Cell at)
        {
            var plan=ReviewPlacement.Find(w,kind,at,(c,r)=>c.X<=2,"home bank")??throw new Exception("No hamlet site: "+kind);
            if(w.Place(plan.Actual,plan.Rotation,kind)==null)throw new Exception("Rejected hamlet order");orders.Add(plan);
        }
        if(arm=="compact")foreach(var at in new[]{new Cell(-7,0),new(-7,4),new(-7,8),new(-3,9)})Place(BuildingKind.Cottage,at);
        else if(arm!="twelve")
        {
            Place(BuildingKind.Sawmill,new(-8,8));
            Place(BuildingKind.Lodge,new(-7,0));Place(BuildingKind.Lodge,new(-3,9));
            Place(BuildingKind.SeatingGarden,new(-4,3));
        }
        if(arm!="twelve")
        {
            Place(BuildingKind.VegetableGarden,new(-7,2));
            if(arm!="imperfect")Place(BuildingKind.FishingDock,new(3,3));
        }
        w.SaveFile(dir+"/"+arm+"-start.json");
        double hungry=0,lateHungry=0,mealTravel=0;float? settled=null;int leisureBefore=w.People.Sum(p=>p.LeisureVisits);
        for(int i=0;i<12000;i++)
        {
            if(arm=="imperfect" && i==6000)Place(BuildingKind.FishingDock,new(3,3));
            var positions=w.People.Select(p=>p.Position).ToArray();var tasks=w.People.Select(p=>p.Task).ToArray();
            w.Tick(.1f);
            for(int n=0;n<positions.Length;n++)if(tasks[n] is Work.ToMealSupply or Work.ToMealSeat)mealTravel+=Vector2.Distance(positions[n],w.People[n].Position);
            double h=w.People.Count(p=>!p.Fed)*.1;hungry+=h;if(i>=9000)lateHungry+=h;
            if(arm!="twelve" && w.Population<20 && w.InvitationProblem()==null && !w.InviteNewcomers())throw new Exception("Invitation failed");
            if(settled==null && w.Population==(arm=="twelve"?12:20) && w.FinishFoundingProblem()==null)settled=w.Food.Time-start;
            if(i%100==0)w.Validate();
            if(i==5999)w.SaveFile(dir+"/"+arm+"-middle.json");
        }
        w.Validate();string json=w.SaveJson();if(World.LoadJson(json).SaveJson()!=json)throw new Exception("Hamlet save differs");
        w.SaveFile(dir+"/"+arm+"-late.json");
        var report=new{arm,w.Population,w.Housed,settled,hungry,lateHungry,mealTravel,visits=w.People.Sum(p=>p.LeisureVisits)-leisureBefore,buildings=w.Cottages.Count,footprint=w.Cottages.Sum(c=>World.Footprint(c.Cell,c.Rotation,c.Kind).Count()),orders};
        File.WriteAllText(dir+"/"+arm+"-report.json",JsonSerializer.Serialize(report,new JsonSerializerOptions{WriteIndented=true}));Console.WriteLine(JsonSerializer.Serialize(report));return w;
    }
    public static void Run(){foreach(string arm in new[]{"compact","spacious","imperfect","twelve"})Observed(arm);}
}

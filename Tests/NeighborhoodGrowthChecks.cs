using Inlanders.Simulation;
using System.Numerics;
using System.Text.Json;

// F31e: larger population is a hypothesis, not an objective to force into the game.
static class NeighborhoodGrowthChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    public static void Recovery()
    {
        const string dir="artifacts/neighborhood-growth";
        var w=World.LoadFile(dir+"/twenty-homes-middle.json");
        var plan=ReviewPlacement.Find(w,BuildingKind.VegetableGarden,new(-7,2),(c,r)=>c.X<=2,"home bank recovery")??throw new Exception("No recovery site");
        var garden=w.Place(plan.Actual,plan.Rotation,plan.Kind);Check(garden!=null,"Recovery order rejected");
        double hungry=0,lateHungry=0;float? built=null;float start=w.Food.Time;
        for(int i=0;i<6000;i++)
        {
            w.Tick(.1f);double h=w.People.Count(p=>!p.Fed)*.1;hungry+=h;if(i>=3000)lateHungry+=h;
            if(built==null && garden!.Complete)built=w.Food.Time-start;
            if(i%100==0)w.Validate();
        }
        w.Validate();string json=w.SaveJson();Check(World.LoadJson(json).SaveJson()==json,"Recovery save mismatch");w.SaveFile(dir+"/recovered.json");
        var result=new{built,hungry,lateHungry,w.Population,stored=w.EdibleStored,plan};
        File.WriteAllText(dir+"/recovery-report.json",JsonSerializer.Serialize(result,new JsonSerializerOptions{WriteIndented=true}));Console.WriteLine(JsonSerializer.Serialize(result));
    }
    public static void Run()
    {
        const string dir="artifacts/neighborhood-growth";Directory.CreateDirectory(dir);
        var baseline=FoundingHallChecks.Ready();baseline.SaveFile(dir+"/initial.json");
        var reports=new List<object>();
        foreach(string arm in new[]{"twelve-control","twenty-homes","twenty-mixed","twenty-dispersed"})
        {
            var w=World.LoadJson(baseline.SaveJson());var orders=new List<ReviewPlacement>();
            bool dispersed=arm=="twenty-dispersed",mixed=arm is "twenty-mixed" or "twenty-dispersed";
            void Place(BuildingKind kind,Cell target)
            {
                var plan=ReviewPlacement.Find(w,kind,target,(c,r)=>dispersed?c.X>=10:c.X<=2,dispersed?"east bank":"home bank")??throw new Exception("No legal site: "+kind);
                Check(w.Place(plan.Actual,plan.Rotation,kind)!=null,"Order rejected");orders.Add(plan);
            }
            if(arm!="twelve-control")
                foreach(var at in dispersed?new[]{new Cell(13,5),new(16,5),new(13,9),new(16,9)}:new[]{new Cell(-7,0),new(-7,4),new(-7,8),new(-3,9)})Place(BuildingKind.Cottage,at);
            if(mixed){Place(BuildingKind.VegetableGarden,dispersed?new(12,2):new(-7,2));Place(BuildingKind.FishingDock,dispersed?new(11,4):new(3,3));}
            w.SaveFile(dir+"/"+arm+"-start.json");
            float? settled=null;double hungry=0,lateHungry=0,mealTravel=0,restTravel=0,lateIdle=0,latePersonSeconds=0;
            var invitations=new List<object>();int targetPopulation=arm=="twelve-control"?12:20;
            for(int i=0;i<12000;i++)
            {
                var positions=w.People.Select(p=>p.Position).ToArray();
                var tasks=w.People.Select(p=>p.Task).ToArray();
                w.Tick(.1f);
                for(int n=0;n<positions.Length;n++)
                {
                    double d=Vector2.Distance(positions[n],w.People[n].Position);
                    if(tasks[n] is Work.ToMealSupply or Work.ToMealSeat)mealTravel+=d;
                    if(tasks[n]==Work.ToRest)restTravel+=d;
                }
                double unfed=w.People.Count(p=>!p.Fed)*.1;hungry+=unfed;
                if(i>=9000){lateHungry+=unfed;lateIdle+=w.People.Count(p=>p.Task==Work.Waiting)*.1;latePersonSeconds+=w.Population*.1;}
                if(w.Population<targetPopulation && w.InvitationProblem()==null)
                {Check(w.InviteNewcomers(),"Invitation rejected");invitations.Add(new{elapsed=w.Food.Time-baseline.Food.Time,w.Population});}
                if(settled==null && w.Population==targetPopulation && w.FinishFoundingProblem()==null)settled=w.Food.Time-baseline.Food.Time;
                if(i%100==0)w.Validate();
                if(i==5999)w.SaveFile(dir+"/"+arm+"-middle.json");
            }
            w.Validate();string json=w.SaveJson();Check(World.LoadJson(json).SaveJson()==json,"Current save mismatch");w.SaveFile(dir+"/"+arm+"-late.json");
            var result=new{arm,settled,w.Population,w.Housed,hungry,lateHungry,mealTravel,restTravel,lateIdleFraction=lateIdle/latePersonSeconds,stored=w.EdibleStored,orders,invitations};
            reports.Add(result);File.WriteAllText(dir+"/report.json",JsonSerializer.Serialize(reports,new JsonSerializerOptions{WriteIndented=true}));Console.WriteLine(JsonSerializer.Serialize(result));
        }
        Recovery();
        Console.WriteLine("PASS: ordinary growth commands, twenty-minute controls, world validation and current saves; design outcomes are observations");
    }
}

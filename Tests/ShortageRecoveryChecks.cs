using Inlanders.Simulation;
using System.Text.Json;
static class ShortageRecoveryChecks
{
    public static World Midpoint()
    {
        var w=World.LoadJson(FoundingHallChecks.Ready().SaveJson());
        foreach(var at in new[]{new Cell(-7,0),new(-7,4),new(-7,8),new(-3,9)})
        {
            var plan=ReviewPlacement.Find(w,BuildingKind.Cottage,at,(c,r)=>c.X<=2,"home bank")??throw new Exception("No growth site");
            if(w.Place(plan.Actual,plan.Rotation,plan.Kind)==null)throw new Exception("Growth order failed");
        }
        for(int i=0;i<6000;i++){w.Tick(.1f);if(w.Population<20 && w.InvitationProblem()==null && !w.InviteNewcomers())throw new Exception("Invitation failed");}
        w.Validate();return w;
    }
    public static void Run()
    {
        const string dir="artifacts/shortage-recovery";Directory.CreateDirectory(dir);
        var baseline=Midpoint();baseline.SaveFile(dir+"/midpoint.json");
        var results=new List<object>();
        foreach(string arm in new[]{"pantry","garden-dock"})
        {
            var w=World.LoadJson(baseline.SaveJson());var orders=new List<ReviewPlacement>();
            Cottage Place(BuildingKind kind,Cell at)
            {
                var plan=ReviewPlacement.Find(w,kind,at,(c,r)=>c.X<=2,"recovery")??throw new Exception("No legal recovery site");orders.Add(plan);
                return w.Place(plan.Actual,plan.Rotation,kind)??throw new Exception("Order rejected");
            }
            if(arm=="pantry"){var pantry=Place(BuildingKind.Pantry,new(-7,2));if(!w.SetPantryTarget(pantry.Id,12))throw new Exception("Target rejected");}
            else{Place(BuildingKind.VegetableGarden,new(-7,2));Place(BuildingKind.FishingDock,new(3,3));}
            double hungry=0,lateHungry=0;
            for(int i=0;i<6000;i++){w.Tick(.1f);double h=w.People.Count(p=>!p.Fed)*.1;hungry+=h;if(i>=3000)lateHungry+=h;if(i%100==0)w.Validate();}
            w.Validate();string saved=w.SaveJson();if(World.LoadJson(saved).SaveJson()!=saved)throw new Exception("Save mismatch");w.SaveFile(dir+"/"+arm+".json");
            var result=new{arm,hungry,lateHungry,stored=w.EdibleStored,orders};results.Add(result);Console.WriteLine(JsonSerializer.Serialize(result));
        }
        File.WriteAllText(dir+"/report.json",JsonSerializer.Serialize(results,new JsonSerializerOptions{WriteIndented=true}));
    }
}

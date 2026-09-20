using Inlanders.Simulation;
using System.Text.Json;
static class WorkingVillageChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    public static void Run()
    {
        const string folder="artifacts/working-village";Directory.CreateDirectory(folder);
        var initial=World.NewWorkingVillage();initial.SaveFile(folder+"/initial.json");var rows=new List<object>();
        foreach(string arm in new[]{"unchanged","crossing","shore","poor-recovered"})
        {
            var w=World.LoadJson(initial.SaveJson());Check(w.Housed==8 && w.Population==8,"Starting village incomplete");
            if(arm=="crossing")Check(w.Place(new(4,2),0,BuildingKind.Bridge)!=null,"Crossing rejected");
            if(arm=="shore")Check(w.Place(new(7,8),1,BuildingKind.FishingDock)!=null,"Shore rejected");
            var oven=w.Cottages.Single(c=>c.Kind==BuildingKind.Bakery);
            if(arm=="poor-recovered")
            {
                w.SetWorkplacePaused(oven.Id,true);
                var at=w.Map.Land.OrderBy(c=>(c.Point-new Cell(-7,3).Point).LengthSquared()).First(c=>w.RelocationProblem(oven.Id,c,0)==null);
                Check(w.MoveBuilding(oven.Id,at,0),"Poor placement rejected");w.SetWorkplacePaused(oven.Id,false);
            }
            double hungry=0,walking=0;float? ready=null;
            for(int i=0;i<9000;i++)
            {
                if(arm=="poor-recovered" && i==4200){w.SetWorkplacePaused(oven.Id,true);Check(w.MoveBuilding(oven.Id,new(6,-3),0),"Recovery rejected");w.SetWorkplacePaused(oven.Id,false);}
                w.Tick(.1f);hungry+=w.People.Count(p=>!p.Fed)*.1;walking+=w.People.Count(p=>p.Route.Count>0 && p.Task is Work.ToMealSupply or Work.ToMealSeat)*.1;
                if(ready==null && w.FinishFoundingProblem()==null)ready=w.Food.Time;
                if(i%300==0)w.Validate();
                if(i==4499){w.SaveFile(folder+"/"+arm+"-middle.json");w=World.LoadJson(w.SaveJson());oven=w.Cottages.Single(c=>c.Kind==BuildingKind.Bakery);}
            }
            Check(w.FinishFoundingProblem()==null && w.Population==8,"Optional ending failed: "+arm);w.FinishFounding();w.SaveFile(folder+"/"+arm+".json");
            w.PathConnection(w.Cottages.First(c=>c.Kind==BuildingKind.Cottage).Entrance,oven.Entrance,out var homeOven);
            var row=new{arm,homeOvenSteps=homeOven.Count,ready,hungry,mealWalking=walking,stored=w.EdibleStored,w.Food.EatenBread,w.Food.EatenFish};rows.Add(row);Console.WriteLine(JsonSerializer.Serialize(row));
        }
        File.WriteAllText(folder+"/report.json",JsonSerializer.Serialize(rows,new JsonSerializerOptions{WriteIndented=true}));
        Console.WriteLine("PASS: inhabited village, two ordinary approaches, recoverable poor placement, optional no-growth finish and current saves.");
    }
}

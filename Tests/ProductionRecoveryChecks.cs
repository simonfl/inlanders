using Inlanders.Simulation;
using System.Text.Json;
static class ProductionRecoveryChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    static void Step(World w,int ticks){for(int i=0;i<ticks;i++){w.Tick(.1f);if(i%100==0)w.Validate();}w.Validate();}
    public static void Run()
    {
        const string folder="artifacts/production-recovery";Directory.CreateDirectory(folder);
        var start=World.LoadFile("artifacts/river-farmstead/bread.json");var oven=start.Cottages.Single(c=>c.Kind==BuildingKind.Bakery);
        var original=oven.Cell;int facing=oven.Rotation;
        Check(start.RelocationProblem(oven.Id)!=null,"Active producer can move without pausing");
        start.SetWorkplacePaused(oven.Id,true);
        var far=start.Map.Land.OrderBy(c=>(c.Point-new Cell(-7,5).Point).LengthSquared()).First(c=>c!=original && start.RelocationProblem(oven.Id,c,0)==null);
        Check(start.MoveBuilding(oven.Id,far,0),"Misplaced oven setup failed");start.SetWorkplacePaused(oven.Id,false);Step(start,1800);
        start.SaveFile(folder+"/misplaced.json");var rows=new List<object>();
        foreach(string arm in new[]{"unchanged","rebuild","relocate"})
        {
            var w=World.LoadJson(start.SaveJson());var old=w.Cottages.Single(c=>c.Kind==BuildingKind.Bakery);int delivered=w.DeliveredBread;float began=w.Food.Time;float? first=null;double hunger=0;
            if(arm=="rebuild")
            {
                Check(w.RequestDemolition(old.Id),"Rebuild demolition rejected");Check(w.Place(original,facing,BuildingKind.Bakery)!=null,"Replacement rejected");
            }
            if(arm=="relocate")
            {
                int food=w.EdibleStored,grain=old.InputGrain,bread=old.OutputBread;float progress=old.BakeProgress;
                w.SetWorkplacePaused(old.Id,true);string before=w.SaveJson();
                Check(w.RelocationProblem(old.Id,new(100,100),0)!=null && w.SaveJson()==before,"Rejected move mutated state");
                Check(w.MoveBuilding(old.Id,original,facing),"Paused recovery rejected");
                Check(old.InputGrain==grain && old.OutputBread==bread && old.BakeProgress==progress && w.EdibleStored==food,"Move lost goods or processing");
                w.SetWorkplacePaused(old.Id,false);w=World.LoadJson(w.SaveJson());
                Check(w.RelocationProblem(w.Cottages.First(c=>c.Kind==BuildingKind.Farm).Id)!=null,"Field became movable");
            }
            for(int i=0;i<3000;i++){w.Tick(.1f);hunger+=w.People.Count(p=>!p.Fed)*.1;if(first==null && w.DeliveredBread>delivered)first=w.Food.Time-began;if(i%100==0)w.Validate();}
            Check(w.DeliveredBread>delivered,"Production failed to resume: "+arm);
            if(arm=="rebuild")Check(w.Cottages.All(c=>c.Id!=old.Id),"Demolition never finished");
            w.Validate();Check(World.LoadJson(w.SaveJson()).SaveJson()==w.SaveJson(),"Recovery save differs");w.SaveFile(folder+"/"+arm+".json");
            var row=new{arm,first,delivered=w.DeliveredBread-delivered,hunger,food=w.EdibleStored};rows.Add(row);Console.WriteLine(JsonSerializer.Serialize(row));
        }
        File.WriteAllText(folder+"/report.json",JsonSerializer.Serialize(rows,new JsonSerializerOptions{WriteIndented=true}));
        Console.WriteLine("PASS: matched misplaced-workplace recovery, goods/processing, rejected queries, field restriction and current saves.");
    }
}

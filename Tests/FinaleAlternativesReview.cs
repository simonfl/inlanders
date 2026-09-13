using Inlanders.Simulation;
using System.Text.Json;
using static FinaleDecisionChecks;

// Opt-in experiment, not a balance acceptance test: record failures as well as successes.
static class FinaleAlternativesReview
{
    static readonly string Folder="artifacts/finale-alternatives";
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    public static void Run()
    {
        Directory.CreateDirectory(Folder);
        var w=World.NewCampaign(10);
        var bridge=Build(w,new(6,3),BuildingKind.Bridge,1);
        Build(w,new(-2,0),BuildingKind.Cottage);
        Until(w,()=>w.PlacementProblem(new(1,5),0,BuildingKind.Cottage)==null,"opening home access");Build(w,new(1,5),BuildingKind.Cottage);
        w.Assign(7,Role.Logger);Until(w,()=>bridge.Complete && w.Beds>=12,"opening homes");Grow(w,12);
        w.Assign(8,Role.Farmer);w.Assign(9,Role.Builder);Check(w.AdvanceFinalePhase(),"first assessment");
        Until(w,()=>w.Campaign!.Finale!.Phase==2,"first support");SecondBuild(w,true);
        Until(w,()=>w.Beds>=20,"expanded homes");Grow(w,20);foreach(int id in new[]{10,12,14})w.Assign(id,Role.Farmer);
        Check(w.AdvanceFinalePhase(),"second assessment");Until(w,()=>w.Campaign!.Finale!.Phase==4,"expanded support");
        float earned=w.Food.Time;
        // Pay the same home replacement/clearing cost in every arm. No teleported buildings or free materials.
        FreeCentralPlot(w,true,new(1,5),new(21,10));FreeCentralPlot(w,true,new(-2,0),new(10,-3),2);FreeCentralPlot(w,true,new(-6,4),new(14,-3),2);
        Until(w,()=>w.PlacementProblem(new(0,2),0,BuildingKind.SeatingGarden)==null,"break space");Build(w,new(0,2),BuildingKind.SeatingGarden);
        Until(w,()=>w.Cottages.All(c=>c.Complete),"common construction");
        w.Assign(16,Role.Farmer);w.Assign(17,Role.Baker);w.Assign(19,Role.Baker);
        Check(!w.Creative && w.Population==20 && !w.Cottages.Any(c=>c.Kind is BuildingKind.Farm or BuildingKind.Bakery),"Unmatched comparison baseline");
        string baseline=w.SaveJson();File.WriteAllText($"{Folder}/baseline.json",baseline);
        Console.WriteLine($"Common preparation: assessment earned {earned:F1}s, base {w.Food.Time:F1}s; {w.Population} people, {w.Beds} beds, {w.YardLogs} yard logs.");
        var results=new List<object>();
        foreach(bool farFarm in new[]{false,true})foreach(bool farBakery in new[]{false,true})foreach(int ovens in new[]{1,2})
        {
            results.Add(Observe(World.LoadJson(baseline),farFarm,farBakery,ovens));
            File.WriteAllText($"{Folder}/results.json",JsonSerializer.Serialize(results,new JsonSerializerOptions{WriteIndented=true}));
        }
        var recoveries=new List<object>();
        foreach(string change in new[]{"keep","add","move"})recoveries.Add(Recover(change));
        File.WriteAllText($"{Folder}/recovery.json",JsonSerializer.Serialize(recoveries,new JsonSerializerOptions{WriteIndented=true}));
    }
    static object Recover(string change)
    {
        var w=World.LoadFile($"{Folder}/farm-central_bakery-east_1.json");float start=w.Food.Time;
        int baked=w.Food.BakedBread,eaten=w.Food.EatenBread;
        if(change=="add")Build(w,new(22,0),BuildingKind.Bakery);
        if(change=="move")
        {
            var site=w.Cottages.Single(c=>c.Kind==BuildingKind.Bakery);Check(w.RequestDemolition(site.Id),"bakery recovery order");
            Until(w,()=>!w.Cottages.Contains(site),"recover bakery materials");Build(w,new(1,5),BuildingKind.Bakery);
        }
        float? built=null,ready=null;var samples=new List<object>();
        for(int tick=0;w.Food.Time<start+1800;tick++)
        {
            w.Tick(.1f);
            if(built==null && w.Cottages.All(c=>c.Complete))built=w.Food.Time-start;
            if(ready==null && w.CanCelebrate)ready=w.Food.Time-start;
            if(tick%1200==0 || w.Food.Time>=start+1800){w.Validate();samples.Add(new{seconds=w.Food.Time-start,bread=w.CentralFoodAvailable(Resource.Bread),baked=w.Food.BakedBread-baked,eaten=w.Food.EatenBread-eaten,meal=w.ReadMealAssessment()});}
        }
        Console.WriteLine($"Recovery {change}: construction/recovery {built:F1}s, ready {ready?.ToString("F1")??"never"}s, bread {w.CentralFoodAvailable(Resource.Bread)}.");
        File.WriteAllText($"{Folder}/recovery-{change}.json",w.SaveJson());
        return new{change,built,ready,newConstructionLogs=change=="keep"?0:Buildings.Get(BuildingKind.Bakery).Cost,demolishedLogs=change=="move"?Buildings.Get(BuildingKind.Bakery).Cost:0,samples};
    }
    static object Observe(World w,bool farFarm,bool farBakery,int ovens)
    {
        string key=$"farm-{(farFarm?"east":"central")}_bakery-{(farBakery?"east":"central")}_{ovens}";
        float start=w.Food.Time;int baselineBaked=w.Food.BakedBread,baselineEaten=w.Food.EatenBread;
        Build(w,farFarm?new(22,7):new(-2,0),BuildingKind.Farm);
        var bakeryCells=farBakery?new[]{new Cell(8,10),new Cell(22,0)}:new[]{new Cell(1,5),new Cell(-6,4)};
        foreach(var cell in bakeryCells.Take(ovens))
        {Until(w,()=>w.PlacementProblem(cell,0,BuildingKind.Bakery)==null,$"{key} bakery access");Build(w,cell,BuildingKind.Bakery);}
        var samples=new List<object>();float? ready=null,constructed=null;string? readySave=null;
        for(int tick=0;w.Food.Time<start+1800;tick++)
        {
            w.Tick(.1f);
            if(constructed==null && w.Cottages.All(c=>c.Complete))constructed=w.Food.Time-start;
            if(ready==null && w.CanCelebrate){ready=w.Food.Time-start;readySave=w.SaveJson();}
            if(tick%1200==0 || w.Food.Time>=start+1800)
            {
                w.Validate();samples.Add(new{seconds=w.Food.Time-start,central=w.CentralFoodAvailable(Resource.Bread),storedBread=w.StoredFood(Resource.Bread),grain=w.Food.Grain,baked=w.Food.BakedBread-baselineBaked,eaten=w.Food.EatenBread-baselineEaten,
                    meal=w.ReadMealAssessment(),work=w.Cottages.Where(c=>c.Kind is BuildingKind.Farm or BuildingKind.Bakery).Select(c=>new{c.Kind,c.Cell,report=w.ReadWorkplace(c)}).ToArray()});
            }
        }
        File.WriteAllText($"{Folder}/{key}.json",w.SaveJson());
        float? completed=null;
        if(readySave!=null)
        {
            var supper=World.LoadJson(readySave);Check(supper.BeginSupper(),"Ready alternative cannot host supper");
            Until(supper,()=>supper.Food.SupperComplete,"alternative supper");Check(supper.Campaign!.Complete,"Alternative supper did not complete campaign");completed=supper.Food.Time-start;
        }
        Console.WriteLine($"{key}: built {constructed:F1}s, first ready {ready?.ToString("F1")??"never"}s, central {w.CentralFoodAvailable(Resource.Bread)}, baked {w.Food.BakedBread-baselineBaked}, eaten {w.Food.EatenBread-baselineEaten}.");
        return new{key,start,constructed,ready,completed,people=w.Population,roles=w.People.GroupBy(p=>p.Role).ToDictionary(g=>g.Key.ToString(),g=>g.Count()),producerLogCost=Buildings.Get(BuildingKind.Farm).Cost+ovens*Buildings.Get(BuildingKind.Bakery).Cost,samples};
    }
}

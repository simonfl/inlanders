using Inlanders.Simulation;
using System.Text.Json.Nodes;

static class QuarryChallengeExperiment
{
    static World Setup(int logs,int food,bool frontier)
    {
        var data=JsonNode.Parse(World.NewCampaign(8).SaveJson())!;
        data["InitialLogs"]=data["InitialLogs"]!.GetValue<int>()-16+logs;
        data["Stored"]=logs;data["Food"]!["Berries"]=food;data["Food"]!["InitialBerries"]=food;
        if(frontier)
        {
            var buildings=data["Buildings"]!.AsArray();
            var garden=buildings.First(b=>b!["Kind"]!.GetValue<int>()==(int)BuildingKind.VegetableGarden)!;
            buildings.Remove(garden);data["InitialLogs"]=data["InitialLogs"]!.GetValue<int>()-4;
            var bushes=data["Bushes"]!.AsArray();bushes.RemoveAt(1);
        }
        return World.LoadJson(data.ToJsonString());
    }
    public static void Run()
    {
        Directory.CreateDirectory("artifacts");
        foreach(var (name,logs,food,frontier,strategy) in new[]{
            ("baseline",16,72,false,"none"),("low reserves",4,16,false,"none"),("almost empty",4,8,false,"none"),
            ("food frontier untreated",16,24,true,"none"),("food frontier garden",16,24,true,"garden"),
            ("food frontier bread",16,24,true,"bread"),("food frontier recovery",16,24,true,"late-garden")})
        foreach(bool near in new[]{true,false})
        {
            var w=Setup(logs,food,frontier);var misses=new HashSet<int>();
            float hallTime=0;int lowFood=food;bool intervened=false;
            var actions=new List<string>();
            void Build(Cell cell,BuildingKind kind)
            {
                if(w.Place(cell,0,kind)==null)throw new Exception($"{name}: cannot build {kind}: {w.PlacementProblem(cell,0,kind)}");
                actions.Add($"{w.Food.Time:F0}s plan {kind}");
            }
            void FoodPlan()
            {
                if(strategy=="bread")
                {Build(new(2,0),BuildingKind.Farm);Build(new(-3,-1),BuildingKind.Bakery);w.Assign(4,Role.Baker);actions.Add($"{w.Food.Time:F0}s forager to baker");}
                else Build(new(2,0),BuildingKind.VegetableGarden);
                intervened=true;
            }
            if(strategy is "garden" or "bread")FoodPlan();
            if(near)Build(new(-4,-4),BuildingKind.Quarry);
            Build(new(3,-5),BuildingKind.Sawmill);Build(new(11,-4),BuildingKind.Quarry);Build(new(0,-3),BuildingKind.GatheringHall);
            w.Assign(6,Role.Quarrier);w.Assign(7,Role.Sawyer);actions.Add("0s assign quarrier and sawyer");
            for(int i=0;i<18000 && !w.Campaign!.Complete;i++)
            {
                w.Tick(.1f);w.Validate();
                foreach(var m in w.Food.MealOutcomes.Where(m=>!m.Timely || m.Skipped))misses.Add(m.Request);
                lowFood=Math.Min(lowFood,World.EdibleKinds.Sum(w.StoredFood));
                if(w.Campaign.Quarry!.Phase==0 && w.CampaignHall?.Complete==true)
                {hallTime=w.Food.Time;w.AdvanceQuarryPhase();actions.Add($"{hallTime:F0}s begin assessment");}
                if(strategy=="late-garden" && !intervened && w.Food.Time>=600)
                {
                    if(w.Campaign!.Complete)throw new Exception("Recovery fixture completed before intervention");
                    if(w.QuarryServiceProblem()==null)throw new Exception("Recovery fixture has no service blocker");
                    Console.WriteLine($"  Before intervention at {w.Food.Time:F0}s: {w.QuarryServiceProblem()}");
                    string saved=w.SaveJson();w=World.LoadJson(saved);
                    if(w.SaveJson()!=saved)throw new Exception("Challenge recovery save differs");
                    FoodPlan();
                }
            }
            Console.WriteLine($"{name} | {(near?"two-camp":"remote-only")} | complete {w.Campaign!.Complete} | hall {hallTime:F0}s | end {w.Food.Time:F0}s | minimum stored food {lowFood} | missed/skipped {misses.Count} | logged commands {actions.Count} | food intervention {intervened} | {w.QuarryServiceProblem()}");
            Console.WriteLine("  "+string.Join("; ",actions));
            if(strategy!="none" && !w.Campaign!.Complete)throw new Exception($"Proposed {strategy} route does not recover");
        }
    }
}

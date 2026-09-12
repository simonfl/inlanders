using Inlanders.Simulation;
using System.Text.Json.Nodes;

static class WoodsCampaignChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    static void Until(World w,Func<bool> done,string why,int limit=24000)
    {
        for(int i=0;i<limit && !done();i++){w.Tick(.1f);if(i%50==0)w.Validate();}
        w.Validate();Check(done(),$"{why} failed at {w.Food.Time:F0}s: {w.WoodsObjective}");
    }
    static void Build(World w,Cell cell,BuildingKind kind)=>Check(w.Place(cell,0,kind)!=null,$"Cannot place {kind}: {w.PlacementProblem(cell,0,kind)}");
    static World Reload(World w)
    {
        string save=w.SaveJson();var copy=World.LoadJson(save);Check(copy.SaveJson()==save,"Woods exact save differs");
        for(int i=0;i<10;i++){w.Tick(.1f);copy.Tick(.1f);}Check(w.SaveJson()==copy.SaveJson(),"Woods saved continuation diverges");return copy;
    }
    static World Prepare(bool mixed,bool extra=false)
    {
        var w=World.NewCampaign(9);Check(!w.AdvanceWoodsPhase(),"First catch bypassed");
        foreach(var t in w.Trees.Where(t=>w.Map.Wildlife.Any(h=>h.Contains(t.Cell))))w.SetTreePreserved(t.Cell,true);
        if(mixed)
        {
            foreach(var cell in new[]{new Cell(-7,-3),new(-4,-3)})w.SetClearing(cell,true);
            Build(w,new(-3,0),BuildingKind.VegetableGarden);
        }
        Build(w,new(0,-3),BuildingKind.HuntingLodge);
        if(!mixed)Build(w,new(5,-3),BuildingKind.HuntingLodge);
        Build(w,new(2,-6),BuildingKind.Cottage);Build(w,new(5,0),BuildingKind.Cottage);
        w.Assign(6,Role.Hunter);w.Assign(7,mixed?Role.Farmer:Role.Hunter);
        Until(w,()=>w.DeliveredGame>=4,"first game delivery");
        Check(w.Campaign!.Woods!.Phase==0,"Delivery auto-advanced player commitment");
        w=Reload(w);
        if(mixed && !extra)File.WriteAllText("artifacts/woods-catch.json",w.SaveJson());
        Check(w.AdvanceWoodsPhase(),"Delivered game rejected");
        Check(!w.AdvanceWoodsPhase(),"Eight residents accepted");
        Until(w,()=>w.Beds>=12,"new homes");
        if(extra){Build(w,new(9,1),BuildingKind.Cottage);Until(w,()=>w.Beds>=14,"optional home");}
        for(int pair=0;pair<(extra?3:2);pair++)
        {
            Until(w,()=>w.InvitationProblem()==null,"newcomer provisions");Check(w.InviteNewcomers(),"Newcomers rejected");
        }
        Until(w,()=>w.WoodsActionProblem()==null,"supported habitat before assessment");
        w=Reload(w);Check(w.Campaign!.Woods!.Phase==1 && !w.Campaign.Complete,"Preparation auto-completed");
        return w;
    }
    static void Start(World w)
    {
        Check(w.AdvanceWoodsPhase(),"Assessment rejected");
        Check(!w.AdvanceWoodsPhase(),"Assessment restart accepted");
        Check(w.Campaign!.Woods!.AssessmentStarted==w.Food.Time,"Assessment time not captured");
        Check(!w.ReadMealAssessment(w.Campaign.Woods.AssessmentStarted).Reliable,"Pre-assessment meals counted");
    }
    public static void Run()
    {
        Directory.CreateDirectory("artifacts");
        Check(World.CampaignLevels.Select(l=>l.Id).SequenceEqual(Enumerable.Range(1,World.CampaignLevels.Length)),"Campaign IDs duplicate or out of order");
        var initial=Reload(World.NewCampaign(9));
        void Reject(Action<JsonObject> mutate)
        {
            var j=JsonNode.Parse(initial.SaveJson())!.AsObject();mutate(j);bool rejected=false;
            try{World.LoadJson(j.ToJsonString());}catch{rejected=true;}Check(rejected,"Malformed woods state accepted");
        }
        Reject(j=>j["Campaign"]!["Woods"]=null);
        Reject(j=>j["Campaign"]!["Woods"]!["Phase"]=4);
        Reject(j=>j["Campaign"]!["Woods"]!["AssessmentStarted"]=100);
        Reject(j=>j["Campaign"]!["Woods"]!["Phase"]=1);
        foreach(bool mixed in new[]{false,true})foreach(bool extra in new[]{false,true})
        {
            var w=Prepare(mixed,extra);
            if(mixed && !extra)File.WriteAllText("artifacts/woods-prepared.json",w.SaveJson());
            Start(w);w=Reload(w);
            if(mixed && !extra)File.WriteAllText("artifacts/woods-assessing.json",w.SaveJson());
            if(mixed)
            {
                Until(w,()=>w.AvailableGame(w.Map.Wildlife[0])<2,"selective woodland hunting pressure");
                Check(!w.Campaign!.Complete,"Depleted route unexpectedly complete");
                Console.WriteLine($"Mixed route responds to west hunting pressure at {w.Food.Time:F0}s by resting the lodge; cultivation continues.");
                if(!extra)File.WriteAllText("artifacts/woods-depleted.json",w.SaveJson());
                foreach(var lodge in w.Cottages.Where(c=>c.Kind==BuildingKind.HuntingLodge))w.SetWorkplacePaused(lodge.Id,true);
                w=Reload(w);
            }
            Until(w,()=>w.Campaign!.Complete,"full woodland assessment");
            Check(w.WoodsServiceProblem()==null && w.Campaign!.Woods!.Phase==3,"Completed with unsupported village");
            Check(w.Food.EatenGame>0,"No actual game meals");
            Console.WriteLine($"Woods {(mixed?"mixed":"preserved")}, {w.Population} residents: assessment {w.Campaign!.Woods!.AssessmentStarted:F0}s, complete {w.Food.Time:F0}s; trees {string.Join('/',w.Map.Wildlife.Select(w.HabitatTrees))}, available game {string.Join('/',w.Map.Wildlife.Select(w.AvailableGame))}.");
            w=Reload(w);if(mixed && !extra)File.WriteAllText("artifacts/woods-complete.json",w.SaveJson());
            var book=new CampaignBook();book.Capture(w);Check(book.Completed.Contains(9),"Completion not recorded");
        }
        GrowDuringAssessment();RecoverClearing();RecoverStock();
        Console.WriteLine("PASS: woodland campaign routes, optional growth, real assessment history, exact phase saves, claims and recovery.");
    }
    static void GrowDuringAssessment()
    {
        var w=Prepare(false);Start(w);float started=w.Campaign!.Woods!.AssessmentStarted;
        Build(w,new(9,1),BuildingKind.Cottage);
        Until(w,()=>w.Beds>=14 && w.InvitationProblem()==null,"mid-assessment growth preparation");
        Check(!w.Campaign.Complete && w.InviteNewcomers(),"Mid-assessment arrival fixture too late");
        Check(!w.ReadMealAssessment(started).Reliable,"New residents inherited older residents' meal proof");
        w=Reload(w);Until(w,()=>w.Campaign!.Complete,"new residents' actual meals");
        Check(w.Population==14 && w.Housed==14 && w.Campaign!.Woods!.AssessmentStarted==started,"Growth bypassed homes or reset earned progress");
        Console.WriteLine($"Mid-assessment arrival completes with fourteen housed at {w.Food.Time:F0}s.");
    }
    static void RecoverClearing()
    {
        var w=Prepare(true);Start(w);
        var west=w.Map.Wildlife[0];var cells=w.Trees.Where(t=>west.Contains(t.Cell)).Select(t=>t.Cell).ToArray();
        foreach(var cell in cells)w.SetClearing(cell,true);
        Until(w,()=>w.HabitatTrees(west)==0,"over-clearing");
        Check(!w.Campaign!.Complete && w.WoodsServiceProblem()!.Contains("plant"),"Missing habitat not explained");
        File.WriteAllText("artifacts/woods-cleared.json",w.SaveJson());
        float assessment=w.Campaign.Woods!.AssessmentStarted;
        Until(w,()=>cells.All(c=>w.Trees.All(t=>t.Cell!=c)),"timber and root collection");
        foreach(var cell in cells.Take(4)){Check(w.PlantTree(cell)!=null,"Restoration planting rejected");Check(w.SetTreePreserved(cell,true),"Cannot preserve planting order");}
        w=Reload(w);west=w.Map.Wildlife[0];
        Check(w.HabitatTrees(west)==0 && !w.Campaign!.Complete,"Saplings counted as mature habitat");
        File.WriteAllText("artifacts/woods-restoring.json",w.SaveJson());
        // Rest the depleted hunting ground while the existing cultivation supports the village.
        foreach(var lodge in w.Cottages.Where(c=>c.Kind==BuildingKind.HuntingLodge))w.SetWorkplacePaused(lodge.Id,true);
        Until(w,()=>w.Campaign!.Complete,"saved woodland restoration");
        Check(w.Campaign!.Woods!.AssessmentStarted==assessment && w.HabitatTrees(west)>=4,"Recovery reset progress or bypassed tree maturity");
        Console.WriteLine($"Clearing recovery completes at {w.Food.Time:F0}s, original assessment {assessment:F0}s retained.");
    }
    static void RecoverStock()
    {
        var w=Prepare(false);var west=w.Map.Wildlife[0];
        // Controlled source boundary: stock is environmental, not accounted food. Existing claims must remain valid.
        int claims=w.People.Where(p=>p.HabitatId==west.Id).Sum(p=>p.Reserved);
        west.Stock=claims+1.9f;
        Check(w.AvailableGame(west)==1 && w.WoodsActionProblem()!.Contains("Pause hunting"),"Claims/fractional stock counted twice or wrong remedy");
        Check(!w.AdvanceWoodsPhase(),"Insufficient unclaimed stock accepted");
        foreach(var lodge in w.Cottages.Where(c=>c.Kind==BuildingKind.HuntingLodge))w.SetWorkplacePaused(lodge.Id,true);
        w=Reload(w);west=w.Map.Wildlife[0];
        Until(w,()=>w.WoodsActionProblem()==null,"hunting pause recovery");Start(w);
        Until(w,()=>w.Campaign!.Complete,"rested woods and cultivation");
        Check(w.HabitatTrees(west)==6,"Hunting recovery required tree changes");
        Console.WriteLine($"Stock-only recovery completes at {w.Food.Time:F0}s with six west trees retained.");
    }
}

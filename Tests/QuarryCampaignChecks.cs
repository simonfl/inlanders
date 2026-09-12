using Inlanders.Simulation;

static class QuarryCampaignChecks
{
    static void Check(bool ok,string message){if(!ok)throw new Exception(message);}
    static void Until(World w,Func<bool> done,string label,int ticks=24000)
    {
        for(int i=0;i<ticks && !done();i++){w.Tick(.1f);w.Validate();}
        Check(done(),$"{label} failed at {w.Food.Time:F0}s: {w.QuarryServiceProblem()}");
    }
    static World Build(bool near,bool paths,Cell hallCell)
    {
        var w=World.NewCampaign(8);
        Check(!w.AdvanceQuarryPhase(),"Empty village assessment accepted");
        if(near)Check(w.Place(new(-4,-4),0,BuildingKind.Quarry)!=null,"Near camp rejected");
        if(hallCell.X>10)Check(w.Place(new(-3,0),0,BuildingKind.Square)!=null,"Competing central square rejected");
        foreach(var (cell,kind) in new[]{(new Cell(3,-5),BuildingKind.Sawmill),(new Cell(11,-4),BuildingKind.Quarry),(hallCell,BuildingKind.GatheringHall)})
            Check(w.Place(cell,0,kind)!=null,$"Rejected {kind} at {cell}");
        if(paths)
        {
            for(int x=-6;x<=15;x++)w.SetPath(new(x,1),true);
            foreach(int x in new[]{-6,-2,15})for(int z=-5;z<=3;z++)w.SetPath(new(x,z),true);
        }
        w.Assign(6,Role.Quarrier);w.Assign(7,Role.Sawyer);
        Until(w,()=>w.CampaignHall?.Complete==true,"hall construction");
        if(near && !paths && hallCell==new Cell(0,-3))File.WriteAllText("artifacts/quarry-prepared.json",w.SaveJson());
        Check(w.ReadCampaignConditions().Where(c=>c.Key.StartsWith("hall-")).All(c=>c.Met),"Already-used hall materials do not count");
        Check(w.AdvanceQuarryPhase(),"Completed hall cannot start assessment");
        Check(!w.AdvanceQuarryPhase(),"Assessment restarted");
        string saved=w.SaveJson();var copy=World.LoadJson(saved);
        for(int i=0;i<100;i++){w.Tick(.1f);copy.Tick(.1f);}
        Check(w.SaveJson()==copy.SaveJson(),"Assessment reload diverged");return w;
    }
    public static void Run()
    {
        Directory.CreateDirectory("artifacts");
        void Reject(Action<System.Text.Json.Nodes.JsonNode> change)
        {
            var data=System.Text.Json.Nodes.JsonNode.Parse(World.NewCampaign(8).SaveJson())!;change(data);
            bool rejected=false;try{World.LoadJson(data.ToJsonString());}catch(InvalidOperationException){rejected=true;}
            Check(rejected,"Invalid quarry assessment save accepted");
        }
        Reject(j=>j["Campaign"]!["Quarry"]=null);
        Reject(j=>j["Campaign"]!["Quarry"]!["Phase"]=3);
        Reject(j=>j["Campaign"]!["Quarry"]!["AssessmentStarted"]=100);
        Reject(j=>j["Campaign"]!["Complete"]=true);
        foreach(bool near in new[]{true,false})foreach(bool paths in new[]{false,true})
        {
            var w=Build(near,paths,new(0,-3));
            Until(w,()=>w.Campaign!.Complete,"supported hall");
            Check(w.QuarryServiceProblem()==null,"Completion without current services");
            Check(World.LoadJson(w.SaveJson()).Campaign!.Complete,"Completion lost on reload");
            if(!near)Check(w.Map.StoneDeposits[0].Remaining==8,"Remote-only route mined nearby stone");
            else Check(w.Map.StoneDeposits[0].Remaining==0,"Two-camp route did not use nearby stone");
            if(near)Check(w.ReadResourceSurvey(new(SourceKind.Stone,0))!.Detail.Contains("Exhausted."),"Exhausted outcrop survey missing");
            Console.WriteLine($"Quarry {(near?"two-camp":"remote-only")}, paths {paths}: completed {w.Food.Time:F0}s; hall visits {w.RecentHallVisitors}, stone remaining {string.Join('/',w.Map.StoneDeposits.Select(d=>d.Remaining))}.");
            if(near && !paths)File.WriteAllText("artifacts/quarry-complete.json",w.SaveJson());
        }
        var rough=Build(false,false,new(12,1));
        for(int i=0;i<3600 && !rough.Campaign!.Complete;i++){rough.Tick(.1f);rough.Validate();}
        Check(!rough.Campaign!.Complete,"Distant hall did not create an adverse route");
        Console.WriteLine($"Distant hall failure at {rough.Food.Time:F0}s: {rough.QuarryServiceProblem()}");
        File.WriteAllText("artifacts/quarry-rough.json",rough.SaveJson());
        int oldHall=rough.CampaignHall!.Id;
        int square=rough.Cottages.Single(c=>c.Kind==BuildingKind.Square).Id;
        Check(rough.RequestDemolition(square),"Cannot remove competing square");
        Check(rough.RequestDemolition(oldHall),"Cannot recover distant hall");
        Until(rough,()=>rough.Cottages.All(c=>c.Id!=oldHall && c.Id!=square),"hall recovery demolition");
        Check(rough.Place(new(0,-3),0,BuildingKind.GatheringHall)!=null,"Central recovery hall rejected");
        rough=World.LoadJson(rough.SaveJson());
        Until(rough,()=>rough.Campaign!.Complete,"central hall recovery");
        Console.WriteLine($"Distant hall rebuilt centrally: recovered at {rough.Food.Time:F0}s.");
        Console.WriteLine("PASS: quarry campaign routes, paths, actual service, exact phase saves, used materials and poor-placement recovery.");
    }
}

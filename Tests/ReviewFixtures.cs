using Inlanders.Simulation;

// Explicit development preparation, never part of ordinary campaign startup.
static class ReviewFixtures
{
    public static void Run(string[] args)
    {
        int at=Array.IndexOf(args,"--review-fixture");
        string name=args[at+1],path=args[at+2];
        if(name=="check")
        {
            var loaded=World.LoadFile(path);loaded.Validate();
            if(World.LoadJson(loaded.SaveJson()).SaveJson()!=loaded.SaveJson())throw new Exception("Review snapshot roundtrip differs");
            Console.WriteLine($"PASS: review snapshot {loaded.Population} residents, {loaded.Cottages.Count} buildings");return;
        }
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);Directory.CreateDirectory("artifacts");
        World w=name switch
        {
            "opening"=>World.NewCampaign(1),
            "river"=>World.NewCampaign(6),
            "shared-work"=>World.NewSharedWorkExperiment(),
            "ordinary"=>FinaleCampaignChecks.RunRoute(true,false,false),
            "dense"=>Inlanders.Development.ReviewWorlds.Dense(FinaleCampaignChecks.RunRoute(true,false,true)),
            _=>throw new ArgumentException("Unknown review scenario: "+name)
        };
        w.Validate();string json=w.SaveJson();
        if(World.LoadJson(json).SaveJson()!=json)throw new Exception("Prepared snapshot roundtrip differs");
        File.WriteAllText(path,json);
        Console.WriteLine($"PREPARED {name}: {w.Population} residents, {w.Cottages.Count} buildings, {w.Food.Time:F1} simulation seconds");
    }
}

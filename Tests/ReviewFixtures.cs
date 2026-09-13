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
            "creative-court"=>World.NewCreativeCourt(),
            "creative-court-expanded"=>World.CreativeCourtFrom(CourtChecks.Expanded()),
            "court-life"=>CreativeCourtChecks.Arranged(),
            "court-experience"=>World.NewCourtExperience(true),
            "creative-court-arranged"=>CreativeCourtChecks.Arranged(),
            "willow-court"=>World.NewArrangementCourt(),
            "willow-court-expanded"=>CourtChecks.Expanded(),
            "neighborhood-inherited"=>World.NewInheritedShoreline(),
            "commons-open" or "commons-untouched" or "commons-recurring" or "commons-event"=>CommonsComparison(name),
            "gathering"=>GatheringChecks.Prepare(),
            "opening"=>World.NewCampaign(1),
            "river"=>World.NewCampaign(6),
            "shared-work"=>World.NewSharedWorkExperiment(),
            "neighborhood"=>World.NewNeighborhoodExperiment(),
            "neighborhood-landscape"=>World.NewNeighborhoodLandscapeExperiment(),
            "neighborhood-complete"=>WelcomeMealChecks.PrepareCompletedReview(),
            "neighborhood-awkward"=>WelcomeMealChecks.PrepareAwkwardReview(),
            "neighborhood-workplace-food"=>World.NewWorkplaceFoodExperiment(),
            "neighborhood-working-village"=>WorkplaceFoodChecks.PrepareVillage(),
            "neighborhood-food-land"=>World.NewFoodLandChallenge(),
            "welcome-meal"=>WelcomeMealChecks.PrepareReview(),
            "ordinary"=>FinaleCampaignChecks.RunRoute(true,false,false),
            "dense"=>Inlanders.Development.ReviewWorlds.Dense(FinaleCampaignChecks.RunRoute(true,false,true)),
            _=>throw new ArgumentException("Unknown review scenario: "+name)
        };
        w.Validate();string json=w.SaveJson();
        if(World.LoadJson(json).SaveJson()!=json)throw new Exception("Prepared snapshot roundtrip differs");
        File.WriteAllText(path,json);
        Console.WriteLine($"PREPARED {name}: {w.Population} residents, {w.Cottages.Count} buildings, {w.Food.Time:F1} simulation seconds");
    }
    private static World CommonsComparison(string name)
    {
        var w=GatheringChecks.Prepare();
        var center=w.Map.Land.Where(c=>w.CommonsProblem(c)==null).OrderBy(c=>(c.Point-(name=="commons-open"?new Cell(21,10):new Cell(17,9)).Point).LengthSquared()).First();
        if((name=="commons-recurring" || name=="commons-open") && !w.SetCommons(center))throw new Exception("Comparison commons refused");
        if(name=="commons-event" && !w.BeginGathering(center))throw new Exception("Comparison event refused");
        return w;
    }
}

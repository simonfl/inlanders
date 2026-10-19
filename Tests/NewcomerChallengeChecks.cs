using Inlanders.Simulation;
static class NewcomerChallengeChecks
{
    public static World Solve(bool relaxed,bool northern=false)
    {
        var w=World.NewNewcomerChallenge(relaxed);
        if(w.InviteChallengeNeighbors())throw new Exception("Unprepared invitation accepted");
        w.ApplyWoodlandCare(new(-10,-12),new(-5,13),WoodlandIntent.Renew,false);
        void Place(BuildingKind kind,Cell at){var p=ReviewPlacement.Find(w,kind,at,(_,_)=>true,"challenge")??throw new Exception("No site "+kind);if(w.Place(p.Actual,p.Rotation,kind)==null)throw new Exception("Placement rejected");}
        if(northern)
        {
            if(w.Place(new(5,0),0,BuildingKind.Bridge)==null)throw new Exception("Bridge: "+w.PlacementProblem(new(5,0),0,BuildingKind.Bridge));
            Place(BuildingKind.Cottage,new(0,-8));Place(BuildingKind.Cottage,new(5,-9));
            Place(BuildingKind.VegetableField,new(1,-5));Place(BuildingKind.VegetableField,new(5,-5));
        }
        else
        {
            Place(BuildingKind.Cottage,new(-2,3));Place(BuildingKind.Cottage,new(2,3));
            Place(BuildingKind.VegetableField,new(1,-5));Place(BuildingKind.FishingDock,new(8,8));
        }
        for(int i=0;i<18000 && w.Challenge!.InvitedAt==null;i++){w.Tick(.1f);if(w.ChallengeInvitationProblem()==null)w.InviteChallengeNeighbors();}
        if(w.Challenge!.InvitedAt==null)throw new Exception("Invitation stalled: "+w.ChallengeInvitationProblem());
        var copy=World.LoadJson(w.SaveJson());
        for(int i=0;i<50;i++){w.Tick(.1f);copy.Tick(.1f);}if(w.SaveJson()!=copy.SaveJson())throw new Exception("Invitation save diverged");
        for(int i=0;i<18000 && !w.Challenge.Complete;i++)w.Tick(.1f);
        if(!w.Challenge.Complete)throw new Exception($"Not complete: housed{w.Housed} meals{w.ChallengeNeighborsFed} delivered{w.ChallengeDeliveries} food{w.EdibleStored}");
        if(w.PublicPlace!.Create().Challenge==null || World.LoadJson(w.SaveJson()).SaveJson()!=w.SaveJson())throw new Exception("Challenge identity/save");
        Directory.CreateDirectory("artifacts");w.SaveFile($"artifacts/215-challenge-{relaxed}-{northern}.json");
        Console.WriteLine($"PASS newcomer challenge relaxed={relaxed} northern={northern} completes at {w.Food.Time:0}s with actual construction, arrival, housing, deliveries and two meals per newcomer.");return w;
    }
    public static void Run(){Solve(false);Solve(false,true);Solve(true);Solve(true,true);}
}

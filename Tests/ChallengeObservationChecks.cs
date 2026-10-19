using Inlanders.Simulation;
static class ChallengeObservationChecks
{
    public static void Run()
    {
        var w=World.NewNewcomerChallenge(repair:true);
        w.ApplyWoodlandCare(new(-10,2),new(-5,13),WoodlandIntent.Renew,false);
        foreach(var at in new[]{new Cell(-2,3),new Cell(2,3)}){var p=ReviewPlacement.Find(w,BuildingKind.Cottage,at,(c,r)=>c.Z>0,"homes")!;w.Place(p.Actual,p.Rotation,BuildingKind.Cottage);}
        if(w.Place(new(5,0),0,BuildingKind.Bridge)==null)throw new Exception("Crossing refused");
        bool arrivalSaved=false;
        for(int i=0;i<6000 && !w.Challenge!.Complete;i++)
        {
            w.Tick(.1f);
            if(w.ChallengeInvitationProblem()==null){w.InviteChallengeNeighbors();w.SaveFile("artifacts/220-invited.json");}
            if(w.Challenge!.Arrived && !arrivalSaved){arrivalSaved=true;w.SaveFile("artifacts/220-arrived.json");}
        }
        if(!w.Challenge!.Complete || !arrivalSaved)throw new Exception("Observation journey stalled");
        if(World.LoadJson(w.SaveJson()).SaveJson()!=w.SaveJson())throw new Exception("Complete current save differs");
        w.SaveFile("artifacts/220-complete.json");Console.WriteLine($"PASS actual invitation/arrival/complete fixtures at {w.Food.Time:0}s");
    }
}

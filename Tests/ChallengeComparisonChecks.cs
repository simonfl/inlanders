using Inlanders.Simulation;
using System.Text.Json;
static class ChallengeComparisonChecks
{
    public static void Run()
    {
        var results=new List<object>();
        foreach(string choice in new[]{"homes-only","one-field","far-field","two-fields-control"})
        {
            var w=World.NewNewcomerChallenge();w.TraceSharedWork=true;
            var events=new List<string>();float? invited=null;int misses=0;
            void Place(BuildingKind kind,Cell at){var p=ReviewPlacement.Find(w,kind,at,(_,_)=>true,choice)??throw new Exception("No site "+kind);w.Place(p.Actual,p.Rotation,kind);events.Add($"{w.Food.Time:0}s place {kind} at {p.Actual}");}
            w.ApplyWoodlandCare(new(-10,-12),new(-5,13),WoodlandIntent.Renew,false);
            Place(BuildingKind.Cottage,new(-2,3));Place(BuildingKind.Cottage,new(2,3));
            if(choice!="homes-only")Place(BuildingKind.VegetableField,choice=="far-field"?new(1,-12):new(1,-5));
            if(choice=="two-fields-control")Place(BuildingKind.VegetableField,new(5,-5));
            float walked=0;
            for(int i=0;i<12000 && !w.Challenge!.Complete;i++)
            {
                var positions=w.People.Select(p=>p.Position).ToArray();w.Tick(.1f);
                walked+=w.People.Take(positions.Length).Select((p,j)=>(p.Position-positions[j]).Length()).Sum();
                if(w.People.Any(p=>!p.Fed))misses++;
                if(w.ChallengeInvitationProblem()==null){w.InviteChallengeNeighbors();invited=w.Food.Time;events.Add($"{w.Food.Time:0}s invite");}
            }
            results.Add(new{choice,complete=w.Challenge!.Complete,time=w.Food.Time,invited,missedMealSeconds=misses*.1,walked,delivered=w.ChallengeDeliveries,problem=w.ChallengeSettlingProblem(),events});
            Console.WriteLine($"COMPARE {choice} complete={w.Challenge.Complete} time={w.Food.Time:0} missed={misses*.1:0}s walked={walked:0} deliveries={w.ChallengeDeliveries}");
            w.SaveFile($"artifacts/216-{choice}.json");
        }
        File.WriteAllText("artifacts/216-comparison.json",JsonSerializer.Serialize(results,new JsonSerializerOptions{WriteIndented=true}));
    }
}

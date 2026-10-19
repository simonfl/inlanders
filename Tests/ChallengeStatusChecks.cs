using Inlanders.Simulation;
static class ChallengeStatusChecks
{
    public static void Run()
    {
        var w=World.LoadFile("artifacts/220-arrived.json");var c=w.Challenge!;
        foreach(var person in w.People.Where(p=>p.Id>=12))c.Meals[person.Id]=2;
        foreach(var person in w.People)person.Fed=true;
        c.DeliveryBaseline=w.ChallengeDeliveries+c.DeliveryBaseline;
        if(!w.ChallengeStatus.StartsWith("Fresh food"))throw new Exception("Meal count concealed delivery goal");
        var newcomer=w.People[12];newcomer.HomeId=null;
        if(!w.ChallengeStatus.StartsWith("Homes") || !w.ChallengeNeedsHome(newcomer))throw new Exception("Housing blocker hidden");
        Console.WriteLine("PASS collapsed status follows remaining deliveries and reachable housing, not only two-meal count.");
    }
}

using Inlanders.Simulation;
static class FoodAccessChecks
{
    public static void Run()
    {
        var w=World.NewTransformationHamlet(true);var spot=new Cell(2,-1);string before=w.SaveJson();
        var routes=w.ReadFoodAccess(spot);if(routes.Length==0 || routes[0].Store.Id!=null || w.SaveJson()!=before)throw new Exception("Food survey source or purity differs");
        int longWay=routes[0].Route.Length;if(w.Place(new(2,0),0,BuildingKind.Bridge)==null)throw new Exception("Bridge refused");
        var shortWay=w.ReadFoodAccess(spot);if(shortWay[0].Route.Length>=longWay)throw new Exception("Survey ignored new crossing");
        if(w.ReadFoodAccess(w.Stockpile).Length!=0 || w.ReadFoodAccess(new(50,50)).Length!=0)throw new Exception("Survey routes through blocked ground");
        var distant=w.ReadFoodAccess(new(-1,13));
        if(distant.Length==0)throw new Exception("Food-access fixture needs reachable clear ground outside shared-place range");
        if(distant.First(r=>r.Store.Id==null).ServesSharedPlace)throw new Exception("Shared-place range ignored");
        var copy=World.LoadJson(w.SaveJson());if(!w.ReadFoodAccess(spot)[0].Route.SequenceEqual(copy.ReadFoodAccess(spot)[0].Route))throw new Exception("Restored access differs");
        Console.WriteLine($"PASS: pure food access query, actual crossing changes {longWay} to {shortWay[0].Route.Length} steps, blocked/outside rejection and real shared-place reach.");
    }
}

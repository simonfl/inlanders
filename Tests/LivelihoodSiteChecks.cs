using Inlanders.Simulation;
static class LivelihoodSiteChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    public static void Run()
    {
        var w=World.NewPlayerFounded();string before=w.SaveJson();var missing=w.ReadLivelihoodSite(new(1,-5),0,BuildingKind.Farm);
        Check(missing.Summary.Contains("oven") && missing.Route.Length==0 && before==w.SaveJson(),"Missing dependency preview incorrect/impure");
        var oven=w.Place(new(5,-3),0,BuildingKind.Bakery)!;before=w.SaveJson();var route=w.ReadLivelihoodSite(new(1,-5),0,BuildingKind.Farm);
        Check(route.Route.Length>1 && route.Route[^1]==oven.Entrance && route.Destination.Contains("Planned") && before==w.SaveJson(),"Planned grain link wrong/impure");
        Check(!route.Route.Any(World.Footprint(new(1,-5),0,BuildingKind.Farm).Contains),"Preview route crosses proposed footprint");
        var fish=w.ReadLivelihoodSite(new(8,8),1,BuildingKind.FishingDock);Check(fish.Route.Length>1 && fish.Route.All(w.Map.Water.Contains),"Dock route leaves water");
        Check(w.ReadLivelihoodSite(w.Stockpile,0,BuildingKind.Farm).Summary=="","Illegal site promises connection");
        Check(before==w.SaveJson(),"Siting queries changed world");Console.WriteLine("PASS: actual planned grain dependency, proposed footprint exclusion, water reachability and pure/invalid livelihood preview.");
    }
}

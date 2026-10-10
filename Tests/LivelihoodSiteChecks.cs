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
        Check(before==w.SaveJson(),"Siting queries changed world");var homes=World.NewPlayerFounded();var home=homes.PlaceHomePlot(new(1,8),0,BuildingKind.Cottage,0)!;
        string planned=homes.SaveJson();var gardenLink=homes.ReadLivelihoodSite(new(4,8),0,BuildingKind.VegetableGarden);
        Check(gardenLink.Destination=="Planned home" && gardenLink.Route[^1]==home.Entrance && planned==homes.SaveJson(),"Garden ignores planned home or changes state");
        var food=World.NewPlayerFounded();var garden=food.Place(new(4,8),0,BuildingKind.VegetableGarden)!;planned=food.SaveJson();
        for(int turn=0;turn<4;turn++)
        {
            var link=food.ReadLivelihoodSite(new(1,8),turn,BuildingKind.Cottage);
            Check(link.Destination=="Planned vegetable plot" && link.Route[^1]==garden.Entrance && !link.Route.Any(World.Footprint(new(1,8),turn,BuildingKind.Cottage).Contains),"Home-first/food-first link or rotated footprint wrong");
        }
        Check(planned==food.SaveJson(),"Home connection query mutates state");
        var river=World.NewRiverLivelihood();string riverBefore=river.SaveJson();
        for(int turn=0;turn<4;turn++)
        {
            var gathering=river.ReadLivelihoodSite(new(0,-7),turn,BuildingKind.ForagerHut);
            Check(gathering.Route.Length>1 && river.Bushes.Any(b=>b.Access==gathering.Route[^1]),"Gathering preview does not reach real berry access");
            Check(!gathering.Route.Any(World.Footprint(new(0,-7),turn,BuildingKind.ForagerHut).Contains),"Gathering route crosses proposed hut");
        }
        Check(riverBefore==river.SaveJson(),"Gathering preview changes world");
        river.Bushes.Clear();Check(river.ReadLivelihoodSite(new(0,-7),0,BuildingKind.ForagerHut).Summary.Contains("No reachable"),"Missing berry source is hidden");
        Console.WriteLine("PASS: actual planned grain dependency, proposed footprint exclusion, water reachability and pure/invalid livelihood preview.");
    }
}

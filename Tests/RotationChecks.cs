using Inlanders.Simulation;

public static class RotationChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    static void Until(World w,Func<bool> done,string why)
    {for(int i=0;i<12000 && !done();i++){w.Tick(.1f);w.Validate();}Check(done(),why);}
    public static void Run()
    {
        var origin=new Cell(0,0);var doors=new[]{new Cell(0,1),new(1,0),new(0,-1),new(-1,0)};
        for(int r=0;r<4;r++)
        {
            Check(World.Door(origin,r)==doors[r],"Wrong facing entrance");
            var footprint=World.Footprint(origin,r).ToHashSet();
            Check(footprint.Count==6 && !footprint.Contains(doors[r]) && footprint.Contains(doors[(r+2)%4]),"Rotated footprint does not sit behind entrance");
            foreach(var kind in Enum.GetValues<BuildingKind>().Where(k=>k is not (BuildingKind.FishingDock or BuildingKind.Bridge)))
            {
                var creative=World.NewCreative(true);
                var cell=creative.Map.Land.First(c=>creative.PlacementProblem(c,r,kind)==null);
                var site=creative.Place(cell,r,kind)!;creative.Validate();
                Check(site.Rotation==r && World.LoadJson(creative.SaveJson()).SaveJson()==creative.SaveJson(),"Orientation save changed");
                Check(creative.RemoveBuilding(site.Id),"Rotated building removal failed");creative.Validate();
            }
            var w=new World();var at=w.Map.Land.First(c=>w.PlacementProblem(c,r)==null);var home=w.Place(at,r)!;
            Until(w,()=>home.Complete,"Normal rotated construction stalled");
            Until(w,()=>w.People.Any(p=>p.HomeId==home.Id && p.RestVisits>0),"Rotated home rest stalled");
            Check(w.RequestDemolition(home.Id),"Rotated demolition rejected");Until(w,()=>!w.Cottages.Contains(home),"Rotated demolition stalled");
            var bridgeWorld=new World();var crossing=new Cell(3,-2);bridgeWorld.Map.Water.Add(crossing);
            var bridge=bridgeWorld.Place(crossing,r,BuildingKind.Bridge);Check(bridge!=null,"Rotated bridge rejected");
            Until(bridgeWorld,()=>bridge!.Complete,"Rotated bridge build stalled");
            var lake=World.NewLakeMap();var shore=lake.Map.Land.First(c=>lake.PlacementProblem(c,r,BuildingKind.FishingDock)==null);
            var dock=lake.Place(shore,r,BuildingKind.FishingDock)!;
            Check(lake.Map.Water.Contains(dock.Launch) && !lake.Map.Water.Contains(dock.Entrance),"Dock launch or entrance faces wrong terrain");
            Until(lake,()=>dock.Complete,"Rotated dock construction stalled");lake.Assign(0,Role.Fisher);
            Until(lake,()=>lake.DeliveredFish>0,"Rotated dock fishing stalled");
            Check(World.LoadJson(lake.SaveJson()).SaveJson()==lake.SaveJson(),"Rotated boat save changed");
        }
        Check(new World().Place(origin,4)==null,"Out-of-range rotation accepted");
        Console.WriteLine("PASS: all building kinds in four orientations, footprints/entrances, exact saves, construction, rest, demolition, bridges and actual dock fishing.");
    }
}

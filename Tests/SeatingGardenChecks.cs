using Inlanders.Simulation;

public static class SeatingGardenChecks
{
    static void Check(bool ok,string why) { if(!ok) throw new Exception(why); }
    static void Step(World w,int n=1) { for(int i=0;i<n;i++) { w.Tick(.1f); w.Validate(); } }
    public static void Run()
    {
        foreach(bool rotated in new[]{false,true})
        {
            var w=World.NewCreative();
            Check(w.PlaceDecoration(new(2,-1),DecorationKind.Flowers),"Fixture flower rejected");
            Check(w.PlacementProblem(new(3,0),rotated,BuildingKind.Square)!=null,"Square should overlap planted ground");
            var garden=w.Place(new(3,0),rotated,BuildingKind.SeatingGarden);
            Check(garden!=null && World.Footprint(garden.Cell,rotated,garden.Kind).Count()==1,"Garden must fit the single free tile");
            Check(w.PlacementProblem(new(3,0),false,BuildingKind.SeatingGarden)!=null,"Garden footprint not occupied");
            for(int i=0;i<1600 && !w.People.Any(p=>p.Task==Work.Leisure);i++) Step(w);
            Check(w.People.Any(p=>p.Task==Work.Leisure),"Garden did not receive visitors");
            Check(w.People.Count(p=>p.LeisureSiteId==garden!.Id)<=2,"Garden exceeded capacity");
            string saved=w.SaveJson(); var copy=World.LoadJson(saved);
            Check(saved==copy.SaveJson(),"Garden visit save changed"); Step(w,200); Step(copy,200);
            Check(w.SaveJson()==copy.SaveJson(),"Garden resumed differently");
            Check(w.RemoveBuilding(garden!.Id),"Garden removal failed");
            Check(w.People.All(p=>p.LeisureSiteId!=garden.Id),"Removed garden retained visitor"); w.Validate();
        }
        var normal=new World(40); normal.Food.InitialBerries=normal.Food.Berries=1000;
        var built=normal.Place(new(3,0),false,BuildingKind.SeatingGarden)!;
        for(int i=0;i<3000 && !built.Complete;i++) Step(normal);
        Check(built.Complete && built.Delivered==4,"Garden construction must consume four delivered logs");
        for(int i=0;i<1000 && !normal.People.Any(p=>p.LastLeisureSiteId==built.Id);i++) Step(normal);
        Check(normal.People.Any(p=>p.LastLeisureSiteId==built.Id),"Garden did not give earned recreation");
        Check(normal.RequestDemolition(built.Id),"Garden demolition failed");
        for(int i=0;i<4000 && normal.Cottages.Any(c=>c.Id==built.Id);i++) Step(normal);
        Check(normal.Cottages.All(c=>c.Id!=built.Id),"Garden dismantling failed");
        Compare();
        Console.WriteLine("PASS: one-tile/rotated garden placement, real visits, capacity, construction, demolition, removal and exact active-visit saves.");
    }
    static void Compare()
    {
        foreach(bool gardens in new[]{false,true})
        {
            var w=World.NewCreative(true);
            foreach(var p in w.People) w.Assign(p.Id,Role.Unassigned);
            var a=w.Place(new(3,0),false,gardens?BuildingKind.SeatingGarden:BuildingKind.Square)!;
            Check(a!=null,"Comparison central venue rejected");
            // Same two population clusters in both runs; no claim about production gains.
            var far=w.Map.Land.Where(c=>c.X < -5 && c.Z < 0 && w.PlacementProblem(c,false,BuildingKind.SeatingGarden)==null)
                .OrderBy(c=>c.Z).ThenBy(c=>c.X).First();
            if(gardens) Check(w.Place(far,false,BuildingKind.SeatingGarden)!=null,"Far garden rejected");
            foreach(var p in w.People) p.Position=(p.Id<4?a!.Entrance:World.Door(far,false)).Point;
            float travel=0;
            for(int i=0;i<3000;i++) { travel+=w.People.Count(p=>p.Task==Work.ToLeisure)*.1f; Step(w); }
            Console.WriteLine($"SEATING comparison {(gardens?"two local gardens":"one central square")}: {w.People.Sum(p=>p.LeisureVisits)} visits, {travel:0.0} resident-seconds travelling, {w.People.Count(p=>p.LeisureVisits>0)}/8 residents served; {(gardens?8:6)} logs, {(gardens?2:6)} planted/building tiles plus open visit space.");
        }
    }
}

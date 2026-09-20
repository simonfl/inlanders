using Inlanders.Simulation;
static class HomeWaitingChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    static void Step(World w,int ticks){for(int i=0;i<ticks;i++){w.Tick(.1f);if(i%100==0)w.Validate();}}
    public static void Run()
    {
        var w=World.LoadFile("artifacts/river-farmstead/mixed.json");
        foreach(var t in w.Trees)w.SetTreePreserved(t.Cell,true);
        foreach(var c in w.Cottages.Where(c=>Buildings.Get(c.Kind).Worker!=null))w.SetWorkplacePaused(c.Id,true);
        Step(w,1600);
        Check(w.People.Any(p=>p.Status=="At home — available for work"),"Normal idle workers did not disperse home");
        Check(w.People.Where(p=>p.Status=="At home — available for work").All(p=>!w.Cottages.Any(c=>c.Entrance==World.At(p)) && World.At(p)!=w.YardAccess),"Idle waiting blocks an entrance");
        Directory.CreateDirectory("artifacts/home-waiting");w.SaveFile("artifacts/home-waiting/quiet.json");
        var home=w.Cottages.First(c=>c.Kind==BuildingKind.Cottage);
        var move=w.Map.Land.OrderBy(c=>(c.Point-new Cell(5,7).Point).LengthSquared()).First(c=>w.RelocationProblem(home.Id,c,0)==null);
        Check(w.MoveBuilding(home.Id,move,0),"Home move failed");Step(w,2);
        var walking=w.People.Where(p=>p.Task==Work.Waiting && p.Route.Count>0).ToArray();
        Check(walking.Length>0,"No homeward route after move");
        var copy=World.LoadJson(w.SaveJson());Step(w,10);Step(copy,10);Check(copy.SaveJson()==w.SaveJson(),"Homeward save diverged");
        // Restrict the shared pool to the walker so another resident cannot mask delayed work claiming.
        int chosen=walking[0].Id;foreach(var p in w.People.Where(p=>p.Id!=chosen))w.Assign(p.Id,Role.Farmer);
        var garden=w.Cottages.First(c=>c.Kind==BuildingKind.VegetableGarden);w.SetWorkplacePaused(garden.Id,false);
        foreach(var t in w.Trees)w.SetTreePreserved(t.Cell,false);
        var build=ReviewPlacement.Find(w,BuildingKind.SeatingGarden,new(0,4),(c,r)=>true,"new work")!;
        Check(w.Place(build.Actual,build.Rotation,BuildingKind.SeatingGarden)!=null,"New work rejected");
        Step(w,6);Check(w.People[chosen].Task!=Work.Waiting,"Homeward trip did not yield to available work");
        int before=w.Food.GrownVegetables;Step(w,1200);Check(w.Cottages.Any(c=>c.Kind==BuildingKind.SeatingGarden && c.Complete),"Home waiting delayed construction indefinitely");
        Check(w.Food.GrownVegetables>before,"Production did not resume");w.Validate();
        Console.WriteLine("PASS: normal home waiting, clear entrances, moving homes, active-route save continuation and new construction/production.");
    }
}

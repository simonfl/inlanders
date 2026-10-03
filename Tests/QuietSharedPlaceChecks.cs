using Inlanders.Simulation;
static class QuietSharedPlaceChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    public static World Quiet(bool relaxed=false)
    {
        var w=World.NewWorkingClearing(relaxed);Check(w.AddCommons(new(0,7),6),"Shared place");
        foreach(var t in w.Trees)w.SetTreePreserved(t.Cell,true);
        foreach(var c in w.Cottages.Where(c=>Buildings.Get(c.Kind).Worker!=null))w.SetWorkplacePaused(c.Id,true);
        for(int i=0;i<150;i++)w.Tick(.1f);return w;
    }
    public static World Active(bool relaxed=false,bool split=false)
    {
        var w=World.NewWorkingClearing(relaxed);Check(w.AddCommons(new(0,7),split?2:6),"Active shared ground");
        if(split)Check(w.AddCommons(new(0,1),4,SharedPlaceLayout.Line,1),"Active second place");
        var home=w.Cottages.Single(c=>c.Cell==new Cell(-3,9));Check(w.FurnishHomeYard(home.Id,0),"Active furnished yard");return w;
    }
    private static void OrdinaryLife()
    {
        foreach(bool relaxed in new[]{false,true})foreach(bool split in new[]{false,true})
        {
            var w=Active(relaxed,split);var visited=new HashSet<int>();int home=0,work=0,pairs=0,quiet=0;
            for(int i=0;i<6000;i++)
            {
                w.Tick(.1f);if(i%100==0)w.Validate();
                foreach(var p in w.People)
                {
                    if(w.QuietSharedPlace(p)!=null && p.Route.Count==0){visited.Add(p.Id);quiet++;}
                    if(w.QuietAtFurnishedHome(p))home++;
                    if(p.Task!=Work.Waiting && p.Role is Role.Farmer or Role.Builder or Role.Carpenter)work++;
                    if(w.QuietCompanion(p) is {} other){Check(w.QuietCompanion(other)==p,"Asymmetric companion");pairs++;}
                }
            }
            Check(visited.Any(id=>id%2==1),"Odd residents excluded");
            Check(w.People.Any(p=>visited.Contains(p.Id) && w.Cottages.Single(c=>c.Id==p.HomeId).Improved),"Furnished residents excluded");
            Check(home>0 && work>0 && w.Food.EatenVegetables>0 && quiet>0,"Ordinary life missing domestic/work/meal/shared activity");
            Check(w.People.Where(p=>p.QuietSharedCenter!=null).All(p=>p.QuietVisitUntil==0 || p.QuietVisitUntil-w.Food.Time<=33),"Unbounded quiet visit");
            var copy=World.LoadJson(w.SaveJson());for(int i=0;i<100;i++){w.Tick(.1f);copy.Tick(.1f);}Check(w.SaveJson()==copy.SaveJson(),"Active routine continuation");
            Directory.CreateDirectory("artifacts/ordinary-outdoor-life");w.SaveFile($"artifacts/ordinary-outdoor-life/{relaxed}-{split}.json");
            Console.WriteLine($"PASS ordinary outdoor life {relaxed}/{split}: quiet {quiet}, domestic {home}, work {work}, actual companion ticks {pairs}, residents {string.Join(',',visited.Order())}; active work, real meals and exact save");
        }
    }
    public static void Run()
    {
        OrdinaryLife();
        foreach(bool relaxed in new[]{false,true})
        {
            var displaced=Quiet(relaxed);var seated=displaced.People.First(p=>displaced.QuietSharedPlace(p)!=null && p.Route.Count==0);
            Check(relaxed?displaced.RemoveBuilding(seated.HomeId!.Value):displaced.RequestDemolition(seated.HomeId!.Value),"Remove visitor home");
            Check(seated.QuietSharedCenter==null,"Removed home retained seated quiet intent");
            displaced.Validate();
            var displacedCopy=World.LoadJson(displaced.SaveJson());
            for(int i=0;i<100;i++){displaced.Tick(.1f);displacedCopy.Tick(.1f);}
            Check(displaced.SaveJson()==displacedCopy.SaveJson(),"Displaced visitor continuation");
            var w=Quiet(relaxed);var visiting=w.People.FirstOrDefault(p=>w.QuietSharedPlace(p)!=null && p.Route.Count==0);
            Check(visiting!=null,"No actual quiet visit");Check(w.ReadDailyJourney(visiting!.Id).Heading.StartsWith("Sitting together"),"Quiet reader claims home/food");
            var copy=World.LoadJson(w.SaveJson());for(int i=0;i<50;i++){w.Tick(.1f);copy.Tick(.1f);}Check(w.SaveJson()==copy.SaveJson(),"Quiet save continuation");
            int meals=w.Food.MealConsumptions.Count;for(int i=0;i<900;i++){w.Tick(.1f);if(i%100==0)w.Validate();}
            Check(w.Food.MealConsumptions.Count>meals && w.Commons!.FirstDiner!=null,"Quiet visitors blocked actual meals");
            for(int i=0;i<1000 && !w.People.Any(p=>w.QuietSharedPlace(p)!=null);i++)w.Tick(.1f);
            var available=w.People.First(p=>w.QuietSharedPlace(p)!=null);
            foreach(var p in w.People.Where(p=>p.Id!=available.Id))w.Assign(p.Id,Role.Farmer);
            var build=ReviewPlacement.Find(w,BuildingKind.SeatingGarden,new(0,0),(c,r)=>true,"quiet work preemption")!;
            Check(w.Place(build.Actual,build.Rotation,BuildingKind.SeatingGarden)!=null,"New work placement");
            for(int i=0;i<6;i++)w.Tick(.1f);Check(available.Task!=Work.Waiting,"Quiet visit blocked new work");
            Check(w.RemoveCommons(),"Remove quiet place");Check(w.People.All(p=>w.QuietSharedPlace(p)==null),"Quiet visitor retained deleted place");
            for(int i=0;i<100;i++)w.Tick(.1f);w.Validate();
            Console.WriteLine($"PASS quiet shared place {relaxed}: real visits, meals/work preemption, removal and exact active save");
        }
    }
}

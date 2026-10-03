using Inlanders.Simulation;
static class QuietSharedPlaceChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    public static World Quiet(bool relaxed=false)
    {
        var w=World.NewWorkingClearing(relaxed);Check(w.AddCommons(new(0,7),6),"Shared place");
        foreach(var t in w.Trees)w.SetTreePreserved(t.Cell,true);
        foreach(var c in w.Cottages.Where(c=>Buildings.Get(c.Kind).Worker!=null))w.SetWorkplacePaused(c.Id,true);
        for(int i=0;i<400;i++)w.Tick(.1f);return w;
    }
    public static void Run()
    {
        foreach(bool relaxed in new[]{false,true})
        {
            var w=Quiet(relaxed);var visiting=w.People.FirstOrDefault(p=>w.QuietSharedPlace(p)!=null && p.Route.Count==0);
            Check(visiting!=null,"No actual quiet visit");Check(w.ReadDailyJourney(visiting!.Id).Heading.StartsWith("Sitting together"),"Quiet reader claims home/food");
            var copy=World.LoadJson(w.SaveJson());for(int i=0;i<50;i++){w.Tick(.1f);copy.Tick(.1f);}Check(w.SaveJson()==copy.SaveJson(),"Quiet save continuation");
            int meals=w.Food.MealConsumptions.Count;for(int i=0;i<900;i++){w.Tick(.1f);if(i%100==0)w.Validate();}
            Check(w.Food.MealConsumptions.Count>meals && w.Commons!.FirstDiner!=null,"Quiet visitors blocked actual meals");
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

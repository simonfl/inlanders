using Inlanders.Simulation;
static class YardArrangementChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    static void Until(World w,Func<bool> done,string why,int limit=16000){for(int i=0;i<limit && !done();i++){w.Tick(.1f);if(i%20==0)w.Validate();}Check(done(),why);w.Validate();}
    public static void Run()
    {
        Directory.CreateDirectory("artifacts/yard-arrangement");
        foreach(bool relaxed in new[]{false,true})foreach(int side in new[]{0,1,2,3})
        {
            var w=World.NewTransformationHamlet(relaxed,true);
            var h=w.Cottages.First(c=>c.Kind==BuildingKind.Cottage && w.HomeYardProblem(c.Id,side)==null);
            string before=w.SaveJson();w.HomeYardProblem(h.Id,side);Check(before==w.SaveJson(),"Yard query mutated world");
            Check(w.SetHomeYard(h.Id,side) && w.RequestImprovement(h.Id),"Yard order rejected");
            Until(w,()=>h.Improved,"Yard furnishing stalled");
            Until(w,()=>w.People.Any(p=>p.HomeId==h.Id && w.QuietAtFurnishedHome(p)),"No quiet use on side "+side);
            var copy=World.LoadJson(w.SaveJson());for(int i=0;i<100;i++){w.Tick(.1f);copy.Tick(.1f);}Check(copy.SaveJson()==w.SaveJson(),"Yard use reload differs");
            Until(w,()=>w.People.Any(p=>p.HomeId==h.Id && p.Task==Work.EatingMeal && w.AtFurnishedHome(p)),"No meal on chosen side "+side);
            if(!relaxed && side==2)w.SaveFile("artifacts/yard-arrangement/rear-meal.json");
            int next=Enumerable.Range(1,3).Select(i=>(side+i)%4).First(i=>w.HomeYardProblem(h.Id,i)==null);
            Check(w.SetHomeYard(h.Id,next),"Cannot rearrange an occupied yard");w.Validate();
            Until(w,()=>w.People.Any(p=>p.HomeId==h.Id && w.QuietAtFurnishedHome(p)),"Residents did not use rearranged ground");
            before=w.SaveJson();Check(!w.SetHomeYard(h.Id,8) && w.SaveJson()==before,"Rejected yard change mutated state");
        }
        Console.WriteLine("PASS: four yard sides in Normal/relaxed, real quiet work and meals, exact saves, occupied-yard rearrangement and rejection purity.");
    }
}

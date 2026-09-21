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
            Check(w.FurnishHomeYard(h.Id,side),"Combined yard order rejected");
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
        foreach(bool relaxed in new[]{false,true})
        {
            var w=World.NewTransformationHamlet(relaxed,true);
            var a=w.Cottages.Single(c=>c.Cell==new Cell(-3,6));var b=w.Cottages.Single(c=>c.Cell==new Cell(1,6));
            Check(w.SetHomeYard(a.Id,3) && w.SetHomeYard(b.Id,1),"Unfurnished ground selections should not reserve land");
            Check(w.RequestImprovement(a.Id),"First yard order failed");string before=w.SaveJson();
            Check(!w.RequestImprovement(b.Id) && !w.FurnishHomeYard(b.Id,1) && before==w.SaveJson(),"Overlapping yard order accepted or mutated world");
            Check(w.HomeYardProblem(b.Id,1)!=null,"Ordered yard not protected from side selection");
            Check(w.SetHomeYard(b.Id,2) && w.RequestImprovement(b.Id),"Separate yard rejected");
            var claimed=w.PotentialHomeYardPlaces(a).Concat(w.PotentialHomeYardPlaces(b)).ToArray();
            foreach(var c in claimed){Check(!w.SetCommons(c),"Commons center stole domestic ground");Check(!w.CommonsPlaces(new Cell(-1,8)).Intersect(claimed).Any(),"Commons places stole domestic ground");}
            before=w.SaveJson();
            // A second rear yard moved behind the first house overlaps its right-side yard.
            var candidate=w.Map.Land.SelectMany(c=>Enumerable.Range(0,4).Select(r=>(c,r))).FirstOrDefault(v=>w.RelocationProblem(b.Id,v.c,v.r)?.Contains("yard uses")==true);
            Check(candidate!=default,"No overlap relocation counterexample found");
            Check(!w.MoveBuilding(b.Id,candidate.c,candidate.r) && before==w.SaveJson(),"Conflicting move accepted or query mutated world");w.Validate();
        }
        foreach(bool relaxed in new[]{false,true})
        {
            var w=World.NewTransformationHamlet(relaxed,true);var h=w.Cottages.First(c=>c.Kind==BuildingKind.Cottage);
            Check(w.FurnishHomeYard(h.Id,3),"Commons conflict fixture order failed");Until(w,()=>h.Improved,"Commons conflict fixture furnishing stalled");
            Check(w.SetCommons(new Cell(2,-9)),"Commons conflict fixture unavailable");
            string before=w.SaveJson();
            var shared=w.Commons!.Places.Append(w.Commons.Center).ToHashSet();
            var candidate=w.Map.Land.SelectMany(c=>Enumerable.Range(0,4).Select(r=>(c,r))).Where(v=>new[]{World.RotateOffset(v.c,2,-1,v.r),World.RotateOffset(v.c,2,0,v.r)}.Any(shared.Contains))
                .FirstOrDefault(v=>w.RelocationProblem(h.Id,v.c,v.r)==null);
            Check(candidate==default,$"Moving furnished yard onto shared ground accepted: {candidate.c} rotation {candidate.r}");
            Check(!w.MoveBuilding(h.Id,new Cell(-2,-9),0) && before==w.SaveJson(),"Commons conflict move/queries changed world");
            w.Validate();
        }
        Console.WriteLine("PASS: four yard sides in Normal/relaxed, real quiet work and meals, exact saves, occupied-yard rearrangement and rejection purity.");
    }
}

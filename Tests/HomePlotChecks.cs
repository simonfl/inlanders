using Inlanders.Simulation;
static class HomePlotChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    public static void Run()
    {
        Directory.CreateDirectory("artifacts/home-plot");
        foreach(bool relaxed in new[]{false,true})
        {
            var w=World.NewPlayerFounded(relaxed);string before=w.SaveJson();
            foreach(int rotation in Enumerable.Range(0,4))foreach(int side in Enumerable.Range(0,4))
            {
                var plan=w.PreviewHomePlot(new(1,8),rotation,BuildingKind.Cottage,side);
                Check(plan.Problem==null && plan.Yard.Length==2,"Open home/yard proposal rejected");
                Check(!plan.Yard.Intersect(World.Footprint(new(1,8),rotation,BuildingKind.Cottage)).Any(),"Yard inside house");
            }
            Check(before==w.SaveJson(),"Proposal mutates world");
            Check(w.PlaceHomePlot(new(0,0),0,BuildingKind.Cottage,0)==null && before==w.SaveJson(),"Bad home plot not atomic");
            var home=w.PlaceHomePlot(new(1,8),0,BuildingKind.Cottage,0)!;
            if(!relaxed)Check(home.PlannedYard && !home.Complete && !home.ImprovementRequested,"Furnishing started before home complete");
            var clone=World.LoadJson(w.SaveJson());for(int i=0;i<3000;i++){w.Tick(.1f);clone.Tick(.1f);if(i%100==0)w.Validate();}
            Check(w.SaveJson()==clone.SaveJson(),"Planned home continuation differs");
            Check(home.Complete && home.Improved && !home.PlannedYard && w.People.Any(p=>p.HomeId==home.Id && p.RestVisits>0),"Home plan did not become furnished lived home");
            Check(relaxed || home.ImprovementPlanks==4 && w.Planks==0,"Furnishings not physically paid");
            // Earlier safe provisions do not prescribe this option; a bare home remains valid.
            var bare=w.Place(new(-3,8),0,BuildingKind.Cottage)!;Check(!bare.PlannedYard && !bare.Improved,"Bare home auto-furnished");
            w.SaveFile($"artifacts/home-plot/{relaxed}.json");
        }
        var cancel=World.NewPlayerFounded();var h=cancel.PlaceHomePlot(new(1,8),0,BuildingKind.Cottage,0)!;
        Check(cancel.CancelImprovement(h.Id) && !h.PlannedYard,"Could not cancel unbuilt yard");for(int i=0;i<2000;i++)cancel.Tick(.1f);
        Check(h.Complete && !h.Improved && cancel.Planks==4,"Cancelled yard spent planks");
        Console.WriteLine("PASS home plot: pure four-way yard proposal, atomic rejection, ordinary home and furnishing work, exact active saves, optional bare home and no-cost plan cancellation.");
    }
}

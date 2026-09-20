using Inlanders.Simulation;
static class ConstructionStagingChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    public static void Run()
    {
        foreach(var phase in new[]{Work.Waiting,Work.ToMaterials,Work.ToCottage,Work.Building})
        {
            var w=World.NewTransformationHamlet();var p=ReviewPlacement.Find(w,BuildingKind.Cottage,new(4,3),(c,r)=>true,"staging")!;
            var site=w.Place(p.Actual,p.Rotation,BuildingKind.Cottage)!;
            if(phase!=Work.Waiting){int ticks=0;while(!w.People.Any(v=>v.SiteId==site.Id && v.Task==phase) && ticks++<3000)w.Tick(.1f);Check(ticks<3000,"Did not reach "+phase);}
            Check(w.SetConstructionPaused(site.Id,true),"Pause rejected");int delivered=site.Delivered;float progress=site.Construction;
            Check(site.Incoming==0 && site.Builder==null,"Reservations retained");
            var otherPlan=ReviewPlacement.Find(w,BuildingKind.VegetableGarden,new(0,-9),(c,r)=>true,"other project")!;var other=w.Place(otherPlan.Actual,otherPlan.Rotation,BuildingKind.VegetableGarden)!;
            var copy=World.LoadJson(w.SaveJson());
            for(int i=0;i<1800;i++){w.Tick(.1f);copy.Tick(.1f);if(i%100==0)w.Validate();}
            Check(copy.SaveJson()==w.SaveJson(),"Paused continuation differs");
            Check(site.Delivered==delivered && site.Construction==progress && other.Complete,"Staging did not release work to another plan");
            Check(w.SetConstructionPaused(site.Id,false),"Resume rejected");for(int i=0;i<3000 && !site.Complete;i++)w.Tick(.1f);
            Check(site.Complete && !w.SetConstructionPaused(site.Id,true),"Resume did not finish or completed pause allowed");w.Validate();
        }
        Console.WriteLine("PASS: pause before pickup, with reservation/cargo, and during construction; retained progress, other project completion, exact saves and resume.");
    }
}

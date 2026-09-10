using Inlanders.Simulation;

public static class ComfortChecks
{
    static void Check(bool value,string why) { if(!value) throw new Exception(why); }
    static void Until(World world,Func<bool> done,string why,int limit=12000)
    {
        for(int i=0;i<limit && !done();i++) {world.Tick(.1f);world.Validate();}
        Check(done(),why);
    }
    public static void Run()
    {
        var w=new World(16);w.Food.InitialBerries=w.Food.Berries=512;
        foreach(var p in w.People) w.Assign(p.Id,p.Id<2?Role.Logger:p.Id<4?Role.Builder:p.Id==4?Role.Sawyer:p.Id==5?Role.Carpenter:Role.Unassigned);
        var home=w.Place(new(0,0))!;var shop=w.Place(new(3,0),false,BuildingKind.Carpenter)!;var saw=w.Place(new(6,0),false,BuildingKind.Sawmill)!;
        foreach(var cell in new[]{new Cell(0,6),new(3,6),new(6,6),new(-3,6)}) Check(w.Place(cell)!=null,"Comfort spare-home setup rejected");
        Until(w,()=>w.Cottages.All(c=>c.Complete) && w.AvailablePlanks>=4,"Normal carpenter setup stalled");
        Directory.CreateDirectory("artifacts");File.WriteAllText("artifacts/f25b2-ready.json",w.SaveJson());
        var lodgeWorld=World.LoadJson(w.SaveJson());
        Cottage? lodge=null;
        for(int z=-7;z<=7 && lodge==null;z++) for(int x=-7;x<=7 && lodge==null;x++) lodge=lodgeWorld.Place(new(x,z),false,BuildingKind.Lodge);
        Check(lodge!=null,"Lodge fixture placement failed");
        Until(lodgeWorld,()=>lodge!.Complete,"Lodge construction stalled");
        foreach(var p in lodgeWorld.People.Take(4)) Check(lodgeWorld.AssignHome(p.Id,lodge!.Id),"Lodge resident assignment failed");
        Check(lodgeWorld.RequestImprovement(lodge!.Id),"Lodge improvement rejected");
        Until(lodgeWorld,()=>lodge.Improved,"Lodge improvement stalled");
        Check(lodge.ImprovementPlanks==8 && World.ComfortCost(lodge)==8,"Lodge improvement has wrong per-bed cost");
        Check(World.LoadJson(lodgeWorld.SaveJson()).SaveJson()==lodgeWorld.SaveJson(),"Improved lodge save changed");
        Check(w.RequestImprovement(home.Id),"Occupied home rejected improvement");
        Check(!w.RequestImprovement(home.Id),"Duplicate improvement accepted");
        var states=new Dictionary<Work,string>();
        Until(w,()=> {
            var worker=w.People[5];
            if(worker.ComfortHomeId==home.Id && !states.ContainsKey(worker.Task)) states[worker.Task]=w.SaveJson();
            return home.Improved;
        },"Home improvement stalled");
        foreach(var phase in new[]{Work.ToComfortPlanks,Work.ToComfortHome,Work.ToComfortInstall,Work.InstallingComfort})
        {
            Check(states.ContainsKey(phase),$"Missed comfort phase {phase}");
            File.WriteAllText($"artifacts/f25b2-{phase}.json",states[phase]);
            var clone=World.LoadJson(states[phase]);Check(clone.SaveJson()==states[phase],$"Comfort {phase} save changed");
            string state=clone.SaveJson();clone.ComfortSummary(clone.Cottages.Single(c=>c.Id==home.Id));Check(clone.SaveJson()==state,"Comfort inspection changed state");
            Check(clone.CancelImprovement(home.Id),$"Cannot cancel {phase}");clone.Validate();
            Until(clone,()=>clone.Cottages.Single(c=>c.Id==home.Id).ImprovementPlanks==0 && !clone.People.Any(p=>p.ComfortHomeId==home.Id || p.Cargo==Resource.Planks && p.Carried>0),"Cancelled planks were not recovered");
            Check(clone.RequestImprovement(home.Id),"Recovered home could not be ordered again");
            Until(clone,()=>clone.Cottages.Single(c=>c.Id==home.Id).Improved,"Recovered order stalled");
        }
        var empty=World.LoadJson(states[Work.InstallingComfort]);var emptyHome=empty.Cottages.Single(c=>c.Id==home.Id);
        var spare=empty.Cottages.First(c=>Buildings.Get(c.Kind).Beds>0 && !empty.People.Any(p=>p.HomeId==c.Id));
        var occupants=empty.People.Where(p=>p.HomeId==home.Id).ToArray();
        foreach(var p in occupants) Check(empty.AssignHome(p.Id,spare.Id),"Cannot move residents to spare home");
        float progress=emptyHome.ImprovementProgress;
        for(int i=0;i<300;i++) {empty.Tick(.1f);empty.Validate();}
        Check(emptyHome.ImprovementProgress==progress && emptyHome.ImprovementRequested,"Empty home continued installing");
        Check(empty.AssignHome(occupants[0].Id,home.Id),"Cannot reoccupy home");
        Until(empty,()=>emptyHome.Improved,"Reoccupied improvement did not resume");
        var paused=World.LoadJson(states[Work.ToComfortPlanks]);Check(paused.SetWorkplacePaused(shop.Id,true),"Cannot pause carpenter");
        for(int i=0;i<500;i++) {paused.Tick(.1f);paused.Validate();}
        Check(!paused.People.Any(p=>p.WorkplaceId==shop.Id) && !paused.Cottages.Single(c=>c.Id==home.Id).Improved,"Paused carpenter claimed more work");
        Check(paused.SetWorkplacePaused(shop.Id,false),"Cannot resume carpenter");
        Until(paused,()=>paused.Cottages.Single(c=>c.Id==home.Id).Improved,"Unpaused carpenter stalled");
        var removed=World.LoadJson(states[Work.ToComfortHome]);Check(removed.RequestDemolition(shop.Id),"Cannot remove workshop with shipment");removed.Validate();
        Until(removed,()=>!removed.Cottages.Any(c=>c.Id==shop.Id),"Workshop demolition stalled");
        Check(removed.Cottages.Single(c=>c.Id==home.Id).ImprovementRequested,"Workshop removal lost home order");
        var replacement=removed.Place(shop.Cell,false,BuildingKind.Carpenter);Check(replacement!=null,"Cannot replace removed workshop");
        Until(removed,()=>removed.Cottages.Single(c=>c.Id==home.Id).Improved,"Replacement carpenter did not resume order");
        Check(home.ImprovementPlanks==4 && home.ImprovementProgress==1 && !home.ImprovementRequested,"Finished improvement lost materials/state");
        var resident=w.People.First(p=>p.HomeId==home.Id);
        resident.NextRestTime=w.Food.Time;
        Until(w,()=>resident.LastRestWindow==300,"Improved home was not used for rest");
        Check(w.ReadHappiness(resident).Rest==10 && w.ReadHappiness(resident).Score<=100,"Comfort inflated happiness scale");
        string improved=w.SaveJson();Check(World.LoadJson(improved).SaveJson()==improved,"Improved rest save changed");
        File.WriteAllText("artifacts/f25b2-improved.json",improved);
        Check(w.RequestDemolition(home.Id),"Improved home demolition rejected");
        Until(w,()=>!w.Cottages.Any(c=>c.Id==home.Id),"Improved home demolition stranded materials");
        var creative=World.NewCreative();var free=creative.Place(new(0,0))!;
        var resting=creative.People.First(p=>p.HomeId==free.Id);
        foreach(var p in creative.People) creative.Assign(p.Id,Role.Unassigned);
        Until(creative,()=>resting.Task==Work.Resting,"Creative rest fixture stalled");
        Check(creative.RequestImprovement(free.Id) && free.Improved && free.ImprovementPlanks==0,"Creative improvement was not free/instant");
        int visits=resting.RestVisits;Until(creative,()=>resting.RestVisits>visits,"Rest did not finish");
        Check(resting.LastRestWindow==240,"Improvement rewrote a visit already underway");
        Check(creative.RemoveBuilding(free.Id),"Creative improved home cannot be removed");creative.Validate();
        Console.WriteLine("PASS: normal carpenter production/setup, physical improvement phases, exact saves, cancel/recover/reorder, actual rest benefit, demolition and free Creative improvements.");
    }
}

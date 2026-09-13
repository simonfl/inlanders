using Inlanders.Simulation;

static class SharedWorkChecks
{
    static void Check(bool value,string why){if(!value)throw new Exception(why);}
    public static void Run()
    {
        var w=World.NewSharedWorkExperiment();
        Check(w.People.All(p=>p.SharedWorker),"Initial pool missing residents");
        var seen=w.People.ToDictionary(p=>p.Id,p=>new HashSet<Role>());
        void Until(Func<bool> done,string why)
        {
            for(int i=0;i<24000 && !done();i++)
            {
                w.Tick(.1f);foreach(var p in w.People)if(p.Role!=Role.Unassigned)seen[p.Id].Add(p.Role);
                if(i%20==0)w.Validate();
            }
            Check(done(),why+"; "+string.Join("; ",w.People.Select(p=>$"{p.Role}: {p.Status}")));w.Validate();
        }
        Cottage Place(Cell cell,BuildingKind kind,int rotation=0)=>w.Place(cell,rotation,kind)??throw new Exception(w.PlacementProblem(cell,rotation,kind));
        var bridge=Place(new(5,2),BuildingKind.Bridge,1);Until(()=>bridge.Complete,"Shared bridge construction stalled");
        var farm=Place(new(8,7),BuildingKind.Farm);var bakery=Place(new(11,0),BuildingKind.Bakery);
        Until(()=>farm.Complete && bakery.Complete,"Shared workplaces stalled");
        Until(()=>w.Food.BakedBread>=4,"Shared grain chain stalled");
        Check(seen.Values.Any(roles=>roles.Count>=3),"Nobody changed professions");
        Check(seen.Values.Any(roles=>roles.Contains(Role.Logger)),"No shared timber work");
        string save=w.SaveJson();var copy=World.LoadJson(save);
        for(int i=0;i<200;i++){w.Tick(.1f);copy.Tick(.1f);Check(w.SaveJson()==copy.SaveJson(),"Shared save continuation differs");}
        var resident=w.People.First(p=>p.Role==Role.Baker);
        w.Assign(resident.Id,Role.Baker);Check(!resident.SharedWorker,"Same-role dedication left resident shared");
        Check(w.SetWorkplaceAssignment(resident.Id,bakery.Id),"Dedicated oven rejected");
        for(int i=0;i<1000;i++){w.Tick(.1f);w.Validate();Check(resident.Role==Role.Baker && !resident.SharedWorker,"Dedicated role reset");}
        w.Assign(resident.Id,Role.Unassigned);Check(resident.SharedWorker && resident.AssignedWorkplaceId==null,"Release failed");w.Validate();
        var baseline=World.NewCampaign(6);baseline.Assign(0,Role.Unassigned);
        for(int i=0;i<100;i++)baseline.Tick(.1f);
        Check(!baseline.People[0].SharedWorker && baseline.People[0].Role==Role.Unassigned,"Baseline staffing changed");
        Console.WriteLine("Shared staffing: construction, timber, local bread, role changes, dedication, release and saved continuation passed.");
    }
}

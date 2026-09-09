using System;
using System.Linq;
using Inlanders.Simulation;

public static class LeisureChecks
{
    static void Check(bool ok,string message) { if(!ok) throw new Exception(message); }
    static void Step(World w,int n=1) { for(int i=0;i<n;i++) { w.Tick(.1f); w.Validate(); } }
    public static void Run()
    {
        var w = new World(40);
        w.Food.InitialBerries=w.Food.Berries=1000;
        var square=w.Place(new(3,0),false,BuildingKind.Square)!;
        for(int i=0;i<4000 && !square.Complete;i++) Step(w);
        Check(square.Complete,"Square did not build");
        bool walking=false,visiting=false;
        for(int i=0;i<3000;i++)
        {
            Step(w);
            foreach(var task in new[]{Work.ToLeisure,Work.Leisure})
            {
                var p=w.People.FirstOrDefault(p=>p.Task==task);
                if(p==null || (task==Work.ToLeisure ? walking : visiting)) continue;
                if(task==Work.ToLeisure) walking=true; else visiting=true;
                string saved=w.SaveJson(); var clone=World.LoadJson(saved);
                Check(clone.SaveJson()==saved,"Leisure save changed");
                var twin=World.LoadJson(saved); Step(clone,150); Step(twin,150);
                Check(clone.SaveJson()==twin.SaveJson(),"Leisure resume diverged");
                clone=World.LoadJson(saved); clone.Assign(p.Id,Role.Unassigned);
                Check(clone.People[p.Id].LeisureSiteId==null,"Reassignment kept leisure claim");
                Step(clone,20);
                Check(clone.People[p.Id].LeisureSiteId==null,"Interrupted visit restarted immediately");
                if(task==Work.ToLeisure)
                    Check(w.PlantingProblem(p.Destination)!=null,"Planting can block reserved leisure destination");
            }
        }
        Check(walking && visiting && w.People.All(p=>p.LeisureVisits>=2),"Breaks did not recur for all villagers");
        Check(w.Stored>0 && w.People.Any(p=>p.Role==Role.Logger),"Production did not continue");
        var without=new World(); Step(without,800);
        Check(without.People.All(p=>p.LeisureVisits==0),"Leisure without a square");
        var supper=PopulationChecks.Ready();
        var cell=supper.Map.Land.First(c=>supper.PlacementProblem(c,false,BuildingKind.Square)==null);
        var place=supper.Place(cell,false,BuildingKind.Square)!;
        supper.Assign(0,Role.Builder); supper.Assign(1,Role.Logger);
        for(int i=0;i<4000 && !place.Complete;i++) Step(supper);
        Check(place.Complete,"Supper square did not build");
        Step(supper,200);
        supper.Food.Bread+=supper.SupperCost; supper.Food.BakedBread+=supper.SupperCost;
        supper.Food.UsedGrain+=supper.SupperCost/2; supper.Food.GrownGrain+=supper.SupperCost/2;
        Check(supper.BeginSupper(),"Supper could not start during leisure");
        Check(supper.People.All(p=>p.LeisureSiteId==null),"Supper kept leisure reservations");
        for(int i=0;i<3000 && !supper.Food.SupperComplete;i++) Step(supper);
        Check(supper.Food.SupperComplete,"Leisure blocked supper");
        Console.WriteLine("PASS: recurring square visits, capacity, production, assignment, destination protection, saves and supper.");
    }
}

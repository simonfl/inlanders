using Inlanders.Simulation;
using System.Text.Json.Nodes;

public static class HomeChecks
{
    static void Check(bool ok,string message) { if(!ok) throw new Exception(message); }
    static void Until(World w,Func<bool> done,string label,int limit=6000)
    {
        for(int i=0;i<limit && !done();i++) { w.Tick(.1f); if(i%10==0) w.Validate(); }
        Check(done(),"Home check stalled: "+label); w.Validate();
    }
    public static void Run()
    {
        var w=World.NewCreative();
        foreach(var p in w.People) w.Assign(p.Id,Role.Unassigned);
        var first=w.Place(new(0,0))!; var second=w.Place(new(3,0))!;
        Check(w.ResidentsWithHomes==4 && w.People.Count(p=>p.HomeId==first.Id)==2,"Home allocation did not respect beds");
        Check(!w.AssignHome(w.People.Last().Id,first.Id),"Full home accepted a resident");
        var person=w.People.First(p=>p.HomeId==first.Id);
        Until(w,()=>person.Task==Work.ToRest,"home journey");
        Check(!w.PlaceDecoration(person.Destination,DecorationKind.Flowers),"Placement blocked a committed home visit");
        string travelling=w.SaveJson(); var clone=World.LoadJson(travelling);
        for(int i=0;i<50;i++) { w.Tick(.1f); clone.Tick(.1f); }
        Check(w.SaveJson()==clone.SaveJson(),"Home journey continuation diverged");
        Until(w,()=>person.Task==Work.Resting,"doorstep rest");
        string resting=w.SaveJson(); Check(World.LoadJson(resting).SaveJson()==resting,"Rest pose did not roundtrip");
        int visits=person.RestVisits; w.Assign(person.Id,Role.Builder);
        Check(person.RestVisits==visits && person.Task!=Work.Resting && person.HomeId==first.Id,"Job interruption lost home or credited unfinished rest");
        w.Assign(person.Id,Role.Unassigned);
        Until(w,()=>person.RestVisits>visits,"completed home visit");
        Check(w.RecentlyRested(person) && w.ReadHappiness(person).Rest==10,"Completed visit has no rest benefit");
        var expired=World.LoadJson(w.SaveJson());
        foreach(var p in expired.People) p.NextRestTime=expired.Food.Time+1000;
        for(int i=0;i<2401;i++) expired.Tick(.1f);
        Check(expired.ReadHappiness(expired.People[person.Id]).Rest==0,"Old rest never expired"); expired.Validate();
        var extra=w.Place(new(6,0))!;
        // Newly built homes fill automatically; this fifth/sixth resident can move only when a bed is freed.
        var spare=w.Place(new(0,6))!; var overflow=w.Place(new(3,6))!;
        Check(w.AssignHome(person.Id,overflow.Id),"Resident could not choose a spare home");
        Check(person.HomeId==overflow.Id,"Home choice not retained");
        Until(w,()=>person.Task is Work.ToRest or Work.Resting,"visit changed home");
        visits=person.RestVisits; Check(w.RemoveBuilding(overflow.Id),"Creative home removal rejected");
        Check(person.HomeId!=overflow.Id && person.Task is not (Work.ToRest or Work.Resting) && person.RestVisits==visits,"Removed home retained visitor or credited rest");
        w.Validate();
        var invalid=JsonNode.Parse(w.SaveJson())!; invalid["People"]![0]!["HomeId"]=99999;
        try { World.LoadJson(invalid.ToJsonString()); throw new Exception("Bad home save loaded"); } catch(InvalidOperationException) { }
        var normal=World.NewCampaign(6);
        var owner=normal.People.First(p=>p.HomeId!=null); int oldHome=owner.HomeId!.Value;
        Until(normal,()=>owner.Task==Work.Resting,"normal rest");
        Check(normal.RequestDemolition(oldHome),"Home demolition rejected");
        Check(normal.People.All(p=>p.HomeId!=oldHome),"Demolition retained home assignments");
        Check(normal.CancelDemolition(oldHome),"Home demolition cancel failed");
        Check(normal.ResidentsWithHomes==normal.Population,"Cancelled demolition did not restore available housing");
        Until(normal,()=>normal.People.Any(p=>p.Task==Work.Resting),"rest before supper");
        var interrupted=normal.People.First(p=>p.Task==Work.Resting); int beforeSupper=interrupted.RestVisits;
        normal.Food.Bread+=16; normal.Food.BakedBread+=16; normal.Food.UsedGrain+=8; normal.Food.GrownGrain+=8;
        Check(normal.BeginSupper(),"Supper could not interrupt home routine");
        Check(interrupted.Task!=Work.Resting && interrupted.RestVisits==beforeSupper,"Supper credited interrupted rest");
        Until(normal,()=>normal.Food.SupperComplete,"supper after home interruption");
        normal.Validate();
        Console.WriteLine("PASS: stable homes, capacity, actual/interrupted rest, reassignment, removal/cancellation and exact current saves.");
        CompareNeighborhoods();
    }
    static void CompareNeighborhoods()
    {
        var seed=World.NewCampaign(6);
        foreach(var person in seed.People) person.Position=new(-7,7);
        Cottage Ready(Cell at,BuildingKind kind,bool rotate=false)
        {
            var home=seed.Place(at,rotate,kind) ?? throw new Exception("Home experiment placement failed");
            int remaining=home.Required;
            foreach(var tree in seed.Trees) { int take=Math.Min(remaining,tree.Logs); tree.Logs-=take; remaining-=take; if(remaining==0) break; }
            Check(remaining==0,"Fixture has insufficient timber");
            home.Delivered=home.Required; home.Construction=1; return home;
        }
        Ready(new(5,2),BuildingKind.Bridge,true);
        foreach(var cell in new[]{new Cell(8,0),new(8,4),new(11,6),new(14,10)}) Ready(cell,BuildingKind.Cottage);
        Ready(new(-1,6),BuildingKind.VegetableGarden); Ready(new(-1,-3),BuildingKind.VegetableGarden);
        Ready(new(2,3),BuildingKind.Square);
        foreach(var p in seed.People) seed.Assign(p.Id,p.Id<2 ? Role.Forager : p.Id<4 ? Role.Farmer : Role.Unassigned);
        seed.Tick(.1f); string initial=seed.SaveJson();
        (float Travel,int Meals,int Visits,int Leisure,int Food) Run(bool dispersed)
        {
            var world=World.LoadJson(initial);
            if(dispersed)
                foreach(var person in world.People)
                {
                    var home=world.Cottages.First(h=>h.Cell.X>5 && Buildings.Get(h.Kind).Beds>0 && world.HomeAssignmentProblem(person.Id,h.Id)==null);
                    Check(world.AssignHome(person.Id,home.Id),"Dispersed home assignment failed");
                }
            float travel=0; int fullMeals=0;
            for(int i=0;i<6000;i++)
            {
                travel+=world.People.Count(p=>p.Task==Work.ToRest)*.1f;
                float clock=world.Food.MealClock; world.Tick(.1f);
                if(world.Food.MealClock<clock && world.Food.Hunger==0) fullMeals++;
                if(i%50==0) world.Validate();
            }
            Check(world.People.All(p=>p.RestVisits>0) && fullMeals>=9,"Household routines starved the experiment or prevented visits");
            return (travel,fullMeals,world.People.Sum(p=>p.RestVisits),world.People.Sum(p=>p.LeisureVisits),world.DeliveredBerries+world.DeliveredVegetables);
        }
        var near=Run(false); var far=Run(true);
        Console.WriteLine($"HOME LAYOUT 10 minutes: west homes {near}; east homes {far}. Values: home-travel person-seconds, full meals, rest visits, square visits, food delivered.");
        Check(near.Travel<far.Travel,"Home location did not change travel cost");
    }
}

using System;
using System.Linq;
using Inlanders.Simulation;

public static class EconomyChecks
{
    static void Check(bool ok,string message) { if(!ok) throw new Exception(message); }
    public static void Run()
    {
        var w=World.NewScenario();
        string saved=w.SaveJson(); var report=w.ReadEconomy();
        Check(w.SaveJson()==saved,"Economy inspection mutated the world");
        Check(report.Meals==3 && report.Stocks.Single(s=>s.Resource==Resource.Berries).Available==24,"Food reserve includes inedible food or loses berries");
        Check(report.Issues.Any(i=>i.Build==BuildingKind.ForagerHut) && report.Issues.Any(i=>i.Build==BuildingKind.Farm),"Missing workplaces not explained");
        w.Food.Grain=w.Food.GrownGrain=80;
        Check(w.ReadEconomy().Meals==3,"Raw grain was counted as meals");
        var cottage=w.Place(new(3,0))!;
        foreach(var p in w.People) w.Assign(p.Id,Role.Unassigned);
        Check(w.ReadEconomy().Issues.Any(i=>i.Staff==Role.Builder),"Missing builders not diagnosed");
        w.Assign(0,Role.Logger); w.Assign(1,Role.Builder);
        Check(!w.ReadEconomy().Issues.Any(i=>i.Id=="builders"),"Resolved staffing warning stuck");
        bool sawReserved=false,sawCarried=false;
        for(int i=0;i<5000 && !cottage.Complete;i++) {
            w.Tick(.1f); w.Validate();
            report=w.ReadEconomy(); var logs=report.Stocks.Single(s=>s.Resource==Resource.Logs);
            if(w.ReservedStorage>0) { sawReserved=true; Check(logs.Available==w.Available && logs.Reserved==w.ReservedStorage,"Reservations reported as free stock"); }
            int carriedLogs=w.People.Where(p=>p.Cargo==Resource.Logs).Sum(p=>p.Carried);
            if(carriedLogs>0) sawCarried=true;
            Check(logs.Carried==carriedLogs,"In-flight timber differs from actual log cargo");
            Check(logs.ConstructionNeed==cottage.Required-cottage.Delivered-cottage.Incoming,"Committed deliveries counted as new demand");
        }
        Check(sawReserved && sawCarried && cottage.Complete,"Inventory test missed hauling phases");
        w=World.NewCampaign(2);
        foreach(var p in w.People) w.Assign(p.Id,Role.Unassigned);
        Check(w.ReadEconomy().Issues.Any(i=>i.Staff==Role.Forager),"Completed unstaffed workplace not flagged");
        w.Assign(4,Role.Forager); Check(!w.ReadEconomy().Issues.Any(i=>i.Id=="staff-Forager"),"Staffed hut warning not cleared");
        // Consumed stock changes coverage; cumulative gathering is not a food reserve.
        foreach(var p in w.People) w.Assign(p.Id,Role.Unassigned);
        for(int i=0;i<6800;i++) w.Tick(.1f);
        report=w.ReadEconomy(); Check(report.Meals<2 && report.Issues.Any(i=>i.Id=="food-low"),"Low reserve did not trigger");
        Check(World.LoadJson(w.SaveJson()).ReadEconomy().Meals==report.Meals,"Economy did not refresh from saved inventory");
        Console.WriteLine("PASS: read-only economy, food coverage, reserved/carried timber, committed demand, actionable staffing and low-food warnings.");
    }
}

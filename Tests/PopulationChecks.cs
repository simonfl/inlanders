using System;
using System.Linq;
using System.Text.Json.Nodes;
using Inlanders.Simulation;

public static class PopulationChecks
{
    static void Check(bool ok,string message) { if(!ok) throw new Exception(message); }
    public static World Ready()
    {
        var w=World.NewCampaign(2);
        w.Food.InitialBerries=w.Food.Berries=200;
        Check(w.Place(new(3,-3))!=null && w.Place(new(6,-3))!=null,"Arrival homes rejected");
        for(int i=0;i<6000 && w.Beds<12;i++) w.Tick(.1f);
        Check(w.Beds==12,"Arrival homes not completed");
        foreach(var p in w.People) w.Assign(p.Id,Role.Unassigned);
        // Finish any previously committed return trips before testing meal accounting.
        for(int i=0;i<1000 && w.People.Any(p=>p.Carried>0);i++) w.Tick(.1f);
        return w;
    }
    public static void Run()
    {
        var empty=new World(); string before=empty.SaveJson();
        Check(!empty.InviteNewcomers() && empty.SaveJson()==before,"Rejected arrival changed settlement");
        var w=Ready();
        w.Food.EatenBerries+=w.Food.Berries-19; w.Food.Berries=19;
        w.Food.Grain=w.Food.GrownGrain=100;
        before=w.SaveJson();
        Check(!w.InviteNewcomers() && w.SaveJson()==before,"Inedible grain counted toward arrival reserve");
        w.Food.InitialBerries+=1; w.Food.Berries++;
        Check(w.InviteNewcomers() && w.Population==10 && w.Housed==10 && w.SpareBeds==2,"First pair/housing failed");
        Check(w.People.Skip(8).All(p=>p.Role==Role.Unassigned && p.Carried==0) && w.People[8].Position!=w.People[9].Position,"Arrival roles or positions wrong");
        Check(w.Food.Berries==20 && !w.InviteNewcomers(),"Arrival spent food or reused insufficient reserve");
        w.Food.InitialBerries+=4; w.Food.Berries+=4;
        Check(w.InviteNewcomers() && w.Population==12 && !w.InviteNewcomers(),"Repeated invite exceeded housing");
        Check(w.People.Select(p=>p.Name).Distinct().Count()==12,"Duplicate newcomer names");
        Check(w.ReadEconomy().Meals==2,"Meal coverage ignores newcomers");
        w.Food.MealClock=59.9f; w.Tick(.2f);
        Check(w.Food.Berries==12 && w.Food.Hunger==0,"Daily meal did not feed 12");
        Check(w.ReadEconomy().Issues.Any(i=>i.Id=="food-low"),"Dynamic two-meal warning missing");
        w.Food.EatenBerries+=w.Food.Berries-6; w.Food.Berries=6;
        w.Food.MealClock=59.9f; w.Tick(.2f);
        Check(w.Food.Hunger==.5f,"Hunger fraction ignores population");
        w.Assign(8,Role.Logger); w.Assign(9,Role.Builder);
        for(int i=0;i<30;i++) w.Tick(.1f);
        var restored=World.LoadJson(w.SaveJson());
        Check(restored.SaveJson()==w.SaveJson(),"Expanded population save differs");
        for(int i=0;i<100;i++) { w.Tick(.1f); restored.Tick(.1f); }
        Check(restored.SaveJson()==w.SaveJson(),"Newcomer work continuation diverged");
        var invalid=JsonNode.Parse(w.SaveJson())!;
        invalid["People"]![9]!["Id"]=8;
        bool rejected=false; try { World.LoadJson(invalid.ToJsonString()); } catch { rejected=true; }
        Check(rejected,"Duplicate newcomer ID accepted");
        // A larger supper uses two loaves and one destination per person.
        w.Food.Grain=88; w.Food.UsedGrain=12; w.Food.Bread=w.Food.BakedBread=24;
        Check(w.SupperCost==24 && w.BeginSupper() && w.MeetingSpots.Distinct().Count()==12,"Expanded supper failed");
        before=w.SaveJson();
        Check(!w.InviteNewcomers() && w.SaveJson()==before,"Arrival interrupted supper");
        restored=World.LoadJson(before);
        for(int i=0;i<5000 && !w.Food.SupperComplete;i++) { w.Tick(.1f); restored.Tick(.1f); }
        Check(w.Food.SupperComplete && w.Food.SupperBread==24 && w.SaveJson()==restored.SaveJson(),"Expanded supper restoration failed");
        Check(w.Campaign?.Level==2,"Arrival replaced campaign");
        Console.WriteLine("PASS: optional pairs, spare beds, edible reserve, repeated invitations, dynamic meals/hunger/economy, newcomer work and exact saves, 12-person supper.");
    }
}

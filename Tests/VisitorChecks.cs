using System;
using System.Linq;
using Inlanders.Simulation;

public static class VisitorChecks
{
    static void Check(bool ok,string message) { if(!ok) throw new Exception(message); }
    static void Step(World w,int n=1) { for(int i=0;i<n;i++) { w.Tick(.1f); w.Validate(); } }
    public static World Ready()
    {
        var w=World.NewCampaign(2);
        foreach(var p in w.People) w.Assign(p.Id,Role.Unassigned);
        Step(w,1201); Check(w.Gardener==VisitorState.Pending,"Gardener did not arrive"); return w;
    }
    public static void Run()
    {
        var bare=new World(); Step(bare,1300); Check(bare.Gardener==VisitorState.NotArrived,"Visitor arrived without a hut");
        var early=World.NewCampaign(2); Step(early,100);
        Check(early.Gardener==VisitorState.NotArrived,"Visitor arrived early");
        var w=Ready(); string pending=w.SaveJson();
        Check(World.LoadJson(pending).SaveJson()==pending,"Pending offer not saved");
        int deliveries=w.DeliveredBerries, food=w.Food.Berries;
        Check(!w.PlaceDecoration(new(5,3),DecorationKind.Sunflowers),"Reward available before accepting");
        Check(w.AcceptGardener() && !w.AcceptGardener() && !w.DeclineGardener(),"Trade was not one-time");
        Check(w.Food.Berries==food-8 && w.DeliveredBerries==deliveries,"Trade cost or delivery count incorrect");
        Check(w.PlaceDecoration(new(5,3),DecorationKind.Sunflowers),"Reward unavailable");
        w.Validate();
        string accepted=w.SaveJson(); var saved=World.LoadJson(accepted);
        Check(saved.SaveJson()==accepted && saved.SunflowersUnlocked,"Accepted state/reward not saved");
        Check(saved.RemoveDecoration(new(5,3)) && saved.PlaceDecoration(new(5,3),DecorationKind.Sunflowers),"Reward not reusable");
        var declined=World.LoadJson(pending);
        Check(declined.DeclineGardener() && !declined.AcceptGardener(),"Decline failed");
        Check(declined.DecorationProblem(new(5,3),DecorationKind.Sunflowers)!.Contains("declined"),"Declined reward promises unavailable trade");
        Step(declined,1300); Check(declined.Gardener==VisitorState.Declined,"Declined visitor returned");
        Check(World.LoadJson(declined.SaveJson()).Gardener==VisitorState.Declined,"Decline not saved");
        var poor=World.LoadJson(pending); poor.Food.EatenBerries+=poor.Food.Berries-7; poor.Food.Berries=7;
        string before=poor.SaveJson(); Check(!poor.AcceptGardener() && poor.SaveJson()==before,"Unaffordable trade mutated state");
        Step(poor,700); Check(poor.Gardener==VisitorState.Pending,"Offer expired");
        // A pending visit waits through supper and leaves its bread cost alone.
        var supper=World.LoadJson(pending);
        supper.Food.Bread=supper.Food.BakedBread=supper.SupperCost;
        supper.Food.UsedGrain=supper.Food.GrownGrain=supper.SupperCost/2;
        Check(supper.BeginSupper(),"Supper fixture failed");
        Check(!supper.AcceptGardener() && !supper.DeclineGardener(),"Trade interrupted supper");
        for(int i=0;i<3000 && !supper.Food.SupperComplete;i++) Step(supper);
        Check(supper.Food.SupperComplete && supper.AcceptGardener(),"Supper lost pending offer");
        Console.WriteLine("PASS: gardener timing, no deadline, one-time cost/reward, affordability, reusable sunflowers, campaign delivery accounting, saves and supper.");
    }
}

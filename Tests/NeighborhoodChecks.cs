using Inlanders.Simulation;

static class NeighborhoodChecks
{
    public static void Run()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        var w=World.NewNeighborhoodExperiment();
        Check(w.Campaign==null && !w.CanCelebrate,"Old campaign/supper leaked into experiment");
        Check(!w.InviteNewcomers(),"Commitment accepted without crossing");
        var bridge=w.Place(new(5,2),1,BuildingKind.Bridge)!;
        for(int i=0;i<18000 && !bridge.Complete;i++){w.Tick(.1f);if(i%20==0)w.Validate();}
        Check(bridge.Complete,"Shared crossing stalled");
        Check(w.NeighborhoodLanding()==new Cell(7,2),"Landing did not choose the nearest clear reachable eastern cell");
        var hut=w.Cottages.Single(c=>c.Kind==BuildingKind.ForagerHut);w.SetWorkplacePaused(hut.Id,true);
        for(int i=0;i<18000 && w.EdibleStored>=24;i++)w.Tick(.1f);
        Check(w.EdibleStored<24,"Test did not exercise inadequate food reserve");
        Check(w.SpareBeds==0 && w.InviteNewcomers(),"Commitment incorrectly needs spare homes");
        float start=w.Food.Time;string committed=w.SaveJson();
        Check(!w.InviteNewcomers() && committed==w.SaveJson(),"Repeated commitment mutates world");
        long allocated=GC.GetAllocatedBytesForCurrentThread();
        for(int i=0;i<100;i++)Check(w.InvitationProblem()!=null,"Committed invitation became available");
        Check(GC.GetAllocatedBytesForCurrentThread()-allocated<100_000,"Committed status repeats the expensive landing search");
        var copy=World.LoadJson(committed);
        while(w.Food.Time<start+95){w.Tick(.1f);copy.Tick(.1f);Check(w.SaveJson()==copy.SaveJson(),"Arrival continuation differs");}
        Check(w.Population==12 && w.Neighborhood!.Arrived && w.People.Skip(8).All(p=>p.SharedWorker),"Four shared newcomers missing");
        Check(w.Housed<12,"Test did not exercise unprepared housing");
        Check(w.People.Skip(8).Any(p=>p.Task==Work.ToArrival),"Arrival journey absent");
        w.SetWorkplacePaused(hut.Id,false);
        for(int i=0;i<1500;i++){w.Tick(.1f);if(i%20==0)w.Validate();}
        Check(w.People.Skip(8).All(p=>p.Task!=Work.ToArrival),"Arrivals never reached neighborhood");
        Console.WriteLine("PASS: one committed arrival, no housing gate, four shared newcomers, real crossing journeys, separate progress and exact saved countdown/arrival continuation.");
    }
}

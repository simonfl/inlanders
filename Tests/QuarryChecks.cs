using Inlanders.Simulation;

public static class QuarryChecks
{
    static void Check(bool ok,string why) { if(!ok) throw new Exception(why); }
    static void Step(World w,int count=1) { for(int i=0;i<count;i++) { w.Tick(.1f); w.Validate(); } }
    static void Until(World w,Func<bool> done,string why) { for(int i=0;i<12000 && !done();i++) Step(w); Check(done(),why+" "+string.Join("; ",w.People.Select(p=>p.Status))); }
    public static void Run()
    {
        SharedDeposit();
        RecreationComparison();
        DepositDistanceComparison();
        var w=World.NewLargeMap();
        foreach(var p in w.People) w.Assign(p.Id,Role.Unassigned);
        Check(w.PlacementProblem(new(3,0),false,BuildingKind.Quarry)!=null,"Quarry allowed without an outcrop");
        var camp=w.Place(new(-9,2),false,BuildingKind.Quarry) ?? throw new Exception(w.PlacementProblem(new(-9,2),false,BuildingKind.Quarry));
        var mill=w.Place(new(3,0),false,BuildingKind.Sawmill)!;
        var hall=w.Place(new(0,0),false,BuildingKind.GatheringHall) ?? throw new Exception(w.PlacementProblem(new(0,0),false,BuildingKind.GatheringHall));
        w.Assign(0,Role.Logger); w.Assign(1,Role.Logger); w.Assign(2,Role.Builder); w.Assign(3,Role.Builder); w.Assign(4,Role.Sawyer); w.Assign(5,Role.Quarrier);
        Until(w,()=>w.People[5].Task==Work.Quarrying,"No actual quarry work");
        Check(w.AvailableDeposit(w.Map.StoneDeposits[0])==14 && w.Stone==0,"Stone extraction did not reserve physical stock");
        string mining=w.SaveJson(); var copy=World.LoadJson(mining); Step(w,60); Step(copy,60); Check(w.SaveJson()==copy.SaveJson(),"Quarry save diverged");
        var interrupted=World.LoadJson(mining); interrupted.Assign(5,Role.Unassigned); interrupted.Validate();
        Check(interrupted.Map.StoneDeposits[0].Remaining==16 && interrupted.AvailableDeposit(interrupted.Map.StoneDeposits[0])==16,"Interrupted quarry lost stone");
        Until(w,()=>hall.DeliveredStone>0 && !hall.Complete,"No stone delivery to construction");
        string partial=w.SaveJson();
        var cancelled=World.LoadJson(partial); Check(cancelled.Cancel(hall.Id),"Mixed-material cancel failed");
        Until(cancelled,()=>cancelled.Trees.All(t=>!t.Salvage) && cancelled.People.All(p=>p.SiteId!=hall.Id),"Mixed salvage not recovered");
        Until(w,()=>hall.Complete,"Hall did not complete");
        Check(hall.Delivered==8 && hall.DeliveredStone==12 && hall.IncomingStone==0,"Wrong hall materials");
        float completed=w.Food.Time;
        Until(w,()=>w.People.Any(p=>p.Task==Work.Leisure && p.LeisureSiteId==hall.Id),"Hall not actually used");
        Console.WriteLine($"HALL: completed at {completed:0.0}s; first attendance at {w.Food.Time:0.0}s.");
        Until(w,()=>w.People.Any(p=>p.LastLeisureSiteId==hall.Id),"Hall visit never completed");
        var served=w.People.First(p=>p.LastLeisureSiteId==hall.Id);
        Check(served.LastLeisureWindow==240 && served.NextLeisureTime>=served.LastLeisureTime+120,"Hall service duration not applied");
        Until(w,()=>w.Map.StoneDeposits[0].Remaining==0 && w.People.All(p=>p.Cargo!=Resource.Stone || p.Carried==0),"Finite deposit never exhausted");
        Check(w.Stone==4,"Nearby deposit accounting incorrect");
        var exhausted=World.LoadJson(w.SaveJson()); Step(exhausted,100); Check(exhausted.Map.StoneDeposits[0].Remaining==0,"Exhausted rock regenerated");
        Check(w.RequestDemolition(hall.Id),"Hall demolition rejected");
        Until(w,()=>w.Cottages.All(c=>c.Id!=hall.Id) && w.People.All(p=>p.Carried==0),"Mixed-material demolition stalled");
        Check(w.Stone==16,"Stone lost in demolition");
        float servedAt=served.LastLeisureTime!.Value;
        Until(w,()=>w.Food.Time>=servedAt+125,"Hall benefit timing stalled");
        Check(w.ReadHappiness(served).Leisure==20,"Earned hall benefit lost after demolition or after square window");
        var remembered=World.LoadJson(w.SaveJson());
        Until(remembered,()=>remembered.Food.Time>=servedAt+241,"Hall benefit expiry stalled");
        Check(remembered.ReadHappiness(remembered.People[served.Id]).Leisure==0,"Hall benefit never expires");
        w.Validate();
        Console.WriteLine("PASS: quarry placement, finite reservations, exact saves, interruption, mixed construction/cancellation/salvage, actual recreation, depletion and demolition conservation.");
    }

    static void SharedDeposit()
    {
        var w=World.NewCreative(); foreach(var p in w.People) w.Assign(p.Id,Role.Unassigned);
        w.Map.StoneDeposits.Add(new() { Id=0,Cell=new(5,-4),Capacity=3,Remaining=3 });
        var first=w.Place(new(5,-1),false,BuildingKind.Quarry)!;
        var second=w.Place(new(2,-3),false,BuildingKind.Quarry)!;
        Check(first!=null && second!=null,"Shared quarry camps invalid");
        w.Assign(0,Role.Quarrier); w.Assign(1,Role.Quarrier);
        Until(w,()=>w.People.Count(p=>p.DepositId!=null)==2,"Camps did not share the outcrop");
        Check(w.People.Sum(p=>p.Reserved)==3,"Shared camps overclaimed finite stone");
        Check(w.SetWorkplacePaused(first!.Id,true),"Quarry pause rejected");
        Until(w,()=>w.Stone==3,"Odd final stone load lost");
        Check(w.Map.StoneDeposits[0].Remaining==0,"Finite shared deposit duplicated");
        Check(w.RemoveBuilding(first.Id),"Creative quarry removal rejected"); w.Validate();
        var corrupt=System.Text.Json.Nodes.JsonNode.Parse(w.SaveJson())!; corrupt["Stone"]=4;
        bool refused=false; try { World.LoadJson(corrupt.ToJsonString()); } catch(InvalidOperationException) { refused=true; }
        Check(refused,"Invented stone save accepted");
    }

    static void RecreationComparison()
    {
        var journeys=new System.Collections.Generic.Dictionary<(bool,int,int),float>();
        foreach(bool working in new[]{false,true}) foreach(int population in new[]{8,16}) foreach(int arrangement in new[]{0,1,2})
        {
            var w=World.NewCreative(working); if(working) { w.Map.Heights=Array.Empty<float>(); w.Map.Water.Clear(); } foreach(var p in w.People) w.Assign(p.Id,Role.Unassigned);
            foreach(var cell in new[]{new Cell(0,0),new(3,0),new(6,0),new(0,6)}) Check(w.Place(cell,false,population==16?BuildingKind.Lodge:BuildingKind.Cottage)!=null,"Recreation homes failed");
            while(w.Population<population) Check(w.InviteNewcomers(),"Recreation population failed");
            Check(w.Place(new(3,6),false,arrangement==0?BuildingKind.GatheringHall:BuildingKind.Square)!=null,"Recreation venue failed");
            if(arrangement==2) Check(w.Place(new(6,6),false,BuildingKind.Square)!=null,"Second square failed");
            if(working)
            {
                foreach(var cell in new[]{new Cell(9,-6),new(9,-3),new(12,0),new(6,9)}) Check(w.Place(cell,false,BuildingKind.VegetableGarden)!=null,"Remote garden failed");
                foreach(var p in w.People) w.Assign(p.Id,Role.Farmer);
            }
            float travel=0, satisfied=0, visitTime=0; int peak=0;
            for(int i=0;i<3000;i++)
            {
                var trips=w.People.Where(p=>p.Task==Work.ToLeisure).Select(p=>(Person:p,Before:p.Position)).ToArray();
                Step(w); satisfied+=w.People.Count(p=>w.ReadHappiness(p).Leisure>0)*.1f; visitTime+=w.People.Count(p=>p.Task==Work.Leisure)*.1f; travel+=trips.Sum(t=>(t.Person.Position-t.Before).Length());
                peak=Math.Max(peak,w.People.Count(p=>p.Task==Work.Leisure));
            }
            Check(w.People.All(p=>p.LeisureVisits>0),"Recreation comparison left somebody unserved");
            journeys[(working,population,arrangement)]=travel;
            Console.WriteLine($"RECREATION {(working?"working":"idle")} {population} residents {new[]{"hall","one square","two squares"}[arrangement]}: {w.People.Sum(p=>p.LeisureVisits)} visits / 300s, peak {peak}, {travel:0.0} visit-travel tiles, {visitTime:0.0}s attending, {satisfied/(population*300):P1} recreation coverage, venue footprint {(arrangement==2?12:6)}, {w.Food.Vegetables} vegetables delivered.");
        }
        foreach(int population in new[]{8,16}) Check(journeys[(true,population,0)]<journeys[(true,population,1)],"Hall did not reduce repeat journeys in working fixture");
    }

    static void DepositDistanceComparison()
    {
        var times=new System.Collections.Generic.List<float>();
        foreach(var location in new[]{new Cell(-9,2),new(11,1)})
        {
            var w=World.NewLargeMap(); foreach(var p in w.People) w.Assign(p.Id,Role.Unassigned);
            var camp=w.Place(location,false,BuildingKind.Quarry) ?? throw new Exception(w.PlacementProblem(location,false,BuildingKind.Quarry));
            w.Assign(0,Role.Logger); w.Assign(1,Role.Logger); w.Assign(2,Role.Builder); w.Assign(3,Role.Builder); w.Assign(4,Role.Quarrier);
            Until(w,()=>camp.Complete,"Distance quarry never built"); Check(w.SetOutputTarget(camp.Id,12),"Stone stock target rejected");
            Until(w,()=>w.Stone==12 && w.People.All(p=>p.WorkplaceId!=camp.Id),"Quarry target did not settle");
            float done=w.Food.Time; Step(w,100); Check(w.Stone==12,"Quarry ignored its output target");
            times.Add(done); Console.WriteLine($"QUARRY {location}: 12 delivered stone including camp setup at {done:0.0}s.");
        }
        Check(times[1]>times[0],"Far resource route did not cost more time");
    }
}

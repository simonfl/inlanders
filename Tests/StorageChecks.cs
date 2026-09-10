using System;
using System.Linq;
using System.Text.Json.Nodes;
using Inlanders.Simulation;

public static class StorageChecks
{
    static void Check(bool ok,string message) { if(!ok) throw new Exception(message); }
    static void Step(World w,int count=1) { for(int i=0;i<count;i++) { w.Tick(.1f); w.Validate(); } }
    static void Until(World w,Func<bool> done,string message) { for(int i=0;i<12000 && !done();i++) Step(w); Check(done(),message); }
    static bool Hauling(World w) => w.People.Any(p=>p.Task is Work.ToHaulPickup or Work.ToHaulDrop);
    public static World Ready()
    {
        var w=new World(40); w.Food.InitialBerries=w.Food.Berries=1000;
        var pile=w.Place(new(3,0),false,BuildingKind.Stockpile)!;
        Until(w,()=>pile.Complete,"Stockpile not built");
        foreach(var p in w.People) w.Assign(p.Id,Role.Unassigned);
        Until(w,()=>w.People.All(p=>p.Carried==0),"Initial returns failed");
        w.SetStorageTarget(pile.Id,0); w.Assign(0,Role.Hauler);
        Until(w,()=>pile.StoredLogs==0 && !Hauling(w),"Fixture drain failed");
        w.Assign(0,Role.Unassigned); w.SetStorageTarget(pile.Id,6);
        return w;
    }
    public static void Run()
    {
        var w=Ready(); var pile=w.Cottages.Single(); int total=w.Stored;
        Check(!w.SetStorageTarget(pile.Id,-1) && !w.SetStorageTarget(pile.Id,13),"Invalid target accepted");
        w.SetStorageTarget(pile.Id,12);
        for(int i=0;i<4;i++) w.Assign(i,Role.Hauler);
        bool pickup=false,drop=false;
        for(int i=0;i<6000 && (pile.StoredLogs<12 || Hauling(w));i++)
        {
            Step(w);
            foreach(var phase in new[]{Work.ToHaulPickup,Work.ToHaulDrop})
            {
                if((phase==Work.ToHaulPickup?pickup:drop) || !w.People.Any(p=>p.Task==phase)) continue;
                if(phase==Work.ToHaulPickup) pickup=true; else drop=true;
                string saved=w.SaveJson();
                var a=World.LoadJson(saved); var b=World.LoadJson(saved);
                Check(a.SaveJson()==saved,"Active shipment state changed on load");
                Step(a,300); Step(b,300); Check(a.SaveJson()==b.SaveJson(),"Hauling save continuation differs");
                var interrupted=World.LoadJson(saved);
                int id=interrupted.People.First(p=>p.Task==phase).Id;
                interrupted.Assign(id,Role.Unassigned); Step(interrupted,500);
                Check(interrupted.Stored==total && interrupted.People.All(p=>p.Carried==0),"Interrupted hauling lost timber");
            }
        }
        Check(pickup && drop && pile.StoredLogs==12 && w.Stored==total && w.YardLogs==total-12,"Stockpile fill/aggregate wrong");
        w.SetStorageTarget(pile.Id,0);
        Until(w,()=>pile.StoredLogs==0 && !Hauling(w),"Target zero did not drain");
        Check(w.YardLogs==total,"Drain lost logs");
        w.SetStorageTarget(pile.Id,12);
        Until(w,()=>w.People.Any(p=>p.Task==Work.ToHaulDrop),"No committed delivery");
        w.SetStorageTarget(pile.Id,0);
        Until(w,()=>pile.StoredLogs==0 && !Hauling(w),"Target change stranded committed shipments");

        // Logger output goes directly to the nearby pile without requiring a hauler.
        foreach(var p in w.People) w.Assign(p.Id,Role.Unassigned);
        w.SetStorageTarget(pile.Id,12);
        var tree=w.Trees.Single(t=>t.Cell==new Cell(3,-5));
        w.People[0].Position=tree.Access.Point; w.Assign(0,Role.Logger);
        Until(w,()=>w.People[0].Task==Work.ToStockpile && w.People[0].StorageId==pile.Id,"Logger skipped nearby stockpile");
        Until(w,()=>pile.StoredLogs>=2,"Local logger deposit failed");
        w.Assign(0,Role.Unassigned);
        Until(w,()=>w.People[0].Carried==0,"Logger return failed");

        // Refill and use a local source for construction and processing.
        w.Assign(1,Role.Hauler);
        Until(w,()=>pile.StoredLogs>=6 && !Hauling(w),"Local refill failed");
        w.Assign(1,Role.Unassigned);
        var cottage=w.Place(new(6,0))!;
        w.Assign(2,Role.Builder);
        Until(w,()=>w.People[2].Task==Work.ToMaterials,"Builder did not claim materials");
        Check(w.People[2].StorageId==pile.Id,"Builder did not use nearer stockpile");
        var cancel=World.LoadJson(w.SaveJson()); int cancelTotal=cancel.Stored;
        Check(cancel.Cancel(cottage.Id),"Construction cancellation failed"); Step(cancel,600);
        Check(cancel.Stored==cancelTotal && cancel.ReservedStorage==0,"Cancellation stranded local reservation");
        Until(w,()=>cottage.Complete,"Locally supplied cottage not built");
        var mill=w.Place(new(3,-3),false,BuildingKind.Sawmill)!;
        w.Assign(1,Role.Hauler);
        Until(w,()=>mill.Complete && pile.StoredLogs>=2,"Mill fixture not supplied");
        w.Assign(2,Role.Unassigned); w.Assign(3,Role.Sawyer);
        Until(w,()=>w.People[3].Task==Work.ToSawLogs,"Sawyer did not fetch logs");
        Check(w.People[3].StorageId==pile.Id,"Sawyer skipped local logs");
        var savedMill=w.SaveJson(); var clone=World.LoadJson(savedMill);
        Step(w,400); Step(clone,400); Check(w.SaveJson()==clone.SaveJson(),"Local sawmill continuation differs");
        Check(w.Planks>0,"Local sawmill did not produce planks");

        // Rebalance directly between stockpiles instead of routing every load through the yard.
        var network=Ready(); var first=network.Cottages.Single();
        var second=network.Place(new(6,0),false,BuildingKind.Stockpile)!;
        network.Assign(0,Role.Builder);
        Until(network,()=>second.Complete,"Second stockpile not built");
        network.Assign(0,Role.Unassigned);
        network.SetStorageTarget(first.Id,12); network.SetStorageTarget(second.Id,0); network.Assign(1,Role.Hauler);
        Until(network,()=>first.StoredLogs==12 && !Hauling(network),"First stockpile did not fill");
        network.Assign(1,Role.Unassigned);
        network.SetStorageTarget(first.Id,0); network.SetStorageTarget(second.Id,12);
        network.People[1].Position=first.Entrance.Point; network.Assign(1,Role.Hauler);
        Until(network,()=>network.People[1].Task==Work.ToHaulPickup,"Direct transfer not claimed");
        Check(network.People[1].StorageId==first.Id && network.People[1].HaulTargetId==second.Id,"Surplus did not supply another local target");
        int networkTotal=network.Stored;
        Until(network,()=>first.StoredLogs==0 && second.StoredLogs==12 && !Hauling(network),"Local transfer did not settle");
        Step(network,300); Check(network.Stored==networkTotal && !Hauling(network),"Stable targets caused transfer churn");
        Check(World.LoadJson(network.SaveJson()).Stored==network.Stored,"Multiple stockpile save total changed");

        // Malformed location claims and physical over-capacity saves are rejected.
        var node=JsonNode.Parse(w.SaveJson())!;
        node["Buildings"]![0]!["StoredLogs"]=13;
        bool rejected=false; try { World.LoadJson(node.ToJsonString()); } catch { rejected=true; }
        Check(rejected,"Over-capacity save accepted");
        var missing=JsonNode.Parse(w.SaveJson())!; missing["People"]![0]!["StorageId"]=999;
        rejected=false; try { World.LoadJson(missing.ToJsonString()); } catch { rejected=true; }
        Check(rejected,"Missing storage destination accepted");
        Console.WriteLine("PASS: stockpile fill/drain, four competing haulers, capacity and inventory reservations, active save/load and reassignment, target changes, local logger/builder/sawyer routes and cancellation.");
    }
}

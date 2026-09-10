using Inlanders.Simulation;
using System.Text.Json.Nodes;

public static class PlankStorageChecks
{
    static void Check(bool ok,string why) { if(!ok) throw new Exception(why); }
    static void Step(World w,int count=1) { for(int i=0;i<count;i++) { w.Tick(.1f); w.Validate(); } }
    static void Until(World w,Func<bool> done,string why) { for(int i=0;i<12000 && !done();i++) Step(w); Check(done(),why); }
    public static void Run()
    {
        var w=World.NewCreative(); foreach(var p in w.People) w.Assign(p.Id,Role.Unassigned);
        var mill=w.Place(new(3,0),false,BuildingKind.Sawmill)!;
        var pile=w.Place(new(6,0),false,BuildingKind.Stockpile)!;
        Check(w.SetStorageMaterial(pile.Id,Resource.Planks),"Empty pile material switch rejected");
        w.Assign(0,Role.Logger); w.Assign(1,Role.Sawyer);
        Until(w,()=>w.People.Any(p=>p.Task==Work.ToStockpile && p.Cargo==Resource.Planks && p.StorageId==pile.Id),"Sawyer did not use nearby plank pile");
        Check(!w.SetStorageMaterial(pile.Id,Resource.Logs),"Changed material with a committed delivery");
        string incoming=w.SaveJson(); var copy=World.LoadJson(incoming); Step(w,80); Step(copy,80); Check(w.SaveJson()==copy.SaveJson(),"Incoming plank save diverged");
        Until(w,()=>pile.StoredPlanks==12,"Plank pile did not fill to capacity");
        Check(w.Planks==12 && w.YardPlanks==0 && w.ReadEconomy().Stocks.Single(s=>s.Resource==Resource.Planks).Stored==12,"Local planks missing or double-counted");
        foreach(var p in w.People) w.Assign(p.Id,Role.Unassigned);
        Until(w,()=>w.People.All(p=>p.Carried==0),"Fixture return stalled");
        string full=w.SaveJson();

        // Creative construction is instant; test physical builders in a normal copy of the same conserved stocks.
        var node=JsonNode.Parse(full)!; node["Creative"]=false; var normal=World.LoadJson(node.ToJsonString());
        var plan=normal.Place(new(3,6),false,BuildingKind.Lodge)!; normal.Assign(2,Role.Builder);
        Until(normal,()=>normal.People[2].Task==Work.ToMaterials,"Builder did not reserve planks");
        Check(normal.People[2].StorageId==pile.Id && normal.ReservedPlanks==2,"Builder ignored local plank stock or reservation");
        Until(normal,()=>normal.People[2].Task==Work.ToCottage,"Builder did not collect planks");
        normal.Cancel(plan.Id); Until(normal,()=>normal.People[2].Carried==0,"Cancelled builder lost cargo");
        Check(normal.Planks==12,"Cancelled construction lost local planks");

        var haul=World.LoadJson(full); haul.SetStorageTarget(pile.Id,0); haul.Assign(2,Role.Hauler); haul.Assign(3,Role.Hauler);
        Until(haul,()=>haul.People.Any(p=>p.Task==Work.ToHaulPickup),"Plank drain pickup missing");
        Check(haul.ReservedStorage==0 && haul.ReservedPlanks>0,"Plank hauling reserved logs");
        string hauling=haul.SaveJson(); var haulCopy=World.LoadJson(hauling); Step(haul,80); Step(haulCopy,80); Check(haul.SaveJson()==haulCopy.SaveJson(),"Hauling plank save diverged");
        Until(haul,()=>haul.Cottages.Single(c=>c.Id==pile.Id).StoredPlanks==0 && haul.People.All(p=>p.Carried==0 && p.Reserved==0),"Competing plank haulers did not drain pile");
        Check(haul.Planks==12 && haul.YardPlanks==12 && haul.SetStorageMaterial(pile.Id,Resource.Logs),"Drain/switch lost planks");
        Check(haul.SetStorageMaterial(pile.Id,Resource.Planks) && haul.SetStorageTarget(pile.Id,6),"Empty refill fixture rejected");
        Until(haul,()=>haul.Cottages.Single(c=>c.Id==pile.Id).StoredPlanks==6 && haul.People.All(p=>p.Carried==0 && p.Reserved==0),"Plank target refill from central storage failed");
        Check(haul.Planks==12 && haul.YardPlanks==6,"Refilling local planks changed village inventory");

        var demolition=World.LoadJson(node.ToJsonString()); demolition.Assign(2,Role.Builder);
        Check(demolition.RequestDemolition(pile.Id),"Plank pile demolition rejected");
        Until(demolition,()=>demolition.Cottages.All(c=>c.Id!=pile.Id) && demolition.People.All(p=>p.Carried==0),"Plank evacuation failed");
        Check(demolition.Planks==12 && demolition.YardPlanks==12,"Demolition lost stored planks");
        var liveNode=JsonNode.Parse(incoming)!; liveNode["Creative"]=false; var live=World.LoadJson(liveNode.ToJsonString());
        Check(live.RequestDemolition(pile.Id),"Demolition rejected committed plank delivery"); live.Assign(2,Role.Builder);
        Check(live.People.All(p=>p.StorageId!=pile.Id && p.HaulTargetId!=pile.Id),"Demolition retained a plank storage claim");
        Until(live,()=>live.Cottages.All(c=>c.Id!=pile.Id),"Incoming delivery demolition stalled"); live.Validate();
        var creative=World.LoadJson(full); Check(creative.RemoveBuilding(pile.Id) && creative.YardPlanks==12 && creative.Planks==12,"Creative removal lost/doubled planks"); creative.Validate();
        Console.WriteLine("PASS: local plank deposits/capacity, builder reservations/cancellation, competing haulers, exact saves, drain/material switch, normal and Creative recovery.");
    }
}

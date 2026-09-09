using System;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using Inlanders.Simulation;

public static class WaterChecks
{
    static void Check(bool value, string message) { if (!value) throw new Exception(message); }
    static void Until(World w, Func<bool> done)
    {
        for(int i=0;i<18000 && !done();i++) { w.Tick(.1f); if(i%50==0) w.Validate(); }
        Check(done(), "Water scenario stalled");
        w.Validate();
    }
    static World Divided()
    {
        var w = World.NewLargeMap();
        for(int z=w.Map.MinZ;z<=w.Map.MaxZ;z++) if(w.Map.Contains(new(7,z))) w.Map.Water.Add(new(7,z));
        return World.LoadJson(w.SaveJson());
    }
    public static void Run()
    {
        var northSouth = new World();
        northSouth.Map.Water.Add(new(3,-2));
        Check(northSouth.Place(new(3,-2),false,BuildingKind.Bridge) != null, "North-south span rejected");
        World.LoadJson(northSouth.SaveJson());
        var w = Divided();
        Check(w.Place(new(10,4)) == null,"Construction allowed without a route across water");
        Check(w.Place(new(7,3)) == null && w.PlantTree(new(7,3)) == null && !w.SetPath(new(7,3),true), "Dry-land tool accepted water");
        Check(w.Place(new(7,3),false,BuildingKind.Bridge) == null, "Bridge accepted banks along the river");
        Check(w.Place(new(3,3),true,BuildingKind.Bridge) == null,"Bridge accepted dry land");
        var bridge = w.Place(new(7,3),true,BuildingKind.Bridge) ?? throw new Exception("Bridge site rejected");
        Check(bridge.Entrance == new Cell(6,3),"Builders did not choose reachable bank");
        Check(w.Place(new(7,3),true,BuildingKind.Bridge) == null,"Duplicate bridge accepted");
        for(int i=0;i<100;i++) {
            w.Tick(.1f);
            Check(w.People.All(p => !w.Map.Water.Contains(World.At(p))),"Walked on unfinished crossing");
            Check(w.People.Where(p=>p.TreeId!=null).All(p=>w.Trees.Single(t=>t.Id==p.TreeId).Cell.X <7),"Claimed unreachable timber");
        }
        Until(w,()=>bridge.Delivered > 0);
        string saved = w.SaveJson(); var copy = World.LoadJson(saved);
        Check(copy.SaveJson()==saved,"Bridge delivery save changed");
        Until(w,()=>bridge.Complete);
        Until(copy,()=>copy.Cottages.Single().Complete);
        Check(w.SaveJson()==copy.SaveJson(),"Bridge continuation diverged");
        Check(!w.Cancel(bridge.Id),"Completed crossing could be removed under workers");
        var house=w.Place(new(10,4));
        Check(house!=null,"Completed bridge did not unlock far bank construction");
        bool crossed=false;
        for(int i=0;i<18000 && !house!.Complete;i++) {
            w.Tick(.1f); w.Validate();
            crossed |= w.People.Any(p=>World.At(p)==bridge.Cell);
            Check(w.People.All(p=>!w.Map.Water.Contains(World.At(p)) || World.At(p)==bridge.Cell),"Worker walked in river");
        }
        Check(house!.Complete && crossed,"Workers did not use bridge for far bank deliveries");
        Check(World.LoadJson(w.SaveJson()).SaveJson()==w.SaveJson(),"Completed crossing did not persist");

        // Cancellation keeps delivered timber on a legal bank, with exclusive claims released.
        var cancel=Divided(); var plan=cancel.Place(new(7,3),true,BuildingKind.Bridge)!;
        Until(cancel,()=>plan.Delivered>=2);
        foreach (var c in new[] { new Cell(6,2),new(5,2),new(5,3),new(6,4) }) cancel.SetPath(c,true);
        Check(cancel.Cancel(plan.Id),"Bridge cancellation failed");
        Check(cancel.Trees.Any(t=>t.Salvage) && cancel.Trees.Where(t=>t.Salvage).All(t=>!cancel.Map.Water.Contains(t.Cell)), "Salvage was lost in water");
        Check(!cancel.Trees.Any(t=>cancel.Paths.Contains(t.Cell)), "Salvage left a path beneath it");
        cancel.Validate(); World.LoadJson(cancel.SaveJson());
        Until(cancel,()=>!cancel.Trees.Any(t=>t.Salvage));
        Check(cancel.People.All(p=>World.At(p).X<7),"Cancelled bridge remained traversable");

        var invalid=JsonNode.Parse(w.SaveJson())!;
        invalid["Map"]!["Water"]!.AsArray().Add(new JsonObject {["X"]=-3,["Z"]=3});
        bool rejected=false;
        try { World.LoadJson(invalid.ToJsonString()); } catch(Exception e) when(e is InvalidDataException or InvalidOperationException) { rejected=true; }
        Check(rejected,"Water under yard accepted");
        Console.WriteLine("PASS: water legality, unreachable jobs, bank-side bridge construction, exact saves, far-bank hauling, dry salvage and invalid water rejection.");
    }
}

using Inlanders.Simulation;

public static class ManagedWoodlandChecks
{
    static void Check(bool ok,string why) { if(!ok) throw new Exception(why); }
    static void Step(World w,int count) { for(int i=0;i<count;i++) { w.Tick(.1f); w.Validate(); } }
    public static void Run()
    {
        var w=World.NewCreative(); foreach(var p in w.People) w.Assign(p.Id,Role.Unassigned);
        foreach(var t in w.Trees) Check(w.SetTreePreserved(t.Cell,true),"Could not preserve existing tree");
        w.Assign(0,Role.Logger); Step(w,150);
        Check(w.Trees.All(t=>!t.Felled) && w.People[0].Status.Contains("Preserved"),"Logger cut preserved timber or explained it incorrectly");
        var tree=w.Trees[0]; w.SetTreePreserved(tree.Cell,false);
        for(int i=0;i<300 && w.People[0].Task!=Work.Chopping;i++) Step(w,1);
        Check(w.People[0].Task==Work.Chopping,"Preservation interruption fixture stalled");
        Check(w.SetTreePreserved(tree.Cell,true),"Active cut could not be stopped"); Step(w,80);
        Check(!tree.Felled && tree.Logs==8 && tree.Owner==null,"Preservation lost wood or leaked a claim");

        var grove=new[]{new Cell(3,0),new(6,0),new(3,6)};
        foreach(var cell in grove) Check(w.SetManagedWoodland(cell,true),"Could not mark empty grove spot");
        w.Assign(1,Role.Logger);
        Step(w,9000);
        Check(w.TreesPlanted>=9 && w.GrownLogs>=72 && w.Stored>32,"Managed grove did not sustain repeated physical harvest/replant cycles");
        Check(w.Trees.Where(t=>t.Preserved).All(t=>!t.Felled),"Grove management harvested protected scenery");
        string saved=w.SaveJson(); var copy=World.LoadJson(saved);
        Check(copy.SaveJson()==saved,"Woodland save changed"); Step(w,150); Step(copy,150);
        Check(w.SaveJson()==copy.SaveJson(),"Managed woodland continuation diverged");

        var cleared=grove[0];
        Check(w.SetClearing(cleared,true),"Grove clearing rejected");
        Check(!w.ManagedWoodland.Contains(cleared),"Clearing did not release automatic planting");
        Step(w,2200); Check(w.Trees.All(t=>t.Cell!=cleared),"Cleared grove replanted itself");
        var preserved=w.Trees.First(t=>t.Preserved);
        Check(w.SetClearing(preserved.Cell,true),"Explicit clearing did not override preservation");
        Check(!w.Trees.Contains(preserved),"Creative clearing retained protected tree");

        var plans=World.NewCreative(); foreach(var p in plans.People) plans.Assign(p.Id,Role.Unassigned);
        Check(plans.SetManagedWoodland(new(3,0),true),"Construction grove fixture rejected");
        Check(plans.Place(new(3,0))!=null && !plans.ManagedWoodland.Contains(new(3,0)),"Construction did not replace grove designation");
        Check(plans.SetManagedWoodland(new(6,0),true) && plans.SetPath(new(6,0),true) && !plans.ManagedWoodland.Contains(new(6,0)),"Path did not override automatic planting");
        Check(plans.SetManagedWoodland(new(3,6),true) && plans.PlaceDecoration(new(3,6),DecorationKind.Flowers) && !plans.ManagedWoodland.Contains(new(3,6)),"Decoration did not override automatic planting");
        plans.Validate();
        var normal=World.NewCampaign(2);
        foreach(var p in normal.People) normal.Assign(p.Id,Role.Unassigned);
        normal.Assign(0,Role.Logger); normal.Assign(4,Role.Forager); normal.Assign(5,Role.Forager);
        foreach(var t in normal.Trees) normal.SetTreePreserved(t.Cell,true);
        foreach(var cell in new[]{new Cell(0,-3),new(3,-3),new(6,-3)}) Check(normal.SetManagedWoodland(cell,true),"Normal grove rejected");
        Step(normal,9000);
        Console.WriteLine($"Normal woodland: {normal.GrownLogs} grown logs, {normal.TreesPlanted} planted, {normal.People[0].RestVisits} rest visits, hunger {normal.Food.Hunger}; {normal.People[0].Status}");
        Check(normal.GrownLogs>=48 && normal.TreesPlanted>=9 && normal.People[0].RestVisits>=3 && normal.Food.Hunger==0,"Grove failed alongside actual home rest and food supply");
        var protectedTree=normal.Trees.First(t=>t.Preserved);
        Check(normal.SetManagedWoodland(protectedTree.Cell,true) && normal.SetClearing(protectedTree.Cell,true),"Normal clearing rejected protected grove");
        Check(!protectedTree.Preserved && !normal.ManagedWoodland.Contains(protectedTree.Cell),"Normal clearing retained conflicting settings");
        Check(normal.SetClearing(protectedTree.Cell,false) && !protectedTree.Preserved && !normal.ManagedWoodland.Contains(protectedTree.Cell),"Cancelling clearing restored obsolete woodland orders");
        normal.Validate();
        Console.WriteLine("PASS: preserved trees and active-cut interruption, repeated managed harvest/replant cycles, exact saves, and clearing/building/path/decoration precedence.");
    }
}

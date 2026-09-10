using Inlanders.Simulation;
using System.Text.Json.Nodes;

public static class WildlifeChecks
{
    static void Check(bool ok,string why) { if(!ok) throw new Exception(why); }
    static void Step(World w,int ticks=1) { for(int i=0;i<ticks;i++) { w.Tick(.1f); w.Validate(); } }
    static void Until(World w,Func<bool> done,string why) { for(int i=0;i<10000 && !done();i++) Step(w); Check(done(),why+": "+string.Join("; ",w.People.Select(p=>p.Status))); }
    static World Fixture(bool creative=true)
    {
        var w=creative?World.NewCreative():new World();
        foreach(var p in w.People) w.Assign(p.Id,Role.Unassigned);
        var h=new WoodlandHabitat { Id=0,Cell=new(-3,-3),Stock=10 }; w.Map.Wildlife.Add(h);
        Check(w.Place(new(3,0),false,BuildingKind.HuntingLodge)!=null,"Hunting lodge fixture rejected");
        return w;
    }
    public static void Run()
    {
        var large=World.NewLargeMap(); Check(large.Map.Wildlife.Count==2,"Authored habitats missing");
        Check(new World().PlacementProblem(new(3,0),false,BuildingKind.HuntingLodge)!=null,"Hunting anywhere allowed");
        var w=Fixture(false); w.Assign(1,Role.Logger); w.Assign(2,Role.Builder); w.Assign(0,Role.Hunter);
        Until(w,()=>w.People[0].Task==Work.Hunting,"Normal hunter never reached habitat");
        var saved=w.SaveJson(); var copy=World.LoadJson(saved); Check(copy.SaveJson()==saved,"Hunt save changed");
        Step(w,130); Step(copy,130); Check(w.SaveJson()==copy.SaveJson(),"Hunt continuation diverged");
        Until(w,()=>w.Food.EatenGame>0,"Delivered game never reached meals");
        Check(w.LastMealSummary.Contains("game") && w.ReadFoodFlow().Game>0,"Game feedback omitted");
        Check(w.ReadEconomy().Stocks.Single(s=>s.Resource==Resource.Game).Stored==w.Food.Game,"Game inventory incorrect");
        w=Fixture(); w.Assign(0,Role.Hunter); Until(w,()=>w.People[0].HabitatId!=null,"No wildlife claim");
        float stock=w.Map.Wildlife[0].Stock;
        w.Assign(0,Role.Builder); Check(w.People[0].HabitatId==null && w.Map.Wildlife[0].Stock==stock,"Interrupted hunt consumed wildlife");
        w.Assign(0,Role.Hunter); Until(w,()=>w.People[0].Carried>0,"No game cargo");
        Check(w.People[0].Cargo==Resource.Game && w.Food.Game==0,"Undelivered game counted as food");
        w.Assign(0,Role.Unassigned); Until(w,()=>w.Food.Game==2,"Reassigned hunter lost game");
        w.Assign(0,Role.Hunter); w.SetOutputTarget(w.Cottages[0].Id,4);
        Until(w,()=>w.Food.Game>=4,"Game target never filled"); Step(w,300); Check(w.Food.Game==4,"Game stock target ignored");
        w.SetOutputTarget(w.Cottages[0].Id,-1); Until(w,()=>w.People[0].HabitatId!=null,"Hunter did not resume");
        Check(w.RemoveBuilding(w.Cottages[0].Id),"Active lodge removal failed"); Check(w.People[0].HabitatId==null,"Removed lodge retained claim"); w.Validate();
        SharedAndRecovery(); Meals(); CompareFood();
        Console.WriteLine("PASS: hunting construction, actual meals/flow, physical game, exact saves, role interruption, targets, lodge removal, shared habitat and woodland recovery.");
    }
    static void SharedAndRecovery()
    {
        var w=Fixture(); var h=w.Map.Wildlife[0]; h.Stock=3;
        Check(w.Place(new(0,0),false,BuildingKind.HuntingLodge)!=null,"Second lodge rejected");
        w.Assign(0,Role.Hunter); w.Assign(1,Role.Hunter);
        Until(w,()=>w.People.Count(p=>p.HabitatId!=null)==2,"Shared habitat unclaimed");
        Check(w.People.Sum(p=>p.Reserved)==3,"Lodges duplicated habitat stock");
        foreach(var c in w.Cottages) w.SetWorkplacePaused(c.Id,true);
        Until(w,()=>w.Food.Game==3,"Odd shared catch lost");
        Step(w,3000); Check(h.Stock>=9,"Paused hunting failed to recover");
        var cells=w.Trees.Where(t=>h.Contains(t.Cell)).Select(t=>t.Cell).ToArray();
        Check(w.HabitatLoss(cells[0]).Contains("recovery"),"Tree loss lacks preview");
        foreach(var cell in cells) Check(w.SetClearing(cell,true),"Habitat clearing failed");
        Step(w); Check(w.HabitatCapacity(h)==0 && h.Stock==0,"Clearing left productive habitat");
        Step(w,1000); Check(h.Stock==0,"Treeless habitat regenerated");
        foreach(var cell in cells) { Check(w.PlantTree(cell)!=null,"Replant failed"); w.SetTreePreserved(cell,true); }
        w.Assign(2,Role.Logger); Until(w,()=>w.Trees.Where(t=>h.Contains(t.Cell)).All(t=>!t.NeedsPlanting),"Habitat replant work stalled");
        Check(w.HabitatCapacity(h)==0,"Saplings instantly restored wildlife");
        Until(w,()=>w.HabitatCapacity(h)>=8 && h.Stock>=2,"Mature woodland failed to restore wildlife");
        var restored=World.LoadJson(w.SaveJson()); Step(w,100); Step(restored,100); Check(w.SaveJson()==restored.SaveJson(),"Habitat recovery save diverged");
        var bad=JsonNode.Parse(w.SaveJson())!; bad["Food"]!["Game"]=999; bool rejected=false;
        try { World.LoadJson(bad.ToJsonString()); } catch(InvalidOperationException) { rejected=true; }
        Check(rejected,"Invented game accepted");
    }
    static void CompareFood()
    {
        foreach(bool garden in new[]{false,true})
        {
            var w=Fixture(); w.Assign(0,Role.Hunter);
            if(garden) { Check(w.Place(new(6,0),false,BuildingKind.VegetableGarden)!=null,"Comparison garden rejected"); w.Assign(1,Role.Farmer); }
            Step(w,6000);
            Console.WriteLine($"WILDLIFE 10m {(garden?"hunt + garden":"hunt only")}: {w.Food.Game} game, {w.Food.Vegetables} vegetables, habitat {w.Map.Wildlife[0].Stock:0.0}/{w.HabitatCapacity(w.Map.Wildlife[0])}; {w.Cottages.Sum(c=>Buildings.Get(c.Kind).Cost)} logs setup, {(garden?2:1)} workers.");
        }
    }
    static void Meals()
    {
        foreach(bool mixed in new[]{false,true})
        {
            var w=new World(); foreach(var p in w.People) w.Assign(p.Id,Role.Unassigned);
            w.Food.Berries=w.Food.InitialBerries=mixed?2:0;
            w.Food.Game=w.Food.HuntedGame=mixed?3:8;
            w.Food.Vegetables=w.Food.GrownVegetables=mixed?3:0;
            Step(w,601);
            Check(w.Food.LastMealServed==8 && w.Food.Hunger==0 && w.Food.LastMealGame==(mixed?3:8),"Game failed to nourish residents");
            Check(w.MealVarietyScore==(mixed?20:0),"Game changed existing three-food variety rule");
        }
        var active=Fixture(); active.Assign(0,Role.Hunter);
        Until(active,()=>active.People[0].Task==Work.Hunting,"Active clearing fixture stalled");
        foreach(var t in active.Trees.Where(t=>active.Map.Wildlife[0].Contains(t.Cell)).ToArray()) active.SetClearing(t.Cell,true);
        Step(active); Check(active.Map.Wildlife[0].Stock>=active.People[0].Reserved,"Tree loss broke committed outing");
        Until(active,()=>active.Food.Game==2,"Committed hunt lost after clearing");
        Step(active,800); Check(active.Food.Game==2,"Treeless woodland allowed further hunts");
    }
}

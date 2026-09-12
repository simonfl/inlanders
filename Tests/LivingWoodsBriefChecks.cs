using Inlanders.Simulation;

static class LivingWoodsBriefChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    static void Until(World w,Func<bool> done,string why,int limit=24000)
    {
        for(int i=0;i<limit && !done();i++){w.Tick(.1f);w.Validate();}
        Check(done(),$"{why} failed at {w.Food.Time:F0}s");
    }
    static void Build(World w,Cell cell,BuildingKind kind)=>Check(w.Place(cell,0,kind)!=null,$"Cannot place {kind}: {w.PlacementProblem(cell,0,kind)}");
    public static void Run()
    {
        var initial=World.NewLivingWoodsMap();
        Check(initial.Map.Wildlife.All(h=>initial.HabitatTrees(h)==6),"Each woodland must start with six mature trees");
        Check(initial.Trees.Count(t=>!initial.Map.Wildlife.Any(h=>h.Contains(t.Cell)))==3,"Outside timber budget changed");
        Check(initial.InitialLogs==168 && initial.YardLogs==16 && initial.Food.Berries==32,"Woods starter budget changed");
        foreach(bool mixed in new[]{false,true})
        {
            var w=World.NewLivingWoodsMap();
            foreach(var t in w.Trees.Where(t=>w.Map.Wildlife.Any(h=>h.Contains(t.Cell))))w.SetTreePreserved(t.Cell,true);
            if(mixed)
            {
                foreach(var cell in new[]{new Cell(-7,-3),new(-4,-3)})w.SetClearing(cell,true);
                Build(w,new(-3,0),BuildingKind.VegetableGarden);
            }
            Build(w,new(0,-3),BuildingKind.HuntingLodge);
            if(!mixed)Build(w,new(5,-3),BuildingKind.HuntingLodge);
            Build(w,new(2,-6),BuildingKind.Cottage);Build(w,new(5,0),BuildingKind.Cottage);
            w.Assign(6,Role.Hunter);w.Assign(7,mixed?Role.Farmer:Role.Hunter);
            Until(w,()=>w.Beds>=12,"new homes");
            for(int pair=0;pair<2;pair++)
            {
                Until(w,()=>w.InvitationProblem()==null,"newcomer provisions");Check(w.InviteNewcomers(),"Newcomers rejected");
            }
            Until(w,()=>w.Food.EatenGame>=4,"actual woodland meals");
            Until(w,()=>w.ReadMealAssessment().Reliable && w.ReadMealAssessment().FreshSupply && w.Map.Wildlife.All(h=>w.HabitatTrees(h)>=4),"supported woodland village");
            Console.WriteLine($"Woods {(mixed?"mixed cultivation":"preservation/hunting")}: 12 housed, real game meals and reliable fresh food at {w.Food.Time:F0}s; mature trees {string.Join('/',w.Map.Wildlife.Select(w.HabitatTrees))}, stock {string.Join('/',w.Map.Wildlife.Select(h=>h.Stock.ToString("F1")))}.");
            string saved=w.SaveJson();Check(World.LoadJson(saved).SaveJson()==saved,"Woods save differs");
            if(mixed)Recover(w);else HuntingPressure(w);
        }
    }
    static void HuntingPressure(World w)
    {
        var west=w.Map.Wildlife[0];
        Until(w,()=>west.Stock<2,"hunting pressure with intact trees");
        Check(w.HabitatTrees(west)==6,"Hunting-pressure fixture lost trees");
        float low=west.Stock;
        foreach(var lodge in w.Cottages.Where(c=>c.Kind==BuildingKind.HuntingLodge))w.SetWorkplacePaused(lodge.Id,true);
        w=World.LoadJson(w.SaveJson());west=w.Map.Wildlife[0];
        Until(w,()=>west.Stock>=8 && w.ReadMealAssessment().Reliable && w.ReadMealAssessment().FreshSupply,"resting the hunting ground");
        Console.WriteLine($"Hunting-pressure recovery: west stock {low:F1} to {west.Stock:F1}, six trees retained, continuing food at {w.Food.Time:F0}s; no replanting needed.");
    }
    static void Recover(World w)
    {
        var habitat=w.Map.Wildlife[0];
        var cells=w.Trees.Where(t=>habitat.Contains(t.Cell)).Select(t=>t.Cell).ToArray();
        foreach(var cell in cells)w.SetClearing(cell,true);
        Until(w,()=>w.HabitatTrees(habitat)==0,"over-cleared habitat");
        Check(w.HabitatCapacity(habitat)==0 && w.HabitatRecovery(habitat)==0,"Tree loss did not remove habitat support");
        Console.WriteLine($"Over-clearing at {w.Food.Time:F0}s: west capacity and regeneration both zero.");
        Until(w,()=>cells.All(c=>w.Trees.All(t=>t.Cell!=c)),"collect timber and remove roots");
        foreach(var cell in cells.Take(4))
        {
            Check(w.PlantTree(cell)!=null,"Restoration planting rejected");
            Check(w.SetTreePreserved(cell,true),"Cannot protect restoration planting from later harvest");
        }
        w=World.LoadJson(w.SaveJson());habitat=w.Map.Wildlife[0];
        Until(w,()=>w.HabitatTrees(habitat)>=4 && w.ReadMealAssessment().Reliable && w.ReadMealAssessment().FreshSupply,"woodland restoration and interim food");
        Console.WriteLine($"Saved clearing recovery: restored mature habitat and continuing meals at {w.Food.Time:F0}s.");
    }
}

using Inlanders.Simulation;
static class InletBankChecks
{
    public static void Run()
    {
        var w=new HamletProfile(false,true,false,true).Create();var bank=new HamletProfile(false,true).Create();
        if(w.Trees.Count!=bank.Trees.Count || w.Trees.Sum(t=>t.Logs)!=bank.Trees.Sum(t=>t.Logs) || w.Stored!=bank.Stored || w.Planks!=bank.Planks || w.EdibleStored!=bank.EdibleStored || w.Housed!=bank.Housed)throw new Exception("Bank geometry changed starting inventory");
        if(w.Cottages.Where(c=>World.IsVegetablePlot(c.Kind)).Sum(c=>World.VegetableYield(c.Kind))!=48)throw new Exception("Bank geometry changed crop capacity");
        if(!w.Map.Water.Contains(new(0,1)) || !w.Map.Water.Contains(new(-3,-1)) || w.Map.Water.Contains(new(10,-8)))throw new Exception("Backwaters or northern bank missing");
        if(w.Map.FishingGrounds.Any(g=>!w.Map.Water.Contains(g.Cell)) || w.Cottages.Any(c=>!w.Paths.Contains(c.Entrance)))throw new Exception("Resources or approaches disconnected");
        if(w.PlacementProblem(new(5,0),0,BuildingKind.Bridge)!=null)throw new Exception("Practical crossing lost");
        w.Validate();var copy=World.LoadJson(w.SaveJson());if(copy.SaveJson()!=w.SaveJson())throw new Exception("Bank save differs");
        Console.WriteLine("PASS inlet bank: actual backwaters/shore land, matched inventory/crops, accessible places, fishing water and retained crossing.");
    }
}

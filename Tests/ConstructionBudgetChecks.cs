using Inlanders.Simulation;
static class ConstructionBudgetChecks
{
    public static void Run()
    {
        var w=World.NewRestorationChallenge();var home=w.Place(new(-2,10),0,BuildingKind.Cottage)??throw new Exception("Home refused");var bridge=w.Place(new(5,0),0,BuildingKind.Bridge)!;
        MaterialBudget Read()=>w.ReadConstructionBudget(BuildingKind.VegetableField).Single();
        var before=w.SaveJson();var budget=Read();
        if(budget.Planned!=10 || budget.Queued!=12 || budget.Free!=10 || budget.Shortfall!=12 || before!=w.SaveJson())throw new Exception("Budget does not account for competing orders purely");
        w.SetConstructionPaused(bridge.Id,true);if(Read().Shortfall!=6)throw new Exception("Paused bridge still consumes budget");
        w.Cancel(home.Id);if(Read().Shortfall!=0)throw new Exception("Cancelled home still consumes budget");
        w.SetConstructionPaused(bridge.Id,false);for(int i=0;i<5;i++)w.Tick(.1f);
        if(Read().Shortfall!=6)throw new Exception("Claimed deliveries counted twice");
        if(w.ReadConstructionBudget(BuildingKind.VegetableField,3).Single().Planned!=6 || World.NewRestorationChallenge(true).ReadConstructionBudget(BuildingKind.Cottage).Length!=0)throw new Exception("Plot size/Relaxed budget wrong");
        w.Validate();Console.WriteLine("PASS construction budget: competing orders, pause, cancel, reservations, plot size, Relaxed and pure queries");
    }
}

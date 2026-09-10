namespace Inlanders.Simulation;

public sealed partial class World
{
    // Read-only access to the production pathfinder for the test-only experiment.
    public int? MealExperimentRouteTicks(Cell start,Cell goal)
    {
        var route=FindPath(start,goal,Blocked);
        return route==null?null:route.Sum(c=>(int)Math.Ceiling(1/(1.8*(Paths.Contains(c)?1.25:1))*10));
    }
}

using System.Linq;

namespace Inlanders.Simulation;

public sealed partial class World
{
    // Bounded campaign situation prototype: visible resource geography and one larger commitment.
    public static World NewFoodLandChallenge()
    {
        var w=NewWorkplaceFoodExperiment();w.Neighborhood!.FoodLandChallenge=true;
        w.Map.Name="The meadow settlement — prototype";
        // The lone western patch regrows at the normal rate. No artificial famine or crop-yield modifier.
        var source=w.Bushes.Single(b=>b.Cell==new Cell(2,6));
        w.Bushes.RemoveAll(b=>b.Id!=source.Id);
        w.Food.InitialBerries=w.Food.Berries=24;
        return w;
    }
}

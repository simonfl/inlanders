namespace Inlanders.Simulation;
public sealed partial class World
{
    private static bool EstablishmentKind(BuildingKind kind)=>kind is BuildingKind.Farm or BuildingKind.VegetableGarden or BuildingKind.VegetableField or BuildingKind.Bakery or BuildingKind.FishingDock;
    // Finish the real first crop/batch/trip, including collecting every baked loaf.
    // Completed cycles remain completed across relocation and save/load.
    public bool EstablishmentWork(Cottage site)=>site.EstablishmentPending ||
        Founding is {PlayerFounded:true,ReserveOnlyWork:false} && site.Kind==BuildingKind.Bakery && site.OutputBread>0;
}

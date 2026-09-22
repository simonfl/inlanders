namespace Inlanders.Simulation;

// The retained public place owns its entry, restart and save identity together.
public sealed record HamletProfile(bool Relaxed, bool CultivatedBank=false,bool GroupedFarmsteads=false,bool AcrossTheInlet=false)
{
    public string Title => AcrossTheInlet ? "Across the inlet" : GroupedFarmsteads ? "Homes among the fields" : CultivatedBank ? "The cultivated bank" : "Between wood and water";
    public string ModeName => Relaxed ? "Relaxed" : "Normal";
    public string SaveName => AcrossTheInlet ? (Relaxed ? "across-inlet-relaxed.json" : "across-inlet.json") : GroupedFarmsteads ? (Relaxed ? "grouped-farmsteads-relaxed.json" : "grouped-farmsteads.json") : CultivatedBank ? (Relaxed ? "cultivated-bank-relaxed.json" : "cultivated-bank.json") : Relaxed ? "transformation-relaxed.json" : "transformation.json";
    public World Create() => World.NewTransformationHamlet(Relaxed,CultivatedBank,GroupedFarmsteads,AcrossTheInlet);
}
public sealed partial class World
{
    public HamletProfile? PublicPlace => Founding?.TransformationHamlet==true ? new(Creative,Founding.CultivatedBank,Founding.GroupedFarmsteads,Founding.AcrossTheInlet) : null;
}

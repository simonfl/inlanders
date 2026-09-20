namespace Inlanders.Simulation;

// The retained public place owns its entry, restart and save identity together.
public sealed record HamletProfile(bool Relaxed, bool CultivatedBank=false)
{
    public string Title => CultivatedBank ? "The cultivated bank" : "Between wood and water";
    public string ModeName => Relaxed ? "Relaxed" : "Normal";
    public string SaveName => CultivatedBank ? (Relaxed ? "cultivated-bank-relaxed.json" : "cultivated-bank.json") : Relaxed ? "transformation-relaxed.json" : "transformation.json";
    public World Create() => World.NewTransformationHamlet(Relaxed,CultivatedBank);
}
public sealed partial class World
{
    public HamletProfile? PublicPlace => Founding?.TransformationHamlet==true ? new(Creative,Founding.CultivatedBank) : null;
}

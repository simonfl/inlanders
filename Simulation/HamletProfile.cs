namespace Inlanders.Simulation;

// The retained public place owns its entry, restart and save identity together.
public sealed record HamletProfile(bool Relaxed)
{
    public string SaveName => Relaxed ? "transformation-relaxed.json" : "transformation.json";
    public World Create() => World.NewTransformationHamlet(Relaxed);
}
public sealed partial class World
{
    public HamletProfile? PublicPlace => Founding?.TransformationHamlet==true ? new(Creative) : null;
}

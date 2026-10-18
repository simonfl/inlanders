using System.Linq;
namespace Inlanders.Simulation;
public sealed record ConstructionReport(string Detail,Resource? Missing=null);
public sealed partial class World
{
    public ConstructionReport ReadConstruction(Cottage site)
    {
        if(site.Complete)return new("Ready for use.");
        if(site.ConstructionPaused)return new("Construction paused; delivered materials stay here.");
        if(site.Builder is int builder)return new(People[builder].Name+": "+People[builder].Status);
        foreach(var material in new[]{site.Material,Resource.Stone}.Distinct())
            if(site.Remaining(material)>0 && !TryMaterialSource(site.Entrance,material,1,out _))
                return new($"Waiting for reachable, unreserved {material.ToString().ToLowerInvariant()}."+
                    (site.Incoming+site.IncomingStone>0?" Some supplies are already on the way.":""),material);
        if(site.Incoming+site.IncomingStone>0)return new($"{site.Incoming+site.IncomingStone} materials on the way; deliveries are claimed.");
        if(site.MaterialsReady)return new("Materials ready; waiting for a free builder.");
        return new("Supplies available; shared workers take this job after current work.");
    }
}

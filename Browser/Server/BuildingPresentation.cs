using Inlanders.Simulation;

namespace Inlanders.Browser;

public sealed partial class VillageSessions
{
    // Inspection describes the existing work queue; it does not assign or reprioritize it.
    static string BuildingStatus(World w,Cottage site)
    {
        if(site.Id<0)return "Clearing ground, then building automatically";
        if(site.Complete)return site.Kind==BuildingKind.Cottage ? "Home" :
            site.Kind==BuildingKind.Bridge ? "Crossing open" : w.ReadWorkplace(site).State;
        if(site.ConstructionPaused)return "Construction paused";
        if(site.MaterialsReady)return $"Building {site.Construction:P0}" +
            (site.Builder==null ? " · Waiting for a builder" : " · Builder assigned");

        string status=$"Logs {site.Delivered}/{site.Required}";
        if(site.Incoming>0)status+=$" · {site.Incoming} incoming";
        var report=w.ReadConstruction(site);
        if(report.Missing is {} missing)
        {
            status+=$" · Waiting for reachable {missing.ToString().ToLowerInvariant()}";
            if(missing==Resource.Logs && w.VillageStory is {Departed:false,PackedLogs:>0} cargo)
                status+=$" ({cargo.PackedLogs} aboard the boat)";
        }
        else if(site.Incoming==0)status+=" · Ready for delivery";
        return status;
    }
}

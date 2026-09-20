using System.Linq;
namespace Inlanders.Simulation;
public sealed partial class World
{
    public bool SetConstructionPaused(int id,bool paused)
    {
        var site=Cottages.FirstOrDefault(c=>c.Id==id);
        if(site==null || site.Complete || Food.Celebrating)return false;
        if(site.ConstructionPaused==paused)return true;
        site.ConstructionPaused=paused;
        if(paused)foreach(var worker in People.Where(p=>p.SiteId==id).ToArray())Interrupt(worker);
        History.Add($"{Buildings.Get(site.Kind).Name} {id}: construction {(paused?"paused":"resumed")}; delivered materials stay on site.");
        _retry=0;return true;
    }
}

using System;
using System.Linq;

namespace Inlanders.Simulation;

public enum CivicIdentity { Hall, Chapel, PlantedCourt }

public sealed partial class World
{
    public bool SetCivicIdentity(int id,CivicIdentity identity)
    {
        if(!Enum.IsDefined(identity))return false;
        var site=Cottages.FirstOrDefault(c=>c.Id==id && c.Kind==BuildingKind.GatheringHall && !c.DemolitionRequested);
        if(site==null)return false;
        site.Identity=identity;return true;
    }
}

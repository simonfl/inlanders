using System;
using System.Linq;

namespace Inlanders.Simulation;

public enum CottageFinish { Automatic, Clay, Sage, Ochre, Slate, Rose }

public sealed partial class World
{
    public static CottageFinish ResolvedCottageFinish(Cottage home) => home.Finish==CottageFinish.Automatic?(CottageFinish)(1+home.Id%3):home.Finish;
    public bool SetCottageFinish(int id,CottageFinish finish)
    {
        if(!Enum.IsDefined(finish))return false;
        var home=Cottages.FirstOrDefault(c=>c.Id==id && c.Kind==BuildingKind.Cottage && !c.DemolitionRequested);
        if(home==null)return false;
        home.Finish=finish;return true;
    }
}

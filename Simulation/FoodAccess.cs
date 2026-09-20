using System;
using System.Linq;
namespace Inlanders.Simulation;
public sealed record FoodAccessRoute(FoodMapStore Store,Cell[] Route,bool ServesSharedPlace);
public sealed partial class World
{
    public FoodAccessRoute[] ReadFoodAccess(Cell from)
    {
        if(!Map.Contains(from) || Blocked(from))return Array.Empty<FoodAccessRoute>();
        return ReadFoodMap().Select(s=>new{Store=s,Path=FindPath(from,s.Access,Blocked)})
            .Where(s=>s.Path!=null).Select(s=>new FoodAccessRoute(s.Store,s.Path!.ToArray(),(from.Point-s.Store.Access.Point).LengthSquared()<=64))
            .OrderByDescending(s=>s.Store.Available>0).ThenBy(s=>s.Route.Length).ThenBy(s=>s.Store.Id??-1).ToArray();
    }
}

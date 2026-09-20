using System.Linq;
namespace Inlanders.Simulation;
public sealed partial class World
{
    public Cell[] HomeYardPlaces(Cottage home)=>Founding?.RiverFarmstead==true && home.Improved && IsHome(home)
        ? new[]{RotateOffset(home.Cell,-1,1,home.Rotation),RotateOffset(home.Cell,1,1,home.Rotation)}
            .Where(c=>Map.Contains(c) && !Blocked(c) && c!=YardAccess && !Cottages.Any(s=>s.Entrance==c) &&
                !(Commons is {} commons && (commons.Center==c || commons.Places.Contains(c)))).ToArray()
        :System.Array.Empty<Cell>();
    public bool AtFurnishedHome(Villager person)=>person.HomeId is int id && Cottages.FirstOrDefault(c=>c.Id==id) is {} home && HomeYardPlaces(home).Contains(At(person));
    public bool QuietAtFurnishedHome(Villager person)=>person.Task==Work.Waiting && person.Route.Count==0 && AtFurnishedHome(person);
    private Cell? HomeMealPlace(Villager person,Cell supply)
    {
        if(person.HomeId is not int id || Cottages.FirstOrDefault(c=>c.Id==id) is not {} home ||
            (home.Entrance.Point-supply.Point).LengthSquared()>64)return null;
        return HomeYardPlaces(home).Where(c=>!MealSpotReserved(c) && !ComfortSpotReserved(c) &&
            !People.Any(p=>p.Id!=person.Id && (At(p)==c || p.Destination==c && p.Route.Count>0)))
            .Cast<Cell?>().FirstOrDefault(c=>FindPath(supply,c!.Value,Blocked)!=null);
    }
}

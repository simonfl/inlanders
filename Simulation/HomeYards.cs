using System.Linq;
namespace Inlanders.Simulation;
public sealed partial class World
{
    public Cell[] HomeYardPlaces(Cottage home)=>home.Improved?PotentialHomeYardPlaces(home):System.Array.Empty<Cell>();
    public Cell[] PotentialHomeYardPlaces(Cottage home)=>Founding?.RiverFarmstead==true && IsHome(home)
        ? new[]{RotateOffset(home.Cell,-1,1,home.Rotation),RotateOffset(home.Cell,1,1,home.Rotation)}
            .Where(c=>Map.Contains(c) && !Blocked(c) && c!=YardAccess && !Cottages.Any(s=>s.Entrance==c) &&
                !(Commons is {} commons && (commons.Center==c || commons.Places.Contains(c)))).ToArray()
        :System.Array.Empty<Cell>();
    public bool AtFurnishedHome(Villager person)=>person.HomeId is int id && Cottages.FirstOrDefault(c=>c.Id==id) is {} home && HomeYardPlaces(home).Contains(At(person));
    public bool AvailableAtHome(Villager person)=>person.SharedWorker && person.Task==Work.Waiting && person.Route.Count==0 &&
        person.HomeId is int id && Cottages.FirstOrDefault(c=>c.Id==id && c.Complete && IsHome(c)) is {} home &&
        (At(person).Point-home.Entrance.Point).LengthSquared()<=4 && At(person)!=YardAccess && !Cottages.Any(c=>c.Entrance==At(person));
    public bool QuietAtFurnishedHome(Villager person)=>AvailableAtHome(person) && AtFurnishedHome(person);
    private Cell? HomeMealPlace(Villager person,Cell supply)
    {
        if(person.HomeId is not int id || Cottages.FirstOrDefault(c=>c.Id==id) is not {} home ||
            (home.Entrance.Point-supply.Point).LengthSquared()>64)return null;
        return HomeYardPlaces(home).Where(c=>!MealSpotReserved(c) && !ComfortSpotReserved(c) &&
            !People.Any(p=>p.Id!=person.Id && (At(p)==c || p.Destination==c && p.Route.Count>0)))
            .Cast<Cell?>().FirstOrDefault(c=>FindPath(supply,c!.Value,Blocked) is {} path && path.Count<=12);
    }
}

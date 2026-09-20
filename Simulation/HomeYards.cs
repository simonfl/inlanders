using System.Linq;
namespace Inlanders.Simulation;
public sealed partial class World
{
    public Cell[] HomeYardPlaces(Cottage home)=>home.Improved?PotentialHomeYardPlaces(home):System.Array.Empty<Cell>();
    public static string YardSideName(int side)=>new[]{"Entrance","Left side","Behind home","Right side"}[side];
    public Cell[] PotentialHomeYardPlaces(Cottage home)=>YardPlaces(home,home.YardSide);
    private Cell[] YardGround(Cottage home,int side)=>Founding?.RiverFarmstead==true && IsHome(home)
        ? (side switch {
            0=>new[]{new Cell(-1,1),new Cell(1,1)},
            1=>new[]{new Cell(-2,-1),new Cell(-2,0)},
            2=>new[]{new Cell(-1,-2),new Cell(1,-2)},
            3=>new[]{new Cell(2,-1),new Cell(2,0)},
            _=>System.Array.Empty<Cell>()
        }).Select(c=>RotateOffset(home.Cell,c.X,c.Z,home.Rotation)).ToArray()
        :System.Array.Empty<Cell>();
    public Cell[] YardPlaces(Cottage home,int side)=>YardGround(home,side)
            .Where(c=>Map.Contains(c) && !Blocked(c) && c!=YardAccess && !Cottages.Any(s=>s.Entrance==c) &&
                !(Commons is {} commons && (commons.Center==c || commons.Places.Contains(c)))).ToArray();
    private Cell[] ClaimedHomeYardPlaces(Cottage home)=>(home.Improved || home.ImprovementRequested)?YardGround(home,home.YardSide):System.Array.Empty<Cell>();
    private string? YardClaimProblem(Cottage home,int side)=>Cottages.Where(c=>c.Id!=home.Id).SelectMany(ClaimedHomeYardPlaces).Intersect(YardGround(home,side)).Any()
        ?"Another home's furnished or ordered yard uses this ground.":null;
    public string? HomeYardProblem(int id,int side)
    {
        var home=Cottages.FirstOrDefault(c=>c.Id==id && IsHome(c));
        if(PublicPlace==null || home==null)return "Choose a finished home in this hamlet.";
        if(side is <0 or >3)return "Choose one of the four sides.";
        if(Food.Celebrating)return "Wait until supper finishes.";
        if(home.ImprovementRequested || home.ImprovementPlanks>0 && !home.Improved || People.Any(p=>p.ComfortHomeId==id))return "Finish or cancel furnishing and let supplies return first.";
        var places=YardPlaces(home,side);
        if(places.Length==0 || !places.Any(c=>FindPath(home.Entrance,c,Blocked)!=null))return "This side needs reachable open ground.";
        if(YardClaimProblem(home,side) is {} conflict)return conflict;
        return null;
    }
    public string? FurnishHomeYardProblem(int id,int side)=>HomeYardProblem(id,side)??ImprovementProblem(id,side);
    public bool FurnishHomeYard(int id,int side)
    {
        if(FurnishHomeYardProblem(id,side)!=null)return false;
        SetHomeYard(id,side);return RequestImprovement(id);
    }
    public bool SetHomeYard(int id,int side)
    {
        if(HomeYardProblem(id,side)!=null)return false;
        var home=Cottages.Single(c=>c.Id==id);if(home.YardSide==side)return true;
        var old=HomeYardPlaces(home);
        foreach(var person in People.Where(p=>p.HomeId==id && (IdleHomeJourney(p) || old.Contains(At(p)) || p.Meal is {} meal && old.Contains(meal.Seat))).ToArray())Interrupt(person);
        home.YardSide=side;_retry=0;
        History.Add($"Home {id}: outdoor life moved to {YardSideName(side).ToLowerInvariant()}.");return true;
    }
    public bool AtFurnishedHome(Villager person)=>person.HomeId is int id && Cottages.FirstOrDefault(c=>c.Id==id) is {} home && HomeYardPlaces(home).Contains(At(person));
    public bool AvailableAtHome(Villager person)=>person.SharedWorker && person.Task==Work.Waiting && person.Route.Count==0 &&
        person.HomeId is int id && Cottages.FirstOrDefault(c=>c.Id==id && c.Complete && IsHome(c)) is {} home &&
        ((At(person).Point-home.Entrance.Point).LengthSquared()<=4 || HomeYardPlaces(home).Contains(At(person))) && At(person)!=YardAccess && !Cottages.Any(c=>c.Entrance==At(person));
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

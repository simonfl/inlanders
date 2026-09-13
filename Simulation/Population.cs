using System.Linq;
using System.Collections.Generic;

namespace Inlanders.Simulation;

public sealed partial class World
{
    public int ArrivalFoodRequired => (Population + 2) * 2;

    public string? InvitationProblem()
    {
        if(Neighborhood!=null)return NeighborhoodInvitationProblem(NeighborhoodLanding());
        if (Food.Celebrating) return "Welcome newcomers after supper finishes.";
        if (SpareBeds < 2) return "Finish two spare beds to welcome newcomers.";
        if (!Creative && EdibleStored < ArrivalFoodRequired)
            return $"Store {ArrivalFoodRequired} edible portions (berries, vegetables, bread, fish or game): two meals for {Population + 2} people. Carried portions do not count.";
        if (ArrivalSpots().Length < 2) return "Leave two clear arrival spots near the timber yard.";
        return null;
    }

    private Cell[] ArrivalSpots()
    {
        // Stop as soon as two nearby cells are found; the UI checks this while playing.
        var result = new List<Cell>();
        var occupied = People.Select(At).ToHashSet();
        var visited = new HashSet<Cell> { YardAccess };
        var pending = new Queue<Cell>();
        if (!Blocked(YardAccess)) pending.Enqueue(YardAccess);
        while (pending.TryDequeue(out var cell))
        {
            if (!occupied.Contains(cell)) result.Add(cell);
            if (result.Count == 2) break;
            foreach (var next in new[] { new Cell(cell.X + 1, cell.Z), new(cell.X - 1, cell.Z),
                new(cell.X, cell.Z + 1), new(cell.X, cell.Z - 1) })
                if (visited.Add(next) && !Blocked(next)) pending.Enqueue(next);
        }
        return result.ToArray();
    }
    public bool InviteNewcomers()
    {
        if(Neighborhood!=null)return NeighborhoodLanding() is Cell landing && CommitNeighbors(landing);
        if (InvitationProblem() != null) return false;
        var spots = ArrivalSpots();
        var names = new[] { "Lina", "Ash", "Cora", "Remy", "Wren", "Hugo", "Fern", "Kit", "Alma", "Rowan", "June", "Pip" };
        foreach (var spot in spots)
        {
            int id = Population, index = id - InitialPopulation;
            People.Add(new Villager { Id = id, Name = index < names.Length ? names[index] : $"Neighbor {id + 1}",
                NextMealTime=Food.Time+15+index%2*7.5f, Position = spot.Point, Role = Role.Unassigned, Status = "New arrival · choose a job in People" });
        }
        ReconcileHomes(); History.Add($"{People[^2].Name} and {People[^1].Name} joined the village");
        if(Campaign?.River is {Phase:1 or 3} river)
        { river.AssessmentStarted=Food.Time; river.Meals=0; river.Required=0; river.LastResult="New residents arrived. Meal service will be assessed for everyone from now."; }
        if(Campaign?.Lake is {Phase:2} lake)
        { lake.AssessmentStarted=Food.Time; lake.Meals=0; lake.Required=0; lake.LastResult="New residents arrived. Meal service will be assessed for everyone from now."; }
        _retry = 0;
        return true;
    }
}

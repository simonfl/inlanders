using System.Linq;
using System.Collections.Generic;

namespace Inlanders.Simulation;

public sealed partial class World
{
    public int ArrivalFoodRequired => (Population + 2) * 2;

    public string? InvitationProblem()
    {
        if (Food.Celebrating) return "Welcome newcomers after supper finishes.";
        if (SpareBeds < 2) return "Finish two spare beds to welcome newcomers.";
        if (Food.Berries + Food.Bread < ArrivalFoodRequired)
            return $"Store {ArrivalFoodRequired} berries/bread: two meals for {Population + 2} people.";
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
        if (InvitationProblem() != null) return false;
        var spots = ArrivalSpots();
        var names = new[] { "Lina", "Ash", "Cora", "Remy", "Wren", "Hugo", "Fern", "Kit", "Alma", "Rowan", "June", "Pip" };
        foreach (var spot in spots)
        {
            int id = Population, index = id - InitialPopulation;
            People.Add(new Villager { Id = id, Name = index < names.Length ? names[index] : $"Neighbor {id + 1}",
                Position = spot.Point, Role = Role.Unassigned, Status = "New arrival · choose a job in People" });
        }
        History.Add($"{People[^2].Name} and {People[^1].Name} joined the village");
        _retry = 0;
        return true;
    }
}

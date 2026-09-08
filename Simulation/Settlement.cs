using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.Json.Serialization;

namespace Inlanders.Simulation;

public readonly record struct Cell(int X, int Z) { public Vector2 Point => new(X, Z); }
public enum Role { Unassigned, Logger, Builder, Forager, Farmer, Baker }
public enum Resource { Logs, Berries, Grain, Bread }
public enum BuildingKind { Cottage, ForagerHut, Farm, Bakery }
public enum Work { Waiting, ToTree, Chopping, ToStockpile, ToMaterials, ToCottage, ToBuild, Building,
    ToBush, Foraging, ToFarm, Planting, Harvesting, ToGrain, ToOven, Baking, ToBread, ToPantry, ToSupper, Supper }

public sealed class Villager
{
    public int Id { get; init; }
    public string Name { get; init; } = "";
    [JsonInclude]    public Role Role { get; internal set; }
    [JsonInclude]    public Vector2 Position { get; internal set; }
    [JsonInclude]    public Work Task { get; internal set; }
    [JsonInclude]    public string Status { get; internal set; } = "Looking for work";
    [JsonInclude]    public int Carried { get; internal set; }
    [JsonInclude]    public int Reserved { get; internal set; }
    [JsonInclude]    public int? SiteId { get; internal set; }
    [JsonInclude]    public int? TreeId { get; internal set; }
    [JsonInclude]    public Queue<Cell> Route { get; internal set; } = new();
    [JsonInclude]    public float Timer { get; internal set; }
    [JsonInclude]    public Cell Destination { get; internal set; }
    [JsonInclude]    public Resource Cargo { get; internal set; }
    [JsonInclude]    public int? WorkplaceId { get; internal set; }
    [JsonInclude]    public int? BushId { get; internal set; }
    [JsonInclude]    public int FoodReserved { get; internal set; }
}
public sealed class TimberTree
{
    public int Id { get; init; }
    public Cell Cell { get; init; }
    [JsonInclude]    public int Logs { get; internal set; }
    [JsonInclude]    public bool Felled { get; internal set; }
    public bool Salvage { get; init; }
    [JsonInclude]    public int? Owner { get; internal set; }
    public Cell Access => new(Cell.X + 1, Cell.Z);
}
public sealed class Cottage
{
    public int Id { get; init; }
    public Cell Cell { get; init; }
    public bool Rotated { get; init; }
    public BuildingKind Kind { get; init; }
    [JsonInclude]    public bool Planted { get; internal set; }
    [JsonInclude]    public float Growth { get; internal set; }
    [JsonInclude]    public int Harvest { get; internal set; }
    [JsonInclude]    public int InputGrain { get; internal set; }
    [JsonInclude]    public int OutputBread { get; internal set; }
    [JsonInclude]    public float BakeProgress { get; internal set; }
    [JsonInclude]    public int Priority { get; internal set; } = 1;
    [JsonInclude]    public int Delivered { get; internal set; }
    [JsonInclude]    public int Incoming { get; internal set; }
    [JsonInclude]    public int? Builder { get; internal set; }
    [JsonInclude]    public float Construction { get; internal set; }
    public bool Complete => Construction >= 1;
    public Cell Entrance => World.Door(Cell, Rotated);
}

// Commands and fixed-step Tick run on one thread. Claiming a job, storage units,
// and destination capacity is atomic. Rendering never modifies these records.
public sealed partial class World
{
    public const int Cost = 6;
    public const int Population = 8;
    public List<Villager> People { get; } = new();
    public List<TimberTree> Trees { get; } = new();
    public List<Cottage> Cottages { get; } = new();
    public Cell Stockpile { get; } = new(-3, 3);
    public Cell YardAccess => new(Stockpile.X + 1, Stockpile.Z);
    public int Stored { get; private set; }
    public int ReservedStorage => People.Where(v => v.Task == Work.ToMaterials).Sum(v => v.Reserved);
    public int Available => Stored - ReservedStorage;
    public int InitialLogs { get; private set; }
    public int Housed => Math.Min(Population, Cottages.Count(c => c.Complete && c.Kind == BuildingKind.Cottage) * 2);
    public List<string> History { get; } = new();
    private int _nextSite = 1;
    private int _nextTree;
    private float _retry;

    public World(int logsPerTree = 8)
    {
        if (logsPerTree < 0) throw new ArgumentOutOfRangeException(nameof(logsPerTree));
        var cells = new[] { new Cell(-6,-4), new(-3,-4), new(0,-5), new(-6,-1), new(-3,-1), new(3,-5) };
        foreach (var cell in cells) Trees.Add(new TimberTree { Id = Trees.Count, Cell = cell, Logs = logsPerTree });
        _nextTree = Trees.Count;
        InitialLogs = Trees.Sum(t => t.Logs);
        var names = new[] { "Mara", "Ivo", "Nell", "Otis", "Ada", "Finn", "Bea", "Sol" };
        for (int i = 0; i < Population; i++) People.Add(new Villager
        {
            Id = i, Name = names[i], Position = new Vector2(-1 + i % 4, 3 + i / 4),
            Role = i < 4 ? Role.Logger : Role.Builder
        });
        InitializeFood();
    }
    public static Cell Door(Cell c, bool rotated) => rotated ? new(c.X + 1, c.Z) : new(c.X, c.Z + 1);
    public static IEnumerable<Cell> Footprint(Cell c, bool rotated)
    {
        for (int x = -1; x <= (rotated ? 0 : 1); x++)
            for (int z = -1; z <= (rotated ? 1 : 0); z++) yield return new(c.X + x, c.Z + z);
    }
    public static Cell At(Villager v) => new((int)MathF.Round(v.Position.X), (int)MathF.Round(v.Position.Y));
    private static bool Inside(Cell c) => c.X >= -8 && c.X <= 8 && c.Z >= -7 && c.Z <= 7;
    private bool Blocked(Cell c) => !Inside(c) || c == Stockpile || Trees.Any(t => t.Cell == c) || Bushes.Any(b => b.Cell == c) ||
        Cottages.Any(h => Footprint(h.Cell, h.Rotated).Contains(c));

    public bool CanPlace(Cell cell, bool rotated)
    {
        if (Food.Celebrating) return false;
        var footprint = Footprint(cell, rotated).ToHashSet();
        if (footprint.Any(Blocked) || !Inside(Door(cell, rotated))) return false;
        var access = Trees.Select(t => t.Access).Concat(Bushes.Select(b => b.Access)).Concat(Cottages.Select(h => h.Entrance))
            .Append(YardAccess).Append(Door(cell, rotated)).ToArray();
        if (access.Any(footprint.Contains) || People.Any(v => footprint.Contains(At(v)) ||
            (v.Route.TryPeek(out var next) && footprint.Contains(next)))) return false;
        bool Obstacle(Cell c) => Blocked(c) || footprint.Contains(c);
        return access.Concat(People.Select(At)).All(c => FindPath(YardAccess, c, Obstacle) != null);
    }
    public Cottage? Place(Cell cell, bool rotated = false, BuildingKind kind = BuildingKind.Cottage)
    {
        if (!Enum.IsDefined(kind) || !CanPlace(cell, rotated)) return null;
        var site = new Cottage { Id = _nextSite++, Cell = cell, Rotated = rotated, Kind = kind }; Cottages.Add(site);
        foreach (var v in People.Where(v => v.Route.Count > 0)) SetRoute(v, v.Destination);
        History.Add($"{kind} {site.Id} planned"); _retry = 0; return site;
    }
    public bool SetPriority(int id, int priority)
    {
        var site = Cottages.FirstOrDefault(c => c.Id == id);
        if (site == null || site.Complete || priority < 0 || priority > 2) return false;
        site.Priority = priority; _retry = 0; return true;
    }
    public bool Cancel(int id)
    {
        var site = Cottages.FirstOrDefault(c => c.Id == id);
        if (site == null || site.Complete) return false;
        foreach (var person in People.Where(v => v.SiteId == id)) Interrupt(person);
        Cottages.Remove(site);
        // Delivered timber becomes a recoverable pile rather than teleporting to storage.
        if (site.Delivered > 0) Trees.Add(new TimberTree { Id = _nextTree++, Cell = site.Cell, Logs = site.Delivered, Felled = true, Salvage = true });
        foreach (var v in People.Where(v => v.Route.Count > 0)) SetRoute(v, v.Destination);
        History.Add($"Cottage {id} cancelled; {site.Delivered} logs salvaged"); _retry = 0; return true;
    }
    public void Assign(int id, Role role)
    {
        if (Food.Celebrating) return;
        if (!Enum.IsDefined(role)) throw new ArgumentOutOfRangeException(nameof(role));
        var v = People.Single(v => v.Id == id);
        if (v.Role == role) return;
        Interrupt(v); v.Role = role; _retry = 0;
    }
    public bool AdjustWorkers(Role role, int delta)
    {
        if (role == Role.Unassigned || delta == 0) return false;
        var v = delta < 0 ? People.LastOrDefault(v => v.Role == role) :
            People.FirstOrDefault(v => v.Role == Role.Unassigned) ?? People.LastOrDefault(v => v.Role != role);
        if (v == null) return false;
        Assign(v.Id, delta < 0 ? Role.Unassigned : role); return true;
    }
    private void Interrupt(Villager v)
    {
        ReleaseFoodClaims(v);
        if (v.TreeId is int tree) Trees.Single(t => t.Id == tree).Owner = null;
        if (v.SiteId is int id && Cottages.FirstOrDefault(c => c.Id == id) is Cottage site)
        {
            site.Incoming -= v.Reserved;
            if (site.Builder == v.Id) site.Builder = null;
        }
        v.Reserved = 0; v.SiteId = null; v.TreeId = null; v.Route.Clear(); v.Timer = 0;
        if (v.Carried > 0) Go(v, YardAccess, v.Cargo == Resource.Logs ? Work.ToStockpile : Work.ToPantry, $"Returning carried {v.Cargo.ToString().ToLowerInvariant()}");
        else { v.Task = Work.Waiting; v.Status = "Looking for work"; }
    }
    private void SetRoute(Villager v, Cell destination)
    {
        var route = FindPath(At(v), destination, Blocked) ?? throw new InvalidOperationException("Work destination became unreachable");
        v.Route.Clear(); foreach (var c in route) v.Route.Enqueue(c); v.Destination = destination;
    }
    private void Go(Villager v, Cell target, Work task, string status)
    {
        SetRoute(v, target); v.Task = task; v.Timer = 0; v.Status = status;
    }
    private void Finish(Villager v)
    {
        ReleaseFoodClaims(v);
        v.TreeId = null; v.SiteId = null; v.Reserved = 0; v.Task = Work.Waiting;
        v.Timer = 0; v.Status = "Looking for work"; _retry = 0;
    }
    private void ClaimWork(Villager v)
    {
        if (Food.Celebrating) { Go(v, MeetingSpots[v.Id], Work.ToSupper, "Joining the village supper"); return; }
        if (v.Role is Role.Forager or Role.Farmer or Role.Baker) { ClaimFoodWork(v); return; }
        if (v.Role == Role.Unassigned) { v.Status = "Unassigned — choose a job"; return; }
        if (v.Role == Role.Logger)
        {
            var tree = Trees.Where(t => t.Logs > 0 && t.Owner == null)
                .OrderBy(t => Vector2.DistanceSquared(v.Position, t.Access.Point)).ThenBy(t => t.Id).FirstOrDefault();
            if (tree == null) { v.Status = Trees.Any(t => t.Logs > 0) ? "Waiting — remaining trees claimed by other loggers" : "No timber left to harvest"; return; }
            tree.Owner = v.Id; v.TreeId = tree.Id;
            Go(v, tree.Access, Work.ToTree, tree.Felled ? "Walking to felled timber" : "Walking to an alder"); return;
        }
        var sites = Cottages.Where(c => !c.Complete).OrderByDescending(c => c.Priority).ThenBy(c => c.Id).ToArray();
        foreach (var site in sites)
        {
            if (site.Delivered == Cost && site.Builder == null)
            {
                site.Builder = v.Id; v.SiteId = site.Id;
                Go(v, site.Entrance, Work.ToBuild, $"Walking to build cottage {site.Id}"); return;
            }
            int amount = Math.Min(2, Math.Min(Cost - site.Delivered - site.Incoming, Available));
            if (amount <= 0) continue;
            v.SiteId = site.Id; v.Reserved = amount; site.Incoming += amount;
            v.Cargo = Resource.Logs;
            Go(v, YardAccess, Work.ToMaterials, $"Collecting {amount} reserved logs for cottage {site.Id}"); return;
        }
        v.Status = sites.Length == 0 ? "No construction plans — place a cottage" :
            sites.All(c => c.Delivered + c.Incoming == Cost) ? "Waiting — deliveries or another builder already cover each site" :
            Stored == 0 ? "Waiting for timber — assign loggers" : "Waiting — stored timber is reserved by other builders";
    }
    public void Tick(float dt)
    {
        if (dt <= 0 || !float.IsFinite(dt)) return;
        AdvanceFoodTime(dt);
        dt *= Food.WorkEfficiency;
        _retry -= dt; bool retry = _retry <= 0; if (retry) _retry = 0.5f;
        foreach (var v in People)
        {
            if (v.Route.TryPeek(out var waypoint))
            {
                var offset = waypoint.Point - v.Position; float distance = offset.Length();
                if (distance <= 1.8f * dt) { v.Position = waypoint.Point; v.Route.Dequeue(); }
                else v.Position += offset / distance * 1.8f * dt;
                continue;
            }
            v.Timer += dt;
            switch (v.Task)
            {
                case Work.Waiting: if (retry) ClaimWork(v); break;
                case Work.ToTree: v.Task = Work.Chopping; v.Timer = 0; v.Status = "Cutting and collecting timber"; break;
                case Work.Chopping:
                    var tree = Trees.Single(t => t.Id == v.TreeId);
                    if (v.Timer < (tree.Felled ? 1.2f : 4f)) break;
                    tree.Felled = true; v.Cargo = Resource.Logs; v.Carried = Math.Min(2, tree.Logs); tree.Logs -= v.Carried; tree.Owner = null; v.TreeId = null;
                    if (tree.Salvage && tree.Logs == 0) Trees.Remove(tree);
                    Go(v, YardAccess, Work.ToStockpile, $"Carrying {v.Carried} logs to the timber yard"); break;
                case Work.ToStockpile: Stored += v.Carried; v.Carried = 0; Finish(v); break;
                case Work.ToMaterials:
                    if (v.Timer < 0.7f) break;
                    Stored -= v.Reserved; v.Carried = v.Reserved;
                    Go(v, Cottages.Single(c => c.Id == v.SiteId).Entrance, Work.ToCottage, $"Delivering {v.Carried} logs to cottage {v.SiteId}"); break;
                case Work.ToCottage:
                    var delivery = Cottages.Single(c => c.Id == v.SiteId);
                    delivery.Delivered += v.Carried; delivery.Incoming -= v.Reserved; v.Carried = 0; Finish(v); break;
                case Work.ToBuild: v.Task = Work.Building; v.Status = $"Building cottage {v.SiteId}"; break;
                case Work.Building:
                    var build = Cottages.Single(c => c.Id == v.SiteId); build.Construction = Math.Min(1, build.Construction + dt / 12);
                    if (build.Complete) { build.Builder = null; History.Add($"{build.Kind} {build.Id} completed"); Finish(v); } break;
                default: TickFoodWork(v, dt); break;
            }
        }
    }
    public void Validate()
    {
        void Check(bool condition, string error) { if (!condition) throw new InvalidOperationException(error); }
        Check(Stored >= 0 && Available >= 0, "Negative or over-reserved storage");
        Check(Trees.Sum(t => t.Logs) + Stored + People.Where(v => v.Cargo == Resource.Logs).Sum(v => v.Carried) + Cottages.Sum(c => c.Delivered) == InitialLogs, "Timber conservation failed");
        ValidateFood();
        foreach (var site in Cottages)
        {
            Check(site.Incoming == People.Where(v => v.SiteId == site.Id).Sum(v => v.Reserved), "Orphaned site reservation");
            Check(site.Delivered >= 0 && site.Delivered + site.Incoming <= Cost, "Over-delivery");
            var builders = People.Where(v => v.SiteId == site.Id && v.Task is Work.ToBuild or Work.Building).ToArray();
            Check(builders.Length == (site.Builder.HasValue ? 1 : 0) && (builders.Length == 0 || builders[0].Id == site.Builder), "Builder ownership mismatch");
        }
        foreach (var t in Trees)
        {
            var owners = People.Where(v => v.TreeId == t.Id).ToArray();
            Check(t.Logs >= 0 && owners.Length == (t.Owner.HasValue ? 1 : 0) && (owners.Length == 0 || owners[0].Id == t.Owner), "Tree ownership mismatch");
        }
        foreach (var v in People)
        {
            Check(v.Carried is >= 0 and <= 2, "Carry capacity exceeded");
            Check(!Blocked(At(v)) && v.Route.All(c => !Blocked(c)), "Worker route intersects obstacle");
            Check(v.SiteId == null || Cottages.Any(c => c.Id == v.SiteId), "Job targets cancelled site");
        }
    }
    private static List<Cell>? FindPath(Cell start, Cell goal, Func<Cell, bool> blocked)
    {
        if (blocked(goal)) return null;
        var frontier = new PriorityQueue<Cell, int>(); frontier.Enqueue(start, 0);
        var previous = new Dictionary<Cell, Cell>(); var costs = new Dictionary<Cell, int> { [start] = 0 };
        while (frontier.TryDequeue(out var current, out _))
        {
            if (current == goal)
            {
                var result = new List<Cell>();
                while (current != start) { result.Add(current); current = previous[current]; }
                result.Reverse(); return result;
            }
            foreach (var next in new[] { new Cell(current.X + 1, current.Z), new Cell(current.X - 1, current.Z), new Cell(current.X, current.Z + 1), new Cell(current.X, current.Z - 1) })
            {
                if (blocked(next)) continue;
                int cost = costs[current] + 1;
                if (costs.TryGetValue(next, out int old) && old <= cost) continue;
                costs[next] = cost; previous[next] = current;
                frontier.Enqueue(next, cost + Math.Abs(next.X - goal.X) + Math.Abs(next.Z - goal.Z));
            }
        }
        return null;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.Json.Serialization;

namespace Inlanders.Simulation;

public readonly record struct Cell(int X, int Z) { public Vector2 Point => new(X, Z); }
public enum Role { Unassigned, Logger, Builder, Forager, Farmer, Baker, Sawyer, Hauler }
public enum Resource { Logs, Berries, Grain, Bread, Planks, Vegetables }
public enum BuildingKind { Cottage, ForagerHut, Farm, Bakery, Sawmill, Lodge, Square, Bridge, Stockpile, VegetableGarden }
public enum Work { Waiting, ToTree, Chopping, ToStockpile, ToMaterials, ToCottage, ToBuild, Building,
    ToBush, Foraging, ToFarm, Planting, Harvesting, ToGrain, ToOven, Baking, ToBread, ToPantry, ToSupper, Supper,
    ToSapling, PlantingTree, ToSawLogs, ToSawmill, Sawing, ToPlanks, ToClearStump, ClearingStump, ToHaulPickup, ToHaulDrop, ToLeisure, Leisure }

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
    [JsonInclude]    public int? StorageId { get; internal set; }
    [JsonInclude]    public int? HaulTargetId { get; internal set; }
    [JsonInclude] public int? LeisureSiteId { get; internal set; }
    [JsonInclude] public float NextLeisureTime { get; internal set; }
    [JsonInclude] public int LeisureVisits { get; internal set; }
    [JsonInclude] public float? LastLeisureTime { get; internal set; }
}
public sealed class TimberTree
{
    public int Id { get; init; }
    public Cell Cell { get; init; }
    [JsonInclude]    public int Logs { get; internal set; }
    [JsonInclude]    public bool Felled { get; internal set; }
    public bool Salvage { get; init; }
    public Resource Material { get; init; } = Resource.Logs;
    [JsonInclude]    public int? Owner { get; internal set; }
    [JsonInclude]    public float Growth { get; internal set; } = 1;
    [JsonInclude]    public bool NeedsPlanting { get; internal set; }
    [JsonInclude]    public bool ClearRequested { get; internal set; }
    public Cell Access => new(Cell.X + 1, Cell.Z);
}
public sealed class Cottage
{
    [JsonInclude] public bool WorkPaused { get; internal set; }
    [JsonInclude] public int OutputTarget { get; internal set; } = -1;
    public int Id { get; init; }
    public Cell Cell { get; init; }
    public bool Rotated { get; init; }
    public bool BridgeFromFar { get; init; }
    public BuildingKind Kind { get; init; }
    [JsonInclude]    public bool Planted { get; internal set; }
    [JsonInclude]    public float Growth { get; internal set; }
    [JsonInclude]    public int Harvest { get; internal set; }
    [JsonInclude]    public int InputGrain { get; internal set; }
    [JsonInclude]    public int OutputBread { get; internal set; }
    [JsonInclude]    public float BakeProgress { get; internal set; }
    [JsonInclude]    public int InputLogs { get; internal set; }
    [JsonInclude]    public int OutputPlanks { get; internal set; }
    [JsonInclude]    public float SawProgress { get; internal set; }
    [JsonInclude]    public int Priority { get; internal set; } = 1;
    [JsonInclude]    public int Delivered { get; internal set; }
    [JsonInclude]    public int Incoming { get; internal set; }
    [JsonInclude]    public int? Builder { get; internal set; }
    [JsonInclude]    public float Construction { get; internal set; }
    [JsonInclude]    public int StoredLogs { get; internal set; }
    [JsonInclude]    public int LogTarget { get; internal set; } = 6;
    public bool Complete => Construction >= 1;
    public Cell Entrance => Kind == BuildingKind.Bridge && BridgeFromFar ? World.FarBank(Cell, Rotated) : World.Door(Cell, Rotated);
    public Resource Material => Buildings.Get(Kind).Material;
    public int Required => Buildings.Get(Kind).Cost;
}

// Commands and fixed-step Tick run on one thread. Claiming a job, storage units,
// and destination capacity is atomic. Rendering never modifies these records.
public sealed partial class World
{
    public const int InitialPopulation = 8;
    public int Population => People.Count;
    public List<Villager> People { get; } = new();
    public List<TimberTree> Trees { get; } = new();
    public List<Cottage> Cottages { get; } = new();
    public Cell Stockpile { get; } = new(-3, 3);
    public Cell YardAccess => new(Stockpile.X + 1, Stockpile.Z);
    private int _yardLogs;
    public int YardLogs => _yardLogs;
    public int Stored => _yardLogs + Cottages.Sum(c => c.StoredLogs);
    public int ReservedStorage => People.Where(v => (v.Task == Work.ToMaterials && v.Cargo == Resource.Logs) || v.Task is Work.ToSawLogs or Work.ToHaulPickup).Sum(v => v.Reserved);
    public int Available => Stored - ReservedStorage;
    public int InitialLogs { get; private set; }
    public int Beds => Cottages.Where(c => c.Complete).Sum(c => Buildings.Get(c.Kind).Beds);
    public int Housed => Math.Min(Population, Beds);
    public int SpareBeds => Math.Max(0, Beds - Population);
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
        for (int i = 0; i < InitialPopulation; i++) People.Add(new Villager
        {
            Id = i, Name = names[i], Position = new Vector2(-1 + i % 4, 3 + i / 4),
            Role = i < 4 ? Role.Logger : Role.Builder
        });
        InitializeFood();
    }
    public static Cell Door(Cell c, bool rotated) => rotated ? new(c.X + 1, c.Z) : new(c.X, c.Z + 1);
    public static IEnumerable<Cell> Footprint(Cell c, bool rotated, BuildingKind kind = BuildingKind.Cottage)
    {
        if (kind == BuildingKind.Bridge) { yield return c; yield break; }
        for (int x = -1; x <= (rotated ? 0 : 1); x++)
            for (int z = -1; z <= (rotated ? 1 : 0); z++) yield return new(c.X + x, c.Z + z);
    }
    public static Cell At(Villager v) => new((int)MathF.Round(v.Position.X), (int)MathF.Round(v.Position.Y));
    private bool Inside(Cell c) => Map.Contains(c);
    private bool Blocked(Cell c) => Decorations.Any(d => d.Cell == c && d.Solid) || !Inside(c) || (Map.Water.Contains(c) && !Cottages.Any(b => b.Kind == BuildingKind.Bridge && b.Cell == c && b.Complete)) || c == Stockpile || Trees.Any(t => t.Cell == c) || Bushes.Any(b => b.Cell == c) ||
        Cottages.Any(h => h.Kind != BuildingKind.Bridge && Footprint(h.Cell, h.Rotated, h.Kind).Contains(c));

    public bool CanPlace(Cell cell, bool rotated) => PlacementProblem(cell, rotated) == null;
    public Cottage? Place(Cell cell, bool rotated = false, BuildingKind kind = BuildingKind.Cottage)
    {
        if (!Enum.IsDefined(kind) || PlacementProblem(cell, rotated, kind) != null) return null;
        var site = new Cottage { Id = _nextSite++, Cell = cell, Rotated = rotated, Kind = kind, Construction = Creative ? 1 : 0, BridgeFromFar = kind == BuildingKind.Bridge && !Accessible(Door(cell, rotated)) }; Cottages.Add(site);
        if (kind == BuildingKind.Sawmill) site.OutputTarget = PlankStockTarget;
        RemovePaths(Footprint(cell, rotated, kind));
        foreach (var v in People.Where(v => v.Route.Count > 0)) SetRoute(v, v.Destination);
        History.Add($"{kind} {site.Id} {(Creative ? "placed" : "planned")}"); _retry = 0; return site;
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
        Cell salvageCell = site.Cell;
        if (site.Kind == BuildingKind.Bridge && site.Delivered > 0)
        {
            var spot = Map.Land.OrderBy(c => (c.Point - site.Entrance.Point).LengthSquared()).Cast<Cell?>().FirstOrDefault(c => !Trees.Any(t => t.Cell == c!.Value) && PlantingProblem(c!.Value) == null);
            if (spot == null) return false;
            salvageCell = spot.Value;
        }
        foreach (var person in People.Where(v => v.SiteId == id)) Interrupt(person);
        Cottages.Remove(site);
        if (site.Delivered > 0) RemovePaths(new[] { salvageCell });
        // Delivered timber becomes a recoverable pile rather than teleporting to storage.
        if (site.Delivered > 0) Trees.Add(new TimberTree { Id = _nextTree++, Cell = salvageCell, Logs = site.Delivered, Material = site.Material, Felled = true, Salvage = true });
        foreach (var v in People.Where(v => v.Route.Count > 0)) SetRoute(v, v.Destination);
        History.Add($"{site.Kind} {id} cancelled; {site.Delivered} {site.Material} salvaged"); _retry = 0; return true;
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
        var v = WorkerAdjustmentCandidate(role, delta);
        if (v == null) return false;
        Assign(v.Id, delta < 0 ? Role.Unassigned : role); return true;
    }
    public Villager? WorkerAdjustmentCandidate(Role role, int delta) => delta < 0 ? People.LastOrDefault(v => v.Role == role) :
        People.FirstOrDefault(v => v.Role == Role.Unassigned) ?? People.LastOrDefault(v => v.Role != role);
    private void Interrupt(Villager v)
    {
        ReleaseFoodClaims(v);
        if (v.LeisureSiteId != null) v.NextLeisureTime = Food.Time + 60;
        v.LeisureSiteId = null;
        if (v.TreeId is int tree) Trees.Single(t => t.Id == tree).Owner = null;
        if (v.SiteId is int id && Cottages.FirstOrDefault(c => c.Id == id) is Cottage site)
        {
            site.Incoming -= v.Reserved;
            if (site.Builder == v.Id) site.Builder = null;
        }
        v.Reserved = 0; v.SiteId = null; v.TreeId = null; v.StorageId = null; v.HaulTargetId = null; v.Route.Clear(); v.Timer = 0;
        if (v.Carried > 0)
        {
            if (v.Cargo is Resource.Logs or Resource.Planks) ReturnTimber(v);
            else Go(v, YardAccess, Work.ToPantry, $"Returning carried {v.Cargo.ToString().ToLowerInvariant()}");
        }
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
        if (v.LeisureSiteId != null) v.NextLeisureTime = Food.Time + 60;
        v.LeisureSiteId = null;
        v.TreeId = null; v.SiteId = null; v.StorageId = null; v.HaulTargetId = null; v.Reserved = 0; v.Task = Work.Waiting;
        v.Timer = 0; v.Status = "Looking for work"; _retry = 0;
    }
    private void ClaimWork(Villager v)
    {
        if (Food.Celebrating) { Go(v, MeetingSpots[v.Id], Work.ToSupper, "Joining the village supper"); return; }
        if (ClaimLeisure(v)) return;
        if (v.Role == Role.Hauler) { ClaimHauling(v); return; }
        if (v.Role == Role.Sawyer) { ClaimSawWork(v); return; }
        if (v.Role is Role.Forager or Role.Farmer or Role.Baker) { ClaimFoodWork(v); return; }
        if (v.Role == Role.Unassigned) { v.Status = "Unassigned — choose a job"; return; }
        if (v.Role == Role.Logger)
        {
            if (ClaimClearing(v)) return;
            var planting = Trees.Where(t => t.NeedsPlanting && t.Owner == null && Accessible(t.Access))
                .OrderBy(t => Vector2.DistanceSquared(v.Position, t.Access.Point)).ThenBy(t => t.Id).FirstOrDefault();
            if (planting != null)
            {
                planting.Owner = v.Id; v.TreeId = planting.Id;
                Go(v, planting.Access, Work.ToSapling, "Walking to plant an alder"); return;
            }
            var tree = Trees.Where(t => t.Logs > 0 && t.Owner == null && Accessible(t.Access))
                .OrderBy(t => Vector2.DistanceSquared(v.Position, t.Access.Point)).ThenBy(t => t.Id).FirstOrDefault();
            if (tree == null) { v.Status = Trees.Any(t => t.Logs > 0 || t.NeedsPlanting || t.ClearRequested) ? "Waiting — timber work claimed or across water; build a bridge" : Trees.Any(t => t.Growth < 1) ? "Waiting for saplings to grow" : "No timber — mark planting spots with T"; return; }
            tree.Owner = v.Id; v.TreeId = tree.Id;
            Go(v, tree.Access, Work.ToTree, tree.Felled ? "Walking to felled timber" : "Walking to an alder"); return;
        }
        var sites = Cottages.Where(c => !c.Complete).OrderByDescending(c => c.Priority).ThenBy(c => c.Id).ToArray();
        foreach (var site in sites)
        {
            if (site.Delivered == site.Required && site.Builder == null)
            {
                site.Builder = v.Id; v.SiteId = site.Id;
                Go(v, site.Entrance, Work.ToBuild, $"Walking to build {site.Kind} {site.Id}"); return;
            }
            int? source = null;
            if (site.Material == Resource.Logs && !TryLogSource(site.Entrance, 1, out source)) continue;
            int amount = Math.Min(2, Math.Min(site.Required - site.Delivered - site.Incoming, site.Material == Resource.Logs ? AvailableLogsAt(source) : AvailablePlanks));
            if (amount <= 0) continue;
            v.SiteId = site.Id; v.Reserved = amount; site.Incoming += amount;
            v.Cargo = site.Material; v.StorageId = source;
            Go(v, StorageAccess(source), Work.ToMaterials, $"Collecting {amount} reserved {site.Material} for {site.Kind} {site.Id}"); return;
        }
        v.Status = sites.Length == 0 ? "No construction plans — choose a building in Build" :
            sites.All(c => c.Delivered + c.Incoming == c.Required) ? "Waiting — deliveries or another builder already cover each site" :
            sites.Any(c => c.Material == Resource.Planks && c.Delivered + c.Incoming < c.Required) && AvailablePlanks == 0 ? "Waiting for planks — build a sawmill and assign a sawyer" :
            Stored == 0 ? "Waiting for timber — assign loggers" : "Waiting — stored timber is reserved by other builders";
    }
    public void Tick(float dt)
    {
        if (dt <= 0 || !float.IsFinite(dt)) return;
        AdvanceFoodTime(dt);
        AdvanceVisitor();
        AdvanceWoodland(dt);
        dt *= Food.WorkEfficiency;
        _retry -= dt; bool retry = _retry <= 0; if (retry) _retry = 0.5f;
        foreach (var v in People)
        {
            if (v.Route.TryPeek(out var waypoint))
            {
                var offset = waypoint.Point - v.Position; float distance = offset.Length();
                float travel = 1.8f * dt * (Paths.Contains(waypoint) ? 1.25f : 1);
                if (distance <= travel) { v.Position = waypoint.Point; v.Route.Dequeue(); }
                else v.Position += offset / distance * travel;
                continue;
            }
            v.Timer += dt;
            switch (v.Task)
            {
                case Work.Waiting: if (retry) ClaimWork(v); break;
                case Work.ToLeisure: v.Task = Work.Leisure; v.Timer = 0; v.Status = "Taking a break at the square"; break;
                case Work.Leisure: if (v.Timer >= 6) { v.LeisureVisits++; v.LastLeisureTime = Food.Time; Finish(v); } break;
                case Work.ToClearStump: v.Task = Work.ClearingStump; v.Timer = 0; v.Status = "Clearing roots and making ground usable"; break;
                case Work.ClearingStump:
                    if (v.Timer < 4) break;
                    var cleared = Trees.Single(t => t.Id == v.TreeId);
                    if (!cleared.ClearRequested || cleared.Logs != 0) throw new InvalidOperationException("Invalid clearing claim");
                    Trees.Remove(cleared); Finish(v); History.Add("Ground cleared for building or planting"); break;
                case Work.ToSapling: v.Task = Work.PlantingTree; v.Timer = 0; v.Status = "Planting an alder"; break;
                case Work.PlantingTree:
                    if (v.Timer < 4) break;
                    var sapling = Trees.Single(t => t.Id == v.TreeId);
                    sapling.NeedsPlanting = false; TreesPlanted++; sapling.Owner = null; Finish(v); break;
                case Work.ToTree: v.Task = Work.Chopping; v.Timer = 0; v.Status = "Cutting and collecting timber"; break;
                case Work.Chopping:
                    var tree = Trees.Single(t => t.Id == v.TreeId);
                    if (v.Timer < (tree.Felled ? 1.2f : 4f)) break;
                    tree.Felled = true; v.Cargo = tree.Material; v.Carried = Math.Min(2, tree.Logs); tree.Logs -= v.Carried; tree.Owner = null; v.TreeId = null;
                    if (tree.Salvage && tree.Logs == 0) Trees.Remove(tree);
                    ReturnTimber(v); break;
                case Work.ToStockpile:
                    if (v.Cargo == Resource.Planks) Planks += v.Carried; else ChangeLogs(v.StorageId, v.Carried);
                    v.Carried = 0; Finish(v); break;
                case Work.ToMaterials:
                    if (v.Timer < 0.7f) break;
                    if (v.Cargo == Resource.Planks) Planks -= v.Reserved; else ChangeLogs(v.StorageId, -v.Reserved);
                    v.StorageId = null;
                    v.Carried = v.Reserved;
                    Go(v, Cottages.Single(c => c.Id == v.SiteId).Entrance, Work.ToCottage, $"Delivering {v.Carried} {v.Cargo} to site {v.SiteId}"); break;
                case Work.ToCottage:
                    var delivery = Cottages.Single(c => c.Id == v.SiteId);
                    delivery.Delivered += v.Carried; delivery.Incoming -= v.Reserved; v.Carried = 0; Finish(v); break;
                case Work.ToBuild: v.Task = Work.Building; v.Status = $"Building {Cottages.Single(c => c.Id == v.SiteId).Kind} {v.SiteId}"; break;
                case Work.Building:
                    var build = Cottages.Single(c => c.Id == v.SiteId); build.Construction = Math.Min(1, build.Construction + dt / Buildings.Get(build.Kind).ConstructionSeconds);
                    if (build.Complete) { if (build.Kind == BuildingKind.Bridge) { foreach (var walker in People.Where(p => p.Route.Count > 0)) SetRoute(walker, walker.Destination); } build.Builder = null; History.Add($"{build.Kind} {build.Id} completed"); Finish(v); } break;
                default: if (!TickHauling(v) && !TickSawWork(v, dt)) TickFoodWork(v, dt); break;
            }
        }
        UpdateCampaign();
    }
    public void Validate()
    {
        void Check(bool condition, string error) { if (!condition) throw new InvalidOperationException(error); }
        Check(Population >= InitialPopulation && People.Select(v => v.Id).SequenceEqual(Enumerable.Range(0, Population)), "Invalid population identifiers");
        Check(Stored >= 0 && Available >= 0, "Negative or over-reserved storage");
        Check(Trees.Where(t => t.Material == Resource.Logs).Sum(t => t.Logs) + Stored + People.Where(v => v.Cargo == Resource.Logs).Sum(v => v.Carried) + Cottages.Where(c => c.Material == Resource.Logs).Sum(c => c.Delivered) + Cottages.Sum(c => c.InputLogs) + SawnLogs == InitialLogs + GrownLogs, "Timber conservation failed");
        Check(GrownLogs >= 0, "Invalid grown timber total");
        ValidateVisitor();
        ValidateCameraViews();
        ValidateHappiness();
        ValidateDecorations();
        ValidateLeisure();
        ValidateStorage();
        ValidateFood();
        ValidateProduction();
        ValidateFoodFlow();
        ValidateSawmills();
        foreach (var site in Cottages)
        {
            Check(site.Incoming == People.Where(v => v.SiteId == site.Id).Sum(v => v.Reserved), "Orphaned site reservation");
            Check(site.Delivered >= 0 && site.Incoming >= 0 && site.Delivered + site.Incoming <= site.Required, "Over-delivery");
            var builders = People.Where(v => v.SiteId == site.Id && v.Task is Work.ToBuild or Work.Building).ToArray();
            Check(builders.Length == (site.Builder.HasValue ? 1 : 0) && (builders.Length == 0 || builders[0].Id == site.Builder), "Builder ownership mismatch");
        }
        foreach (var t in Trees)
        {
            Check(!t.ClearRequested || !t.Salvage, "Salvage cannot have a root-clearing order");
            Check(float.IsFinite(t.Growth) && t.Growth >= 0 && t.Growth <= 1 &&
                (!t.NeedsPlanting || t.Growth == 0) &&
                (t.Growth == 1 || (t.Logs == 0 && !t.Felled && !t.Salvage)), "Invalid tree growth state");
            var owners = People.Where(v => v.TreeId == t.Id).ToArray();
            Check(t.Logs >= 0 && owners.Length == (t.Owner.HasValue ? 1 : 0) && (owners.Length == 0 || owners[0].Id == t.Owner), "Tree ownership mismatch");
        }
        foreach (var v in People)
        {
            if (v.Task is Work.ToClearStump or Work.ClearingStump)
                Check(Trees.Any(t => t.Id == v.TreeId && t.Owner == v.Id && t.ClearRequested && t.Logs == 0), "Invalid root-clearing worker");
            Check(v.Carried >= 0 && v.Carried <= (v.Cargo is Resource.Bread or Resource.Grain ? 4 : 2), "Carry capacity exceeded");
            Check(!Blocked(At(v)) && v.Route.All(c => !Blocked(c)), "Worker route intersects obstacle");
            Check(v.SiteId == null || Cottages.Any(c => c.Id == v.SiteId), "Job targets cancelled site");
        }
    }
    private List<Cell>? FindPath(Cell start, Cell goal, Func<Cell, bool> blocked)
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
                int cost = costs[current] + (Paths.Contains(next) ? 4 : 5);
                if (costs.TryGetValue(next, out int old) && old <= cost) continue;
                costs[next] = cost; previous[next] = current;
                frontier.Enqueue(next, cost + 4 * (Math.Abs(next.X - goal.X) + Math.Abs(next.Z - goal.Z)));
            }
        }
        return null;
    }
}

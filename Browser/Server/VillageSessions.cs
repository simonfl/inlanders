using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Inlanders.Simulation;

namespace Inlanders.Browser;

public sealed record Order(string Type, int X = 0, int Z = 0, int X2 = 0, int Z2 = 0, int Rotation = 0,
    int SiteId = -1, int Speed = 1, int Seconds = 15, bool UntilEvent = true, string? Note = null, ClientView? View = null);
public sealed record ClientView(bool Active = true, string Panel = "village", string? Selection = null,
    float CameraX = 0, float CameraZ = 0, float Zoom = 19, int Width = 0, int Height = 0,
    string Message = "", Cell[]? PreviewCells = null, bool PreviewValid = false);
sealed class Session(string id, World world)
{
    public string Id = id;
    public World World = world;
    public bool Paused = true;
    public int Speed = 1;
    public long Step;
    public long Sequence;
    public DateTime LastSeen = DateTime.UtcNow;
    public DateTime LastFrame = DateTime.MinValue;
    public DateTime LastSave = DateTime.MinValue;
    public ClientView View = new();
}
public sealed class VillageSessions
{
    public static readonly string Build = BuildHash();
    static string BuildHash()
    {
        using var hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
        hash.AppendData(File.ReadAllBytes(typeof(VillageSessions).Assembly.Location));
        var client = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../client/dist"));
        if (Directory.Exists(client)) foreach (var file in Directory.GetFiles(client, "*", SearchOption.AllDirectories).Order()) hash.AppendData(File.ReadAllBytes(file));
        return Convert.ToHexString(hash.GetHashAndReset())[..12];
    }
    static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);
    readonly object gate = new();
    readonly Dictionary<string, Session> sessions = new();
    readonly string root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../../artifacts/browser/sessions"));
    public VillageSessions() { Directory.CreateDirectory(root); }
    string Folder(string id)
    {
        if (!Guid.TryParseExact(id, "N", out _)) throw new KeyNotFoundException();
        return Path.Combine(root, id);
    }
    Session Get(string id)
    {
        if (sessions.TryGetValue(id, out var s)) return s;
        var directory = Folder(id);
        if (!Directory.Exists(directory)) throw new KeyNotFoundException();
        var path = Directory.GetFiles(directory, "snapshot-*.json").OrderDescending().FirstOrDefault();
        if (path == null) throw new KeyNotFoundException();
        var saved = JsonSerializer.Deserialize<Saved>(File.ReadAllText(path), Json)!;
        if (saved.Build != Build) throw new ArgumentException("This village belongs to an earlier prototype build. Start a new village.");
        s = new(id, World.LoadJson(saved.World)) { Step = saved.Step, Sequence = saved.Sequence, View = saved.View };
        sessions[id] = s;
        Record(s, "restored", new { message = "Restored safely paused" });
        return s;
    }
    public object Create()
    {
        lock (gate)
        {
            if (sessions.Count >= 32) throw new ArgumentException("There are already 32 open villages. Restart the local service to release inactive villages.");
            var s = new Session(Guid.NewGuid().ToString("N"), World.NewCrossingRecovery());
            sessions[s.Id] = s;
            Directory.CreateDirectory(Folder(s.Id));
            Record(s, "start", new { scenario = "broken-crossing" }); Save(s);
            return Observe(s);
        }
    }
    public object Observe(string id) { lock (gate) return Observe(Get(id)); }
    object Observe(Session s)
    {
        var w = s.World;
        var goals = w.ReadFirstHomeObjectives();
        return new
        {
            id = s.Id, build = Build, sequence = s.Sequence, step = s.Step, seconds = Math.Round(s.Step / 10d, 1), s.Paused, s.Speed,
            title = "The broken crossing", complete = w.Founding!.FirstHome!.Complete,
            resources = new { food = w.ReachableEdibleStored, logs = w.Available, population = w.Population, fed = w.People.Count(p => p.Fed), housed = w.Housed },
            objectives = new[] { goals.Home, goals.Food, goals.Meals.Replace("Neighbors fed", "Fresh meals") }, hint = goals.Hint, nextAction = goals.Action,
            map = new { w.Map.MinX, w.Map.MinZ, w.Map.Width, w.Map.Depth, water = w.Map.Water, excluded = w.Map.Excluded, stockpile = w.Stockpile },
            trees = w.Trees.Select(t => new { t.Id, t.Cell.X, t.Cell.Z, t.Logs, t.Felled, t.Growth, managed = w.ManagedWoodland.Contains(t.Cell), t.Preserved }),
            bushes = w.Bushes.Select(b => new { b.Id, b.Cell.X, b.Cell.Z }),
            buildings = w.Cottages.Select(c => new { c.Id, kind = c.Kind.ToString(), name = Buildings.Get(c.Kind).Name, c.Cell.X, c.Cell.Z, c.Rotation, c.Complete,
                c.Construction, c.Delivered, c.Required, c.Growth, c.Harvest, footprint = World.Footprint(c), entrance = c.Entrance,
                status = c.Complete ? c.Kind == BuildingKind.Cottage ? "Home" : c.Kind == BuildingKind.Bridge ? "Crossing open" : w.ReadWorkplace(c).State : $"Timber {c.Delivered}/{c.Required} · building {c.Construction:P0}" }),
            people = w.People.Select(p => new { p.Id, p.Name, x = p.Position.X, z = p.Position.Y, p.Status, task = p.Task.ToString(), p.Fed, p.Carried, cargo = p.Cargo.ToString(), freshMeal = w.Founding!.FirstHome!.Served.Contains(p.Id) }),
            events = w.History.TakeLast(6), view = s.View,
            actions = new[] { new { type = "repair", label = "Repair crossing", logs = 6 }, new { type = "garden", label = "Plant a local garden", logs = 4 }, new { type = "grove", label = "Harvest & renew a grove", logs = 0 } }
        };
    }
    public object Preview(string id, Order order) { lock (gate) return Preview(Get(id), order); }
    object Preview(Session s, Order o)
    {
        var w = s.World;
        if (o.Type == "grove")
        {
            CheckCell(w, o.X, o.Z); CheckCell(w, o.X2, o.Z2);
            var p = w.PreviewWoodlandCare(new(o.X, o.Z), new(o.X2, o.Z2), WoodlandIntent.Renew, false);
            return new { valid = p.Problem == null, problem = p.Problem, cells = p.Cells, logs = p.Timber, label = $"Harvest and renew {p.MatureTrees} trees · {p.Timber} logs" };
        }
        var (cell, kind, rotation) = Site(o);
        CheckCell(w, cell.X, cell.Z);
        var problem = w.PlacementProblem(cell, rotation, kind);
        return new { valid = problem == null, problem, cells = World.Footprint(cell, rotation, kind), entrance = World.Door(cell, rotation), logs = Buildings.Get(kind).Cost,
            label = kind == BuildingKind.Bridge ? "Repair the crossing · 6 logs" : "Plant a garden · 4 logs" };
    }
    static void CheckCell(World w, int x, int z) { if (!w.Map.Contains(new(x, z))) throw new ArgumentException("Choose a location inside the village map."); }
    static (Cell, BuildingKind, int) Site(Order o) => o.Type switch
    {
        "repair" => (World.RecoveryCrossing, BuildingKind.Bridge, 0),
        "garden" when o.Rotation is >= 0 and <= 3 => (new(o.X, o.Z), BuildingKind.VegetableGarden, o.Rotation),
        _ => throw new ArgumentException("Choose repair, garden or grove with a valid orientation.")
    };
    public object Command(string id, Order o)
    {
        lock (gate)
        {
            var s = Get(id); var w = s.World; string? problem = null; string message = "";
            s.LastSeen = DateTime.UtcNow;
            switch (o.Type)
            {
                case "repair": case "garden":
                    var (cell, kind, rotation) = Site(o);
                    problem = w.PlacementProblem(cell, rotation, kind);
                    if (problem == null) { var site = w.Place(cell, rotation, kind); if (site == null) problem = "That site is no longer available."; else message = "Plan placed. Neighbors collect timber and build automatically."; }
                    break;
                case "grove":
                    CheckCell(w, o.X, o.Z); CheckCell(w, o.X2, o.Z2);
                    var p = w.PreviewWoodlandCare(new(o.X, o.Z), new(o.X2, o.Z2), WoodlandIntent.Renew, false);
                    problem = p.Problem;
                    if (problem == null) { w.ApplyWoodlandCare(new(o.X, o.Z), new(o.X2, o.Z2), WoodlandIntent.Renew, false); message = $"Grove ordered: {p.Timber} logs. Neighbors harvest and replant automatically."; }
                    break;
                case "cancel":
                    var planned = w.Cottages.FirstOrDefault(c => c.Id == o.SiteId && !c.Complete);
                    if (planned == null || !w.Cancel(o.SiteId)) problem = "Only an unfinished plan can be cancelled.";
                    else message = "Plan cancelled.";
                    break;
                case "pause": s.Paused = true; message = "Village paused and saved."; break;
                case "resume":
                    if (o.Speed is not (1 or 3 or 6)) problem = "Choose 1×, 3× or 6×.";
                    else { s.Speed = o.Speed; s.Paused = false; message = $"Village running at {s.Speed}×."; }
                    break;
                case "advance":
                    if (!s.Paused) problem = "Pause before advancing to the next decision.";
                    else if (o.Seconds is < 1 or > 30) problem = "Advance between 1 and 30 seconds.";
                    else
                    {
                        var before = w.ReadFirstHomeObjectives(); int events = w.History.Count;
                        for (int i = 0; i < o.Seconds * 10; i++)
                        {
                            Step(s); if (i % 10 == 9) Record(s, "frame", null);
                            var after = w.ReadFirstHomeObjectives();
                            if (o.UntilEvent && i >= 9 && (after.Done != before.Done || after.Action != before.Action || w.History.Count != events)) break;
                        }
                        message = "Time advanced. Village remains paused.";
                    }
                    break;
                case "bookmark": message = "Moment marked. Your view and village state were saved for review."; break;
                case "view": if (o.View != null) s.View = Clean(o.View); break;
                default: problem = "Unknown action. Read /api/guide for ordinary player commands."; break;
            }
            if (o.View != null) s.View = Clean(o.View);
            Record(s, o.Type == "bookmark" ? "bookmark" : "command", new { order = o with { Note = o.Note?[..Math.Min(240, o.Note.Length)] }, accepted = problem == null, message = problem ?? message });
            Save(s);
            return new { accepted = problem == null, message = problem ?? message, state = Observe(s) };
        }
    }
    static ClientView Clean(ClientView v) => v with { Panel = v.Panel[..Math.Min(v.Panel.Length, 80)], Selection = v.Selection?[..Math.Min(v.Selection.Length, 80)],
        CameraX = float.IsFinite(v.CameraX) ? Math.Clamp(v.CameraX, -30, 30) : 0, CameraZ = float.IsFinite(v.CameraZ) ? Math.Clamp(v.CameraZ, -30, 30) : 0,
        Zoom = float.IsFinite(v.Zoom) ? Math.Clamp(v.Zoom, 6, 45) : 19, Width = Math.Clamp(v.Width, 0, 8000), Height = Math.Clamp(v.Height, 0, 8000),
        Message = (v.Message ?? "")[..Math.Min(v.Message?.Length ?? 0, 500)], PreviewCells = v.PreviewCells?.Take(128).ToArray() };
    public object Heartbeat(string id, ClientView v)
    {
        lock (gate)
        {
            var s = Get(id); s.LastSeen = DateTime.UtcNow; s.View = Clean(v);
            if (!v.Active && !s.Paused) { s.Paused = true; Record(s, "interrupted", null); Save(s); }
            return Observe(s);
        }
    }
    static void Step(Session s) { s.World.Tick(.1f); s.Step++; }
    public void Tick()
    {
        lock (gate)
        {
            foreach (var s in sessions.Values.Where(s => !s.Paused))
            {
                if ((DateTime.UtcNow - s.LastSeen).TotalSeconds > 6)
                { s.Paused = true; Record(s, "disconnected", null); Save(s); continue; }
                for (int i = 0; i < s.Speed; i++) Step(s);
                if ((DateTime.UtcNow - s.LastFrame).TotalSeconds >= 1) Record(s, "frame", null);
                if ((DateTime.UtcNow - s.LastSave).TotalSeconds >= 5) Save(s);
            }
        }
    }
    void Record(Session s, string type, object? data)
    {
        s.Sequence++; s.LastFrame = DateTime.UtcNow;
        File.AppendAllText(Path.Combine(Folder(s.Id), "recording.jsonl"), JsonSerializer.Serialize(new { sequence = s.Sequence, utc = s.LastFrame, type, data, state = Observe(s) }, Json) + "\n");
        if (type == "bookmark") File.WriteAllText(Path.Combine(Folder(s.Id), $"bookmark-{s.Sequence}.json"), s.World.SaveJson());
    }
    sealed record Saved(string Build, string World, long Step, long Sequence, ClientView View);
    void Save(Session s)
    {
        // Immutable checkpoints avoid replacing files while Dropbox is indexing them.
        // Rename a complete new file into the journal; a crash can leave only an ignored .tmp.
        var path = Path.Combine(Folder(s.Id), $"snapshot-{s.Sequence:D12}-{Guid.NewGuid():N}.json");
        var temporary = path + ".tmp";
        File.WriteAllText(temporary, JsonSerializer.Serialize(new Saved(Build, s.World.SaveJson(), s.Step, s.Sequence, s.View), Json));
        File.Move(temporary, path);
        s.LastSave = DateTime.UtcNow;
    }
    public string Recording(string id) { lock (gate) { Get(id); return File.ReadAllText(Path.Combine(Folder(id), "recording.jsonl")); } }
    public void SaveAll() { lock (gate) foreach (var s in sessions.Values) { s.Paused = true; Save(s); } }
}

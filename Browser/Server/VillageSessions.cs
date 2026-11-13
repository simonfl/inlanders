using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Inlanders.Simulation;

namespace Inlanders.Browser;

public sealed record Order(string Type, int X = 0, int Z = 0, int X2 = 0, int Z2 = 0, int Rotation = 0,
    int SiteId = -1, int Speed = 1, int Seconds = 15, bool UntilEvent = true, string? Note = null, ClientView? View = null);
public sealed record ClientView(bool Active = true, string Panel = "village", string? Selection = null,
    float CameraX = 0, float CameraZ = 0, float Zoom = 19, int Width = 0, int Height = 0,
    string Message = "", Cell[]? PreviewCells = null, bool PreviewValid = false, bool GoalsExpanded = false);
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
    public string Notice = "";
}
public sealed partial class VillageSessions
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
        lock (gate)
        {
        if (sessions.TryGetValue(id, out var s)) return s;
        var directory = Folder(id);
        if (!Directory.Exists(directory)) throw new KeyNotFoundException();
        var path = Directory.GetFiles(directory, "snapshot-*.json").OrderDescending().FirstOrDefault();
        if (path == null) throw new KeyNotFoundException();
        var saved = JsonSerializer.Deserialize<Saved>(File.ReadAllText(path), Json)!;
        if (saved.Build != Build) throw new ArgumentException("This village belongs to an earlier prototype build. Start a new village.");
        s = new(id, World.LoadJson(saved.World)) { Step = saved.Step, Sequence = saved.Sequence, View = saved.View, Notice = saved.Notice };
        sessions[id] = s;
        Record(s, "restored", new { message = "Restored safely paused" });
        return s;
        }
    }
    public object Create(string scenario = "crossing")
    {
        lock (gate)
        {
            if (sessions.Count >= 32) throw new ArgumentException("There are already 32 open villages. Restart the local service to release inactive villages.");
            var world = scenario switch { "room-by-river" => World.NewRiverside(), "room-by-river-far" => World.NewRiverside(true), "crossing" => World.NewCrossingRecovery(), "river-closing" => World.NewBrowserRiverClosing(), "timber-trade" => World.NewTimberTrade(), "timber-trade-ripe" => World.NewTimberTrade(true), "timber-control" => World.NewTimberTrade(false, false), _ => throw new ArgumentException("Choose crossing, river-closing, timber-trade, timber-trade-ripe or timber-control.") };
            var s = new Session(Guid.NewGuid().ToString("N"), world);
            sessions[s.Id] = s;
            Directory.CreateDirectory(Folder(s.Id));
            Record(s, "start", new { scenario }); Save(s);
            return Observe(s);
        }
    }
    public object Observe(string id) { var s = Get(id); lock (s) return Observe(s); }
    object Observe(Session s)
    {
        var w = s.World;
        var goals = w.ReadFirstHomeObjectives();
        return new
        {
            id = s.Id, build = Build, sequence = s.Sequence, step = s.Step, seconds = Math.Round(s.Step / 10d, 1), s.Paused, s.Speed,
            scenario = Scenario(w), title = Title(w), goalTitle = GoalTitle(w), complete = Complete(w), riverside = w.Riverside==null?null:w.ReadRiverside(), river = River(w), trade = Trade(w), notice = s.Notice,
            resources = new { food = w.ReachableEdibleStored, logs = w.Available, population = w.Population, fed = w.People.Count(p => p.Fed), housed = w.Housed, beds = w.Beds },
            objectives = ObjectiveLines(w), hint = Hint(w), nextAction = goals.Action,
            map = new { w.Map.MinX, w.Map.MinZ, w.Map.Width, w.Map.Depth, water = w.Map.Water, excluded = w.Map.Excluded, stockpile = w.Stockpile,
                fishing = w.Map.FishingGrounds.Select(g => new { g.Id, g.Name, g.Cell.X, g.Cell.Z, available = w.AvailableFish(g), g.Capacity, renewal = g.RegrowthPerSecond * 60 }),
                stone = w.Map.StoneDeposits.Select(d => new { d.Cell.X, d.Cell.Z, access = d.Access }), clearings = w.Map.Wildlife.Select(h => h.Cell) },
            trees = w.Trees.Select(t => new { t.Id, t.Cell.X, t.Cell.Z, t.Logs, t.Felled, t.Growth, managed = w.ManagedWoodland.Contains(t.Cell), t.Preserved }),
            bushes = w.Bushes.Select(b => new { b.Id, b.Cell.X, b.Cell.Z }),
            buildings = VisibleBuildings(w).Select(c => new { c.Id, kind = c.Kind.ToString(), name = Buildings.Get(c.Kind).Name, c.Cell.X, c.Cell.Z, c.Rotation, c.Complete,
                c.Construction, c.Delivered, c.Required, c.Growth, c.Harvest,
                launch = c.Kind == BuildingKind.FishingDock ? (Cell?)c.Launch : null,
                fishing = c.Kind == BuildingKind.FishingDock ? w.FishingSurvey(c.Cell, c.Rotation) + " All docks share these stocks." : null,
                boat = c.Boat == null ? null : new { x = c.Boat.Position.X, z = c.Boat.Position.Y, c.Boat.Heading, phase = c.Boat.Phase.ToString(), c.Boat.FisherId, c.Boat.Fish }, footprint = World.Footprint(c), entrance = c.Entrance,
                status = c.Id<0 ? "Clearing ground, then building automatically" : c.Complete ? c.Kind == BuildingKind.Cottage ? "Home" : c.Kind == BuildingKind.Bridge ? "Crossing open" : w.ReadWorkplace(c).State : $"Timber {c.Delivered}/{c.Required} · building {c.Construction:P0}" }),
            people = w.People.Select(p => new { p.Id, p.Name, x = w.PassengerBoat(p)?.Position.X ?? p.Position.X, z = w.PassengerBoat(p)?.Position.Y ?? p.Position.Y, aboard = p.Task == Work.Aboard, heading = w.PassengerBoat(p)?.Heading ?? 0, p.Status, task = p.Task.ToString(), p.Fed, p.Carried, cargo = p.Cargo.ToString(), freshMeal = w.Riverside?.Served.Contains(p.Id) ?? w.Founding!.FirstHome!.Served.Contains(p.Id), homeId=p.HomeId, route=p.Route.ToArray() }),
            events = w.History.TakeLast(6), view = s.View,
            actions = w.Riverside!=null ? new[]{"home","garden","field","dock","grove","shortcut","welcome"} : (Scenario(w) == "crossing" ? new[] { "repair", "garden", "grove" } : w.Founding!.FirstHome!.Trade != null ? new[] { "home", "garden", "field", "dock", "grove" } : new[] { "home", "garden", "field", "dock", "grove", "next-season" }).Concat(w.Founding!.FirstHome!.Trade != null ? new[] { "trade-buy", "trade-decline", "trade-cancel" } : Array.Empty<string>())
        };
    }
    public object Preview(string id, Order order) { var s = Get(id); lock (s) return Preview(s, order); }
    object Preview(Session s, Order o)
    {
        var w = s.World;
        CheckAction(w, o.Type);
        if (o.Type == "grove")
        {
            CheckCell(w, o.X, o.Z); CheckCell(w, o.X2, o.Z2);
            var p = w.PreviewWoodlandCare(new(o.X, o.Z), new(o.X2, o.Z2), WoodlandIntent.Renew, false);
            return new { valid = (p.Problem ?? w.RiversideGroveProblem(new(o.X,o.Z),new(o.X2,o.Z2))) == null, problem = p.Problem ?? w.RiversideGroveProblem(new(o.X,o.Z),new(o.X2,o.Z2)), cells = p.Cells, logs = p.Timber, label = $"Harvest and renew {p.MatureTrees} trees · {p.Timber} logs" };
        }
        var (cell, kind, rotation) = Site(o);
        CheckCell(w, cell.X, cell.Z);
        var problem = w.Riverside!=null ? w.RiversideSiteProblem(cell,rotation,kind) : w.PlacementProblem(cell, rotation, kind);
        var clearing = w.Riverside!=null ? w.PreviewGroundPreparation(cell,rotation,kind) : null;
        if (problem?.Contains("[C]") == true) problem = "A tree or stump occupies this site. Choose clear ground for this prototype.";
        return new { valid = problem == null, problem, cells = World.Footprint(cell, rotation, kind), entrance = kind == BuildingKind.FishingDock ? w.DockEntrance(cell, rotation) : World.Door(cell, rotation), launch = kind == BuildingKind.FishingDock ? (Cell?)w.DockLaunch(cell, rotation) : null, logs = Buildings.Get(kind).Cost,
            route = o.Type=="shortcut" ? w.RiversideRoute(false) : null, shortcutRoute = o.Type=="shortcut" ? w.RiversideRoute(true) : null, clearing = clearing?.Trees.Length ?? 0, label = (o.Type=="shortcut" ? $"Optional shortcut · 6 logs. Yard to far clearing: {w.ReadRiverside().WalkNow} → {w.ReadRiverside().WalkWithShortcut} steps. Existing crossing stays open. " : clearing!=null ? $"Clear {clearing.Trees.Length} tree {(clearing.Trees.Length==1?"spot":"spots")} ({clearing.Logs} logs recovered), then build. Grove spots here will not regrow. " : "") + $"{Buildings.Get(kind).Name} · {Buildings.Get(kind).Cost} logs" + (kind == BuildingKind.FishingDock && problem == null ? ". Up to 4 fish per trip. " + w.FishingSurvey(cell, rotation).Replace("\n", " ") + " Shared by all docks." : "") };
    }
    static void CheckCell(World w, int x, int z) { if (!w.Map.Contains(new(x, z))) throw new ArgumentException("Choose a location inside the village map."); }
    static (Cell, BuildingKind, int) Site(Order o) => o.Type switch
    {
        "shortcut" => (World.RiversideShortcut, BuildingKind.Bridge, 0),
        "repair" => (World.RecoveryCrossing, BuildingKind.Bridge, 0),
        "garden" when o.Rotation is >= 0 and <= 3 => (new(o.X, o.Z), BuildingKind.VegetableGarden, o.Rotation),
        "dock" when o.Rotation is >= 0 and <= 3 => (new(o.X, o.Z), BuildingKind.FishingDock, o.Rotation),
        "field" when o.Rotation is >= 0 and <= 3 => (new(o.X, o.Z), BuildingKind.VegetableField, o.Rotation),
        "home" when o.Rotation is >= 0 and <= 3 => (new(o.X, o.Z), BuildingKind.Cottage, o.Rotation),
        _ => throw new ArgumentException("Choose a building with a valid orientation.")
    };
    static void CheckAction(World w, string type)
    {
        if(type is "shortcut" or "welcome" && w.Riverside==null)throw new ArgumentException("That action belongs to Room by the river.");
        if(w.Riverside!=null && type is "next-season" or "repair" or "trade-buy" or "trade-decline" or "trade-cancel")throw new ArgumentException("That action is not part of this village.");
        if (w.Founding!.FirstHome!.Trade != null && type == "next-season") throw new ArgumentException("This visit follows the displayed arrival time; there is no supply-season advance.");
        if (Scenario(w) == "crossing" && type is "home" or "field" or "dock" or "next-season") throw new ArgumentException("That action belongs to the longer river scenario.");
        if (Scenario(w) != "crossing" && type == "repair") throw new ArgumentException("This settlement already has a working crossing.");
    }
    public object Command(string id, Order o)
    {
        var s = Get(id);
        lock (s)
        {
            var w = s.World; string? problem = null; string message = "";
            CheckAction(w, o.Type);
            s.LastSeen = DateTime.UtcNow;
            switch (o.Type)
            {
                case "shortcut": case "repair": case "garden": case "home": case "field": case "dock":
                    var (cell, kind, rotation) = Site(o);
                    if(w.Riverside!=null){problem=w.RiversideSiteProblem(cell,rotation,kind);if(problem==null){if(!w.PlanRiverside(cell,rotation,kind))problem="That ground is no longer available.";else message="Work ordered. Neighbors clear any trees, then build automatically.";}break;}
                    problem = w.PlacementProblem(cell, rotation, kind);
                    if (problem == null) { var site = w.Place(cell, rotation, kind); if (site == null) problem = "That site is no longer available."; else message = "Plan placed. Neighbors collect timber and build automatically."; }
                    break;
                case "grove":
                    CheckCell(w, o.X, o.Z); CheckCell(w, o.X2, o.Z2);
                    var p = w.PreviewWoodlandCare(new(o.X, o.Z), new(o.X2, o.Z2), WoodlandIntent.Renew, false);
                    problem = p.Problem ?? w.RiversideGroveProblem(new(o.X,o.Z),new(o.X2,o.Z2));
                    if (problem == null) { w.ApplyWoodlandCare(new(o.X, o.Z), new(o.X2, o.Z2), WoodlandIntent.Renew, false); message = $"Grove ordered: {p.Timber} logs. Neighbors harvest and replant automatically."; }
                    break;
                case "cancel":
                    if(w.CancelRiversidePlan(o.SiteId)){message="Ground plan cancelled; harvested timber is kept.";break;}
                    var planned = w.Cottages.FirstOrDefault(c => c.Id == o.SiteId && !c.Complete);
                    if (planned == null || !w.Cancel(o.SiteId)) problem = "Only an unfinished plan can be cancelled.";
                    else message = "Plan cancelled.";
                    break;
                case "trade-buy": case "trade-decline":
                    problem = w.DecideTimberTrade(o.Type == "trade-buy");
                    if (problem == null) { message = w.History.Last(); s.Notice = ""; }
                    break;
                case "trade-cancel":
                    problem = w.CancelTimberTrade();
                    if(problem==null){message=w.History.Last();s.Notice="";}
                    break;
                case "welcome":
                    problem=w.RiversideWelcomeProblem();if(problem==null){w.WelcomeRiverside();s.Paused=true;s.Notice=w.History.Last();message=s.Notice;}break;
                case "next-season":
                    problem = w.AdvanceRiverScheduleProblem();
                    if (problem == null) { w.AdvanceRiverSchedule(); Step(s); message = "The next event arrived. Village paused so you can look around."; }
                    break;
                case "pause": s.Paused = true; message = "Village paused and saved."; break;
                case "resume":
                    if (o.Speed is not (1 or 3 or 6)) problem = "Choose 1×, 3× or 6×.";
                    else { s.Speed = o.Speed; s.Paused = false; s.Notice = ""; message = $"Village running at {s.Speed}×."; }
                    break;
                case "advance":
                    if (!s.Paused) problem = "Pause before advancing to the next decision.";
                    else if (o.Seconds is < 1 or > 30) problem = "Advance between 1 and 30 seconds.";
                    else
                    {
                        s.Notice = "";
                        var before = w.ReadFirstHomeObjectives(); int events = w.History.Count;
                        for (int i = 0; i < o.Seconds * 10; i++)
                        {
                            bool milestone = Step(s); if (i % 10 == 9 || milestone) Record(s, "frame", null);
                            var after = w.ReadFirstHomeObjectives();
                            if (milestone || o.UntilEvent && i >= 9 && (after.Done != before.Done || after.Action != before.Action || w.History.Count != events)) break;
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
        var s = Get(id);
        lock (s)
        {
            s.LastSeen = DateTime.UtcNow; s.View = Clean(v);
            if (!v.Active && !s.Paused) { s.Paused = true; Record(s, "interrupted", null); Save(s); }
            return Observe(s);
        }
    }
    static bool Step(Session s)
    {
        var r = s.World.Founding!.FirstHome!.RiverClosing;
        var t = s.World.Founding.FirstHome.Trade;
        var roomBefore=(s.World.Riverside?.Arrived,s.World.Riverside?.Complete);
        var tradeBefore = (t?.Decision, t?.Arrived, t?.Complete);
        var before = (r?.Boats, r?.Closed, r?.Complete);
        s.World.Tick(.1f); s.Step++;
        bool milestone = before != (r?.Boats, r?.Closed, r?.Complete);
        if (milestone)
        {
            s.Paused = true;
            s.Notice = r!.Complete ? "Your village made it. All twelve neighbors can stand on their own." : r.Closed ? "The river has closed. No more supplies are coming. Your final trial begins now." : r.Boats == 1 ? "The first boat is here: four neighbors and 24 food. Eight people now call this village home." : "The last boat is here: four neighbors, but no food. Twelve people now depend on your harvests.";
        }
        if (tradeBefore != (t?.Decision, t?.Arrived, t?.Complete))
        { milestone = true; s.Paused = true; s.Notice = s.World.History.Last(); }
        if(roomBefore!=(s.World.Riverside?.Arrived,s.World.Riverside?.Complete)){milestone=true;s.Paused=true;s.Notice=s.World.History.Last();}
        return milestone;
    }
    public void Tick()
    {
        Session[] active;
        lock (gate) active = sessions.Values.ToArray();
        foreach (var s in active)
        {
            // An agent advancing its own session must not block another village's
            // clock, heartbeat or input. Each world still has one writer at a time.
            if (!Monitor.TryEnter(s)) continue;
            try
            {
                if (s.Paused) continue;
                if ((DateTime.UtcNow - s.LastSeen).TotalSeconds > 6)
                { s.Paused = true; Record(s, "disconnected", null); Save(s); continue; }
                for (int i = 0; i < s.Speed && !s.Paused; i++) Step(s);
                if ((DateTime.UtcNow - s.LastFrame).TotalSeconds >= 1) Record(s, "frame", null);
                if ((DateTime.UtcNow - s.LastSave).TotalSeconds >= 5) Save(s);
            }
            finally { Monitor.Exit(s); }
        }
    }
    void Record(Session s, string type, object? data)
    {
        s.Sequence++; s.LastFrame = DateTime.UtcNow;
        File.AppendAllText(Path.Combine(Folder(s.Id), "recording.jsonl"), JsonSerializer.Serialize(new { sequence = s.Sequence, utc = s.LastFrame, type, data, state = Observe(s) }, Json) + "\n");
        if (type == "bookmark") File.WriteAllText(Path.Combine(Folder(s.Id), $"bookmark-{s.Sequence}.json"), s.World.SaveJson());
    }
    sealed record Saved(string Build, string World, long Step, long Sequence, ClientView View, string Notice);
    void Save(Session s)
    {
        // Immutable checkpoints avoid replacing files while Dropbox is indexing them.
        // Rename a complete new file into the journal; a crash can leave only an ignored .tmp.
        var path = Path.Combine(Folder(s.Id), $"snapshot-{s.Sequence:D12}-{Guid.NewGuid():N}.json");
        var temporary = path + ".tmp";
        File.WriteAllText(temporary, JsonSerializer.Serialize(new Saved(Build, s.World.SaveJson(), s.Step, s.Sequence, s.View, s.Notice), Json));
        File.Move(temporary, path);
        s.LastSave = DateTime.UtcNow;
    }
    public string Recording(string id) { var s = Get(id); lock (s) return File.ReadAllText(Path.Combine(Folder(id), "recording.jsonl")); }
    public void SaveAll()
    {
        Session[] active;
        lock (gate) active = sessions.Values.ToArray();
        foreach (var s in active) lock (s) { s.Paused = true; Save(s); }
    }
}

using Inlanders.Simulation;

namespace Inlanders.Browser;

public sealed record RiverView(int Boats, bool Closed, int TargetBeds, int ReserveTarget,
    int Delivered, int Needed, int SampleSeconds, int UntilEvent, string Timetable,
    string EarlyLabel, string? EarlyProblem, int StableSeconds, string Supply, string Phase);

public sealed partial class VillageSessions
{
    static string Scenario(World w) => w.Founding!.FirstHome!.Trade is {} t ? (!t.OfferEnabled ? "timber-control" : t.NearHarvest ? "timber-trade-ripe" : "timber-trade") : w.Founding!.FirstHome!.RiverClosing == null ? "crossing" : "river-closing";
    static string Title(World w) => w.Founding!.FirstHome!.Trade is {} t ? "The price of provisions" + (t.NearHarvest ? " · near harvest" : "") : Scenario(w) == "crossing" ? "The broken crossing" : "Before the river closes";
    static RiverView? River(World w)
    {
        if (w.Founding!.FirstHome!.RiverClosing is not {} r) return null;
        int target = r.Boats == 0 ? 8 : 12;
        var flow = w.ReadFoodFlow();
        int until = Math.Max(0, (int)Math.Ceiling((r.Boats < 2 ? (r.Boats + 1) * 480 : 1440) - w.Food.Time));
        string clock = $"{until / 60}:{until % 60:00}";
        return new(r.Boats, r.Closed, target, r.Closed ? w.Population : target * 2,
            flow.Delivered, target * 3, (int)flow.Seconds, until,
            r.Closed ? "River closed · no more supplies" : r.Boats == 0 ? $"First boat in {clock} · +4 neighbors, +24 food" : r.Boats == 1 ? $"Last boat in {clock} · +4 neighbors, no food" : $"River closes in {clock} · prepare to stand alone",
            r.Boats < 2 ? "Welcome the boat early" : "Begin the final trial",
            w.AdvanceRiverScheduleProblem(), (int)MathF.Floor(r.StableSeconds), w.RiverTrialSupplyText(),
            r.Complete ? "complete" : r.Closed ? "sustain" : r.Boats == 2 ? "reserve" : r.Boats == 1 ? "last-arrival" : "first-arrival");
    }
    static object? Trade(World w)
    {
        if (w.Founding!.FirstHome!.Trade is not {} t) return null;
        var field = w.Cottages.First(c => c.Kind == BuildingKind.VegetableField);
        return new { t.Decision, t.DecidedAt, t.PackedLogs, t.Arrived, t.NearHarvest, t.OfferEnabled, cost = World.TradeLogCost, food = World.TradeFood, availableLogs = w.AvailableLogsAt(null), problem = w.TimberTradeProblem(), departureIn = Math.Max(0, (int)Math.Ceiling(World.TradeDeparture-w.Food.Time)), arrivalIn = Math.Max(0, (int)Math.Ceiling(World.TradeArrival-w.Food.Time)), crop = field.Harvest > 0 ? "First crop ripe" : field.Planted ? $"First crop {(int)(field.Growth*100)}% grown" : "Field being replanted" };
    }
    static string GoalTitle(World w) => w.Founding!.FirstHome!.Trade is {} t ? (t.Complete ? "✓ Eight neighbors are settled" : "Make a home and a living for eight") : River(w) switch
    {
        null => w.Founding!.FirstHome!.Complete ? "✓ Village recovered" : "Help four neighbors feed themselves again",
        { Phase: "complete" } => "✓ Twelve neighbors can stand on their own",
        { Phase: "sustain" } => "Keep twelve neighbors supplied",
        { Phase: "reserve" } => "Build a reserve before the river closes",
        { Phase: "last-arrival" } => "Make room for the last four neighbors",
        _ => "Prepare a home and food for four more"
    };
    static string[] ObjectiveLines(World w)
    {
        var goals = w.ReadFirstHomeObjectives();
        var river = River(w);
        if (river == null) return new[] { goals.Home, goals.Food, goals.Meals.Replace("Neighbors fed", "Fresh meals") };
        if (river.Closed) return new[] {
            $"{(w.Housed == w.Population && w.People.All(p => p.Fed) ? "✓" : "○")} Housed {w.Housed}/{w.Population} · fed {w.People.Count(p => p.Fed)}/{w.Population}",
            $"{(w.ReachableEdibleStored >= w.Population && w.Founding!.FirstHome!.Served.Count == w.Population ? "✓" : "○")} Reserve {w.ReachableEdibleStored}/{w.Population} · fresh meals {w.Founding!.FirstHome!.Served.Count}/{w.Population}"
        }.Concat(goals.Meals.Split('\n').Skip(1)).ToArray();
        return new[] {
            $"{(w.Beds >= river.TargetBeds ? "✓" : "○")} Homes · {w.Beds}/{river.TargetBeds} beds",
            $"{(w.ReachableEdibleStored >= river.ReserveTarget ? "✓" : "○")} Reserve · {w.ReachableEdibleStored}/{river.ReserveTarget} food",
            $"{(river.Delivered >= river.Needed && river.SampleSeconds >= 180 ? "✓" : "○")} Fresh supply · {river.Delivered}/{river.Needed}" + (river.SampleSeconds < 180 ? $" · measuring {river.SampleSeconds}/180s" : " in last 3 min")
        };
    }
    static string Hint(World w)
    {
        var r = River(w);
        if (r == null) return w.ReadFirstHomeObjectives().Hint;
        if (w.Founding!.FirstHome!.Complete) return "Everyone has a home, fresh meals and a working food supply. Your village made it. You can keep playing.";
        var hungry = w.People.Count(p => !p.Fed);
        if (hungry > 0) return $"{hungry} neighbors are hungry. Grow food to recover; work slows during shortages.";
        if (r.Closed)
        {
            var problem = w.RiverStabilityProblem();
            if (problem?.StartsWith("Fresh meals since closure:") == true)
            {
                var waiting = w.People.Where(p => !w.Founding!.FirstHome!.Served.Contains(p.Id)).Select(p => p.Name).ToArray();
                return $"Fresh meals still needed: {string.Join(", ", waiting.Take(3))}{(waiting.Length > 3 ? $" and {waiting.Length - 3} more" : "")}. Neighbors eat when hungry; Village lists everyone.";
            }
            if (problem != null) return problem;
            var flow = w.ReadFoodFlow();
            return flow.Delivered < flow.Required
                ? $"Recent harvests: {flow.Delivered} food / {flow.Required} meals needed. Harvests come in batches; add capacity if stores keep falling."
                : $"Recent harvests: {flow.Delivered} food / {flow.Required} meals needed. Keep production running for {Math.Max(0, 240 - r.StableSeconds)} more steady seconds.";
        }
        var plans = w.Cottages.Where(c => !c.Complete).ToArray();
        if (plans.Length > 0 && plans.Sum(c => c.Remaining(Resource.Logs)) > w.Available && !w.Trees.Any(t => !t.Preserved && t.Logs > 0))
            return "Your building plans need more timber. Work a grove so neighbors can finish them.";
        if (w.Beds < r.TargetBeds)
        {
            int plannedBeds = plans.Count(c => c.Kind == BuildingKind.Cottage) * 2;
            if (w.Beds + plannedBeds >= r.TargetBeds) return "Enough cottages are planned. Neighbors must finish building them before their beds count.";
            int count = (int)Math.Ceiling((r.TargetBeds - w.Beds - plannedBeds) / 2d);
            return $"Plan {count} more {(count == 1 ? "cottage" : "cottages")} for the arriving neighbors. Each sleeps two.";
        }
        if (r.Delivered < r.Needed) return "Grow food near homes and stores. Gardens start cheaply; larger fields yield more per harvest. Neighbors share the work automatically.";
        if (w.ReachableEdibleStored < r.ReserveTarget) return "Let the harvest build a reserve. If stores keep shrinking, plant another food source.";
        return r.EarlyProblem == null ? "You are ready. Welcome the next event early, or keep preparing." : r.EarlyProblem;
    }
}

using System;
using System.Linq;

namespace Inlanders.Simulation;

public sealed class TimberTradeProgress
{
    public bool NearHarvest { get; set; }
    public bool OfferEnabled { get; set; } = true;
    public string Decision { get; set; } = "waiting";
    public float DecidedAt { get; set; }
    public bool Arrived { get; set; }
    public bool Complete { get; set; }
    public int ExportedLogs { get; set; }
    public int PackedLogs { get; set; }
    public int ArrivalDeliveries { get; set; }
    public int ArrivalMeals { get; set; }
}

public sealed partial class World
{
    public const int TradeLogCost = 12, TradeFood = 16;
    public const int TradeOfferArrival = 45, TradeDeparture = TradeOfferArrival + 60, TradeArrival = 120;

    public static World NewTimberTrade(bool nearHarvest = false, bool offer = true)
    {
        var w = NewBrowserRiverClosing();
        w.Founding!.FirstHome!.RiverClosing = null;
        w.Founding.FirstHome.Trade = new() { NearHarvest = nearHarvest, OfferEnabled = offer, Decision = offer ? "waiting" : "unavailable" };
        w.Cottages.Single(c => c.Kind == BuildingKind.VegetableField).Growth = nearHarvest ? .9f : 0;
        w.Food.InitialBerries = w.Food.Berries = 4;
        w.Map.Name = "Le prix des provisions · The price of provisions";
        w.History.Clear();
        w.History.Add("Four portions and twelve logs remain. Four neighbors arrive in two minutes without supplies. Start by working a grove for the timber their cottages will need. " + (nearHarvest ? "The first crop is almost ripe." : "The first crop has only just started growing."));
        w.Validate();
        return w;
    }

    public string? TimberTradeProblem()
    {
        var t = Founding?.FirstHome?.Trade;
        if (t?.Decision == "waiting") return "The trader has not arrived yet.";
        if (t == null || !t.OfferEnabled || t.Decision != "open" || Food.Time >= TradeDeparture) return "The trader's offer is no longer available.";
        return null;
    }

    public string? DecideTimberTrade(bool buy)
    {
        var t = Founding?.FirstHome?.Trade;
        if (t?.Decision == "waiting") return "The trader has not arrived yet.";
        if (t == null || t.Decision != "open" || Food.Time >= TradeDeparture) return "The trader's offer is no longer available.";
        if (buy && TimberTradeProblem() is {} problem) return problem;
        t.DecidedAt = Food.Time;
        t.Decision = buy ? "ordered" : "declined";
        History.Add(buy ? "Timber order agreed. Resume to pack available logs automatically." : "Kept the timber for the village. Neighbors will rely on the local harvest; the trader departs without a sale.");
        if(buy) PackTradeTimber(t);
        _retry = 0;
        return null;
    }

    public string? CancelTimberTrade()
    {
        var t=Founding?.FirstHome?.Trade;
        if(t?.Decision!="ordered")return "Only an unfinished timber order can be cancelled.";
        int reclaimed=t.PackedLogs;_yardLogs+=reclaimed;t.PackedLogs=0;
        t.Decision="cancelled";t.DecidedAt=Food.Time;_retry=0;
        History.Add($"Timber order cancelled. {reclaimed} packed logs returned to village storage; no provisions were received.");
        return null;
    }

    private void PackTradeTimber(TimberTradeProgress t)
    {
        if(t.Decision!="ordered")return;
        // Use only genuinely free central timber. A worker's existing reservation
        // and carried load remain attached to that work, even after this agreement.
        int packed=Math.Min(TradeLogCost-t.PackedLogs,AvailableLogsAt(null));
        _yardLogs-=packed;t.PackedLogs+=packed;
        if(t.PackedLogs<TradeLogCost)return;
        t.PackedLogs=0;t.ExportedLogs=TradeLogCost;t.Decision="traded";t.DecidedAt=Food.Time;
        Food.InitialBerries+=TradeFood;Food.Berries+=TradeFood;_retry=0;
        History.Add($"Timber shipment complete: twelve logs exchanged for sixteen provisions. {AvailableLogsAt(null)} available logs remain. These provisions do not count as local harvests.");
    }

    private void AdvanceTimberTrade()
    {
        if (Founding?.FirstHome?.Trade is not {} t) return;
        PackTradeTimber(t);
        if(t.Complete)return;
        if (t.Decision == "waiting" && Food.Time >= TradeOfferArrival)
        {
            t.Decision = "open";
            History.Add("A trader has arrived. Consider the timber order below; the village is paused.");
        }
        if (t.Decision == "open" && Food.Time >= TradeDeparture)
        { t.DecidedAt = Food.Time; t.Decision = "departed"; History.Add("The trader has departed. Your timber stays here; local harvests will feed the village."); }
        if (!t.Arrived && Food.Time >= TradeArrival)
        {
            foreach (var name in new[] { "Anne", "Jacques", "Madeleine", "Étienne" })
                People.Add(new Villager { Id = Population, Name = name, Position = YardAccess.Point, Role = Role.Unassigned, SharedWorker = true, NextMealTime = Food.Time + 20, Status = "Arrived — looking for a home and useful work" });
            t.Arrived = true; t.ArrivalDeliveries = DeliveredEdible; t.ArrivalMeals = RiverMealsEaten;
            Founding.FirstHome.Served.Clear(); ReconcileHomes(); _retry = 0;
            History.Add("Four neighbors arrived without provisions. Settle all eight: homes, fresh local meals, one meal in reserve and local deliveries covering meals since arrival.");
        }
        if (t.Arrived && Housed == 8 && People.All(p => p.Fed) && ReachableEdibleStored >= 8 &&
            Founding.FirstHome.Served.Count == 8 && DeliveredEdible - t.ArrivalDeliveries >= Math.Max(16, RiverMealsEaten - t.ArrivalMeals))
        { t.Complete = true; History.Add("Eight neighbors are settled. Local harvests have covered their meals since arrival, with food still in reserve. No extra stability timer is required."); }
    }

    private FirstHomeObjectives ReadTimberTradeObjectives()
    {
        var t = Founding!.FirstHome!.Trade!;
        int delivered = t.Arrived ? DeliveredEdible - t.ArrivalDeliveries : 0;
        int eaten = t.Arrived ? RiverMealsEaten - t.ArrivalMeals : 0;
        int needed = Math.Max(16, eaten);
        string hint = t.Complete ? "Everyone has a home and a fresh meal. Local harvests covered meals since arrival. Stay and enjoy your village." :
            t.Decision == "waiting" ? "Start by working a grove. Your neighbors will harvest and renew it, providing timber for two more cottages while the first crop grows." :
            !t.Arrived ? "Prepare eight beds and food for the arrivals. Work a grove when you need more timber." :
            Housed < 8 ? "Finish the remaining cottages. Neighbors collect timber and build automatically; work a grove if plans need logs." :
            Founding.FirstHome.Served.Count < 8 ? "Local harvests must cover meals since arrival. Add food-growing space if stores keep falling. Each neighbor also needs a fresh meal; Village lists who is waiting." :
            $"Bring home local food: {delivered}/{needed} portions since arrival, and keep eight in reserve. The target grows with meals eaten.";
        if (!t.Complete && People.Any(p => !p.Fed)) hint = "Some neighbors are hungry. Keep local farming going; shortages slow work but are recoverable. " + (t.Decision == "open" ? "The trader can supply provisions while the crop grows." : "");
        return new($"{(t.Complete || Beds >= 8 ? "✓" : "○")} Homes · {Beds}/8 beds" + (t.Arrived ? $" · {Housed}/8 housed" : " for the arrivals"),
            t.Arrived ? $"{(t.Complete || delivered >= needed ? "✓" : "○")} Local food · {delivered} delivered / {eaten} eaten" + (eaten < 16 ? " · minimum 16 delivered" : "") : "○ Local food · assessed after arrival",
            t.Arrived ? $"{(t.Complete || Founding.FirstHome.Served.Count == 8 && ReachableEdibleStored >= 8 ? "✓" : "○")} Fresh meals · {Founding.FirstHome.Served.Count}/8 · reserve {ReachableEdibleStored}/8" : $"{(ReachableEdibleStored >= 8 ? "✓" : "○")} Food reserve · {ReachableEdibleStored}/8 portions",
            hint, t.Complete ? "done" : Housed < 8 ? "home" : "food", null, t.Complete ? 3 : 0);
    }
}

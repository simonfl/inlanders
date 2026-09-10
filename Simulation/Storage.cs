using System;
using System.Collections.Generic;
using System.Linq;

namespace Inlanders.Simulation;

public sealed partial class World
{
    public const int StockpileCapacity = 12;
    private IEnumerable<int?> LogStores => new int?[] { null }.Concat(Cottages.Where(c => c.Kind == BuildingKind.Stockpile && c.Complete && !c.DemolitionRequested).Select(c => (int?)c.Id));
    private Cottage Store(int id) => Cottages.Single(c => c.Id == id && c.Kind == BuildingKind.Stockpile && c.Complete);
    private Cell StorageAccess(int? id) => id is int n ? Store(n).Entrance : YardAccess;
    public int LogsAt(int? id) => id is int n ? Store(n).StoredLogs : _yardLogs;
    public int ReservedLogsAt(int? id) => People.Where(v => v.StorageId == id && v.Cargo == Resource.Logs &&
        v.Task is Work.ToMaterials or Work.ToSawLogs or Work.ToHaulPickup).Sum(v => v.Reserved);
    public int IncomingLogsAt(int? id) => People.Sum(v =>
        v.Task == Work.ToHaulPickup && v.HaulTargetId == id ? v.Reserved :
        v.Cargo == Resource.Logs && v.StorageId == id && v.Task is Work.ToStockpile or Work.ToHaulDrop ? v.Carried : 0);
    public int AvailableLogsAt(int? id) => LogsAt(id) - ReservedLogsAt(id);
    private int FreeLogSpace(int? id) => id == null ? int.MaxValue : StockpileCapacity - LogsAt(id) - IncomingLogsAt(id);
    private void ChangeLogs(int? id, int amount)
    {
        if (id is int n) Store(n).StoredLogs += amount;
        else _yardLogs += amount;
    }
    public bool SetLogTarget(int id, int target)
    {
        var site = Cottages.FirstOrDefault(c => c.Id == id && c.Kind == BuildingKind.Stockpile && c.Complete);
        if (site == null || site.DemolitionRequested || target < 0 || target > StockpileCapacity || Food.Celebrating) return false;
        site.LogTarget = target; _retry = 0; return true;
    }
    private int TravelCost(Cell from, Cell to)
    {
        var path = FindPath(from, to, Blocked);
        return path == null ? int.MaxValue : path.Sum(c => Paths.Contains(c) ? 4 : 5);
    }
    private bool TryLogSource(Cell work, int minimum, out int? source)
    {
        foreach (var id in LogStores.Where(id => AvailableLogsAt(id) >= minimum).OrderBy(id => TravelCost(work, StorageAccess(id))))
        {
            if (TravelCost(work, StorageAccess(id)) == int.MaxValue) continue;
            source = id; return true;
        }
        source = null; return false;
    }
    private void ReturnTimber(Villager v)
    {
        // Retargeting releases this worker's old capacity reservation before choosing a destination.
        v.Task = Work.Waiting; v.StorageId = null; v.HaulTargetId = null;
        if (v.Cargo == Resource.Logs)
            v.StorageId = LogStores.Where(id => (id == null || !Store(id.Value).DemolitionRequested) && FreeLogSpace(id) >= v.Carried)
                .OrderBy(id => TravelCost(At(v), StorageAccess(id))).First();
        Go(v, StorageAccess(v.StorageId), Work.ToStockpile,
            $"Carrying {v.Carried} {v.Cargo} to " + (v.StorageId == null ? "the timber yard" : $"stockpile {v.StorageId}"));
    }
    private int Surplus(int? id) => Math.Max(0, AvailableLogsAt(id) - (id is int n ? Store(n).LogTarget : 0));
    private void StartHaul(Villager v, int? source, int? target, int amount)
    {
        v.StorageId = source; v.HaulTargetId = target; v.Reserved = amount; v.Cargo = Resource.Logs;
        Go(v, StorageAccess(source), Work.ToHaulPickup, $"Collecting {amount} logs for " + (target == null ? "the timber yard" : $"stockpile {target}"));
    }
    private void ClaimHauling(Villager v)
    {
        var depots = Cottages.Where(c => c.Kind == BuildingKind.Stockpile && c.Complete && !c.DemolitionRequested)
            .OrderByDescending(c => c.Priority).ThenBy(c => c.Id).ToArray();
        foreach (var depot in depots)
        {
            int need = Math.Min(FreeLogSpace(depot.Id), depot.LogTarget - AvailableLogsAt(depot.Id) - IncomingLogsAt(depot.Id));
            if (need <= 0) continue;
            foreach (var source in LogStores.Where(id => id != depot.Id && Surplus(id) > 0)
                .OrderBy(id => TravelCost(At(v), StorageAccess(id)) + (long)TravelCost(StorageAccess(id), depot.Entrance)))
            {
                StartHaul(v, source, depot.Id, Math.Min(2, Math.Min(need, Surplus(source)))); return;
            }
        }
        foreach (var depot in depots.Where(c => Surplus(c.Id) > 0).OrderBy(c => TravelCost(At(v), c.Entrance)))
        {
            StartHaul(v, depot.Id, null, Math.Min(2, Surplus(depot.Id))); return;
        }
        v.Status = depots.Length == 0 ? "Needs a finished stockpile" : "Stockpile targets met or waiting for unreserved logs";
    }
    private bool TickHauling(Villager v)
    {
        if (v.Task == Work.ToHaulPickup)
        {
            ChangeLogs(v.StorageId, -v.Reserved); v.Carried = v.Reserved; v.Reserved = 0;
            v.StorageId = v.HaulTargetId; v.HaulTargetId = null;
            Go(v, StorageAccess(v.StorageId), Work.ToHaulDrop, $"Hauling {v.Carried} logs to " + (v.StorageId == null ? "the timber yard" : $"stockpile {v.StorageId}"));
            return true;
        }
        if (v.Task != Work.ToHaulDrop) return false;
        ChangeLogs(v.StorageId, v.Carried); v.Carried = 0; Finish(v); return true;
    }
    private void ValidateStorage()
    {
        void Check(bool ok, string message) { if (!ok) throw new InvalidOperationException(message); }
        foreach (var c in Cottages)
        {
            Check(c.LogTarget is >= 0 and <= StockpileCapacity, "Invalid log target");
            Check(c.Kind == BuildingKind.Stockpile && c.Complete || c.StoredLogs == 0, "Logs stored outside a completed stockpile");
        }
        foreach (var id in LogStores)
        {
            Check(LogsAt(id) >= 0 && AvailableLogsAt(id) >= 0, "Storage over-reserved");
            Check(id == null || LogsAt(id) + IncomingLogsAt(id) <= StockpileCapacity, "Stockpile capacity over-reserved");
        }
        foreach (var v in People)
        {
            Check(v.StorageId == null || Cottages.Any(c => c.Id == v.StorageId && c.Kind == BuildingKind.Stockpile && c.Complete), "Missing storage source/destination");
            Check(v.HaulTargetId == null || Cottages.Any(c => c.Id == v.HaulTargetId && c.Kind == BuildingKind.Stockpile && c.Complete), "Missing hauling target");
            Check(v.Task == Work.ToHaulPickup || v.HaulTargetId == null, "Orphaned hauling destination");
            if (v.Task == Work.ToHaulPickup) Check(v.Reserved is > 0 and <= 2 && v.Carried == 0 && v.Cargo == Resource.Logs && v.StorageId != v.HaulTargetId, "Invalid hauling pickup");
            if (v.Task == Work.ToHaulDrop) Check(v.Reserved == 0 && v.Carried is > 0 and <= 2 && v.Cargo == Resource.Logs, "Invalid hauling delivery");
        }
    }
}

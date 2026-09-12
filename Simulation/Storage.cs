using System;
using System.Collections.Generic;
using System.Linq;

namespace Inlanders.Simulation;

public sealed partial class World
{
    public const int StockpileCapacity = 12;
    private IEnumerable<int?> MaterialStores(Resource material) => new int?[] { null }.Concat(Cottages.Where(c => c.Kind == BuildingKind.Stockpile && c.Complete && !c.DemolitionRequested && c.StorageMaterial==material).Select(c => (int?)c.Id));
    private Cottage Store(int id) => Cottages.Single(c => c.Id == id && c.Kind == BuildingKind.Stockpile && c.Complete);
    private Cell StorageAccess(int? id) => id is int n ? Store(n).Entrance : YardAccess;
    public int MaterialAt(int? id,Resource material) => id is int n
        ? material switch { Resource.Logs=>Store(n).StoredLogs, Resource.Planks=>Store(n).StoredPlanks, Resource.Stone=>Store(n).StoredStone, _=>throw new ArgumentOutOfRangeException(nameof(material)) }
        : material switch { Resource.Logs=>_yardLogs, Resource.Planks=>_yardPlanks, Resource.Stone=>_stone, _=>throw new ArgumentOutOfRangeException(nameof(material)) };
    public int LogsAt(int? id) => MaterialAt(id,Resource.Logs);
    public int ReservedLogsAt(int? id) => ReservedMaterialAt(id,Resource.Logs);
    public int IncomingLogsAt(int? id) => IncomingMaterialAt(id,Resource.Logs);
    public int AvailableLogsAt(int? id) => AvailableMaterialAt(id,Resource.Logs);
    public int ReservedMaterialAt(int? id,Resource material) => People.Where(v => v.StorageId == id && v.Cargo == material &&
        v.Task is Work.ToMaterials or Work.ToSawLogs or Work.ToComfortPlanks or Work.ToHaulPickup).Sum(v => v.Reserved);
    public int IncomingMaterialAt(int? id,Resource material) => People.Where(v=>v.Cargo==material).Sum(v =>
        v.Task == Work.ToHaulPickup && v.HaulTargetId == id ? v.Reserved :
        v.StorageId == id && v.Task is Work.ToStockpile or Work.ToHaulDrop ? v.Carried : 0);
    public int AvailableMaterialAt(int? id,Resource material) => MaterialAt(id,material) - ReservedMaterialAt(id,material);
    private int FreeMaterialSpace(int? id,Resource material) => id == null ? int.MaxValue : StockpileCapacity - MaterialAt(id,material) - IncomingMaterialAt(id,material);
    private void ChangeLogs(int? id,int amount) => ChangeMaterial(id,Resource.Logs,amount);
    private void ChangeMaterial(int? id,Resource material,int amount)
    {
        if(id is int n) { if(material==Resource.Stone) Store(n).StoredStone+=amount; else if(material==Resource.Logs) Store(n).StoredLogs+=amount; else Store(n).StoredPlanks+=amount; }
        else if(material==Resource.Stone) _stone+=amount; else if(material==Resource.Logs) _yardLogs+=amount; else _yardPlanks+=amount;
    }
    public string? StorageMaterialProblem(int id)
    {
        var site=Cottages.FirstOrDefault(c=>c.Id==id && c.Kind==BuildingKind.Stockpile);
        return site==null ? "Choose a stockpile or its construction plan." : site.DemolitionRequested || Food.Celebrating ? "Wait until the current village order finishes." :
            site.StoredLogs+site.StoredPlanks+site.StoredStone>0 || People.Any(p=>p.StorageId==id || p.HaulTargetId==id) ? "Drain the pile and wait for committed trips before changing its material." : null;
    }
    public bool SetStorageMaterial(int id,Resource material)
    {
        if(material is not (Resource.Logs or Resource.Planks or Resource.Stone) || StorageMaterialProblem(id)!=null) return false;
        Cottages.Single(c=>c.Id==id).StorageMaterial=material; _retry=0; return true;
    }
    public bool SetStorageTarget(int id, int target)
    {
        var site = Cottages.FirstOrDefault(c => c.Id == id && c.Kind == BuildingKind.Stockpile && c.Complete);
        if (site == null || site.DemolitionRequested || target < 0 || target > StockpileCapacity || Food.Celebrating) return false;
        site.StorageTarget = target; _retry = 0; return true;
    }
    private int TravelCost(Cell from, Cell to)
    {
        var path = FindPath(from, to, Blocked);
        return path == null ? int.MaxValue : path.Sum(c => Paths.Contains(c) ? 4 : 5);
    }
    private bool TryLogSource(Cell work,int minimum,out int? source) => TryMaterialSource(work,Resource.Logs,minimum,out source);
    private bool TryMaterialSource(Cell work,Resource material,int minimum,out int? source)
    {
        foreach (var id in MaterialStores(material).Where(id => AvailableMaterialAt(id,material) >= minimum).OrderBy(id => TravelCost(work, StorageAccess(id))))
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
            v.StorageId = MaterialStores(v.Cargo).Where(id => FreeMaterialSpace(id,v.Cargo) >= v.Carried)
                .OrderBy(id => TravelCost(At(v), StorageAccess(id))).First();
        Go(v, StorageAccess(v.StorageId), Work.ToStockpile,
            $"Carrying {v.Carried} {v.Cargo} to " + (v.StorageId == null ? "central storage" : $"stockpile {v.StorageId}"));
    }
    private int Surplus(int? id,Resource material) => Math.Max(0, AvailableMaterialAt(id,material) - (id is int n ? Store(n).StorageTarget : 0));
    private void StartHaul(Villager v, int? source, int? target, int amount,Resource material)
    {
        v.StorageId = source; v.HaulTargetId = target; v.Reserved = amount; v.Cargo = material;
        Go(v, StorageAccess(source), Work.ToHaulPickup, $"Collecting {amount} {material} for " + (target == null ? "central storage" : $"stockpile {target}"));
    }
    private void ClaimHauling(Villager v)
    {
        var depots = Cottages.Where(c => c.Kind == BuildingKind.Stockpile && c.Complete && !c.DemolitionRequested)
            .OrderByDescending(c => c.Priority).ThenBy(c => c.Id).ToArray();
        foreach (var depot in depots)
        {
            var material=depot.StorageMaterial;
            int need = Math.Min(FreeMaterialSpace(depot.Id,material), depot.StorageTarget - AvailableMaterialAt(depot.Id,material) - IncomingMaterialAt(depot.Id,material));
            if (need <= 0) continue;
            foreach (var source in MaterialStores(material).Where(id => id != depot.Id && Surplus(id,material) > 0)
                .OrderBy(id => TravelCost(At(v), StorageAccess(id)) + (long)TravelCost(StorageAccess(id), depot.Entrance)))
            {
                StartHaul(v, source, depot.Id, Math.Min(2, Math.Min(need, Surplus(source,material))),material); return;
            }
        }
        foreach (var depot in depots.Where(c => Surplus(c.Id,c.StorageMaterial) > 0).OrderBy(c => TravelCost(At(v), c.Entrance)))
        {
            StartHaul(v, depot.Id, null, Math.Min(2, Surplus(depot.Id,depot.StorageMaterial)),depot.StorageMaterial); return;
        }
        v.Status = depots.Length == 0 ? "Needs a finished stockpile" : "Stockpile targets met or waiting for unreserved materials";
    }
    private bool TickHauling(Villager v)
    {
        if (v.Task == Work.ToHaulPickup)
        {
            ChangeMaterial(v.StorageId,v.Cargo, -v.Reserved); v.Carried = v.Reserved; v.Reserved = 0;
            v.StorageId = v.HaulTargetId; v.HaulTargetId = null;
            Go(v, StorageAccess(v.StorageId), Work.ToHaulDrop, $"Hauling {v.Carried} {v.Cargo} to " + (v.StorageId == null ? "central storage" : $"stockpile {v.StorageId}"));
            return true;
        }
        if (v.Task != Work.ToHaulDrop) return false;
        ChangeMaterial(v.StorageId,v.Cargo, v.Carried); v.Carried = 0; Finish(v); return true;
    }
    private void ValidateStorage()
    {
        void Check(bool ok, string message) { if (!ok) throw new InvalidOperationException(message); }
        foreach (var c in Cottages)
        {
            Check(c.StorageTarget is >= 0 and <= StockpileCapacity, "Invalid stockpile target");
            Check(c.Kind == BuildingKind.Stockpile && c.Complete || c.StoredLogs == 0, "Logs stored outside a completed stockpile");
            Check(c.StorageMaterial is Resource.Logs or Resource.Planks or Resource.Stone && c.StoredLogs>=0 && c.StoredPlanks>=0 && c.StoredStone>=0 &&
                (c.Kind==BuildingKind.Stockpile && c.Complete || c.StoredPlanks+c.StoredStone==0) &&
                (c.StorageMaterial==Resource.Logs || c.StoredLogs==0) && (c.StorageMaterial==Resource.Planks || c.StoredPlanks==0) &&
                (c.StorageMaterial==Resource.Stone || c.StoredStone==0) && c.StoredLogs+c.StoredPlanks+c.StoredStone<=StockpileCapacity,"Invalid stockpile material/inventory");
        }
        foreach(var material in new[]{Resource.Logs,Resource.Planks,Resource.Stone})
        foreach (var id in MaterialStores(material))
        {
            Check(MaterialAt(id,material) >= 0 && AvailableMaterialAt(id,material) >= 0, "Storage over-reserved");
            Check(id == null || MaterialAt(id,material) + IncomingMaterialAt(id,material) <= StockpileCapacity, "Stockpile capacity over-reserved");
        }
        foreach (var v in People)
        {
            Check(v.StorageId == null || Cottages.Any(c => c.Id == v.StorageId && c.Kind == BuildingKind.Stockpile && c.Complete), "Missing storage source/destination");
            Check(v.HaulTargetId == null || Cottages.Any(c => c.Id == v.HaulTargetId && c.Kind == BuildingKind.Stockpile && c.Complete), "Missing hauling target");
            Check(v.Task == Work.ToHaulPickup || v.HaulTargetId == null, "Orphaned hauling destination");
            if(v.StorageId is int store) Check(Store(store).StorageMaterial==v.Cargo,"Wrong material at claimed store");
            if(v.HaulTargetId is int target) Check(Store(target).StorageMaterial==v.Cargo,"Wrong material at hauling destination");
            if (v.Task == Work.ToHaulPickup) Check(v.Reserved is > 0 and <= 2 && v.Carried == 0 && v.Cargo is Resource.Logs or Resource.Planks or Resource.Stone && v.StorageId != v.HaulTargetId, "Invalid hauling pickup");
            if (v.Task == Work.ToHaulDrop) Check(v.Reserved == 0 && v.Carried is > 0 and <= 2 && v.Cargo is Resource.Logs or Resource.Planks or Resource.Stone, "Invalid hauling delivery");
        }
    }
}

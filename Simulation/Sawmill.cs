using System;
using System.Linq;

namespace Inlanders.Simulation;

public sealed partial class World
{
    // Keep enough whole four-plank batches to pre-stock one lodge.
    public static int PlankStockTarget => Math.Max(8, ((Buildings.Get(BuildingKind.Lodge).Cost + 3) / 4) * 4);
    public int Planks { get; private set; }
    public int SawnLogs { get; private set; }
    public int ReservedPlanks => People.Where(v => v.Task == Work.ToMaterials && v.Cargo == Resource.Planks).Sum(v => v.Reserved);
    public int AvailablePlanks => Planks - ReservedPlanks;
    private int PendingPlanks => Planks + People.Where(v => v.Cargo == Resource.Planks).Sum(v => v.Carried) +
        Cottages.Sum(c => c.OutputPlanks + c.InputLogs * 2) +
        People.Where(v => v.Task == Work.ToSawLogs).Sum(v => v.Reserved * 2) +
        People.Where(v => v.Task == Work.ToSawmill).Sum(v => v.Carried * 2);

    private void ClaimSawWork(Villager v)
    {
        var mill = FoodSite(BuildingKind.Sawmill, c => c.OutputPlanks > 0) ?? FoodSite(BuildingKind.Sawmill, c => c.InputLogs > 0);
        if (mill == null && PendingPlanks <= PlankStockTarget - 4 && Available >= 2)
            mill = FoodSite(BuildingKind.Sawmill, c => TryLogSource(c.Entrance, 2, out _));
        if (mill == null)
        {
            v.Status = !Cottages.Any(c => c.Complete && c.Kind == BuildingKind.Sawmill) ? "Needs a finished sawmill" :
                PendingPlanks > PlankStockTarget - 4 ? $"Enough planks ready or on the way (target {PlankStockTarget})" :
                Available < 2 ? "Waiting for 2 unreserved logs" : "Waiting for a free sawmill";
            return;
        }
        v.WorkplaceId = mill.Id;
        if (mill.OutputPlanks > 0) Go(v, mill.Entrance, Work.ToPlanks, "Collecting sawn planks");
        else if (mill.InputLogs > 0) Go(v, mill.Entrance, Work.ToSawmill, "Resuming a sawmill batch");
        else
        {
            TryLogSource(mill.Entrance, 2, out int? source);
            v.Reserved = 2; v.Cargo = Resource.Logs; v.StorageId = source;
            Go(v, StorageAccess(source), Work.ToSawLogs, "Fetching 2 reserved logs for the sawmill");
        }
    }

    private bool TickSawWork(Villager v, float dt)
    {
        if (v.Task is not (Work.ToSawLogs or Work.ToSawmill or Work.Sawing or Work.ToPlanks)) return false;
        var mill = Cottages.Single(c => c.Id == v.WorkplaceId);
        switch (v.Task)
        {
            case Work.ToSawLogs:
                ChangeLogs(v.StorageId, -v.Reserved); v.StorageId = null; v.Carried = v.Reserved; v.Reserved = 0;
                Go(v, mill.Entrance, Work.ToSawmill, "Delivering logs to the sawmill"); break;
            case Work.ToSawmill:
                mill.InputLogs += v.Carried; v.Carried = 0;
                v.Task = Work.Sawing; v.Status = "Sawing 2 logs into 4 planks"; break;
            case Work.Sawing:
                mill.SawProgress = Math.Min(1, mill.SawProgress + dt / 10);
                if (mill.SawProgress < 1) break;
                SawnLogs += mill.InputLogs; mill.OutputPlanks += mill.InputLogs * 2;
                mill.InputLogs = 0; mill.SawProgress = 0; v.Task = Work.ToPlanks; break;
            case Work.ToPlanks:
                v.Carried = Math.Min(2, mill.OutputPlanks); mill.OutputPlanks -= v.Carried; v.Cargo = Resource.Planks;
                ReturnTimber(v); break;
        }
        return true;
    }

    private void ValidateSawmills()
    {
        void Check(bool value, string message) { if (!value) throw new InvalidOperationException(message); }
        Check(Planks >= 0 && AvailablePlanks >= 0 && SawnLogs >= 0, "Invalid plank inventory");
        Check(Planks + People.Where(v => v.Cargo == Resource.Planks).Sum(v => v.Carried) +
            Cottages.Sum(c => c.OutputPlanks) + Cottages.Where(c => c.Material == Resource.Planks).Sum(c => c.Delivered) +
            Trees.Where(t => t.Material == Resource.Planks).Sum(t => t.Logs) == SawnLogs * 2, "Plank conservation failed");
        foreach (var c in Cottages)
            Check(c.InputLogs is >= 0 and <= 2 && c.OutputPlanks is >= 0 and <= 4 && float.IsFinite(c.SawProgress) && c.SawProgress >= 0 && c.SawProgress < 1 &&
                (c.Kind == BuildingKind.Sawmill || (c.InputLogs == 0 && c.OutputPlanks == 0 && c.SawProgress == 0)), "Invalid sawmill buffer");
        foreach (var t in Trees) Check(t.Material == Resource.Logs || (t.Material == Resource.Planks && t.Salvage && t.Felled), "Invalid salvage material");
        foreach (var v in People)
        {
            if (v.Task is Work.ToSawLogs or Work.ToSawmill or Work.Sawing or Work.ToPlanks)
                Check(Cottages.Any(c => c.Id == v.WorkplaceId && c.Kind == BuildingKind.Sawmill && c.Complete), "Missing sawmill workplace");
            if (v.Task == Work.ToSawLogs) Check(v.Reserved == 2 && v.Cargo == Resource.Logs, "Invalid sawmill log claim");
        }
    }
}

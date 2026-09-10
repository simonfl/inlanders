using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Inlanders.Simulation;

public sealed class FishHabitat
{
    public int Id { get; init; }
    public string Name { get; init; } = "Fishing ground";
    public Cell Cell { get; init; }
    public int Capacity { get; init; } = 16;
    public float Stock { get; set; } = 16;
    public float RegrowthPerSecond { get; init; } = 1f/15;
    public int Available => (int)MathF.Floor(Stock);
    public void Advance(float seconds)
    {
        if(seconds>0 && float.IsFinite(seconds)) Stock=Math.Min(Capacity,Stock+seconds*RegrowthPerSecond);
    }
    public int Take(int wanted)
    {
        int caught=Math.Min(Math.Max(0,wanted),Available); Stock-=caught; return caught;
    }
}

public sealed partial class MapLayout
{
    public List<FishHabitat> FishingGrounds { get; set; } = new();
    internal void ValidateFishingGrounds()
    {
        if(FishingGrounds==null || FishingGrounds.Any(h=>h==null || h.Id<0 || string.IsNullOrWhiteSpace(h.Name) ||
            !Water.Contains(h.Cell) || h.Capacity<=0 || !float.IsFinite(h.Stock) || h.Stock<0 || h.Stock>h.Capacity ||
            !float.IsFinite(h.RegrowthPerSecond) || h.RegrowthPerSecond<=0) ||
            FishingGrounds.Select(h=>h.Id).Distinct().Count()!=FishingGrounds.Count || FishingGrounds.Select(h=>h.Cell).Distinct().Count()!=FishingGrounds.Count)
            throw new InvalidDataException("Invalid fishing habitat");
    }
}

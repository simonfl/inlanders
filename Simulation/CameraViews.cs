using System;

namespace Inlanders.Simulation;

public sealed record CameraView(string Name, float X, float Z, float Zoom, float Angle);
public sealed partial class World
{
    public CameraView?[] CameraViews { get; private set; } = new CameraView?[3];
    public float MaximumViewZoom => Math.Max(32, (Map.Width + Map.Depth) * .95f);
    private bool ValidView(CameraView view) => !string.IsNullOrWhiteSpace(view.Name) && view.Name.Length <= 24 &&
        float.IsFinite(view.X) && float.IsFinite(view.Z) && float.IsFinite(view.Zoom) && float.IsFinite(view.Angle) &&
        view.X >= Map.MinX && view.X <= Map.MaxX && view.Z >= Map.MinZ && view.Z <= Map.MaxZ &&
        view.Zoom >= 12 && view.Zoom <= MaximumViewZoom && view.Angle >= 0 && view.Angle < MathF.Tau;
    public bool SaveCameraView(int slot, CameraView view)
    {
        if (slot < 0 || slot >= 3 || view == null || !ValidView(view)) return false;
        CameraViews[slot] = view; return true;
    }
    public bool ClearCameraView(int slot)
    {
        if (slot < 0 || slot >= 3 || CameraViews[slot] == null) return false;
        CameraViews[slot] = null; return true;
    }
    private void ValidateCameraViews()
    {
        if (CameraViews.Length != 3) throw new InvalidOperationException("Invalid camera view slots");
        foreach (var view in CameraViews) if (view != null && !ValidView(view)) throw new InvalidOperationException("Invalid saved camera view");
    }
}

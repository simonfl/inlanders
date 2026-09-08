using Godot;
using Inlanders.Simulation;
using System.Linq;

public partial class Game
{
    private bool _clearingTrees;
    private Button _clearTreeButton = null!;
    private void ToggleClearing()
    {
        _placing = !(_placing && _clearingTrees); _clearingTrees = true; _plantingTrees = false;
        ClearSelection(); RefreshGhost();
    }
    private string ClearingHint()
    {
        var tree = _world.Trees.FirstOrDefault(t => t.Cell == _hover);
        if (tree?.ClearRequested == true) return "Click to cancel this order · cut timber stays cut";
        return _world.People.Any(p => p.Role == Role.Logger) ? "Click to mark · loggers recover timber, then remove roots" : "Click to mark · assign loggers in People [V] to do the work";
    }
    private void ClearingCross(Node3D parent, Vector3 at, Color color, float size)
    {
        foreach (float angle in new[] { -45f, 45f })
        {
            var bar = Box(parent, at, new(size, 0.07f, 0.09f), color); bar.RotationDegrees = new(0, angle, 0);
        }
    }
    private void RefreshClearingGhost()
    {
        Clear(_ghostModel); _previewMaterials.Clear(); _ghostModelKey = "clearing";
        Clear(_ghostCells);
        ClearingCross(_ghostCells, new(_hover.X, 0.16f, _hover.Z), _ghostValid ? new("f0bd70") : new("e38673"), 1.15f);
    }
    private void MarkClearing(Cell at)
    {
        var tree = _world.Trees.FirstOrDefault(t => t.Cell == at);
        bool requested = tree?.ClearRequested != true;
        if (_world.SetClearing(at, requested)) UiCue(requested ? Cue.Place : Cue.Click);
        else UiCue(Cue.Reject);
        RefreshGhost();
    }
}

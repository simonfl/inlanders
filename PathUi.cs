using Godot;
using Inlanders.Simulation;
using System.Linq;

public partial class Game
{
    private int _pathTool;
    private bool _pathStroke;
    private Cell? _lastPathCell;
    private Node3D _pathView = null!;
    private World? _pathWorld;
    private int _pathRevision = -1;
    private void TogglePaths(int tool)
    {
        _placing = !(_placing && _pathTool == tool); _pathTool = tool; _decorating = false;
        _clearingTrees = _plantingTrees = false; _pathStroke = false; _lastPathCell = null;
        ClearSelection(); RefreshGhost();
    }
    private void PaintPath(Cell cell)
    {
        if (_lastPathCell == cell) return;
        bool applied = false;
        if (_lastPathCell is Cell previous)
        {
            int steps = System.Math.Max(System.Math.Abs(cell.X - previous.X), System.Math.Abs(cell.Z - previous.Z));
            var cursor = previous;
            for (int i = 1; i <= steps; i++)
            {
                var next = new Cell(previous.X + (int)System.Math.Round((cell.X - previous.X) * i / (double)steps), previous.Z + (int)System.Math.Round((cell.Z - previous.Z) * i / (double)steps));
                if (cursor.X != next.X && cursor.Z != next.Z) applied |= _world.SetPath(new(next.X, cursor.Z), _pathTool == 1);
                applied |= _world.SetPath(next, _pathTool == 1); cursor = next;
            }
        }
        else applied = _world.SetPath(cell, _pathTool == 1);
        _lastPathCell = cell;
        if (applied) UiCue(Cue.Click);
        else UiCue(Cue.Reject);
        RefreshGhost();
    }
    private void RenderPaths()
    {
        if (_pathView == null) { _pathView = new(); AddChild(_pathView); }
        if (_pathWorld == _world && _pathRevision == _world.PathsRevision) return;
        Clear(_pathView); _pathWorld = _world; _pathRevision = _world.PathsRevision;
        foreach (var cell in _world.Paths)
        {
            GroundPatch(_pathView,cell.X,cell.Z,.66f,.66f,new("b8a17b"));
            foreach (var offset in new[] { new Cell(1, 0), new(-1, 0), new(0, 1), new(0, -1) })
                if (_world.Paths.Contains(new(cell.X + offset.X, cell.Z + offset.Z)))
                    GroundPatch(_pathView,cell.X+offset.X*.4f,cell.Z+offset.Z*.4f,offset.X != 0 ? .34f : .66f,offset.Z != 0 ? .34f : .66f,new("b8a17b"));
        }
    }
    private void RefreshPathGhost()
    {
        Clear(_ghostModel); _previewMaterials.Clear(); _ghostModelKey = "path"; Clear(_ghostCells);
        GroundPatch(_ghostCells,_hover.X,_hover.Z,.72f,.72f,_ghostValid ? new("e2c795") : new("e38673"),.09f);
    }
}

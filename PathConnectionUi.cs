using Godot;
using Inlanders.Simulation;
using System.Collections.Generic;
using System.Linq;

public partial class Game
{
    private Button _connectPathsButton = null!;
    private Cell? _pathAnchor;
    private World? _connectionWorld;
    private Cell _connectionEnd;
    private Cell? _connectionStart;
    private ulong _connectionTime;
    private string? _connectionProblem;
    private List<Cell> _connectionRoute = new();
    private Cell PathEndpoint(Cell cell)
    {
        var site = _world.Cottages.FirstOrDefault(c => c.Complete && World.Footprint(c.Cell, c.Rotation, c.Kind).Contains(cell));
        return site?.Entrance ?? cell;
    }
    private string? ConnectionProblem(Cell cell)
    {
        var end = PathEndpoint(cell);
        ulong now = Time.GetTicksMsec();
        if (_connectionWorld == _world && _connectionEnd == end && _connectionStart == _pathAnchor && now - _connectionTime < 250) return _connectionProblem;
        _connectionWorld = _world; _connectionEnd = end; _connectionStart = _pathAnchor; _connectionTime = now;
        _connectionRoute.Clear();
        if (_pathAnchor is Cell start) _connectionProblem = _world.PathConnection(start, end, out _connectionRoute);
        else { _connectionProblem = _world.PathProblem(end); _connectionRoute.Add(end); }
        return _connectionProblem;
    }
    private void ClickPathConnection(Cell cell)
    {
        var end = PathEndpoint(cell);
        if (_pathAnchor is not Cell start)
        {
            if (_world.PathProblem(end) != null) { UiCue(Cue.Reject); return; }
            _pathAnchor = end; UiCue(Cue.Click);
        }
        else if (_world.ConnectPaths(start, end)) { _pathAnchor = null; UiCue(Cue.Click); }
        else UiCue(Cue.Reject);
        _connectionWorld = null; RefreshGhost();
    }
    private void DrawPathConnection()
    {
        ConnectionProblem(_hover);
        foreach (var cell in _connectionRoute)
            GroundPatch(_ghostCells,cell.X,cell.Z,.72f,.72f,new("8fd3d1"),.09f);
        if (_pathAnchor is Cell start) GroundPatch(_ghostCells,start.X,start.Z,.86f,.86f,new("a4caa0"),.1f);
        if (!_ghostValid) { var end=PathEndpoint(_hover); GroundPatch(_ghostCells,end.X,end.Z,.72f,.72f,new("e38673"),.09f); }
    }
}

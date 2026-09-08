using Godot;
using Inlanders.Simulation;
using System.Collections.Generic;
using System.Linq;

public partial class Game
{
    private Node3D _ghostModel = null!, _ghostCells = null!;
    private string _ghostModelKey = "", _placementProblem = "";
    private readonly List<StandardMaterial3D> _previewMaterials = new();
    private Label _buildDescription = null!;
    private Vector2 _pointerPosition;
    public override void _Input(InputEvent input)
    {
        if (input is InputEventMouse mouse) _pointerPosition = mouse.Position;
        if (input is InputEventMouseMotion && PointerOverHud(_pointerPosition)) _lastPathCell = null;
        if (input is InputEventMouseButton button && button.ButtonIndex == MouseButton.Left && !button.Pressed) { _pathStroke = false; _lastPathCell = null; }
        if (input is InputEventMouseMotion && _pathStroke && _placing && _pathTool > 0 && !PointerOverHud(_pointerPosition) && Ground(_pointerPosition) is Vector3 p)
            PaintPath(new(Mathf.RoundToInt(p.X), Mathf.RoundToInt(p.Z)));
    }
    private static string BuildingDescription(BuildingKind kind) => kind switch
    {
        BuildingKind.Cottage => "A home for 2 neighbors. No staff needed.",
        BuildingKind.Lodge => "A home for 4 neighbors. Needs planks made at a sawmill. No staff needed.",
        BuildingKind.ForagerHut => "Supports 2 foragers who gather berries from nearby bushes and bring them to storage.",
        BuildingKind.Farm => "Supports 1 farmer. Crops grow for 45 seconds, yielding 6 grain. Grain must be baked to feed villagers.",
        BuildingKind.Bakery => "Supports 1 baker. Turns 2 grain into 4 loaves in 10 work seconds. Needs a grain supply.",
        BuildingKind.Sawmill => "Supports 1 sawyer. Turns 2 logs into 4 planks in 10 work seconds. Aims for 8 planks in stock.",
        _ => ""
    };
    private string PlacementProblem(Cell cell) => (_pathTool > 0 ? _world.PathProblem(cell, _pathTool == 2) : _clearingTrees ? _world.ClearingProblem(cell) : _plantingTrees ? _world.PlantingProblem(cell) : _world.PlacementProblem(cell, _rotated)) ?? "";
    private bool PointerOverHud(Vector2 point) =>
        _topBar.GetGlobalRect().HasPoint(point) || _bottomBar.GetGlobalRect().HasPoint(point) ||
        (_drawer.Visible && _drawer.GetGlobalRect().HasPoint(point)) || (_inspector.Visible && _inspector.GetGlobalRect().HasPoint(point));

    private void RefreshGhost()
    {
        if (_ghostCells == null)
        {
            _ghostCells = new(); _ghost.AddChild(_ghostCells); _ghostModel = new(); _ghost.AddChild(_ghostModel);
        }
        _ghost.Visible = _placing && !PointerOverHud(_pointerPosition);
        if (!_placing) return;
        _placementProblem = PlacementProblem(_hover); _ghostValid = _placementProblem.Length == 0;
        if (_pathTool > 0) { RefreshPathGhost(); return; }
        if (_clearingTrees) { RefreshClearingGhost(); return; }
        var tint = _ghostValid ? new Color("a4caa0") : new Color("e38673");
        string key = _plantingTrees ? "tree" : _buildKind.ToString();
        if (_ghostModelKey != key)
        {
            Clear(_ghostModel); _previewMaterials.Clear(); _ghostModelKey = key;
            if (_plantingTrees) MakeTree(Vector3.Zero, 0.4f, new("8cad69")).Reparent(_ghostModel, false);
            else MakeBuilding(_ghostModel, new Cottage { Kind = _buildKind }, 3);
            PreparePreview(_ghostModel);
        }
        foreach (var material in _previewMaterials) material.AlbedoColor = new(tint.R, tint.G, tint.B, 0.42f);
        _ghostModel.Position = new(_hover.X + (!_plantingTrees && _rotated ? -0.5f : 0), 0.1f, _hover.Z + (!_plantingTrees && !_rotated ? -0.5f : 0));
        _ghostModel.RotationDegrees = new(0, !_plantingTrees && _rotated ? 90 : 0, 0);
        Clear(_ghostCells);
        var footprint = _plantingTrees ? new[] { _hover } : World.Footprint(_hover, _rotated);
        foreach (var cell in footprint) Box(_ghostCells, new(cell.X, 0.045f, cell.Z), new(0.94f, 0.05f, 0.94f), tint.Darkened(0.15f));
        var door = _plantingTrees ? new Cell(_hover.X + 1, _hover.Z) : World.Door(_hover, _rotated);
        var marker = new Node3D { Position = new(door.X, 0.10f, door.Z), RotationDegrees = new(0, _plantingTrees || _rotated ? 90 : 0, 0) }; _ghostCells.AddChild(marker);
        Box(marker, Vector3.Zero, new(0.11f, 0.06f, 0.5f), _cream);
        foreach (float side in new[] { -1f, 1f })
        {
            var arm = Box(marker, new(side * 0.09f, 0, -0.15f), new(0.1f, 0.06f, 0.28f), _cream); arm.RotationDegrees = new(0, side * 45, 0);
        }
        marker.AddChild(new Label3D { Text = _plantingTrees ? "ACCESS" : "ENTRANCE", Position = new(0, 0.32f, 0), FontSize = 32, PixelSize = 0.01f,
            Billboard = BaseMaterial3D.BillboardModeEnum.Enabled, Modulate = _cream, OutlineSize = 4 });
    }
    private void PreparePreview(Node root)
    {
        foreach (var child in root.GetChildren())
        {
            if (child is Label3D label) { label.Hide(); continue; }
            if (child is MeshInstance3D mesh)
            {
                var material = new StandardMaterial3D { Transparency = BaseMaterial3D.TransparencyEnum.Alpha, ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded };
                mesh.MaterialOverride = material; mesh.CastShadow = GeometryInstance3D.ShadowCastingSetting.Off; _previewMaterials.Add(material);
            }
            PreparePreview(child);
        }
    }
    private void UpdateBuildDescription()
    {
        if (_pathTool > 0 && _placing) { _buildDescription.Text = "PATHS\nClick or drag on clear land to paint/remove paths for free. Villagers choose quicker routes and move 25% faster toward path tiles. Building or planting replaces paths beneath it."; return; }
        if (_clearingTrees && _placing)
        {
            _buildDescription.Text = "CLEAR TREES & STUMPS\nLoggers prioritize marked trees, recover existing timber, then remove roots. Land becomes usable when the roots are gone. Saplings yield no timber. Click a marked tree again to cancel.\n\n" +
                $"{_world.Trees.Count(t => t.ClearRequested)} clearing orders · {_world.People.Count(p => p.Role == Role.Logger)} loggers";
            return;
        }
        int available = _buildKind == BuildingKind.Lodge ? _world.AvailablePlanks : _world.Available;
        int cost = _buildKind == BuildingKind.Lodge ? 8 : World.Cost;
        string material = _buildKind == BuildingKind.Lodge ? "planks" : "logs";
        _buildDescription.Text = _plantingTrees && _placing ? "ALDERS\nLoggers plant for free. Grow for 3 days; yield 8 logs. Replant exhausted stumps." :
            $"{BuildingName(_buildKind).ToUpperInvariant()}\n{BuildingDescription(_buildKind)}\n\n{available} {material} available · {cost} needed" + (available < cost ? "\nYou can plan now; builders wait for materials." : "");
    }
}

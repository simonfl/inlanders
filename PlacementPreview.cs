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
        if (_atMainMenu) return;
        if (input is InputEventMouse mouse) _pointerPosition = mouse.Position;
        if (input is InputEventMouseMotion && PointerOverHud(_pointerPosition)) _lastPathCell = null;
        if (input is InputEventMouseButton button && button.ButtonIndex == MouseButton.Left && !button.Pressed) { _pathStroke = false; _lastPathCell = null; }
        if (input is InputEventMouseMotion && _pathStroke && _placing && _pathTool > 0 && !PointerOverHud(_pointerPosition) && Ground(_pointerPosition) is Vector3 p)
            PaintPath(new(Mathf.RoundToInt(p.X), Mathf.RoundToInt(p.Z)));
    }
    private string BuildingDescription(BuildingKind kind) => (_world.Creative, kind) switch
    {
        (true, BuildingKind.Bridge) => "Instant crossing between two dry banks. R turns the crossing. Keep both banks accessible.",
        (true, BuildingKind.Lodge) => "A home for 4 neighbors. No staff needed.",
        (true, BuildingKind.Stockpile) => "Stores up to 12 real logs. Loggers drop timber nearby; haulers balance its target.",
        (true, BuildingKind.Farm) => "Supports 1 farmer. Crops grow for 45 seconds, yielding 6 grain for bakers. Food needs are disabled.",
        (true, BuildingKind.VegetableGarden) => "Supports 1 farmer. Crops grow for 60 seconds, yielding 8 vegetables. Food needs are disabled.",
        (true, BuildingKind.Square) => "Up to four villagers take short breaks between jobs. No staff. Leave open space nearby.",
        _ => OrdinaryBuildingDescription(kind)
    };
    private static string OrdinaryBuildingDescription(BuildingKind kind) => kind switch
    {
        BuildingKind.FishingDock => "One fisher and boat. Needs dry shore, a clear water launch and reachable fishing grounds. Shared fish stocks replenish over time; catches must return to the pantry.",
        BuildingKind.Stockpile => "Stores up to 12 logs. Place between woodland and timber work; local logger deposits need no hauler. Optional haulers move existing stocks to its target. Food and planks stay at the main yard.",
        BuildingKind.Bridge => "Crosses one water tile between dry banks. Builders work at the marked bank; opens only when complete. R turns the crossing.",
        BuildingKind.Square => "Up to four villagers take short breaks here between jobs. Also hosts village supper. No staff. Leave one walkable tile per villager within four tiles of the entrance.",
        BuildingKind.Cottage => "A home for 2 neighbors. No staff needed.",
        BuildingKind.Lodge => "A home for 4 neighbors. Needs planks made at a sawmill. No staff needed.",
        BuildingKind.ForagerHut => "Supports 2 foragers who gather berries from nearby bushes and bring them to storage.",
        BuildingKind.VegetableGarden => "Supports 1 farmer. Grows 8 vegetables in 60 seconds after planting; harvested in pairs and carried to the pantry. Eaten directly without a bakery. Farmers share gardens and grain farms.",
        BuildingKind.Farm => "Supports 1 farmer. Grows 6 grain in 45 seconds; harvest loads hold up to 4. Grain needs a bakery before villagers can eat it.",
        BuildingKind.Bakery => "Supports 1 baker. Bakes 2 grain into 4 loaves in 10 work seconds; carries the whole batch. Build near the pantry to shorten trips.",
        BuildingKind.Sawmill => $"Supports 1 sawyer. Turns 2 logs into 4 planks in 10 work seconds. Starts with an adjustable {World.PlankStockTarget}-plank stock target.",
        _ => ""
    };
    private string PlacementProblem(Cell cell) => (_decorating ? _world.DecorationProblem(cell, _decorationKind, _removeDecoration) : _pathTool > 0 ? _world.PathProblem(cell, _pathTool == 2) : _clearingTrees ? _world.ClearingProblem(cell) : _plantingTrees ? _world.PlantingProblem(cell) : _world.PlacementProblem(cell, _rotated, _buildKind)) ?? "";
    private bool PointerOverHud(Vector2 point) => _watching ? _watchBar.GetGlobalRect().HasPoint(point) :
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
        if (_decorating) { RefreshDecorationGhost(); return; }
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
        bool compact = _buildKind is BuildingKind.Bridge or BuildingKind.FishingDock;
        bool dockFar = !_plantingTrees && _buildKind == BuildingKind.FishingDock && _world.DockEntrance(_hover,_rotated)==World.FarBank(_hover,_rotated);
        _ghostModel.Position = OnGround(_hover.X + (!_plantingTrees && !compact && _rotated ? -0.5f : 0), _hover.Z + (!_plantingTrees && !compact && !_rotated ? -0.5f : 0), .1f);
        _ghostModel.RotationDegrees = new(0, (!_plantingTrees && _rotated ? 90 : 0)+(dockFar?180:0), 0);
        Clear(_ghostCells);
        var footprint = _plantingTrees ? new[] { _hover } : World.Footprint(_hover, _rotated, _buildKind);
        foreach (var cell in footprint) GroundPatch(_ghostCells,cell.X,cell.Z,.94f,.94f,tint.Darkened(.15f),.06f);
        var door = _plantingTrees ? new Cell(_hover.X + 1, _hover.Z) : _buildKind == BuildingKind.FishingDock ? _world.DockEntrance(_hover,_rotated) : _buildKind == BuildingKind.Bridge ? _world.BridgeEntrance(_hover, _rotated) : World.Door(_hover, _rotated);
        var marker = new Node3D { Position = OnGround(door.X,door.Z,.10f), RotationDegrees = new(0, (_plantingTrees || _rotated ? 90 : 0) + (dockFar || !_plantingTrees && _buildKind == BuildingKind.Bridge && door == World.FarBank(_hover, _rotated) ? 180 : 0), 0) }; _ghostCells.AddChild(marker);
        if(!_plantingTrees && _buildKind==BuildingKind.FishingDock)
        {
            var launch=_world.DockLaunch(_hover,_rotated);
            GroundPatch(_ghostCells,launch.X,launch.Z,.88f,.88f,tint,.06f);
            var sign=new Node3D { Position=OnGround(launch.X,launch.Z,.2f) }; _ghostCells.AddChild(sign); FoodSign(sign,"LAUNCH",.25f);
        }
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
        if (_decorating && _placing) { _buildDescription.Text = DecorationDescription; return; }
        if (_pathTool > 0 && _placing) { _buildDescription.Text = "PATHS\nClick or drag on clear land to paint/remove paths for free. Villagers choose quicker routes and move 25% faster toward path tiles. Building or planting replaces paths beneath it."; return; }
        if (_clearingTrees && _placing)
        {
            if (_world.Creative) { _buildDescription.Text = "CLEAR TREES & STUMPS\nClick to clear immediately. Existing timber returns to the yard; saplings yield no timber."; return; }
            _buildDescription.Text = "CLEAR TREES & STUMPS\nLoggers prioritize marked trees, recover existing timber, then remove roots. Land becomes usable when the roots are gone. Saplings yield no timber. Click a marked tree again to cancel.\n\n" +
                $"{_world.Trees.Count(t => t.ClearRequested)} clearing orders · {_world.People.Count(p => p.Role == Role.Logger)} loggers";
            return;
        }
        if (_world.Creative && !_plantingTrees) { _buildDescription.Text = $"{BuildingName(_buildKind).ToUpperInvariant()}\n{BuildingDescription(_buildKind)}\n\nInstant · Free. Choose level ground for the footprint and entrance."; return; }
        var definition = Buildings.Get(_buildKind);
        int available = definition.Material == Inlanders.Simulation.Resource.Planks ? _world.AvailablePlanks : _world.Available;
        int cost = definition.Cost;
        string material = definition.Material.ToString().ToLowerInvariant();
        _buildDescription.Text = _plantingTrees && _placing ? "ALDERS\nLoggers plant for free. Grow for 3 days; yield 8 logs. Replant exhausted stumps." :
            $"{BuildingName(_buildKind).ToUpperInvariant()}\n{BuildingDescription(_buildKind)}\n\nBuildings need level ground, including the entrance.\n{available} {material} available · {cost} needed" + (available < cost ? "\nYou can plan now; builders wait for materials." : "");
        if(!_plantingTrees && _placing && _buildKind==BuildingKind.FishingDock) _buildDescription.Text+="\n\n"+_world.FishingSurvey(_hover,_rotated);
    }
}

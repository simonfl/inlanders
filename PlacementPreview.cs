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
        if (_atMainMenu) { HandleMainMenuKey(input);return; }
        if(HandleTerrainInput(input)){GetViewport().SetInputAsHandled();return;}
        if(HandleBushMoveInput(input)){GetViewport().SetInputAsHandled();return;}
        if(HandleAreaRemovalInput(input)){GetViewport().SetInputAsHandled();return;}
        if(HandleRelocationInput(input)){GetViewport().SetInputAsHandled();return;}
        if(HandleSurveyKeyboard(input)) { GetViewport().SetInputAsHandled();return; }
        if(HandleGoalsKeyboard(input)) { GetViewport().SetInputAsHandled();return; }
        if(HandleEconomyKeyboard(input)) { GetViewport().SetInputAsHandled();return; }
        if(HandlePeopleKeyboard(input)) { GetViewport().SetInputAsHandled();return; }
        if(HandleCatalogKeyboard(input)) { GetViewport().SetInputAsHandled();return; }
        // Handle Tab before GUI focus traversal consumes it after clicking Watch controls.
        if(_watching && input is InputEventKey { Pressed:true, Echo:false, Keycode:Key.Tab })
        { ToggleCleanWatch(); GetViewport().SetInputAsHandled(); return; }
        if (input is InputEventMouse mouse) _pointerPosition = mouse.Position;
        HandleDecorationStroke(input);
        if (HandleCameraDrag(input)) { GetViewport().SetInputAsHandled(); return; }
        if (input is InputEventMouseMotion && PointerOverHud(_pointerPosition)) _lastPathCell = null;
        if(input is InputEventMouseMotion && PointerOverHud(_pointerPosition)) _lastWoodlandCell=null;
        if(input is InputEventMouseButton released && released.ButtonIndex==MouseButton.Left && !released.Pressed) { _woodlandStroke=false; _lastWoodlandCell=null; }
        if(input is InputEventMouseMotion && _woodlandStroke && _placing && _woodlandTool>0 && !PointerOverHud(_pointerPosition) && Ground(_pointerPosition) is Vector3 grovePoint)
            PaintWoodland(new(Mathf.RoundToInt(grovePoint.X),Mathf.RoundToInt(grovePoint.Z)));
        if (input is InputEventMouseButton button && button.ButtonIndex == MouseButton.Left && !button.Pressed) { _pathStroke = false; _lastPathCell = null; }
        if (input is InputEventMouseMotion && _pathStroke && _placing && _pathTool > 0 && !PointerOverHud(_pointerPosition) && Ground(_pointerPosition) is Vector3 p)
            PaintPath(new(Mathf.RoundToInt(p.X), Mathf.RoundToInt(p.Z)));
    }
    private string BuildingDescription(BuildingKind kind) => (_world.Creative, kind) switch
    {
        (_,BuildingKind.Carpenter) => "One carpenter installs improvements in occupied homes. Order at a home: 4 planks per cottage, 8 per lodge. Actual rest lasts longer; beds stay available during work.",
        (true, BuildingKind.Bridge) => "Instant crossing between two dry banks. R turns the crossing. Keep both banks accessible.",
        (true, BuildingKind.Lodge) => "A home for 4 neighbors. No staff needed.",
        (true, BuildingKind.Stockpile) => "Stores up to 12 real logs. Loggers drop timber nearby; haulers balance its target.",
        (true, BuildingKind.Farm) => "Supports 1 farmer. Crops grow for 45 seconds, yielding 6 grain for bakers. Food needs are disabled.",
        (true, BuildingKind.VegetableGarden) => "Supports 1 farmer. Crops grow for 60 seconds, yielding 8 vegetables. Food needs are disabled.",
        (true, BuildingKind.Square) => "Up to four villagers take short breaks between jobs. No staff. Visitors stand on reachable ground within 2 tiles of the marked entrance; keep that frontage open.",
        _ => OrdinaryBuildingDescription(kind)
    };
    private static string OrdinaryBuildingDescription(BuildingKind kind) => kind switch
    {
        BuildingKind.HuntingLodge => "One hunter brings up to 2 game to the pantry after 10 work seconds and travel. Needs wooded habitat within 8 tiles. Retain mature trees; nearby lodges share stock. Pause hunting for recovery or supplement it with gardens.",
        BuildingKind.Quarry => "One quarrier extracts 2 stone in 6 work seconds, then carries it to the nearest stone pile with room or central storage. Needs a reachable outcrop within 4 tiles. Deposits are finite and shared by nearby camps; pause or set a stock target to reserve stone for later.",
        BuildingKind.GatheringHall => "A stone-and-plank recreation venue for up to 8 visitors, in the same footprint as a square. No staff. Visitors gather on reachable ground within 2 tiles of the entrance; keep that frontage open. Twelve-second visits satisfy recreation for 4 minutes, with a 2-minute interval before returning. Squares are cheaper and can be spread near homes.",
        BuildingKind.Pantry => "Store up to 24 edible portions near homes or work. Producers choose nearby space; optional haulers replenish a target from central food. Residents collect and eat actual food. Leave open meal seating near the entrance. Grain stays central.",
        BuildingKind.SeatingGarden => "A one-tile planted meeting spot for 2 visitors. No staff. Leave open walkable space near its entrance for residents to sit. Six-second visits give 2 minutes of recreation, like a square. Fits small plots; squares serve twice as many visitors for 6 logs.",
        BuildingKind.FishingDock => "One fisher and boat. Needs dry shore, a clear water launch and reachable fishing grounds. Shared fish stocks replenish over time; catches must return to the pantry.",
        BuildingKind.Stockpile => "Stores 12 logs, planks or stone; choose its material when empty. Producers deposit locally and builders collect here without haulers. Optional haulers balance targets. Food stays at the pantry.",
        BuildingKind.Bridge => "Crosses one water tile between dry banks. Builders work at the marked bank; opens only when complete. R turns the crossing.",
        BuildingKind.Square => "Up to four villagers take short breaks on reachable ground within 2 tiles of the marked entrance. Keep that frontage open. Supper needs one reachable tile per villager within four tiles of the entrance. No staff; the table is a serving place, not assigned seating.",
        BuildingKind.Cottage => "A home for 2 neighbors. No staff needed.",
        BuildingKind.Lodge => "A home for 4 neighbors. Needs planks made at a sawmill. No staff needed.",
        BuildingKind.ForagerHut => "Supports 2 foragers who gather berries from nearby bushes and bring them to storage.",
        BuildingKind.VegetableGarden => "Supports 1 farmer. Grows 8 vegetables in 60 seconds after planting; harvested in pairs and carried to the pantry. Eaten directly without a bakery. Farmers share gardens and grain farms.",
        BuildingKind.Farm => "Supports 1 farmer. Grows 6 grain in 45 seconds; harvest loads hold up to 4. Grain needs a bakery before villagers can eat it.",
        BuildingKind.Orchard => "1 farmer plants trees once. First fruit takes 3 minutes; mature trees grow 8 fruit every 60 seconds after picking. Farmers carry pairs to food storage. Keep quick food during establishment. Targets hold new batches; clearing loses mature trees.",
        BuildingKind.Bakery => "Supports 1 baker. Bakes 2 grain into 4 loaves in 10 work seconds; carries the whole batch. Build near the pantry to shorten trips.",
        BuildingKind.Sawmill => $"Supports 1 sawyer. Turns 2 logs into 4 planks in 10 work seconds. Starts with an adjustable {World.PlankStockTarget}-plank stock target.",
        _ => ""
    };
    private string PlacementProblem(Cell cell) => (_movingSite>=0?MovePreviewProblem(cell):_woodlandTool>0 ? WoodlandProblem(cell) : _decorating ? _world.DecorationProblem(cell, _decorationKind, _removeDecoration) : _pathTool > 0 ? _world.PathProblem(cell, _pathTool == 2) : _clearingTrees ? _world.ClearingProblem(cell) : _plantingTrees ? _world.PlantingProblem(cell) : _world.PlacementProblem(cell, _rotation, _buildKind)) ?? "";
    private bool PointerOverHud(Vector2 point) => _watching ? (_watchBar.Visible && _watchBar.GetGlobalRect().HasPoint(point)) :
        _topBar.GetGlobalRect().HasPoint(point) || _bottomBar.GetGlobalRect().HasPoint(point) ||
        (_areaPanel!=null && _areaPanel.Visible && _areaPanel.GetGlobalRect().HasPoint(point)) ||
        (_terrainPanel!=null && _terrainPanel.Visible && _terrainPanel.GetGlobalRect().HasPoint(point)) ||
        (_bushMovePanel!=null && _bushMovePanel.Visible && _bushMovePanel.GetGlobalRect().HasPoint(point)) ||
        (_trackedGoalPanel!=null && _trackedGoalPanel.Visible && _trackedGoalPanel.GetGlobalRect().HasPoint(point)) ||
        (_drawer.Visible && _drawer.GetGlobalRect().HasPoint(point)) || (_inspector.Visible && _inspector.GetGlobalRect().HasPoint(point));

    private void RefreshGhost()
    {
        if(_movingSite>=0 && (!_placing || _plantingTrees || _clearingTrees || _decorating || _pathTool>0 || _woodlandTool>0))DiscardRelocation();
        if(!_placing || !_decorating) CancelDecorationStroke();
        if(_placing) StopResourceSurvey();
        if (_ghostCells == null)
        {
            _ghostCells = new(); _ghost.AddChild(_ghostCells); _ghostModel = new(); _ghost.AddChild(_ghostModel);
        }
        _ghost.Visible = _placing && !PointerOverHud(_pointerPosition);
        if (!_placing) return;
        _placementProblem = PlacementProblem(_hover); _ghostValid = _placementProblem.Length == 0;
        if(_woodlandTool>0) { RefreshWoodlandGhost(); return; }
        if (_decorating) { RefreshDecorationGhost(); return; }
        if (_pathTool > 0) { RefreshPathGhost(); return; }
        if (_clearingTrees) { RefreshClearingGhost(); return; }
        var tint = _ghostValid ? new Color("a4caa0") : new Color("e38673");
        var moving=_movingSite>=0?_world.Cottages.FirstOrDefault(c=>c.Id==_movingSite):null;
        string key = moving!=null?RelocationModelKey(moving):_plantingTrees ? "tree" : _buildKind.ToString();
        if (_ghostModelKey != key)
        {
            Clear(_ghostModel); _previewMaterials.Clear(); _ghostModelKey = key;
            if (_plantingTrees) MakeTree(Vector3.Zero, 0.4f, new("8cad69")).Reparent(_ghostModel, false);
            else MakeBuilding(_ghostModel, moving??new Cottage { Kind = _buildKind }, 3);
            if(!_plantingTrees && _buildKind==BuildingKind.Orchard)MakeOrchardTrees(_ghostModel,moving??new Cottage{Kind=_buildKind,Planted=true,OrchardMature=true,Harvest=8},moving==null?4:moving.Harvest>0?4:moving.Planted?1+(int)(moving.Growth*2.9f):0);
            PreparePreview(_ghostModel,moving!=null);
        }
        foreach (var material in _previewMaterials) material.AlbedoColor = new(tint.R, tint.G, tint.B, 0.42f);
        var moveDoor=moving!=null?_world.RelocationEntrance(moving.Id,_hover,_rotation):(Cell?)null;
        bool dockFar = !_plantingTrees && _buildKind == BuildingKind.FishingDock && (moveDoor??_world.DockEntrance(_hover,_rotation))==World.FarBank(_hover,_rotation);
        _ghostModel.Position = _plantingTrees?OnGround(_hover.X,_hover.Z,.1f):BuildingPosition(_hover,_rotation,_buildKind,.1f);
        _ghostModel.RotationDegrees = new(0, (_plantingTrees?0:_rotation*90)+(dockFar?180:0), 0);
        Clear(_ghostCells);
        var footprint = _plantingTrees ? new[] { _hover } : World.Footprint(_hover, _rotation, _buildKind);
        foreach (var cell in footprint) GroundPatch(_ghostCells,cell.X,cell.Z,.94f,.94f,tint.Darkened(.15f),.06f);
        var door = moveDoor??(_plantingTrees ? new Cell(_hover.X + 1, _hover.Z) : _buildKind == BuildingKind.FishingDock ? _world.DockEntrance(_hover,_rotation) : _buildKind == BuildingKind.Bridge ? _world.BridgeEntrance(_hover, _rotation) : World.Door(_hover, _rotation));
        var marker = new Node3D { Position = OnGround(door.X,door.Z,.10f), RotationDegrees = new(0, (_plantingTrees?90:_rotation*90) + (dockFar || !_plantingTrees && _buildKind == BuildingKind.Bridge && door == World.FarBank(_hover, _rotation) ? 180 : 0), 0) }; _ghostCells.AddChild(marker);
        if(!_plantingTrees && _buildKind==BuildingKind.FishingDock)
        {
            var launch=moveDoor!=null?(dockFar?World.Door(_hover,_rotation):World.FarBank(_hover,_rotation)):_world.DockLaunch(_hover,_rotation);
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
    private void PreparePreview(Node root,bool retainColors=false)
    {
        foreach (var child in root.GetChildren())
        {
            if (child is Label3D label) { label.Hide(); continue; }
            if (child is MeshInstance3D mesh)
            {
                var material = new StandardMaterial3D { VertexColorUseAsAlbedo=retainColors,Transparency = BaseMaterial3D.TransparencyEnum.Alpha, ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded };
                mesh.MaterialOverride = material; mesh.CastShadow = GeometryInstance3D.ShadowCastingSetting.Off; _previewMaterials.Add(material);
            }
            PreparePreview(child,retainColors);
        }
    }
    private void UpdateBuildDescription()
    {
        if(_movingSite>=0){_buildDescription.Text=$"MOVE {BuildingName(_buildKind).ToUpperInvariant()} {_movingSite}\nRetains goods, controls and improvements.\nClick a valid destination · R / Shift+R rotates · Esc cancels";return;}
        if(_woodlandTool>0 && _placing)
        {
            _buildDescription.Text=_woodlandTool switch {
                1=>"PRESERVE TREES\nClick or drag. Loggers leave these trees.\nExplicit clearing takes precedence.",
                2=>"ALLOW HARVESTING\nClick or drag over preserved trees.\nLoggers may harvest them again.",
                3=>$"MANAGE GROVE · {_world.ManagedWoodland.Count}/{World.ManagedWoodlandLimit}\nClick or drag. Loggers replant these spots.\nBuilding and clearing replace grove spots.",
                _=>"REMOVE GROVE SPOTS\nClick or drag to stop future replanting.\nCurrent trees and planting work remain."
            }; return;
        }
        if (_decorating && _placing) { _buildDescription.Text = DecorationDescription; return; }
        if (_pathTool > 0 && _placing) { _buildDescription.Text = "PATHS\nClick or drag on clear land to paint/remove paths for free. Villagers choose quicker routes and move 25% faster toward path tiles. Building or planting replaces paths beneath it."; return; }
        if (_clearingTrees && _placing)
        {
            if (_world.Creative) { _buildDescription.Text = "CLEAR TREES & STUMPS\nClick to clear immediately. Existing timber returns to the yard; saplings yield no timber.\n"+_world.HabitatLoss(_hover); return; }
            _buildDescription.Text = "CLEAR TREES & STUMPS\nLoggers prioritize marked trees, recover existing timber, then remove roots. Land becomes usable when the roots are gone. Saplings yield no timber. Click a marked tree again to cancel.\n\n" +
                $"{_world.Trees.Count(t => t.ClearRequested)} clearing orders · {_world.People.Count(p => p.Role == Role.Logger)} loggers\n"+_world.HabitatLoss(_hover);
            return;
        }
        if (_world.Creative && !_plantingTrees) { _buildDescription.Text = $"{BuildingName(_buildKind).ToUpperInvariant()}\n{BuildingDescription(_buildKind)}\n\nInstant · Free. Choose level ground for the footprint and entrance."+(_buildKind==BuildingKind.Quarry?"\n"+_world.QuarrySurvey(_hover):_buildKind==BuildingKind.HuntingLodge?"\n"+_world.WildlifeSurvey(_hover):""); return; }
        var definition = Buildings.Get(_buildKind);
        int available = definition.Material == Inlanders.Simulation.Resource.Planks ? _world.AvailablePlanks : _world.Available;
        int cost = definition.Cost;
        string material = definition.Material.ToString().ToLowerInvariant();
        _buildDescription.Text = _plantingTrees && _placing ? "ALDERS\nLoggers plant for free. Grow for 3 days; yield 8 logs. Replant exhausted stumps." :
            $"{BuildingName(_buildKind).ToUpperInvariant()}\n{BuildingDescription(_buildKind)}\n\nBuildings need level ground, including the entrance.\n{available} {material} available · {cost} needed" + (available < cost ? "\nYou can plan now; builders wait for materials." : "");
        if(!_plantingTrees && _placing && _buildKind==BuildingKind.FishingDock) _buildDescription.Text+="\n\n"+_world.FishingSurvey(_hover,_rotation);
        if(!_plantingTrees && _placing && _buildKind==BuildingKind.Quarry) _buildDescription.Text+="\n\n"+_world.QuarrySurvey(_hover);
        if(!_plantingTrees && _placing && _buildKind==BuildingKind.HuntingLodge) _buildDescription.Text+="\n\n"+_world.WildlifeSurvey(_hover);
        if(!_plantingTrees && definition.StoneCost>0) _buildDescription.Text+=$"\n{_world.AvailableStone} stone available · {definition.StoneCost} needed. Stone is hauled from the central store.";
    }
}

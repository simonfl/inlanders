using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Collections.Generic;
using Resource = Inlanders.Simulation.Resource;

public partial class Game : Node3D
{
    private World _world = World.NewScenario();
    private Camera3D _camera = null!;
    private Node3D _dynamic = null!, _stored = null!, _ghost = null!, _selection = null!;
    private sealed class TreeView { public Node3D Top = null!, Pile = null!; public int Logs = -1, Stage = -1; }
    private readonly List<PersonView> _people = new();
    private readonly Dictionary<int, TreeView> _trees = new();
    private readonly Dictionary<int, (Node3D Body, int Stage)> _cottages = new();
    private int _lastStored = -1, _lastPlanks = -1, _selectedPerson = -1, _selectedSite = -1;
    private bool _placing, _plantingTrees, _rotated, _paused, _ghostValid;
    private float _speed = 1, _clock, _accumulator, _angle = 0.72f;
    private Vector3 _focus = new(0, 0, 0);
    private Cell _hover = new(3, 0);
    private BuildingKind _buildKind;
    private readonly Color _wood = new("80553c"), _cream = new("f1ddb3"), _roof = new("ac5844");
    private static readonly string[] PriorityNames = { "Low", "Normal", "High" };

    public override void _Ready()
    {
        GetTree().NodeAdded += RegisterWorldLabel;
        if (OS.GetCmdlineUserArgs().Any(a => a.EndsWith("smoke-test"))) _campaignPath = "artifacts/campaign-smoke.json";
        GetWindow().MinSize = new(960, 640);
        MakeLandscape(); MakeAudio(); MakeUi();
        _dynamic = new(); AddChild(_dynamic);
        _ghost = new(); AddChild(_ghost); _selection = new(); AddChild(_selection);
        CreateActors(); UpdateCamera(); RefreshGhost();
        if (!OS.GetCmdlineUserArgs().Any(a => a.EndsWith("smoke-test"))) MakeMainMenu();
        if (OS.GetCmdlineUserArgs().Contains("--smoke-test")) CallDeferred(MethodName.RunSmoke);
        if (OS.GetCmdlineUserArgs().Contains("--audio-smoke-test")) CallDeferred(MethodName.RunAudioSmoke);
        if (OS.GetCmdlineUserArgs().Contains("--hud-smoke-test")) CallDeferred(MethodName.RunHudSmoke);
        if (OS.GetCmdlineUserArgs().Contains("--home-smoke-test")) CallDeferred(MethodName.RunHomeSmoke);
        if (OS.GetCmdlineUserArgs().Contains("--campaign-smoke-test")) CallDeferred(MethodName.RunCampaignSmoke);
        if (OS.GetCmdlineUserArgs().Contains("--map-smoke-test")) CallDeferred(MethodName.RunMapSmoke);
        if (OS.GetCmdlineUserArgs().Contains("--clearing-smoke-test")) CallDeferred(MethodName.RunClearingSmoke);
        if (OS.GetCmdlineUserArgs().Contains("--menu-smoke-test")) CallDeferred(MethodName.RunMainMenuSmoke);
        if (OS.GetCmdlineUserArgs().Contains("--art-smoke-test")) CallDeferred(MethodName.RunArtSmoke);
        if (OS.GetCmdlineUserArgs().Contains("--catalog-smoke-test")) CallDeferred(MethodName.RunCatalogSmoke);
        if (OS.GetCmdlineUserArgs().Contains("--river-smoke-test")) CallDeferred(MethodName.RunRiverSmoke);
        if (OS.GetCmdlineUserArgs().Contains("--production-smoke-test")) CallDeferred(MethodName.RunProductionSmoke);
    }
    private void CreateActors()
    {
        RebuildLandscape();
        _camera.Size = Math.Min(_camera.Size, MaximumZoom); UpdateCamera();
        ResetWorldAudio();
        Clear(_dynamic); _people.Clear(); _trees.Clear(); _cottages.Clear(); _lastStored = -1; _lastPlanks = -1;
        CreateFoodViews();
        _stored = new(); _dynamic.AddChild(_stored);
        foreach (var p in _world.People)
        {
            var view = MakeVillager(p.Id); _dynamic.AddChild(view.Body); view.Body.Position = OnGround(p.Position.X, p.Position.Y);
            _people.Add(view);
        }
    }
    private void Reset()
    {
        if (_world.Campaign != null) { SwitchCampaign(_world.Campaign.Level, true); return; }
        try
        {
            var next = _world.Creative ? World.NewCreative(_world.Map.Name == "Three clearings") : _world.Map.Name == "Three clearings" ? World.NewLargeMap() : World.NewScenario();
            _world.SaveFile(CurrentSavePath + ".before-new");
            _buildKind = BuildingKind.Cottage; _plantingTrees = false; _rotated = false; AdoptWorld(next);
            Notice("New village ready and paused. Options can restore the village before restart.");
        }
        catch (Exception e) { Notice("Could not restart; current village kept. " + e.Message); }
    }
    private void TogglePause() { _paused = !_paused; _pauseButton.Text = _paused ? "Resume  [Space]" : "Pause  [Space]"; }
    private void UpdateCamera()
    {
        var map = _world.Map;
        _focus.X = Mathf.Clamp(_focus.X, map.MinX, map.MaxX); _focus.Z = Mathf.Clamp(_focus.Z, map.MinZ, map.MaxZ);
        _focus.Y = Height(_focus.X, _focus.Z);
        float distance = Math.Max(25, Math.Max(map.Width, map.Depth) * 1.5f);
        _camera.Far = distance * 4;
        _camera.Position = _focus + new Vector3(MathF.Sin(_angle) * distance, distance * 0.96f, MathF.Cos(_angle) * distance); _camera.LookAt(_focus);
    }
    private void RefreshSelection()
    {
        Clear(_selection); _selection.Position = Vector3.Zero;
        if (_selectedPerson >= 0)
        {
            for (int i=0;i<12;i++) { float a=i*Mathf.Tau/12; var mark=Box(_selection,new(MathF.Cos(a)*0.4f,0.035f,MathF.Sin(a)*0.4f),new(0.12f,0.03f,0.05f),new("f1d292")); mark.Rotation=new(0,-a,0); }
            return;
        }
        if (_world.Cottages.FirstOrDefault(c => c.Id == _selectedSite) is not Cottage site) return;
        foreach (var c in World.Footprint(site.Cell, site.Rotated, site.Kind)) GroundPatch(_selection,c.X,c.Z,1.04f,1.04f,new("e8c688"),.035f);
    }
    private void PlaceCottage(Cell at)
    {
        _hover = at;
        if (_decorating) { EditDecoration(at); return; }
        if (_pathTool > 0) { _pathStroke = true; PaintPath(at); return; }
        if (_clearingTrees) { MarkClearing(at); return; }
        if (_plantingTrees)
        {
            if (_world.PlantTree(at) != null) { UiCue(Cue.Place); Notice("Planting marked. Loggers plant after clearing orders; trees grow for 3 days and yield 8 logs."); }
            else UiCue(Cue.Reject);
            RefreshGhost(); return;
        }
        var site = _world.Place(at, _rotated, _buildKind); if (site == null) { UiCue(Cue.Reject); RefreshGhost(); return; }
        UiCue(Cue.Place);
        SelectBuilding(site.Id); _placing = false; RefreshGhost(); RebuildQueue();
    }
    private void ToggleTreePlanting()
    {
        _pathTool = 0; _decorating = false;
        _clearingTrees = false;
        _placing = !(_placing && _plantingTrees); _plantingTrees = true; RefreshGhost();
    }

    public override void _UnhandledInput(InputEvent input)
    {
        if (_atMainMenu) return;
        if (input is InputEventKey key && key.Pressed && !key.Echo)
        {
            if (EditingText) return;
            if (key.Keycode == Key.H) { ToggleWatch(); return; }
            if (_watching && key.Keycode == Key.Escape) { ExitWatch(); return; }
            if (_watching && key.Keycode is Key.B or Key.V or Key.G or Key.O or Key.I or Key.T or Key.C or Key.P) ExitWatch();
            if (key.Keycode is Key.Key1 or Key.Key2 or Key.Key3) { int slot = (int)key.Keycode - (int)Key.Key1; if (key.CtrlPressed) StoreCameraView(slot); else RecallCameraView(slot); return; }
            if (key.Keycode == Key.Space) TogglePause();
            if (key.Keycode == Key.M) ToggleSoundMute();
            if (key.Keycode == Key.F5) SaveWorld();
            if (key.Keycode == Key.F9) LoadWorld();
            if (key.Keycode == Key.Home) FrameMap();
            if (key.Keycode == Key.R && _placing && !_plantingTrees && !_clearingTrees && _pathTool == 0) { _rotated = !_rotated; RefreshGhost(); }
            if (key.Keycode == Key.Escape) { if (_placing) { _placing = false; RefreshGhost(); } else if (_drawer.Visible) CloseDrawer(); else ClearSelection(); }
            if (key.Keycode == Key.I) ToggleDrawer(4);
            if (key.Keycode == Key.B) ToggleDrawer(1);
            if (key.Keycode == Key.V) ToggleDrawer(0);
            if (key.Keycode == Key.G) ToggleDrawer(2);
            if (key.Keycode == Key.O) ToggleDrawer(3);
            if (key.Keycode == Key.T) { ToggleTreePlanting(); ClearSelection(); }
            if (key.Keycode == Key.C) ToggleClearing();
            if (key.Keycode == Key.P) TogglePaths(key.ShiftPressed ? 2 : 1);
            if (key.Keycode == Key.Q) { _angle -= Mathf.Pi / 2; UpdateCamera(); }
            if (key.Keycode == Key.E) { _angle += Mathf.Pi / 2; UpdateCamera(); }
        }
        if (input is InputEventMouseButton mouse && mouse.Pressed)
        {
            if (mouse.ButtonIndex == MouseButton.WheelUp) _camera.Size = Math.Max(12, _camera.Size - 1);
            if (mouse.ButtonIndex == MouseButton.WheelDown) _camera.Size = Math.Min(MaximumZoom, _camera.Size + 1);
            if (_watching || mouse.ButtonIndex != MouseButton.Left) return;
            if (_placing) { if (Ground(mouse.Position) is Vector3 point) PlaceCottage(new(Mathf.RoundToInt(point.X), Mathf.RoundToInt(point.Z))); return; }
            var closest = _people.Select((v, i) => (Index: i, Distance: _camera.UnprojectPosition(v.Body.Position + Vector3.Up * 0.6f).DistanceTo(mouse.Position))).OrderBy(v => v.Distance).First();
            if (closest.Distance < 25) SelectPerson(closest.Index);
            else if (Ground(mouse.Position) is Vector3 p)
            {
                var site = _world.Cottages.FirstOrDefault(c => World.Footprint(c.Cell, c.Rotated, c.Kind).Contains(new(Mathf.RoundToInt(p.X), Mathf.RoundToInt(p.Z))));
                if (site != null) SelectBuilding(site.Id); else ClearSelection();
            }
        }
    }
    public override void _Process(double delta)
    {
        if (_atMainMenu) { RenderActors(0); RenderFoodViews(); UpdateAudio(Math.Min((float)delta, 0.1f)); return; }
        float dt = Math.Min((float)delta, 0.1f); _clock += dt * (_paused ? 0 : _speed); _uiTime += dt;
        var pan = new Vector3((Input.IsPhysicalKeyPressed(Key.D) ? 1 : 0) - (Input.IsPhysicalKeyPressed(Key.A) ? 1 : 0), 0,
            (Input.IsPhysicalKeyPressed(Key.S) ? 1 : 0) - (Input.IsPhysicalKeyPressed(Key.W) ? 1 : 0));
        if (!EditingText && pan != Vector3.Zero) { _followPerson = false; _focus += pan.Rotated(Vector3.Up, _angle) * dt * Math.Max(7, _camera.Size * 0.35f); UpdateCamera(); }
        _ghost.Visible = _placing && !PointerOverHud(_pointerPosition);
        if (_ghost.Visible && Ground(_pointerPosition) is Vector3 p)
        {
            var cell = new Cell(Mathf.RoundToInt(p.X), Mathf.RoundToInt(p.Z));
            if (cell != _hover || _placementProblem != PlacementProblem(cell)) { _hover = cell; RefreshGhost(); }
        }
        if (!_paused) { _accumulator += dt * _speed; while (_accumulator >= 0.1f) { _world.Tick(0.1f); _accumulator -= 0.1f; } }
        AdvanceAutosave(delta); UpdateRecoveryUi(); RenderActors(dt); UpdateAtmosphere(); UpdateFollowing(); RenderFoodViews(); UpdateHud(); UpdateWatchUi(); UpdateAudio(dt);
    }
    private void RenderActors(float dt)
    {
        RenderPaths(); RenderDecorations(); RenderVisitor();
        while (_people.Count < _world.Population)
        {
            var p = _world.People[_people.Count]; var view = MakeVillager(p.Id);
            _dynamic.AddChild(view.Body); view.Body.Position = OnGround(p.Position.X, p.Position.Y); _people.Add(view);
        }
        foreach (var v in _world.People)
        {
            var view = _people[v.Id]; var target = OnGround(v.Position.X, v.Position.Y);
            var movement = target - view.Body.Position; movement.Y = 0;
            if (movement.Length() > 0.025f) view.Body.Rotation = new(0, MathF.Atan2(-movement.X, -movement.Z), 0);
            view.Body.Position = view.Body.Position.Lerp(target, Math.Min(1, dt * 18 * _speed));
            view.Body.Position = OnGround(view.Body.Position.X, view.Body.Position.Z);
            AnimateVillager(view, v);
        }
        foreach (int id in _trees.Keys.Where(id => !_world.Trees.Any(t => t.Id == id)).ToArray())
        {
            _trees[id].Top.QueueFree(); _trees[id].Pile.QueueFree(); _trees.Remove(id);
        }
        foreach (var t in _world.Trees)
        {
            if (!_trees.TryGetValue(t.Id, out var view))
            {
                var top = MakeTree(OnGround(t.Cell.X, t.Cell.Z), 0.9f, new("6e8b50")); top.Reparent(_dynamic);
                var pile = new Node3D { Position = OnGround(t.Cell.X, t.Cell.Z) }; _dynamic.AddChild(pile);
                view = new TreeView { Top = top, Pile = pile }; _trees[t.Id] = view;
            }
            view.Top.Visible = !t.Felled && !t.NeedsPlanting && (t.Logs > 0 || t.Growth < 1);
            view.Top.Scale = Vector3.One * 0.9f * (0.2f + 0.8f * t.Growth);
            int treeStage = (t.NeedsPlanting ? 0 : t.Felled ? 2 : 1) + (t.ClearRequested ? 10 : 0);
            if (view.Logs == t.Logs && view.Stage == treeStage) continue;
            view.Logs = t.Logs; view.Stage = treeStage; Clear(view.Pile);
            if (t.ClearRequested)
            {
                ClearingCross(view.Pile, new(0, 0.10f, 0), new("f0bd70"), 1.25f);
                view.Pile.AddChild(new Label3D { Text = "×", Position = new(0, t.Felled || t.NeedsPlanting ? 0.6f : 3.1f * (0.2f + 0.8f * t.Growth), 0),
                    FontSize = 48, PixelSize = 0.014f, Billboard = BaseMaterial3D.BillboardModeEnum.Enabled, Modulate = new("f0bd70"), OutlineSize = 5 });
            }
            if (t.NeedsPlanting)
            {
                Cylinder(view.Pile, new(0, 0.025f, 0), 0.35f, 0.05f, new("805f42"));
                Box(view.Pile, new(0, 0.3f, 0), new(0.07f, 0.6f, 0.07f), _cream);
            }
            if (!t.Felled) continue;
            if (!t.Salvage) Cylinder(view.Pile, new(0, 0.12f, 0), 0.17f, 0.24f, _wood);
            for (int i = 0; i < t.Logs; i++)
            {
                var at = new Vector3(0, 0.16f + i / 3 * 0.22f, -0.5f + i % 3 * 0.28f);
                if (t.Material == Resource.Planks) Plank(view.Pile, at); else Log(view.Pile, at, 0.65f);
            }
        }
        if (_lastStored != _world.YardLogs || _lastPlanks != _world.Planks)
        {
            _lastStored = _world.YardLogs; _lastPlanks = _world.Planks; Clear(_stored);
            for (int i = 0; i < _world.YardLogs; i++) Log(_stored, new(-3.4f + (i % 2) * 0.65f, 0.25f + i / 8 * 0.22f, 2.5f + i / 2 % 4 * 0.3f), 0.55f);
            for (int i = 0; i < _world.Planks; i++) Plank(_stored, new(-3, 0.18f + i / 3 * 0.12f, 4.5f + i % 3 * 0.22f));
        }
        foreach (int id in _cottages.Keys.Where(id => !_world.Cottages.Any(c => c.Id == id)).ToArray()) { _cottages[id].Body.QueueFree(); _cottages.Remove(id); }
        foreach (var h in _world.Cottages)
        {
            int stage = h.DemolitionRequested && h.DemolitionProgress > 0 ? (h.DemolitionProgress > .7f ? 1 : 2) : h.Complete ? 3 : h.Construction > 0.4f ? 2 : h.Delivered > 0 ? 1 : 0;
            if (!_cottages.TryGetValue(h.Id, out var view)) { view = (new Node3D(), -1); _dynamic.AddChild(view.Body); }
            int viewKey = h.Kind == BuildingKind.Stockpile ? stage * 100 + h.StoredLogs : stage;
            if (h.Kind == BuildingKind.Bakery) viewKey = stage * 100 + h.InputGrain * 10 + h.OutputBread;
            if (h.Kind == BuildingKind.Sawmill) viewKey = stage * 100 + h.InputLogs * 10 + h.OutputPlanks;
            if (h.DemolitionRequested) viewKey += 10000;
            if (view.Stage != viewKey)
            {
                Clear(view.Body); MakeBuilding(view.Body, h, stage);
                if (h.DemolitionRequested) { Box(view.Body, new(0,.55f,1.2f), new(.9f,.12f,.12f), new("d7a453")); Box(view.Body, new(-.32f,.3f,1.2f), new(.1f,.6f,.1f), _wood); Box(view.Body, new(.32f,.3f,1.2f), new(.1f,.6f,.1f), _wood); }
                view.Body.Position = OnGround(h.Cell.X + (h.Kind != BuildingKind.Bridge && h.Rotated ? -0.5f : 0), h.Cell.Z + (h.Kind == BuildingKind.Bridge || h.Rotated ? 0 : -0.5f));
                view.Body.RotationDegrees = new(0, h.Rotated ? 90 : 0, 0); _cottages[h.Id] = (view.Body, viewKey);
            }
            if (stage == 3 && h.Kind == BuildingKind.Sawmill)
            {
                var blade = view.Body.GetNode<Node3D>("SawBlade");
                // Progress is simulation time: pause and interrupted batches hold their pose.
                blade.Position = new(0, MathF.Sin(h.SawProgress * Mathf.Tau * 12) * .13f, 0);
            }
            if (stage == 3 && h.Kind == BuildingKind.Bakery)
                view.Body.GetNode<Node3D>("OvenGlow").Visible = _world.People.Any(p => p.WorkplaceId == h.Id && p.Task == Work.Baking);
        }
    }
}

using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Collections.Generic;
using Resource = Inlanders.Simulation.Resource;

public partial class Game
{
    private Label _objective = null!, _hint = null!, _staffing = null!, _inspect = null!, _siteInfo = null!, _day = null!, _housing = null!, _foodStatus = null!, _drawerTitle = null!;
    private Button _buildButton = null!, _pauseButton = null!, _speedButton = null!, _resetButton = null!, _assignButton = null!, _cancelButton = null!;
    private Button _supperButton = null!, _saveButton = null!, _loadButton = null!, _plantTreeButton = null!;
    private readonly Dictionary<Role, Label> _counts = new();
    private readonly Dictionary<(Role, int), Button> _allocationButtons = new();
    private readonly Dictionary<Resource, Label> _resourceValues = new();
    private readonly List<Button> _roster = new(), _priorityButtons = new(), _menuButtons = new();
    private readonly Dictionary<int, Button> _queueButtons = new();
    private readonly Dictionary<BuildingKind, Button> _kindButtons = new();
    private VBoxContainer _queue = null!, _personDetails = null!, _buildingDetails = null!;
    private ProgressBar _progress = null!;
    private TabContainer _tabs = null!;
    private readonly List<ScrollContainer> _drawerPages = new();
    private Control _hud = null!;
    private PanelContainer _topBar = null!, _bottomBar = null!, _drawer = null!, _inspector = null!, _hintPanel = null!;
    private Label _brand = null!, _shortcuts = null!;
    private Vector2 _hudSize;
    private static readonly string[] MenuNames = { "People", "Build", "Goals", "Options", "Economy" };

    private static string RoleName(Role role) => role.ToString();
    private static Role NextRole(Role role) => (Role)(((int)role + 1) % Enum.GetValues<Role>().Length);
    private static string BuildCost(BuildingKind kind) => kind == BuildingKind.Lodge ? "8 planks · 4 beds" : "6 logs";
    private static string BuildingName(BuildingKind kind) => kind == BuildingKind.VegetableGarden ? "Vegetable garden" : kind == BuildingKind.ForagerHut ? "Forager hut" : kind == BuildingKind.Square ? "Village square" : kind.ToString();
    private static string TaskName(Work task) => task switch
    {
        Work.ToHaulPickup => "Collecting logs", Work.ToHaulDrop => "Hauling logs",
        Work.ToTree => "To timber", Work.Chopping => "Logging", Work.ToStockpile => "Hauling",
        Work.ToSapling or Work.PlantingTree => "Planting tree", Work.ToSawLogs => "Fetching logs",
        Work.ToClearStump or Work.ClearingStump => "Clearing roots",
        Work.ToSawmill or Work.Sawing => "Sawing", Work.ToPlanks => "Collecting planks",
        Work.ToMaterials => "Fetching", Work.ToCottage => "Delivering", Work.ToBuild => "To site", Work.Building => "Building",
        Work.ToBush or Work.Foraging => "Foraging", Work.ToFarm or Work.Planting => "Sowing", Work.Harvesting => "Harvesting",
        Work.ToGrain => "Fetching", Work.ToOven or Work.Baking => "Baking", Work.ToBread or Work.ToPantry => "Hauling food",
        Work.ToSupper or Work.Supper => "Supper", _ => "Idle"
    };
    private Button Button(string text, Action pressed, float width = 0)
    {
        var button = new Button { Text = text, CustomMinimumSize = new(width, 34), FocusMode = Control.FocusModeEnum.None };
        button.Pressed += () => { UiCue(Cue.Click); pressed(); }; return button;
    }
    private Label Text(string text, int size = 16, bool wrap = false)
    {
        var label = new Label { Text = text, AutowrapMode = wrap ? TextServer.AutowrapMode.WordSmart : TextServer.AutowrapMode.Off, MouseFilter = Control.MouseFilterEnum.Ignore };
        label.AddThemeFontSizeOverride("font_size", size); return label;
    }
    private void MakeUi()
    {
        var layer = new CanvasLayer(); AddChild(layer);
        _hud = new Control { MouseFilter = Control.MouseFilterEnum.Ignore, Theme = HudTheme() };
        layer.AddChild(_hud); _hud.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
        _topBar = HudPanel(_hud); var top = new HBoxContainer(); top.AddThemeConstantOverride("separation", 16); _topBar.AddChild(top);
        _brand = Text("INLANDERS", 18); _brand.Modulate = _cream; top.AddChild(_brand);
        foreach (var resource in new[] { Resource.Logs, Resource.Planks, Resource.Berries, Resource.Grain, Resource.Bread, Resource.Vegetables })
        {
            var col = new VBoxContainer { CustomMinimumSize = new(62, 0), SizeFlagsHorizontal = Control.SizeFlags.ExpandFill };
            col.AddThemeConstantOverride("separation", 0); top.AddChild(col);
            var label = Text(resource == Resource.Vegetables ? "VEG" : resource.ToString().ToUpperInvariant(), 11); label.Modulate = new("a8bcb0"); col.AddChild(label);
            var count = Text("0", 20); col.AddChild(count); _resourceValues[resource] = count;
            col.MouseFilter = Control.MouseFilterEnum.Stop;
            col.MouseDefaultCursorShape = Control.CursorShape.PointingHand;
            col.GuiInput += input => { if (input is InputEventMouseButton { ButtonIndex: MouseButton.Left, Pressed: true }) { OpenEconomy(); col.AcceptEvent(); } };
            col.TooltipText = resource == Resource.Grain ? "Raw grain feeds the bakery; villagers eat berries, vegetables, and bread." : "Stored " + resource.ToString().ToLowerInvariant();
        }
        var population = new VBoxContainer { CustomMinimumSize = new(84, 0) }; top.AddChild(population);
        population.AddChild(Text("HOUSING", 11)); _housing = Text("0 / 8", 18); population.AddChild(_housing);
        var clock = new VBoxContainer { CustomMinimumSize = new(90, 0) }; top.AddChild(clock);
        _day = Text("Day 1", 17); clock.AddChild(_day); _foodStatus = Text("Well fed", 12); clock.AddChild(_foodStatus);
        _pauseButton = Button("Pause", TogglePause, 78); _pauseButton.TooltipText = "Pause / resume [Space]"; top.AddChild(_pauseButton);
        _speedButton = Button("1×", () => _speed = _speed == 1 ? 3 : _speed == 3 ? 6 : 1, 48); _speedButton.TooltipText = "Change game speed: 1× / 3× / 6×"; top.AddChild(_speedButton);
        _bottomBar = HudPanel(_hud); var bottom = new HBoxContainer(); bottom.AddThemeConstantOverride("separation", 8); _bottomBar.AddChild(bottom);
        foreach (int index in new[] { 1, 0, 4, 2, 3 })
        {
            var b = Button(MenuNames[index], () => ToggleDrawer(index), 94); bottom.AddChild(b);
            while (_menuButtons.Count <= index) _menuButtons.Add(null!); _menuButtons[index] = b;
            b.TooltipText = index switch { 0 => "Workforce and villagers [V]", 1 => "Buildings and planting [B]", 2 => "The first village supper [G]", 4 => "Inventory, shortages, and idle workers [I]", _ => "Save, load, audio, and controls [O]" };
        }
        bottom.AddChild(new Control { SizeFlagsHorizontal = Control.SizeFlags.ExpandFill });
        _shortcuts = Text("WASD pan · Wheel zoom · Q/E orbit", 13); _shortcuts.Modulate = new("a8bcb0"); bottom.AddChild(_shortcuts);
        _drawer = HudPanel(_hud); var drawerColumn = new VBoxContainer(); drawerColumn.AddThemeConstantOverride("separation", 12); _drawer.AddChild(drawerColumn);
        var heading = new HBoxContainer(); drawerColumn.AddChild(heading);
        _drawerTitle = Text("Build", 21); _drawerTitle.Modulate = _cream; _drawerTitle.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill; heading.AddChild(_drawerTitle);
        heading.AddChild(Button("×", CloseDrawer, 32));
        _tabs = new TabContainer { TabsVisible = false, SizeFlagsVertical = Control.SizeFlags.ExpandFill }; drawerColumn.AddChild(_tabs);
        var people = DrawerPage("People"); var build = DrawerPage("Build"); var goals = DrawerPage("Goals"); var options = DrawerPage("Options"); var economy = DrawerPage("Economy");
        MakePeopleMenu(people); MakeBuildMenu(build); MakeGoalsMenu(goals); MakeOptionsMenu(options); MakeEconomyMenu(economy);
        _inspector = HudPanel(_hud); _inspectionScroll = new ScrollContainer { HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled }; _inspector.AddChild(_inspectionScroll);
        var inspection = new VBoxContainer { SizeFlagsHorizontal = Control.SizeFlags.ExpandFill }; inspection.AddThemeConstantOverride("separation", 12); _inspectionScroll.AddChild(inspection);
        var inspectHeading = new HBoxContainer(); inspection.AddChild(inspectHeading);
        var inspectLabel = Text("SELECTED", 12); inspectLabel.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill; inspectLabel.Modulate = new("a8bcb0"); inspectHeading.AddChild(inspectLabel);
        inspectHeading.AddChild(Button("×", ClearSelection, 32));
        _personDetails = new VBoxContainer(); _personDetails.AddThemeConstantOverride("separation", 12); inspection.AddChild(_personDetails);
        _inspect = Text("", 16, true); _personDetails.AddChild(_inspect); MakeJobChoice();
        _assignButton = Button("Assign", () => { if (_selectedPerson >= 0) _world.Assign(_selectedPerson, (Role)_jobChoice.GetSelectedId()); }); _personDetails.AddChild(_assignButton);
        _buildingDetails = new VBoxContainer(); _buildingDetails.AddThemeConstantOverride("separation", 12); inspection.AddChild(_buildingDetails);
        _siteInfo = Text("", 16, true); _buildingDetails.AddChild(_siteInfo);
        var priorities = new HBoxContainer(); _buildingDetails.AddChild(priorities);
        for (int i = 0; i < 3; i++)
        {
            int priority = i; var b = Button(PriorityNames[i], () => _world.SetPriority(_selectedSite, priority)); b.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            b.TooltipText = "Construction priority affects new jobs; committed deliveries finish."; priorities.AddChild(b); _priorityButtons.Add(b);
        }
        _cancelButton = Button("Cancel construction", () => { if (_world.Cancel(_selectedSite)) { ClearSelection(); RebuildQueue(); } });
        _cancelButton.TooltipText = "Delivered materials remain as salvage; carried materials return to storage."; _buildingDetails.AddChild(_cancelButton);
        MakeStorageControls(); MakeManagementControls(); MakeHappinessUi();
        inspection.AddChild(Button("Move camera here", () =>
        {
            if (_selectedSite >= 0 && _world.Cottages.FirstOrDefault(c => c.Id == _selectedSite) is Cottage c) _focus = new(c.Cell.X, 0, c.Cell.Z);
            else if (_selectedPerson >= 0) { var p = _world.People[_selectedPerson].Position; _focus = new(p.X, 0, p.Y); }
            UpdateCamera();
        }));
        _hintPanel = HudPanel(_hud); _hint = Text("", 14, true); _hintPanel.AddChild(_hint); _hintPanel.MouseFilter = Control.MouseFilterEnum.Ignore;
        _drawer.Hide(); _inspector.Hide(); _hintPanel.Hide(); LayoutHud(); MakeWatchUi();
    }
    private void MakePeopleMenu(VBoxContainer column)
    {
        column.AddChild(Text("WORK ASSIGNMENTS", 12));
        foreach (var role in Enum.GetValues<Role>().Where(r => r != Role.Unassigned))
        {
            var row = new HBoxContainer(); column.AddChild(row); _counts[role] = Text(""); _counts[role].SizeFlagsHorizontal = Control.SizeFlags.ExpandFill; row.AddChild(_counts[role]);
            foreach (int change in new[] { -1, 1 })
            {
                var b = Button(change < 0 ? "−" : "+", () => _world.AdjustWorkers(role, change), 34);
                b.TooltipText = change < 0 ? "Unassign a worker; carried goods return first." : "Use an unassigned worker or transfer one from another job.";
                _allocationButtons[(role, change)] = b; row.AddChild(b);
            }
        }
        _staffing = Text("", 13, true); column.AddChild(_staffing);
        MakeArrivalControls(column);
        column.AddChild(Text("VILLAGERS · SELECT TO INSPECT", 12)); MakeRosterFilter(column);
        var roster = new GridContainer { Columns = 2 }; column.AddChild(roster);
        _rosterContainer = roster;
    }
    private void MakeBuildMenu(VBoxContainer column)
    {
        MakeBuildingFilter(column);
        var kinds = new GridContainer { Columns = 2 }; kinds.AddThemeConstantOverride("h_separation", 8); kinds.AddThemeConstantOverride("v_separation", 8); column.AddChild(kinds);
        foreach (var kind in Enum.GetValues<BuildingKind>())
        {
            var b = Button(BuildingName(kind) + "\n" + BuildCost(kind), () => BeginPlacement(kind));
            b.AddThemeFontSizeOverride("font_size", 14); b.CustomMinimumSize = new(128, 60); b.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            b.TooltipText = BuildingDescription(kind);
            kinds.AddChild(b); _kindButtons[kind] = b;
        }
        _plantTreeButton = Button("Plant alders · free [T]", () => { ToggleTreePlanting(); ClearSelection(); }); column.AddChild(_plantTreeButton);
        _plantTreeButton.TooltipText = "Mark open ground or exhausted stumps. Loggers plant; trees grow for three days and yield eight logs.";
        _clearTreeButton = Button("Clear trees & stumps [C]", ToggleClearing); column.AddChild(_clearTreeButton);
        _clearTreeButton.TooltipText = "Click trees or stumps to mark logger work; click again to cancel. Existing timber is recovered, then roots are removed.";
        column.AddChild(Button("Paint paths [P]", () => TogglePaths(1)));
        column.AddChild(Button("Remove paths [Shift+P]", () => TogglePaths(2)));
        MakeDecorationMenu(column);
        _buildDescription = Text("", 14, true); column.AddChild(_buildDescription);
        _buildButton = Button("", () => { if (_placing) { _placing = false; RefreshGhost(); } else BeginPlacement(_buildKind); }); column.AddChild(_buildButton);
        column.AddChild(Text("BUILDINGS & CONSTRUCTION", 12)); MakeConstructionFilter(column); _queue = new VBoxContainer(); column.AddChild(_queue);
    }
    private void MakeGoalsMenu(VBoxContainer column)
    {
        _goalTitle = Text("", 20, true); column.AddChild(_goalTitle);
        _goalArrival = Text("", 15, true); column.AddChild(_goalArrival);
        _objective = Text("", 18, true); column.AddChild(_objective);
        _progress = new ProgressBar { ShowPercentage = false, CustomMinimumSize = new(0, 8) }; column.AddChild(_progress);
        _supperButton = Button("Host supper", () => { if (_world.BeginSupper()) { _placing = false; RefreshGhost(); CloseDrawer(); Notice("The villagers are gathering for supper."); } }); column.AddChild(_supperButton);
        _standaloneGuide = new(); column.AddChild(_standaloneGuide);
        _standaloneGuide.AddChild(Text("GETTING THERE", 12));
        _standaloneGuide.AddChild(Text("Forager hut → berries\nVegetable garden → ready-to-eat food\nFarm → grain → bakery → bread\nSawmill → planks → four-bed lodge\n\nMeals use one food per person daily, berries first. Grain must be baked. Cottages house two; lodges four. Invite newcomers from People when you have spare beds and food.", 15, true));
        MakeCampaignUi(column);
    }
    private void MakeOptionsMenu(VBoxContainer column)
    {
        column.AddChild(Text("YOUR VILLAGE", 12));
        column.AddChild(Button("Watch village [H]", ToggleWatch));
        MakeCameraViewsUi(column);
        var saves = new HBoxContainer(); column.AddChild(saves);
        _saveButton = Button("Save [F5]", SaveWorld); _loadButton = Button("Load [F9]", LoadWorld);
        _saveButton.SizeFlagsHorizontal = _loadButton.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill; saves.AddChild(_saveButton); saves.AddChild(_loadButton);
        _resetButton = Button("Start again", Reset); column.AddChild(_resetButton);
        column.AddChild(Button("Return to main menu", ReturnToMainMenu));
        column.AddChild(Button("Explore larger map", OpenLargeMap));
        column.AddChild(Button("Return to original map", OpenOriginalMap));
        column.AddChild(Text("Three clearings · 32×32 landscape. Starts or resumes a separate save. Home frames the map.", 14, true));
        MakeAtmosphereUi(column);
        column.AddChild(Text("SOUND", 12)); MakeAudioUi(column);
        column.AddChild(Text("CONTROLS", 12));
        column.AddChild(Text("WASD  Pan camera\nWheel  Zoom\nQ / E  Orbit\nSpace  Pause / resume\nB  Build menu · T  Plant trees\nV  People · I  Economy · G  Goals · O  Options\nR  Rotate building preview\nEsc  Cancel preview / close panel\nM  Mute sound · H  Watch village", 14, true));
    }
    private void RebuildQueue()
    {
        foreach (var child in _queue.GetChildren()) { _queue.RemoveChild(child); child.QueueFree(); } _queueButtons.Clear();
        foreach (var site in _world.Cottages)
        {
            int id = site.Id; var b = Button("", () => SelectBuilding(id)); b.Alignment = HorizontalAlignment.Left; b.AddThemeFontSizeOverride("font_size", 14);
            _queue.AddChild(b); _queueButtons[id] = b;
        }
    }
    private void UpdateHud()
    {
        if (_hudSize != _hud.Size) LayoutHud();
        UpdatePopulationUi();
        UpdateCameraViewsUi();
        _day.Text = $"Day {_world.Food.Day}"; _housing.Text = $"{_world.Housed} / {_world.Population}";
        _pauseButton.Text = _paused ? "Resume" : "Pause"; _speedButton.Text = $"{_speed}×";
        _foodStatus.Text = _world.Food.Hunger > 0 ? "Hungry" : "Well fed";
        _foodStatus.GetParent<Control>().TooltipText = $"Work efficiency: {_world.Food.WorkEfficiency:P0}. Villagers eat berries first, then vegetables, then bread.";
        _foodStatus.Modulate = _world.Food.Hunger > 0 ? new("ffd39b") : new("a8bcb0");
        foreach (var (resource, label) in _resourceValues)
            label.Text = (resource switch { Resource.Logs => _world.Stored, Resource.Planks => _world.Planks, Resource.Berries => _world.Food.Berries, Resource.Vegetables => _world.Food.Vegetables, Resource.Grain => _world.Food.Grain, _ => _world.Food.Bread }).ToString();
        _resourceValues[Resource.Logs].GetParent<Control>().TooltipText = $"{_world.ReservedStorage} logs reserved · {_world.Trees.Count(t => t.ClearRequested)} clearing orders · {_world.Trees.Count(t => t.NeedsPlanting && !t.ClearRequested)} trees to plant · {_world.Trees.Count(t => !t.NeedsPlanting && !t.ClearRequested && t.Growth < 1)} growing";
        _resourceValues[Resource.Planks].GetParent<Control>().TooltipText = $"{_world.ReservedPlanks} planks reserved · sawmills aim for stock of 8";
        _objective.Text = _world.Food.SupperComplete ? "A supper to remember.\nKeep enjoying your village." : $"Housing  {_world.Housed} / {_world.Population}\nBread for supper  {Math.Min(_world.SupperCost, _world.Food.Bread)} / {_world.SupperCost}";
        _progress.Value = _world.Food.SupperComplete ? 100 : _world.Housed / (float)_world.Population * 50 + Math.Min(_world.SupperCost, _world.Food.Bread) / (float)_world.SupperCost * 50;
        _supperButton.Disabled = !_world.CanCelebrate;
        _supperButton.Text = _world.Food.SupperComplete ? "Supper complete" : _world.Food.Celebrating ? "Gathering…" : $"Host supper · {_world.SupperCost} loaves";
        for (int i = 0; i < _menuButtons.Count; i++)
        { _menuButtons[i].Modulate = _drawer.Visible && _tabs.CurrentTab == i ? _cream : Colors.White; _menuButtons[i].Text = i == 2 && _world.CanCelebrate ? "Goals · Ready" : MenuNames[i]; }
        foreach (var role in _counts.Keys)
        {
            int count = _world.People.Count(v => v.Role == role); _counts[role].Text = $"{RoleName(role)}s  {count}";
            _allocationButtons[(role, -1)].Disabled = count == 0 || _world.Food.Celebrating; _allocationButtons[(role, 1)].Disabled = count == _world.Population || _world.Food.Celebrating;
        }
        _staffing.Text = $"{_world.People.Count(v => v.Role == Role.Unassigned)} unassigned · {_world.People.Count(v => v.Task == Work.Waiting)} idle\nVillage happiness: {_world.VillageHappiness}/100";
        _buildButton.Text = _placing ? "Cancel preview [Esc]" : $"Place {BuildingName(_buildKind).ToLowerInvariant()}";
        _buildButton.Disabled = _plantTreeButton.Disabled = _clearTreeButton.Disabled = _world.Food.Celebrating;
        _clearTreeButton.Modulate = _placing && _clearingTrees ? _cream : Colors.White;
        foreach (var (kind, b) in _kindButtons) { b.Modulate = _placing && !_decorating && _pathTool == 0 && !_plantingTrees && !_clearingTrees && kind == _buildKind ? _cream : Colors.White; b.Disabled = _world.Food.Celebrating; }
        if (_queueButtons.Count != _world.Cottages.Count) RebuildQueue();
        var selected = _world.Cottages.FirstOrDefault(c => c.Id == _selectedSite);
        _buildingDetails.Visible = selected != null; _personDetails.Visible = selected == null && _selectedPerson >= 0;
        _siteInfo.Text = selected == null ? "" : $"{BuildingName(selected.Kind).ToUpperInvariant()} {selected.Id}\n\n" + (selected.Complete ? selected.Kind switch
        {
            BuildingKind.Stockpile => $"Log storage · {_world.LogsAt(selected.Id)}/{World.StockpileCapacity}\n{_world.ReservedLogsAt(selected.Id)} reserved · {_world.IncomingLogsAt(selected.Id)} arriving\nTarget: {selected.LogTarget} logs\nBuilders and sawyers collect here; haulers balance targets.",
            BuildingKind.Cottage => "2 beds ready", BuildingKind.Lodge => "4 beds ready",
            BuildingKind.Bridge => "Open crossing · no staff\nVillagers can walk across. Keep both banks clear.",
            BuildingKind.Square => $"{_world.People.Count(v => v.LeisureSiteId == selected.Id)}/4 visitors · no staff\nShort breaks between jobs, once per minute.\nHouse everyone and stock {_world.SupperCost} bread, then host supper in Goals. Leave {_world.Population} nearby walkable tiles.",
            BuildingKind.Sawmill => $"1 sawyer slot · batch {selected.SawProgress:P0}\n{selected.InputLogs} logs in · {selected.OutputPlanks} planks out\nStock target: 8 planks",
            BuildingKind.ForagerHut => "2 forager slots\nBerries regrow after picking.",
            BuildingKind.VegetableGarden => $"Vegetables · 1 farmer slot\nCrop {selected.Growth:P0}\n{selected.Harvest} vegetables ripe\n8 food per harvest · eaten directly",
            BuildingKind.Farm => $"Crop {selected.Growth:P0}\n{selected.Harvest} grain ripe", _ => $"Oven: {selected.InputGrain} grain\n{selected.OutputBread} loaves ready"
        } : $"Construction {selected.Construction:P0}\n{selected.Delivered}/{selected.Required} {selected.Material.ToString().ToLowerInvariant()} delivered\n{selected.Incoming} on the way");
        foreach (var b in _priorityButtons) b.Visible = selected != null && !selected.Complete;
        for (int i = 0; i < 3; i++) _priorityButtons[i].Modulate = selected?.Priority == i ? _cream : Colors.White;
        _cancelButton.Visible = selected != null && !selected.Complete;
        foreach (var v in _world.People) { _roster[v.Id].TooltipText = $"{RoleName(v.Role)} · {v.Status}"; _roster[v.Id].Modulate = v.Id == _selectedPerson ? _cream : Colors.White; }
        if (_selectedPerson >= 0)
        {
            var p = _world.People[_selectedPerson]; UpdateHappinessUi(p); _inspect.Text = $"{p.Name.ToUpperInvariant()}\n{RoleName(p.Role)} · {TaskName(p.Task)}\n\n{p.Status}\n\n{(p.Carried == 0 ? "Hands free" : $"Carrying {p.Carried} {p.Cargo.ToString().ToLowerInvariant()}")}";

        }
        UpdateVillageDirectory();
        UpdateStorageControls();
        UpdateBuildDescription();
        _hint.Text = _placing ? (_decorating ? (_removeDecoration ? "Remove decorations · click · Esc finishes" : $"{DecorationName(_decorationKind)} · free · R rotates · Esc finishes") : _pathTool > 0 ? (_pathTool == 1 ? "Paint paths · drag or click · Esc finishes" : "Remove paths · drag or click · Esc finishes") : _clearingTrees ? "Clear trees & stumps · click to mark/cancel · Esc finishes" : _plantingTrees ? "Plant alders · click to mark · Esc finishes" : $"{BuildingName(_buildKind)} · {BuildCost(_buildKind)} · {(_buildKind == BuildingKind.Bridge ? "1 water tile" : _rotated ? "2 × 3" : "3 × 2")} · R rotates · Esc cancels") : "";
        if (_placing) _hint.Text += "\n" + (PointerOverHud(_pointerPosition) ? "Move the pointer onto the map to preview." : _ghostValid ? (_pathTool > 0 ? "Click or drag to edit paths" : _clearingTrees ? ClearingHint() : "Clear spot · click to place") : _placementProblem);
        else if (_uiTime < _noticeUntil) _hint.Text = _notice;
        _hintPanel.Visible = _hint.Text.Length > 0;
        _inspector.Size = new(308, Math.Min(620, _hud.Size.Y - 184));
        UpdateManagementControls();
        UpdateCampaignUi(); UpdateEconomyUi();
    }
}

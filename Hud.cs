using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Collections.Generic;

public partial class Game
{
    private Label _resources = null!, _objective = null!, _hint = null!, _staffing = null!, _inspect = null!, _siteInfo = null!;
    private Button _buildButton = null!, _pauseButton = null!, _speedButton = null!, _resetButton = null!, _assignButton = null!, _cancelButton = null!;
    private readonly Dictionary<Role, Label> _counts = new();
    private readonly Dictionary<(Role, int), Button> _allocationButtons = new();
    private readonly List<Button> _roster = new(), _priorityButtons = new();
    private readonly Dictionary<int, Button> _queueButtons = new();
    private VBoxContainer _queue = null!;
    private ProgressBar _progress = null!;
    private Label _foodStatus = null!;
    private Button _supperButton = null!, _saveButton = null!, _loadButton = null!, _plantTreeButton = null!;
    private TabContainer _tabs = null!;
    private readonly Dictionary<BuildingKind, Button> _kindButtons = new();

    private static string RoleName(Role role) => role.ToString();
    private static Role NextRole(Role role) => (Role)(((int)role + 1) % Enum.GetValues<Role>().Length);
    private static string BuildCost(BuildingKind kind) => kind == BuildingKind.Lodge ? "8 planks · 4 beds" : "6 logs";
    private static string BuildingName(BuildingKind kind) => kind == BuildingKind.ForagerHut ? "Forager hut" : kind.ToString();
    private static string TaskName(Work task) => task switch
    {
        Work.ToTree => "To timber", Work.Chopping => "Logging", Work.ToStockpile => "Hauling",
        Work.ToSapling or Work.PlantingTree => "Planting tree",
        Work.ToSawLogs => "Fetching logs", Work.ToSawmill or Work.Sawing => "Sawing", Work.ToPlanks => "Collecting planks",
        Work.ToMaterials => "Fetching", Work.ToCottage => "Delivering", Work.ToBuild => "To site", Work.Building => "Building",
        Work.ToBush or Work.Foraging => "Foraging", Work.ToFarm or Work.Planting => "Sowing", Work.Harvesting => "Harvesting",
        Work.ToGrain => "Fetching", Work.ToOven or Work.Baking => "Baking", Work.ToBread or Work.ToPantry => "Hauling food",
        Work.ToSupper or Work.Supper => "Supper", _ => "Idle"
    };
    private Button Button(string text, Action pressed, float width = 0)
    {
        var button = new Button { Text = text, CustomMinimumSize = new(width, 34), FocusMode = Control.FocusModeEnum.None };
        button.Pressed += pressed; return button;
    }
    private Label Text(string text, int size = 16, bool wrap = false)
    {
        var label = new Label { Text = text, AutowrapMode = wrap ? TextServer.AutowrapMode.WordSmart : TextServer.AutowrapMode.Off, MouseFilter = Control.MouseFilterEnum.Ignore };
        label.AddThemeFontSizeOverride("font_size", size); return label;
    }
    private VBoxContainer Panel(Control root, bool right)
    {
        var panel = new PanelContainer(); root.AddChild(panel);
        if (right) { panel.AnchorLeft = 1; panel.AnchorRight = 1; panel.OffsetLeft = -318; panel.OffsetRight = -28; }
        else { panel.OffsetLeft = 28; panel.OffsetRight = 318; }
        panel.OffsetTop = 170;
        panel.AddThemeStyleboxOverride("panel", new StyleBoxFlat
        {
            BgColor = new("283c38ef"), CornerRadiusTopLeft = 12, CornerRadiusTopRight = 12, CornerRadiusBottomLeft = 12, CornerRadiusBottomRight = 12,
            ContentMarginLeft = 16, ContentMarginRight = 16, ContentMarginTop = 16, ContentMarginBottom = 16
        });
        var column = new VBoxContainer(); column.AddThemeConstantOverride("separation", right ? 9 : 5); panel.AddChild(column); return column;
    }
    private void MakeUi()
    {
        var layer = new CanvasLayer(); AddChild(layer);
        var root = new Control { MouseFilter = Control.MouseFilterEnum.Ignore }; layer.AddChild(root); root.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
        var title = Text("INLANDERS", 32); title.Position = new(28, 23); title.Modulate = _cream; root.AddChild(title);
        var subtitle = Text("T H E   F I R S T   V I L L A G E   S U P P E R", 12); subtitle.Position = new(31, 68); root.AddChild(subtitle);
        _resources = Text("", 17); _resources.Position = new(30, 105); root.AddChild(_resources);
        _foodStatus = Text("", 16); _foodStatus.Position = new(30, 135); root.AddChild(_foodStatus);
        var left = Panel(root, false);
        var heading = Text("03  /  THE FIRST SUPPER", 17); heading.Modulate = _cream; left.AddChild(heading);
        _objective = Text("", 16, true); _objective.CustomMinimumSize = new(258, 42); left.AddChild(_objective);
        _progress = new ProgressBar { ShowPercentage = false, CustomMinimumSize = new(0, 7) }; left.AddChild(_progress);
        _supperButton = Button("Host supper · 16 loaves", () => { if (_world.BeginSupper()) { _placing = false; RefreshGhost(); Notice("The villagers are gathering for supper."); } }); left.AddChild(_supperButton);
        _tabs = new TabContainer(); left.AddChild(_tabs);
        var workforce = new VBoxContainer { Name = "Workforce" }; workforce.AddThemeConstantOverride("separation", 5); _tabs.AddChild(workforce);
        var construction = new VBoxContainer { Name = "Build" }; construction.AddThemeConstantOverride("separation", 5); _tabs.AddChild(construction);
        left = workforce;
        left.AddChild(Text("WORKFORCE", 13));
        foreach (var role in Enum.GetValues<Role>().Where(r => r != Role.Unassigned))
        {
            var row = new HBoxContainer(); left.AddChild(row);
            _counts[role] = Text(""); _counts[role].SizeFlagsHorizontal = Control.SizeFlags.ExpandFill; row.AddChild(_counts[role]);
            foreach (int change in new[] { -1, 1 })
            {
                var b = Button(change < 0 ? "−" : "+", () => _world.AdjustWorkers(role, change), 34);
                b.TooltipText = change < 0 ? "Unassign one worker; carried goods return to storage." : "Use an unassigned worker, or transfer one from another job.";
                _allocationButtons[(role, change)] = b; row.AddChild(b);
            }
        }
        _staffing = Text("", 13, true); _staffing.CustomMinimumSize = new(234, 32); left.AddChild(_staffing);
        var guide = Text("Forager hut → berries\nFarm → grain → bakery → bread\nMeals: 8 food/day, berries first.\n2 grain → 4 loaves.\nFour cottages house everyone.", 13, true); left.AddChild(guide);
        left = construction;
        var kinds = new GridContainer { Columns = 2 }; left.AddChild(kinds);
        foreach (var kind in Enum.GetValues<BuildingKind>())
        {
            var button = Button(BuildingName(kind), () => { _buildKind = kind; _plantingTrees = false; _placing = true; RefreshGhost(); }, 110);
            button.TooltipText = "Plan a " + BuildingName(kind).ToLowerInvariant() + " · " + BuildCost(kind); kinds.AddChild(button); _kindButtons[kind] = button;
        }
        _buildButton = Button("Plan cottage  ·  6 logs  [B]", () => { if (_placing) _placing = false; else { _plantingTrees = false; _placing = true; } RefreshGhost(); }); left.AddChild(_buildButton);
        _plantTreeButton = Button("Plant alders · free  [T]", ToggleTreePlanting); left.AddChild(_plantTreeButton);
        _plantTreeButton.TooltipText = "Mark open ground or an exhausted stump. Loggers plant before harvesting. Trees grow for 3 days, then yield 8 logs. Esc finishes marking.";
        left.AddChild(Text("CONSTRUCTION QUEUE", 13));
        var scroll = new ScrollContainer { CustomMinimumSize = new(234, 48), HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled }; left.AddChild(scroll);
        _queue = new VBoxContainer { SizeFlagsHorizontal = Control.SizeFlags.ExpandFill }; scroll.AddChild(_queue);
        _siteInfo = Text("Select a building to inspect.", 14, true); _siteInfo.CustomMinimumSize = new(234, 56); left.AddChild(_siteInfo);
        var priorities = new HBoxContainer(); left.AddChild(priorities);
        for (int i = 0; i < 3; i++)
        {
            int priority = i; var b = Button(PriorityNames[i], () => _world.SetPriority(_selectedSite, priority));
            b.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill; b.TooltipText = "Applies to newly claimed work. Existing deliveries finish."; priorities.AddChild(b); _priorityButtons.Add(b);
        }
        _cancelButton = Button("Cancel selected plan", () =>
        {
            if (_world.Cancel(_selectedSite)) { _selectedSite = -1; RefreshSelection(); RebuildQueue(); }
        }); _cancelButton.TooltipText = "Releases claims. Carried materials return to storage; delivered materials remain as salvage for loggers."; left.AddChild(_cancelButton);

        var right = Panel(root, true); var peopleTitle = Text("THE VILLAGERS", 17); peopleTitle.Modulate = _cream; right.AddChild(peopleTitle);
        foreach (var v in _world.People)
        {
            int id = v.Id; var b = Button("", () => _selectedPerson = id);
            b.Alignment = HorizontalAlignment.Left; b.CustomMinimumSize = new(258, 34); b.AddThemeFontSizeOverride("font_size", 14); right.AddChild(b); _roster.Add(b);
        }
        _inspect = Text("", 14, true); _inspect.CustomMinimumSize = new(258, 110); right.AddChild(_inspect);
        _assignButton = Button("", () =>
        {
            var v = _world.People[_selectedPerson]; _world.Assign(v.Id, NextRole(v.Role));
        }); right.AddChild(_assignButton);
        var note = Text("Click a villager to inspect.\nJob changes return carried goods first.", 12, true); note.Modulate = new("bccbbc"); right.AddChild(note);

        var bottom = new HBoxContainer(); root.AddChild(bottom); bottom.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.BottomLeft);
        bottom.OffsetLeft = 28; bottom.OffsetTop = -79; bottom.OffsetBottom = -37;
        _pauseButton = Button("Pause  [Space]", TogglePause, 140); bottom.AddChild(_pauseButton);
        _speedButton = Button("Speed  1×", () => { _speed = _speed == 1 ? 3 : _speed == 3 ? 6 : 1; _speedButton.Text = $"Speed  {_speed}×"; }, 110); bottom.AddChild(_speedButton);
        _resetButton = Button("Start again", Reset, 115); bottom.AddChild(_resetButton);
        _saveButton = Button("Save  [F5]", SaveWorld, 110); bottom.AddChild(_saveButton);
        _loadButton = Button("Load  [F9]", LoadWorld, 110); bottom.AddChild(_loadButton);
        _hint = Text("", 15); root.AddChild(_hint); _hint.Modulate = _cream;
        _hint.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.BottomLeft); _hint.OffsetLeft = 28; _hint.OffsetTop = -117;
        var controls = Text("WASD pan   ·   Wheel zoom   ·   Q/E orbit   ·   B build   ·   T plant trees   ·   R rotate plan   ·   Esc cancel preview", 14); root.AddChild(controls);
        controls.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.BottomLeft); controls.OffsetLeft = 28; controls.OffsetTop = -30;
    }
    private void RebuildQueue()
    {
        foreach (var child in _queue.GetChildren()) { _queue.RemoveChild(child); child.QueueFree(); }
        _queueButtons.Clear();
        foreach (var site in _world.Cottages)
        {
            int id = site.Id; var button = Button("", () => { _selectedSite = id; RefreshSelection(); });
            button.Alignment = HorizontalAlignment.Left; _queue.AddChild(button); _queueButtons[id] = button;
        }
    }
    private void UpdateHud()
    {
        _resources.Text = $"Day {_world.Food.Day} · 8 villagers · {_world.Stored} logs ({_world.ReservedStorage} reserved) · {_world.Planks} planks ({_world.ReservedPlanks} reserved) · {_world.Trees.Count(t => t.NeedsPlanting)} to plant · {_world.Trees.Count(t => !t.NeedsPlanting && t.Growth < 1)} growing";
        _foodStatus.Text = $"Berries {_world.Food.Berries}   ·   Grain {_world.Food.Grain}   ·   Bread {_world.Food.Bread}   /   {(_world.Food.Hunger > 0 ? $"Hungry — work at {_world.Food.WorkEfficiency:P0}" : "Well fed")}";
        _foodStatus.Modulate = _world.Food.Hunger > 0 ? new("ffd39b") : Colors.White;
        _objective.Text = _world.Food.SupperComplete ? "A supper to remember.\nKeep enjoying your village." : $"Shelter: {_world.Housed} / 8 neighbors\nSupper bread: {Math.Min(16, _world.Food.Bread)} / 16";
        _progress.Value = _world.Food.SupperComplete ? 100 : _world.Housed / 8f * 50 + Math.Min(16, _world.Food.Bread) / 16f * 50;
        _supperButton.Disabled = !_world.CanCelebrate;
        _supperButton.Text = _world.Food.SupperComplete ? "The village supper is complete" : _world.Food.Celebrating ? "Gathering for supper…" : "Host supper · 16 loaves";
        _supperButton.TooltipText = "House all eight villagers and store 16 loaves, then gather to celebrate.";
        foreach (var role in _counts.Keys)
        {
            int count = _world.People.Count(v => v.Role == role); _counts[role].Text = $"{RoleName(role)}s   {count}";
            _allocationButtons[(role, -1)].Disabled = count == 0 || _world.Food.Celebrating; _allocationButtons[(role, 1)].Disabled = count == 8 || _world.Food.Celebrating;
        }
        _staffing.Text = $"{_world.People.Count(v => v.Role == Role.Unassigned)} unassigned · {_world.People.Count(v => v.Task == Work.Waiting)} idle\nBuilders haul supplies and construct.";
        _buildButton.Text = _placing ? "Cancel placement  [Esc]" : $"Plan {BuildingName(_buildKind).ToLowerInvariant()}  [B]";
        _buildButton.Disabled = _world.Food.Celebrating;
        _buildButton.TooltipText = BuildCost(_buildKind);
        _plantTreeButton.Disabled = _world.Food.Celebrating;
        _plantTreeButton.Modulate = _placing && _plantingTrees ? _cream : Colors.White;
        foreach (var (kind, button) in _kindButtons) { button.Modulate = !_plantingTrees && kind == _buildKind ? _cream : Colors.White; button.Disabled = _world.Food.Celebrating; }
        if (_queueButtons.Count != _world.Cottages.Count) RebuildQueue();
        foreach (var site in _world.Cottages)
        {
            var button = _queueButtons[site.Id];
            button.Text = $"{(site.Id == _selectedSite ? "›" : " ")} {BuildingName(site.Kind)} {site.Id} · {(site.Complete ? "Ready" : PriorityNames[site.Priority])}";
            button.AddThemeFontSizeOverride("font_size", 14);
            button.Modulate = site.Id == _selectedSite ? _cream : Colors.White;
        }
        var selected = _world.Cottages.FirstOrDefault(c => c.Id == _selectedSite);
        _siteInfo.Text = selected == null ? "Select a building to inspect." :
            $"{BuildingName(selected.Kind).ToUpperInvariant()} {selected.Id}\n" + (selected.Complete ? selected.Kind switch {
                BuildingKind.Cottage => "2 beds ready", BuildingKind.Lodge => "4 beds ready",
                BuildingKind.Sawmill => $"1 sawyer · {selected.SawProgress:P0} · target 8\n{selected.InputLogs} logs in · {selected.OutputPlanks} planks out",
                BuildingKind.ForagerHut => "2 forager slots · berries regrow", BuildingKind.Farm => $"Crop {selected.Growth:P0} · {selected.Harvest} grain ripe", _ => $"Oven: {selected.InputGrain} grain · {selected.OutputBread} loaves" } : $"{selected.Construction:P0} built · {selected.Delivered}/{selected.Required} {selected.Material.ToString().ToLowerInvariant()}\n{selected.Incoming} on the way");
        for (int i = 0; i < 3; i++) { _priorityButtons[i].Disabled = selected == null || selected.Complete; _priorityButtons[i].Modulate = selected?.Priority == i ? _cream : Colors.White; }
        _cancelButton.Disabled = selected == null || selected.Complete;
        foreach (var v in _world.People)
        {
            _roster[v.Id].Text = $"{v.Name}  ·  {RoleName(v.Role)}  ·  {TaskName(v.Task)}";
            _roster[v.Id].TooltipText = v.Status; _roster[v.Id].Modulate = v.Id == _selectedPerson ? _cream : Colors.White;
        }
        var person = _world.People[_selectedPerson];
        _inspect.Text = $"{person.Name.ToUpperInvariant()} · {RoleName(person.Role)}\n{person.Status}\nCargo: {person.Carried} {person.Cargo.ToString().ToLowerInvariant()}\nClaims: {person.Reserved} {person.Cargo.ToString().ToLowerInvariant()} / {person.FoodReserved} grain";
        _assignButton.Text = $"Assign {person.Name}: {RoleName(NextRole(person.Role))}";
        _assignButton.Disabled = _world.Food.Celebrating;
        _hint.Text = _placing ? (_ghostValid ? "Click to plan · R rotates · pale square marks the entrance" : "Keep trees, workers, entrances, and routes accessible") :
            _world.Food.SupperComplete ? "Good food, good neighbors. Keep playing, or save your village." : "Build a forager hut, farm, and bakery. House everyone and save 16 loaves for supper.";
        if (_placing && _plantingTrees) _hint.Text = _ghostValid ? "Click to mark an alder · open ground or empty stump · 3 days to grow · Esc finishes" : "Choose open ground or an empty stump; keep workers and entrances accessible.";
        if (_uiTime < _noticeUntil) _hint.Text = _notice;
    }
}

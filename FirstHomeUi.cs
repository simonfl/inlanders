using Godot;
using Inlanders.Simulation;
using System.IO;
using System.Linq;

public partial class Game
{
    private VBoxContainer _firstHomeGoals = null!;
    private Label _firstHomeStory = null!;
    private Button _firstHomeAction = null!, _firstHomePinnedAction = null!;
    private PanelContainer _firstHomePinned = null!;
    private Label _firstHomePinnedText = null!;
    private VBoxContainer _firstHomeFoodChoices = null!;
    private bool _firstHomeToolsOpen;
    private Button _firstHomeTools = null!;
    private World? _firstHomeObserved;
    private int? _firstHomeNoticed;
    private int _firstHomeMeals;

    private void FirstHomeMenu()
    {
        MenuPage("A first home");
        _mainColumn.AddChild(Text("Marie and Louis have offered Jeanne and Pierre a place to begin. Help the pair make a home and a living on this riverbank.", 17, true));
        _mainColumn.AddChild(Text("Four neighbors. Real materials and meals. Choose where to build and which work to begin. Space pauses or resumes.", 14, true));
        string path = Path.Combine(Path.GetDirectoryName(_creativeSavePath)!, "first-home.json");
        if (File.Exists(path))
            MenuButton("Resume first home", () => MenuAttempt(() => EnterFromMenu(World.LoadFile(path))));
        void Start() => MenuAttempt(() => { var world = World.NewFirstHome(); world.SaveFile(path); EnterFromMenu(world); });
        MenuButton("Begin a first home", () => {
            if (File.Exists(path)) ConfirmMenu("Begin again?", "Replace this first-home village?", Start, FirstHomeMenu);
            else Start();
        });
        MenuButton("Back", ShowMainMenu);
    }

    private void MakeFirstHomeUi(VBoxContainer column)
    {
        _firstHomeGoals = new VBoxContainer();
        column.AddChild(_firstHomeGoals);
        _firstHomeStory = Text("", 16, true);
        _firstHomeGoals.AddChild(_firstHomeStory);
        _firstHomeAction = Button("", ActOnFirstHome);
        _firstHomeGoals.AddChild(_firstHomeAction);
        _firstHomeFoodChoices = new VBoxContainer();
        _firstHomeGoals.AddChild(_firstHomeFoodChoices);
        foreach (var choice in new[] { ("Berry hut · gather nearby berries", BuildingKind.ForagerHut),
            ("Garden · grow vegetables", BuildingKind.VegetableGarden), ("Landing · catch fish", BuildingKind.FishingDock) })
            _firstHomeFoodChoices.AddChild(Button(choice.Item1, () => { _firstHomeFoodChoices.Hide(); CloseManagementUi(); BeginPlacement(choice.Item2); }));
        _firstHomeFoodChoices.Hide();
        _firstHomeTools = Button("Village tools ▸", () => _firstHomeToolsOpen = !_firstHomeToolsOpen);
        _firstHomeGoals.AddChild(_firstHomeTools);
        _firstHomePinned = HudPanel(_hud);
        var pinned = new VBoxContainer(); _firstHomePinned.AddChild(pinned);
        _firstHomePinnedText = Text("", 14, true); pinned.AddChild(_firstHomePinnedText);
        _firstHomePinnedAction = Button("", ActOnFirstHome); pinned.AddChild(_firstHomePinnedAction);
        _firstHomePinned.Hide();
        _firstHomeGoals.Hide();
    }

    private void ActOnFirstHome()
    {
        var goal = _world.ReadFirstHomeObjectives();
        if (_paused && goal.Action is "inspect" or "watch") { TogglePause(); return; }
        switch (goal.Action)
        {
            case "home": CloseManagementUi(); BeginPlacement(BuildingKind.Cottage); break;
            case "grove": BeginWoodlandCare(); break;
            case "food":
                if (!_drawer.Visible || _tabs.CurrentTab != 2) ToggleDrawer(2);
                _firstHomeFoodChoices.Show(); break;
            case "inspect": if (goal.Site is int id) SelectBuilding(id); break;
            case "watch": ShowDailyLife(_world.Founding!.FirstHome!.Served.Contains(2) ? 3 : 2); _followPerson = true; break;
            case "done": ReturnToMainMenu(); break;
        }
    }

    private void UpdateFirstHomeUi()
    {
        var story = _world.Founding?.FirstHome;
        _firstHomeGoals.Visible = story != null;
        _firstHomePinned.Visible = story != null && !_atMainMenu && !_watching && !(_drawer.Visible && _tabs.CurrentTab == 2);
        if (story == null) return;
        if (_firstHomeObserved != _world) _firstHomeToolsOpen = false;
        _firstHomeTools.Text = _firstHomeToolsOpen ? "Village tools ▾" : "Village tools ▸";
        _foundingGoals.Hide();
        _foundingHallGoals.Hide();
        _challengeGoals.Hide();
        _objective.Hide();
        _goalArrival.Hide();
        _goalTitle.Text = "A FIRST HOME";
        var goal = _world.ReadFirstHomeObjectives();
        _menuButtons[2].Text = story.Complete ? "Objectives · complete" : $"Objectives · {goal.Done}/3";
        _menuButtons[2].TooltipText = "Objective and progress [G]";
        if (_tabs.CurrentTab == 2) _drawerTitle.Text = "A first home";
        string progress = $"A home for Jeanne and Pierre\n\n{goal.Home}\n{goal.Food}\n{goal.Meals}\n\n{goal.Hint}";
        _firstHomeStory.Text = _firstHomePinnedText.Text = progress;
        _firstHomePinned.Position = new(_drawer.Visible ? (_inspector.Visible ? 344 : _hud.Size.X - 300) : 16, 92);
        _firstHomePinned.Size = new(284, 0);
        string action = _paused && goal.Action is "inspect" or "watch" ? "Resume village life" : goal.Action switch {
            "home" => "Build their cottage", "grove" => "Work a grove", "food" => "Choose a food source",
            "inspect" => "Inspect current work", "watch" => "Follow the next diner", _ => "Save and leave" };
        _firstHomeAction.Text = _firstHomePinnedAction.Text = action;
        if (goal.Action != "food") _firstHomeFoodChoices.Hide();
        if (_firstHomeObserved == _world)
        {
            if (_firstHomeNoticed != story.NewHomeId && story.NewHomeId != null)
                Notice("Cottage ready. Objectives [G] shows the next step and your progress.");
            if (_firstHomeMeals != story.Served.Count && story.Served.Count > 0)
                Notice(story.Complete ? "All objectives complete. Jeanne and Pierre have a home and fresh meals." : $"Fresh meals: {story.Served.Count}/2 residents. See Objectives [G].");
        }
        _firstHomeObserved = _world;
        _firstHomeNoticed = story.NewHomeId;
        _firstHomeMeals = story.Served.Count;
    }
}

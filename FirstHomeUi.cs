using Godot;
using Inlanders.Simulation;
using System.IO;
using System.Linq;

public partial class Game
{
    private VBoxContainer _firstHomeGoals = null!;
    private sealed record HouseholdCard(Label Title, Label Count, Label Home, Label Food, Label Meals, Label Supply, Label Hint);
    private HouseholdCard _firstHomeCard = null!, _firstHomePinnedCard = null!;
    private Button _firstHomeAction = null!, _firstHomePinnedAction = null!;
    private PanelContainer _firstHomePinned = null!;
    private VBoxContainer _firstHomeFoodChoices = null!;
    private bool _firstHomeToolsOpen;
    private Button _firstHomeTools = null!;
    private World? _firstHomeObserved;
    private int? _firstHomeNoticed;
    private int _firstHomeMeals;
    private int _riverBoatsObserved; private bool _riverClosedObserved;

    private void FirstHomeMenu()
    {
        MenuPage("Household stories");
        MenuButton("A first home", () => FirstHomeMenu(false));
        MenuButton("A lean landing · short provisions", () => FirstHomeMenu(true));
        MenuButton("The broken crossing · recovery", CrossingRecoveryMenu);
        MenuButton("Before the river closes · longer scenario", RiverClosingMenu);
        MenuButton("Back", ShowMainMenu);
    }

    private void FirstHomeMenu(bool lean)
    {
        MenuPage(lean ? "A lean landing" : "A first home");
        _mainColumn.AddChild(Text(lean ? "The crossing cost most of your provisions. Four people, twelve portions and ten logs remain. Fish near the landing or seek food inland; establish a fresh supply, then a home for Jeanne and Pierre." : "Marie and Louis have offered Jeanne and Pierre a place to begin. Help the pair make a home and a living on this riverbank.", 17, true));
        _mainColumn.AddChild(Text("Food runs down as people eat. Missed meals slow work; fresh food lets them recover. Space pauses or resumes.", 14, true));
        string path = Path.Combine(Path.GetDirectoryName(_creativeSavePath)!, lean ? "lean-landing.json" : "first-home.json");
        if (File.Exists(path))
            MenuButton("Resume this story", () => MenuAttempt(() => EnterFromMenu(World.LoadFile(path))));
        void Start() => MenuAttempt(() => { var world = World.NewFirstHome(lean); world.SaveFile(path); EnterFromMenu(world); });
        MenuButton("Begin this story", () => {
            if (File.Exists(path)) ConfirmMenu("Begin again?", "Replace this story's village?", Start, () => FirstHomeMenu(lean));
            else Start();
        });
        MenuButton("Back", FirstHomeMenu);
    }

    private HouseholdCard MakeHouseholdCard(VBoxContainer column)
    {
        column.AddThemeConstantOverride("separation", 8);
        var title = Text("", 17, true); title.Modulate = _cream; column.AddChild(title);
        var count = Text("", 12); count.Modulate = new("a8bcb0"); column.AddChild(count);
        column.AddChild(new HSeparator());
        var home = Text("", 14, true); column.AddChild(home);
        var food = Text("", 14, true); column.AddChild(food);
        var meals = Text("", 14, true); column.AddChild(meals);
        column.AddChild(new HSeparator());
        var supply = Text("", 13, true); column.AddChild(supply);
        var hint = Text("", 14, true); column.AddChild(hint);
        return new(title, count, home, food, meals, supply, hint);
    }

    private void RenderHouseholdCard(HouseholdCard card, FirstHomeObjectives goal)
    {
        card.Title.Text = _world.Founding!.FirstHome!.RiverClosing!=null ? "Before the river closes" : _world.Founding.FirstHome.Recovery ? "The broken crossing" : _world.Founding.FirstHome.LeanLanding ? "A lean landing" : "A home for Jeanne & Pierre";
        card.Count.Text = goal.Done == 3 ? "ALL OBJECTIVES COMPLETE" : $"OBJECTIVES · {goal.Done} OF 3 COMPLETE";
        bool lean = _world.Founding.FirstHome.LeanLanding;
        card.Home.Text = lean ? goal.Food : goal.Home; card.Food.Text = lean ? goal.Home : goal.Food; card.Meals.Text = goal.Meals;
        foreach(var row in new[] { card.Home, card.Food, card.Meals }) row.Modulate = new(row.Text.StartsWith("✓") ? "a9d8ae" : "e9e5d7");
        int food = _world.ReachableEdibleStored, hungry = _world.People.Count(p => !p.Fed);
        card.Supply.Text = hungry > 0 ? $"{hungry} hungry · work slows\n{food} food portions available" : $"{(food <= _world.Population * 2 ? "Low food reserve" : "Food in store")} · {food} portions";
        card.Supply.Modulate = new(hungry > 0 ? "efb192" : food <= _world.Population * 2 ? "f0d397" : "a8bcb0");
        card.Hint.Text = goal.Hint;
    }

    private void MakeFirstHomeUi(VBoxContainer column)
    {
        _firstHomeGoals = new VBoxContainer();
        column.AddChild(_firstHomeGoals);
        _firstHomeCard = MakeHouseholdCard(_firstHomeGoals);
        _firstHomeAction = Button("", ActOnFirstHome);
        _firstHomeGoals.AddChild(_firstHomeAction);
        _firstHomeFoodChoices = new VBoxContainer();
        _firstHomeGoals.AddChild(_firstHomeFoodChoices);
        foreach (var choice in new[] { ("Berry hut", BuildingKind.ForagerHut, "Gather wild berries. Place near berry bushes."),
            ("Garden", BuildingKind.VegetableGarden, "Grow vegetables. The first crop takes time to ripen."),
            ("Landing", BuildingKind.FishingDock, "Catch fish. Needs a dry shore and an open boat route.") })
        {
            var button = Button($"{choice.Item1} · {Buildings.Get(choice.Item2).CostText}", () => { _firstHomeFoodChoices.Hide(); CloseManagementUi(); BeginPlacement(choice.Item2); });
            button.TooltipText = choice.Item3;
            _firstHomeFoodChoices.AddChild(button);
        }
        _firstHomeFoodChoices.Hide();
        MakeRecoveryUi();
        _firstHomeTools = Button("Village tools ▸", () => _firstHomeToolsOpen = !_firstHomeToolsOpen);
        _firstHomeGoals.AddChild(_firstHomeTools);
        _firstHomePinned = HudPanel(_hud);
        var pinned = new VBoxContainer(); _firstHomePinned.AddChild(pinned);
        _firstHomePinnedCard = MakeHouseholdCard(pinned);
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
            case "recover": OpenRecoveryChoices(); break;
            case "advance": if(_world.AdvanceRiverSchedule() && _paused)TogglePause(); break;
            case "grove": BeginWoodlandCare(); break;
            case "food":
                _firstHomeToolsOpen = false;
                if (!_drawer.Visible || _tabs.CurrentTab != 2) { ToggleDrawer(2); _firstHomeFoodChoices.Show(); }
                else _firstHomeFoodChoices.Visible = !_firstHomeFoodChoices.Visible;
                break;
            case "inspect": if (goal.Site is int id) SelectBuilding(id); break;
            case "watch": ShowDailyLife(_world.People.FirstOrDefault(p=>(_world.Founding!.FirstHome!.Recovery || p.Id>=2) && !_world.Founding!.FirstHome!.Served.Contains(p.Id))?.Id ?? 2); _followPerson = true; break;
            case "done": ReturnToMainMenu(); break;
        }
    }

    private void UpdateFirstHomeUi()
    {
        var story = _world.Founding?.FirstHome;
        _firstHomeGoals.Visible = story != null;
        _firstHomePinned.Visible = story != null && !_atMainMenu && !_watching && !(_drawer.Visible && _tabs.CurrentTab == 2);
        if (story == null) return;
        if (_firstHomeObserved != _world) { _firstHomeToolsOpen = false; _recoveryChoices.Hide(); }
        _firstHomeTools.Text = _firstHomeToolsOpen ? "Village tools ▾" : "Village tools ▸";
        _foundingGoals.Hide();
        _foundingHallGoals.Hide();
        _challengeGoals.Hide();
        _objective.Hide();
        _goalArrival.Hide();
        _goalTitle.Hide();
        var goal = _world.ReadFirstHomeObjectives();
        _menuButtons[2].Text = story.Complete ? "Objectives · complete" : $"Objectives · {goal.Done}/3";
        _menuButtons[2].TooltipText = "Objective and progress [G]";
        if (_tabs.CurrentTab == 2) _drawerTitle.Text = "Objectives";
        RenderHouseholdCard(_firstHomeCard, goal);
        RenderHouseholdCard(_firstHomePinnedCard, goal);
        _firstHomePinned.Position = new(_drawer.Visible ? (_inspector.Visible ? 344 : _hud.Size.X - 300) : 16, 92);
        _firstHomePinned.Size = new(284, 0);
        string action = _paused && goal.Action is "inspect" or "watch" ? "Resume village life" : goal.Action switch {
            "home" => _world.Founding!.FirstHome!.RiverClosing!=null?"Prepare another cottage":"Build their cottage", "grove" => "Work a grove", "food" => "Choose a food source", "recover" => "Choose a recovery plan",
            "advance" => _world.Founding!.FirstHome!.RiverClosing!.Boats<2?"Welcome next boat now":"End supply season now", "inspect" => "Inspect current work", "watch" => "Follow the next diner", _ => "Save and leave" };
        if (goal.Action == "food" && _firstHomeFoodChoices.Visible) action = "Hide food choices";
        if (goal.Action == "recover" && _recoveryChoices.Visible) action = "Hide recovery plans";
        _firstHomeAction.Text = _firstHomePinnedAction.Text = action;
        if (goal.Action != "food") _firstHomeFoodChoices.Hide();
        bool choosingFood = _firstHomeFoodChoices.Visible || _recoveryChoices.Visible;
        _firstHomeCard.Hint.Visible = _firstHomeCard.Supply.Visible = _firstHomeTools.Visible = !choosingFood;
        if (_firstHomeObserved == _world)
        {
            if (_firstHomeNoticed != story.NewHomeId && story.NewHomeId != null)
                Notice("Cottage ready. Objectives [G] shows the next step and your progress.");
            if (_firstHomeMeals != story.Served.Count && story.Served.Count > 0)
                Notice(story.Complete ? "All objectives complete. Your neighbors have fresh food again." : $"Fresh meals: {story.Served.Count}/{(story.RiverClosing!=null?_world.Population:story.Recovery?4:2)} residents. See Objectives [G].");
        }
        if(story.RiverClosing is {} river)
        {
            if(_firstHomeObserved==_world && river.Boats!=_riverBoatsObserved) Notice(river.Boats==1?"Four neighbors arrived with 24 food. Prepare for four more." : "Four more neighbors arrived without food. All twelve now depend on the village.");
            if(_firstHomeObserved==_world && river.Closed && !_riverClosedObserved) Notice("The river is closed. The self-sufficiency trial has begun.");
            _riverBoatsObserved=river.Boats;_riverClosedObserved=river.Closed;
        }
        _firstHomeObserved = _world;
        _firstHomeNoticed = story.NewHomeId;
        _firstHomeMeals = story.Served.Count;
    }
}

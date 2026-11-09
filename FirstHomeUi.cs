using Godot;
using Inlanders.Simulation;
using System.IO;
using System.Linq;

public partial class Game
{
    private VBoxContainer _firstHomeGoals = null!;
    private Label _firstHomeStory = null!;
    private Button _firstHomeWatch = null!;
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
        _firstHomeStory = Text("", 17, true);
        _firstHomeGoals.AddChild(_firstHomeStory);
        _firstHomeGoals.AddChild(Button("Work a grove", BeginWoodlandCare));
        _firstHomeGoals.AddChild(Button("Choose their home", () => { CloseManagementUi(); BeginPlacement(BuildingKind.Cottage); }));
        _firstHomeGoals.AddChild(Button("Choose a livelihood", () => OpenFoodBuildings(0)));
        _firstHomeWatch = Button("Find Jeanne", () => { ShowDailyLife(2); _followPerson = true; });
        _firstHomeGoals.AddChild(_firstHomeWatch);
        _firstHomeGoals.AddChild(Button("Save and leave", ReturnToMainMenu));
        _firstHomeGoals.Hide();
    }

    private void UpdateFirstHomeUi()
    {
        var story = _world.Founding?.FirstHome;
        _firstHomeGoals.Visible = story != null;
        if (story == null) return;
        _foundingGoals.Hide();
        _foundingHallGoals.Hide();
        _challengeGoals.Hide();
        _objective.Hide();
        _goalArrival.Hide();
        _goalTitle.Text = "A FIRST HOME";
        _menuButtons[2].Text = story.Complete ? "Jeanne & Pierre · settled" : "Jeanne & Pierre";
        _menuButtons[2].TooltipText = "Their first home [G]";
        if (_tabs.CurrentTab == 2) _drawerTitle.Text = "A first home";
        _firstHomeStory.Text = story.Complete
            ? "A place worth staying.\n\nJeanne and Pierre have a home, and both have eaten from the livelihood you established. Stay with them a while, or save this beginning and leave."
            : story.NewHomeId != null
                ? "A home of their own.\n\nNow let Jeanne and Pierre taste what this land provides. Choose a nearby garden, berry hut or landing. They carry and eat ordinary meals themselves."
                : "Marie and Louis have room to welcome their friends, but Jeanne and Pierre need a home of their own.\n\nChoose ground for their cottage and a livelihood. The stored provisions give you time. Timber comes from the grove you choose to work.";
        if (_firstHomeObserved == _world)
        {
            if (_firstHomeNoticed != story.NewHomeId && story.NewHomeId != null)
                Notice("Jeanne and Pierre have a home. Find them through their story [G].");
            if (_firstHomeMeals != story.Served.Count && story.Served.Count > 0)
                Notice(story.Complete ? "Their first meals from this land. A place worth staying." : "A first meal from the new livelihood. Find Jeanne through [G].");
        }
        _firstHomeObserved = _world;
        _firstHomeNoticed = story.NewHomeId;
        _firstHomeMeals = story.Served.Count;
        _firstHomeWatch.Text = "Find Jeanne · " + _world.People[2].Status;
    }
}

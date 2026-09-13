using Godot;
using Inlanders.Simulation;
using System.IO;
using System.Linq;

public partial class Game
{
    private VBoxContainer _courtExperienceGoals=null!;
    private Button _courtMealPlace=null!,_courtRemovePlace=null!;
    private Button _courtBeforeButton=null!,_courtFinishButton=null!,_courtWatchButton=null!,_courtReopenButton=null!,_courtLeaveButton=null!;
    private Node3D? _courtStartingLayout;
    private bool _courtShowBefore;
    private World? _courtStartingWorld;
    private string CourtStudyPath(bool finite)=>Path.Combine(Path.GetDirectoryName(_creativeSavePath)!,finite?"court-finite.json":"court-open.json");
    private void CourtStartMenu(bool finite)
    {
        string title=finite?"A place to gather":"An open court";
        string path=CourtStudyPath(finite);
        MenuPage(finite?title:"Free arrangement");
        _mainColumn.AddChild(Text(finite?
            "A short introduction: make an everyday meal place, see a neighbor use it, then finish when satisfied. You can keep building and watching afterward.":
            "Sixteen neighbors and a village of your own. Arrange or watch freely, with no assigned project or finish line.",16,true));
        if(File.Exists(path))MenuButton("Resume · "+title,()=>MenuAttempt(()=>EnterFromMenu(World.LoadFile(path))));
        void Start()=>MenuAttempt(()=>{var world=World.NewCourtExperience(finite);world.SaveFile(path);EnterFromMenu(world);});
        MenuButton("New · "+title,()=>{if(File.Exists(path))ConfirmMenu("New · "+title,"Replace this village with a fresh starting court?",Start,()=>CourtStartMenu(finite));else Start();});
        _mainColumn.AddChild(Text("Buildings and moves are free and instant. Residents collect real food, work and rest; missing meals carry no hunger penalty.",15,true));
        _mainColumn.AddChild(Text("New replaces only this village. Continue on the main menu returns to the last village you played.",14,true));
        MenuButton("Back",ShowMainMenu);
    }
    private void MakeCourtExperienceUi(VBoxContainer column)
    {
        _courtExperienceGoals=new();column.AddChild(_courtExperienceGoals);
        _courtMealPlace=Button("Choose a meal place",()=>BeginGatheringPlan(_world.Commons?.Center,true));_courtExperienceGoals.AddChild(_courtMealPlace);
        _courtRemovePlace=Button("Remove meal place",()=>{_world.RemoveCommons();SaveWorld();UpdateHud();});_courtExperienceGoals.AddChild(_courtRemovePlace);
        _courtBeforeButton=Button("Show starting footprints",()=>{_courtShowBefore=!_courtShowBefore;UpdateCourtExperienceUi();});_courtExperienceGoals.AddChild(_courtBeforeButton);
        _courtWatchButton=Button("Watch village life",()=>{_courtShowBefore=false;UpdateCourtExperienceUi();CloseDrawer();ClearSelection();_speed=1;_paused=false;ToggleWatch();});_courtExperienceGoals.AddChild(_courtWatchButton);
        _courtFinishButton=Button("This place is ready",()=>{if(_world.FinishCourtPlace()){_paused=true;_courtShowBefore=false;SaveWorld();UpdateHud();_drawerPages[2].ScrollVertical=0;}});_courtExperienceGoals.AddChild(_courtFinishButton);
        _courtFinishButton.TooltipText="Finish after the shared place has served an ordinary meal. No stock target or attendance streak.";
        _courtReopenButton=Button("Keep shaping this place",()=>{_world.CourtStudy!.Finished=false;SaveWorld();UpdateHud();CloseDrawer();});_courtExperienceGoals.AddChild(_courtReopenButton);
        _courtLeaveButton=Button("Finish here · main menu",ReturnToMainMenu);_courtExperienceGoals.AddChild(_courtLeaveButton);
        _courtExperienceGoals.Hide();
    }
    private void UpdateCourtExperienceUi()
    {
        if(_world.CourtStudy is not {} study)return;
        _neighborhoodGoals.Hide();_journeyAction.Hide();_visitorPanel.Hide();_courtExperienceGoals.Show();
        _goalTitle.Text=study.Finite?(study.Finished?"A place you made":"A place to gather"):"An open court";
        _goalArrival.Text=study.Finite?(study.Finished?"Your gathering place is finished. Stay with the neighbors, leave it here, or reopen it when another idea comes.":"Make an everyday meal place between homes and food. Open room for six seats, or choose a new spot. See a neighbor eat there, then finish when satisfied."):"Make whatever interests you. Follow daily life, change an arrangement, or simply watch. There is no assigned project or finish line.";
        _objective.Visible=!study.Finished;
        if(_tabs.CurrentTab==2)_drawerTitle.Text="Your place";
        _objective.Text=_courtShowBefore?"Amber outlines show the starting building footprints, not current buildings. Hide them to watch daily life.":(_world.Commons?.FirstDiner is int diner?$"{_world.People[diner].Name} ate at your meal place. Ordinary life continues.":_world.Commons!=null?"Place ready. Play to let neighbors bring their next meal. Food must be nearby.":study.Finite?"Choose a meal place to preview real seats. Select a home → Move home to open space.":"Build, move or remove whatever you choose. Select residents to follow daily life.");
        _courtBeforeButton.Text=_courtShowBefore?"Hide starting footprints":"Show starting footprints";
        if(_courtStartingLayout!=null)_courtStartingLayout.Visible=_courtShowBefore && !_watching && !_atMainMenu;
        _courtFinishButton.Visible=study.Finite && !study.Finished;
        _courtFinishButton.Disabled=_world.FinishCourtPlaceProblem()!=null;
        _courtFinishButton.TooltipText=_world.FinishCourtPlaceProblem()??"The place has served a neighbor. Finish when satisfied.";
        _courtMealPlace.Text=_world.Commons==null?(study.Finite?"Choose a meal place":"Meal place tool"):"Move meal place";
        _courtRemovePlace.Visible=_world.Commons!=null && !study.Finished;
        _courtMealPlace.Visible=!study.Finished;
        _courtReopenButton.Visible=study.Finite && study.Finished;
        _courtLeaveButton.Visible=study.Finite && study.Finished;
        _menuButtons[2].Text=study.Finished?"Your place · Finished":"Your place";
        _menuButtons[2].TooltipText="Your place [G] · Drag to pan · Wheel zoom · Q/E orbit · Space play/pause";
    }
    private void MakeCourtStartingLayout()
    {
        if(_courtStartingWorld!=_world)_courtShowBefore=false;
        _courtStartingWorld=_world;_courtStartingLayout=null;
        if(_world.CourtStudy is not {} study)return;
        _courtStartingLayout=new(){Visible=_courtShowBefore};_dynamic.AddChild(_courtStartingLayout);
        foreach(var building in study.StartingBuildings)
        {
            var footprint=World.Footprint(building.Cell,building.Rotation,building.Kind).ToHashSet();
            foreach(var c in footprint)foreach(var step in new[]{new Cell(1,0),new(-1,0),new(0,1),new(0,-1)})
            {
                if(footprint.Contains(new(c.X+step.X,c.Z+step.Z)))continue;
                Box(_courtStartingLayout,new(c.X+step.X*.48f,Height(c.X,c.Z)+.12f,c.Z+step.Z*.48f),new(step.X==0?1:.055f,.04f,step.Z==0?1:.055f),new("f4cb79"));
            }
        }
    }
}

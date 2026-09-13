using Godot;
using Inlanders.Simulation;
using System;
using System.IO;
using System.Linq;

public partial class Game
{
    private string _neighborhoodPath="saves/neighborhood.json";
    private VBoxContainer _neighborhoodGoals=null!;
    private Button _neighborhoodCommit=null!,_neighborhoodVenue=null!;
    private Button _neighborhoodHome=null!,_studyDetailsButton=null!;
    private bool _studyGoalDetails,_lastCompactGoals;
    private bool CompactNeighborhoodGoals=>_storybookScene && _world.Neighborhood?.Complete==true && !_studyGoalDetails;
    private void NeighborhoodMenu()
    {
        MenuPage("A new neighborhood");
        _mainColumn.AddChild(Text("Choose a crossing and make an eastern home for four neighbors. New neighborhood uses the original river map; Try landing and meadow offers a compact landing and a larger clearing farther away. Both use the same food rules. Starting either replaces this experiment's save. All buildings remain available.",15,true));
        if(File.Exists(_neighborhoodPath))MenuButton("Resume neighborhood",()=>MenuAttempt(()=>EnterFromMenu(World.LoadFile(_neighborhoodPath))));
        MenuButton("New neighborhood",()=>MenuAttempt(()=>{var world=World.NewNeighborhoodExperiment();world.SaveFile(_neighborhoodPath);EnterFromMenu(world);}));
        MenuButton("Try landing and meadow",()=>MenuAttempt(()=>{var world=World.NewNeighborhoodLandscapeExperiment();world.SaveFile(_neighborhoodPath);EnterFromMenu(world);}));
        MenuButton("Play original river level",()=>OpenMenuCampaign(6,false));
        MenuButton("Back",ShowMainMenu);
    }
    private void MakeNeighborhoodGoals(VBoxContainer column)
    {
        _neighborhoodGoals=new();column.AddChild(_neighborhoodGoals);
        _neighborhoodCommit=Button("Welcome four neighbors",()=>{if(_world.InviteNewcomers()){SaveWorld();UpdateHud();}else Notice(_world.InvitationProblem()??"Not ready yet.");});_neighborhoodGoals.AddChild(_neighborhoodCommit);
        _neighborhoodHome=Button("Plan east-bank homes",()=>{CloseDrawer();BeginPlacement(BuildingKind.Cottage);});_neighborhoodGoals.AddChild(_neighborhoodHome);
        _neighborhoodVenue=Button("Find a gathering place",()=>{
            var venue=_world.Cottages.FirstOrDefault(c=>c.Id==_world.Neighborhood?.VenueId)??_world.Cottages.FirstOrDefault(c=>c.Cell.X>5 && c.Complete && !c.DemolitionRequested && Buildings.Get(c.Kind).RecreationSlots>0);
            if(venue!=null){CloseDrawer();SelectBuilding(venue.Id);_focus=OnGround(venue.Cell.X,venue.Cell.Z);UpdateCamera();}
            else {CloseDrawer();BeginPlacement(BuildingKind.SeatingGarden);}
        });_neighborhoodGoals.AddChild(_neighborhoodVenue);
        _neighborhoodGoals.AddChild(Button("Keep watching the village",CloseDrawer));
        _studyDetailsButton=Button("Show village details",()=>{_studyGoalDetails=!_studyGoalDetails;UpdateHud();LayoutHud();});_neighborhoodGoals.AddChild(_studyDetailsButton);_studyDetailsButton.Hide();
        _neighborhoodGoals.Hide();
    }
    private void UpdateNeighborhoodGoals()
    {
        var n=_world.Neighborhood!;
        _goalDashboard.Hide();_campaignControls.Hide();_standaloneGuide.Hide();_supperButton.Hide();_supperBreadLink.Hide();_progress.Hide();
        _trackedGoalPanel.Hide();
        _goalArrival.Show();_objective.Show();_neighborhoodGoals.Show();
        _goalTitle.Text=n.Complete?"A neighborhood to call home":"A new neighborhood";
        _goalArrival.Text=n.Complete?"The newcomers have settled in. Keep shaping the village, or compare another approach from the main menu.":"Make an eastern home for four neighbors, then share a welcome meal at a square, hall or seating garden. All buildings are available. Shared workers take available jobs; dedicate residents at workplaces when needed.";
        _objective.Text=_world.NeighborhoodStatus;
        _neighborhoodHome.Visible=!CompactNeighborhoodGoals;
        _studyDetailsButton.Visible=_storybookScene && n.Complete;
        _studyDetailsButton.Text=_studyGoalDetails?"Hide village details":"Show village details";
        if(CompactNeighborhoodGoals)
        {
            _goalTitle.Text="Welcome shared";
            _goalArrival.Text="The newcomers have settled in. Village life continues.";
            _objective.Text=$"{_world.Housed}/{_world.Population} housed · {_world.People.Count(p=>p.Fed)}/{_world.Population} fed";
            _visitorPanel.Hide();
        }
        _neighborhoodCommit.Disabled=_world.InvitationProblem()!=null;
        _neighborhoodCommit.TooltipText=_world.InvitationProblem()??"Four neighbors arrive after 90 seconds, even without homes or food. This commitment happens once.";
        _neighborhoodCommit.Visible=n.CommittedAt==null;
        if(_lastCompactGoals!=CompactNeighborhoodGoals){_lastCompactGoals=CompactNeighborhoodGoals;LayoutHud();}
        _neighborhoodVenue.Text=n.VenueId!=null?"Inspect welcome table":"Choose a gathering place";
        _menuButtons[2].Text=n.Complete?"Goals · Complete":"Goals · Neighborhood";
        _menuButtons[2].TooltipText="Newcomer homes and the shared welcome meal [G]";
        if(n.Complete && !_completionAnnounced){_completionAnnounced=true;SaveWorld();Notice("The neighborhood is complete. Everyone shared the welcome, and the newcomers have homes. Keep enjoying your village.");}
    }
}

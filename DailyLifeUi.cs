using Godot;
using Inlanders.Simulation;
using System.Linq;

public partial class Game
{
    private PanelContainer _dailyCard=null!;
    private Label _dailyText=null!;
    private bool _dailyExpanded;
    private Button _dailySource=null!,_dailyMove=null!,_dailyRestore=null!,_dailyFollow=null!,_dailyDetails=null!;
    private Line2D _dailyRoute=null!;
    private int _dailyPerson=-1;
    private World? _dailyWorld;
    private DailyJourney? _dailyJourney;
    private float _nextDaily;
    private string? _dailyRestoreProblem;
    private void RestoreTrialInWorld()
    {
        if(!_world.RestoreArrangement()){Notice(_world.RestoreArrangementProblem()??"Cannot restore here.");return;}
        CreateActors();RenderActors(0);RebuildQueue();_nextDaily=0;
        Notice("Original building position restored. Village time and daily life continue.");
    }
    private void MakeDailyLifeUi()
    {
        _dailyRoute=new(){Width=3,DefaultColor=new("edc57c"),Antialiased=true};_hud.AddChild(_dailyRoute);
        _dailyCard=HudPanel(_hud);var column=new VBoxContainer();_dailyCard.AddChild(column);
        var row=new HBoxContainer();column.AddChild(row);
        row.AddChild(Button("Next resident",()=>ShowDailyLife((_dailyPerson+1)%_world.Population)));
        row.AddChild(Button("Journey",()=>{_dailyExpanded=!_dailyExpanded;}));
        _dailyDetails=Button("Details",()=>{int id=_dailyPerson;_dailyPerson=-1;SelectPerson(id);ShowInspector();});row.AddChild(_dailyDetails);
        row.AddChild(Button("×",()=>{_dailyPerson=-1;ClearSelection();}));
        _dailyText=Text("",14,true);_dailyText.CustomMinimumSize=new(330,0);column.AddChild(_dailyText);
        var actions=new HBoxContainer();column.AddChild(actions);
        _dailySource=Button("Inspect food",()=>{if(_dailyJourney?.Source is int id){_dailyPerson=-1;SelectBuilding(id);ShowInspector();}else{var at=_dailyJourney?.Route.LastOrDefault()??_world.YardAccess;_focus=OnGround(at.X,at.Z);UpdateCamera();}});actions.AddChild(_dailySource);
        _dailyMove=Button("Try home elsewhere",()=>{var p=_world.People[_dailyPerson];if(p.HomeId is not int id)return;_selectedSite=id;_selectedPerson=-1;BeginRelocation();});actions.AddChild(_dailyMove);
        _dailyFollow=Button("Follow",()=>_followPerson=!_followPerson);actions.AddChild(_dailyFollow);
        _dailyRestore=Button("Restore trial building",RestoreTrialInWorld);column.AddChild(_dailyRestore);_dailyCard.Hide();
    }
    private void ShowDailyLife(int id)
    {
        CloseDrawer();_inspector.Hide();_selectedPerson=id;_selectedSite=-1;_dailyPerson=id;_nextDaily=0;RefreshSelection();
    }
    private void RenderDailyLife()
    {
        if(_dailyWorld!=_world){_dailyWorld=_world;_dailyPerson=-1;_dailyJourney=null;_nextDaily=0;}
        bool show=(_world.IsArrangementCourt || _world.Founding!=null) && _dailyPerson>=0 && !_atMainMenu && !_watching && !_placing && !_drawer.Visible && !_inspector.Visible;
        _dailyCard.Visible=show;_dailyRoute.Visible=show;if(!show)return;
        var p=_world.People[_dailyPerson];
        if(_uiTime>=_nextDaily){_nextDaily=_uiTime+.5f;_dailyJourney=_world.ReadDailyJourney(p.Id);_dailyRestoreProblem=_world.RestoreArrangementProblem();}
        var j=_dailyJourney;if(j==null)return;
        _dailyText.Text=$"{p.Name} · {j.Heading}\n"+(_dailyExpanded?j.Detail:p.Status);
        _dailyText.TooltipText=_dailyExpanded?"":"Journey expands the current activity; Details opens the full resident inspector.";
        _dailyMove.Text=_world.Creative || _world.Founding!=null?"Move home":"Try home elsewhere";
        _dailySource.Text=j.InspectLabel;_dailyFollow.Text=_followPerson?"Unfollow":"Follow";
        _dailySource.Disabled=!j.CanInspect || j.Source==null && j.Route.Length==0;
        _dailyMove.Disabled=p.HomeId is not int home || _world.RelocationProblem(home)!=null;
        _dailyMove.TooltipText=p.HomeId is int homeId?(_world.RelocationProblem(homeId)??(_world.Creative || _world.Founding!=null?"Move this home freely. Residents keep their home assignment.":"Try this home in another place. Restore its position from this card; daily life keeps progressing.")):"This resident has no home.";
        _dailyRestore.Visible=_world.Neighborhood?.Arrangement?.BuildingId!=null;
        _dailyRestore.TooltipText=_dailyRestoreProblem??"Restore the original location. Food, time and work are not rewound.";
        _dailyCard.Size=new(354,0);
        // Keep the selected resident and destination visible from either bank/camera side.
        var anchors=new[]{_camera.UnprojectPosition(OnGround(p.Position.X,p.Position.Y,.25f))}
            .Concat(j.Route.TakeLast(1).Select(c=>_camera.UnprojectPosition(OnGround(c.X,c.Z,.25f)))).ToArray();
        var corners=new[]{new Vector2(_hud.Size.X-370,92),new(16,92),new(_hud.Size.X-370,_hud.Size.Y-86-_dailyCard.Size.Y),new(16,_hud.Size.Y-86-_dailyCard.Size.Y)};
        _dailyCard.Position=corners.OrderBy(c=>anchors.Count(a=>new Rect2(c,_dailyCard.Size).Grow(30).HasPoint(a))*100+
            _world.Cottages.Count(s=>new Rect2(c,_dailyCard.Size).HasPoint(_camera.UnprojectPosition(OnGround(s.Cell.X,s.Cell.Z))))).First();
        _dailyRoute.DefaultColor=new(j.Claimed?"edc57c":"89c7cd");
        _dailyRoute.Points=new[]{_camera.UnprojectPosition(OnGround(p.Position.X,p.Position.Y,.25f))}.Concat(j.Route.Select(c=>_camera.UnprojectPosition(OnGround(c.X,c.Z,.25f)))).ToArray();
    }
}

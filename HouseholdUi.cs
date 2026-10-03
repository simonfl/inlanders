using Godot;
using Inlanders.Simulation;
using System.Linq;
public partial class Game
{
    private GridContainer _householdPeople=null!;
    private readonly Button[] _householdResidents=new Button[4];
    private int _householdOrigin=-1;
    private World? _householdOriginWorld;
    private Button _dailyHomeBack=null!;
    private void MakeHouseholdUi(VBoxContainer column)
    {
        _householdPeople=new(){Columns=2};column.AddChild(_householdPeople);
        for(int i=0;i<4;i++)
        {
            var button=Button("",()=>{});button.CustomMinimumSize=new(148,45);button.AddThemeFontSizeOverride("font_size",13);_householdResidents[i]=button;_householdPeople.AddChild(button);
            button.Pressed+=()=>{if(!button.HasMeta("person"))return;int person=(int)button.GetMeta("person");int origin=_workCardSite;_workCardSite=-1;ShowDailyLife(person,origin);_dailyExpanded=true;_followPerson=true;};
        }
        _householdChange=Button("Choose another home",()=>{_householdFrom=_workCardSite;_householdTo=-1;_householdMoveWorld=_world;});column.AddChild(_householdChange);MakeHouseholdMoveUi();
    }
    private void RenderHouseholdUi(Cottage home)
    {
        bool show=_world.PublicPlace!=null && home.Complete && Buildings.Get(home.Kind).Beds>0 && _yardPreviewSide<0 && _placeJourneySite!=home.Id;
        _householdPeople.Visible=show && !_homeOptions;_householdChange.Visible=show && _homeOptions && _homeMore && _world.People.Any(p=>p.HomeId==home.Id);if(!show)return;
        var residents=_world.People.Where(p=>p.HomeId==home.Id).ToArray();
        for(int i=0;i<4;i++)
        {
            var button=_householdResidents[i];button.Visible=i<residents.Length;if(i>=residents.Length)continue;
            var p=residents[i];string activity=p.Task==Work.EatingMeal?"Eating":p.Task==Work.Resting?"Resting":p.Task==Work.Leisure?"Taking a break":p.Route.Count>0?"Walking":p.Task==Work.Waiting?(_world.QuietSharedPlace(p)!=null?"At shared ground":_world.AvailableAtHome(p)?"At home":"Available"):"Working";
            button.Text=p.Name+"\n"+activity;button.TooltipText=p.Status+". Select to follow the actual resident and current journey.";button.SetMeta("person",p.Id);
        }
    }
}

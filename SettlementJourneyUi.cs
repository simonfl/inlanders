using Godot;
using Inlanders.Simulation;
using System.Linq;

public partial class Game
{
    private Button _journeyAction=null!,_staySettlement=null!;
    private int _journeyStep;
    private void MakeSettlementJourney()
    {
        _journeyAction=Button("Plan a crossing",()=>
        {
            switch(_journeyStep)
            {
                case 0: CloseDrawer();BeginPlacement(BuildingKind.Bridge);break;
                case 1: CloseDrawer();SelectBuilding(_world.Cottages.First(c=>c.Kind==BuildingKind.Bridge).Id);break;
                case 2: CloseDrawer();BeginPlacement(BuildingKind.Cottage);break;
                case 3: OpenEconomy();break;
                case 4:
                    var venue=_world.Cottages.FirstOrDefault(c=>c.Complete && c.Cell.X>5 && Buildings.Get(c.Kind).RecreationSlots>0);
                    if(venue!=null){CloseDrawer();SelectBuilding(venue.Id);}else{CloseDrawer();BeginPlacement(BuildingKind.SeatingGarden);}break;
            }
        });
        var column=_objective.GetParent();column.AddChild(_journeyAction);column.MoveChild(_journeyAction,_objective.GetIndex());
        _neighborhoodGoals.MoveChild(_nextSettlement,0);_neighborhoodGoals.MoveChild(_staySettlement,1);
    }
    private void UpdateSettlementJourney()
    {
        var n=_world.Neighborhood!;
        _journeyAction.Hide();_staySettlement.Visible=n.Complete;
        _staySettlement.Text="Stay and reshape the village";
        _nextSettlement.Text="Finish & choose another settlement";
        if(n.Complete)
        {
            _goalTitle.Text=n.FoodLandChallenge?"Meadow settlement complete":"Welcome complete";
            _goalArrival.Text="You made homes and shared a welcome. This settlement is complete. Finish here, or stay to arrange and watch ordinary village life. Nothing else is required.";
            return;
        }
        if(n.FoodLandChallenge)
        {
            _goalTitle.Text="The meadow · a supply challenge";
            return;
        }
        _goalTitle.Text="A new neighborhood · guided opening";
        _neighborhoodHome.Hide();
        int beds=_world.Cottages.Where(c=>c.Cell.X>5 && !c.DemolitionRequested).Sum(c=>Buildings.Get(c.Kind).Beds);
        bool crossing=_world.Cottages.Any(c=>c.Kind==BuildingKind.Bridge && c.Complete && !c.DemolitionRequested);
        bool buildingCrossing=_world.Cottages.Any(c=>c.Kind==BuildingKind.Bridge && !c.DemolitionRequested);
        _journeyStep=!crossing?(buildingCrossing?1:0):_world.Food.Hunger>0?3:beds<_world.NeighborhoodArrivals?2:n.VenueId==null?4:5;
        string advice=_journeyStep switch {
            0=>"Start with a crossing. Choose a river tile with clear access on both banks; R rotates the bridge.",
            1=>"Your crossing is being built. Shared workers fetch timber and build it. You can plan homes while they work.",
            2=>"Choose homes on the east bank. Cottages and lodges both work; place them where you want the neighborhood to grow.",
            3=>"Some residents missed meals. Inspect food sources and journeys in Economy; restore production or bring food closer.",
            4=>"Choose a place to welcome people. A seating garden, square or hall on the east bank can host it. Select the finished place to set the welcome table.",
            _=>n.CommittedAt==null?"Ready to invite? Four people will arrive in 90 simulation seconds. Keep food working; preparations can continue while they travel.":"People bring food to the welcome table in small groups. Keep homes and food reachable; you can inspect the table below."};
        _goalArrival.Text=advice+"\nAll buildings remain available; choose your own order.";
        if(n.CommittedAt==null)_objective.Text="Invite when you choose. Newcomers arrive after 90 simulation seconds.";
        _journeyAction.Visible=_journeyStep<5;
        _journeyAction.Text=_journeyStep switch {0=>"Plan a crossing",1=>"Inspect crossing work",2=>"Plan east-bank homes",3=>"Find the food problem",4=>"Choose a welcome place",_=>""};
    }
}

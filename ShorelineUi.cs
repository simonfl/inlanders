using Godot;
using Inlanders.Simulation;

public partial class Game
{
    private void UpdateShorelineGoals()
    {
        var n=_world.Neighborhood!;
        _visitorPanel.Hide();
        _goalTitle.Text=_world.IsArrangementCourt?"Willow court · optional welcome":"Willow inlet · make it your village";
        _goalArrival.Text=(_world.Creative?"Free arrangement: instant construction, unrestricted moves and no hunger penalties. Real meals still use food. Welcome more residents if you want a busier village.\n":"")+"Eight neighbors want to join. Homes cluster west; working gardens are across the inlet. Follow the footway, or change how village life fits together. Shared workers take available jobs.";
        _objective.Text=$"Newcomers housed: {_world.NewNeighborsHoused}/8 · homes on either shore\nWelcome shared: {n.Welcomed.Count}/16\n"+
            (n.CommittedAt==null?"Invite when you choose · eight arrive in 90 simulation seconds.":n.Arrived?"Eight newcomers have arrived.":$"Arrival in {System.Math.Max(0,90-(_world.Food.Time-n.CommittedAt.Value)):F0} simulation seconds.")+"\n"+
            (_world.ShorelineFoodProblem()??"Food is available at an open producer. No stockpile target.");
        _neighborhoodHome.Text="Plan homes · either shore";
        _neighborhoodVenue.Text=n.VenueId==null?"Inspect the inherited meeting place":"Inspect welcome table";
        _journeyAction.Visible=true;_journeyAction.Text="See food and journeys";_journeyStep=6;
        _menuButtons[2].Text=_world.IsArrangementCourt?"Goals · optional welcome":"Goals · Willow inlet";
        _menuButtons[2].TooltipText="Homes anywhere, a shared welcome and reachable food [G]";
    }
}

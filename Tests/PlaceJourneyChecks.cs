using Inlanders.Simulation;
static class PlaceJourneyChecks
{
    public static void Run()
    {
        var w=new HamletProfile(false,true,false,true).Create();bool work=false,meal=false,home=false;
        for(int i=0;i<6000 && !(work&&meal&&home);i++)
        {
            w.Tick(.1f);
            foreach(var site in w.Cottages)
            {
                var trips=w.ReadPlaceJourneys(site.Id);
                foreach(var trip in trips)
                {
                    var p=w.People[trip.Person];if(!trip.Steps.SequenceEqual(p.Route) || trip.Position!=p.Position || trip.Amount!=p.Carried)throw new Exception("Journey differs from actual committed trip");
                    bool sample=(!meal && trip.Relation=="Meal from this place") || (!work && trip.Relation=="Work at this place") || (!home && trip.Relation=="Household trip");
                    if(sample){string before=w.SaveJson();trip.Steps[0]=new(999,999);if(before!=w.SaveJson())throw new Exception("Query exposes mutable simulation routes");}
                    meal|=trip.Relation=="Meal from this place";work|=trip.Relation=="Work at this place";home|=trip.Relation=="Household trip";
                }
            }
        }
        if(!(work&&meal&&home) || w.ReadPlaceJourneys(-1).Length!=0)throw new Exception("Missing actual place relationship");
        w.Validate();Console.WriteLine("PASS place journeys: real work, meals and household trips; detached committed paths/cargo; no hypothetical trip.");
    }
}

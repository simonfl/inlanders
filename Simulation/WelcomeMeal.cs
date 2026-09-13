using System;
using System.Linq;

namespace Inlanders.Simulation;

public sealed partial class World
{
    private bool IsWelcomeStore(Cottage c)=>Neighborhood!=null && Buildings.Get(c.Kind).RecreationSlots>0;
    private bool IsFoodStore(Cottage c)=>c.Kind==BuildingKind.Pantry || IsWelcomeStore(c) || IsWorkplaceFoodStore(c);
    public string FoodStoreName(int? id)=>id is int n?IsWelcomeStore(Cottages.Single(c=>c.Id==n))?$"Welcome table {n}":IsWorkplaceFoodStore(Cottages.Single(c=>c.Id==n))?$"{Buildings.Get(Cottages.Single(c=>c.Id==n).Kind).Name} {n} store":$"Pantry {n}":"Central pantry";
    public string? WelcomeVenueProblem(int id)
    {
        if(Neighborhood==null)return "Welcome meals belong to the neighborhood experiment.";
        if(Neighborhood.Complete)return "The neighborhood has already shared its welcome.";
        var site=Cottages.FirstOrDefault(c=>c.Id==id);
        if(site==null || !site.Complete || site.DemolitionRequested || !IsWelcomeStore(site) || site.Cell.X<=5)return "Choose a finished square, hall or seating garden on the east bank.";
        if(!Accessible(site.Entrance))return "Open a route to this gathering place.";
        return null;
    }
    public bool ChooseWelcomeVenue(int id)
    {
        if(WelcomeVenueProblem(id)!=null)return false;
        if(Neighborhood!.VenueId==id)return true;
        if(Neighborhood.VenueId is int old)ClosePantry(old);
        Neighborhood.VenueId=id;_retry=0;History.Add("Preparing a welcome meal here. Shared workers bring edible food; residents visit for their meals after the newcomers arrive.");return true;
    }
    private int WelcomeNeed(Cottage venue)
    {
        if(Neighborhood is not {} n || n.VenueId!=venue.Id || n.Complete)return 0;
        int participants=n.Arrived?Population:12;
        int carrying=People.Count(p=>p.Meal is {Welcome:true,Carrying:true} r && r.SourceId==venue.Id);
        return Math.Max(0,participants-n.Welcomed.Count-carrying-venue.PantryFood.Sum()-FoodIncoming(venue.Id));
    }
    private bool ClaimWelcomeDelivery(Villager person)
    {
        if(Neighborhood?.VenueId is not int id)return false;
        var venue=Cottages.Single(c=>c.Id==id);
        int need=Math.Min(WelcomeNeed(venue),PantrySpace(id));if(need<=0)return false;
        int waiting=People.Count(p=>p.Meal is {Reserved:false,Carrying:false,Eaten:false,Closed:false});
        foreach(var source in FoodStores().OrderBy(s=>TravelCost(At(person),FoodAccess(s))+TravelCost(FoodAccess(s),venue.Entrance)).ThenBy(s=>s??-1))
        {
            if(!Accessible(FoodAccess(source)))continue;
            int surplus=Math.Max(0,EdibleKinds.Sum(k=>FoodAvailableAt(source,k))-waiting);
            if(surplus==0)continue;
            var kind=EdibleKinds.OrderByDescending(k=>FoodAvailableAt(source,k)).First();
            int amount=Math.Min(4,Math.Min(need,Math.Min(surplus,FoodAvailableAt(source,kind))));
            if(amount<=0)continue;
            person.Cargo=kind;person.PantryReserved=amount;person.FoodSourceId=source;person.FoodDestinationId=id;person.FoodTransfer=true;
            Go(person,FoodAccess(source),Work.ToFoodPickup,$"Collecting {amount} {kind} for the welcome meal");return true;
        }
        return false;
    }
    private int? WelcomeMealSource(Villager person)
    {
        if(Neighborhood is not {Arrived:true,Complete:false,VenueId:int id} n || n.Welcomed.Contains(person.Id))return null;
        var venue=Cottages.Single(c=>c.Id==id);
        int visiting=People.Count(p=>p.Meal is {Welcome:true} r && (r.Reserved || r.Carrying) && r.SourceId==id);
        return visiting<Buildings.Get(venue.Kind).RecreationSlots?id:null;
    }
    private void RecordWelcomeMeal(Villager person,MealRequest meal)
    {
        if(meal.Welcome && Neighborhood is {Arrived:true} n && meal.SourceId==n.VenueId && n.Welcomed.Add(person.Id))
            History.Add($"{person.Name} shared the welcome meal ({n.Welcomed.Count}/{Population}).");
    }
    public int NewNeighborsHoused=>People.Skip(8).Count(p=>p.HomeId is int id && Cottages.Any(c=>c.Id==id && c.Cell.X>5 && !c.DemolitionRequested));
    public string WelcomeStatus
    {
        get
        {
            if(Neighborhood is not {} n)return "";
            if(n.Complete)return "A neighborhood to call home. Everyone shared the welcome meal, and the four newcomers have east-bank homes.";
            string status=$"Welcome meal: {n.Welcomed.Count}/{(n.Arrived?Population:12)} neighbors have eaten here.\nNewcomers with east-bank homes: {NewNeighborsHoused}/4.\n";
            if(n.VenueId is not int id)return status+"Choose an eastern square, hall or seating garden. Helpers carry any edible food there.";
            var venue=Cottages.Single(c=>c.Id==id);
            status+=$"{venue.PantryFood.Sum()} portions ready · {FoodIncoming(id)} on the way. ";
            if(!n.Arrived)return status+"Visits begin when the newcomers arrive.";
            if(n.Welcomed.Count==Population)return status+"The meal is shared. Finish eastern homes for the newcomers.";
            var blocked=People.FirstOrDefault(p=>!n.Welcomed.Contains(p.Id) && FindPath(At(p),venue.Entrance,Blocked)==null);
            if(blocked!=null)return status+$"{blocked.Name} cannot reach this table. Reopen the approach or crossing.";
            if(venue.PantryFood.Sum()==0 && FoodIncoming(id)==0)return status+"Waiting for food. Keep production working and shared workers available to carry portions.";
            return status+"Residents visit between jobs when their next meal is due. Keep food and the approach available.";
        }
    }
}

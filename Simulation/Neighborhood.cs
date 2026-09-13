using System;
using System.Linq;

namespace Inlanders.Simulation;

public sealed class NeighborhoodProgress
{
    public bool WorkplaceFood { get; set; }
    public bool FoodLandChallenge { get; set; }
    // Saved comparison rule; the control retains the existing village-wide hunger penalty.
    public bool HungerSlowsActivity { get; set; } = true;
    public int? VenueId { get; set; }
    public System.Collections.Generic.HashSet<int> Welcomed { get; set; } = new();
    public bool Complete { get; set; }
    public Cell? Destination { get; set; }
    public float? CommittedAt { get; set; }
    public bool Arrived { get; set; }
}

public sealed partial class World
{
    public NeighborhoodProgress? Neighborhood { get; private set; }
    public int NeighborhoodArrivals=>Neighborhood?.FoodLandChallenge==true?8:4;
    public string NeighborhoodArrivalWord=>NeighborhoodArrivals==8?"eight":"four";
    public int NeighborhoodReserveTarget=>2*(8+NeighborhoodArrivals);
    public static World NewNeighborhoodExperiment()
    {
        var w=NewSharedWorkExperiment();w.Campaign=null;w.Neighborhood=new();
        w.Map.Name="A new neighborhood — experiment";return w;
    }
    public Cell? NeighborhoodLanding()
    {
        // One flood fill also handles the no-crossing case; a path search per eastern tile stalls the HUD.
        var reached=Map.Water.Count==0?null:Reachable(YardAccess,Blocked);
        return Map.Land.Where(c=>c.X>5 && !Blocked(c) && (reached==null || reached.Contains(c)))
            .OrderBy(c=>(c.Point-new Cell(7,2).Point).LengthSquared()).Select(c=>(Cell?)c).FirstOrDefault();
    }
    public string? NeighborhoodInvitationProblem(Cell? destination)
    {
        if(Neighborhood==null)return "This is not the neighborhood experiment.";
        if(Neighborhood.CommittedAt!=null)return $"{NeighborhoodArrivals} neighbors are already committed to joining.";
        if(destination is not Cell cell || cell.X<=5 || Blocked(cell) || !Accessible(cell))return "Build a crossing to a clear arrival spot on the east bank.";
        return null;
    }
    public bool CommitNeighbors(Cell destination)
    {
        if(NeighborhoodInvitationProblem(destination)!=null)return false;
        Neighborhood!.Destination=destination;Neighborhood.CommittedAt=Food.Time;
        History.Add($"{NeighborhoodArrivals} neighbors will reach the village in 90 seconds, even if homes and food are not ready.");return true;
    }
    public string NeighborhoodStatus=>Neighborhood is not { } n?"":n.CommittedAt is not float started?
        $"Choose a crossing, then welcome {NeighborhoodArrivalWord} neighbors. They arrive after 90 seconds even if preparations are incomplete.":!n.Arrived?
        $"{NeighborhoodArrivals} neighbors are on their way · {Math.Max(0,90-(Food.Time-started)):F0}s\nPrepare homes and food. The commitment will not reset.":
        $"{NeighborhoodArrivals} neighbors have arrived · {Housed}/{Population} housed\n"+WelcomeStatus;
    private void AdvanceNeighborhood()
    {
        if(Neighborhood is {Arrived:true} progress && progress.Welcomed.Count==Population && NewNeighborsHoused==NeighborhoodArrivals &&
            (!progress.FoodLandChallenge || EdibleStored>=NeighborhoodReserveTarget))progress.Complete=true;
        if(Neighborhood is not {Arrived:false,CommittedAt:float started,Destination:Cell destination} n || Food.Time<started+90)return;
        // If construction has closed the landing, keep the promise: enter at the yard and recover access in play.
        var names=new[]{"Lina","Ash","Cora","Remy","Iris","Leo","Nora","Theo"};
        foreach(string name in names.Take(NeighborhoodArrivals))
        {
            var person=new Villager{Id=Population,Name=name,Position=YardAccess.Point,Role=Role.Unassigned,SharedWorker=true,NextMealTime=Food.Time+15};
            People.Add(person);
            if(!Blocked(destination) && Accessible(destination))Go(person,destination,Work.ToArrival,"Walking to the new neighborhood");
            else person.Status="New arrival — reopen access to the east bank";
        }
        n.Arrived=true;ReconcileHomes();_retry=0;History.Add($"{NeighborhoodArrivals} neighbors arrived. Food and housing shortages remain recoverable.");
    }
    private void ValidateNeighborhood()
    {
        if(Neighborhood is not {} n)return;
        if(n.Complete && (!n.Arrived || n.Welcomed?.Count!=8+NeighborhoodArrivals))throw new InvalidOperationException("Incomplete neighborhood marked complete");
        if(n.FoodLandChallenge && !n.WorkplaceFood)throw new InvalidOperationException("Food/land situation requires workplace supply");
        if(n.Welcomed==null || n.Welcomed.Any(id=>id<0 || id>=Population) || n.Welcomed.Count>0 && !n.Arrived || n.VenueId is int venue && !Cottages.Any(c=>c.Id==venue && IsWelcomeStore(c) && c.Complete && !c.DemolitionRequested))throw new InvalidOperationException("Invalid welcome gathering");
        if(!SharedWork || !LocalGrainSupply || Campaign!=null || Creative ||
            n.CommittedAt is float time && (!float.IsFinite(time) || time<0 || time>Food.Time || n.Destination==null) ||
            n.Destination is Cell cell && (!Map.Contains(cell) || cell.X<=5) ||
            (n.CommittedAt==null)!=(n.Destination==null) || n.Arrived && n.CommittedAt==null || Population!=(n.Arrived?8+NeighborhoodArrivals:8))throw new InvalidOperationException("Invalid neighborhood progress");
    }
}

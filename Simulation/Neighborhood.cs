using System;
using System.Linq;

namespace Inlanders.Simulation;

public sealed class NeighborhoodProgress
{
    public Cell? Destination { get; set; }
    public float? CommittedAt { get; set; }
    public bool Arrived { get; set; }
}

public sealed partial class World
{
    public NeighborhoodProgress? Neighborhood { get; private set; }
    public static World NewNeighborhoodExperiment()
    {
        var w=NewSharedWorkExperiment();w.Campaign=null;w.Neighborhood=new();
        w.Map.Name="A new neighborhood — experiment";return w;
    }
    public Cell? NeighborhoodLanding()=>Map.Land.Where(c=>c.X>5 && !Blocked(c) && Accessible(c))
        .OrderBy(c=>(c.Point-new Cell(7,2).Point).LengthSquared()).Select(c=>(Cell?)c).FirstOrDefault();
    public string? NeighborhoodInvitationProblem(Cell? destination)
    {
        if(Neighborhood==null)return "This is not the neighborhood experiment.";
        if(Neighborhood.CommittedAt!=null)return "Four neighbors are already committed to joining.";
        if(destination is not Cell cell || cell.X<=5 || Blocked(cell) || !Accessible(cell))return "Build a crossing to a clear arrival spot on the east bank.";
        return null;
    }
    public bool CommitNeighbors(Cell destination)
    {
        if(NeighborhoodInvitationProblem(destination)!=null)return false;
        Neighborhood!.Destination=destination;Neighborhood.CommittedAt=Food.Time;
        History.Add("Four neighbors will reach the village in 90 seconds, even if homes and food are not ready.");return true;
    }
    public string NeighborhoodStatus=>Neighborhood is not { } n?"":n.CommittedAt is not float started?
        "Choose a crossing, then welcome four neighbors. They arrive after 90 seconds even if preparations are incomplete.":!n.Arrived?
        $"Four neighbors are on their way · {Math.Max(0,90-(Food.Time-started)):F0}s\nPrepare homes and food. The commitment will not reset.":
        $"Four neighbors have arrived · {Housed}/{Population} housed\nEstablish homes and food for the new neighborhood.";
    private void AdvanceNeighborhood()
    {
        if(Neighborhood is not {Arrived:false,CommittedAt:float started,Destination:Cell destination} n || Food.Time<started+90)return;
        // If construction has closed the landing, keep the promise: enter at the yard and recover access in play.
        var names=new[]{"Lina","Ash","Cora","Remy"};
        foreach(string name in names)
        {
            var person=new Villager{Id=Population,Name=name,Position=YardAccess.Point,Role=Role.Unassigned,SharedWorker=true,NextMealTime=Food.Time+15};
            People.Add(person);
            if(!Blocked(destination) && Accessible(destination))Go(person,destination,Work.ToArrival,"Walking to the new neighborhood");
            else person.Status="New arrival — reopen access to the east bank";
        }
        n.Arrived=true;ReconcileHomes();_retry=0;History.Add("Lina, Ash, Cora and Remy arrived. Food and housing shortages remain recoverable.");
    }
    private void ValidateNeighborhood()
    {
        if(Neighborhood is not {} n)return;
        if(!SharedWork || !LocalGrainSupply || Campaign!=null || Creative ||
            n.CommittedAt is float time && (!float.IsFinite(time) || time<0 || time>Food.Time || n.Destination==null) ||
            n.Destination is Cell cell && (!Map.Contains(cell) || cell.X<=5) ||
            (n.CommittedAt==null)!=(n.Destination==null) || n.Arrived && n.CommittedAt==null || Population!=(n.Arrived?12:8))throw new InvalidOperationException("Invalid neighborhood progress");
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace Inlanders.Simulation;

public sealed class FoundingProgress
{
    public bool Finished { get; set; }
    public HashSet<int> Settled { get; set; } = new();
}

public sealed partial class World
{
    public FoundingProgress? Founding { get; private set; }
    public static World NewFoundingSettlement()
    {
        var w=NewLakeMap();w.Founding=new();w.Map.Name="A home by the water";
        // The unused inherited homes become the founders' timber, not extra resources.
        foreach(var home in w.Cottages.Where(c=>c.Kind==BuildingKind.Cottage).Skip(1).ToArray())
        {w._yardLogs+=home.Delivered;w.Cottages.Remove(home);}
        w.SharedWork=true;w.LocalGrainSupply=true;
        foreach(var person in w.People)w.Assign(person.Id,Role.Unassigned);
        // Existing mature lake trees support a woodland alternative; clearing them has its ordinary consequence.
        var habitat=new WoodlandHabitat{Id=0,Cell=new(-5,-6)};
        habitat.Stock=w.HabitatCapacity(habitat);w.Map.Wildlife.Add(habitat);
        w.History.Clear();w.History.Add("Eight founders have reached the lake. Give them homes, choose a livelihood, then invite more neighbors when you want to grow.");
        w.ReconcileHomes();w.Validate();w.ValidateMapOccupancy();return w;
    }
    private string? FoundingInvitationProblem()
    {
        if(SpareBeds<2)return "Finish a home for two more neighbors first.";
        if(People.Any(p=>!p.Fed))return "Some neighbors missed a meal. Restore food service before inviting more.";
        if(ArrivalSpots().Length<2)return "Leave two arrival spots clear near the timber yard.";
        return null;
    }
    public string? FinishFoundingProblem()=>Founding==null?"This is not a founding settlement.":
        Population<12?"Invite two households when ready: a village of twelve.":
        Housed<Population?"Give every resident a finished home.":
        People.Any(p=>p.Id>=InitialPopulation && !Founding.Settled.Contains(p.Id))?"Let each new neighbor collect and eat an ordinary meal.":
        People.Any(p=>!p.Fed)?"Some neighbors missed a meal. Restore food service before finishing.":null;
    public bool FinishFounding()
    {
        if(Founding?.Finished==true || FinishFoundingProblem()!=null)return false;
        Founding!.Finished=true;return true;
    }
    private void ValidateFounding()
    {
        if(Founding is not {} f)return;
        if(Creative || Neighborhood!=null || Campaign!=null || !SharedWork || !LocalGrainSupply || f.Settled==null ||
            f.Settled.Any(id=>id<InitialPopulation || id>=Population) || f.Finished && (Population<12 || f.Settled.Count<4))
            throw new InvalidOperationException("Invalid founding settlement");
    }
}

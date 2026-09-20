using System;
using System.Collections.Generic;
using System.Linq;

namespace Inlanders.Simulation;

public sealed class FoundingProgress
{
    public SharedCommons? Commons { get; set; }
    public bool ProvisionedLife { get; set; }
    public bool RiverFarmstead { get; set; }
    public bool TransformationHamlet { get; set; }
    public bool CultivatedBank { get; set; }
    public List<StartingBuilding> StartingBuildings { get; set; } = new();
    public bool WorkingVillage { get; set; }
    public bool Finished { get; set; }
    public int HallProject { get; set; }
    public Dictionary<int,int> HallVisitors { get; set; } = new();
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
        w.Map.StoneDeposits.Add(new(){Id=0,Cell=new(15,-1),Capacity=36,Remaining=36});
        w.History.Clear();w.History.Add("Eight founders have reached the lake. Give them homes, choose a livelihood, then invite more neighbors when you want to grow.");
        w.ReconcileHomes();w.Validate();w.ValidateMapOccupancy();return w;
    }
    private string? FoundingInvitationProblem()
    {
        if(SpareBeds<2)return "Finish a home for two more neighbors first.";
        if(!Creative && People.Any(p=>!p.Fed))return "Some neighbors missed a meal. Restore food service before inviting more.";
        if(ArrivalSpots().Length<2)return "Leave two arrival spots clear near the timber yard.";
        return null;
    }
    public string? FinishFoundingProblem()=>Founding==null?"This is not a founding settlement.":
        Founding.TransformationHamlet?null:Founding.RiverFarmstead?FarmsteadReadyProblem():
        Population<12?"Invite two households when ready: a village of twelve.":
        Housed<Population?"Give every resident a finished home.":
        People.Any(p=>p.Id>=InitialPopulation && !Founding.Settled.Contains(p.Id))?"Let each new neighbor collect and eat an ordinary meal.":
        People.Any(p=>!p.Fed)?"Some neighbors missed a meal. Restore food service before finishing.":null;
    public bool FinishFounding()
    {
        if(Founding?.Finished==true || FinishFoundingProblem()!=null)return false;
        Founding!.Finished=true;return true;
    }
    public bool BeginFoundingHall()
    {
        if(Founding is not {Finished:true,HallProject:0,RiverFarmstead:false})return false;
        Founding.HallProject=1;return true;
    }
    public Cottage? FoundingHall=>Cottages.Where(c=>c.Kind==BuildingKind.GatheringHall && !c.DemolitionRequested)
        .OrderByDescending(c=>Founding?.HallVisitors.ContainsKey(c.Id)==true && c.Complete).ThenBy(c=>c.Id).FirstOrDefault();
    public string? FinishFoundingHallProblem()=>Founding?.HallProject!=1?"Begin the hall project first.":
        FoundingHall is not {} hall?"Choose a site and build a gathering hall.":!hall.Complete?"Supply the hall with 8 planks and 12 stone, then let builders finish it.":
        !Founding.HallVisitors.ContainsKey(hall.Id)?"Let a neighbor finish an ordinary break at the hall.":null;
    public bool FinishFoundingHall()
    {
        if(FinishFoundingHallProblem()!=null)return false;
        Founding!.HallProject=2;return true;
    }
    private void RecordFoundingHallUse(Villager person)
    {
        if(Founding?.HallProject==1 && person.LeisureSiteId is int id && Cottages.Any(c=>c.Id==id && c.Kind==BuildingKind.GatheringHall))
            Founding.HallVisitors.TryAdd(id,person.Id);
    }
    private void ValidateFounding()
    {
        if(Founding is not {} f)return;
        if(f.HallProject is <0 or >2 || f.HallVisitors==null || f.HallVisitors.Any(p=>p.Key<1 || p.Key>=_nextSite || p.Value<0 || p.Value>=Population) || f.HallProject>0 && !f.Finished || f.HallProject==2 && f.HallVisitors.Count==0)throw new InvalidOperationException("Invalid hall project");
        if(f.TransformationHamlet && (f.StartingBuildings==null || f.StartingBuildings.Count!=9 || f.StartingBuildings.Select(b=>b.Id).Distinct().Count()!=9 || f.StartingBuildings.Any(b=>b.Id<1 || b.Id>=_nextSite || b.Rotation is <0 or >3 || !Enum.IsDefined(b.Kind) || !Map.Contains(b.Cell))))throw new InvalidOperationException("Invalid hamlet opening layout");
        if(f.CultivatedBank && !f.TransformationHamlet)throw new InvalidOperationException("Cultivated bank needs hamlet rules");
        if((f.WorkingVillage || f.TransformationHamlet) && !f.RiverFarmstead || Creative && !f.TransformationHamlet || Neighborhood!=null || Campaign!=null || !SharedWork || !LocalGrainSupply || f.Settled==null ||
            f.Settled.Any(id=>id<InitialPopulation || id>=Population) || f.Finished && !f.RiverFarmstead && (Population<12 || f.Settled.Count<4))
            throw new InvalidOperationException("Invalid founding settlement");
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace Inlanders.Simulation;

public sealed class SharedGathering
{
    public Cell Center { get; set; }
    public bool Spread { get; set; }=true;
    public Dictionary<int,Cell> Seats { get; set; }=new();
    public HashSet<int> Ate { get; set; }=new();
    public float Started { get; set; }
    public bool Eating { get; set; }
    public bool Complete { get; set; }
    public bool Cancelled { get; set; }
    public bool Active=>!Complete && !Cancelled;
}
public sealed partial class World
{
    public SharedGathering? Gathering=>Neighborhood?.Gathering;
    public Cell[] GatheringPlaces(Cell center,bool spread=true)
    {
        if(!Map.Contains(center) || Map.Water.Contains(center))return Array.Empty<Cell>();
        var busy=People.Where(p=>p.LeisureSiteId!=null || p.Task is Work.ToRest or Work.Resting).Select(p=>p.Destination).ToHashSet();
        var reached=Reachable(YardAccess,Blocked);
        var available=Map.Land.Where(c=>(c.Point-center.Point).LengthSquared()<=16 && reached.Contains(c) && !Blocked(c) &&
            !MealSpotReserved(c) && !ComfortSpotReserved(c) && !busy.Contains(c)).ToList();
        if(!spread)return available.OrderBy(c=>(c.Point-center.Point).LengthSquared()).ThenBy(c=>c.Z).ThenBy(c=>c.X).Take(Population).ToArray();
        // Match successive places around the circle rather than taking one side of a distance tie.
        var selected=new List<Cell>();float radius=Math.Clamp(Population/(2*MathF.PI),2,3);
        for(int i=0;i<Population && available.Count>0;i++)
        {
            float angle=2*MathF.PI*i/Population;
            var ideal=center.Point+new System.Numerics.Vector2(MathF.Cos(angle),MathF.Sin(angle))*radius;
            var place=available.OrderBy(c=>(c.Point-ideal).LengthSquared()).ThenBy(c=>c.Z).ThenBy(c=>c.X).First();
            selected.Add(place);available.Remove(place);
        }
        return selected.ToArray();
    }
    public string? GatheringProblem(Cell center,bool spread=true)
    {
        if(Neighborhood?.Complete!=true)return "Settle the newcomers first, then choose an outdoor meal spot.";
        if(Gathering?.Active==true)return "A shared meal is already gathering.";
        int places=GatheringPlaces(center,spread).Length;
        return places<Population?$"{places}/{Population} clear reachable places nearby. Choose more open ground.":null;
    }
    public bool BeginGathering(Cell center,bool spread=true)
    {
        if(GatheringProblem(center,spread)!=null)return false;
        var seats=GatheringPlaces(center,spread);
        Neighborhood!.Gathering=new(){Center=center,Spread=spread,Started=Food.Time,Seats=People.ToDictionary(p=>p.Id,p=>seats[p.Id])};
        History.Add("An outdoor meal is gathering. Everyone brings their next meal; work resumes afterwards.");_retry=0;return true;
    }
    public bool CancelGathering()
    {
        if(Gathering is not {Active:true} g)return false;
        g.Cancelled=true;
        foreach(var p in People.Where(p=>p.Meal?.Gathering==true).ToArray())
        {p.Meal!.Gathering=false;InterruptMeal(p);}
        History.Add("Outdoor meal cancelled. Carried portions return to storage; ordinary meals continue.");return true;
    }
    private void AdvanceGathering()
    {
        if(Gathering is not {Active:true} g)return;
        if(Food.Time-g.Started>180 && !g.Eating){CancelGathering();History.Add("The gathering dispersed after waiting for food or arrivals. Try again when the village is ready.");return;}
        if(!g.Eating && People.All(p=>p.Task==Work.EatingMeal && p.Meal?.Gathering==true))
        {
            g.Eating=true;
            foreach(var p in People){p.Timer=0;p.Meal!.Due=Food.Time;p.Status="Sharing an outdoor meal";}
        }
    }
    private void RecordGatheringMeal(Villager p,MealRequest meal)
    {
        if(!meal.Gathering || Gathering is not {Active:true} g)return;
        g.Ate.Add(p.Id);meal.Gathering=false;
        if(g.Ate.Count==Population){g.Complete=true;History.Add("Everyone shared the outdoor meal. Village life resumes.");}
    }
    private void ValidateGathering()
    {
        if(Gathering is not {} g)return;
        if(Neighborhood?.Complete!=true || g.Seats==null || g.Ate==null || !float.IsFinite(g.Started) || g.Started<0 || g.Started>Food.Time ||
            g.Seats.Count!=Population || g.Seats.Keys.Any(id=>id<0 || id>=Population) || g.Seats.Values.Distinct().Count()!=Population ||
            g.Ate.Any(id=>!g.Seats.ContainsKey(id)) || g.Complete && g.Ate.Count!=Population || g.Complete && g.Cancelled ||
            g.Active && g.Seats.Values.Any(c=>Blocked(c) || (c.Point-g.Center.Point).LengthSquared()>16))throw new InvalidOperationException("Invalid shared outdoor gathering");
        foreach(var p in People.Where(p=>p.Meal?.Gathering==true))
            if(!g.Active || p.Meal!.Seat!=g.Seats[p.Id] || p.Meal.Welcome || g.Ate.Contains(p.Id))throw new InvalidOperationException("Invalid gathering meal");
    }
}

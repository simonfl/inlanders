using System;
using System.Collections.Generic;
using System.Linq;

namespace Inlanders.Simulation;

public sealed class WoodlandHabitat
{
    public int Id { get; set; }
    public Cell Cell { get; set; }
    public float Stock { get; set; }
    public const int Radius=5;
    public bool Contains(Cell cell) => (cell.Point-Cell.Point).LengthSquared()<=Radius*Radius;
}
public sealed partial class MapLayout
{
    public List<WoodlandHabitat> Wildlife { get; set; } = new();
}
public sealed partial class World
{
    public int HabitatTrees(WoodlandHabitat h) => Trees.Count(t=>h.Contains(t.Cell) && !t.Salvage && !t.Felled && !t.NeedsPlanting && t.Growth>=1);
    public int HabitatCapacity(WoodlandHabitat h) => Math.Min(12,HabitatTrees(h)*2);
    public float HabitatRecovery(WoodlandHabitat h) => Math.Min(6,HabitatTrees(h))*.5f; // game units per minute
    public IEnumerable<Cell> WildlifeSpots(WoodlandHabitat h) => Map.Land.Where(c=>h.Contains(c) && c!=h.Cell && !Blocked(c)).OrderBy(c=>(c.Point-h.Cell.Point).LengthSquared());
    private int ClaimedGame(WoodlandHabitat h) => People.Where(p=>p.HabitatId==h.Id).Sum(p=>p.Reserved);
    public int AvailableGame(WoodlandHabitat h) => Math.Max(0,Math.Min((int)h.Stock,HabitatCapacity(h))-ClaimedGame(h));
    private IEnumerable<WoodlandHabitat> HuntingGrounds(Cell lodge) => Map.Wildlife.Where(h=>(h.Cell.Point-lodge.Point).LengthSquared()<=64 && Accessible(h.Cell));
    public string WildlifeSurvey(Cell lodge) => string.Join("\n",HuntingGrounds(lodge).Select(h=>$"Woodland {h.Id}: {AvailableGame(h)} game available · {HabitatTrees(h)} mature trees · capacity {HabitatCapacity(h)} · +{HabitatRecovery(h):0.#}/min. Shared habitat."));
    public string HabitatLoss(Cell treeCell)
    {
        var tree=Trees.FirstOrDefault(t=>t.Cell==treeCell && !t.Salvage && !t.Felled && !t.NeedsPlanting && t.Growth>=1);
        if(tree==null) return "";
        return string.Join("\n",Map.Wildlife.Where(h=>h.Contains(treeCell)).Select(h=>
            $"Woodland {h.Id}: felling this tree changes capacity {HabitatCapacity(h)} → {Math.Min(12,(HabitatTrees(h)-1)*2)}, recovery {HabitatRecovery(h):0.#} → {Math.Min(6,HabitatTrees(h)-1)*.5f:0.#} game/min. Replant and let trees mature to restore habitat."));
    }
    private void AdvanceWildlife(float dt)
    {
        foreach(var h in Map.Wildlife)
        {
            // Honor an outing already reserved before tree loss; no new claims above the reduced capacity.
            h.Stock=Math.Max(ClaimedGame(h),Math.Min(HabitatCapacity(h),h.Stock+dt*HabitatRecovery(h)/60));
        }
    }
    private void ClaimHunting(Villager person)
    {
        var lodge=FoodSite(person,BuildingKind.HuntingLodge,c=>BelowOutputTarget(c) && HuntingGrounds(c.Cell).Any(h=>AvailableGame(h)>0));
        if(lodge==null) { person.Status=ProductionWait(person); return; }
        var habitat=HuntingGrounds(lodge.Cell).Where(h=>AvailableGame(h)>0).OrderBy(h=>TravelCost(At(person),h.Cell)).ThenBy(h=>h.Id).First();
        person.WorkplaceId=lodge.Id; person.HabitatId=habitat.Id; person.Reserved=Math.Min(2,AvailableGame(habitat)); person.Cargo=Resource.Game;
        Go(person,habitat.Cell,Work.ToHunt,$"Tracking game in woodland {habitat.Id}");
    }
    private void TickHunting(Villager person)
    {
        if(person.Task==Work.ToHunt) { person.Task=Work.Hunting; person.Timer=0; person.Status="Tracking and hunting woodland game"; return; }
        if(person.Timer<10) return;
        var habitat=Map.Wildlife.Single(h=>h.Id==person.HabitatId);
        habitat.Stock-=person.Reserved; Food.HuntedGame+=person.Reserved; person.Carried=person.Reserved;
        person.Reserved=0; person.HabitatId=null;
        DeliverFood(person);
    }
    private void ValidateWildlife()
    {
        if(Map.Wildlife==null || Map.Wildlife.Select(h=>h.Id).Distinct().Count()!=Map.Wildlife.Count || Map.Wildlife.Any(h=>h.Id<0 || !Map.Contains(h.Cell) || Map.Water.Contains(h.Cell) || !float.IsFinite(h.Stock) || h.Stock<ClaimedGame(h) || h.Stock>12))
            throw new InvalidOperationException("Invalid woodland habitat");
        foreach(var p in People)
            if((p.Task is Work.ToHunt or Work.Hunting)!=(p.HabitatId!=null) || p.HabitatId is int id &&
                (p.Cargo!=Resource.Game || p.Carried!=0 || p.Reserved is <1 or >2 || !Map.Wildlife.Any(h=>h.Id==id && h.Cell==p.Destination) ||
                 !Cottages.Any(c=>c.Id==p.WorkplaceId && c.Complete && c.Kind==BuildingKind.HuntingLodge)))
                throw new InvalidOperationException("Invalid hunter claim");
        if(Food.Game<0 || Food.HuntedGame<0 || Food.EatenGame<0 || StoredFood(Resource.Game)+Food.EatenGame+People.Where(p=>p.Cargo==Resource.Game).Sum(p=>p.Carried)!=Food.HuntedGame+CreativeNet(Resource.Game))
            throw new InvalidOperationException("Game conservation failed");
    }
}

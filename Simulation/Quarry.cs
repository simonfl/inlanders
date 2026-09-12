using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Inlanders.Simulation;

public sealed class StoneDeposit
{
    public int Id { get; init; }
    public Cell Cell { get; init; }
    public int Capacity { get; init; }
    [JsonInclude] public int Remaining { get; internal set; }
    public Cell Access => new(Cell.X+1,Cell.Z);
}

public sealed partial class MapLayout
{
    public List<StoneDeposit> StoneDeposits { get; set; } = new();
}

public sealed partial class World
{
    private int _stone;
    public int YardStone => _stone;
    public int Stone => _stone+Cottages.Sum(c=>c.StoredStone);
    public int QuarriedStone { get; private set; }
    public int ReservedStone => People.Where(v=>v.Cargo==Resource.Stone && v.Task is Work.ToMaterials or Work.ToHaulPickup).Sum(v=>v.Reserved);
    public int AvailableStone => Stone-ReservedStone;
    public int AvailableDeposit(StoneDeposit deposit) => deposit.Remaining-People.Where(p=>p.DepositId==deposit.Id).Sum(p=>p.Reserved);
    private IEnumerable<StoneDeposit> QuarryDeposits(Cottage camp) => Map.StoneDeposits.Where(d=>(d.Cell.Point-camp.Cell.Point).LengthSquared()<=16 && Accessible(d.Access));
    public string QuarrySurvey(Cell cell) => string.Join("\n",Map.StoneDeposits.Where(d=>(d.Cell.Point-cell.Point).LengthSquared()<=16)
        .Select(d=>$"Outcrop {d.Id}: {AvailableDeposit(d)} available / {d.Remaining} remaining of {d.Capacity} stone · {(Accessible(d.Access)?"access open":"access blocked")}.")) is string text && text.Length>0 ? text : "Needs an outcrop within 4 tiles. Use Resource survey [U] to find marked stone.";

    private void ClaimQuarry(Villager person)
    {
        var camp=FoodSite(BuildingKind.Quarry,c=>BelowOutputTarget(c) && QuarryDeposits(c).Any(d=>AvailableDeposit(d)>0));
        if(camp==null) { person.Status=ProductionWait(Role.Quarrier); return; }
        var deposit=QuarryDeposits(camp).Where(d=>AvailableDeposit(d)>0).OrderBy(d=>TravelCost(At(person),d.Access)).ThenBy(d=>d.Id).First();
        person.WorkplaceId=camp.Id; person.DepositId=deposit.Id; person.Reserved=Math.Min(2,AvailableDeposit(deposit)); person.Cargo=Resource.Stone;
        Go(person,deposit.Access,Work.ToQuarry,$"Walking to outcrop {deposit.Id} for {person.Reserved} stone");
    }
    private void TickQuarry(Villager person)
    {
        if(person.Task==Work.ToQuarry) { person.Task=Work.Quarrying; person.Timer=0; person.Status="Cutting stone from the outcrop"; return; }
        if(person.Timer<6) return;
        var deposit=Map.StoneDeposits.Single(d=>d.Id==person.DepositId);
        deposit.Remaining-=person.Reserved; QuarriedStone+=person.Reserved; person.Carried=person.Reserved;
        person.Reserved=0; person.DepositId=null; ReturnTimber(person);
    }
    private void ValidateQuarry()
    {
        if(Map.StoneDeposits==null || Map.StoneDeposits.Select(d=>d.Id).Distinct().Count()!=Map.StoneDeposits.Count ||
            Map.StoneDeposits.Any(d=>d.Id<0 || d.Capacity<=0 || d.Remaining<0 || d.Remaining>d.Capacity || AvailableDeposit(d)<0 || !Map.Contains(d.Cell) || Map.Water.Contains(d.Cell)))
            throw new InvalidOperationException("Invalid stone deposits");
        if(Stone<0 || AvailableStone<0 || QuarriedStone<0 || Map.StoneDeposits.Sum(d=>d.Capacity-d.Remaining)!=QuarriedStone ||
            Stone+People.Where(p=>p.Cargo==Resource.Stone).Sum(p=>p.Carried)+Cottages.Sum(c=>c.DeliveredStone)+Trees.Where(t=>t.Material==Resource.Stone).Sum(t=>t.Logs)!=QuarriedStone+CreativeNet(Resource.Stone))
            throw new InvalidOperationException("Stone conservation failed");
        foreach(var person in People)
        {
            bool quarrying=person.Task is Work.ToQuarry or Work.Quarrying;
            if(quarrying!=(person.DepositId!=null) || quarrying && (person.Cargo!=Resource.Stone || person.Carried!=0 || person.Reserved is <1 or >2 ||
                !Cottages.Any(c=>c.Id==person.WorkplaceId && c.Kind==BuildingKind.Quarry && c.Complete && QuarryDeposits(c).Any(d=>d.Id==person.DepositId && d.Access==person.Destination))))
                throw new InvalidOperationException("Invalid quarry worker");
        }
    }
}

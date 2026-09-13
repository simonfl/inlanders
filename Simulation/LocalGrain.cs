using System;
using System.Linq;

namespace Inlanders.Simulation;

public sealed partial class World
{
    public bool LocalGrainSupply { get; private set; }
    public const int FarmGrainCapacity=12;

    // Development comparison factory; the full neighborhood scenario is integrated separately.
    public static World NewLocalSupplyExperiment()
    {
        var world=NewCampaign(6);world.LocalGrainSupply=true;return world;
    }
    public Cell GrainAccess(int? id)=>id is int n?Cottages.Single(c=>c.Id==n && c.Kind==BuildingKind.Farm).Entrance:YardAccess;
    public int GrainAt(int? id)=>id is int n?Cottages.Single(c=>c.Id==n && c.Kind==BuildingKind.Farm).StoredGrain:Food.Grain;
    public int GrainReservedAt(int? id)=>People.Where(p=>p.Task==Work.ToGrain && p.GrainSourceId==id).Sum(p=>p.FoodReserved);
    public int GrainAvailableAt(int? id)=>GrainAt(id)-GrainReservedAt(id);
    public int GrainIncoming(int id)=>People.Where(p=>p.GrainDestinationId==id && p.Task==Work.ToPantry).Sum(p=>p.Carried);
    public int StoredGrain=>Food.Grain+Cottages.Sum(c=>c.StoredGrain);

    private bool TryGrainSource(Cell from,Cell bakery,out int? source)
    {
        if(!LocalGrainSupply){source=null;return GrainAvailableAt(null)>=2;}
        var stores=new int?[]{null}.AsEnumerable();
        if(LocalGrainSupply)stores=stores.Concat(Cottages.Where(c=>c.Kind==BuildingKind.Farm && c.Complete && !c.DemolitionRequested).Select(c=>(int?)c.Id));
        var available=stores.Where(id=>GrainAvailableAt(id)>=2 && Accessible(GrainAccess(id)))
            .OrderBy(id=>TravelCost(from,GrainAccess(id))+TravelCost(GrainAccess(id),bakery)).ThenBy(id=>id??-1).ToArray();
        source=available.FirstOrDefault();return available.Length>0;
    }
    private void ChangeGrainAt(int? id,int amount)
    {
        if(id is int n)Cottages.Single(c=>c.Id==n).StoredGrain+=amount;else Food.Grain+=amount;
    }
    private bool DeliverLocalGrain(Villager person)
    {
        if(!LocalGrainSupply || person.Cargo!=Resource.Grain || person.WorkplaceId is not int farmId)return false;
        var farm=Cottages.FirstOrDefault(c=>c.Id==farmId && c.Kind==BuildingKind.Farm && c.Complete && !c.DemolitionRequested);
        if(farm==null || farm.StoredGrain+GrainIncoming(farmId)+person.Carried>FarmGrainCapacity)return false;
        person.GrainDestinationId=farmId;
        Go(person,farm.Entrance,Work.ToPantry,$"Carrying {person.Carried} grain to farm {farm.Id}'s store");return true;
    }
    private void CloseGrainStore(int id)
    {
        foreach(var p in People.Where(p=>p.GrainSourceId==id || p.GrainDestinationId==id).ToArray())Interrupt(p);
    }
    private void ValidateLocalGrain()
    {
        foreach(var c in Cottages)
        {
            if(c.StoredGrain<0 || c.StoredGrain+GrainIncoming(c.Id)>FarmGrainCapacity ||
                c.StoredGrain>0 && (!LocalGrainSupply || c.Kind!=BuildingKind.Farm || !c.Complete))throw new InvalidOperationException("Invalid local grain store");
            if(c.Kind==BuildingKind.Farm && GrainAvailableAt(c.Id)<0)throw new InvalidOperationException("Local grain overreserved");
        }
        foreach(var p in People)
        {
            bool StoreExists(int id)=>LocalGrainSupply && Cottages.Any(c=>c.Id==id && c.Kind==BuildingKind.Farm && c.Complete && !c.DemolitionRequested);
            if(p.GrainSourceId is int source && (p.Task!=Work.ToGrain || p.FoodReserved!=2 || !StoreExists(source)))throw new InvalidOperationException("Orphaned local grain pickup");
            if(p.GrainDestinationId is int destination && (p.Task!=Work.ToPantry || p.Cargo!=Resource.Grain || p.Carried<=0 || !StoreExists(destination)))throw new InvalidOperationException("Orphaned local grain delivery");
        }
    }
}

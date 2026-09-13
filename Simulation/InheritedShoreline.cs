using System;
using System.Linq;

namespace Inlanders.Simulation;

public sealed partial class World
{
    public bool IsInheritedShoreline=>Neighborhood?.InheritedShoreline==true;
    public bool NeighborhoodHomeRegion(Cell cell)=>IsInheritedShoreline || cell.X>5;
    public string NeighborhoodHomeRegionName=>IsInheritedShoreline?"village":"east-bank";

    public static World NewInheritedShoreline()
    {
        var w=NewNeighborhoodExperiment();
        w.Neighborhood!.WorkplaceFood=true;w.Neighborhood.InheritedShoreline=true;
        w.Map.Name="Willow inlet";
        // This inherited village grew around its gardens, not the opening's western forager.
        w.Cottages.RemoveAll(c=>c.Kind==BuildingKind.ForagerHut);
        // Existing footway around the inlet. A bridge is an optional shortcut, not an entry gate.
        w.Map.Water.Remove(new(5,8));
        for(int x=6;x<=8;x++)for(int z=-6;z<=-2;z++)
            if(w.Map.Contains(new(x,z)))w.Map.Water.Add(new(x,z));
        w.Trees.RemoveAll(t=>w.Map.Water.Contains(t.Cell) || w.Map.Water.Contains(t.Access));
        w.Bushes.RemoveAll(b=>b.Cell!=new Cell(2,6));
        void Ready(Cell cell,BuildingKind kind)
        {
            var occupied=Footprint(cell,0,kind).Append(Door(cell,0)).ToHashSet();
            w.Trees.RemoveAll(t=>occupied.Contains(t.Cell) || occupied.Contains(t.Access));
            var site=w.Place(cell,0,kind)??throw new InvalidOperationException($"Inherited {kind}: {w.PlacementProblem(cell,0,kind)}");
            site.Delivered=site.Required;site.Construction=1;
        }
        Ready(new(10,2),BuildingKind.VegetableGarden);
        Ready(new(13,2),BuildingKind.VegetableGarden);
        Ready(new(13,6),BuildingKind.VegetableGarden);
        Ready(new(3,4),BuildingKind.SeatingGarden);
        // Authored resources include materials already invested in inherited buildings.
        w._yardLogs=12;w.InitialLogs=w.Trees.Sum(t=>t.Logs)+w.Cottages.Sum(c=>c.Delivered)+w._yardLogs;
        w.Food.InitialBerries=w.Food.Berries=24;
        // Inherited harvested crops use the same conservation ledger as later production.
        foreach(var garden in w.Cottages.Where(c=>c.Kind==BuildingKind.VegetableGarden))
        {garden.PantryFood[Array.IndexOf(EdibleKinds,Resource.Vegetables)]=8;w.Food.GrownVegetables+=8;}
        for(int x=2;x<=10;x++)if(!w.Blocked(new(x,8)))w.Paths.Add(new(x,8));
        w.Map.FishingGrounds.Add(new(){Id=0,Name="Inlet shallows",Cell=new(7,-3),Capacity=12,Stock=12,RegrowthPerSecond=1f/15});
        w.History.Clear();w.History.Add("Willow inlet: homes west, food gardens east, an old footway around the southern shore. Keep it, bridge it, or arrange another way.");
        w.ReconcileHomes();w.Validate();w.ValidateMapOccupancy();return w;
    }

    // A current welcome readiness check, deliberately not a rolling stability certificate.
    public string? ShorelineFoodProblem()
    {
        if(!IsInheritedShoreline)return null;
        if(People.Any(p=>!p.Fed))return "Some residents missed a meal. Restore food access before finishing the welcome.";
        var stores=Cottages.Where(c=>c.Complete && !c.WorkPaused && !c.DemolitionRequested && IsWorkplaceFoodStore(c)
            && EdibleKinds.Sum(k=>FoodAvailableAt(c.Id,k))>0).ToArray();
        if(stores.Length==0)return "Keep an open food producer with a meal available. Stored welcome portions alone do not finish this village.";
        if(People.Any(p=>!stores.Any(s=>FindPath(At(p),s.Entrance,Blocked)!=null)))return "A resident cannot reach the available food. Restore access.";
        return null;
    }
}

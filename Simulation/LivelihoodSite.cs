using System;
using System.Linq;
namespace Inlanders.Simulation;
public sealed record LivelihoodSite(string Summary,string Destination,Cell[] Route);
public sealed partial class World
{
    private string NaturalSupplyText(float rate)=>$"Shared renewal up to {rate:0.#}/min · village eats {Population}/min. More buildings do not add supply.";
    public LivelihoodSite ReadRelocationLivelihood(int id,Cell at,int rotation)
    {
        if(RelocationProblem(id,at,rotation)!=null)return new("","",Array.Empty<Cell>());
        var site=Cottages.Single(c=>c.Id==id);int index=Cottages.IndexOf(site);Cottages.RemoveAt(index);
        try{return ReadLivelihoodSite(at,rotation,site.Kind,site.PlotRows);}
        finally{Cottages.Insert(index,site);}
    }
    public LivelihoodSite ReadLivelihoodSite(Cell at,int rotation,BuildingKind kind,int rows=0)
    {
        if(PlacementProblem(at,rotation,kind,rows)!=null)return new("","",Array.Empty<Cell>());
        Cell door=kind==BuildingKind.FishingDock?DockEntrance(at,rotation):Door(at,rotation);
        var footprint=Footprint(at,rotation,kind,rows).ToHashSet();
        bool Closed(Cell c)=>Blocked(c)||footprint.Contains(c);
        if(kind==BuildingKind.FishingDock)
        {
            var launch=DockLaunch(at,rotation);
            var reachable=Map.FishingGrounds.Select(g=>new{Ground=g,Route=FindBoatRoute(launch,g.Cell)}).Where(x=>x.Route!=null).OrderBy(x=>x.Route!.Count).ThenBy(x=>x.Ground.Id).ToArray();
            var target=reachable.FirstOrDefault();
            return target==null?new("No reachable fishing water.","",Array.Empty<Cell>()):new($"Water route preview · {AvailableFish(target.Ground)} fish available now; stocks shared\n"+NaturalSupplyText(reachable.Sum(x=>x.Ground.RegrowthPerSecond)*60),target.Ground.Name,new[]{launch}.Concat(target.Route!).ToArray());
        }
        if(kind==BuildingKind.ForagerHut)
        {
            var reachable=Bushes.Select(b=>new{Bush=b,Route=FindPath(door,b.Access,Closed)}).Where(x=>x.Route!=null).OrderBy(x=>x.Route!.Count).ThenBy(x=>x.Bush.Id).ToArray();
            var target=reachable.FirstOrDefault();
            return target==null?new("No reachable berry bushes · this hut cannot gather here.","",Array.Empty<Cell>()):new($"Possible gathering walk · {target.Route!.Count} ground steps · {target.Bush.Ripe} ripe berries now\n"+NaturalSupplyText(reachable.Length*7.5f), "Berry bushes · workers choose available fruit",new[]{door}.Concat(target.Route!).ToArray());
        }
        if(kind==BuildingKind.HuntingLodge)
        {
            var reachable=Map.Wildlife.Where(h=>(h.Cell.Point-at.Point).LengthSquared()<=64).Select(h=>new{Habitat=h,Route=FindPath(door,h.Cell,Closed)}).Where(x=>x.Route!=null).OrderBy(x=>x.Route!.Count).ToArray();
            var target=reachable.FirstOrDefault();
            return target==null?new("No reachable hunting ground.","",Array.Empty<Cell>()):new($"Possible hunting walk · {target.Route!.Count} ground steps\n"+NaturalSupplyText(reachable.Sum(x=>HabitatRecovery(x.Habitat))),"Shared woodland · mature trees sustain game",new[]{door}.Concat(target.Route!).ToArray());
        }
        if(kind is BuildingKind.Farm or BuildingKind.Bakery)
        {
            var partner=kind==BuildingKind.Farm?BuildingKind.Bakery:BuildingKind.Farm;
            var target=Cottages.Where(c=>c.Kind==partner && !c.DemolitionRequested).Select(c=>new{Site=c,Route=FindPath(door,c.Entrance,Closed)}).Where(x=>x.Route!=null).OrderBy(x=>x.Route!.Count).ThenBy(x=>x.Site.Id).FirstOrDefault();
            string name=partner==BuildingKind.Farm?"grain field":"oven";
            if(target==null)return new($"Needs a {name} before bread can be made · "+(Creative?"free to build":$"{Buildings.Get(partner).CostText} extra"),"",Array.Empty<Cell>());
            return new($"Grain connection preview · {target.Route!.Count} ground steps · both workplaces needed",(target.Site.Complete?"":"Planned ")+name,new[]{door}.Concat(target.Route).ToArray());
        }
        if(IsVegetablePlot(kind))
        {
            var target=Cottages.Where(c=>Buildings.Get(c.Kind).Beds>0 && !c.DemolitionRequested).Select(c=>new{Site=c,Route=FindPath(door,c.Entrance,Closed)}).Where(x=>x.Route!=null).OrderBy(x=>x.Route!.Count).ThenBy(x=>x.Site.Id).FirstOrDefault();
            return target==null?new("Vegetables are eaten here or carried to a seat · choose homes nearby","",Array.Empty<Cell>()):new($"Possible meal walk · {target.Route!.Count} ground steps to nearest home",(target.Site.Complete?"":"Planned ")+"home",new[]{door}.Concat(target.Route).ToArray());
        }
        if(Buildings.Get(kind).Beds>0)
        {
            var target=Cottages.Where(c=>!c.DemolitionRequested && (IsVegetablePlot(c.Kind) || c.Kind is BuildingKind.Bakery or BuildingKind.FishingDock)).Select(c=>new{Site=c,Route=FindPath(door,c.Entrance,Closed)}).Where(x=>x.Route!=null).OrderBy(x=>x.Route!.Count).ThenBy(x=>x.Site.Id).FirstOrDefault();
            if(target==null)return new("No reachable vegetable plot, oven or landing in this preview · other food sources are not shown", "",Array.Empty<Cell>());
            string name=IsVegetablePlot(target.Site.Kind)?"vegetable plot":target.Site.Kind==BuildingKind.Bakery?"oven":"landing";
            return new($"Possible food walk · {target.Route!.Count} ground steps · meals depend on available food and seating",(target.Site.Complete?"":"Planned ")+name,new[]{door}.Concat(target.Route).ToArray());
        }
        return new("","",Array.Empty<Cell>());
    }
}

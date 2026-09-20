using System;
using System.Linq;
using System.Collections.Generic;
namespace Inlanders.Simulation;
public sealed partial class World
{
    private EconomyReport SharedEconomy(EconomyStock[] stocks)
    {
        var issues=new List<EconomyIssue>();
        var sites=Cottages.Where(c=>!c.DemolitionRequested).ToArray();
        if(EdibleStored<Population*2)
        {
            var food=sites.Where(c=>ProductionOutput(c.Kind) is Resource r && EdibleKinds.Contains(r)).ToArray();
            if(food.Length==0)issues.Add(new("food-low","No edible-food producer is planned. Place a food source with room and access.",Build:BuildingKind.VegetableGarden));
            else
            {
                var source=food.FirstOrDefault(c=>c.Complete && !c.WorkPaused)??food[0];
                string detail=!source.Complete?(source.ConstructionPaused?"Food is low; construction of this producer is paused. Resume its plan when ready.":"Food is low; a producer is still being built."):source.WorkPaused?"Food is low and producers are paused. Resume production.":"Food is low. Inspect production, ingredients and access; shared workers take available jobs.";
                issues.Add(new(source.WorkPaused?"food-paused":"food-low",detail,Workplace:source.Id));
            }
        }
        void NeedSite(BuildingKind kind,bool needed,string why)
        {
            if(!needed)return;
            var site=sites.FirstOrDefault(c=>c.Kind==kind);
            issues.Add(site==null?new("build-"+kind,why,Build:kind):new("supply-"+kind,why+" Inspect the existing workplace.",Workplace:site.Id));
        }
        bool Short(Resource r)=>stocks.Single(s=>s.Resource==r) is var s && s.ConstructionNeed>s.Available;
        NeedSite(BuildingKind.Sawmill,Short(Resource.Planks),"Plank orders exceed available planks.");
        NeedSite(BuildingKind.Quarry,Short(Resource.Stone),"Stone orders exceed available stone.");
        NeedSite(BuildingKind.Carpenter,PublicPlace==null && sites.Any(c=>c.ImprovementRequested),"Home improvements need an open workshop, planks and an accessible installation spot.");
        if(Short(Resource.Logs) && !Trees.Any(t=>t.Logs>0 || t.Growth<1 || t.NeedsPlanting))issues.Add(new("plant","Construction needs timber; mark new alders to plant.",Plant:true));
        var bakery=sites.FirstOrDefault(c=>c.Kind==BuildingKind.Bakery && c.Complete && !c.WorkPaused);
        if(bakery!=null && StoredGrain+sites.Sum(c=>c.InputGrain)==0 && !sites.Any(c=>c.Kind==BuildingKind.Farm))
            issues.Add(new("build-Farm","The bakery has no grain supply or planned farm.",Build:BuildingKind.Farm));
        return new(stocks,EdibleStored/Population,Math.Max(0,60-Food.MealClock),issues.ToArray());
    }
}

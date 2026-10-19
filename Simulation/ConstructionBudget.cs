using System;
using System.Linq;
namespace Inlanders.Simulation;
public sealed record MaterialBudget(Resource Material,int Planned,int Queued,int Free)
{
    public int Shortfall=>Math.Max(0,Planned+Queued-Free);
    public override string ToString()=>$"{Material}: {Planned} new + {Queued} queued; {Free} free"+(Shortfall>0?$" · need {Shortfall} more":" · covered");
}
public sealed partial class World
{
    public MaterialBudget[] ReadConstructionBudget(BuildingKind kind,int rows=0)
    {
        if(Creative)return Array.Empty<MaterialBudget>();
        var definition=Buildings.Get(kind);
        return new[]{definition.Material,Resource.Stone}.Distinct().Select(material=>{
            int cost=material==Resource.Stone?definition.StoneCost:kind==BuildingKind.VegetableField && rows>0?rows*2:definition.Cost;
            int queue=Cottages.Where(c=>!c.Complete && !c.ConstructionPaused && (material==Resource.Stone || c.Material==material)).Sum(c=>c.Remaining(material));
            int free=MaterialStores(material).Where(id=>Accessible(StorageAccess(id))).Sum(id=>AvailableMaterialAt(id,material));
            return new MaterialBudget(material,cost,queue,free);
        }).Where(b=>b.Planned>0).ToArray();
    }
    public string ConstructionBudgetText(BuildingKind kind,int rows=0)=>string.Join(" / ",ReadConstructionBudget(kind,rows).Select(b=>b.ToString()));
}

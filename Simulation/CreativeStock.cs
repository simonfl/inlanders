using System;
using System.Collections.Generic;
using System.Linq;

namespace Inlanders.Simulation;

public sealed partial class World
{
    public const int CreativeStockLimit=9999;
    private Dictionary<Resource,long> _creativeAdded=new(),_creativeRemoved=new();
    public long CreativeAdded(Resource kind)=>_creativeAdded.GetValueOrDefault(kind);
    public long CreativeRemoved(Resource kind)=>_creativeRemoved.GetValueOrDefault(kind);
    private long CreativeNet(Resource kind)=>CreativeAdded(kind)-CreativeRemoved(kind);
    public int CreativeCentralStock(Resource kind)=>kind switch
    {
        Resource.Logs or Resource.Planks or Resource.Stone=>MaterialAt(null,kind),
        Resource.Grain=>Food.Grain,_=>CentralFood(kind)
    };
    public int CreativeProtectedStock(Resource kind)=>kind switch
    {
        Resource.Logs or Resource.Planks or Resource.Stone=>ReservedMaterialAt(null,kind),
        Resource.Grain=>ReservedGrain,_=>FoodReservedAt(null,kind)
    };
    public string? CreativeStockProblem(Resource kind,int target)
    {
        if(!Creative)return "Resource setup is available in Creative only.";
        if(!Enum.IsDefined(kind))return "Choose a resource.";
        if(target<0 || target>CreativeStockLimit)return $"Choose a whole number from 0 to {CreativeStockLimit}.";
        int reserved=CreativeProtectedStock(kind);
        if(target<reserved)return $"At least {reserved} are reserved for workers. Let those trips finish before reducing stock further.";
        long delta=(long)target-CreativeCentralStock(kind);
        if((delta>0?CreativeAdded(kind)+delta:CreativeRemoved(kind)-delta)>1_000_000_000L)return "This settlement has reached its resource-edit accounting limit.";
        return null;
    }
    public bool SetCreativeCentralStock(Resource kind,int target)
    {
        if(CreativeStockProblem(kind,target)!=null)return false;
        int delta=target-CreativeCentralStock(kind);if(delta==0)return true;
        if(kind is Resource.Logs or Resource.Planks or Resource.Stone)ChangeMaterial(null,kind,delta);
        else if(kind==Resource.Grain)Food.Grain=target;
        else ChangeCentralFood(kind,delta);
        if(delta>0)_creativeAdded[kind]=CreativeAdded(kind)+delta;else _creativeRemoved[kind]=CreativeRemoved(kind)-delta;
        History.Add($"Creative resource setup: central {kind} set to {target} ({delta:+#;-#;0}).");_retry=0;return true;
    }
    private void ValidateCreativeStock()
    {
        if(_creativeAdded==null || _creativeRemoved==null || (!Creative && (_creativeAdded.Count>0 || _creativeRemoved.Count>0)) ||
            _creativeAdded.Concat(_creativeRemoved).Any(p=>!Enum.IsDefined(p.Key) || p.Value is <=0 or >1_000_000_000L))
            throw new InvalidOperationException("Invalid Creative resource ledger");
    }
}

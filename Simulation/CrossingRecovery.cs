using System;
using System.Linq;

namespace Inlanders.Simulation;

public sealed partial class World
{
    public static readonly Cell RecoveryCrossing = new(0,-3);

    public static World NewCrossingRecovery()
    {
        var w = NewFirstHome();
        w.Founding!.FirstHome!.Recovery = true;
        w.Trees.RemoveAll(t => t.Cell.Z is >= -11 and <= -5 && t.Cell.X is >= -9 and <= -5);
        w.Trees.RemoveAll(t => t.Cell.Z is >= -4 and <= -2);
        w.Map.Wildlife[0].Cell = new(-4,-8);
        w.Bushes[0].Cell = new(-10,-7); w.Bushes[1].Cell = new(3,-9);
        var home = w.Place(new(2,2),0,BuildingKind.Cottage) ?? throw new Exception("Recovery home rejected");
        home.Delivered=home.Required; home.Construction=1;
        var field = w.Place(new(-7,-6),0,BuildingKind.VegetableField) ?? throw new Exception("Recovery field rejected");
        field.Delivered=field.Required; field.Construction=1;
        field.Planted=true; field.Growth=1; field.Harvest=VegetableYield(field); w.Food.GrownVegetables=field.Harvest;
        // A broad channel with one narrow former crossing. Both banks remain ordinary ground.
        for(int x=w.Map.MinX;x<=w.Map.MaxX;x++)
            for(int z=-4;z<=-2;z++)
                if(x!=0 || z==-3)w.Map.Water.Add(new(x,z));
        w.Founding.StartingBuildings=w.Cottages.Select(c=>new StartingBuilding{Id=c.Id,Cell=c.Cell,Rotation=c.Rotation,Kind=c.Kind}).ToList();
        w._yardLogs=4;
        w.InitialLogs=w.Trees.Sum(t=>t.Logs)+w.Cottages.Sum(c=>c.Delivered)+w._yardLogs;
        w.Food.InitialBerries=w.Food.Berries=8;
        w.Map.Name="Le passage emporté · The broken crossing";
        w.History.Clear();
        w.History.Add("The flood spared both homes, but swept away the crossing. Twenty ripe portions wait in the northern field. Four logs and eight portions remain here. Repair access and bring the harvest home, or grow food on this bank. Everyone already has a home.");
        w.ReconcileHomes(); w.Validate(); w.ValidateMapOccupancy();
        return w;
    }

    private FirstHomeObjectives ReadCrossingRecoveryObjectives()
    {
        var story=Founding!.FirstHome!;
        var food=Cottages.Where(c=>!c.DemolitionRequested && ProductionOutput(c.Kind) is {} k && EdibleKinds.Contains(k)).ToArray();
        bool access=food.Any(c=>c.Complete && Accessible(c.Entrance));
        bool delivered=DeliveredEdible>0;
        var pending=Cottages.FirstOrDefault(c=>c.Kind==BuildingKind.Bridge && !c.Complete && !c.DemolitionRequested) ?? food.FirstOrDefault(c=>!c.Complete);
        string action="recover", hint="Both homes are safe. Repair access to the ripe field, or grow food on this bank.";
        int? site=null;
        string route=access?"✓ Food route restored":"○ Food route · choose a plan";
        if(story.Complete){action="done";hint="All four neighbors have eaten fresh food. The village has a way to feed itself again.";access=delivered=true;route="✓ Food route restored";}
        else if(pending!=null)
        {
            route=$"○ {(pending.Kind==BuildingKind.Bridge?"Crossing":"Local food")} · "+(pending.MaterialsReady?$"building {pending.Construction:P0}":$"timber {pending.Delivered}/{pending.Required}");
            action="inspect";site=pending.Id;
            hint=pending.ConstructionPaused?"Construction is paused. Inspect the plan to resume it.":"Villagers collect materials and finish the plan automatically.";
            if(!pending.ConstructionPaused && !pending.MaterialsReady && Available<pending.Remaining(Resource.Logs) && !Trees.Any(t=>!t.Preserved && t.Logs>0 && Accessible(t.Access)))
            {action="grove";hint=$"The plan needs {pending.Remaining(Resource.Logs)-Available} more logs than stores cover. Work a grove on this bank.";}
        }
        else if(access && !delivered)
        {
            var source=food.First(c=>c.Complete && Accessible(c.Entrance));site=source.Id;action="inspect";
            hint=source.Kind==BuildingKind.VegetableField?"The crossing is open. Watch villagers reach the field and bring the ripe harvest home.":ReadWorkplace(source).State+". The new crop must grow before food can be delivered.";
        }
        else if(delivered){action="watch";hint="Fresh food has arrived. Each neighbor collects a meal when hungry; starting provisions do not count.";}
        return new(route,delivered?"✓ Fresh food delivered":"○ Fresh food · none delivered",$"{(story.Complete?"✓":"○")} Neighbors fed · {story.Served.Count}/4",hint,action,site,(access?1:0)+(delivered?1:0)+(story.Complete?1:0));
    }
}

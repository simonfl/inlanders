using System;
using System.Linq;

namespace Inlanders.Simulation;

public sealed partial class World
{
    public static readonly ShipmentOption[] ReliefShipments = {
        new("food","Prepare their supplies",16,48,"Anse aux Saules lost its crop. Send food for the families and logs to repair their garden fences. Packing is reversible; keep developing your own food supply.")
    };

    public static World NewHarvestRelief()
    {
        var w=NewVillageStory();
        w.Cottages.Clear();w.Trees.Clear();w.Bushes.Clear();w.ManagedWoodland.Clear();w._nextSite=1;w._nextTree=0;
        w.Map=new(){Name="Une récolte à partager · A harvest to share",MinX=-12,MinZ=-12,Width=25,Depth=23};
        for(int x=-12;x<=12;x++)for(int z=-12;z<=10;z++)
        {
            if(x>=8 || z==-1 && x>=-4)w.Map.Water.Add(new(x,z));
            else if(x<-9 && z>0 || x<-10 && z<-7 || z==10 && x<3 || z==-12 && x<0)w.Map.Excluded.Add(new(x,z));
        }
        w.Map.FishingGrounds.Add(new(){Id=0,Name="Village shallows",Cell=new(10,5),Capacity=10,Stock=10,RegrowthPerSecond=1f/30});
        w.Map.FishingGrounds.Add(new(){Id=1,Name="Outer reach",Cell=new(10,-8),Capacity=20,Stock=20,RegrowthPerSecond=1f/12});
        w.Bushes.Add(new(){Id=0,Cell=new(-9,-8)});
        foreach(var p in w.People){p.HomeId=null;p.Position=w.YardAccess.Point;}
        foreach(var name in new[]{"Anne","Jacques","Madeleine","Étienne"})
            w.People.Add(new(){Id=w.Population,Name=name,Position=w.YardAccess.Point,Role=Role.Unassigned,SharedWorker=true,NextMealTime=20+w.Population*5,Status="At home — available for work"});
        Cottage Built(Cell c,BuildingKind kind,int rotation=0)
        {
            var b=w.Place(c,rotation,kind)??throw new InvalidOperationException($"Relief setup {kind} at {c}: {w.PlacementProblem(c,rotation,kind)}");
            b.Delivered=b.Required;b.Construction=1;return b;
        }
        Built(new(-5,2),BuildingKind.Cottage);Built(new(1,2),BuildingKind.Cottage);
        Built(new(-6,7),BuildingKind.Cottage);Built(new(2,7),BuildingKind.Cottage);
        Built(new(7,4),BuildingKind.FishingDock,1);
        foreach(var c in new[]{new Cell(-6,4),new(-1,5),new(-3,6),new(4,4),new(5,7),new(5,2),new(-6,-5),new(-7,-2),new(-2,-5),new(1,-6)})
            w.Trees.Add(new(){Id=w._nextTree++,Cell=c,Logs=8,Preserved=true});
        w.SetHarvestGrove(new(-6,4));w.SetHarvestGrove(new(-7,-2));
        w._yardLogs=12;w.Food.InitialBerries=w.Food.Berries=24;
        w.InitialLogs=w.Trees.Sum(t=>t.Logs)+w.Cottages.Sum(c=>c.Delivered)+w._yardLogs;
        w.VillageStory=new(){Situation="harvest-relief",Established=true,Arrived=true,Settled=true};
        w.Riverside=new();
        w.Founding!.StartingBuildings=w.Cottages.Select(c=>new StartingBuilding{Id=c.Id,Cell=c.Cell,Kind=c.Kind,Rotation=c.Rotation}).ToList();
        w.ReconcileHomes();w.History.Clear();
        w.History.Add("The eight people of this fishing hamlet already have homes. Upriver, Anse aux Saules has lost its crop. Prepare 48 food and 16 logs so they can eat and replant. Your one dock cannot supply everyone and a relief boat alone: develop a food surplus. Clear gardens near home, cultivate the inland meadow, or reach more fishing water. No newcomers or deadline; shortages can be recovered.");
        w.Validate();w.ValidateMapOccupancy();return w;
    }

    private VillageStoryReport ReadHarvestRelief()
    {
        var s=VillageStory!;var request=ReliefShipments[0];
        int logs=s.Departed?s.ExportedLogs:s.PackedLogs,food=s.Departed?s.ExportedFood.Values.Sum():s.PackedFood.Values.Sum();
        bool ordered=s.Manifest!="none";
        string phase=s.Complete?"complete":s.Departed?"departing":!ordered?"choose-cargo":s.ReadyAnnounced?"ready":"packing";
        int chapter=s.Departed || s.ReadyAnnounced?3:ordered?2:1;
        string mark(bool b)=>b?"✓":"○";
        string title=s.Complete?"✓ A harvest shared":s.Departed?"Relief is on its way":!ordered?"Help the families at Anse aux Saules":"Grow enough for two villages";
        string[] objectives={
            $"{mark(food>=request.Food)} Relief food · {food}/{request.Food}",
            $"{mark(logs>=request.Logs)} Repair logs · {logs}/{request.Logs}",
            s.Departed?(s.Complete?"✓ Supplies sent · your village still has a home":"→ Relief boat departing"):$"{mark(People.All(p=>p.Fed) && ReachableEdibleStored>=StoryFoodFloor)} Your village · fed {People.Count(p=>p.Fed)}/8 · food kept {ReachableEdibleStored}/{StoryFoodFloor}"
        };
        string hint=s.Complete?"Your hamlet supplied another holding through a failed harvest. Keep tending this place or try the other village story.":
            s.Departed?"The families upriver will eat while they replant. Watch your supplies leave the bay.":
            !ordered?"Eight people already have homes. The existing dock helps with meals, but relief needs a surplus. Choose gardens near home, fields inland or another fishing route. Open the request when ready to pack.":
            $"Packing leaves {StoryFoodFloor} available food before taking more. Your people still eat that reserve. Add food production or shorten deliveries; cancel cargo to recover supplies for construction.";
        if(People.Any(p=>!p.Fed) && !s.Complete)hint="Your neighbors are hungry. Keep food work running; cancel packing if you need its supplies back. "+hint;
        return new(phase,title,objectives,hint,chapter,new[]{"Plan the relief","Grow a surplus","Send help upriver"},s.Notice,s.Event,0,"Everyone already lives here.",s.Manifest,
            request.Logs,request.Food,logs,food,StoryFoodFloor,StoryOrderProblem(),StorySendProblem(),s.Departed,
            s.Departed?Math.Clamp((Food.Time-s.DepartedAt)/StoryBoatJourney,0,1):0,
            new[]{"Anse aux Saules can eat while they replant",$"{s.ExportedFood.Values.Sum()} food and {s.ExportedLogs} repair logs sent upriver",$"Your eight neighbors kept their home · {Cottages.Count(c=>c.Complete && c.Kind is BuildingKind.VegetableGarden or BuildingKind.VegetableField or BuildingKind.FishingDock)} food workplaces"},ReliefShipments,StorySupplyText());
    }
}

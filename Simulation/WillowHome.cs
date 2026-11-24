using System;
using System.Linq;

namespace Inlanders.Simulation;

public sealed record WillowHomeReport(string Focus,string Progress,string? SupperProblem,bool Gathering,int Gathered,int Restored,int Repairs,int MealCost,int KeepFood);

public sealed partial class World
{
    public static World NewWillowHome()
    {
        var w=NewHarvestRelief();
        w.Cottages.Clear();w.Trees.Clear();w.Bushes.Clear();w.ManagedWoodland.Clear();w._nextSite=1;w._nextTree=0;
        w.VillageStory=new(){Situation="willow-home",Arrived=true};w.Riverside=new();
        w.Map=new(){Name="Chez nous, aux Saules · Home again at Les Saules",MinX=-10,MinZ=-10,Width=23,Depth=21};
        for(int x=-10;x<=12;x++)for(int z=-10;z<=10;z++)
        {
            if(x>=8 || z==-1)w.Map.Water.Add(new(x,z));
            else if(x<-7 && z>0 || z==10 || z<-8 && x<-6)w.Map.Excluded.Add(new(x,z));
        }
        w.Map.FishingGrounds.Add(new(){Id=0,Name="Willow shallows",Cell=new(10,4),Capacity=18,Stock=18,RegrowthPerSecond=1f/6});
        w.Map.FishingGrounds.Add(new(){Id=1,Name="Upper river",Cell=new(10,-6),Capacity=12,Stock=12,RegrowthPerSecond=1f/15});
        w.Bushes.Add(new(){Id=0,Cell=new(-9,-7)});
        foreach(var p in w.People){p.HomeId=null;p.Position=w.YardAccess.Point;}
        Cottage Built(Cell cell,BuildingKind kind,int rotation=0)
        {
            var b=w.Place(cell,rotation,kind)??throw new InvalidOperationException($"Willow setup {kind} {cell}: {w.PlacementProblem(cell,rotation,kind)}");
            b.Delivered=b.Required;b.Construction=1;return b;
        }
        Built(new(-5,2),BuildingKind.Cottage);Built(new(1,2),BuildingKind.Cottage);
        var crossing=Built(RiversideShortcut,BuildingKind.Bridge);
        var west=Built(new(-6,-5),BuildingKind.Cottage);var east=Built(new(3,-6),BuildingKind.Cottage);
        var field=Built(new(-3,-6),BuildingKind.VegetableField);field.Planted=true;field.Growth=1;field.Harvest=12;field.HarvestThenPause=true;w.Food.GrownVegetables=12;
        w.VillageStory.SalvageField=field.Id;
        var dock=Built(new(7,4),BuildingKind.FishingDock,1);
        void Damage(Cottage b,int salvage)
        {b.Construction=0;b.Delivered=salvage;b.ConstructionPaused=true;w.VillageStory.Ruins.Add(b.Id);}
        Damage(west,0);Damage(east,0);Damage(dock,4);Damage(crossing,0);w.VillageStory.Ruins.Add(field.Id);
        foreach(var cell in new[]{new Cell(-6,4),new(-1,6),new(5,7),new(5,2),new(-6,-8),new(-7,-2),new(1,-9),new(5,-8),new(4,-3),new(6,-6)})
            w.Trees.Add(new(){Id=w._nextTree++,Cell=cell,Logs=8,Preserved=true});
        w.SetHarvestGrove(new(-6,4));
        w._yardLogs=8;w.Food.InitialBerries=w.Food.Berries=24;
        w.InitialLogs=w.Trees.Sum(t=>t.Logs)+w.Cottages.Sum(c=>c.Delivered)+w._yardLogs;
        w.Founding!.StartingBuildings=w.Cottages.Select(c=>new StartingBuilding{Id=c.Id,Cell=c.Cell,Kind=c.Kind,Rotation=c.Rotation}).ToList();
        w.ReconcileHomes();
        foreach(var p in w.People)
        {
            p.Position=p.HomeId is int home?w.Cottages.Single(c=>c.Id==home).Entrance.Point:new Cell(-2+p.Id%3,5).Point;
            p.Status=p.HomeId==null?"Staying with neighbors — our roof needs rebuilding":"At home — ready to help";
        }
        w.History.Clear();w.History.Add("The supplies you sent kept Les Saules going. Across the broken crossing: two roofless homes, a woodlot and twelve portions in the last standing crop. Its damaged ground will need six logs of restoration after harvest. Reopen that holding or build a new livelihood on this bank. Bring home twenty-four locally produced portions: eight to share, sixteen to keep.");
        w.Validate();w.ValidateMapOccupancy();return w;
    }

    public bool IsWillowRuin(Cottage site)=>IsWillowHome && VillageStory!.Ruins.Contains(site.Id) && !site.Complete;
    public bool WillowSiteAccessible(Cottage site)=>Accessible(site.Entrance);
    public string? RestoreWillowProblem(int id)
    {
        if(!IsWillowHome)return "This restoration belongs to Les Saules.";
        var site=Cottages.FirstOrDefault(c=>c.Id==id);
        if(site==null || !IsWillowRuin(site))return "Choose a damaged home, landing or crossing.";
        if(Food.Celebrating)return "The neighbors are sharing supper.";
        return site.ConstructionPaused?null:"Restoration is already underway.";
    }
    public string? RestoreWillow(int id)
    {
        if(RestoreWillowProblem(id) is {} problem)return problem;
        if(id==VillageStory!.SalvageField){var field=Cottages.Single(c=>c.Id==id);field.WorkPaused=false;field.HarvestThenPause=false;field.Planted=false;field.Growth=0;}
        SetConstructionPaused(id,false);History.Add("Restoration ordered. Neighbors deliver logs and rebuild the existing site.");return null;
    }
    public string? PauseWillow(int id)
    {
        var site=Cottages.FirstOrDefault(c=>c.Id==id);
        if(site==null || !IsWillowRuin(site) || site.ConstructionPaused || !SetConstructionPaused(id,true))return "Choose an active restoration to pause.";
        History.Add("Restoration paused. Materials already delivered stay at the site; uncollected logs are released.");return null;
    }
    private bool WillowLivelihood => DeliveredEdible>0 && Cottages.Any(c=>c.Complete && c.Kind is BuildingKind.VegetableGarden or BuildingKind.VegetableField or BuildingKind.FishingDock && Accessible(c.Entrance) && !c.WorkPaused && (c.Id!=VillageStory!.SalvageField || VillageStory.SalvageExhausted));
    private int WillowAvailableFood=>EdibleKinds.Where(k=>k!=Resource.Berries).Sum(CentralFoodAvailable);
    public string? WillowSupperProblem() => !IsWillowHome?"This supper belongs to Les Saules.":
        Food.SupperComplete?"The neighbors have already shared supper.":Food.Celebrating?"The neighbors are gathering.":
        Housed<Population?$"Homes for {Housed}/{Population} neighbors. Restore a cottage or build a new one.":
        !WillowLivelihood?"Bring home fresh food from a working field, garden or landing.":
        WillowAvailableFood<Population*3?$"Local food {WillowAvailableFood}/{Population*3} · eight to share, sixteen to keep. Relief provisions do not count.":
        SupperSpots().Count!=Population?"Leave clear space around the log yard for supper.":null;
    public string? BeginWillowSupper()
    {
        if(WillowSupperProblem() is {} problem)return problem;
        MeetingSpots.Clear();MeetingSpots.AddRange(SupperSpots());
        int remaining=Population;
        foreach(var kind in EdibleKinds.Where(k=>k!=Resource.Berries))
        {
            int count=Math.Min(remaining,CentralFoodAvailable(kind));if(count==0)continue;
            ChangeCentralFood(kind,-count);remaining-=count;
            switch(kind){case Resource.Berries:Food.EatenBerries+=count;break;case Resource.Vegetables:Food.EatenVegetables+=count;break;case Resource.Fish:Food.EatenFish+=count;break;case Resource.Bread:Food.EatenBread+=count;break;case Resource.Fruit:Food.EatenFruit+=count;break;case Resource.Game:Food.EatenGame+=count;break;}
        }
        foreach(var p in People)Interrupt(p);
        Food.Celebrating=true;Food.MeetingClock=0;_retry=0;
        History.Add("Eight portions are set out. Sixteen remain for tomorrow. The neighbors are coming to supper.");return null;
    }
    private void AdvanceWillowHome()
    {
        var s=VillageStory!;if(s.Complete)return;
        var oldField=Cottages.First(c=>c.Id==s.SalvageField);
        if(!s.SalvageExhausted && oldField.Harvest==0 && !People.Any(p=>p.WorkplaceId==oldField.Id))
        {
            s.SalvageExhausted=true;oldField.RestorationCost=oldField.Required+6;oldField.Construction=0;oldField.ConstructionPaused=true;oldField.Planted=false;oldField.Growth=0;
            History.Add("The last crop is recovered. Restore the damaged field for six logs, or grow tomorrow's food elsewhere.");
        }
        if(!s.Established && WillowLivelihood){s.Established=true;History.Add("Fresh food is reaching the village again.");}
        if(!s.Settled && Housed==Population){s.Settled=true;History.Add("Every neighbor has a home again.");}
        if(Food.SupperComplete){s.Complete=true;StoryEvent("Les Saules is home again. The table is shared, the roofs are sound, and tomorrow's food is waiting.");}
    }
    public WillowHomeReport? ReadWillowHomeProgress()
    {
        if(!IsWillowHome)return null;
        string focus=VillageStory!.Complete?"Les Saules is home again":Food.Celebrating?"The neighbors are coming home":!WillowLivelihood?(DeliveredEdible>0?"Grow the next harvest":"Bring fresh food home"):Housed<Population?"Put roofs over your neighbors":"Set the table for eight";
        string progress=Food.Celebrating?$"At supper · {People.Count(p=>p.Task==Work.Supper)}/{Population}":!WillowLivelihood?(VillageStory.SalvageExhausted?"The old crop bought time. Restore the field or build another food source.":"Rescue the last crop, repair the landing, or plant nearby."):Housed<Population?$"Neighbors housed · {Housed}/{Population}":$"Local food · {WillowAvailableFood}/{Population*3} · 8 to share, 16 to keep";
        return new(focus,progress,WillowSupperProblem(),Food.Celebrating,People.Count(p=>p.Task==Work.Supper),VillageStory.Ruins.Count(id=>(id!=VillageStory.SalvageField || VillageStory.SalvageExhausted) && Cottages.Any(c=>c.Id==id && c.Complete)),VillageStory.Ruins.Count,Population,Population*2);
    }
    private VillageStoryReport ReadWillowHome()
    {
        var s=VillageStory!;var r=ReadWillowHomeProgress()!;
        string Mark(bool b)=>b?"✓":"○";
        int chapter=!WillowLivelihood?1:Housed<Population?2:3;
        return new(s.Complete?"complete":Food.Celebrating?"gathering":chapter==1?"livelihood":chapter==2?"homes":"supper",r.Focus,
            new[]{$"{Mark(WillowLivelihood)} Fresh food · {(WillowLivelihood?"reaching home":"restore a route or choose a food source")}",$"{Mark(Housed==Population)} Homes · {Housed}/{Population}",$"{Mark(Food.SupperComplete)} Supper · {(Food.SupperComplete?"shared":$"{WillowAvailableFood}/{Population*3} food")}"},
            r.Progress,chapter,new[]{"Food flowing","Homes restored","Supper together"},s.Notice,s.Event,0,StoryWelcomeProblem(),"none",0,0,0,0,Population*2,StoryOrderProblem(),"This level has no outbound shipment.",false,0,
            new[]{"Eight neighbors have a home again",$"{r.Restored} old sites restored · {Cottages.Count(c=>c.Complete && c.Kind is BuildingKind.VegetableGarden or BuildingKind.VegetableField or BuildingKind.FishingDock)} food workplaces",$"Eight portions shared · {ReachableEdibleStored} food at home"},Array.Empty<ShipmentOption>(),StorySupplyText());
    }
    private void ValidateWillowHome()
    {
        var s=VillageStory!;
        if(Riverside==null || Population!=8 || !s.Arrived || s.Ruins==null || s.Ruins.Count!=5 || s.Ruins.Distinct().Count()!=5 || s.Ruins.Any(id=>!Cottages.Any(c=>c.Id==id)) || !s.Ruins.Contains(s.SalvageField) || Cottages.Any(c=>c.RestorationCost<0 || c.RestorationCost>32 || c.RestorationCost>0 && c.Id!=s.SalvageField) || s.Manifest!="none" || s.PackedLogs!=0 || s.ExportedLogs!=0 || s.PackedFood.Count!=0 || s.ExportedFood.Count!=0 || s.Departed || s.Complete&&!Food.SupperComplete || s.FreshMeals.Any(id=>id<0 || id>=Population))
            throw new InvalidOperationException("Invalid Les Saules restoration");
    }
}

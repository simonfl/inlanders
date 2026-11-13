using System;
using System.Collections.Generic;
using System.Linq;

namespace Inlanders.Simulation;

public sealed class RiversidePlan
{
    public int Id { get; set; }
    public Cell Cell { get; set; }
    public int Rotation { get; set; }
    public BuildingKind Kind { get; set; }
    public Cell[] Clearing { get; set; } = Array.Empty<Cell>();
    public Cell[] Preserved { get; set; } = Array.Empty<Cell>();
    public Cell[] Renewing { get; set; } = Array.Empty<Cell>();
}
public sealed class RiversideProgress
{
    public bool FarGarden { get; set; }
    public bool Arrived { get; set; }
    public bool Complete { get; set; }
    public int ArrivalDeliveries { get; set; }
    public HashSet<int> Served { get; set; } = new();
    public List<RiversidePlan> Plans { get; set; } = new();
    public int NextPlan { get; set; } = -1000;
}
public sealed record RiversideReport(string Phase, string Title, string[] Objectives, string Hint, int ArrivalIn,
    string? WelcomeProblem, Cell Shortcut, bool ShortcutBuilt, int WalkNow, int WalkWithShortcut, int FreshMeals, int Delivered, int Woodland);

public sealed partial class World
{
    public RiversideProgress? Riverside { get; private set; }
    public static readonly Cell RiversideShortcut = new(0,-1);
    public const int RiversideArrival = 180;

    public static World NewRiverside(bool farGarden=false)
    {
        var w=NewFirstHome();
        w.Cottages.Clear();w.Trees.Clear();w.Bushes.Clear();w._nextSite=1;w._nextTree=0;
        w.ManagedWoodland.Clear();
        foreach(var p in w.People){p.HomeId=null;p.Position=w.YardAccess.Point;}
        w.Map=new(){Name="Une place au bord de l'eau · Room by the river",MinX=-10,MinZ=-10,Width=23,Depth=20};
        for(int x=-10;x<=12;x++)for(int z=-10;z<=9;z++)
        {
            if(x>=8 || z==-1)w.Map.Water.Add(new(x,z));
            else if(z>=0 && (x < -8 || z>=9) || z < -7 && x < -8)w.Map.Excluded.Add(new(x,z));
        }
        w.Map.FishingGrounds.Add(new(){Id=0,Name="River shallows",Cell=new(10,4),Capacity=16,Stock=16,RegrowthPerSecond=.1f});
        w.Map.FishingGrounds.Add(new(){Id=1,Name="Upper reach",Cell=new(10,-6),Capacity=16,Stock=16,RegrowthPerSecond=1f/15});
        w.Bushes.Add(new(){Id=0,Cell=new(-9,-6)});
        Cottage Built(Cell c,BuildingKind kind)
        {
            var b=w.Place(c,0,kind)??throw new InvalidOperationException($"Riverside setup {kind}: {w.PlacementProblem(c,0,kind)}");
            b.Delivered=b.Required;b.Construction=1;return b;
        }
        Built(new(-5,2),BuildingKind.Cottage);Built(new(1,2),BuildingKind.Cottage);
        Built(new(-8,-1),BuildingKind.Bridge);
        var garden=Built(farGarden?new(-3,-6):new(-6,6),BuildingKind.VegetableGarden);garden.Planted=true;garden.Growth=.7f;
        foreach(var cell in new[]{new Cell(2,5),new(5,5),new(-6,8),new(-3,8),new(0,8),new(3,8),new(-7,-8),new(5,-8)})
            w.Trees.Add(new(){Id=w._nextTree++,Cell=cell,Logs=8,Preserved=true});
        foreach(var c in new[]{new Cell(2,5),new(5,5),new(0,8),new(3,8)})w.SetHarvestGrove(c);
        w._yardLogs=10;w.InitialLogs=w.Trees.Sum(t=>t.Logs)+w.Cottages.Sum(c=>c.Delivered)+w._yardLogs;
        w.Food.InitialBerries=w.Food.Berries=12;
        w.Founding!.FirstHome=new(){Recovery=true};w.Riverside=new(){FarGarden=farGarden};
        w.Founding.StartingBuildings=w.Cottages.Select(c=>new StartingBuilding{Id=c.Id,Cell=c.Cell,Kind=c.Kind,Rotation=c.Rotation}).ToList();
        w.ReconcileHomes();w.History.Clear();
        w.History.Add("Four neighbors will join this working village in three minutes. Make room near the homes by clearing woodland, or keep the grove and use the open far bank. The old crossing works; a six-log shortcut is optional. No trader or final stability timer.");
        w.Validate();w.ValidateMapOccupancy();return w;
    }

    public string? RiversideWelcomeProblem()=>Riverside is not {Arrived:false}?"Everyone has arrived.":Beds<8?"Finish eight beds before welcoming them early.":ReachableEdibleStored<8?"Set aside eight portions before welcoming them early.":null;
    public bool WelcomeRiverside()
    {
        if(RiversideWelcomeProblem()!=null)return false;
        ArriveRiverside();return true;
    }
    private void ArriveRiverside()
    {
        var r=Riverside!;r.Arrived=true;r.ArrivalDeliveries=DeliveredEdible;
        foreach(var name in new[]{"Anne","Jacques","Madeleine","Étienne"})
            People.Add(new(){Id=Population,Name=name,Position=YardAccess.Point,Role=Role.Unassigned,SharedWorker=true,NextMealTime=Food.Time+20,Status="Finding our new home"});
        ReconcileHomes();_retry=0;
        History.Add("Four neighbors have arrived. Help each settle into a home and eat a fresh meal. Bring home sixteen local portions and leave eight in reserve.");
    }
    public Cell[] RiversideRoute(bool shortcut)=>FindPath(YardAccess,new Cell(1,-5),c=>!(shortcut && c==RiversideShortcut) && Blocked(c))?.ToArray() ?? Array.Empty<Cell>();
    public string? RiversideGroveProblem(Cell first,Cell last)=>Riverside?.Plans.Any(p=>p.Clearing.Any(c=>c.X>=Math.Min(first.X,last.X) && c.X<=Math.Max(first.X,last.X) && c.Z>=Math.Min(first.Z,last.Z) && c.Z<=Math.Max(first.Z,last.Z)))==true ? "This grove includes a building's clearing order. Cancel that building plan before renewing these trees." : null;
    public string? RiversideSiteProblem(Cell at,int rotation,BuildingKind kind)
    {
        if(Riverside==null)return "This plan belongs to Room by the river.";
        if(kind is not (BuildingKind.Cottage or BuildingKind.VegetableGarden or BuildingKind.VegetableField or BuildingKind.FishingDock or BuildingKind.Bridge))return "Choose a home, food source or shortcut.";
        if(kind==BuildingKind.Bridge && at!=RiversideShortcut)return "Use the marked shortcut; the old crossing already works.";
        var occupied=Footprint(at,rotation,kind).Append(Door(at,rotation)).ToHashSet();
        if(Riverside.Plans.Any(p=>Footprint(p.Cell,p.Rotation,p.Kind).Append(Door(p.Cell,p.Rotation)).Any(occupied.Contains)))return "A ground-clearing plan already uses this space.";
        return PlacementProblem(at,rotation,kind) is {} problem && PreviewGroundPreparation(at,rotation,kind)==null?problem:null;
    }
    public bool PlanRiverside(Cell at,int rotation,BuildingKind kind)
    {
        if(RiversideSiteProblem(at,rotation,kind)!=null)return false;
        var preparation=PreviewGroundPreparation(at,rotation,kind);
        if(preparation==null)return Place(at,rotation,kind)!=null;
        var preserved=preparation.Trees.Where(c=>Trees.Any(t=>t.Cell==c && t.Preserved)).ToArray();
        var renewing=preparation.Trees.Where(ManagedWoodland.Contains).ToArray();
        if(!PrepareBuildingGround(at,rotation,kind))return false;
        Riverside!.Plans.Add(new(){Id=Riverside.NextPlan--,Cell=at,Rotation=rotation,Kind=kind,Clearing=preparation.Trees,Renewing=renewing,Preserved=preserved});
        History.Add($"{kind} ordered on wooded ground: clear {preparation.Trees.Length} tree spots, then build automatically. {renewing.Length} renewing grove spots will be lost.");
        return true;
    }
    public bool CancelRiversidePlan(int id)
    {
        var plan=Riverside?.Plans.FirstOrDefault(p=>p.Id==id);if(plan==null)return false;
        foreach(var c in plan.Clearing)if(Trees.Any(t=>t.Cell==c && t.ClearRequested))SetClearing(c,false);
        foreach(var c in plan.Preserved)if(PreserveTreeProblem(c)==null)SetTreePreserved(c,true);
        foreach(var c in plan.Renewing)if(ManagedWoodlandProblem(c)==null)SetHarvestGrove(c);
        Riverside!.Plans.Remove(plan);History.Add("Clearing/building plan cancelled. Felled timber stays felled; the former managed grove is renewed where possible.");return true;
    }
    private void AdvanceRiverside()
    {
        if(Riverside is not {} r)return;
        foreach(var plan in r.Plans.ToArray())
            if(!Trees.Any(t=>plan.Clearing.Contains(t.Cell)) && PlacementProblem(plan.Cell,plan.Rotation,plan.Kind)==null)
            {if(Place(plan.Cell,plan.Rotation,plan.Kind)!=null){r.Plans.Remove(plan);History.Add("Ground cleared. Construction now follows the original plan.");}}
        if(!r.Arrived && Food.Time>=RiversideArrival)ArriveRiverside();
        if(!r.Complete && r.Arrived && Housed==8 && People.All(p=>p.Fed) && r.Served.Count==4 && DeliveredEdible-r.ArrivalDeliveries>=16 && ReachableEdibleStored>=8)
        {r.Complete=true;History.Add("Eight neighbors have a home here. The newcomers have eaten fresh local food, with food still in reserve. Keep tending this place if you like.");}
    }
    public RiversideReport ReadRiverside()
    {
        var r=Riverside!;int delivered=r.Arrived?DeliveredEdible-r.ArrivalDeliveries:0;
        var destination=new Cell(1,-5);
        int walk=FindPath(YardAccess,destination,Blocked)?.Count??0;
        int shortcut=FindPath(YardAccess,destination,c=>c!=RiversideShortcut && Blocked(c))?.Count??0;
        bool built=Cottages.Any(c=>c.Kind==BuildingKind.Bridge && c.Cell==RiversideShortcut && c.Complete);
        string phase=r.Complete?"complete":!r.Arrived?"prepare":Housed<8?"settle":"feed";
        string hint=r.Complete?"Your village has grown around your choices. Keep watching, or improve a route you use.":
            People.Any(p=>!p.Fed)?"Some neighbors are hungry. Add food close to useful routes; shortages slow work but can be recovered.":
            r.Plans.Count>0?"Neighbors are clearing the chosen ground, then will build automatically. Cleared grove spots will no longer renew trees.":
            !r.Arrived?"The near bank has useful woodland; the far bank has open ground. Choose where to make their home.":
            Housed<8?"Finish homes for the newcomers. The old crossing still works; the shortcut is optional.":
            $"Fresh meals {r.Served.Count}/4 newcomers; local food {delivered}/16 delivered. Keep eight portions for the next meal. The shortcut can shorten far-bank journeys.";
        return new(phase,r.Complete?"✓ Eight neighbors call this place home":"Make room for four neighbors",new[]{
            $"{(r.Complete || Beds>=8?"✓":"○")} Homes · {Beds}/8 beds"+(r.Arrived?$" · {Housed}/8 housed":""),
            r.Arrived?$"{(r.Complete || r.Served.Count==4?"✓":"○")} Newcomers' fresh meals · {r.Served.Count}/4":$"○ Neighbors arrive in {Math.Max(0,(int)Math.Ceiling(RiversideArrival-Food.Time))}s · or welcome when ready",
            r.Arrived?$"{(r.Complete || delivered>=16 && ReachableEdibleStored>=8 && People.All(p=>p.Fed)?"✓":"○")} Local food {delivered}/16 · reserve {ReachableEdibleStored}/8 · fed {People.Count(p=>p.Fed)}/8":$"{(ReachableEdibleStored>=8?"✓":"○")} Food for arrival · {ReachableEdibleStored}/8 portions"},hint,
            Math.Max(0,(int)Math.Ceiling(RiversideArrival-Food.Time)),RiversideWelcomeProblem(),RiversideShortcut,built,walk,shortcut,r.Served.Count,delivered,ManagedWoodland.Count);
    }
    private void ValidateRiverside()
    {
        if(Riverside is not {} r)return;
        if(r.Served==null || r.Plans==null || r.Served.Any(id=>id<4 || id>=Population) || Population!=(r.Arrived?8:4) || r.Complete&&!r.Arrived || r.ArrivalDeliveries<0 || r.ArrivalDeliveries>DeliveredEdible || r.NextPlan>-1000 || r.Plans.Select(p=>p.Id).Distinct().Count()!=r.Plans.Count || r.Plans.Any(p=>p.Clearing==null || p.Renewing==null || p.Preserved==null || !Map.Contains(p.Cell) || p.Renewing.Any(c=>!p.Clearing.Contains(c)) || p.Preserved.Any(c=>!p.Clearing.Contains(c)) || p.Id<=r.NextPlan || p.Id>-1000 || p.Rotation is <0 or >3 || p.Kind is not (BuildingKind.Cottage or BuildingKind.VegetableGarden or BuildingKind.VegetableField)))
            throw new InvalidOperationException("Invalid riverside progress");
    }
}

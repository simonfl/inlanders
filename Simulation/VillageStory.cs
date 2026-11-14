using System;
using System.Collections.Generic;
using System.Linq;

namespace Inlanders.Simulation;

public sealed class VillageStoryProgress
{
    public bool Established { get; set; }
    public bool Arrived { get; set; }
    public bool Settled { get; set; }
    public HashSet<int> FreshMeals { get; set; } = new();
    public string Manifest { get; set; } = "none";
    public int PackedLogs { get; set; }
    public Dictionary<Resource,int> PackedFood { get; set; } = new();
    public int ExportedLogs { get; set; }
    public Dictionary<Resource,int> ExportedFood { get; set; } = new();
    public bool ReadyAnnounced { get; set; }
    public bool Departed { get; set; }
    public float DepartedAt { get; set; }
    public bool Complete { get; set; }
    public int Event { get; set; }
    public string Notice { get; set; } = "";
}

public sealed record ShipmentOption(string Id,string Label,int Logs,int Food,string Description);
public sealed record VillageStoryReport(string Phase,string Title,string[] Objectives,string Hint,int Chapter,string[] Chapters,
    string Notice,int Event,int ArrivalIn,string? WelcomeProblem,string Manifest,int LogTarget,int FoodTarget,int PackedLogs,int PackedFood,
    int FoodFloor,string? OrderProblem,string? SendProblem,bool Departed,float DepartureProgress,string[] Summary,ShipmentOption[] Options);

public sealed partial class World
{
    public VillageStoryProgress? VillageStory { get; private set; }
    public const int StoryArrival = 480;
    public const float StoryBoatJourney = 12;
    public static readonly ShipmentOption[] StoryShipments = {
        new("building","Help them build",64,24,"More logging, less surplus food. Keep a renewing grove and enough food for this village."),
        new("food","Help them eat",24,64,"More surplus food, fewer logs. Invest in fields or fishing while keeping local meals safe.")
    };
    private ShipmentOption? StoryManifest => StoryShipments.FirstOrDefault(o=>o.Id==VillageStory?.Manifest);
    public int StoryFoodAway(Resource kind) => VillageStory is {} s ? s.PackedFood.GetValueOrDefault(kind)+s.ExportedFood.GetValueOrDefault(kind) : 0;
    public int StoryLogsAway => VillageStory is {} s ? s.PackedLogs+s.ExportedLogs : 0;
    public int StoryFoodFloor => Population*2;

    public static World NewVillageStory()
    {
        var w=NewRiverside();
        w.VillageStory=new();
        w.Cottages.RemoveAll(c=>c.Kind==BuildingKind.VegetableGarden);
        w.InitialLogs=w.Trees.Sum(t=>t.Logs)+w.Cottages.Sum(c=>c.Delivered)+w._yardLogs;
        w.Founding!.StartingBuildings=w.Cottages.Select(c=>new StartingBuilding{Id=c.Id,Cell=c.Cell,Kind=c.Kind,Rotation=c.Rotation}).ToList();
        w.Map.Name="Un chez-nous au bord du fleuve · A home on the river";
        w.History.Clear();
        w.History.Add("Four people have homes, twelve portions and ten logs. Establish fresh food before provisions dwindle. Four neighbors will join you in eight minutes, or earlier when ready. Together, prepare a boat of supplies for another riverside settlement. Food shortages are recoverable; the outbound boat waits for you.");
        w.Validate();w.ValidateMapOccupancy();return w;
    }

    private void StoryEvent(string notice)
    {
        VillageStory!.Event++;VillageStory.Notice=notice;History.Add(notice);
    }
    public string? StoryWelcomeProblem() => VillageStory is not {Arrived:false} ? "The neighbors have already arrived." :
        !VillageStory.Established ? "First help the four founders eat fresh local food." : Beds<8 ? "Finish eight beds before welcoming the neighbors early." :
        ReachableEdibleStored<12 ? "Keep twelve food portions for the arrival." : null;
    public string? WelcomeStory()
    {
        if(StoryWelcomeProblem() is {} problem)return problem;
        ArriveStory();return null;
    }
    private void ArriveStory()
    {
        var s=VillageStory!;s.Arrived=true;
        foreach(var name in new[]{"Anne","Jacques","Madeleine","Étienne"})
            People.Add(new(){Id=Population,Name=name,Position=YardAccess.Point,Role=Role.Unassigned,SharedWorker=true,NextMealTime=Food.Time+20,Status="Arrived — finding a home and useful work"});
        ReconcileHomes();_retry=0;
        StoryEvent("Anne, Jacques, Madeleine and Étienne have arrived. Help them settle and eat from this land. They bring news: another riverside holding could use supplies. Once everyone is settled, choose what your village can send.");
    }
    public string? StoryOrderProblem() => VillageStory is not {} s ? "This shipment belongs to A home on the river." :
        s.Departed ? "Your supply boat has already departed." : !s.Settled ? "First settle all eight neighbors into homes and fresh meals." :
        s.Manifest!="none" ? "Cancel the current cargo order before choosing another." : null;
    public string? OrderStoryShipment(string manifest)
    {
        if(StoryOrderProblem() is {} problem)return problem;
        if(!StoryShipments.Any(o=>o.Id==manifest))return "Choose building supplies or food supplies.";
        var s=VillageStory!;s.Manifest=manifest;s.ReadyAnnounced=false;
        History.Add($"Cargo ordered: {StoryManifest!.Logs} logs and {StoryManifest.Food} local food. Spare food above {StoryFoodFloor} portions is packed automatically; claimed meals and carried goods stay with their workers. New construction may wait for logs. Cancel to return packed supplies.");
        PackStoryShipment();_retry=0;return null;
    }
    public string? CancelStoryShipment()
    {
        if(VillageStory is not {Departed:false} s || s.Manifest=="none")return "There is no packed order to cancel.";
        int logs=s.PackedLogs,food=s.PackedFood.Values.Sum();
        _yardLogs+=logs;s.PackedLogs=0;
        foreach(var (kind,count) in s.PackedFood)ChangeCentralFood(kind,count);
        s.PackedFood.Clear();s.Manifest="none";s.ReadyAnnounced=false;
        History.Add($"Cargo cancelled: {logs} logs and {food} food returned. Choose a different contribution whenever ready.");_retry=0;return null;
    }
    private void PackStoryShipment()
    {
        if(VillageStory is not {Departed:false} s || StoryManifest is not {} manifest)return;
        int logs=Math.Min(manifest.Logs-s.PackedLogs,AvailableLogsAt(null));
        _yardLogs-=logs;s.PackedLogs+=logs;
        // Keep two unclaimed meals per resident in the central store. Never steal
        // an existing meal/haul reservation or export the opening provisions.
        int surplus=Math.Max(0,EdibleKinds.Sum(CentralFoodAvailable)-StoryFoodFloor);
        foreach(var kind in EdibleKinds.Where(k=>k!=Resource.Berries))
        {
            int count=Math.Min(Math.Min(surplus,CentralFoodAvailable(kind)),manifest.Food-s.PackedFood.Values.Sum());
            if(count<=0)continue;
            ChangeCentralFood(kind,-count);s.PackedFood[kind]=s.PackedFood.GetValueOrDefault(kind)+count;surplus-=count;
        }
        if(!s.ReadyAnnounced && s.PackedLogs==manifest.Logs && s.PackedFood.Values.Sum()==manifest.Food)
        {s.ReadyAnnounced=true;StoryEvent("The cargo is ready. Check that eight neighbors have homes, everyone is fed and sixteen food portions remain. Then send your boat upriver.");}
    }
    public string? StorySendProblem() => VillageStory is not {Departed:false} s ? "The supply boat has already departed." :
        StoryManifest is not {} manifest ? "Choose a cargo order first." :
        s.PackedLogs<manifest.Logs ? $"Pack {manifest.Logs-s.PackedLogs} more logs." :
        s.PackedFood.Values.Sum()<manifest.Food ? $"Pack {manifest.Food-s.PackedFood.Values.Sum()} more local food portions." :
        Housed<8 ? "Give all eight neighbors a home before sending the boat." : People.Any(p=>!p.Fed) ? "Let hungry neighbors eat before sending the boat." :
        ReachableEdibleStored<StoryFoodFloor ? $"Keep {StoryFoodFloor} food portions in the village; {ReachableEdibleStored} are ready." : null;
    public string? SendStoryShipment()
    {
        if(StorySendProblem() is {} problem)return problem;
        var s=VillageStory!;s.ExportedLogs=s.PackedLogs;s.PackedLogs=0;
        s.ExportedFood=new(s.PackedFood);s.PackedFood.Clear();s.Departed=true;s.DepartedAt=Food.Time;
        History.Add($"The boat leaves with {s.ExportedLogs} logs and {s.ExportedFood.Values.Sum()} food. Eight neighbors have a home here; another settlement now shares in your work.");return null;
    }
    private void AdvanceVillageStory()
    {
        if(VillageStory is not {} s)return;
        if(s.Complete)return;
        if(!s.Established && DeliveredEdible>=4 && s.FreshMeals.Count(id=>id<4)==4)
        {s.Established=true;StoryEvent("The first four have eaten from this land. Your livelihood has begun. Make room for four neighbors: finish eight beds and twelve food portions to welcome them early, or keep preparing until their boat arrives.");}
        if(!s.Arrived && Food.Time>=StoryArrival)ArriveStory();
        if(s.Arrived && !s.Settled && Housed==8 && s.FreshMeals.Count==8 && People.All(p=>p.Fed))
        {s.Settled=true;StoryEvent("Eight people now call this village home. Choose your contribution to the next holding: building supplies or a larger food shipment. Your earlier harvests count, and local meals stay protected while you pack.");}
        PackStoryShipment();
        if(s.Departed && Food.Time-s.DepartedAt>=StoryBoatJourney)
        {s.Complete=true;StoryEvent("Your boat is on its way. From a few provisions, you made a home for eight and helped another holding begin. This village is yours to keep tending.");}
    }
    public VillageStoryReport ReadVillageStory()
    {
        var s=VillageStory!;var manifest=StoryManifest;
        int arrival=Math.Max(0,(int)Math.Ceiling(StoryArrival-Food.Time));
        int chapter=!s.Established?1:!s.Settled?2:3;
        string phase=s.Complete?"complete":s.Departed?"departing":chapter==1?"livelihood":!s.Arrived?"prepare":!s.Settled?"settle":manifest==null?"choose-cargo":s.ReadyAnnounced?"ready":"packing";
        int packedFood=s.Departed?s.ExportedFood.Values.Sum():s.PackedFood.Values.Sum();
        int packedLogs=s.Departed?s.ExportedLogs:s.PackedLogs;
        string Mark(bool ready)=>ready?"✓":"○";
        string[] objectives;string title,hint;
        if(chapter==1)
        {
            var sources=Cottages.Where(c=>c.Kind is BuildingKind.VegetableGarden or BuildingKind.VegetableField or BuildingKind.FishingDock).ToArray();
            title="Feed the first four";
            objectives=new[]{ $"{Mark(sources.Any(c=>c.Complete))} Food source · {(sources.Any(c=>c.Complete)?"working":sources.Length>0?"under construction":"choose garden, field or fishing")}",
                $"{Mark(DeliveredEdible>=4)} Local food delivered · {Math.Min(4,DeliveredEdible)}/4",$"{Mark(s.FreshMeals.Count>=4)} First fresh meals · {s.FreshMeals.Count(id=>id<4)}/4"};
            hint=sources.Length==0?"Open Food sources and choose a livelihood. Twelve provisions will not last forever. Your grove renews automatically; add more grove area when you need logs.":
                "Neighbors build, grow and deliver automatically. The first four need fresh meals from your food source; they eat when hungry. More homes will be needed next.";
        }
        else if(chapter==2)
        {
            title=s.Arrived?"Make the newcomers at home":"Make room for four neighbors";
            objectives=new[]{ $"{Mark(Beds>=8)} Homes · {Beds}/8 beds"+(s.Arrived?$" · {Housed}/8 housed":""),
                s.Arrived?$"{Mark(s.FreshMeals.Count==8)} Newcomers' fresh meals · {s.FreshMeals.Count(id=>id>=4)}/4":$"○ Neighbors in {arrival/60}:{arrival%60:00} · or welcome early",
                s.Arrived?$"{Mark(People.All(p=>p.Fed))} Everyone fed · {People.Count(p=>p.Fed)}/8":$"{Mark(ReachableEdibleStored>=12)} Arrival food · {ReachableEdibleStored}/12"};
            hint=s.Arrived?"New hands help with work, but eight people eat more. Strengthen food supply or shorten a useful route. Once they are settled, choose the supplies your village will send upriver.":
                "Fit homes nearby, clear woodland if needed, or use the far bank. The old crossing works; a shortcut is optional. Earlier preparation lets you welcome the boat without waiting.";
        }
        else
        {
            title=s.Complete?"✓ A home here, help upriver":s.Departed?"Your supply boat is leaving":manifest==null?"Choose how your village will help":"Prepare the boat for upriver";
            objectives=manifest==null?new[]{"✓ Eight neighbors settled","○ Choose building supplies or food supplies","○ Prepare and send your contribution"}:new[]{
                $"{Mark(packedLogs>=manifest.Logs)} Boat logs · {packedLogs}/{manifest.Logs}",$"{Mark(packedFood>=manifest.Food)} Boat food · {packedFood}/{manifest.Food}",
                s.Departed?(s.Complete?"✓ Boat sent · eight neighbors settled":"→ Boat departing · supplies aboard"):$"{Mark(StorySendProblem()==null)} Village · housed {Housed}/8 · fed {People.Count(p=>p.Fed)}/8 · food kept {ReachableEdibleStored}/{StoryFoodFloor}"};
            hint=s.Complete?"Your earlier choices made this village and this contribution possible. Stay to tend it, or begin a fresh village with a different livelihood.":
                s.Departed?"Your logs and food are aboard. Watch the boat depart; the village keeps working.":
                manifest==null?"Building supplies need more logging; a food shipment needs more surplus harvests. Choose what suits your village. The boat waits, so a shortage can be recovered.":
                $"Packing uses unclaimed supplies and leaves {StoryFoodFloor} food available for local meals. Logs for new construction can be delayed. Improve production or routes; cancel cargo to return everything packed.";
        }
        if(!s.Complete && !s.Departed && People.Any(p=>!p.Fed))hint="Some neighbors are hungry. Keep food work running or add a food source to recover. "+hint;
        return new(phase,title,objectives,hint,chapter,new[]{"Find a livelihood","Welcome neighbors","Send help upriver"},s.Notice,s.Event,arrival,StoryWelcomeProblem(),s.Manifest,
            manifest?.Logs??0,manifest?.Food??0,packedLogs,packedFood,StoryFoodFloor,StoryOrderProblem(),StorySendProblem(),s.Departed,
            s.Departed?Math.Clamp((Food.Time-s.DepartedAt)/StoryBoatJourney,0,1):0,
            new[]{"Eight neighbors made their home here",$"{s.ExportedLogs} logs and {s.ExportedFood.Values.Sum()} food sent upriver",$"{Cottages.Count(c=>c.Complete && c.Kind is BuildingKind.VegetableGarden or BuildingKind.VegetableField or BuildingKind.FishingDock)} food workplaces · {ManagedWoodland.Count} renewing tree spots"},StoryShipments);
    }
    private void ValidateVillageStory()
    {
        if(VillageStory is not {} s)return;
        var manifest=StoryManifest;
        if(Riverside==null || Population!=(s.Arrived?8:4) || s.FreshMeals==null || s.FreshMeals.Any(id=>id<0 || id>=Population) ||
            s.PackedFood==null || s.ExportedFood==null || s.PackedFood.Concat(s.ExportedFood).Any(p=>p.Key==Resource.Berries || !EdibleKinds.Contains(p.Key) || p.Value<0) ||
            s.PackedLogs<0 || s.ExportedLogs<0 || s.Event<0 || !float.IsFinite(s.DepartedAt) || s.DepartedAt<0 || s.DepartedAt>Food.Time ||
            s.Settled && (!s.Established || !s.Arrived || s.FreshMeals.Count!=8) || s.Complete&&!s.Departed ||
            manifest==null && (s.Manifest!="none" || StoryLogsAway!=0 || s.PackedFood.Values.Sum()+s.ExportedFood.Values.Sum()!=0 || s.Departed) ||
            manifest!=null && (StoryLogsAway>manifest.Logs || s.PackedFood.Values.Sum()+s.ExportedFood.Values.Sum()>manifest.Food) ||
            !s.Departed && (s.ExportedLogs!=0 || s.ExportedFood.Values.Sum()!=0) ||
            s.Departed && (manifest==null || s.PackedLogs!=0 || s.PackedFood.Values.Sum()!=0 || s.ExportedLogs!=manifest.Logs || s.ExportedFood.Values.Sum()!=manifest.Food))
            throw new InvalidOperationException("Invalid village story or shipment accounting");
    }
}

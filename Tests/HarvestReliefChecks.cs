using Inlanders.Simulation;
using System.Text.Json;

static class HarvestReliefChecks
{
    public static void Run()
    {
        var rows=new List<object>();
        foreach(string plan in new[]{"baseline","gardens","fields","fishing"})
        {
            var w=World.NewHarvestRelief();
            void Advance(float seconds){for(int i=0;i<(int)(seconds*10);i++)w.Tick(.1f);}
            void SaveCheck(){var copy=World.LoadJson(w.SaveJson());for(int i=0;i<10;i++){w.Tick(.1f);copy.Tick(.1f);}if(w.SaveJson()!=copy.SaveJson())throw new Exception("Relief save diverged");w.Validate();}
            void Plan(BuildingKind kind,Cell at,int facing=0)
            {
                for(int i=0;!w.PlanRiverside(at,facing,kind);i++){if(i>40)throw new Exception($"{plan}: {at} {w.RiversideSiteProblem(at,facing,kind)}");Advance(1);}
            }
            if(w.Population!=8 || w.Beds!=8 || w.Cottages.Count!=5 || w.Cottages.Any(c=>!c.Complete) || w.StoryWelcomeProblem()==null || w.RiversideSiteProblem(World.RiversideShortcut,0,BuildingKind.Bridge)==null)throw new Exception("Relief opening/commands wrong");
            if(w.OrderStoryShipment("building")==null || w.SendStoryShipment()==null)throw new Exception("Wrong shipment accepted");
            if(plan=="gardens"){Plan(BuildingKind.VegetableGarden,new(-4,6));Plan(BuildingKind.VegetableGarden,new(-1,8));Plan(BuildingKind.VegetableGarden,new(4,4));}
            if(plan=="fields"){Plan(BuildingKind.VegetableField,new(3,-4));Plan(BuildingKind.VegetableField,new(-4,-5));}
            if(plan=="fishing"){Plan(BuildingKind.FishingDock,new(7,-6),1);Plan(BuildingKind.VegetableGarden,new(-1,8));}
            Advance(20);SaveCheck();
            if(w.OrderStoryShipment("food")!=null)throw new Exception("Relief request rejected");
            bool refunded=false,partialSaved=false,departureSaved=false;float hungry=0;int minimum=1000;
            while(w.Food.Time<1800 && !w.VillageStory!.Complete)
            {
                w.Tick(.1f);var s=w.VillageStory!;minimum=Math.Min(minimum,w.ReachableEdibleStored);if(w.People.Any(p=>!p.Fed))hungry+=.1f;
                if(!refunded && s.PackedLogs>0 && s.PackedFood.Values.Sum()>0)
                {
                    int food=w.ReachableEdibleStored+s.PackedFood.Values.Sum(),logs=w.Available+s.PackedLogs;
                    if(w.CancelStoryShipment()!=null || w.ReachableEdibleStored!=food || w.Available!=logs)throw new Exception("Relief refund wrong");
                    if(w.OrderStoryShipment("food")!=null)throw new Exception("Relief reorder failed");refunded=true;SaveCheck();partialSaved=true;
                }
                if(w.StorySendProblem()==null)
                {if(w.SendStoryShipment()!=null)throw new Exception("Ready boat rejected");SaveCheck();departureSaved=true;if(w.SendStoryShipment()==null || w.CancelStoryShipment()==null)throw new Exception("Duplicate relief accepted");}
            }
            w.Validate();SaveCheck();
            var row=new{plan,complete=w.VillageStory!.Complete,time=w.Food.Time,hungry,minimum,food=w.ReachableEdibleStored,packed=w.VillageStory.PackedFood.Values.Sum(),exported=w.VillageStory.ExportedFood.Values.Sum(),refunded,partialSaved,departureSaved};rows.Add(row);Console.WriteLine(JsonSerializer.Serialize(row));
            if(plan=="baseline" && w.VillageStory.Complete)throw new Exception("Starting dock alone completes relief; revise starting economy");
            if(plan!="baseline" && (!w.VillageStory.Complete || !refunded || !partialSaved || !departureSaved))throw new Exception($"{plan} failed full route");
        }
        Directory.CreateDirectory("artifacts/browser/relief265");File.WriteAllText("artifacts/browser/relief265/comparisons.json",JsonSerializer.Serialize(rows,new JsonSerializerOptions{WriteIndented=true}));
        Console.WriteLine("PASS established opening, inactive baseline, distinct supply plans, refunds, exact saves, dispatch and conservation");
    }
}

using Inlanders.Simulation;
using System.Text.Json;

static class VillageStoryChecks
{
    public static void Run()
    {
        var rows=new List<object>();
        foreach(string manifest in new[]{"building","food"})foreach(bool fishing in new[]{false,true})
        {
            var w=World.NewVillageStory();
            void Plan(BuildingKind kind,Cell at,int rotation=0)
            {if(!w.PlanRiverside(at,rotation,kind))throw new Exception($"Cannot plan {kind} at {at}: {w.RiversideSiteProblem(at,rotation,kind)}");}
            void ExactSave()
            {
                var copy=World.LoadJson(w.SaveJson());
                for(int i=0;i<10;i++){w.Tick(.1f);copy.Tick(.1f);}
                if(w.SaveJson()!=copy.SaveJson())throw new Exception("Story save continuation diverged");
            }
            if(w.OrderStoryShipment(manifest)==null || w.SendStoryShipment()==null || w.WelcomeStory()==null)throw new Exception("Early gated command accepted");
            Plan(fishing?BuildingKind.FishingDock:BuildingKind.VegetableGarden,fishing?new(7,4):new(-6,6),fishing?1:0);
            bool expanded=false,ordered=false,refunded=false,savedCargo=false,savedDeparture=false;int lastEvent=-1;float hungry=0,settledAt=0,lastOrder=0;
            while(w.Food.Time<2400 && !w.VillageStory!.Complete)
            {
                w.Tick(.1f);var s=w.VillageStory!;
                if(w.People.Any(p=>!p.Fed))hungry+=.1f;
                if(lastEvent!=s.Event){lastEvent=s.Event;ExactSave();w.Validate();}
                if(s.Established && !expanded)
                {
                    Plan(BuildingKind.Cottage,new(5,3));Plan(BuildingKind.Cottage,new(2,7));
                    Plan(BuildingKind.VegetableField,new(3,-5));Plan(BuildingKind.Bridge,World.RiversideShortcut);expanded=true;lastOrder=w.Food.Time;
                }
                if(w.StoryWelcomeProblem()==null && w.WelcomeStory()!=null)throw new Exception("Ready welcome failed");
                if(s.Settled && !ordered)
                {
                    settledAt=w.Food.Time;
                    if(manifest=="food")Plan(BuildingKind.VegetableField,new(-4,-4));
                    else {w.SetHarvestGrove(new(-7,4));w.SetHarvestGrove(new(-2,1));}
                    lastOrder=w.Food.Time;
                    if(w.OrderStoryShipment(manifest)!=null)throw new Exception("Cargo order rejected");ordered=true;
                    if(w.OrderStoryShipment(manifest)==null)throw new Exception("Duplicate cargo order accepted");
                }
                if(ordered && !refunded && s.PackedLogs>0 && s.PackedFood.Values.Sum()>0)
                {
                    int logs=w.Available+s.PackedLogs,food=w.ReachableEdibleStored+s.PackedFood.Values.Sum(),delivered=(w.DeliveredVegetables+w.DeliveredBerries+w.DeliveredBread);
                    if(w.CancelStoryShipment()!=null || w.Available!=logs || w.ReachableEdibleStored!=food || (w.DeliveredVegetables+w.DeliveredBerries+w.DeliveredBread)!=delivered)throw new Exception("Cargo refund changed goods or delivery accounting");
                    w.Validate();if(w.OrderStoryShipment(manifest)!=null)throw new Exception("Reordering after refund failed");refunded=true;
                }
                if(ordered && !savedCargo && s.PackedFood.Values.Sum()>0 && !s.ReadyAnnounced){ExactSave();savedCargo=true;}
                if(w.StorySendProblem()==null)
                {
                    int logs=w.Available,food=w.ReachableEdibleStored,delivered=(w.DeliveredVegetables+w.DeliveredBerries+w.DeliveredBread);
                    if(w.SendStoryShipment()!=null || w.Available!=logs || w.ReachableEdibleStored!=food || (w.DeliveredVegetables+w.DeliveredBerries+w.DeliveredBread)!=delivered)throw new Exception("Sending cargo touched village stores twice");
                    if(w.CancelStoryShipment()==null || w.SendStoryShipment()==null)throw new Exception("Departed cargo refunded or duplicated");
                    w.Validate();ExactSave();savedDeparture=true;
                }
                if((int)(w.Food.Time*10)%100==0)w.Validate();
            }
            w.Validate();
            var row=new{manifest,fishing,complete=w.VillageStory!.Complete,time=w.Food.Time,settledAt,lastOrder,hungry,logsSent=w.VillageStory.ExportedLogs,foodSent=w.VillageStory.ExportedFood.Values.Sum(),foodKept=w.ReachableEdibleStored,refunded,savedCargo,savedDeparture};
            rows.Add(row);Console.WriteLine(JsonSerializer.Serialize(row));
            if(!w.VillageStory.Complete || !refunded || !savedCargo || !savedDeparture)throw new Exception("Story route did not complete all checks");
            for(int i=0;i<300;i++)w.Tick(.1f);
            if(!w.VillageStory.Complete)throw new Exception("Completion did not latch");w.Validate();
        }
        Directory.CreateDirectory("artifacts/browser/village264");
        File.WriteAllText("artifacts/browser/village264/comparisons.json",JsonSerializer.Serialize(rows,new JsonSerializerOptions{WriteIndented=true}));
        Console.WriteLine("PASS both manifests with farming/fishing, full chapter progression, protected claims, exact saves, refunds, departure and continuation");
    }
}

using Inlanders.Simulation;
using System.Text.Json;

static class TimberShipmentChecks
{
    public static void Run()
    {
        var results=new List<object>();
        foreach(bool ripe in new[]{false,true})foreach(bool accept in new[]{false,true})
        {
            var w=World.NewTimberTrade(ripe);int stage=0;float shipped=-1,homes=-1,hungry=0;int foodBeforeShipment=-1,logsAtOffer=-1;
            void Place(BuildingKind kind,int x,int z,int rotation)
            {if(w.Place(new(x,z),rotation,kind)==null)throw new Exception($"Recorded placement rejected: {kind} {x},{z}");}
            // Reproduce the owner's dock-first spending, grove and subsequent plans.
            while(w.Food.Time<900 && !w.Founding!.FirstHome!.Complete)
            {
                if(stage==0 && w.Food.Time>=18.1f){Place(BuildingKind.FishingDock,8,4,1);stage++;}
                if(stage==1 && w.Food.Time>=24.5f){w.ApplyWoodlandCare(new(-11,5),new(5,12),WoodlandIntent.Renew,false);stage++;}
                if(stage==2 && w.Founding!.FirstHome!.Trade!.Decision=="open")
                {
                    logsAtOffer=w.AvailableLogsAt(null);
                    if(w.DecideTimberTrade(accept)!=null)throw new Exception("Spent-timber opening could not choose");
                    Place(BuildingKind.VegetableField,6,3,0);stage++;
                }
                if(stage==3 && w.Food.Time>=50.3f){Place(BuildingKind.Cottage,-6,0,0);stage++;}
                if(stage==4 && w.Food.Time>=55.7f){Place(BuildingKind.Cottage,-6,2,2);stage++;}
                if(stage==5 && w.Food.Time>=71.3f){Place(BuildingKind.Cottage,-9,2,2);stage++;}
                int before=w.ReachableEdibleStored;w.Tick(.1f);
                if(accept && shipped<0 && w.Founding!.FirstHome!.Trade!.Decision=="traded") {shipped=w.Food.Time;foodBeforeShipment=before;}
                if(homes<0 && w.Beds>=8)homes=w.Food.Time;
                if(w.People.Any(p=>!p.Fed))hungry+=.1f;
            }
            w.Validate();
            if(!w.Founding!.FirstHome!.Complete || accept && shipped<0)throw new Exception("Recorded route did not finish shipment/village");
            if(accept && w.CancelTimberTrade()==null)throw new Exception("Completed shipment refunded");
            var result=new{ripe,accept,logsAtOffer,shipped,foodBeforeShipment,homes,hungry,complete=w.Food.Time,food=w.ReachableEdibleStored};
            results.Add(result);Console.WriteLine(JsonSerializer.Serialize(result));
        }
        var empty=World.NewTimberTrade();
        if(empty.Place(new(-6,0),0,BuildingKind.Cottage)==null || empty.Place(new(-6,2),2,BuildingKind.Cottage)==null)throw new Exception("Empty-store setup");
        while(empty.Food.Time<World.TradeOfferArrival)empty.Tick(.1f);
        if(empty.AvailableLogsAt(null)!=0 || empty.DecideTimberTrade(true)!=null || empty.Founding!.FirstHome!.Trade!.Decision!="ordered")throw new Exception("Zero-log order rejected");
        while(empty.Food.Time<World.TradeDeparture+10)empty.Tick(.1f);
        if(empty.Founding.FirstHome.Trade.Decision!="ordered" || empty.Founding.FirstHome.Trade.ExportedLogs!=0 || empty.Food.InitialBerries!=4)throw new Exception("Unpaid order expired or created food");
        if(empty.CancelTimberTrade()!=null)throw new Exception("Zero-log cancellation failed");
        empty.Validate();
        Directory.CreateDirectory("artifacts/browser/shipment262");
        File.WriteAllText("artifacts/browser/shipment262/owner-routes.json",JsonSerializer.Serialize(results,new JsonSerializerOptions{WriteIndented=true}));
        Console.WriteLine("PASS recorded dock-first routes, zero-log agreement, no unpaid food, no fulfillment deadline, cancellation and conservation");
    }
}

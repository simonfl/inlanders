using Inlanders.Simulation;
using System.Text.Json;

static class TimberTradeChecks
{
    public static void Run()
    {
        var results = new List<object>();
        var opening = World.NewTimberTrade();
        if(opening.Founding!.FirstHome!.Trade!.Decision!="waiting" || opening.DecideTimberTrade(true)==null || opening.DecideTimberTrade(false)==null)
            throw new Exception("Trade interrupts opening or accepts an early decision");
        while(opening.Food.Time<30)opening.Tick(.1f);
        var openingClone=World.LoadJson(opening.SaveJson());
        while(opening.Food.Time<World.TradeOfferArrival-1){opening.Tick(.1f);openingClone.Tick(.1f);}
        if(opening.Founding.FirstHome.Trade.Decision!="waiting")throw new Exception("Trader arrived too early");
        while(opening.Food.Time<World.TradeOfferArrival){opening.Tick(.1f);openingClone.Tick(.1f);}
        if(opening.Founding.FirstHome.Trade.Decision!="open" || opening.SaveJson()!=openingClone.SaveJson())throw new Exception("Arrival or opening save continuation failed");
        foreach (bool ripe in new[] { false, true }) foreach (bool buy in new[] { false, true })
        {
            var w = World.NewTimberTrade(ripe);
            while(w.Food.Time<World.TradeOfferArrival)w.Tick(.1f);
            int foodBefore=w.ReachableEdibleStored;
            if (w.DecideTimberTrade(buy) != null || w.DecideTimberTrade(buy) == null) throw new Exception("Exchange is not one-shot");
            if (buy && (w.Stored != 0 || w.ReachableEdibleStored != foodBefore+World.TradeFood)) throw new Exception("Trade accounting");
            void Place(BuildingKind kind, Cell target)
            { var p = ReviewPlacement.Find(w, kind, target, (c,r) => c.Z > 0, "trade check") ?? throw new Exception("No legal site"); w.Place(p.Actual,p.Rotation,kind); }
            Place(BuildingKind.Cottage,new(1,6)); Place(BuildingKind.Cottage,new(5,6));
            float hungrySeconds = 0, homesAt = -1; int low = 100; bool grove = false, field = false, saved = false;
            for (int i=0; i<12000 && !w.Founding!.FirstHome!.Complete; i++)
            {
                if (!grove && w.Food.Time>=30) { grove=true; w.ApplyWoodlandCare(new(-11,5),new(6,12),WoodlandIntent.Renew,false); }
                if (!field && w.Food.Time>=120) { field=true; Place(BuildingKind.VegetableField,new(-6,5)); }
                w.Tick(.1f); low=Math.Min(low,w.ReachableEdibleStored);
                if(w.People.Any(p=>!p.Fed))hungrySeconds+=.1f;
                if(homesAt<0 && w.Beds>=8)homesAt=w.Food.Time;
                if(!saved && w.Food.Time>=150)
                {
                    saved=true; var clone=World.LoadJson(w.SaveJson());
                    for(int n=0;n<20;n++){w.Tick(.1f);clone.Tick(.1f);}
                    if(w.SaveJson()!=clone.SaveJson())throw new Exception("Trade save diverged");
                }
            }
            w.Validate();
            if (!w.Founding!.FirstHome!.Complete || w.Population!=8) throw new Exception("Trade route cannot settle: "+w.ReadFirstHomeObjectives().Hint);
            var result=new { ripe,buy,seconds=w.Food.Time,hungrySeconds,homesAt,minimumFood=low,food=w.ReachableEdibleStored,logs=w.Stored };
            results.Add(result);Console.WriteLine(JsonSerializer.Serialize(result));
        }
        var missed=World.NewTimberTrade();while(missed.Food.Time<World.TradeDeparture+1)missed.Tick(.1f);
        if(missed.DecideTimberTrade(true)==null || missed.Stored!=12)throw new Exception("Expired trade accepted");
        var control=World.NewTimberTrade(false,false);if(control.DecideTimberTrade(true)==null)throw new Exception("Control has trade");
        var reserved=World.NewTimberTrade();while(reserved.Food.Time<World.TradeOfferArrival)reserved.Tick(.1f);
        var site=ReviewPlacement.Find(reserved,BuildingKind.Cottage,new(1,6),(c,r)=>c.Z>0,"reservation")!;
        reserved.Place(site.Actual,site.Rotation,BuildingKind.Cottage);for(int i=0;i<20;i++)reserved.Tick(.1f);
        if(reserved.TimberTradeProblem()==null)throw new Exception("Trade steals committed construction logs");reserved.Validate();
        var neglected=World.NewTimberTrade();for(int i=0;i<3000;i++)neglected.Tick(.1f);
        if(neglected.Founding!.FirstHome!.Complete)throw new Exception("Neglect wins");
        Directory.CreateDirectory("artifacts/browser/trade260");File.WriteAllText("artifacts/browser/trade260/routes.json",JsonSerializer.Serialize(results,new JsonSerializerOptions{WriteIndented=true}));
        Console.WriteLine("PASS quiet opening, timed arrival, pre-arrival save/rejection, both trade decisions and crop starts, conservation, local-food accounting, exact save, missed offer, reservation safety and neglect");
    }
}

using Inlanders.Simulation;
static class TimberReserveChecks
{
    public static void Run()
    {
        var w=World.NewRiverFrontage(false,true,true);
        if(w.TimberStockTarget!=32)throw new Exception("Opening reserve should cover a small expansion");
        if(!w.ApplyWoodlandCare(new(w.Map.MinX,w.Map.MinZ),new(w.Map.MaxX,w.Map.MaxZ),WoodlandIntent.Renew,false))throw new Exception("Grove permission failed");
        for(int i=0;i<6000 && w.Stored<32;i++)w.Tick(.1f);
        if(w.Stored<32)throw new Exception("Automatic work did not stock timber ahead of construction");
        // Let committed trips settle, then ensure standing permission is not endless felling.
        for(int i=0;i<600;i++)w.Tick(.1f);
        int felled=w.Trees.Count(t=>t.Felled);int stock=w.Stored;
        for(int i=0;i<300;i++)w.Tick(.1f);
        if(w.Stored!=stock || w.Trees.Count(t=>t.Felled)!=felled)throw new Exception("Satisfied reserve kept cutting");
        var placement=ReviewPlacement.Find(w,BuildingKind.Cottage,new(0,4),(_,_)=>true,"reserve")??throw new Exception("No cottage site");
        var home=w.Place(placement.Actual,placement.Rotation,BuildingKind.Cottage)!;
        if(w.TimberStockTarget!=32+home.Required)throw new Exception("Construction displaced standing reserve");
        w.SetConstructionPaused(home.Id,true);if(w.TimberStockTarget!=32)throw new Exception("Paused order still requests timber");w.SetConstructionPaused(home.Id,false);
        var copy=World.LoadJson(w.SaveJson());for(int i=0;i<50;i++){w.Tick(.1f);copy.Tick(.1f);}
        if(w.SaveJson()!=copy.SaveJson())throw new Exception("Reserve dispatch diverged on reload");
        for(int i=0;i<6000 && (!home.Complete || w.Stored<32);i++)w.Tick(.1f);
        if(!home.Complete || w.Stored<32 || w.TimberStockTarget!=32)throw new Exception("Construction did not finish with replenished reserve");
        w.Validate();Console.WriteLine($"PASS proactive reserve, stop, queued/paused demand, actual cottage construction and replenishment to {w.Stored} logs; exact save continuation.");
    }
}

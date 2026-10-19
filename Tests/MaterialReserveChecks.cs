using Inlanders.Simulation;
static class MaterialReserveChecks
{
    public static void Run()
    {
        var w=World.NewRiverFrontage(false,true,true);
        w.ApplyWoodlandCare(new(w.Map.MinX,w.Map.MinZ),new(w.Map.MaxX,w.Map.MaxZ),WoodlandIntent.Renew,false);
        Cottage Place(BuildingKind kind){var p=ReviewPlacement.Find(w,kind,new(0,4),(_,_)=>true,"reserve")??throw new Exception("No site for "+kind);return w.Place(p.Actual,p.Rotation,kind)!;}
        var mill=Place(BuildingKind.Sawmill);
        if(w.MaterialStockTarget(Resource.Planks)!=24)throw new Exception("Log building requested planks");
        for(int i=0;i<6000 && w.Planks<24;i++)w.Tick(.1f);
        if(w.Planks<24)throw new Exception("Did not pre-stock planks: "+w.Planks);
        w.SetOutputTarget(mill.Id,0);var lodge=Place(BuildingKind.Lodge);
        if(w.MaterialStockTarget(Resource.Planks)!=36 || w.EffectiveOutputTarget(mill)!=0)throw new Exception("Queue/manual cap mismatch");
        w.UseAutomaticOutputTarget(mill.Id);
        if(w.EffectiveOutputTarget(mill)!=36)throw new Exception("Auto restore failed");
        Console.WriteLine("Initial reserve stocked; building lodge.");
        var copy=World.LoadJson(w.SaveJson());for(int i=0;i<50;i++){w.Tick(.1f);copy.Tick(.1f);}
        if(w.SaveJson()!=copy.SaveJson())throw new Exception("Saved target diverged");
        for(int i=0;i<6000 && (!lodge.Complete || w.Planks<24);i++)w.Tick(.1f);
        if(!lodge.Complete || w.Planks<24)throw new Exception("Lodge/reserve did not complete");
        var hall=Place(BuildingKind.GatheringHall);
        if(w.MaterialStockTarget(Resource.Stone)!=36)throw new Exception("Hall stone demand missing");
        w.SetConstructionPaused(hall.Id,true);
        if(w.MaterialStockTarget(Resource.Stone)!=24)throw new Exception("Paused demand remained");
        w.Validate();Console.WriteLine("PASS actual plank stocking, lodge construction/replenishment, manual/automatic caps, stone demand, exact save continuation.");
    }
}

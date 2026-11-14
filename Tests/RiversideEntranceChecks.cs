using Inlanders.Simulation;
using System.Text.Json;

static class RiversideEntranceChecks
{
    public static void Run()
    {
        // Recreate the evaluator's ordinary orders at the moment they noticed
        // the bad journey. No ignored recording file is required to rerun this.
        var w=World.NewRiverside();
        w.PlanRiverside(new(5,3),0,BuildingKind.Cottage);w.PlanRiverside(new(2,7),0,BuildingKind.Cottage);
        while(w.Food.Time<58.5f)w.Tick(.1f);
        if(!w.PlanRiverside(new(-1,5),0,BuildingKind.VegetableGarden))throw new Exception("Evaluator garden rejected");
        while(w.Food.Time<69.3f)w.Tick(.1f);
        if(!w.WelcomeRiverside())throw new Exception("Evaluator arrival rejected");
        while(w.Food.Time<248.6f)w.Tick(.1f);
        var garden=w.Cottages.Single(c=>c.Kind==BuildingKind.VegetableGarden && c.Cell==new Cell(-1,5));
        var cells=World.Footprint(garden).ToHashSet();
        var before=w.SaveJson();var preview=w.ReadGardenEntrance(garden.Id)!;
        if(w.SaveJson()!=before)throw new Exception("Entrance preview mutated village");
        if(preview.Problem!=null || preview.After>=preview.Before)throw new Exception("No useful legal entrance correction: "+JsonSerializer.Serialize(preview));
        var crop=(garden.Planted,garden.Growth,garden.Harvest,garden.WorkPaused);
        if(!w.ChangeGardenEntrance(garden.Id) || crop!=(garden.Planted,garden.Growth,garden.Harvest,garden.WorkPaused) || !cells.SetEquals(World.Footprint(garden)))throw new Exception("Entrance change moved or reset crops");
        w.Validate();var copy=World.LoadJson(w.SaveJson());
        for(int i=0;i<800;i++){w.Tick(.1f);copy.Tick(.1f);}
        if(w.SaveJson()!=copy.SaveJson())throw new Exception("Changed entrance save diverged");
        Console.WriteLine($"PASS evaluator layout: actual pantry journey {preview.Before} -> {preview.After} steps; crop/ground retained, exact continuation");
        // Check crop phases and active carrying with fresh ordinary orders.
        foreach(float time in new[]{20f,70f,130f,220f})
        {
            var live=World.NewRiverside();while(live.Food.Time<time)live.Tick(.1f);
            var g=live.Cottages.Single(c=>c.Kind==BuildingKind.VegetableGarden);var original=World.Footprint(g).ToHashSet();
            var p=live.ReadGardenEntrance(g.Id)!;var state=live.SaveJson();
            if(p.Problem!=null){if(live.ChangeGardenEntrance(g.Id) || live.SaveJson()!=state)throw new Exception("Rejected entrance mutated state");continue;}
            var content=(g.Planted,g.Growth,g.Harvest,g.WorkPaused);if(!live.ChangeGardenEntrance(g.Id) || content!=(g.Planted,g.Growth,g.Harvest,g.WorkPaused))throw new Exception("Crop phase lost");
            for(int i=0;i<600;i++){live.Tick(.1f);live.Validate();}
            if(!original.SetEquals(World.Footprint(g)))throw new Exception("Footprint changed");
        }
        Console.WriteLine("PASS growing/ripe/live-work entrance conservation and rejected-change no mutation");
    }
}

using Inlanders.Simulation;
static class GrainRelocationChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    public static void Run()
    {
        Directory.CreateDirectory("artifacts/grain-relocation");
        foreach(bool relaxed in new[]{false,true})foreach(bool ripe in new[]{false,true})
        {
            var w=new HamletProfile(relaxed,true,false,true).Create();
            var field=w.Place(new(-3,-5),0,BuildingKind.Farm)!;
            // Edible harvests take priority during shortages; allow the later grain
            // crop to mature before testing relocation, not scheduler timing.
            for(int i=0;i<6000 && !(field.Complete && (ripe?field.Harvest>0:field.Planted && field.Growth>0 && field.Harvest==0));i++)w.Tick(.1f);
            Check(field.Complete && (ripe?field.Harvest>0:field.Planted && field.Growth>0),$"Required real crop phase absent relaxed={relaxed}, ripe={ripe}: complete={field.Complete}, planted={field.Planted}, harvest={field.Harvest}, grain={w.StoredGrain}, target={w.EffectiveOutputTarget(field)}, report={w.ReadWorkplace(field)}");
            if(ripe){string unchanged=w.SaveJson();Check(w.RelocationIntentProblem(field.Id)?.Contains("Ripe crops")==true && !w.MoveBuilding(field.Id,new(0,-9),1) && w.SaveJson()==unchanged,"Ripe land-bound crop moved or rejection mutated state");w.Validate();continue;}
            w.SaveFile($"artifacts/grain-relocation/{relaxed}-{ripe}-before.json");
            Check(w.RelocationProblem(field.Id)!=null && w.RelocationIntentProblem(field.Id)==null,"Intent unavailable or unpaused grain field directly movable");
            Check(w.SetWorkplacePaused(field.Id,true),"Pause failed");
            var target=w.Map.Land.OrderBy(c=>c.Z).ThenBy(c=>c.X).First(c=>c!=field.Cell && w.RelocationProblem(field.Id,c,1)==null);
            int grain=w.StoredGrain,harvest=field.Harvest;float growth=field.Growth;string saved=w.SaveJson();
            Check(w.RelocationProblem(field.Id,target,1)==null && saved==w.SaveJson(),"Query changed active crop/routes");
            Check(!w.MoveBuilding(field.Id,w.Stockpile,0) && saved==w.SaveJson(),"Invalid move changed crop");
            Check(w.MoveBuilding(field.Id,field.Cell,field.Rotation) && saved==w.SaveJson(),"No-op changed state");
            Check(w.MoveBuilding(field.Id,target,1) && field.WorkPaused,"Valid move failed/resumed");
            Check(w.StoredGrain==grain && field.Harvest==harvest,"Grain/ripe crop lost");
            Check(ripe?field.Growth==growth:!field.Planted && field.Growth==0,"Growing crop consequence incorrect");
            w.Validate();w.SetWorkplacePaused(field.Id,false);int grown=w.Food.GrownGrain;var copy=World.LoadJson(w.SaveJson());
            for(int i=0;i<2400;i++){w.Tick(.1f);copy.Tick(.1f);if(i%100==0)w.Validate();}
            Check(w.Food.GrownGrain>grown && w.SaveJson()==copy.SaveJson(),"Moved grain stopped producing or diverged");
        }
        Console.WriteLine("PASS: growing grain relocation and ripe-move rejection in both modes, crop cost, goods preservation, rejected/no-op purity and exact resumed production.");
    }
}

using Inlanders.Simulation;
static class GroundPreparationChecks
{
    public static void Run()
    {
        foreach(bool relaxed in new[]{false,true})foreach(var kind in new[]{BuildingKind.Cottage,BuildingKind.VegetableField,BuildingKind.SeatingGarden})
        {
            var w=World.NewRiverFrontage(relaxed,true,true);int rows=kind==BuildingKind.VegetableField?2:0;
            var choices=(from c in w.Map.Land from r in Enumerable.Range(0,4) let p=w.PreviewGroundPreparation(c,r,kind,rows) where p!=null select(c,r,p)).ToArray();
            if(choices.Length==0)throw new Exception("No preparable "+kind);var choice=choices[0];string before=w.SaveJson();
            if(w.PreviewGroundPreparation(choice.c,choice.r,kind,rows)==null || w.SaveJson()!=before)throw new Exception("Impure preparation");
            if(!w.PrepareBuildingGround(choice.c,choice.r,kind,rows))throw new Exception("Rejected preparation");
            var saved=World.LoadJson(w.SaveJson());for(int i=0;i<100;i++){w.Tick(.1f);saved.Tick(.1f);}if(w.SaveJson()!=saved.SaveJson())throw new Exception("Clearing save divergence");
            Cottage? site=null;
            for(int i=0;i<6000 && site==null;i++){w.Tick(.1f);site=w.Place(choice.c,choice.r,kind,rows);}
            if(site==null || choice.p!.Trees.Any(c=>w.Trees.Any(t=>t.Cell==c)))throw new Exception("Ground never became usable");
            for(int i=0;i<3000 && !site.Complete;i++)w.Tick(.1f);
            if(!site.Complete)throw new Exception("Prepared building never built");w.Validate();
            Console.WriteLine($"PASS prepare {kind} relaxed={relaxed}: pure proposal, physical clearance, exact saved clearing and completed building.");
        }
        var invalid=World.NewRiverLivelihood();string untouched=invalid.SaveJson();
        if(invalid.PrepareBuildingGround(invalid.Cottages[0].Cell,0,BuildingKind.Cottage) || invalid.SaveJson()!=untouched)throw new Exception("Unsafe clearance on occupied ground");
    }
}

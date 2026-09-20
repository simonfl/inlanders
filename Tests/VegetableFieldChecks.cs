using Inlanders.Simulation;
static class VegetableFieldChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    static void Until(World w,Func<bool> done,string why,int limit=20000){for(int i=0;i<limit && !done();i++){w.Tick(.1f);w.Validate();}Check(done(),why);}
    public static void Run()
    {
        Directory.CreateDirectory("artifacts/vegetable-field");
        foreach(int rotation in new[]{0,1,2,3})
        {
            var w=World.NewTransformationHamlet();
            var at=w.Map.Land.First(c=>w.PlacementProblem(c,rotation,BuildingKind.VegetableField)==null);
            var field=w.Place(at,rotation,BuildingKind.VegetableField)!;
            Check(World.Footprint(at,rotation,field.Kind).Count()==15,"Field does not occupy its land");
            Until(w,()=>field.Harvest==20,"Large crop did not mature");
            Check(w.ProductionCommitted(Resource.Vegetables)>=20,"Field crop omitted from committed stock");
            if(rotation==0)w.SaveFile("artifacts/vegetable-field/ripe.json");
            Until(w,()=>w.People.Any(p=>p.WorkplaceId==field.Id && p.Task==Work.Harvesting),"No field harvest work");
            var twin=World.LoadJson(w.SaveJson());for(int i=0;i<120;i++){w.Tick(.1f);twin.Tick(.1f);w.Validate();twin.Validate();}Check(w.SaveJson()==twin.SaveJson(),"Active harvest diverged on reload");
            Check(w.SetWorkplacePaused(field.Id,true),"Field pause rejected");
            var other=w.Map.Land.First(c=>c!=field.Cell && w.RelocationProblem(field.Id,c,rotation)==null);
            Check(w.MoveBuilding(field.Id,other,rotation),"Field relocation rejected");w.Validate();
            Check(w.SetWorkplacePaused(field.Id,false),"Field resume rejected");
            Until(w,()=>field.Harvest==0,"Field harvest never collected");
            Check(w.RequestDemolition(field.Id),"Field demolition rejected");Until(w,()=>!w.Cottages.Contains(field),"Field demolition stalled");
        }
        Console.WriteLine("PASS: 15-tile vegetable fields in four orientations; real20 crop, work, committed stock, conservation, exact harvest saves, relocation and demolition.");
    }
}

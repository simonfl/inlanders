using Inlanders.Simulation;
static class CultivationChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    public static void Run()
    {
        Directory.CreateDirectory("artifacts/cultivation");
        foreach(bool relaxed in new[]{false,true})foreach(int rows in new[]{1,3,8})
        {
            var w=World.NewPlayerFounded(relaxed);var at=w.Map.Land.First(c=>w.PlacementProblem(c,0,BuildingKind.VegetableField,rows)==null);
            var f=w.Place(at,0,BuildingKind.VegetableField,rows)!;
            Check(World.Footprint(f).Count()==rows*3 && f.Required==rows*2,"Ground/cost not proportional");
            for(int i=0;i<15000 && f.Harvest==0;i++){w.Tick(.1f);w.Validate();}
            Check(f.Harvest==rows*4,"Actual crop differs from chosen ground");
            var copy=World.LoadJson(w.SaveJson());for(int i=0;i<100;i++){w.Tick(.1f);copy.Tick(.1f);w.Validate();copy.Validate();}Check(w.SaveJson()==copy.SaveJson(),"Plot continuation diverged");
            Check(w.SetWorkplacePaused(f.Id,true),"Pause failed");
            for(int turn=0;turn<4;turn++)
            {
                var dest=w.Map.Land.First(c=>w.RelocationProblem(f.Id,c,turn)==null);string saved=w.SaveJson();Check(w.RelocationProblem(f.Id,dest,turn)==null && w.SaveJson()==saved,"Move query impure");Check(w.MoveBuilding(f.Id,dest,turn),"Sized field move failed");w.Validate();Check(World.Footprint(f).Count()==rows*3,"Rotation changed cultivated ground");
            }
            if(rows==3)w.SaveFile($"artifacts/cultivation/{relaxed}.json");
            Check(w.Place(new(0,0),0,BuildingKind.Cottage,2)==null,"Custom rows accepted on house");
            Console.WriteLine($"PASS cultivation {relaxed}/{rows}: actual {rows*4} crop, {rows*2} logs, rotations, access and exact saves.");
        }
    }
}

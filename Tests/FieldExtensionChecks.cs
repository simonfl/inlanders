using Inlanders.Simulation;
static class FieldExtensionChecks
{
    public static void Run()
    {
        foreach(bool relaxed in new[]{false,true})
        {
            void Check(bool ok,string why){if(!ok)throw new Exception(why);}
            var w=World.NewPlayerFounded(relaxed);var f=w.Place(new(1,-3),0,BuildingKind.VegetableField,2)!;
            for(int i=0;i<10000 && !f.Complete;i++)w.Tick(.1f);
            string before=w.SaveJson();
            Check(w.ExtendCultivationProblem(f.Id,4)==null && before==w.SaveJson(),"Extension proposal mutated world");
            int food=f.PantryFood.Sum(),logs=f.Delivered;
            Check(w.ExtendCultivation(f.Id,4) && f.PantryFood.Sum()==food && f.Required==8,"Extension lost stock or cost");
            if(!relaxed)Check(!f.Complete && f.Delivered==logs,"Expansion skipped timber/work");
            w.SetWorkplacePaused(f.Id,false);
            for(int i=0;i<8000 && (w.Food.EatenVegetables==0 || w.Food.GrownVegetables<16);i++){w.Tick(.1f);if(i%100==0)w.Validate();}
            Check(f.Complete && w.Food.GrownVegetables>=16 && w.Food.EatenVegetables>0,$"Extended field did not feed residents: complete={f.Complete} delivered={f.Delivered}/{f.Required} built={f.Construction} grown={w.Food.GrownVegetables} eaten={w.Food.EatenVegetables}");
            var copy=World.LoadJson(w.SaveJson());for(int i=0;i<100;i++){w.Tick(.1f);copy.Tick(.1f);}Check(w.SaveJson()==copy.SaveJson(),"Extension save differs");
            before=w.SaveJson();Check(!w.ExtendCultivation(f.Id,9) && w.SaveJson()==before,"Invalid extension mutated world");
            Console.WriteLine($"PASS field extension {relaxed}: real timber/preparation/crop/meals and exact continuation");
        }
    }
}

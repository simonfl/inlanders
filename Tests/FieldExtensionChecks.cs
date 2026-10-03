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
            if(!relaxed)
            {
                for(int i=0;i<3000 && (f.Harvest>0 || f.PantryFood.Sum()==0 || w.ExtendCultivationProblem(f.Id,6)!=null);i++)w.Tick(.1f);
                Check(f.PantryFood.Sum()>0 && w.ExtendCultivation(f.Id,6),"No food-bearing extension fixture");
                for(int i=0;i<1000 && f.Delivered==8;i++)w.Tick(.1f);
                Check(!f.Complete && f.Delivered>8,"No partial delivered extension fixture");
                var cancelled=World.LoadJson(w.SaveJson());int foodBefore=cancelled.Cottages.Single(c=>c.Id==f.Id).PantryFood.Sum();
                Check(foodBefore>0 && cancelled.Cancel(f.Id),"Cannot cancel saved extension");var restored=cancelled.Cottages.Single(c=>c.Id==f.Id);
                Check(restored.Complete && restored.Depth==4 && restored.Required==8 && restored.PantryFood.Sum()==foodBefore,"Cancellation lost original field/food");cancelled.Validate();
                for(int i=0;i<1000;i++)cancelled.Tick(.1f);cancelled.Validate();
                var exact=World.LoadJson(cancelled.SaveJson());for(int i=0;i<100;i++){cancelled.Tick(.1f);exact.Tick(.1f);}Check(cancelled.SaveJson()==exact.SaveJson(),"Cancelled extension continuation differs");
                Console.WriteLine("PASS saved partial extension cancellation preserves original field/food and salvages only extra timber");
            }
            before=w.SaveJson();Check(!w.ExtendCultivation(f.Id,9) && w.SaveJson()==before,"Invalid extension mutated world");
            Console.WriteLine($"PASS field extension {relaxed}: real timber/preparation/crop/meals and exact continuation");
        }
    }
}

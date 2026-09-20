using Inlanders.Simulation;
static class HamletEndingChecks
{
    public static void Run()
    {
        foreach(bool relaxed in new[]{false,true})
        {
            var w=new HamletProfile(relaxed).Create();
            if(w.FoundingHasNewFood || w.Commons!=null || !w.FinishFounding())throw new Exception("Personal ending acquired a prescribed goal");
            if(w.FinishFounding())throw new Exception("Repeated finish mutated state");
            var copy=World.LoadJson(w.SaveJson());
            for(int i=0;i<600;i++){w.Tick(.1f);copy.Tick(.1f);}
            if(w.SaveJson()!=copy.SaveJson() || !copy.Founding!.Finished || copy.Food.Time<59)throw new Exception("Finished village cannot continue daily life/reload");
            copy.Founding.Finished=false;copy.Validate();
            if(!copy.FinishFounding())throw new Exception("Reopened village cannot finish again");
        }
        Console.WriteLine("PASS: personal ending without building/food gates; daily life, exact reload and reopening in both modes.");
    }
}


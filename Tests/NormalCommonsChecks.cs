using Inlanders.Simulation;
static class NormalCommonsChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    public static void Run()
    {
        Directory.CreateDirectory("artifacts/normal-commons");
        var w=World.NewWorkingVillage();
        Check(w.SetCommons(new(3,4)),"Normal shared place refused");
        bool use=false;
        for(int i=0;i<5000;i++)
        {
            w.Tick(.1f);if(i%100==0)w.Validate();
            if(!use && w.People.Any(p=>p.Task==Work.EatingMeal && p.Meal?.Commons==true))
            {use=true;w.SaveFile("artifacts/normal-commons/used.json");var copy=World.LoadJson(w.SaveJson());for(int k=0;k<10;k++){w.Tick(.1f);copy.Tick(.1f);}Check(copy.SaveJson()==w.SaveJson(),"Active commons continuation differs");}
        }
        Check(use && w.Commons?.FirstDiner!=null,"Shared place never used");
        Check(w.RemoveCommons(),"Removal refused");w.Validate();Check(w.People.All(p=>p.Meal?.Commons!=true),"Removal leaves claims");
        var far=w.Map.Land.Where(c=>w.CommonsProblem(c)==null && !w.CommonsFoodNearby(c)).Last();
        Check(w.SetCommons(far),"Distant optional place refused");
        for(int i=0;i<100;i++)w.Tick(.1f);w.Validate();
        Check(w.SetCommons(new(3,4)),"Rearrangement refused");w.Validate();
        Console.WriteLine("PASS: normal shared-place use, active saves, removal claims, distant choice and rearrangement.");
    }
}

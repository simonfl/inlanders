using Inlanders.Simulation;
static class SharedPlaceScaleChecks
{
    public static void Run()
    {
        foreach(bool relaxed in new[]{false,true})foreach(int seats in new[]{2,4,6})
        {
            void Check(bool ok,string why){if(!ok)throw new Exception(why);}
            var w=World.NewRiverFrontage(relaxed,true);var center=w.Commons!.Center;
            string before=w.SaveJson();Check(!w.SetCommons(center,3) && w.SaveJson()==before,"Invalid capacity mutated shared place");
            Check(w.SetCommons(center,seats) && w.Commons!.Places.Length==seats,"Wrong shared-place extent");
            int peak=0;bool resumed=false;
            for(int i=0;i<1200;i++)
            {
                w.Tick(.1f);if(i%100==0)w.Validate();
                int eating=w.People.Count(p=>p.Task==Work.EatingMeal && p.Meal?.Commons==true);peak=Math.Max(peak,eating);
                Check(w.People.Count(p=>p.Meal is {Commons:true,Reserved:true} or {Commons:true,Carrying:true})<=seats,"Shared place overbooked");
                if(!resumed && eating>0){var copy=World.LoadJson(w.SaveJson());for(int j=0;j<20;j++){w.Tick(.1f);copy.Tick(.1f);}Check(copy.SaveJson()==w.SaveJson(),"Active shared-place continuation");resumed=true;}
            }
            Check(resumed && peak>0 && peak<=seats && w.Commons!.FirstDiner!=null,"No actual dining at chosen scale");
            Check(w.RemoveCommons(),"Scaled shared place removal");for(int i=0;i<100;i++)w.Tick(.1f);w.Validate();
            Console.WriteLine($"PASS shared scale {relaxed}/{seats}: actual meals, peak {peak}, bounded claims, active continuation and removal");
        }
    }
}

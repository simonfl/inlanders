using Inlanders.Simulation;
using System.Text.Json;
static class ProvisionedLifeChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    public static void Run()
    {
        Directory.CreateDirectory("artifacts/provisioned-life");var rows=new List<object>();double oldQuiet=0,newQuiet=0;
        foreach(bool provisioned in new[]{false,true})
        {
            var w=World.NewWorkingVillage();w.Founding!.ProvisionedLife=provisioned;
            Check(w.Place(new(7,8),1,BuildingKind.FishingDock)!=null,"Dock rejected");
            double quiet=0,hungry=0,working=0;int projectId=0;float? completed=null;
            for(int i=0;i<18000;i++)
            {
                if(i==9000){var p=ReviewPlacement.Find(w,BuildingKind.Square,new(4,4),(c,r)=>true,"new village project")!;projectId=w.Place(p.Actual,p.Rotation,BuildingKind.Square)!.Id;}
                w.Tick(.1f);
                if(i<9000){quiet+=w.People.Count(p=>p.Task==Work.Waiting)*.1;working+=w.People.Count(p=>p.Role!=Role.Unassigned)*.1;}
                hungry+=w.People.Count(p=>!p.Fed)*.1;
                if(projectId>0 && completed==null && w.Cottages.Single(c=>c.Id==projectId).Complete)completed=w.Food.Time;
                if(i%300==0)w.Validate();
                if(i==7500){var copy=World.LoadJson(w.SaveJson());for(int k=0;k<10;k++){w.Tick(.1f);copy.Tick(.1f);}Check(w.SaveJson()==copy.SaveJson(),"Quiet save continuation diverged");}
                if(i==8999)w.SaveFile($"artifacts/provisioned-life/{provisioned}-quiet.json");
            }
            Check(hungry==0 && completed<960,"Provisioning blocks meals or new projects");
            if(provisioned)newQuiet=quiet;else oldQuiet=quiet;
            rows.Add(new{provisioned,quiet,working,hungry,completed,food=w.EdibleStored});
            w.SaveFile($"artifacts/provisioned-life/{provisioned}-final.json");
        }
        Check(newQuiet>oldQuiet+200,"No meaningful released resident time");
        string json=JsonSerializer.Serialize(rows,new JsonSerializerOptions{WriteIndented=true});Console.WriteLine(json);File.WriteAllText("artifacts/provisioned-life/report.json",json);
        Console.WriteLine("PASS: matched provisioning/time, no hunger, responsive new work and exact current continuation.");
    }
}

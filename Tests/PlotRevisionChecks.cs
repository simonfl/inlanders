using Inlanders.Simulation;
static class PlotRevisionChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    public static void Run()
    {
        Directory.CreateDirectory("artifacts/plot-revision");
        foreach(bool relaxed in new[]{false,true})
        {
            var w=World.NewPlayerFounded(relaxed);var f=w.Place(new(1,-5),0,BuildingKind.VegetableField,5)!;
            for(int i=0;i<12000 && !(w.Food.GrownVegetables>0 && f.Harvest==0 && !f.Planted && !w.People.Any(p=>p.WorkplaceId==f.Id));i++){w.Tick(.1f);if(i%10==0)w.Validate();}
            Check(w.Food.GrownVegetables>0 && f.Harvest==0,"Crop did not finish");w.SetWorkplacePaused(f.Id,true);
            w.SetWorkplacePaused(f.Id,false);string running=w.SaveJson();Check(w.ReshapePlotProblem(f.Id,3,false)==null && w.SaveJson()==running,"Running preview mutated work");
            Check(w.ReviseCultivation(f.Id,3) && !f.WorkPaused,"Revision left automatic work paused");
            running=w.SaveJson();Check(!w.ReviseCultivation(f.Id,9) && w.SaveJson()==running,"Invalid automatic revision changed state");
            Check(w.ReviseCultivation(f.Id,5) && !f.WorkPaused,"Restore failed");w.SetWorkplacePaused(f.Id,true);
            Check(w.ReviseCultivation(f.Id,4) && f.WorkPaused && w.ReviseCultivation(f.Id,5) && f.WorkPaused,"Explicit pause not preserved");
            int timber=f.Delivered,food=f.PantryFood.Sum();string before=w.SaveJson();Check(w.ReshapePlotProblem(f.Id,2)==null && before==w.SaveJson(),"Preview mutates/rejects");
            Check(w.ReshapePlot(f.Id,2),"Release ground failed");w.Validate();Check(f.Required==10 && f.Delivered==timber && f.PantryFood.Sum()==food && World.Footprint(f).Count()==6,"Resize lost timber/food or ground");
            var copy=World.LoadJson(w.SaveJson());for(int i=0;i<100;i++){w.Tick(.1f);copy.Tick(.1f);w.Validate();copy.Validate();}Check(w.SaveJson()==copy.SaveJson(),"Reshaped save differs");
            Check(w.ReshapePlot(f.Id,5),"Restore prepared rows failed");Check(!w.ReshapePlot(f.Id,6),"Free extra capacity accepted");Check(w.ReshapePlot(f.Id,2),"Second shrink failed");
            Check(w.Place(new(1,-8),0,BuildingKind.Cottage)!=null,"Released land not usable by a home");before=w.SaveJson();Check(!w.ReshapePlot(f.Id,5) && w.SaveJson()==before,"Restored rows over occupied land");w.Validate();
            w.SaveFile($"artifacts/plot-revision/{relaxed}.json");
            Console.WriteLine($"PASS plot revision {relaxed}: pure proposal, actual land for a home, retained goods/materials, bounded restore, rejection and exact continuation.");
        }
    }
}

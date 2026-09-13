using Inlanders.Simulation;

static class GatheringChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    static void Until(World w,Func<bool> done,string why,int ticks=2000)
    {for(int i=0;i<ticks && !done();i++){w.Tick(.1f);if(i%20==0)w.Validate();}Check(done(),why+"; "+string.Join(" | ",w.People.Select(p=>$"{p.Id}:{p.Task}:{p.Meal?.Gathering}:{p.Status}")));w.Validate();}
    static void Roundtrip(World w)
    {var a=World.LoadJson(w.SaveJson());var b=World.LoadJson(w.SaveJson());for(int i=0;i<60;i++){a.Tick(.1f);b.Tick(.1f);Check(a.SaveJson()==b.SaveJson(),"Gathering continuation differs");}}
    public static World Prepare()=>WorkplaceFoodChecks.PrepareVillage();
    public static void Run(bool meadow=false)
    {
        Directory.CreateDirectory("artifacts/shared-gathering");var original=meadow?CommonsLandComparison.Prepare(new()):Prepare();string baseline=original.SaveJson();
        original.SaveFile("artifacts/shared-gathering/start.json");
        foreach(bool spread in new[]{false,true})
        foreach(var center in new[]{new Cell(17,4),new(22,10)})
        {
            var w=World.LoadJson(baseline);Check(w.BeginGathering(center,spread),"Gathering refused: "+w.GatheringProblem(center));
            Check(w.Gathering!.Seats.Values.Distinct().Count()==w.Population,"Duplicate places");
            Check(w.Gathering.Seats.Values.All(c=>Enumerable.Range(0,4).All(r=>w.PlacementProblem(c,r,BuildingKind.SeatingGarden)!=null)),"Placing a building can overwrite a planned gathering place");
            Until(w,()=>w.People.Any(p=>p.Meal is {Gathering:true,Reserved:true}),"No gathering pickup");Roundtrip(w);
            Until(w,()=>w.People.Any(p=>p.Meal is {Gathering:true,Carrying:true}),"No physical gathering food");Roundtrip(w);
            var cancel=World.LoadJson(w.SaveJson());Check(cancel.CancelGathering(),"Cancellation refused");Roundtrip(cancel);
            Until(cancel,()=>cancel.People.All(p=>p.Meal?.Gathering!=true && p.Task!=Work.ReturnMeal),"Cancelled goods stranded");
            Until(w,()=>w.Gathering.Eating,"Residents never sat together");
            Check(w.People.All(p=>p.Meal is {Gathering:true,Carrying:true} && p.Task==Work.EatingMeal),"Shared meal began before everyone held food");
            w.SaveFile($"artifacts/shared-gathering/seated-{center.X}-{center.Z}-{spread}.json");Roundtrip(w);
            Until(w,()=>w.Gathering.Complete,"Gathering did not finish");
            Check(w.Gathering.Ate.Count==w.Population && w.People.All(p=>p.Carried==0),"Shared food not actually consumed");
            Roundtrip(w);Until(w,()=>w.People.Any(p=>p.WorkplaceId!=null),"No return to work");
            Console.WriteLine($"PASS: shared meal at {center}, all {w.Population} physically seated/ate, current saves, cancellation and return to work.");
        }
    }
}

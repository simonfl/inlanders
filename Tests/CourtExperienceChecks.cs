using Inlanders.Simulation;
using System.Text.Json;

static class CourtExperienceChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    static string Physical(World w)
    {
        var copy=World.LoadJson(w.SaveJson());copy.Neighborhood!.Arrangement!.Experience=null;return copy.SaveJson();
    }
    public static void Run()
    {
        var finite=World.NewCourtExperience(true);var open=World.NewCourtExperience(false);
        Check(finite.Population==16 && finite.Housed==16 && finite.SimulatesMeals,"Starting residents/rules differ");
        Check(Physical(finite)==Physical(open),"Comparison arms start from different worlds");
        Check(!open.FinishCourtPlace(),"Open court acquired a completion gate");
        Check(finite.FinishCourtPlace() && !finite.FinishCourtPlace(),"Player finish is not idempotent");
        // Ending is a personal stopping point, never a hidden quota or pause in simulation.
        for(int i=0;i<1200;i++){finite.Tick(.1f);open.Tick(.1f);}
        Check(Physical(finite)==Physical(open),"Finished state changed physical life");
        Check(finite.People.Any(p=>p.Meal!=null) && finite.Food.EatenVegetables>0,"No actual meals after finish");
        string saved=finite.SaveJson();finite=World.LoadJson(saved);Check(finite.SaveJson()==saved && finite.CourtStudy!.Finished,"Finished current save failed");
        var home=finite.Cottages.First(c=>c.Kind==BuildingKind.Cottage);
        var original=finite.CourtStudy!.StartingBuildings.Single(b=>b.Id==home.Id).Cell;
        var at=finite.Map.Land.OrderBy(c=>(c.Point-new Cell(-7,2).Point).LengthSquared()).First(c=>c!=home.Cell && finite.RelocationProblem(home.Id,c,1)==null);
        Check(finite.MoveBuilding(home.Id,at,1),"Finished place cannot be edited");
        Check(finite.CourtStudy.StartingBuildings.Single(b=>b.Id==home.Id).Cell==original,"Starting layout moved with building");
        finite.Validate();
        saved=finite.SaveJson();Check(World.LoadJson(saved).SaveJson()==saved,"Edited reference layout did not roundtrip");
        finite.CourtStudy.Finite=false;
        bool rejected=false;try{finite.Validate();}catch(InvalidOperationException){rejected=true;}
        Check(rejected,"Invalid free/finished save accepted");
        Console.WriteLine("PASS: matched sixteen-resident arms, identical continued life, user finish, no free-mode gate, independent starting layout, editable ending and current saves");
    }
}

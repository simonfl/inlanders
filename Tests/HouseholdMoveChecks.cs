using Inlanders.Simulation;
static class HouseholdMoveChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    public static void Run()
    {
        foreach(bool relaxed in new[]{false,true})
        {
            var w=World.NewTransformationHamlet(relaxed,true,false,true);var homes=w.Cottages.Where(c=>Buildings.Get(c.Kind).Beds>0).Take(2).ToArray();
            for(int i=0;i<1000 && !w.People.Any(p=>p.Meal?.Carrying==true);i++)w.Tick(.1f);
            Check(w.People.Any(p=>p.Meal?.Carrying==true),"No active meal exercised");var rests=w.People.ToDictionary(p=>p.Id,p=>p.RestVisits);
            var source=w.People.Where(p=>p.HomeId==homes[0].Id).ToArray();var target=w.People.Where(p=>p.HomeId==homes[1].Id).ToArray();var positions=w.People.Select(p=>p.Position).ToArray();string before=w.SaveJson();
            Check(w.HouseholdMoveProblem(homes[0].Id,homes[1].Id)==null && w.SaveJson()==before,"Household preview impure");
            Check(!w.MoveHousehold(homes[0].Id,homes[0].Id) && w.SaveJson()==before,"Invalid exchange mutates");
            Check(w.MoveHousehold(homes[0].Id,homes[1].Id),"Full homes cannot exchange");w.Validate();
            Check(source.All(p=>p.HomeId==homes[1].Id) && target.All(p=>p.HomeId==homes[0].Id) && w.People.Select(p=>p.Position).SequenceEqual(positions),"Household assignment or physical position wrong");
            var twin=World.LoadJson(w.SaveJson());for(int i=0;i<2400;i++){w.Tick(.1f);twin.Tick(.1f);if(i%10==0){w.Validate();twin.Validate();}}Check(w.SaveJson()==twin.SaveJson(),"Exchange continuation differs");
            Check(source.All(p=>p.RestVisits>rests[p.Id]) && target.All(p=>p.RestVisits>rests[p.Id]),"New homes never actually used");
            Console.WriteLine($"PASS household exchange {relaxed}: occupied homes, pure rejection, no teleport, active meal/save continuation and actual home visits.");
        }
    }
}

using Inlanders.Simulation;
using System.Text.Json;
static class MultipleSharedPlacesChecks
{
    public static void Run()
    {
        foreach(bool relaxed in new[]{false,true})
        {
            void Check(bool ok,string why){if(!ok)throw new Exception(why);}
            var w=World.NewRiverFrontage(relaxed,true);var first=w.Commons!;Check(w.SetCommons(first.Center,2),"First small place");
            var at=w.Map.Land.Where(c=>w.CommonsProblem(c,2)==null).OrderBy(c=>(c.Point-new Cell(-1,-4).Point).LengthSquared()).First();
            Check(w.AddCommons(at,2) && w.SharedPlaces.Count==2,"Second shared place replaced first");var second=w.SharedPlaces[1];
            string before=w.SaveJson();Check(!w.AddCommons(first.Center,2) && before==w.SaveJson(),"Overlapping place accepted or mutation on rejection");
            for(int i=0;i<6000;i++){w.Tick(.1f);if(i%100==0)w.Validate();}
            Check(first.FirstDiner!=null && second.FirstDiner!=null,"Both places did not serve actual meals");
            var copy=World.LoadJson(w.SaveJson());for(int i=0;i<100;i++){w.Tick(.1f);copy.Tick(.1f);}Check(w.SaveJson()==copy.SaveJson(),"Multiple-place continuation");
            for(int i=0;i<1200 && !w.People.Any(p=>p.Meal is {Commons:true,Reserved:true} or {Commons:true,Carrying:true} && second.Places.Contains(p.Meal.Seat));i++)w.Tick(.1f);
            var others=w.People.Where(p=>p.Meal is {Commons:true,Reserved:true} or {Commons:true,Carrying:true} && second.Places.Contains(p.Meal.Seat)).ToDictionary(p=>p.Id,p=>JsonSerializer.Serialize(p));
            Check(others.Count>0,"No active other-place claims for interruption check");
            string unchanged=JsonSerializer.Serialize(second);var target=w.Map.Land.Where(c=>c!=first.Center && w.CommonsProblem(c,2,first.Center)==null).OrderBy(c=>(c.Point-first.Center.Point).LengthSquared()).First();
            Check(w.MoveCommons(first.Center,target,2),"Selected move refused");Check(unchanged==JsonSerializer.Serialize(second),"Selected move modified another place");
            Check(others.All(pair=>pair.Value==JsonSerializer.Serialize(w.People[pair.Key])),"Selected move interrupted other diners");
            Check(w.RemoveCommons(target) && w.SharedPlaces.Count==1 && w.Commons==second,"Selected removal removed another place");
            Check(others.All(pair=>pair.Value==JsonSerializer.Serialize(w.People[pair.Key])),"Selected removal interrupted other diners");
            for(int i=0;i<200;i++)w.Tick(.1f);w.Validate();
            Console.WriteLine($"PASS multiple shared places {relaxed}: independent actual diners, overlap rejection, exact saves, selected move/removal and other-meal preservation");
        }
    }
}

using Inlanders.Simulation;
using System.Text.Json;

// F31c2: test the necessity of another livelihood before changing the playable rules.
static class FoundingLandUseChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    public static void Run()
    {
        const string folder="artifacts/founding-land-use";
        Directory.CreateDirectory(folder);
        var initial=World.NewFoundingSettlement();initial.SaveFile(folder+"/initial.json");
        var results=new List<object>();
        foreach(var arm in new[]{"homes-only","fish","woods","preserved-woods"})
        {
            var w=World.LoadJson(initial.SaveJson());var habitat=w.Map.Wildlife.Single();
            var events=new List<object>();
            if(arm=="preserved-woods")foreach(var t in w.Trees.Where(t=>habitat.Contains(t.Cell)))Check(w.SetTreePreserved(t.Cell,true),"Preservation rejected");
            void Build(BuildingKind kind,Cell target)
            {
                var p=ReviewPlacement.Find(w,kind,target,(c,r)=>true,"founding land-use")??throw new Exception("No legal site");
                Check(w.Place(p.Actual,p.Rotation,kind)!=null,"Order rejected");events.Add(new{kind,p.Actual,p.Rotation});
            }
            foreach(var at in new[]{new Cell(-3,0),new(0,0),new(0,6),new(-3,5),new(-6,6)})Build(BuildingKind.Cottage,at);
            if(arm=="fish")Build(BuildingKind.FishingDock,new(3,3));
            if(arm is "woods" or "preserved-woods")Build(BuildingKind.HuntingLodge,new(-5,-2));
            w.SaveFile(folder+"/"+arm+"-start.json");
            float? finish=null;double hungry=0,lateHungry=0;int lateMeals=0;
            for(int i=0;i<9000;i++)
            {
                w.Tick(.1f);hungry+=w.People.Count(p=>!p.Fed)*.1;
                if(i>=6000)lateHungry+=w.People.Count(p=>!p.Fed)*.1;
                if(w.Population<12 && w.InvitationProblem()==null)Check(w.InviteNewcomers(),"Invitation failed");
                if(finish==null && w.FinishFoundingProblem()==null){finish=w.Food.Time;Check(w.FinishFounding(),"Finish failed");}
                if(i==5999)lateMeals=w.Food.EatenBerries+w.Food.EatenFish+w.Food.EatenGame;
                if(i%100==0)w.Validate();
            }
            lateMeals=w.Food.EatenBerries+w.Food.EatenFish+w.Food.EatenGame-lateMeals;
            w.Validate();string saved=w.SaveJson();Check(World.LoadJson(saved).SaveJson()==saved,"Current save differs");
            w.SaveFile(folder+"/"+arm+"-late.json");
            results.Add(new{arm,finish,hungry,lateHungry,lateMeals,w.Population,w.Housed,stored=w.EdibleStored,
                berriesEaten=w.Food.EatenBerries,fishEaten=w.Food.EatenFish,gameEaten=w.Food.EatenGame,
                berriesGathered=w.Food.GatheredBerries,fishCaught=w.Food.CaughtFish,gameHunted=w.Food.HuntedGame,
                habitatTrees=w.HabitatTrees(habitat),habitatRecovery=w.HabitatRecovery(habitat),events});
            File.WriteAllText(folder+"/report.json",JsonSerializer.Serialize(results,new JsonSerializerOptions{WriteIndented=true}));
            Console.WriteLine(JsonSerializer.Serialize(results[^1]));
        }
        Console.WriteLine("PASS: matched starts, actual commands, fifteen-minute controls, conservation/current saves; no challenge assertion");
    }
}

using Inlanders.Simulation;
using System.Text.Json;

static class OrchardChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    static void Step(World w,int n=1){for(int i=0;i<n;i++){w.Tick(.1f);w.Validate();}}
    static void Until(World w,Func<bool> done,string why){for(int i=0;i<12000 && !done();i++)Step(w);Check(done(),why);}
    static void Roundtrip(World w)
    {
        string saved=w.SaveJson();var a=World.LoadJson(saved);var b=World.LoadJson(saved);Check(a.SaveJson()==saved,"Orchard save changed");
        Step(a,100);Step(b,100);Check(a.SaveJson()==b.SaveJson(),"Orchard continuation diverged");
    }
    public static void Run()
    {
        Directory.CreateDirectory("artifacts/orchard-playable");
        foreach(int rotation in new[]{0,1,2,3})
        {
            var w=World.NewScenario();w.Food.InitialBerries=w.Food.Berries=200;
            var at=w.Map.Land.First(c=>c.X>=2 && c.X<=6 && c.Z>=-4 && c.Z<=0 && w.PlacementProblem(c,rotation,BuildingKind.Orchard)==null);
            var orchard=w.Place(at,rotation,BuildingKind.Orchard)!;
            Until(w,()=>orchard.Complete,"Orchard construction failed");
            Check(w.SetOutputTarget(orchard.Id,0),"Cannot hold orchard target");Step(w,80);
            Check(!orchard.Planted,"Target zero planted new trees");w.SetOutputTarget(orchard.Id,-1);
            Until(w,()=>orchard.Planted,"No farmer planted orchard");float planted=w.Food.Time;
            Until(w,()=>orchard.Growth>.25f,"Trees did not establish");Roundtrip(w);
            w.SaveFile($"artifacts/orchard-playable/immature-{rotation}.json");
            Until(w,()=>orchard.Harvest==8,"Fruit never ripened");Check(w.Food.Time-planted>=179 && orchard.OrchardMature,"First fruit skipped establishment");Roundtrip(w);
            w.SaveFile($"artifacts/orchard-playable/ripe-{rotation}.json");
            Until(w,()=>w.People.Any(p=>p.WorkplaceId==orchard.Id && p.Task==Work.Harvesting && p.Timer>.8f),"No picking pose");
            w.SaveFile($"artifacts/orchard-playable/picking-{rotation}.json");
            Until(w,()=>w.People.Any(p=>p.Cargo==Resource.Fruit && p.Carried>0),"Fruit never carried");Roundtrip(w);
            w.SaveFile($"artifacts/orchard-playable/carry-{rotation}.json");
            var redirected=World.LoadJson(w.SaveJson());var carrier=redirected.People.First(p=>p.Cargo==Resource.Fruit && p.Carried>0);redirected.Assign(carrier.Id,Role.Builder);
            Until(redirected,()=>carrier.Carried==0,"Reassignment stranded fruit");redirected.Validate();
            Check(w.SetWorkplacePaused(orchard.Id,true),"Cannot pause orchard");Step(w,300);int remaining=orchard.Harvest;
            Step(w,700);Check(orchard.Harvest==remaining,"Paused orchard kept starting picks");
            w.SetOutputTarget(orchard.Id,0);w.SetWorkplacePaused(orchard.Id,false);
            Until(w,()=>orchard.Harvest==0,"Ripe fruit ignored at target zero");Step(w,50);
            Check(orchard.OrchardMature && !orchard.Planted,"Target did not hold next batch or erased trees");Roundtrip(w);
            int grown=w.Food.GrownFruit;w.SetOutputTarget(orchard.Id,-1);float repeat=w.Food.Time;
            Until(w,()=>w.Food.GrownFruit>grown,"Mature orchard did not regrow");Check(w.Food.Time-repeat<62,"Mature trees required replanting");
            w.SaveFile($"artifacts/orchard-playable/repeat-{rotation}.json");
            Check(w.RequestDemolition(orchard.Id),"Cannot clear orchard");Roundtrip(w);
            Until(w,()=>!w.Cottages.Any(c=>c.Id==orchard.Id),"Orchard demolition failed");
            Check(w.Food.GrownFruit==grown+8,"Demolished orchard kept growing");
            Check(w.Food.GrownFruit==w.StoredFood(Resource.Fruit)+w.Food.EatenFruit+w.People.Where(p=>p.Cargo==Resource.Fruit).Sum(p=>p.Carried),"Clearing lost fruit");
            Until(w,()=>w.PlacementProblem(at,rotation,BuildingKind.Orchard)==null,"Cleared plot stayed blocked");
            var replanted=w.Place(at,rotation,BuildingKind.Orchard)!;Check(!replanted.OrchardMature,"Rebuilt orchard inherited mature trees");
        }
        var reports=new List<object>();
        foreach(string route in new[]{"gardens","orchards","mixed"})
        {
            var w=World.NewQuarryMap();w.Food.InitialBerries=w.Food.Berries=24;w.Assign(3,Role.Unassigned);w.Assign(4,Role.Farmer);
            if(route=="orchards")
            {
                var old=w.Cottages.Single(c=>c.Kind==BuildingKind.VegetableGarden);w.Cottages.Remove(old);
                w.Cottages.Add(new Cottage{Id=old.Id,Cell=old.Cell,Kind=BuildingKind.Orchard,Delivered=4,Construction=1});
            }
            w.Place(new(3,-5),0,route=="gardens"?BuildingKind.VegetableGarden:BuildingKind.Orchard);w.Validate();
            bool sawMiss=false;
            for(int i=0;i<12000;i++){if(i==3600)w.Assign(4,Role.Builder);if(i==4800)w.Assign(4,Role.Farmer);Step(w);sawMiss|=w.Food.MealOutcomes.Any(m=>m.Skipped || !m.Timely);}
            Roundtrip(w);var meal=w.ReadMealAssessment();
            reports.Add(new{route,sawMiss,grownFruit=w.Food.GrownFruit,eatenFruit=w.Food.EatenFruit,grownVegetables=w.Food.GrownVegetables,stored=w.EdibleStored,meal});
            Check(!sawMiss,$"{route}: unexpected meal failure in selected comparison");
            if(route!="gardens")Check(w.Food.EatenFruit>0,"Orchard fruit was not eaten");
            w.SaveFile($"artifacts/orchard-playable/{route}.json");
        }
        File.WriteAllText("artifacts/orchard-playable/results.json",JsonSerializer.Serialize(reports,new JsonSerializerOptions{WriteIndented=true}));
        Console.WriteLine("PASS: playable orchards, four orientations, construction/targets, establishment/repeat growth, paused picking, fruit carrying/interruption, saved stages, demolition recovery and actual twenty-minute meals.");
    }
}

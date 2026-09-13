using Inlanders.Simulation;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;

static class WorkplaceFoodComparison
{
    public static void Run(bool foraging=false)
    {
        string folder="artifacts/workplace-food-comparison"+(foraging?"-siting":"");Directory.CreateDirectory(folder);
        var json=new JsonSerializerOptions{WriteIndented=true};var timer=Stopwatch.StartNew();
        var sources=Directory.GetFiles("Simulation","*.cs").Concat(new[]{"Tests/NeighborhoodComparison.cs","Tests/WorkplaceFoodComparison.cs"})
            .ToDictionary(path=>path,path=>Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path))));
        var baseline=NeighborhoodComparison.PrepareRecovery();string saved=baseline.SaveJson();
        NeighborhoodComparison.SaveChecked(baseline,$"{folder}/mistake.json");
        File.WriteAllText($"{folder}/manifest.json",JsonSerializer.Serialize(new{status="running",sources,baselineSha256=Convert.ToHexString(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(saved))),seconds=2400,step=.1},json));
        var results=new List<object>();
        foreach(bool local in new[]{false,true})foreach(string plan in foraging?new[]{"west-foraging","meadow-foraging","meadow-foraging-pantry"}:new[]{"restore-foraging","three-gardens","two-farms-two-bakeries"})
        {
            var w=World.LoadJson(saved);w.Neighborhood!.WorkplaceFood=local;
            Cottage Place(BuildingKind kind)=>NeighborhoodComparison.PlaceNear(w,kind,new(17,6));
            if(foraging)
            {
                // Unlike the old orientation-first plan, use all four facings before moving away from the requested site.
                Cell center=plan=="west-foraging"?new(-8,-3):new(17,8);
                var placement=w.Map.Land.SelectMany(cell=>Enumerable.Range(0,4).Select(rotation=>new{cell,rotation}))
                    .Where(p=>w.PlacementProblem(p.cell,p.rotation,BuildingKind.ForagerHut)==null)
                    .OrderBy(p=>(p.cell.Point-center.Point).LengthSquared()).ThenBy(p=>p.cell.Z).ThenBy(p=>p.cell.X).ThenBy(p=>p.rotation).First();
                w.Place(placement.cell,placement.rotation,BuildingKind.ForagerHut);
                if(plan.EndsWith("-pantry")){var pantry=Place(BuildingKind.Pantry);w.SetPantryTarget(pantry.Id,24);}
            }
            else if(plan=="restore-foraging")NeighborhoodComparison.PlaceNear(w,BuildingKind.ForagerHut,new(8,2));
            else
            {
                if(plan=="three-gardens")
                {
                    w.SetWorkplacePaused(w.Cottages.Single(c=>c.Kind==BuildingKind.Farm).Id,true);
                    for(int i=0;i<3;i++)Place(BuildingKind.VegetableGarden);
                }
                else {Place(BuildingKind.Farm);Place(BuildingKind.Bakery);Place(BuildingKind.Bakery);}
                var pantry=Place(BuildingKind.Pantry);w.SetPantryTarget(pantry.Id,24);
            }
            string name=(local?"workplace":"control")+"-"+plan;
            NeighborhoodComparison.SaveChecked(w,$"{folder}/{name}-start.json");
            double anyHunger=0,hungryPersonSeconds=0;float? firstFed=null;
            var activity=new Dictionary<string,double>();var samples=new List<object>();
            for(int tick=0;tick<24000;tick++)
            {
                w.Tick(.1f);if(tick%100==0)w.Validate();
                if(w.Food.Hunger==0)firstFed??=w.Food.Time-baseline.Food.Time;
                if(tick>=21000)
                {
                    if(w.Food.Hunger>0)anyHunger+=.1;
                    hungryPersonSeconds+=w.People.Count(p=>!p.Fed)*.1;
                    foreach(var person in w.People){string task=person.Task.ToString();activity[task]=activity.GetValueOrDefault(task)+.1;}
                }
                if(tick%600==599)samples.Add(new{seconds=(tick+1)*.1,food=w.EdibleStored,grain=w.StoredGrain,hunger=w.Food.Hunger,
                    vegetables=w.Food.GrownVegetables,bread=w.Food.BakedBread});
            }
            w.Validate();NeighborhoodComparison.SaveChecked(w,$"{folder}/{name}.json");
            int closed=w.Food.MealOutcomes.Count,timely=w.Food.MealOutcomes.Count(m=>m.Timely);
            results.Add(new{name,firstFed,last300AnyHungerSeconds=anyHunger,last300FedFraction=1-hungryPersonSeconds/(300*w.Population),
                recentClosedMeals=closed,recentTimelyMeals=timely,food=w.EdibleStored,grain=w.StoredGrain,
                vegetables=w.Food.GrownVegetables,bread=w.Food.BakedBread,
                buildings=w.Cottages.Select(c=>new{c.Id,c.Kind,c.Cell,c.Rotation,c.Complete}),activity,samples});
            File.WriteAllText($"{folder}/results.json",JsonSerializer.Serialize(results,json));
            Console.WriteLine($"{name}: final300 fed {1-hungryPersonSeconds/(300*w.Population):P1}, hunger {anyHunger:F1}s, food {w.EdibleStored}, recent meals {timely}/{closed}");
        }
        File.WriteAllText($"{folder}/manifest.json",JsonSerializer.Serialize(new{status="complete",sources,baselineSha256=Convert.ToHexString(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(saved))),arms=results.Count,seconds=2400,step=.1,wallSeconds=timer.Elapsed.TotalSeconds},json));
    }
}

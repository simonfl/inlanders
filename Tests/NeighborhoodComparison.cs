using Inlanders.Simulation;
using System.Text.Json;
using System.Diagnostics;
using System.Security.Cryptography;

// Design evidence, not an acceptance test: failed and dominating strategies remain in the report.
static class NeighborhoodComparison
{
    sealed record Sample(float Seconds,int Food,float Hunger,int Housed,int Welcomed,int Bread,int Vegetables);
    static readonly JsonSerializerOptions Json=new(){WriteIndented=true};
    static void SaveChecked(World w,string path)
    {
        string json=w.SaveJson();
        if(World.LoadJson(json).SaveJson()!=json)throw new Exception($"Comparison save failed roundtrip: {path}");
        File.WriteAllText(path,json);
    }
    static Cottage PlaceNear(World w,BuildingKind kind,Cell center)
    {
        // Preserve the fixed-plan orientation when possible, then try the other legal facings.
        foreach(int rotation in new[]{0,1,2,3})
        {
            var cell=w.Map.Land.Where(c=>c.X>5 && w.PlacementProblem(c,rotation,kind)==null)
                .OrderBy(c=>(c.Point-center.Point).LengthSquared()).ThenBy(c=>c.Z).ThenBy(c=>c.X).Select(c=>(Cell?)c).FirstOrDefault();
            if(cell is Cell chosen)return w.Place(chosen,rotation,kind)!;
        }
        throw new InvalidOperationException($"No legal east-bank space for {kind}; failed layout, not a successful recovery.");
    }
    public static void Recovery(bool capacity=false)
    {
        string folder=capacity?"artifacts/neighborhood-recovery-capacity":"artifacts/neighborhood-recovery";Directory.CreateDirectory(folder);
        var timer=Stopwatch.StartNew();
        var sources=Directory.GetFiles("Simulation","*.cs").Append("Tests/NeighborhoodComparison.cs")
            .OrderBy(p=>p,StringComparer.Ordinal).ToDictionary(p=>p,p=>Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(p))));
        File.WriteAllText($"{folder}/manifest.json",JsonSerializer.Serialize(new{status="running",sources,capacity},Json));
        var w=World.NewNeighborhoodLandscapeExperiment();
        var bridge=w.Place(new(5,2),1,BuildingKind.Bridge)!;
        for(int i=0;i<12000 && !bridge.Complete;i++)w.Tick(.1f);
        if(!bridge.Complete)throw new Exception("Recovery crossing failed");
        PlaceNear(w,BuildingKind.Cottage,new(8,2));PlaceNear(w,BuildingKind.Cottage,new(8,2));
        var venue=PlaceNear(w,BuildingKind.SeatingGarden,new(8,2));
        PlaceNear(w,BuildingKind.Farm,new(17,6));
        if(!w.RequestDemolition(w.Cottages.Single(c=>c.Kind==BuildingKind.ForagerHut).Id) || !w.InviteNewcomers())
            throw new Exception("Premature workplace replacement refused");
        for(int i=0;i<18000 && w.Food.Hunger==0;i++)
        {
            if(venue.Complete && w.Neighborhood!.VenueId==null)w.ChooseWelcomeVenue(venue.Id);
            w.Tick(.1f);if(i%100==0)w.Validate();
        }
        SaveChecked(w,$"{folder}/mistake.json");
        if(w.Food.Hunger==0)throw new Exception("Natural mistake did not create a shortage; revise experiment, do not inject food loss.");
        var results=new List<object>();string baseline=w.SaveJson();
        foreach(var (kind,count,pantry,pauseGrain) in new[]{(BuildingKind.VegetableGarden,1,false,false),(BuildingKind.Bakery,1,false,false),(BuildingKind.ForagerHut,1,false,false),
            (BuildingKind.VegetableGarden,2,false,false),(BuildingKind.Bakery,2,false,false),
            (BuildingKind.VegetableGarden,2,true,false),(BuildingKind.VegetableGarden,2,true,true),(BuildingKind.Bakery,2,true,false),
            (BuildingKind.VegetableGarden,3,true,true),(BuildingKind.VegetableGarden,4,true,true),(BuildingKind.Bakery,3,true,false)}
            .Where(a=>capacity?a.Item2>=3:a.Item2<3))
        {
            string name=$"{kind}-{count}"+(pantry?"-pantry":"")+(pauseGrain?"-pause-grain":"");
            var branch=World.LoadJson(baseline);var site=PlaceNear(branch,kind,kind==BuildingKind.Bakery?new(17,6):new(8,2));
            for(int n=1;n<count;n++)PlaceNear(branch,kind,new(17,6));
            if(pantry){var store=PlaceNear(branch,BuildingKind.Pantry,new(17,6));branch.SetPantryTarget(store.Id,24);}
            if(pauseGrain && !branch.SetWorkplacePaused(branch.Cottages.Single(c=>c.Kind==BuildingKind.Farm).Id,true))
                throw new Exception("Could not pause unused grain production");
            int ticks=capacity?24000:12000;
            float? firstFed=null;double hungerSeconds=0,last300HungerSeconds=0;
            for(int i=0;i<ticks;i++)
            {
                branch.Tick(.1f);if(i%100==0)branch.Validate();
                if(branch.Food.Hunger>0){hungerSeconds+=.1;if(i>=ticks-3000)last300HungerSeconds+=.1;}
                else firstFed??=branch.Food.Time-w.Food.Time;
            }
            SaveChecked(branch,$"{folder}/{name}.json");
            results.Add(new{recovery=name,mistakeAt=w.Food.Time,welcomeAlreadyComplete=w.Neighborhood!.Complete,observationSeconds=ticks*.1,firstFed,hungerSeconds,last300HungerSeconds,
                finalHunger=branch.Food.Hunger,food=branch.EdibleStored,site.Cell,site.Complete});
            File.WriteAllText($"{folder}/results.json",JsonSerializer.Serialize(results,Json));
            Console.WriteLine($"Recovery {name}: first fed {firstFed}, welcome already complete {w.Neighborhood!.Complete}, hunger {hungerSeconds:F1}s, final hunger {branch.Food.Hunger}, food {branch.EdibleStored}");
        }
        File.WriteAllText($"{folder}/manifest.json",JsonSerializer.Serialize(new{status="complete",sources,capacity,arms=results.Count,wallSeconds=timer.Elapsed.TotalSeconds},Json));
    }
    public static void Run(bool landscape=false)
    {
        string Folder=landscape?"artifacts/neighborhood-landscape":"artifacts/neighborhood-comparison";
        Directory.CreateDirectory(Folder);
        var timer=Stopwatch.StartNew();
        var sources=Directory.GetFiles("Simulation","*.cs").Append("Tests/NeighborhoodComparison.cs")
            .OrderBy(p=>p,StringComparer.Ordinal).ToDictionary(p=>p,p=>Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(p))));
        File.WriteAllText($"{Folder}/manifest.json",JsonSerializer.Serialize(new{startedUtc=DateTime.UtcNow,
            status="running",sources,stepSeconds=.1f,observationTicks=12000},Json));
        var opening=landscape?World.NewNeighborhoodLandscapeExperiment():World.NewNeighborhoodExperiment();var bridge=opening.Place(new(5,2),1,BuildingKind.Bridge)!;
        if(landscape)
        {
            var control=World.NewNeighborhoodExperiment();
            if(opening.InitialLogs!=control.InitialLogs || opening.EdibleStored!=control.EdibleStored || opening.Population!=control.Population ||
                opening.Bushes.Count!=control.Bushes.Count || !opening.Map.Land.Where(c=>c.X<5).ToHashSet().SetEquals(control.Map.Land.Where(c=>c.X<5)))
                throw new Exception("Landscape comparison changed starting resources, population or west-bank land");
        }
        for(int i=0;i<12000 && !bridge.Complete;i++)opening.Tick(.1f);
        if(!bridge.Complete)throw new Exception("Common bridge failed");
        string baseline=opening.SaveJson();SaveChecked(opening,$"{Folder}/baseline.json");
        var results=new List<object>();
        foreach(string location in new[]{"landing","meadow"})foreach(string food in new[]{"existing-berries","garden","grain-bakery"})
        {
            string name=$"{location}-{food}";var w=World.LoadJson(baseline);Cell center=location=="landing"?new(8,2):landscape?new(17,6):new(14,6);
            var homes=new[]{PlaceNear(w,BuildingKind.Cottage,center),PlaceNear(w,BuildingKind.Cottage,center)};
            var venue=PlaceNear(w,BuildingKind.SeatingGarden,center);
            if(food=="garden")PlaceNear(w,BuildingKind.VegetableGarden,center);
            if(food=="grain-bakery"){PlaceNear(w,BuildingKind.Farm,center);PlaceNear(w,BuildingKind.Bakery,center);}
            if(food!="existing-berries"){var pantry=PlaceNear(w,BuildingKind.Pantry,center);w.SetPantryTarget(pantry.Id,12);}
            if(!w.InviteNewcomers())throw new Exception("Common arrival commitment failed");
            float? completed=null;float hungerSeconds=0;float peakHunger=0;var samples=new List<Sample>();
            double distance=0,movingSeconds=0,idleSeconds=0;
            for(int i=0;i<12000;i++)
            {
                if(venue.Complete && w.Neighborhood!.VenueId==null)w.ChooseWelcomeVenue(venue.Id);
                var before=w.People.Select(p=>p.Position).ToArray();
                w.Tick(.1f);if(w.Food.Hunger>0)hungerSeconds+=.1f;peakHunger=Math.Max(peakHunger,w.Food.Hunger);
                for(int p=0;p<before.Length;p++)
                {
                    float moved=(w.People[p].Position-before[p]).Length();distance+=moved;
                    if(moved>.0001f)movingSeconds+=.1;
                    if(w.People[p].Task==Work.Waiting)idleSeconds+=.1;
                }
                if(w.Neighborhood!.Complete && completed==null){completed=w.Food.Time;SaveChecked(w,$"{Folder}/{name}-completion.json");}
                if(i%100==0)w.Validate();
                if(i%600==0)samples.Add(new(w.Food.Time,w.EdibleStored,w.Food.Hunger,w.Housed,w.Neighborhood.Welcomed.Count,w.Food.BakedBread,w.Food.GrownVegetables));
            }
            w.Validate();SaveChecked(w,$"{Folder}/{name}-final.json");
            results.Add(new{name,completed,elapsedAfterCrossing=completed-opening.Food.Time,hungerSeconds,peakHunger,
                food=w.EdibleStored,bread=w.Food.BakedBread,vegetables=w.Food.GrownVegetables,distance,movingSeconds,idleSeconds,
                constructionLogs=w.Cottages.Where(c=>c.Id>opening.Cottages.Max(b=>b.Id)).Sum(c=>c.Required),
                buildings=w.Cottages.Where(c=>c.Id>opening.Cottages.Max(b=>b.Id)).Select(c=>new{c.Id,c.Kind,c.Cell,c.Complete}),samples});
            File.WriteAllText($"{Folder}/results.json",JsonSerializer.Serialize(results,Json));
            Console.WriteLine($"{name}: complete {completed?.ToString("F1")??"NO"}s; hunger {hungerSeconds:F1}s peak {peakHunger:P0}; stock {w.EdibleStored}, baked {w.Food.BakedBread}, vegetables {w.Food.GrownVegetables}");
        }
        File.WriteAllText($"{Folder}/manifest.json",JsonSerializer.Serialize(new{finishedUtc=DateTime.UtcNow,
            status="complete",sources,stepSeconds=.1f,observationTicks=12000,arms=results.Count,
            baselineSha256=Convert.ToHexString(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(baseline))),
            wallSeconds=timer.Elapsed.TotalSeconds},Json));
        Console.WriteLine($"Comparison complete: {results.Count} arms, validated saves, {timer.Elapsed.TotalSeconds:F1}s wall time.");
    }
}

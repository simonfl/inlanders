using Inlanders.Simulation;
using System.Text.Json;
using System.Diagnostics;
using System.Security.Cryptography;

// Design evidence, not an acceptance test: failed and dominating strategies remain in the report.
static class NeighborhoodComparison
{
    sealed record Sample(float Seconds,int Food,float Hunger,int Housed,int Welcomed,int Bread,int Vegetables);
    static readonly string Folder="artifacts/neighborhood-comparison";
    static readonly JsonSerializerOptions Json=new(){WriteIndented=true};
    static void SaveChecked(World w,string path)
    {
        string json=w.SaveJson();
        if(World.LoadJson(json).SaveJson()!=json)throw new Exception($"Comparison save failed roundtrip: {path}");
        File.WriteAllText(path,json);
    }
    static Cottage PlaceNear(World w,BuildingKind kind,Cell center)
    {
        var cell=w.Map.Land.Where(c=>c.X>5 && w.PlacementProblem(c,0,kind)==null)
            .OrderBy(c=>(c.Point-center.Point).LengthSquared()).ThenBy(c=>c.Z).ThenBy(c=>c.X).First();
        return w.Place(cell,0,kind)!;
    }
    public static void Run()
    {
        Directory.CreateDirectory(Folder);
        var timer=Stopwatch.StartNew();
        var sources=Directory.GetFiles("Simulation","*.cs").Append("Tests/NeighborhoodComparison.cs")
            .OrderBy(p=>p,StringComparer.Ordinal).ToDictionary(p=>p,p=>Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(p))));
        File.WriteAllText($"{Folder}/manifest.json",JsonSerializer.Serialize(new{startedUtc=DateTime.UtcNow,
            status="running",sources,stepSeconds=.1f,observationTicks=12000},Json));
        var opening=World.NewNeighborhoodExperiment();var bridge=opening.Place(new(5,2),1,BuildingKind.Bridge)!;
        for(int i=0;i<12000 && !bridge.Complete;i++)opening.Tick(.1f);
        if(!bridge.Complete)throw new Exception("Common bridge failed");
        string baseline=opening.SaveJson();SaveChecked(opening,$"{Folder}/baseline.json");
        var results=new List<object>();
        foreach(string location in new[]{"landing","meadow"})foreach(string food in new[]{"existing-berries","garden","grain-bakery"})
        {
            string name=$"{location}-{food}";var w=World.LoadJson(baseline);Cell center=location=="landing"?new(8,2):new(14,6);
            var homes=new[]{PlaceNear(w,BuildingKind.Cottage,center),PlaceNear(w,BuildingKind.Cottage,center)};
            var venue=PlaceNear(w,BuildingKind.SeatingGarden,center);
            if(food=="garden")PlaceNear(w,BuildingKind.VegetableGarden,center);
            if(food=="grain-bakery"){PlaceNear(w,BuildingKind.Farm,center);PlaceNear(w,BuildingKind.Bakery,center);}
            if(food!="existing-berries"){var pantry=PlaceNear(w,BuildingKind.Pantry,center);w.SetPantryTarget(pantry.Id,12);}
            if(!w.InviteNewcomers())throw new Exception("Common arrival commitment failed");
            float? completed=null;float hungerSeconds=0;float peakHunger=0;var samples=new List<Sample>();
            for(int i=0;i<12000;i++)
            {
                if(venue.Complete && w.Neighborhood!.VenueId==null)w.ChooseWelcomeVenue(venue.Id);
                w.Tick(.1f);if(w.Food.Hunger>0)hungerSeconds+=.1f;peakHunger=Math.Max(peakHunger,w.Food.Hunger);
                if(w.Neighborhood!.Complete && completed==null){completed=w.Food.Time;SaveChecked(w,$"{Folder}/{name}-completion.json");}
                if(i%100==0)w.Validate();
                if(i%600==0)samples.Add(new(w.Food.Time,w.EdibleStored,w.Food.Hunger,w.Housed,w.Neighborhood.Welcomed.Count,w.Food.BakedBread,w.Food.GrownVegetables));
            }
            w.Validate();SaveChecked(w,$"{Folder}/{name}-final.json");
            results.Add(new{name,completed,elapsedAfterCrossing=completed-opening.Food.Time,hungerSeconds,peakHunger,
                food=w.EdibleStored,bread=w.Food.BakedBread,vegetables=w.Food.GrownVegetables,
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

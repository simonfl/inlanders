using Inlanders.Simulation;
using System.Security.Cryptography;
using System.Text.Json;

static class CommonsBypassCheck
{
    public static void Run()
    {
        const string folder="artifacts/commons-land-comparison";
        using var manifest=JsonDocument.Parse(File.ReadAllText(folder+"/manifest.json"));
        if(manifest.RootElement.GetProperty("status").GetString()!="complete")throw new Exception("Finish --commons-land first.");
        foreach(var source in manifest.RootElement.GetProperty("sources").EnumerateObject())
            if(Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(source.Name)))!=source.Value.GetString())throw new Exception("Comparison source changed: "+source.Name);
        var w=World.LoadFile(folder+"/first-act.json");
        if(Convert.ToHexString(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(w.SaveJson())))!=manifest.RootElement.GetProperty("baselineHash").GetString())throw new Exception("First-act state changed.");
        var gardens=w.Cottages.Where(c=>c.Kind==BuildingKind.VegetableGarden).Select(c=>c.Id).ToArray();
        bool Near(Cell c)=>c.X>5 && (c.Point-new Cell(18,6).Point).LengthSquared()<=25;
        var plan=ReviewPlacement.Find(w,BuildingKind.Square,new(18,6),
            (c,r)=>Near(c),"within five tiles of near center without demolition");
        if(plan==null)throw new Exception("Free-square evidence no longer reproduces; revisit the design inference.");
        float start=w.Food.Time;var site=w.Place(plan.Actual,plan.Rotation,BuildingKind.Square)!;
        for(int i=0;i<24000 && !site.Complete;i++){w.Tick(.1f);if(i%100==0)w.Validate();}
        if(!site.Complete || gardens.Any(id=>!w.Cottages.Any(c=>c.Id==id && !c.DemolitionRequested)))throw new Exception("Free square construction did not preserve the gardens.");
        float constructionSeconds=w.Food.Time-start;w.Validate();string state=w.SaveJson();var copy=World.LoadJson(state);
        if(copy.SaveJson()!=state)throw new Exception("Free square save differs.");
        for(int i=0;i<100;i++){w.Tick(.1f);copy.Tick(.1f);if(w.SaveJson()!=copy.SaveJson())throw new Exception("Free square continuation differs.");}
        w.SaveFile(folder+"/near-free-square.json");
        File.WriteAllText(folder+"/near-free-square-result.json",JsonSerializer.Serialize(new{plan,constructionSeconds,preservedGardens=gardens.Length},new JsonSerializerOptions{WriteIndented=true}));
        Console.WriteLine($"PASS: square built at {plan.Actual}, facing {plan.Rotation}, without removing any of {gardens.Length} gardens; active save continuation exact.");
    }
}

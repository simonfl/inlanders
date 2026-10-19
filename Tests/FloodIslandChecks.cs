using Inlanders.Simulation;
using System.Diagnostics;
using System.Text.Json;
static class FloodIslandChecks
{
    public static void Run()
    {
        Directory.CreateDirectory("artifacts/restoration");var results=new List<object>();
        foreach(bool relaxed in new[]{false,true})foreach(string choice in new[]{"bridge","single-island","adaptive-island"})
        {
            var watch=Stopwatch.StartNew();var w=World.NewRestorationChallenge(relaxed);
            bool begun=false,second=false,expanded=false;float? expansion=null;float? intervention=null,complete=null,firstDelivery=null,firstMissedMeal=null;
            var old=w.Cottages.First(c=>c.Kind==BuildingKind.VegetableField);
            var before=w.SaveJson();
            if(w.RelocationIntentProblem(old.Id)?.Contains("Ripe crops")!=true || w.GroupEligible(old) || w.MoveBuilding(old.Id,new(1,10),0) || before!=w.SaveJson())throw new Exception("Ripe crop can teleport or rejected query mutated world");
            if(w.PlacementProblem(new(0,-7),0,BuildingKind.Bridge)==null)throw new Exception("Far crossing available before opening island");
            for(int i=0;i<4500;i++)
            {
                if(!begun)
                {
                    begun=true;w.ApplyWoodlandCare(new(-10,3),new(-5,13),WoodlandIntent.Renew,false);
                    foreach(var at in new[]{new Cell(-2,10),new Cell(2,10)}){var p=ReviewPlacement.Find(w,BuildingKind.Cottage,at,(c,r)=>c.Z>3,"replacement")!;w.Place(p.Actual,p.Rotation,BuildingKind.Cottage);}
                    if(w.Place(new(5,0),0,BuildingKind.Bridge)==null)throw new Exception("Near crossing refused");
                }
                if(begun && !second && w.Cottages.Any(c=>c.Kind==BuildingKind.Bridge && c.Complete))
                {
                    if(w.Cottages.Where(c=>c.Kind==BuildingKind.VegetableField).Any(c=>!w.ReadWorkplace(c).MissingAccess))throw new Exception("Near crossing unexpectedly connects old fields");
                    second=true;intervention=w.Food.Time;w.SaveFile($"artifacts/restoration/{relaxed}-{choice}-island-open.json");
                    if(choice=="bridge"){if(w.Place(new(0,-7),0,BuildingKind.Bridge)==null)throw new Exception("Far crossing refused");}
                    else
                    {
                        var p=ReviewPlacement.Find(w,BuildingKind.VegetableField,new(1,-2),(c,r)=>World.Footprint(c,r,BuildingKind.VegetableField).All(t=>t.Z>-7 && t.Z<0),"new island cultivation")!;
                        if(w.Place(p.Actual,p.Rotation,BuildingKind.VegetableField)==null)throw new Exception("Island cultivation failed");
                    }
                }
                if(second && !expanded && choice=="adaptive-island" && w.Food.Time>=240 && w.ReachableEdibleStored<24 && w.ReadFoodFlow().Delivered<w.ReadFoodFlow().Required)
                {
                    w.SaveFile($"artifacts/restoration/{relaxed}-before-expansion.json");
                    var p=ReviewPlacement.Find(w,BuildingKind.VegetableField,new(5,-2),(c,r)=>World.Footprint(c,r,BuildingKind.VegetableField).All(t=>t.Z>-7 && t.Z<0),"expand undersized supply")??throw new Exception("No second island plot");
                    if(w.Place(p.Actual,p.Rotation,BuildingKind.VegetableField)==null)throw new Exception("Expansion refused");expanded=true;expansion=w.Food.Time;
                }
                w.Tick(.1f);
                if(firstDelivery==null && w.ChallengeDeliveries>0)firstDelivery=w.Food.Time;
                if(firstMissedMeal==null && w.People.Any(p=>!p.Fed))firstMissedMeal=w.Food.Time;
                if(w.Challenge!.Complete && complete==null){complete=w.Food.Time;w.SaveFile($"artifacts/restoration/{relaxed}-{choice}-complete.json");}
            }
            if(choice!="bridge" && w.Cottages.Where(c=>c.Kind==BuildingKind.VegetableField && c.Cell.Z==-9).Sum(c=>c.Harvest)!=40)throw new Exception("Island strategy collected disconnected crops");
            w.SaveFile($"artifacts/restoration/{relaxed}-{choice}-end.json");
            var copy=World.LoadJson(w.SaveJson());for(int i=0;i<20;i++){w.Tick(.1f);copy.Tick(.1f);}if(w.SaveJson()!=copy.SaveJson())throw new Exception("Island continuation differs");w.Validate();
            results.Add(new{relaxed,choice,intervention,expansion,firstDelivery,firstMissedMeal,complete,fields=w.Cottages.Count(c=>c.Kind==BuildingKind.VegetableField),salvageRemaining=w.Cottages.Where(c=>c.Kind==BuildingKind.VegetableField && c.Cell.Z==-9).Sum(c=>c.Harvest),horizon=450,population=w.Population,housed=w.ChallengeHoused,fed=w.People.Count(p=>p.Fed),food=w.EdibleStored,wallSeconds=watch.Elapsed.TotalSeconds});
            Console.WriteLine(JsonSerializer.Serialize(results.Last()));
            File.WriteAllText("artifacts/restoration/island-comparison.json",JsonSerializer.Serialize(results,new JsonSerializerOptions{WriteIndented=true}));
            if((choice!="single-island" && complete==null) || !second || w.Population!=12 || choice=="adaptive-island" && !expanded)throw new Exception($"Island choice failed {relaxed}/{choice}: {w.RestorationProblem()}");
        }
        File.WriteAllText("artifacts/restoration/island-comparison.json",JsonSerializer.Serialize(results,new JsonSerializerOptions{WriteIndented=true}));
    }
}

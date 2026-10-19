using Inlanders.Simulation;
using System.Diagnostics;
using System.Text.Json;
static class FloodIslandChecks
{
    public static void Run()
    {
        Directory.CreateDirectory("artifacts/restoration");var results=new List<object>();
        foreach(bool relaxed in new[]{false,true})foreach(string choice in new[]{"bridge","island","late-island"})
        {
            var watch=Stopwatch.StartNew();var w=World.NewRestorationChallenge(relaxed);
            bool begun=false,second=false;float? intervention=null,complete=null;
            if(w.PlacementProblem(new(0,-7),0,BuildingKind.Bridge)==null)throw new Exception("Far crossing available before opening island");
            for(int i=0;i<3600;i++)
            {
                if(!begun && (choice!="late-island" || w.Food.Time>=90))
                {
                    begun=true;w.ApplyWoodlandCare(new(-10,3),new(-5,13),WoodlandIntent.Renew,false);
                    foreach(var at in new[]{new Cell(-2,10),new Cell(2,10)}){var p=ReviewPlacement.Find(w,BuildingKind.Cottage,at,(c,r)=>c.Z>3,"replacement")!;w.Place(p.Actual,p.Rotation,BuildingKind.Cottage);}
                    if(w.Place(new(5,0),0,BuildingKind.Bridge)==null)throw new Exception("Near crossing refused");
                }
                if(begun && !second && w.Cottages.Any(c=>c.Kind==BuildingKind.Bridge && c.Complete))
                {
                    second=true;intervention=w.Food.Time;w.SaveFile($"artifacts/restoration/{relaxed}-{choice}-island-open.json");
                    if(choice=="bridge"){if(w.Place(new(0,-7),0,BuildingKind.Bridge)==null)throw new Exception("Far crossing refused");}
                    else
                    {
                        var field=w.Cottages.First(c=>c.Kind==BuildingKind.VegetableField);w.SetWorkplacePaused(field.Id,true);
                        var p=w.Map.Land.Where(c=>c.Z>-7 && c.Z<0).SelectMany(c=>Enumerable.Range(0,4).Select(r=>new{c,r})).FirstOrDefault(p=>w.RelocationProblem(field.Id,p.c,p.r)==null)??throw new Exception("No island field site");
                        if(!w.MoveBuilding(field.Id,p.c,p.r))throw new Exception("Island relocation failed");w.SetWorkplacePaused(field.Id,false);
                    }
                }
                w.Tick(.1f);
                if(w.Challenge!.Complete && complete==null){complete=w.Food.Time;w.SaveFile($"artifacts/restoration/{relaxed}-{choice}-complete.json");}
            }
            if(complete==null || !second || w.Population!=12)throw new Exception($"Island choice failed {relaxed}/{choice}: {w.RestorationProblem()}");
            var copy=World.LoadJson(w.SaveJson());for(int i=0;i<20;i++){w.Tick(.1f);copy.Tick(.1f);}if(w.SaveJson()!=copy.SaveJson())throw new Exception("Island continuation differs");w.Validate();
            results.Add(new{relaxed,choice,intervention,complete,horizon=360,population=w.Population,housed=w.ChallengeHoused,fed=w.People.Count(p=>p.Fed),food=w.EdibleStored,wallSeconds=watch.Elapsed.TotalSeconds});
            Console.WriteLine(JsonSerializer.Serialize(results.Last()));
        }
        File.WriteAllText("artifacts/restoration/island-comparison.json",JsonSerializer.Serialize(results,new JsonSerializerOptions{WriteIndented=true}));
    }
}

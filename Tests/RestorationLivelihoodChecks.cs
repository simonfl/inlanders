using Inlanders.Simulation;
using System.Text.Json;
static class RestorationLivelihoodChecks
{
    public static void Run()
    {
        Directory.CreateDirectory("artifacts/restoration-livelihood");
        var results=new List<object>();string status="running";var started=DateTime.UtcNow;
        void Report()=>File.WriteAllText("artifacts/restoration-livelihood/comparison.json",JsonSerializer.Serialize(new{started,status,assembly=typeof(RestorationLivelihoodChecks).Assembly.ManifestModule.ModuleVersionId,horizon=600,results},new JsonSerializerOptions{WriteIndented=true}));
        Report();try
        {
            foreach(bool relaxed in new[]{false,true})foreach(string plan in new[]{"salvage","fishing","gathering"})
            {
                var w=World.NewRestorationChallenge(relaxed);float? complete=null,firstSourceActivity=null;int initialTimber=w.Cottages.Sum(c=>c.Delivered);var sourceIds=new HashSet<int>();
                w.ApplyWoodlandCare(new(-10,3),new(-5,13),WoodlandIntent.Renew,false);
                foreach(var at in new[]{new Cell(-2,10),new Cell(2,10)}){var p=ReviewPlacement.Find(w,BuildingKind.Cottage,at,(c,r)=>c.Z>3,"replacement")!;w.Place(p.Actual,p.Rotation,BuildingKind.Cottage);}
                if(plan=="salvage")
                {
                    sourceIds.UnionWith(w.Cottages.Where(c=>c.Kind==BuildingKind.VegetableField && c.Cell.Z==-9).Select(c=>c.Id));
                    if(w.Place(new(5,0),0,BuildingKind.Bridge)==null)throw new Exception("Salvage crossing refused");
                }
                else
                {
                    var kind=plan=="fishing"?BuildingKind.FishingDock:BuildingKind.ForagerHut;
                    var p=ReviewPlacement.Find(w,kind,plan=="fishing"?new(8,4):new(-6,10),(c,r)=>c.Z>2,"home bank livelihood")??throw new Exception("No home-bank site");
                    var workplace=w.Place(p.Actual,p.Rotation,kind)??throw new Exception("Livelihood refused");
                    sourceIds.Add(workplace.Id);
                }
                for(int i=0;i<6000;i++)
                {
                    w.Tick(.1f);
                    if(firstSourceActivity==null && sourceIds.Any(id=>w.WorkplaceFoodDelivered(id)>0))firstSourceActivity=w.Food.Time;
                    if(complete==null && w.Challenge!.Complete){complete=w.Food.Time;w.SaveFile($"artifacts/restoration-livelihood/{relaxed}-{plan}-complete.json");}
                }
                w.Validate();w.SaveFile($"artifacts/restoration-livelihood/{relaxed}-{plan}-end.json");
                var copy=World.LoadJson(w.SaveJson());for(int i=0;i<20;i++){w.Tick(.1f);copy.Tick(.1f);}if(w.SaveJson()!=copy.SaveJson())throw new Exception("Livelihood save differs");
                results.Add(new{relaxed,plan,complete,firstSourceActivity,activityMeaning="first edible delivery attributed to the chosen workplace(s)",chosenSourceRecent=sourceIds.Sum(w.WorkplaceFoodDelivered),deliveredFish=w.DeliveredFish,food=w.ReachableEdibleStored,fed=w.People.Count(p=>p.Fed),timberCommitted=w.Cottages.Sum(c=>c.Delivered)-initialTimber,flow=w.ReadFoodFlow(),northernRipeAtHorizon=w.Cottages.Where(c=>c.Kind==BuildingKind.VegetableField && c.Cell.Z==-9).Sum(c=>c.Harvest)});
                Report();Console.WriteLine(JsonSerializer.Serialize(results.Last()));
                if(firstSourceActivity==null || complete==null)throw new Exception("Chosen livelihood did not deliver and complete by the horizon");
            }
            status="completed";
        }catch{status="failed";throw;}finally{Report();}
    }
}

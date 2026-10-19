using Inlanders.Simulation;
static class FloodRecoveryChecks
{
    public static void Run()
    {
        Directory.CreateDirectory("artifacts/restoration");
        foreach(bool relaxed in new[]{false,true})foreach(string choice in new[]{"bridge","move","nothing"})
        {
            var w=World.NewRestorationChallenge(relaxed);float? completed=null;
            if(w.Population!=12 || w.ChallengeHoused!=8 || w.InviteNewcomers() || w.PublicPlace!.Create().Challenge?.Restoration!=true)throw new Exception("Restoration opening/identity");
            if(choice!="nothing")
            {
                w.ApplyWoodlandCare(new(-10,3),new(-5,13),WoodlandIntent.Renew,false);
                foreach(var at in new[]{new Cell(-2,10),new Cell(2,10)}){var p=ReviewPlacement.Find(w,BuildingKind.Cottage,at,(c,r)=>c.Z>3,"replacement")!;if(w.Place(p.Actual,p.Rotation,BuildingKind.Cottage)==null)throw new Exception("Home refused");}
                if(choice=="bridge" && w.Place(new(5,0),0,BuildingKind.Bridge)==null)throw new Exception("Crossing refused");
                if(choice=="move")
                {
                    var field=w.Cottages.First(b=>b.Kind==BuildingKind.VegetableField);w.SetWorkplacePaused(field.Id,true);
                    var at=w.Map.Land.Where(c=>c.Z>3).SelectMany(c=>Enumerable.Range(0,4).Select(r=>new{c,r})).FirstOrDefault(p=>w.RelocationProblem(field.Id,p.c,p.r)==null)??throw new Exception("No move site");
                    w.MoveBuilding(field.Id,at.c,at.r);w.SetWorkplacePaused(field.Id,false);
                }
            }
            for(int i=0;i<6000;i++)
            {
                w.Tick(.1f);
                if(choice=="bridge" && w.Cottages.Any(c=>c.Kind==BuildingKind.Bridge && c.Cell==new Cell(5,0) && c.Complete) && !w.Cottages.Any(c=>c.Kind==BuildingKind.Bridge && c.Cell==new Cell(0,-7)))
                    if(w.Place(new(0,-7),0,BuildingKind.Bridge)==null)throw new Exception("Second crossing refused");
                if(w.Challenge!.Complete && completed==null){completed=w.Food.Time;w.SaveFile($"artifacts/restoration/{relaxed}-{choice}-complete.json");}
                if(i==1199)w.SaveFile($"artifacts/restoration/{relaxed}-{choice}-120s.json");
            }
            if(w.Challenge!.Complete!=(choice!="nothing") || w.Population!=12)throw new Exception($"Unexpected restoration {relaxed}/{choice}: {w.RestorationProblem()}");
            var copy=World.LoadJson(w.SaveJson());for(int i=0;i<20;i++){w.Tick(.1f);copy.Tick(.1f);}if(w.SaveJson()!=copy.SaveJson())throw new Exception("Restoration save diverged");
            w.Validate();Console.WriteLine($"PASS {relaxed}/{choice}: complete at {completed}, horizon600s, housed{w.ChallengeHoused}, fed{w.People.Count(p=>p.Fed)}, food{w.EdibleStored}, population{w.Population}");
        }
    }
}

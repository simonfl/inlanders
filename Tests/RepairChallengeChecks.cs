using Inlanders.Simulation;
static class RepairChallengeChecks
{
    public static void Run()
    {
        foreach(bool relaxed in new[]{false,true})foreach(string choice in new[]{"bridge","move","nothing"})
        {
            var w=World.NewNewcomerChallenge(relaxed,true);
            if(World.LoadJson(w.SaveJson()).SaveJson()!=w.SaveJson() || w.PublicPlace!.Create().Challenge?.Repair!=true)throw new Exception("Repair identity/save");
            w.ApplyWoodlandCare(new(-10,2),new(-5,13),WoodlandIntent.Renew,false);
            void Place(BuildingKind kind,Cell at){var p=ReviewPlacement.Find(w,kind,at,(c,r)=>c.Z>0,"south")??throw new Exception("No site "+kind);if(w.Place(p.Actual,p.Rotation,kind)==null)throw new Exception("Rejected site");}
            Place(BuildingKind.Cottage,new(-2,3));Place(BuildingKind.Cottage,new(2,3));
            if(choice=="bridge" && w.Place(new(5,0),0,BuildingKind.Bridge)==null)throw new Exception("Bridge: "+w.PlacementProblem(new(5,0),0,BuildingKind.Bridge));
            if(choice=="move")
            {
                var field=w.Cottages.First(b=>b.Kind==BuildingKind.VegetableField);w.SetWorkplacePaused(field.Id,true);
                var at=w.Map.Land.Where(c=>c.Z>0).SelectMany(c=>Enumerable.Range(0,4).Select(r=>new{c,r})).FirstOrDefault(p=>w.RelocationProblem(field.Id,p.c,p.r)==null)??throw new Exception("No move site");
                if(!w.MoveBuilding(field.Id,at.c,at.r))throw new Exception("Move failed");w.SetWorkplacePaused(field.Id,false);
            }
            int ticks=choice=="nothing"?600:12000;
            for(int i=0;i<ticks && !w.Challenge!.Complete;i++){w.Tick(.1f);if(i%1000==0)Console.WriteLine($"progress {choice} {relaxed} {w.Food.Time:0}s");if(w.ChallengeInvitationProblem()==null)w.InviteChallengeNeighbors();}
            if(w.Challenge!.Complete!=(choice!="nothing"))throw new Exception($"Unexpected {choice} result {w.ChallengeInvitationProblem()} / {w.ChallengeSettlingProblem()}");
            if(World.LoadJson(w.SaveJson()).SaveJson()!=w.SaveJson())throw new Exception("Repair result save");
            w.SaveFile($"artifacts/217-repair-{relaxed}-{choice}.json");
            Console.WriteLine($"PASS repair {choice} relaxed={relaxed} complete={w.Challenge.Complete} time={w.Food.Time:0} delivered={w.ChallengeDeliveries}");
        }
    }
}

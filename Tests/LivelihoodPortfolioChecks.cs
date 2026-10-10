using Inlanders.Simulation;
static class LivelihoodPortfolioChecks
{
public static void Run()
{
Directory.CreateDirectory("artifacts/livelihood-portfolios");
foreach(string option in new[]{"shore","woods-shore","bread","distributed"})
{
    var w=World.NewRiverLivelihood();
    Cottage Place(BuildingKind kind,Cell near)
    {
        foreach(var at in w.Map.Land.OrderBy(c=>(c.Point-near.Point).LengthSquared()))for(int r=0;r<4;r++)
            if(w.PlacementProblem(at,r,kind)==null){var site=w.Place(at,r,kind)!;Console.WriteLine($"{option}: {kind} at {at} r{r}");return site;}
        throw new Exception("No site "+kind);
    }
    if(option is "shore" or "woods-shore"){Place(BuildingKind.ForagerHut,new(-6,10));w.Place(new(8,5),1,BuildingKind.FishingDock);}
    if(option=="woods-shore")Place(BuildingKind.HuntingLodge,new(-5,-6));
    if(option=="bread"){Place(BuildingKind.Farm,new(0,-7));Place(BuildingKind.Bakery,new(0,0));}
    if(option=="distributed")foreach(var at in new[]{new Cell(2,-7),new(0,0),new(-3,4)})Place(BuildingKind.VegetableGarden,at);
    int initial=w.Cottages.Where(c=>!c.Complete).Sum(c=>c.Required),misses=0;float firstMiss=0;double travel=0,work=0;
    for(int i=0;i<36000;i++){w.Tick(.1f);if(w.People.Any(p=>!p.Fed)){if(firstMiss==0)firstMiss=w.Food.Time;misses++;}travel+=w.People.Count(p=>p.Route.Count>0)*.1;work+=w.People.Count(p=>p.WorkplaceId!=null)*.1;}
    w.Validate();w.SaveFile($"artifacts/livelihood-portfolios/{option}.json");
    Console.WriteLine($"RESULT {option}: investment{initial} firstMiss{firstMiss:0} missTicks{misses} food{w.EdibleStored} berries{w.Food.GatheredBerries} fish{w.Food.CaughtFish} game{w.Food.HuntedGame} veg{w.Food.GrownVegetables} bread{w.Food.BakedBread} travel{travel:0} work{work:0}");
}

}
}

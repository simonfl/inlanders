using Inlanders.Simulation;
static class ArrivalExperiments
{
    public static void Run()
    {
        foreach(string option in new[]{"berries","fish","late-field","woodland"})
        {
            var w=World.NewRiverLivelihood();
            Cottage Place(BuildingKind kind)
            {
                foreach(var at in w.Map.Land.OrderBy(c=>(c.Point-w.Stockpile.Point).LengthSquared()))for(int r=0;r<4;r++)
                    if(w.PlacementProblem(at,r,kind)==null)return w.Place(at,r,kind)!;
                throw new Exception("No ground for "+kind);
            }
            if(option is "berries" or "woodland")Place(BuildingKind.ForagerHut);
            if(option=="woodland")Place(BuildingKind.HuntingLodge);
            if(option=="fish")w.Place(new(8,5),1,BuildingKind.FishingDock);
            float hungerAt=0,first=0;
            for(int i=0;i<36000;i++)
            {
                if(option=="late-field" && i>=3000 && !w.Cottages.Any(c=>c.Kind==BuildingKind.VegetableField)){if(i==3000)Console.WriteLine("Late proposal: "+w.PlacementProblem(new(2,-7),1,BuildingKind.VegetableField,8));w.Place(new(2,-7),1,BuildingKind.VegetableField,8);}
                w.Tick(.1f);if(hungerAt==0 && w.People.Any(p=>!p.Fed))hungerAt=w.Food.Time;
                if(first==0 && w.FoundingHasNewFood)first=w.Food.Time;
            }
            w.Validate();Console.WriteLine($"ARRIVAL {option}: first {first:0}s hunger {hungerAt:0}s food {w.EdibleStored} berries {w.Food.GatheredBerries} fish {w.Food.CaughtFish} game {w.Food.HuntedGame} vegetables {w.Food.GrownVegetables}");
        }
    }
}

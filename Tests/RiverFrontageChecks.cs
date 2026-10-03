using Inlanders.Simulation;
static class RiverFrontageChecks
{
    public static void Run()
    {
        foreach(bool relaxed in new[]{false,true})
        {
            var w=World.NewRiverFrontage(relaxed,true);var control=World.NewTransformationHamlet(relaxed,true,false,true);
            if(w.Population!=12 || w.Housed!=12 || w.Cottages.Count!=9 || w.EdibleStored!=control.EdibleStored || w.InitialLogs!=control.InitialLogs || w.PublicPlace!.Create().SaveJson()!=w.SaveJson())throw new Exception("Inhabited frontage inventory/identity differs");
            for(int i=0;i<6000;i++)w.Tick(.1f);w.Validate();
            if(w.Food.EatenVegetables==0 || w.Housed!=12)throw new Exception("Inhabited frontage life failed");
            var copy=World.LoadJson(w.SaveJson());for(int i=0;i<100;i++){w.Tick(.1f);copy.Tick(.1f);}if(w.SaveJson()!=copy.SaveJson())throw new Exception("Inhabited continuation differs");
            Console.WriteLine($"PASS inhabited frontage {relaxed}: equal inventory, actual meals and exact continuation");
        }
        foreach(bool relaxed in new[]{false,true})foreach(bool fish in new[]{false,true})
        {
            var w=World.NewRiverFrontage(relaxed);var control=World.NewPlayerFounded(relaxed);
            void Check(bool ok,string why){if(!ok)throw new Exception(why);}
            Check(w.Population==control.Population && w.EdibleStored==control.EdibleStored && w.InitialLogs==control.InitialLogs,"Comparison changed starting economy");
            Check(w.PublicPlace!.Create().SaveJson()==w.SaveJson(),"Frontage restart identity");
            foreach(var c in new[]{new Cell(-3,7),new(1,7),new(-3,11),new(1,11)})Check(w.Place(c,0,BuildingKind.Cottage)!=null,"Home rejected");
            if(fish){var at=w.Map.Land.OrderBy(c=>(c.Point-new Cell(8,5).Point).LengthSquared()).First(c=>w.PlacementProblem(c,1,BuildingKind.FishingDock)==null);Check(w.Place(at,1,BuildingKind.FishingDock)!=null,"Frontage landing rejected");}
            else foreach(var c in new[]{new Cell(4,3),new(4,7)})Check(w.Place(c,0,BuildingKind.VegetableGarden)!=null,"Garden rejected");
            for(int i=0;i<6000;i++)w.Tick(.1f);
            w.Validate();Check(w.Housed==8 && w.FoundingHasNewFood,"Frontage not inhabited/productive");
            var copy=World.LoadJson(w.SaveJson());for(int i=0;i<100;i++){w.Tick(.1f);copy.Tick(.1f);}Check(w.SaveJson()==copy.SaveJson(),"Frontage save continuation");
            Console.WriteLine($"PASS river frontage {relaxed}/{fish}: homes, livelihood, equal reserves, restart and continuation");
        }
    }
}

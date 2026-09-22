using Inlanders.Simulation;
static class PlayerFoundedChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    public static void Run()
    {
        Directory.CreateDirectory("artifacts/player-founded");
        foreach(bool relaxed in new[]{false,true})foreach(string livelihood in new[]{"gardens","bread","fish"})
        {
            var w=World.NewPlayerFounded(relaxed);Check(w.Population==8 && w.Cottages.Count==0 && w.EdibleStored==120 && w.PublicPlace!.PlayerFounded,"Founding not genuinely open");
            Check(w.PublicPlace!.Create().SaveJson()==w.SaveJson(),"Restart identity differs");
            Check(w.FinishFoundingProblem()==null,"Founding regained prescribed completion gates");
            foreach(var c in new[]{new Cell(-3,7),new(1,7),new(-3,11),new(1,11)})Check(w.Place(c,0,BuildingKind.Cottage)!=null,"Home proposal rejected");
            if(livelihood=="gardens")foreach(var c in new[]{new Cell(4,3),new(4,7)})Check(w.Place(c,0,BuildingKind.VegetableGarden)!=null,"Garden rejected");
            if(livelihood=="bread"){Check(w.Place(new(1,-5),0,BuildingKind.Farm)!=null,"Grain rejected");Check(w.Place(new(5,-3),0,BuildingKind.Bakery)!=null,"Oven rejected");}
            if(livelihood=="fish")Check(w.Place(new(8,8),1,BuildingKind.FishingDock)!=null,"Landing rejected");
            int hungry=0;for(int i=0;i<12000;i++){w.Tick(.1f);hungry+=w.People.Count(p=>!p.Fed);if(i%100==0)w.Validate();}
            w.SaveFile($"artifacts/player-founded/{relaxed}-{livelihood}-diagnostic.json");
            Check(w.Housed==8 && w.FoundingHasNewFood && hungry==0,"Founding failed: "+livelihood+$" housed={w.Housed} newfood={w.FoundingHasNewFood} hungryTicks={hungry} stored={w.EdibleStored}");
            w.SaveFile($"artifacts/player-founded/{relaxed}-{livelihood}.json");var copy=World.LoadJson(w.SaveJson());for(int i=0;i<100;i++){w.Tick(.1f);copy.Tick(.1f);}Check(w.SaveJson()==copy.SaveJson(),"Founding continuation differs");
            Console.WriteLine($"PASS founded {relaxed}/{livelihood}: food {w.EdibleStored}, no hungry ticks; authored homes, exact continuation.");
        }
    }
}

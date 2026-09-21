using Inlanders.Simulation;
static class GroupedFarmsteadChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    public static void Run()
    {
        Directory.CreateDirectory("artifacts/grouped-farmsteads");
        foreach(bool relaxed in new[]{false,true})
        {
            var bank=new HamletProfile(relaxed,true).Create();var profile=new HamletProfile(relaxed,true,true);var w=profile.Create();
            Check(w.Population==bank.Population && w.Housed==bank.Housed && w.Stored==bank.Stored && w.Planks==bank.Planks && w.EdibleStored==bank.EdibleStored,"Unmatched people or supplies");
            Check(w.Cottages.Select(c=>c.Kind).OrderBy(k=>k).SequenceEqual(bank.Cottages.Select(c=>c.Kind).OrderBy(k=>k)),"Unmatched building inventory");
            Check(w.Trees.Select(t=>(t.Cell,t.Logs,t.Preserved)).SequenceEqual(bank.Trees.Select(t=>(t.Cell,t.Logs,t.Preserved))),"Unmatched woodland");
            Check(w.Cottages.Where(c=>World.IsVegetablePlot(c.Kind)).Sum(c=>World.VegetableYield(c.Kind))==48,"Unmatched crops");
            Check(w.Cottages.All(c=>w.Paths.Contains(c.Entrance)),"Missing entrance path");
            Check(w.Cottages.Where(c=>c.Kind==BuildingKind.Cottage).All(c=>Enumerable.Range(0,4).Any(side=>w.HomeYardProblem(c.Id,side)==null)),"Home lacks domestic ground");
            w.Validate();w.SaveFile($"artifacts/grouped-farmsteads/{(relaxed?"relaxed":"normal")}-opening.json");
            int hungry=0;for(int i=0;i<6000;i++){w.Tick(.1f);if(w.People.Any(p=>!p.Fed))hungry++;if(i%100==0)w.Validate();}
            Check(w.FoundingHasNewFood,"No actual harvest delivered");
            var copy=World.LoadJson(w.SaveJson());for(int i=0;i<100;i++){w.Tick(.1f);copy.Tick(.1f);}Check(w.SaveJson()==copy.SaveJson() && copy.PublicPlace==profile && copy.PublicPlace.Create().PublicPlace==profile,"Save/restart identity differs");
            w.SaveFile($"artifacts/grouped-farmsteads/{(relaxed?"relaxed":"normal")}-lived.json");
            Console.WriteLine($"PASS grouped farmsteads {profile.ModeName}: equal inventory, legal domestic ground, actual cultivation, exact continuation; hungry ticks {hungry}, food {w.EdibleStored}. No enjoyment claim.");
        }
    }
}

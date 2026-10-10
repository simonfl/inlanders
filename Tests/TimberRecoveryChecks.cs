using Inlanders.Simulation;
static class TimberRecoveryChecks
{
    public static void Run()
    {
        var w=World.NewRiverLivelihood();
        for(int n=0;n<7;n++)
        {
            var at=w.Map.Land.First(c=>w.PlacementProblem(c,0,BuildingKind.SeatingGarden)==null);w.Place(at,0,BuildingKind.SeatingGarden);
        }
        for(int n=0;n<3;n++)
        {
            var at=w.Map.Land.First(c=>w.PlacementProblem(c,0,BuildingKind.SeatingGarden)==null);w.Place(at,0,BuildingKind.SeatingGarden);
        }
        if(!w.ReadEconomy().Issues.Any(i=>i.Harvest))throw new Exception("Preserved timber deadlock hidden");
        var tree=w.Trees.First(t=>w.PreserveTreeProblem(t.Cell)==null);int id=tree.Id;
        if(!w.SetTreePreserved(tree.Cell,false))throw new Exception("Harvest rejected");
        for(int i=0;i<4000 && (!tree.Felled || tree.Logs>0);i++)w.Tick(.1f);
        if(!tree.Felled || tree.Logs!=0 || !w.Trees.Any(t=>t.Id==id) || tree.ClearRequested)throw new Exception("Harvest uprooted tree or failed");
        w.Validate();var copy=World.LoadJson(w.SaveJson());for(int i=0;i<100;i++){w.Tick(.1f);copy.Tick(.1f);}if(w.SaveJson()!=copy.SaveJson())throw new Exception("Harvest continuation differs");
        Console.WriteLine("PASS preserved timber shortage, selected harvest, real timber work with stump retained, exact continuation.");
    }
}

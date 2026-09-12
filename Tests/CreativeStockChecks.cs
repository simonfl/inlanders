using Inlanders.Simulation;

static class CreativeStockChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    static void Step(World w,int ticks=1){for(int i=0;i<ticks;i++){w.Tick(.1f);w.Validate();}}
    public static void Run()
    {
        var normal=World.NewScenario();string ordinary=normal.SaveJson();Check(!normal.SetCreativeCentralStock(Resource.Logs,50) && normal.SaveJson()==ordinary,"Normal stock edited");
        var w=World.NewCreative(true);foreach(var p in w.People)w.Assign(p.Id,Role.Unassigned);
        int initialLogs=w.InitialLogs,initialBerries=w.Food.InitialBerries;
        foreach(var kind in Enum.GetValues<Resource>())
        {
            Check(w.SetCreativeCentralStock(kind,64),"Cannot add "+kind);w.Validate();
            string saved=w.SaveJson();Check(w.SetCreativeCentralStock(kind,64) && saved==w.SaveJson(),"No-op edit changed ledger");
            Check(!w.SetCreativeCentralStock(kind,-1) && !w.SetCreativeCentralStock(kind,10000) && saved==w.SaveJson(),"Invalid edit changed stock");
            Check(w.SetCreativeCentralStock(kind,16) && w.CreativeCentralStock(kind)==16 && w.CreativeRemoved(kind)==48,"Removal ledger incorrect");w.Validate();
        }
        Check(w.InitialLogs==initialLogs && w.Food.InitialBerries==initialBerries && w.SawnLogs==0 && w.QuarriedStone==0 && w.Food.BakedBread==0 && w.Food.GrownGrain==0 && w.Food.GrownFruit==0 && w.DeliveredBerries==0 && w.DeliveredBread==0,"Creative edits fabricated production or deliveries");
        string before=w.SaveJson();var copy=World.LoadJson(before);Check(copy.SaveJson()==before,"Stock ledger roundtrip differs");
        foreach(var kind in new[]{BuildingKind.Bakery,BuildingKind.Sawmill,BuildingKind.Pantry})w.Place(w.Map.Land.First(c=>w.PlacementProblem(c,0,kind)==null),0,kind);
        foreach(var site in w.Cottages.Where(c=>c.Kind is BuildingKind.Bakery or BuildingKind.Sawmill))w.SetOutputTarget(site.Id,64);
        w.Assign(0,Role.Baker);w.Assign(1,Role.Sawyer);w.Assign(2,Role.Hauler);
        for(int i=0;i<100 && (w.ReservedGrain==0 || w.CreativeProtectedStock(Resource.Logs)==0);i++)Step(w);
        Check(w.ReservedGrain>0 && w.CreativeProtectedStock(Resource.Logs)>0,"No live input reservations");
        var foodCarrier=w.People.FirstOrDefault(p=>p.Task==Work.ToFoodPickup);
        Check(foodCarrier!=null,"No local pantry food reservation");
        foreach(var kind in new[]{Resource.Grain,Resource.Logs,foodCarrier!.Cargo})
        {
            string saved=w.SaveJson();int minimum=w.CreativeProtectedStock(kind);
            Check(!w.SetCreativeCentralStock(kind,minimum-1) && w.SaveJson()==saved,"Edit destroyed reserved inputs");
            Check(w.SetCreativeCentralStock(kind,minimum),"Protected minimum refused");w.Validate();
        }
        for(int i=0;i<3000 && (w.Food.BakedBread==0 || w.SawnLogs==0);i++)Step(w);
        Check(w.Food.BakedBread>0 && w.SawnLogs>0,"Reserved jobs failed after stock edit");
        foreach(var kind in World.EdibleKinds)
        {
            int local=w.StoredFood(kind)-w.CreativeCentralStock(kind);var cargo=w.People.Select(p=>p.Carried).ToArray();
            Check(w.SetCreativeCentralStock(kind,w.CreativeCentralStock(kind)+8),"Cannot replenish central food");
            Check(w.StoredFood(kind)-w.CreativeCentralStock(kind)==local && cargo.SequenceEqual(w.People.Select(p=>p.Carried)),"Setup changed local or carried stock");w.Validate();
        }
        before=w.SaveJson();copy=World.LoadJson(before);Step(w,300);Step(copy,300);Check(w.SaveJson()==copy.SaveJson(),"Stock-edited continuation differs");
        Console.WriteLine("PASS: all ten Creative central resources, explicit ledgers, unchanged production/delivery history, input reservations, live production and exact saves.");
    }
}

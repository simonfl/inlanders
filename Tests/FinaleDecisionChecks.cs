using Inlanders.Simulation;

static class FinaleDecisionChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    internal static void Until(World w,Func<bool> done,string why,int limit=24000)
    {
        for(int i=0;i<limit && !done();i++){w.Tick(.1f);if(i%100==0)w.Validate();}
        if(!done()) {File.WriteAllText("artifacts/finale-stalled.json",w.SaveJson());Console.WriteLine($"Bread {w.Food.Bread}, grain {w.Food.Grain}, baked {w.Food.BakedBread}, eaten bread {w.Food.EatenBread}");foreach(var c in w.Cottages.Where(c=>c.Kind is BuildingKind.Farm or BuildingKind.Bakery))Console.WriteLine($"{c.Kind} {c.Cell} target {c.OutputTarget}: {w.ReadWorkplace(c)}");}
        w.Validate();Check(done(),$"{why} stalled at {w.Food.Time:F0}s: residents {w.Population}, beds {w.Beds}, food {w.EdibleStored}, rested {w.People.Count(w.RecentlyRested)}, recreation {Recreation(w)}; {w.ReadMealAssessment().Summary}");
    }
    internal static Cottage Build(World w,Cell c,BuildingKind kind,int r=0)=>w.Place(c,r,kind)??throw new Exception($"Cannot place {kind} at {c}: {w.PlacementProblem(c,r,kind)}");
    internal static void Grow(World w,int target)
    {
        while(w.Population<target){Until(w,()=>w.InvitationProblem()==null,"newcomer provisions");Check(w.InviteNewcomers(),"Invitation failed");}
    }
    static void SaveCheck(World w)
    {
        string save=w.SaveJson();var copy=World.LoadJson(save);Check(copy.SaveJson()==save,"Finale map save differs");
    }
    static int Recreation(World w)=>w.People.Count(p=>p.LastLeisureTime is float t && w.Food.Time-t<120);
    static bool Supported(World w,float since)=>w.ReadMealAssessment(since).Reliable && w.ReadMealAssessment(since).FreshSupply && w.People.Count(w.RecentlyRested)>=(w.Population*3+3)/4 && Recreation(w)>=(w.Population+1)/2;
    public static void Run()
    {
        Directory.CreateDirectory("artifacts");
        var initial=World.NewLastingVillageMap();SaveCheck(initial);
        Console.WriteLine($"Finale budget: {initial.InitialLogs} total logs, {initial.YardLogs} yard, {initial.Trees.Count(t=>t.Cell.X<6)*8} standing west, {initial.Trees.Count(t=>t.Cell.X>6)*8} standing east; {initial.Map.Land.Count()} land tiles.");
        File.WriteAllText("artifacts/finale-initial.json",initial.SaveJson());
        foreach(bool local in new[]{false,true})RunRoute(local,false);
        RunRoute(true,true);
        RunRoute(true,true,false,true);
        RunRoute(true,false,true);
    }
    static void RunRoute(bool local,bool prebuild,bool poor=false,bool earlyStaff=false)
    {
        string stem=$"{(local?"local":"central")}-prebuild{prebuild}-early{earlyStaff}-poor{poor}";
        var w=World.NewLastingVillageMap();
        var bridge=Build(w,new(6,3),BuildingKind.Bridge,1);
        Build(w,new(-2,0),BuildingKind.Cottage);
        Until(w,()=>w.PlacementProblem(new(1,5),0,BuildingKind.Cottage)==null,"central home access");
        Build(w,new(1,5),BuildingKind.Cottage);
        w.Assign(7,Role.Logger);
        Until(w,()=>bridge.Complete && w.Beds>=12,"opening construction");
        if(prebuild)
        {
            if(earlyStaff){w.Assign(4,Role.Baker);w.Assign(6,Role.Baker);}
            SecondBuild(w,local);CelebrationBuild(w,local,false);
        }
        Grow(w,12);w.Assign(8,Role.Farmer);w.Assign(9,Role.Builder);
        float first=w.Food.Time;
        Until(w,()=>w.ReadMealAssessment(first).Reliable && w.ReadMealAssessment(first).FreshSupply,"twelve-person support");
        Console.WriteLine($"Finale {(local?"local":"central")}, prebuild {prebuild}: twelve supported at {w.Food.Time:F0}s.");
        if(!prebuild)SecondBuild(w,local);
        Until(w,()=>w.Beds>=20,"second neighborhood homes");
        Grow(w,20);
        foreach(int id in new[]{10,12,14})w.Assign(id,Role.Farmer);
        float second=w.Food.Time;
        Until(w,()=>Supported(w,second),"twenty-person support");
        Console.WriteLine($"Finale {(local?"local":"central")}, prebuild {prebuild}: twenty supported at {w.Food.Time:F0}s; food {w.Food.EdibleStored}, last-window missed {w.ReadMealAssessment(second).Missed}.");
        SaveCheck(w);File.WriteAllText($"artifacts/finale-{stem}.json",w.SaveJson());
        if(!prebuild)CelebrationBuild(w,local,poor);
        w.Assign(16,Role.Farmer);
        if(!earlyStaff){w.Assign(17,Role.Baker);if(!poor)w.Assign(19,Role.Baker);}
        if(poor)
        {
            float checkpoint=w.Food.Time+600;
            Until(w,()=>w.Food.Time>=checkpoint,"observe remote bakery");
            Console.WriteLine($"Remote bakery after 600s: central bread {w.Food.Bread}/{w.SupperCost}, all stored bread {w.StoredFood(Resource.Bread)}, can celebrate {w.CanCelebrate}.");
            Check(!w.CanCelebrate,"Poor allocation fixture no longer needs repair");
            File.WriteAllText("artifacts/finale-poor-bakery.json",w.SaveJson());
            foreach(var producer in w.Cottages.Where(c=>c.Kind is BuildingKind.Farm or BuildingKind.Bakery).ToArray())
            {Check(w.RequestDemolition(producer.Id),"Civic production relocation rejected");Until(w,()=>!w.Cottages.Contains(producer),"recover remote production");}
            w=World.LoadJson(w.SaveJson());CelebrationBuild(w,local,false);w.Assign(19,Role.Baker);
        }
        Until(w,()=>w.CanCelebrate,"celebration bread and access");
        float celebration=w.Food.Time;
        Check(w.BeginSupper(),"Prepared celebration rejected");
        string gathering=w.SaveJson();w=World.LoadJson(gathering);Check(w.SaveJson()==gathering,"Gathering save differs");
        Until(w,()=>w.Food.SupperComplete,"shared village supper");
        float after=w.Food.Time;
        Until(w,()=>Supported(w,after),"food and services after celebration");
        Console.WriteLine($"Finale {(local?"local":"central")}, prebuild {prebuild}, early staffing {earlyStaff}, poor {poor}: supper begins {celebration:F0}s, finishes {after:F0}s, renewed support {w.Food.Time:F0}s; {w.Food.SupperBread} bread shared.");
        File.WriteAllText($"artifacts/finale-celebrated-{stem}.json",w.SaveJson());
    }
    internal static void SecondBuild(World w,bool local)
    {
        foreach(var c in new[]{new Cell(10,0),new(14,0),new(18,0),new(18,4)})Build(w,c,BuildingKind.Cottage);
        if(!local)
        {
            // Moving opening homes releases scarce central production plots.
            Build(w,new(10,4),BuildingKind.Cottage);Build(w,new(14,4),BuildingKind.Cottage);
            foreach(var cell in new[]{new Cell(-2,0),new(1,5)})
            {
                var home=w.Cottages.Single(c=>c.Cell==cell);Check(w.RequestDemolition(home.Id),"Home relocation rejected");
                Until(w,()=>!w.Cottages.Contains(home),"recover central home materials");
            }
        }
        foreach(var c in local?new[]{new Cell(10,5),new(14,5),new(18,7)}:new[]{new Cell(-2,0),new(1,5),new(10,7)})Build(w,c,BuildingKind.VegetableGarden);
        Build(w,new(14,8),BuildingKind.Square);
        if(local)Build(w,new(10,8),BuildingKind.Pantry);
    }
    internal static void FreeCentralPlot(World w,bool local,Cell cell,Cell replacementCell,int rotation=0)
    {
        if(local)
        {
            var replacement=Build(w,replacementCell,BuildingKind.Cottage,rotation);
            Until(w,()=>replacement.Complete,"replacement home before civic conversion");
        }
        var site=w.Cottages.Single(c=>c.Cell==cell);
        Check(w.RequestDemolition(site.Id),"Central civic plot conversion rejected");
        Until(w,()=>!w.Cottages.Contains(site),"recover central plot materials");
    }
    internal static void CelebrationBuild(World w,bool local,bool remote)
    {
        if(remote){Build(w,new(22,7),BuildingKind.Farm);Until(w,()=>w.PlacementProblem(new(8,10),0,BuildingKind.Bakery)==null,"remote bakery access");Build(w,new(8,10),BuildingKind.Bakery);return;}
        FreeCentralPlot(w,local,new(1,5),new(21,10));
        FreeCentralPlot(w,local,new(-2,0),new(10,-3),2);
        FreeCentralPlot(w,true,new(-6,4),new(14,-3),2);
        Build(w,new(-2,0),BuildingKind.Farm);
        Until(w,()=>w.PlacementProblem(new(1,5),0,BuildingKind.Bakery)==null,"central bakery approach");
        Build(w,new(1,5),BuildingKind.Bakery);
        Build(w,new(-6,4),BuildingKind.Bakery);
        Until(w,()=>w.PlacementProblem(new(0,2),0,BuildingKind.SeatingGarden)==null,"local break space");
        Build(w,new(0,2),BuildingKind.SeatingGarden);
    }
}

using Inlanders.Simulation;

public static class ResourceSurveyChecks
{
    static void Check(bool ok,string why) { if(!ok) throw new Exception(why); }
    public static void Run()
    {
        Check(!new World().ResourceSources().Any(),"Original map invented source opportunities");
        var w=World.NewCreative(true);
        var quarry=w.Place(new(-9,2),false,BuildingKind.Quarry)!;
        Check(quarry!=null,"Survey quarry rejected");
        var stone=new SourceKey(SourceKind.Stone,0);
        var report=w.ReadResourceSurvey(stone)!;
        Check(report.Workplaces.SequenceEqual(new[]{quarry!.Id}) && report.Detail.Contains("16 stone available") && report.Detail.Contains("no regrowth"),"Stone stock/range omitted");
        w.Assign(0,Role.Quarrier);
        for(int i=0;i<1000 && w.People[0].DepositId==null;i++) w.Tick(.1f);
        Check(w.People[0].DepositId==0 && w.ReadResourceSurvey(stone)!.Detail.Contains("2 reserved"),"Stone claim not shown");
        string saved=w.SaveJson();
        foreach(var source in w.ResourceSources()) for(int i=0;i<5;i++) w.ReadResourceSurvey(source.Key);
        Check(saved==w.SaveJson(),"Resource survey mutated world");
        Check(w.ReadResourceSurvey(new(SourceKind.Stone,99))==null,"Missing source reused an ID");
        w.Assign(0,Role.Unassigned); w.RemoveBuilding(quarry.Id);
        Check(w.ReadResourceSurvey(stone)!.Workplaces.Length==0,"Removed quarry remains linked");
        var woodland=new SourceKey(SourceKind.Woodland,0); var habitat=w.Map.Wildlife[0];
        var before=w.ReadResourceSurvey(woodland)!.Detail;
        var tree=w.Trees.First(t=>habitat.Contains(t.Cell) && !t.Felled);
        w.SetClearing(tree.Cell,true); w.Tick(.1f);
        Check(before!=w.ReadResourceSurvey(woodland)!.Detail,"Tree loss did not change survey");
        var lake=World.NewLakeMap(); var dock=lake.Place(new(3,4),true,BuildingKind.FishingDock)!;
        Check(dock!=null,"Survey dock rejected");
        foreach(var source in lake.ResourceSources())
        {
            var fish=lake.ReadResourceSurvey(source.Key)!;
            Check(fish.Workplaces.Contains(dock!.Id) && fish.Detail.Contains("Recovery") && fish.WorkplaceHeading.Contains("WATER ROUTE"),"Connected planned dock missing");
        }
        saved=lake.SaveJson(); foreach(var s in lake.ResourceSources()) lake.ReadResourceSurvey(s.Key);
        Check(saved==lake.SaveJson(),"Fish survey mutated simulation");
        var barrier=World.NewLargeMap();
        for(int z=-16;z<=15;z++) barrier.Map.Water.Add(new(7,z));
        Check(barrier.ReadResourceSurvey(new(SourceKind.Stone,1))!.Detail.Contains("no route from the yard"),"Unreachable source claimed access");
        Console.WriteLine("PASS: source stock/claims, related workplaces, planned docks, removal, habitat change, inaccessible stone and read-only survey.");
    }
}

using Inlanders.Simulation;
using System.Text.Json;

static class InheritedShorelineChecks
{
    // Run after --inherited: repair the observed weak layout, preserving its failed snapshot.
    public static void RecoverLayout()
    {
        using var results=JsonDocument.Parse(File.ReadAllText("artifacts/inherited/results.json"));
        double previous=results.RootElement.EnumerateArray().Single(a=>a.GetProperty("arm").GetString()=="east-homes").GetProperty("hungry").GetDouble();
        var w=World.LoadFile("artifacts/inherited/east-homes.json");
        var plan=ReviewPlacement.Find(w,BuildingKind.VegetableGarden,new(0,6),(c,r)=>c.X<5,"home shore")??throw new Exception("No recovery site");
        var site=w.Place(plan.Actual,plan.Rotation,plan.Kind)!;
        for(int i=0;i<24000 && !site.Complete;i++)w.Tick(.1f);
        if(!site.Complete)throw new Exception("Recovery garden did not build");
        for(int i=0;i<3000;i++)w.Tick(.1f); // Allow the first new harvest and interrupted routines to settle.
        double hungry=0;
        for(int i=0;i<6000;i++){hungry+=w.People.Count(p=>!p.Fed)*.1;w.Tick(.1f);}
        w.Validate();w.SaveFile("artifacts/inherited/east-homes-recovered.json");
        File.WriteAllText("artifacts/inherited/recovery.json",JsonSerializer.Serialize(new{plan,hungry,food=w.EdibleStored},new JsonSerializerOptions{WriteIndented=true}));
        if(hungry>=previous)throw new Exception("New local garden did not improve the observed hungry layout");
        Console.WriteLine($"PASS: local garden repaired east-homes layout; final600 hunger {hungry:F1} person-s, food {w.EdibleStored}");
    }
    public static void Run()
    {
        const string folder="artifacts/inherited";Directory.CreateDirectory(folder);
        var baseline=World.NewInheritedShoreline().SaveJson();
        File.WriteAllText(folder+"/initial.json",baseline);
        var results=new List<object>();
        foreach(string arm in new[]{"keep-footway","bridge","local-food","east-homes","pause-recover"})
        {
            var w=World.LoadJson(baseline);int tick=0;var placements=new List<ReviewPlacement>();
            void Check(bool ok,string why){if(!ok)throw new Exception(arm+": "+why);}
            void Step(){w.Tick(.1f);if(++tick%100==0)w.Validate();}
            void Until(Func<bool> done,string why){float start=w.Food.Time;Console.WriteLine($"{arm} {start:F1}s: {why}");for(int i=0;i<24000 && !done();i++)Step();Check(done(),why);}
            Cottage Build(BuildingKind kind,Cell target,bool east=false)
            {
                var p=kind==BuildingKind.Bridge?new ReviewPlacement(kind,new(5,2),new(5,2),1,ReviewPlacementPolicy.Exact,"shortcut",0):
                    ReviewPlacement.Find(w,kind,target,(c,r)=>east?c.X>5:c.X<5,east?"garden shore":"home shore");
                Check(p!=null,"No placement "+kind);placements.Add(p!);
                var site=w.Place(p!.Actual,p.Rotation,kind);Check(site!=null,"Placement refused "+kind);
                Until(()=>site!.Complete,"build "+kind);return site!;
            }
            Check(w.IsInheritedShoreline && w.Population==8 && w.Cottages.Count==8 && w.NewNeighborsHoused==0,"initial identity");
            Check(w.InvitationProblem()==null,"footway must support invitation without bridge");
            Check(w.ReadEconomy().Issues.All(i=>i.Staff==null),"legacy staffing advice");
            if(arm=="bridge")Build(BuildingKind.Bridge,new(5,2));
            if(arm=="local-food")Build(BuildingKind.VegetableGarden,new(-1,-3));
            bool east=arm=="east-homes";
            foreach(var target in east?new[]{new Cell(8,3),new(10,6),new(16,2),new(16,7)}:
                new[]{new Cell(-5,-2),new(-2,-2),new(1,-2),new(1,6)})Build(BuildingKind.Cottage,target,east);
            var venue=w.Cottages.Single(c=>c.Kind==BuildingKind.SeatingGarden);
            Check(w.ChooseWelcomeVenue(venue.Id),"western inherited venue allowed");
            var sources=w.Cottages.Where(c=>w.IsWorkplaceFoodStore(c)).ToArray();
            if(arm=="pause-recover")foreach(var s in sources)Check(w.SetWorkplacePaused(s.Id,true),"pause source");
            Check(w.InviteNewcomers(),"invite");
            if(arm=="pause-recover")
            {
                for(int i=0;i<7000;i++)Step();
                Check(!w.Neighborhood!.Complete && w.ShorelineFoodProblem()!=null,"paused food falsely completed");
                foreach(var s in sources)Check(w.SetWorkplacePaused(s.Id,false),"resume source");
            }
            Until(()=>w.Neighborhood!.Complete,"finish welcome");
            float completed=w.Food.Time;string saved=w.SaveJson();
            var copy=World.LoadJson(saved);Check(copy.SaveJson()==saved,"roundtrip");
            for(int i=0;i<10;i++){w.Tick(.1f);copy.Tick(.1f);Check(w.SaveJson()==copy.SaveJson(),"saved continuation");}
            double hungry=0,foodWalking=0;
            for(int i=0;i<6000;i++)
            {
                hungry+=w.People.Count(p=>!p.Fed)*.1;
                foodWalking+=w.People.Count(p=>p.Route.Count>0 && p.Task is Work.ToPantry or Work.ToFoodPickup or Work.ToMealSupply or Work.ToMealSeat)*.1;
                Step();
            }
            w.SaveFile(folder+"/"+arm+".json");
            results.Add(new{arm,completed,foodWalking,hungry,food=w.EdibleStored,placements});
            File.WriteAllText(folder+"/results.json",JsonSerializer.Serialize(results,new JsonSerializerOptions{WriteIndented=true}));
            Console.WriteLine($"PASS {arm}: complete {completed:F1}s, next600 food travel {foodWalking:F1} person-s, hunger {hungry:F1} person-s");
        }
    }
}


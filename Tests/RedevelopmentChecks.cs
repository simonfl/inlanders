using Inlanders.Simulation;
using System.Text.Json;

// F31b2 comparison only: ordinary rules, legal commands, no new scenario or balance overrides.
static class RedevelopmentChecks
{
    const string Folder="artifacts/redevelopment";
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    static Cottage Build(World w,Cell target)
    {
        var p=ReviewPlacement.Find(w,BuildingKind.VegetableGarden,target,(c,r)=>c.X<5,"home-side replacement")
            ??throw new Exception("No replacement plot");
        return w.Place(p.Actual,p.Rotation,p.Kind)??throw new Exception("Placement rejected");
    }
    public static void Run()
    {
        Directory.CreateDirectory(Folder);
        var source=World.NewInheritedShoreline();
        foreach(var at in new[]{new Cell(-5,-2),new(-2,-2),new(1,-2),new(1,6)})
        {
            var p=ReviewPlacement.Find(source,BuildingKind.Cottage,at,(c,r)=>c.X<5,"existing home shore")!;
            Check(source.Place(p.Actual,p.Rotation,p.Kind)!=null,"Baseline home rejected");
        }
        for(int i=0;i<24000 && source.Cottages.Any(c=>!c.Complete);i++)source.Tick(.1f);
        Check(source.Cottages.Where(c=>c.Kind==BuildingKind.Cottage).All(c=>c.Complete),"Baseline homes unfinished");
        Check(source.InviteNewcomers(),"Baseline invitation rejected");
        for(int i=0;i<9000 && source.Population<16;i++)source.Tick(.1f);
        for(int i=0;i<1800;i++)source.Tick(.1f);
        Check(source.Population==16 && source.Housed==16 && !source.Creative,"Baseline identity");
        source.Validate();source.SaveFile(Folder+"/initial.json");
        var rows=new List<object>();
        foreach(string arm in new[]{"unchanged","retain-until-built","replace-one-at-a-time","demolish-first","bulk-orders"})
        {
            var w=World.LoadJson(source.SaveJson());var old=w.Cottages.Where(c=>c.Kind==BuildingKind.VegetableGarden).Select(c=>c.Id).ToArray();
            var replacements=new List<Cottage>();var events=new List<object>();int next=0;bool removed=false;float? finished=null;
            double unfed=0,lateUnfed=0;int peakUnfed=0;int meals=w.Food.EatenBerries+w.Food.EatenVegetables;
            var targets=new[]{new Cell(-5,-5),new(-1,-5),new(1,6)};
            void Event(string action)=>events.Add(new{seconds=w.Food.Time-source.Food.Time,action});
            void Add(){var c=Build(w,targets[next++]);replacements.Add(c);Event($"Order garden {c.Id} at {c.Cell}");}
            void Remove(int id){Check(w.RequestDemolition(id),"Demolition rejected");Event($"Order demolition {id}");}
            if(arm is "retain-until-built" or "bulk-orders")while(next<3)Add();
            if(arm=="replace-one-at-a-time")Add();
            if(arm is "demolish-first" or "bulk-orders"){foreach(int id in old)Remove(id);removed=true;}
            Check(w.RelocationProblem(old[0])!=null,"Free relocation bypasses normal construction");
            w.SaveFile(Folder+"/"+arm+"-start.json");
            for(int i=0;i<9000;i++)
            {
                if(arm=="retain-until-built" && !removed && replacements.All(c=>c.Complete))
                {foreach(int id in old)Remove(id);removed=true;}
                if(arm=="replace-one-at-a-time" && replacements.Count>0 && replacements[^1].Complete)
                {
                    int id=old[next-1];var previous=w.Cottages.FirstOrDefault(c=>c.Id==id);
                    if(previous!=null && !previous.DemolitionRequested)Remove(id);
                    if(previous==null && next<3)Add();
                }
                if(arm=="demolish-first" && next==0 && old.All(id=>w.Cottages.All(c=>c.Id!=id)))while(next<3)Add();
                if(arm!="unchanged" && finished==null && next==3 && replacements.All(c=>c.Complete) && old.All(id=>w.Cottages.All(c=>c.Id!=id)))
                {finished=w.Food.Time-source.Food.Time;Event("Redevelopment complete");}
                w.Tick(.1f);int hungry=w.People.Count(p=>!p.Fed);unfed+=hungry*.1;peakUnfed=Math.Max(peakUnfed,hungry);if(i>=6000)lateUnfed+=hungry*.1;
                if(i%100==0)w.Validate();
                if(i==1199)w.SaveFile(Folder+"/"+arm+"-early.json");
            }
            w.Validate();string json=w.SaveJson();Check(World.LoadJson(json).SaveJson()==json,"Current save mismatch");
            w.SaveFile(Folder+"/"+arm+"-late.json");
            rows.Add(new{arm,finished,unfed,lateUnfed,peakUnfed,totalMeals=w.Food.EatenBerries+w.Food.EatenVegetables-meals,food=w.EdibleStored,events});
            File.WriteAllText(Folder+"/report.json",JsonSerializer.Serialize(new{start=source.Food.Time,duration=900,rows},new JsonSerializerOptions{WriteIndented=true}));
            Console.WriteLine($"{arm}: finished {finished}, unfed {unfed:F1}, late unfed {lateUnfed:F1}, peak {peakUnfed}, food {w.EdibleStored}");
        }
        Recovery();
        Console.WriteLine("PASS: ordinary construction/demolition, fixed-age comparisons, resource validation and current saves; design verdict separate");
    }
    public static void Recovery()
    {
        var control=World.LoadFile(Folder+"/demolish-first-late.json");
        var repaired=World.LoadJson(control.SaveJson());
        var plan=ReviewPlacement.Find(repaired,BuildingKind.VegetableGarden,new(10,2),(c,r)=>c.X>5,"restore an eastern producer")!;
        var garden=repaired.Place(plan.Actual,plan.Rotation,plan.Kind)??throw new Exception("Recovery placement rejected");
        double controlUnfed=0,repairedUnfed=0;
        for(int i=0;i<6000;i++)
        {
            control.Tick(.1f);repaired.Tick(.1f);
            if(i>=3000){controlUnfed+=control.People.Count(p=>!p.Fed)*.1;repairedUnfed+=repaired.People.Count(p=>!p.Fed)*.1;}
            if(i%100==0){control.Validate();repaired.Validate();}
        }
        Check(garden.Complete,"Recovery construction stalled");
        control.Validate();repaired.Validate();
        control.SaveFile(Folder+"/recovery-control.json");repaired.SaveFile(Folder+"/recovered.json");
        var result=new{duration=600,measuredFinalSeconds=300,controlUnfed,repairedUnfed,plan,recoveryBuilt=garden.Complete};
        File.WriteAllText(Folder+"/recovery.json",JsonSerializer.Serialize(result,new JsonSerializerOptions{WriteIndented=true}));
        Console.WriteLine("RECOVERY: "+JsonSerializer.Serialize(result));
    }

}

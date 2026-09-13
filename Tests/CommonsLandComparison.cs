using Inlanders.Simulation;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Security.Cryptography;

// Geometry/economy prototype, not a shipped commons system or a human playtest.
static class CommonsLandComparison
{
    static readonly JsonSerializerOptions Json=new(){WriteIndented=true,Converters={new JsonStringEnumConverter()}};
    static readonly string Folder="artifacts/commons-land-comparison";
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    static int validationTicks;
    static void Step(World w,int ticks=1){for(int i=0;i<ticks;i++){w.Tick(.1f);if(++validationTicks%100==0)w.Validate();}}
    static void Until(World w,Func<bool> done,string why)
    {for(int i=0;i<24000 && !done();i++)Step(w);Check(done(),why+" at "+w.Food.Time);w.Validate();}
    static bool Near(Cell c)=>c.X>=16 && c.X<=20 && c.Z>=4 && c.Z<=8;
    static bool Far(Cell c)=>c.X>=20 && c.X<=24 && c.Z>=8 && c.Z<=12;
    static Cottage Build(World w,BuildingKind kind,Cell requested,List<ReviewPlacement> placements,Func<Cell,int,bool>? region=null,string regionName="east bank")
    {
        ReviewPlacement? plan=null;
        Until(w,()=> (plan=ReviewPlacement.Find(w,kind,requested,region??((c,r)=>c.X>5),regionName))!=null,"No legal placement for "+kind);
        placements.Add(plan!);var site=w.Place(plan!.Actual,plan.Rotation,kind)!;
        Until(w,()=>site.Complete,"Construction "+kind);Console.WriteLine($"Built {kind} at {plan.Actual} facing {plan.Rotation}, time {w.Food.Time:F1}");return site;
    }
    static World Prepare(List<ReviewPlacement> placements)
    {
        var w=World.NewFoodLandChallenge();
        var bridge=w.Place(new(5,2),1,BuildingKind.Bridge)!;Until(w,()=>bridge.Complete,"Crossing");
        for(int i=0;i<3;i++)Build(w,BuildingKind.VegetableGarden,new(17+i*2,6),placements);
        var pantry=Build(w,BuildingKind.Pantry,new(14,5),placements);w.SetPantryTarget(pantry.Id,16);
        foreach(var c in new[]{new Cell(8,2),new(10,4),new(17,1),new(21,1)})Build(w,BuildingKind.Cottage,c,placements);
        var welcome=Build(w,BuildingKind.SeatingGarden,new(18,5),placements);
        Check(w.ChooseWelcomeVenue(welcome.Id) && w.InviteNewcomers(),"Welcome refused");
        Until(w,()=>w.Neighborhood!.Complete && w.Food.Hunger==0,"Competent first act");
        return w;
    }
    static void Save(World w,string name)
    {
        w.Validate();string state=w.SaveJson();var copy=World.LoadJson(state);Check(copy.SaveJson()==state,"Save differs: "+name);
        var original=World.LoadJson(state);
        for(int i=0;i<100;i++){original.Tick(.1f);copy.Tick(.1f);Check(original.SaveJson()==copy.SaveJson(),"Continuation differs: "+name);}
        w.SaveFile(Folder+"/"+name+".json");
    }
    public static void Run()
    {
        Directory.CreateDirectory(Folder);var started=System.Diagnostics.Stopwatch.StartNew();
        var sources=Directory.GetFiles("Simulation","*.cs").Concat(new[]{"Tests/CommonsLandComparison.cs","Tests/ReviewPlacement.cs"})
            .ToDictionary(p=>p,p=>Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(p))));
        void Write(string name,object value)=>File.WriteAllText(Folder+"/"+name+".json",JsonSerializer.Serialize(value,Json));
        Write("manifest",new{status="running",sources});Write("results",Array.Empty<object>());
        var initialPlacements=new List<ReviewPlacement>();var first=Prepare(initialPlacements);Save(first,"first-act");
        string baseline=first.SaveJson();string baselineHash=Convert.ToHexString(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(baseline)));
        Write("first-act-placements",initialPlacements);
        var results=new List<object>();
        foreach(string arm in new[]{"stay-stable","near-replace-production","far-preserve-production","near-small-garden"})
        {
            var w=World.LoadJson(baseline);float start=w.Food.Time;var placements=new List<ReviewPlacement>();var removed=new List<object>();
            int? venue=null;
            if(arm=="near-replace-production")
            {
                var displaced=w.Cottages.Where(c=>World.Footprint(c.Cell,c.Rotation,c.Kind).Any(Near)).ToArray();
                int gardens=displaced.Count(c=>c.Kind==BuildingKind.VegetableGarden);
                // An informed player replaces food before clearing the productive core.
                for(int i=0;i<gardens;i++)Build(w,BuildingKind.VegetableGarden,new(22,9+i*2),placements,
                    (c,r)=>c.X>5 && World.Footprint(c,r,BuildingKind.VegetableGarden).Append(World.Door(c,r)).All(x=>!Near(x)),"outside near commons");
                foreach(var site in displaced){removed.Add(new{site.Id,site.Kind,site.Cell});Check(w.RequestDemolition(site.Id),"Cannot remove "+site.Id);}
                Until(w,()=>displaced.All(c=>w.Cottages.All(s=>s.Id!=c.Id)),"Site recovery");
                foreach(var tree in w.Trees.Where(t=>Near(t.Cell)).ToArray())Check(w.SetClearing(tree.Cell,true),"Cannot clear "+tree.Cell);
                Until(w,()=>!w.Trees.Any(t=>Near(t.Cell)),"Ground clearing");
                venue=Build(w,BuildingKind.Square,new(18,6),placements,
                    (c,r)=>World.Footprint(c,r,BuildingKind.Square).Append(World.Door(c,r)).All(Near),"near commons").Id;
            }
            if(arm=="far-preserve-production")venue=Build(w,BuildingKind.Square,new(22,11),placements,
                (c,r)=>World.Footprint(c,r,BuildingKind.Square).Append(World.Door(c,r)).All(Far),"far commons").Id;
            if(arm=="near-small-garden")venue=Build(w,BuildingKind.SeatingGarden,new(18,6),placements,
                (c,r)=>c.X>5 && (c.Point-new Cell(18,6).Point).LengthSquared()<=25,"within five tiles of near site").Id;
            float prepared=w.Food.Time;Save(w,arm+"-prepared");
            int[] visits=w.People.Select(p=>p.LeisureVisits).ToArray();int edibleStart=w.EdibleStored;
            double hungryPersonSeconds=0,foodTravel=0,leisureTravel=0;int newVenueVisits=0;var seen=new HashSet<int>();
            // Equal post-construction horizon. Setup cost is reported separately, not mixed into the comparison.
            for(int tick=0;tick<12000;tick++)
            {
                foreach(var p in w.People)
                {
                    if(!p.Fed)hungryPersonSeconds+=.1;
                    if(p.Route.Count>0 && p.Task is Work.ToPantry or Work.ToFoodPickup or Work.ToMealSupply or Work.ToMealSeat or Work.ToFarm)foodTravel+=.1;
                    if(p.Task==Work.ToLeisure && p.Route.Count>0)leisureTravel+=.1;
                }
                Step(w);
                foreach(var p in w.People)if(p.LeisureVisits>visits[p.Id])
                {if(p.LastLeisureSiteId==venue){newVenueVisits+=p.LeisureVisits-visits[p.Id];seen.Add(p.Id);}visits[p.Id]=p.LeisureVisits;}
            }
            Save(w,arm+"-final");
            results.Add(new{arm,start,prepared,setupSeconds=prepared-start,postSetupSeconds=1200,removed,placements,
                edibleStart,edibleEnd=w.EdibleStored,hungryPersonSeconds,foodTravelPersonSeconds=foodTravel,leisureTravelPersonSeconds=leisureTravel,
                newVenueVisits,distinctNewVenueVisitors=seen.Count,meal=w.ReadMealAssessment()});
            Write("results",results);Console.WriteLine($"{arm}: setup {prepared-start:F1}s, food {edibleStart}->{w.EdibleStored}, hungry {hungryPersonSeconds:F1} person-s, new venue {newVenueVisits} visits/{seen.Count} people");
        }
        Write("manifest",new{status="complete",sources,baselineHash,elapsedSeconds=started.Elapsed.TotalSeconds,arms=results.Count});
    }
}

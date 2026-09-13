using Inlanders.Simulation;
using System.Text.Json;
using System.Security.Cryptography;

// Factorial design probe: identical starts, two home layouts, two supply chains,
// and early/late invitation. These are scripted plans, not preference evidence.
static class MeadowDecisionComparison
{
    public static void Run()
    {
        const string folder="artifacts/meadow-decision";
        Directory.CreateDirectory(folder);
        var json=new JsonSerializerOptions{WriteIndented=true};
        var initial=World.NewFoodLandChallenge().SaveJson();
        var results=new List<object>();
        File.WriteAllText(folder+"/initial.json",initial);
        var sources=Directory.GetFiles("Simulation","*.cs")
            .Concat(new[]{"Tests/MeadowDecisionComparison.cs","Tests/ReviewPlacement.cs"})
            .ToDictionary(p=>p,p=>Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(p))));
        void Write(string name,object value)=>File.WriteAllText(folder+"/"+name+".json",JsonSerializer.Serialize(value,json));
        Write("manifest",new{status="running",sources,initialHash=Convert.ToHexString(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(initial)))});
        foreach(bool bread in new[]{false,true})foreach(bool meadowHomes in new[]{false,true})foreach(bool early in new[]{false,true})
        {
            string arm=$"{(bread?"bread":"gardens")}-{(meadowHomes?"meadow":"split")}-{(early?"early":"late")}";
            var w=World.LoadJson(initial);var placements=new List<ReviewPlacement>();
            float? completed=null,welcomeReady=null;double hungry=0,travel=0;int ticks=0;
            void Tick()
            {
                w.Tick(.1f);ticks++;
                hungry+=w.People.Count(p=>!p.Fed)*.1;
                travel+=w.People.Count(p=>p.Route.Count>0 && p.Task is Work.ToFoodPickup or Work.ToPantry or Work.ToMealSupply or Work.ToMealSeat)*.1;
                if(w.Neighborhood!.Arrived && w.Neighborhood.Welcomed.Count==w.Population && w.NewNeighborsHoused==8)welcomeReady??=w.Food.Time;
                if(w.Neighborhood.Complete)completed??=w.Food.Time;
                if(ticks%100==0)w.Validate();
            }
            void Until(Func<bool> done,string why)
            {for(int i=0;i<24000 && !done();i++)Tick();if(!done())throw new Exception(arm+": "+why);}
            Cottage Build(BuildingKind kind,Cell target)
            {
                var choice=ReviewPlacement.Find(w,kind,target,(c,r)=>c.X>5,"east bank")??throw new Exception(arm+": no site for "+kind);
                placements.Add(choice);var site=w.Place(choice.Actual,choice.Rotation,kind)!;
                Until(()=>site.Complete,"construction "+kind);return site;
            }
            var bridge=w.Place(new(5,2),1,BuildingKind.Bridge)!;Until(()=>bridge.Complete,"crossing");
            if(early && !w.InviteNewcomers())throw new Exception("Invitation refused");
            if(bread)
            {
                Build(BuildingKind.Farm,new(17,3));Build(BuildingKind.Farm,new(21,3));
                Build(BuildingKind.Bakery,new(17,7));Build(BuildingKind.Bakery,new(21,7));
            }
            else for(int i=0;i<3;i++)Build(BuildingKind.VegetableGarden,new(17+i*2,6));
            var pantry=Build(BuildingKind.Pantry,new(14,5));w.SetPantryTarget(pantry.Id,16);
            var homes=meadowHomes?new[]{new Cell(16,0),new(20,0),new(20,10),new(23,6)}:
                new[]{new Cell(8,2),new(10,4),new(17,1),new(21,1)};
            foreach(var c in homes)Build(BuildingKind.Cottage,c);
            var venue=Build(BuildingKind.SeatingGarden,new(18,5));
            if(!w.ChooseWelcomeVenue(venue.Id) || !early && !w.InviteNewcomers())throw new Exception("Welcome refused");
            Until(()=>w.Neighborhood!.Complete,"completion");
            double hungryAtCompletion=hungry,travelAtCompletion=travel;
            int foodAtCompletion=w.EdibleStored;
            for(int i=0;i<6000;i++)Tick();
            w.Validate();
            string state=w.SaveJson();if(World.LoadJson(state).SaveJson()!=state)throw new Exception("Save differs");
            File.WriteAllText(folder+"/"+arm+".json",state);
            results.Add(new{arm,completed,welcomeReady,reserveWait=completed-welcomeReady,
                committedAt=w.Neighborhood!.CommittedAt,hungryAtCompletion,travelAtCompletion,foodAtCompletion,
                final600FedFraction=1-(hungry-hungryAtCompletion)/(600*w.Population),foodEnd=w.EdibleStored,
                buildCostLogs=placements.Sum(p=>Buildings.Get(p.Kind).Cost)+Buildings.Get(BuildingKind.Bridge).Cost,placements});
            Write("results",results);
            Console.WriteLine($"{arm}: complete {completed:F1}, reserve wait {completed-welcomeReady:F1}, hunger {hungryAtCompletion:F1} person-s, final fed {1-(hungry-hungryAtCompletion)/(600*w.Population):P1}");
        }
        Write("manifest",new{status="complete",sources,arms=results.Count});
    }
}

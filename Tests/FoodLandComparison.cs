using Inlanders.Simulation;
using System.Text.Json;
using System.Security.Cryptography;

static class FoodLandComparison
{
    public static void Run()
    {
        const string folder="artifacts/food-land-reserve-comparison";Directory.CreateDirectory(folder);
        var json=new JsonSerializerOptions{WriteIndented=true};var results=new List<object>();
        var sources=Directory.GetFiles("Simulation","*.cs").Append("Tests/FoodLandComparison.cs")
            .ToDictionary(p=>p,p=>Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(p))));
        File.WriteAllText(folder+"/manifest.json",JsonSerializer.Serialize(new{status="running",sources},json));
        foreach(string plan in new[]{"wild-only","gardens-prepared","bread-prepared","gardens-early"})
        {
            var w=World.NewFoodLandChallenge();
            float? completed=null;double hunger=0,last300Hungry=0;int ticks=0;
            void Tick(){w.Tick(.1f);ticks++;if(ticks%100==0)w.Validate();if(w.Neighborhood!.Complete)completed??=w.Food.Time;if(w.Food.Hunger>0)hunger+=.1;}
            void Until(Func<bool> done,string why){for(int i=0;i<24000 && !done();i++)Tick();if(!done())throw new Exception(plan+": "+why);}
            Cottage Build(BuildingKind kind,Cell center)
            {
                var choice=w.Map.Land.SelectMany(cell=>Enumerable.Range(0,4).Select(rotation=>new{cell,rotation}))
                    .Where(p=>p.cell.X>5 && w.PlacementProblem(p.cell,p.rotation,kind)==null)
                    .OrderBy(p=>(p.cell.Point-center.Point).LengthSquared()).ThenBy(p=>p.cell.Z).ThenBy(p=>p.cell.X).ThenBy(p=>p.rotation).First();
                var site=w.Place(choice.cell,choice.rotation,kind)!;Until(()=>site.Complete,"construction "+kind);return site;
            }
            var bridge=w.Place(new(5,2),1,BuildingKind.Bridge)!;Until(()=>bridge.Complete,"crossing");
            if(plan.EndsWith("early") && !w.InviteNewcomers())throw new Exception("Early commitment refused");
            if(plan.StartsWith("gardens"))for(int i=0;i<3;i++)Build(BuildingKind.VegetableGarden,new(17+i*2,6));
            if(plan.StartsWith("bread"))
            {
                Build(BuildingKind.Farm,new(17,3));Build(BuildingKind.Farm,new(21,3));
                Build(BuildingKind.Bakery,new(17,7));Build(BuildingKind.Bakery,new(21,7));
            }
            if(plan!="wild-only"){var pantry=Build(BuildingKind.Pantry,new(14,5));w.SetPantryTarget(pantry.Id,16);}
            foreach(var cell in new[]{new Cell(8,2),new(10,4),new(17,1),new(21,1)})Build(BuildingKind.Cottage,cell);
            var venue=Build(BuildingKind.SeatingGarden,new(18,5));
            if(!w.ChooseWelcomeVenue(venue.Id))throw new Exception("Venue refused");
            if(w.Neighborhood!.CommittedAt==null && !w.InviteNewcomers())throw new Exception("Prepared commitment refused");
            for(int i=0;i<24000;i++){Tick();if(i>=21000)last300Hungry+=w.People.Count(p=>!p.Fed)*.1;}
            w.Validate();NeighborhoodComparison.SaveChecked(w,$"{folder}/{plan}.json");
            results.Add(new{plan,completed,finalTime=w.Food.Time,hungerSeconds=hunger,last300FedFraction=1-last300Hungry/(300*w.Population),
                food=w.EdibleStored,w.Food.GrownVegetables,w.Food.BakedBread,w.Neighborhood.Welcomed,
                buildings=w.Cottages.Select(c=>new{c.Kind,c.Cell,c.Rotation})});
            File.WriteAllText(folder+"/results.json",JsonSerializer.Serialize(results,json));
            Console.WriteLine($"{plan}: completion {completed}, final300 fed {1-last300Hungry/(300*w.Population):P1}, food {w.EdibleStored}");
        }
        File.WriteAllText(folder+"/manifest.json",JsonSerializer.Serialize(new{status="complete",sources,arms=results.Count},json));
    }
}

using Inlanders.Simulation;

public static class PantryLayoutComparison
{
    public static void Run()
    {
        foreach(bool dispersed in new[]{false,true})
        foreach(string service in new[]{"central","local","awkward","hauler"})
        {
            var w=World.NewLargeMap(false,false);
            w.Food.InitialBerries=w.Food.Berries=96;
            foreach(var p in w.People) w.Assign(p.Id,p.Id<2?Role.Logger:p.Id<4?Role.Builder:Role.Farmer);
            var centers=dispersed?new[]{new Cell(-10,-4),new Cell(9,5)}:new[]{new Cell(1,0),new Cell(1,6)};
            Cottage PlaceNear(Cell target,BuildingKind kind)
            {
                var cell=w.Map.Land.Where(c=>w.PlacementProblem(c,false,kind)==null)
                    .OrderBy(c=>(c.Point-target.Point).LengthSquared()).ThenBy(c=>c.X).ThenBy(c=>c.Z).First();
                return w.Place(cell,false,kind)!;
            }
            foreach(var center in centers)
            {
                PlaceNear(center,BuildingKind.VegetableGarden);
                PlaceNear(new(center.X,center.Z+3),BuildingKind.VegetableGarden);
                PlaceNear(new(center.X+3,center.Z),BuildingKind.Cottage);
                PlaceNear(new(center.X+3,center.Z+3),BuildingKind.Cottage);
                PlaceNear(new(center.X-1,center.Z+1),BuildingKind.SeatingGarden);
            }
            var pantries=new List<Cottage>();
            if(service!="central")
                for(int i=0;i<centers.Length;i++)
                {
                    var target=service=="awkward"?(i==0?new Cell(-13,5):new Cell(12,-11)):new Cell(centers[i].X-2,centers[i].Z+2);
                    var pantry=PlaceNear(target,BuildingKind.Pantry);pantries.Add(pantry);
                    w.SetPantryTarget(pantry.Id,service=="hauler"?12:0);
                }
            float mealTravel=0,supplier=0,producerTravel=0,setup=0,eating=0,homeTravel=0,recreationTravel=0,hunger=0;
            float? built=null;bool staffed=false;int timely=0,missed=0,skipped=0;
            var observed=new HashSet<int>();
            for(int i=0;i<12000;i++)
            {
                if(built==null && w.Cottages.All(c=>c.Complete)) built=w.Food.Time;
                if(service=="hauler" && built!=null && !staffed) {w.Assign(3,Role.Hauler);staffed=true;}
                foreach(var p in w.People)
                {
                    bool personal=p.Task is Work.ToMealSupply or Work.ToMealSeat or Work.EatingMeal or Work.ReturnMeal or Work.ToRest or Work.Resting or Work.ToLeisure or Work.Leisure;
                    if(p.Task is Work.ToMealSupply or Work.ToMealSeat or Work.ReturnMeal) mealTravel+=.1f;
                    if(p.Task==Work.EatingMeal) eating+=.1f;
                    if(p.FoodTransfer) supplier+=.1f;
                    if(p.Task==Work.ToPantry && !p.FoodTransfer) producerTravel+=.1f;
                    if(p.Role is Role.Logger or Role.Builder && p.Task!=Work.Waiting && !personal) setup+=.1f;
                    if(p.Task==Work.ToRest) homeTravel+=.1f;
                    if(p.Task==Work.ToLeisure) recreationTravel+=.1f;
                }
                hunger+=w.Food.Hunger*.1f;w.Tick(.1f);w.Validate();
                foreach(var outcome in w.Food.MealOutcomes.Where(m=>observed.Add(m.Request)))
                {if(outcome.Skipped) skipped++;else if(outcome.Timely) timely++;else missed++;}
            }
            var result=new {Layout=dispersed?"dispersed":"compact",Service=service,Seconds=w.Food.Time,BuildComplete=built,
                ConstructionLogs=w.Cottages.Sum(c=>c.Delivered),TimberAndConstructionWorkerSeconds=setup,MealTravel=mealTravel,Eating=eating,
                SupplierWorkerSeconds=supplier,ProducerDeliveryTravel=producerTravel,HomeTravel=homeTravel,RecreationTravel=recreationTravel,
                RestVisits=w.People.Sum(p=>p.RestVisits),RecreationVisits=w.People.Sum(p=>p.LeisureVisits),HungerIntegral=hunger,
                Timely=timely,Missed=missed,Skipped=skipped,Produced=w.Food.GrownVegetables,Delivered=w.DeliveredVegetables,
                CentralFood=World.EdibleKinds.Sum(w.CentralFood),LocalFood=pantries.Select(p=>new {p.Id,p.Cell,Stock=p.PantryFood.Sum()}).ToArray(),
                FinalService=w.ReadMealAssessment()};
            string json=System.Text.Json.JsonSerializer.Serialize(result);
            Console.WriteLine(json);
            Directory.CreateDirectory("artifacts");File.WriteAllText($"artifacts/pantry-layout-{result.Layout}-{service}.json",json);
            File.WriteAllText($"artifacts/pantry-layout-{result.Layout}-{service}-world.json",w.SaveJson());
        }
    }
}

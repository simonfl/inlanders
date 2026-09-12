using Inlanders.Simulation;

public static class PantryProducerChecks
{
    static void Check(bool ok,string why) {if(!ok) throw new Exception(why);}
    static void Step(World w) {w.Tick(.1f);w.Validate();}
    static void Until(World w,Func<bool> done,string why)
    {for(int i=0;i<8000 && !done();i++) Step(w);Check(done(),why);}
    static Cottage Ready(World w,Cell cell,BuildingKind kind,bool rotated=false)
    {
        var site=w.Place(cell,rotated,kind) ?? throw new Exception($"Producer fixture placement {kind} {cell}");
        // Construction is covered by PantryChecks; this fixture isolates actual producer output and routing.
        int needed=site.Required;
        foreach(var tree in w.Trees.Where(t=>t.Material==Resource.Logs))
        {
            int take=Math.Min(needed,tree.Logs);tree.Logs-=take;needed-=take;
            if(tree.Logs==0) tree.Felled=true;
            if(needed==0) break;
        }
        Check(needed==0,"Not enough fixture timber");
        site.Delivered=site.Required;site.Construction=1;return site;
    }
    public static void Run()
    {
        foreach(var kind in World.EdibleKinds)
        {
            var w=kind==Resource.Fish?World.NewLakeMap():new World(40);
            foreach(var p in w.People) w.Assign(p.Id,Role.Unassigned);
            w.Food.InitialBerries=w.Food.Berries=1000;
            Role role;Cell origin;
            switch(kind)
            {
                case Resource.Berries:
                    Ready(w,new(3,0),BuildingKind.ForagerHut);role=Role.Forager;
                    var bush=w.Bushes.OrderByDescending(b=>(b.Access.Point-w.YardAccess.Point).LengthSquared()).First();
                    w.Bushes.Clear();w.Bushes.Add(bush);origin=bush.Access;break;
                case Resource.Vegetables:
                    origin=Ready(w,new(3,-3),BuildingKind.VegetableGarden).Entrance;role=Role.Farmer;break;
                case Resource.Fruit:
                    origin=Ready(w,new(3,-3),BuildingKind.Orchard).Entrance;role=Role.Farmer;break;
                case Resource.Bread:
                    origin=Ready(w,new(6,-3),BuildingKind.Bakery).Entrance;role=Role.Baker;
                    w.Food.Grain=w.Food.GrownGrain=20;break;
                case Resource.Fish:
                    origin=Ready(w,new(14,0),BuildingKind.FishingDock,true).Entrance;role=Role.Fisher;break;
                default:
                    w.Map.Wildlife.Add(new WoodlandHabitat {Id=0,Cell=new(-3,-3),Stock=10});
                    Ready(w,new(3,0),BuildingKind.HuntingLodge);origin=new(-3,-3);role=Role.Hunter;break;
            }
            var location=w.Map.Land.Where(c=>w.PlacementProblem(c,false,BuildingKind.Pantry)==null)
                .OrderBy(c=>(World.Door(c,false).Point-origin.Point).LengthSquared()).First();
            var pantry=Ready(w,location,BuildingKind.Pantry);w.SetPantryTarget(pantry.Id,0);
            w.Assign(0,role);
            Until(w,()=>w.People[0].Task==Work.ToPantry && w.People[0].Cargo==kind && w.People[0].FoodDestinationId==pantry.Id,$"No direct {kind} delivery to local pantry at {location}");
            Check(!w.People[0].FoodTransfer && w.ReadFoodFlow().Delivered==0,$"{kind} first trip counted before delivery");
            string saved=w.SaveJson();var closing=World.LoadJson(saved);
            Check(closing.SaveJson()==saved,"Producer shipment save changed");
            Check(closing.RequestDemolition(pantry.Id),"Incoming producer pantry could not close");
            Check(closing.People[0].Carried>0 && closing.People[0].FoodDestinationId==null,"Closing pantry lost producer cargo or kept destination");
            Until(closing,()=>closing.ReadFoodFlow().Delivered>0,"Redirected first delivery was not credited");
            Until(w,()=>w.FoodAt(pantry.Id,kind)>0,$"{kind} not deposited locally");
            Check(w.ReadFoodFlow().Delivered>0 && !w.People.Any(p=>p.Role==Role.Hauler),"Direct supply needed a hauler or failed fresh-delivery credit");
            Check(World.LoadJson(w.SaveJson()).SaveJson()==w.SaveJson(),"Deposited pantry save changed");
            if(kind==Resource.Fruit){System.IO.Directory.CreateDirectory("artifacts/orchard-playable");w.SaveFile("artifacts/orchard-playable/pantry-fruit.json");}
            Console.WriteLine($"PASS: actual {kind} production, direct local deposit without hauler, in-flight save and closure redirection.");
        }
    }
}

using Inlanders.Simulation;

// Authoring comparisons, deliberately separate from pass/fail campaign checks.
public static class LakePressureExperiments
{
    public static void Run()
    {
        var draft=World.NewCampaign(7);
        Console.WriteLine($"NARROW LAKE: {draft.Map.Land.Count()} land tiles, {draft.Trees.Count} trees, {draft.Trees.Count(t=>t.Cell.X<3)} near-shore trees.");
        foreach(var kind in new[]{BuildingKind.Cottage,BuildingKind.VegetableGarden})
        {
            var plots=draft.Map.Land.Where(c=>draft.PlacementProblem(c,false,kind)==null).ToArray();
            Console.WriteLine($"NARROW LAKE {kind}: {plots.Count(c=>c.X<3)} legal near-shore anchor positions; examples {string.Join("; ",plots.Where(c=>c.X<3).Take(8))}.");
        }
        NarrowRoute(false); NarrowRoute(true); NarrowRoute(false,true); NarrowRoute(false,false,true);
        foreach(bool limited in new[]{false,true})
        foreach(string plan in new[]{"fish only","fish + garden","fish + bread"})
        {
            var initial=System.Text.Json.Nodes.JsonNode.Parse(World.NewLakeMap().SaveJson())!;
            initial["Campaign"]=System.Text.Json.JsonSerializer.SerializeToNode(new CampaignState { Level=7,Lake=new() });
            var w=World.LoadJson(initial.ToJsonString());
            if(limited)
            {
                w.Bushes.RemoveAll(b=>b.Id!=0);
                w.Map.FishingGrounds=w.Map.FishingGrounds.Select(g=>new FishHabitat
                {
                    Id=g.Id,Name=g.Name,Cell=g.Cell,Capacity=8,Stock=8,RegrowthPerSecond=1f/30
                }).ToList();
            }
            Cottage Build(Cell c,BuildingKind kind,bool rotated=false) => w.Place(c,rotated,kind) ?? throw new Exception(w.PlacementProblem(c,rotated,kind));
            bool Until(Func<bool> done,float deadline)
            {
                while(!done() && w.Food.Time<deadline) { w.Tick(.1f); if((int)(w.Food.Time*10)%20==0) w.Validate(); }
                return done();
            }
            Build(new(3,4),BuildingKind.FishingDock,true); w.Assign(6,Role.Fisher);
            if(plan!="fish only")
            {
                Build(new(0,-3),plan=="fish + garden"?BuildingKind.VegetableGarden:BuildingKind.Bakery);
                w.Assign(7,Role.Farmer);
                if(plan=="fish + bread") { Build(new(-3,-3),BuildingKind.Farm); w.Assign(1,Role.Baker); }
            }
            if(!Until(()=>w.DeliveredFish>=4,500)) throw new Exception("Opening fish delivery stalled");
            w.AdvanceLakePhase();
            Build(new(-6,-6),BuildingKind.Cottage); Build(new(-3,-7),BuildingKind.Cottage); Build(new(-3,6),BuildingKind.Square);
            if(!Until(()=>w.Beds>=12 && w.InvitationProblem()==null,700)) throw new Exception("Initial expansion stalled");
            w.InviteNewcomers(); w.Assign(8,Role.Logger); w.Assign(9,Role.Builder);
            if(Until(()=>w.InvitationProblem()==null,750)) w.InviteNewcomers();
            bool assessed=Until(()=>w.LakeActionProblem()==null,850) && w.AdvanceLakePhase();
            Until(()=>w.Campaign!.Complete,1200);
            float finished=w.Campaign!.Complete?w.Food.Time:-1;
            int startFood=w.Food.EdibleStored,full=0,meals=0; float previous=w.Food.MealClock;
            // Keep the chosen staffing unchanged beyond an opening stock burst.
            for(int i=0;i<9000;i++)
            {
                w.Tick(.1f); if(i%20==0) w.Validate();
                if(w.Food.MealClock<previous) { meals++; if(w.Food.LastMealServed==w.Population) full++; }
                previous=w.Food.MealClock;
            }
            Console.WriteLine($"LAKE PRESSURE {(limited?"one bush / shallow stocks":"original broad map")} / {plan}: finish {finished:0}s; assessed {assessed}; next 15m {full}/{meals} full meals, food {startFood}->{w.Food.EdibleStored}, fish {w.DeliveredFish}, vegetables {w.DeliveredVegetables}, bread {w.DeliveredBread}. {(!w.Campaign!.Complete?w.Campaign.Lake!.LastResult:"")}");
        }
    }
    static void NarrowRoute(bool bread,bool centralSquare=false,bool recover=false)
    {
        var w=World.NewCampaign(7);
        var failures=new Dictionary<string,int>(); float leisureTravel=0,foodTravel=0;
        bool Until(Func<bool> done,float deadline)
        {
            while(!done() && w.Food.Time<deadline)
            {
                float meal=w.Food.MealClock;
                leisureTravel+=.1f*w.People.Count(p=>p.Task==Work.ToLeisure && p.Route.Count>0);
                foodTravel+=.1f*w.People.Count(p=>p.Role is Role.Farmer or Role.Fisher or Role.Forager or Role.Baker && p.Route.Count>0);
                w.Tick(.1f); if((int)(w.Food.Time*10)%20==0) w.Validate();
                if(w.Food.MealClock<meal && w.Campaign!.Lake!.Phase==2 && w.Campaign.Lake.Meals==0)
                { string reason=w.Campaign.Lake.LastResult; failures[reason]=failures.GetValueOrDefault(reason)+1; }
            }
            return done();
        }
        void Plan(BuildingKind kind,Cell preferred)
        {
            var plots=w.Map.Land.SelectMany(c=>new[]{(Cell:c,Rotated:false),(Cell:c,Rotated:true)})
                .Where(p=>w.PlacementProblem(p.Cell,p.Rotated,kind)==null)
                .OrderBy(p=>(p.Cell.Point-preferred.Point).LengthSquared()).ThenBy(p=>p.Cell.X).ThenBy(p=>p.Cell.Z).ToArray();
            if(plots.Length==0) throw new Exception($"No draft plot for {kind}");
            var plot=plots[0]; w.Place(plot.Cell,plot.Rotated,kind);
            Console.WriteLine($"NARROW {(bread?"bread":"garden")}{(centralSquare?" / central square":"")} planned {kind} at {plot.Cell.X},{plot.Cell.Z} (rotated {plot.Rotated}) at {w.Food.Time:0}s.");
        }
        Plan(BuildingKind.FishingDock,new(3,4)); w.Assign(6,Role.Fisher);
        if(centralSquare) Plan(BuildingKind.Square,new(-3,6));
        Plan(bread?BuildingKind.Farm:BuildingKind.VegetableGarden,new(-3,6)); w.Assign(7,Role.Farmer);
        if(bread) { Until(()=>w.Food.Time>20,21); Plan(BuildingKind.Bakery,new(0,3)); w.Assign(1,Role.Baker); }
        if(!Until(()=>w.DeliveredFish>=4,600)) throw new Exception("Narrow first catch stalled");
        w.AdvanceLakePhase();
        Console.WriteLine($"NARROW first catch: {w.Food.Time:0}s, food {w.Food.EdibleStored}");
        foreach(var tree in w.Trees.Where(t=>t.Cell.X<3)) w.SetClearing(tree.Cell,true);
        Plan(BuildingKind.Cottage,new(-6,6)); Plan(BuildingKind.Cottage,new(0,3)); if(!centralSquare) Plan(BuildingKind.Square,new(-3,6));
        if(Until(()=>w.Beds>=12 && w.InvitationProblem()==null,1200))
        {
            Console.WriteLine($"NARROW first expansion: {w.Food.Time:0}s, food {w.Food.EdibleStored}");
            w.InviteNewcomers(); w.Assign(8,Role.Logger); w.Assign(9,Role.Builder);
            if(Until(()=>w.InvitationProblem()==null,1300)) w.InviteNewcomers();
            if(Until(()=>w.LakeActionProblem()==null,1500)) { Console.WriteLine($"NARROW assessment: {w.Food.Time:0}s, food {w.Food.EdibleStored}"); w.AdvanceLakePhase(); }
        }
        if(recover)
        {
            Until(()=>false,w.Food.Time+180);
            Console.WriteLine($"NARROW recovery begins {w.Food.Time:0}s: {w.Campaign!.Lake!.LastResult}");
            Plan(BuildingKind.Square,new(0,3));
        }
        Until(()=>w.Campaign!.Complete,1800);
        Console.WriteLine($"NARROW {(bread?"bread":"garden")}{(centralSquare?" / central square":"")}{(recover?" / recovery":"")} outcome: {w.Food.Time:0}s, complete {w.Campaign!.Complete}, food {w.Food.EdibleStored}, {w.Population} residents; leisure travel {leisureTravel:0} person-seconds; food-worker land travel {foodTravel:0} person-seconds. Failed meals: {string.Join("; ",failures.Select(f=>$"{f.Value} x {f.Key}"))}");
        System.IO.Directory.CreateDirectory("artifacts");
        System.IO.File.WriteAllText($"artifacts/narrow-lake-{(bread?"bread":"garden")}-{centralSquare}-{recover}.json",w.SaveJson());
    }
}

using Inlanders.Simulation;

public static class RiverChecks
{
    static void Check(bool ok,string why) { if(!ok) throw new Exception(why); }
    static void Until(World w,Func<bool> done,string step,int limit=24000)
    {
        for(int i=0;i<limit && !done();i++) { w.Tick(.1f); if(i%25==0) w.Validate(); }
        Check(done(),$"River stalled at {step}: {w.Food.Time:0}s, food {w.Food.EdibleStored}, {w.RiverObjective}");
        w.Validate(); Console.WriteLine($"RIVER {step}: {w.Food.Time:0}s, population {w.Population}, food {w.Food.EdibleStored}");
    }
    static Cottage Build(World w,Cell cell,BuildingKind kind,bool rotated=false) => w.Place(cell,rotated,kind) ?? throw new Exception($"River rejected {kind} {cell}: {w.PlacementProblem(cell,rotated,kind)}");
    public static void Run()
    {
        var w=World.NewCampaign(6);
        Check(w.CurrentCampaignHint()!=null,"River guidance missing");
        w.Campaign!.Dismissed.Add("river"); Check(w.CurrentCampaignHint()==null,"River hint ignored dismissal"); w.Campaign.Dismissed.Clear();
        Check(w.Housed==8 && w.EastBankBeds==0 && !w.AdvanceRiverPhase(),"River starts with an unearned neighborhood");
        Check(w.Place(new(8,0))==null,"Far bank available without a bridge");
        string initial=w.SaveJson(); Check(World.LoadJson(initial).SaveJson()==initial,"River map did not save exactly");
        var bridge=Build(w,new(5,2),BuildingKind.Bridge,true);
        Build(w,new(-1,6),BuildingKind.VegetableGarden); w.Assign(6,Role.Farmer); w.Assign(7,Role.Logger);
        Until(w,()=>bridge.Complete,"early bridge");
        Build(w,new(8,0),BuildingKind.Cottage); Build(w,new(8,4),BuildingKind.Cottage);
        Build(w,new(8,7),BuildingKind.VegetableGarden);
        Until(w,()=>w.Beds>=12 && w.InvitationProblem()==null,"first homes");
        Check(w.InviteNewcomers(),"First invitation failed"); w.Assign(8,Role.Farmer); w.Assign(9,Role.Builder);
        Until(w,()=>w.InvitationProblem()==null,"second invitation ready"); Check(w.InviteNewcomers(),"Second invitation failed");
        w.Assign(10,Role.Logger); w.Assign(11,Role.Forager);
        Until(w,()=>w.Available>=22,"reserve final construction timber");
        foreach(var p in w.People.Where(p=>p.Role==Role.Logger).ToArray()) w.Assign(p.Id,Role.Unassigned);
        Check(w.AdvanceRiverPhase(),"First assessment rejected");
        Until(w,()=>w.Campaign!.River!.Meals>=2,"first assessment");
        var holding=World.LoadJson(w.SaveJson()); foreach(var p in holding.People) holding.Assign(p.Id,Role.Unassigned);
        for(int i=0;i<1300;i++) holding.Tick(.1f);
        Check(holding.Campaign!.River!.Meals==2 && World.LoadJson(holding.SaveJson()).Campaign!.River!.Phase==1,"Completed first proof expired while waiting for the player");
        Check(w.AdvanceRiverPhase(),"Final expansion rejected");
        Build(w,new(11,6),BuildingKind.Cottage); Build(w,new(14,10),BuildingKind.Cottage);
        Build(w,new(11,0),BuildingKind.Square); Build(w,new(9,-6),BuildingKind.VegetableGarden);
        Until(w,()=>w.Beds>=16 && w.InvitationProblem()==null,"final homes");
        Check(w.InviteNewcomers(),"Third invitation failed"); w.Assign(12,Role.Farmer); w.Assign(13,Role.Forager);
        Until(w,()=>w.InvitationProblem()==null,"fourth invitation ready"); Check(w.InviteNewcomers(),"Fourth invitation failed");
        w.Assign(14,Role.Farmer); w.Assign(15,Role.Forager);
        Until(w,()=>w.RiverActionProblem()==null,"final preparation"); Check(w.AdvanceRiverPhase(),"Final assessment rejected");
        Until(w,()=>w.Campaign!.Complete,"completion");
        Console.WriteLine("PASS: river cottage/garden approach reaches both player-triggered expansions and fresh-food assessments.");
        AssessmentChecks(w);
        BreadApproach();
    }
    static void BreadApproach()
    {
        var w=World.NewCampaign(6);
        Build(w,new(-1,6),BuildingKind.Farm); w.Assign(3,Role.Farmer); w.Assign(6,Role.Builder); w.Assign(7,Role.Baker);
        Until(w,()=>w.PlacementProblem(new(3,2),true,BuildingKind.Bakery)==null,"bakery frontage clear");
        Build(w,new(3,2),BuildingKind.Bakery,true);
        Until(w,()=>w.DeliveredBread>=4,"bread before crossing");
        var bridge=Build(w,new(5,6),BuildingKind.Bridge,true);
        Build(w,new(-1,-3),BuildingKind.Sawmill); w.Assign(1,Role.Sawyer);
        Until(w,()=>bridge.Complete,"southern bridge");
        Build(w,new(8,0),BuildingKind.Lodge); Build(w,new(8,7),BuildingKind.Farm);
        Until(w,()=>w.Beds>=12 && w.InvitationProblem()==null,"first lodge");
        Check(w.InviteNewcomers(),"Lodge first invitation failed"); w.Assign(8,Role.Farmer); w.Assign(9,Role.Builder);
        Until(w,()=>w.InvitationProblem()==null,"lodge second invitation ready"); Check(w.InviteNewcomers(),"Lodge second invitation failed");
        w.Assign(10,Role.Logger); w.Assign(11,Role.Builder);
        Check(w.AdvanceRiverPhase(),"Bread assessment rejected"); Until(w,()=>w.Campaign!.River!.Meals>=2,"bread assessment");
        string halfway=w.SaveJson(); var copy=World.LoadJson(halfway);
        for(int i=0;i<50;i++) { w.Tick(.1f); copy.Tick(.1f); }
        Check(w.SaveJson()==copy.SaveJson(),"River assessment continuation diverged");
        Check(w.AdvanceRiverPhase(),"Lodge final expansion rejected");
        Build(w,new(11,6),BuildingKind.Lodge); Build(w,new(11,0),BuildingKind.Square);
        Until(w,()=>w.Beds>=16 && w.InvitationProblem()==null,"second lodge");
        Check(w.InviteNewcomers(),"Lodge third invitation failed");
        Until(w,()=>w.InvitationProblem()==null,"lodge fourth invitation ready"); Check(w.InviteNewcomers(),"Lodge fourth invitation failed");
        w.Assign(12,Role.Farmer); w.Assign(13,Role.Baker);
        Until(w,()=>w.RiverActionProblem()==null,"lodge final preparation"); Check(w.AdvanceRiverPhase(),"Lodge final assessment rejected");
        Until(w,()=>w.Campaign!.Complete,"lodge completion");
        Console.WriteLine("PASS: food-first southern crossing, sawmill/lodges and bread approach with exact assessment continuation.");
    }
    static void AssessmentChecks(World complete)
    {
        World Prepare()
        {
            var copy=World.LoadJson(complete.SaveJson());
            copy.Campaign!.Complete=false; copy.Campaign.River!.Phase=2; copy.Campaign.River.Meals=0;
            foreach(var p in copy.People) copy.Assign(p.Id,Role.Unassigned);
            Until(copy,()=>copy.People.All(p=>p.Carried==0) && copy.RiverActionProblem()==null,"assessment fixture settled");
            return copy;
        }
        var idle=Prepare(); idle.Food.Vegetables+=200; idle.Food.GrownVegetables+=200;
        Check(idle.AdvanceRiverPhase(),"Idle assessment did not start");
        for(int i=0;i<650;i++) idle.Tick(.1f);
        Check(idle.Campaign!.River!.Meals==0 && !idle.Campaign.Complete && idle.Campaign.River.LastResult.Contains("Fresh pantry"),"Stored food won without fresh production");
        var token=Prepare(); token.Food.EatenVegetables+=token.Food.Vegetables; token.Food.Vegetables=1; token.Food.GrownVegetables++;
        token.Food.EatenBread+=token.Food.Bread; token.Food.Bread=0;
        Check(token.AdvanceRiverPhase(),"Token assessment did not start");
        token.Food.Berries+=token.Population; token.Food.GatheredBerries+=token.Population;
        token.Food.MealClock=59.9f; token.Tick(.2f); token.Validate();
        Check(token.Campaign!.River!.Meals==0 && token.Campaign.River.LastResult.Contains("dominant food"),"One vegetable passed a village meal");
        var hungry=Prepare();
        hungry.Food.EatenBerries+=hungry.Food.Berries; hungry.Food.Berries=0;
        hungry.Food.EatenVegetables+=hungry.Food.Vegetables; hungry.Food.Vegetables=0;
        hungry.Food.EatenBread+=hungry.Food.Bread; hungry.Food.Bread=0;
        Check(hungry.AdvanceRiverPhase(),"Recoverable shortage could not enter assessment");
        hungry.Food.MealClock=59.9f; hungry.Tick(.2f); hungry.Validate();
        Check(hungry.Food.Hunger==1 && hungry.Campaign!.River!.Meals==0,"Shortage did not reset streak");
        for(int i=0;i<complete.Population;i++) hungry.Assign(i,complete.People[i].Role);
        Until(hungry,()=>hungry.Campaign!.Complete,"supply recovery");
        var larger=World.LoadJson(complete.SaveJson()); larger.Campaign!.Complete=false; larger.Campaign.River!.Phase=2; larger.Campaign.River.Meals=0;
        larger.Assign(0,Role.Logger); // Expansion beyond the funded plan needs fresh timber work.
        var extra=Build(larger,new(17,4),BuildingKind.Cottage);
        Until(larger,()=>extra.Complete && larger.InvitationProblem()==null,"extra homes"); Check(larger.InviteNewcomers(),"Extra invitation failed");
        Until(larger,()=>larger.RiverActionProblem()==null,"18-person preparation"); Check(larger.AdvanceRiverPhase(),"Larger assessment rejected");
        Until(larger,()=>larger.Campaign!.Complete,"18-person completion");
        Check(larger.Food.LastMealRequired==18 && larger.Campaign.River!.Required>=54,"Assessment ignored extra residents");
        var rearranged=World.LoadJson(complete.SaveJson()); rearranged.Campaign!.Complete=false; rearranged.Campaign.River!.Phase=2; rearranged.Campaign.River.Meals=0;
        Check(rearranged.AdvanceRiverPhase(),"Layout assessment did not start");
        Until(rearranged,()=>rearranged.Campaign.River.Meals==1,"layout initial meal");
        var house=rearranged.Cottages.Single(c=>c.Cell==new Cell(8,4)); var square=rearranged.Cottages.Single(c=>c.Kind==BuildingKind.Square);
        Check(rearranged.RequestDemolition(house.Id),"Layout recovery could not remove house");
        Until(rearranged,()=>rearranged.Campaign.River.Meals==0,"housing setback resets proof");
        Check(rearranged.RequestDemolition(square.Id),"Layout recovery could not move center");
        Until(rearranged,()=>!rearranged.Cottages.Any(c=>c.Id==house.Id || c.Id==square.Id),"clear new center");
        Build(rearranged,new(17,4),BuildingKind.Cottage); Build(rearranged,new(8,4),BuildingKind.Square);
        Until(rearranged,()=>rearranged.Campaign.Complete,"rearranged neighborhood recovery");
        Console.WriteLine("PASS: stored-food and token-food exclusions, recoverable hunger, actual recreation and expanded-population assessments.");
    }
}

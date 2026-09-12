using Inlanders.Simulation;

static class BushRelocationChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    static void Step(World w,int count=1){for(int i=0;i<count;i++){w.Tick(.1f);w.Validate();}}
    static void Until(World w,Func<bool> done,string why){for(int i=0;i<10000 && !done();i++)Step(w);Check(done(),why);}
    static Cell Destination(World w,int id)=>w.Map.Land.Where(c=>w.Bushes.Single(b=>b.Id==id).Cell!=c && w.BushMoveProblem(id,c)==null).OrderBy(c=>(new Cell(c.X+1,c.Z).Point-w.YardAccess.Point).LengthSquared()).First();
    static void Roundtrip(World w)
    {
        string saved=w.SaveJson();var loaded=World.LoadJson(saved);Check(loaded.SaveJson()==saved,"Moved bush save changed");
        for(int i=0;i<100;i++){Step(w);Step(loaded);Check(w.SaveJson()==loaded.SaveJson(),$"Original moved bush continuation diverged at tick {i+1}");}
    }
    public static void Run()
    {
        var normal=World.NewScenario();string original=normal.SaveJson();Check(!normal.MoveBush(normal.Bushes[0].Id,new(3,0)) && normal.SaveJson()==original,"Normal mode moved bush");
        foreach(var phase in new[]{Work.ToBush,Work.Foraging,Work.ToPantry})
        {
            var w=World.NewCreative();foreach(var person in w.People)w.Assign(person.Id,Role.Unassigned);
            var hut=w.Place(new(3,0),0,BuildingKind.ForagerHut)!;w.Assign(0,Role.Forager);w.SetWorkplaceAssignment(0,hut.Id);
            Until(w,()=>w.People[0].BushId!=null,"No bush claim");int id=w.People[0].BushId!.Value;
            Until(w,()=>w.People[0].Task==phase,"No phase "+phase);var picker=w.People[0];var bush=w.Bushes.Single(b=>b.Id==id);
            var at=Destination(w,id);string before=w.SaveJson();int ripe=bush.Ripe;float regrowth=bush.Regrowth;int gathered=w.Food.GatheredBerries,carried=picker.Carried;var task=picker.Task;
            for(int i=0;i<10;i++)Check(w.BushMoveProblem(id,at)==null,"Valid preview rejected");
            Check(w.SaveJson()==before && ReferenceEquals(bush,w.Bushes.Single(b=>b.Id==id)),"Preview mutated source/order");
            Check(w.MoveBush(id,bush.Cell) && w.SaveJson()==before,"Same-place confirmation interrupted picker");
            Check(w.MoveBush(id,at) && bush.Cell==at && bush.Ripe==ripe && bush.Regrowth==regrowth && w.Food.GatheredBerries==gathered,"Move changed bush food/state");
            Check(picker.AssignedWorkplaceId==hut.Id && picker.Role==Role.Forager,"Move changed worker assignment");
            if(phase==Work.ToPantry)Check(picker.Task==task && picker.Carried==carried,"Move interrupted already harvested cargo");
            else Check(picker.BushId==null && bush.Owner==null,"Move kept old picker claim");
            // Move again without advancing time or replenishing food.
            var second=Destination(w,id);Check(w.MoveBush(id,second) && bush.Ripe==ripe && bush.Regrowth==regrowth,"Repeated move reset regrowth");
            Roundtrip(w);
            Until(w,()=>w.People[0].BushId==id,"Forager did not return to the moved bush");
            Until(w,()=>w.Food.GatheredBerries>gathered && w.People[0].Carried>0,"Forager never resumed after relocation");
            Until(w,()=>w.DeliveredBerries>0 && w.People[0].Carried==0,"Post-move delivery failed");w.Validate();
        }
        RejectionsAndHabitat();
        Console.WriteLine("PASS: Creative-only bush relocation, read-only previews, no-op, walking/picking interruption, detached cargo, repeated moves, resumed foraging, state/assignment preservation, invalid destinations, habitat and exact saves.");
    }
    static void RejectionsAndHabitat()
    {
        var w=World.NewCreative(true);foreach(var person in w.People)w.Assign(person.Id,Role.Unassigned);var bush=w.Bushes[0];
        var habitat=w.Map.Wildlife.Select(h=>(h.Id,h.Stock,Capacity:w.HabitatCapacity(h),Recovery:w.HabitatRecovery(h))).ToArray();
        void Refuse(Cell at)
        {string before=w.SaveJson();Check(w.BushMoveProblem(bush.Id,at)!=null && !w.MoveBush(bush.Id,at) && w.SaveJson()==before,"Rejected destination mutated world: "+at);}
        Refuse(new(1000,1000));Refuse(w.Stockpile);Refuse(w.YardAccess);Refuse(w.Trees[0].Cell);Refuse(w.Bushes[1].Cell);Refuse(w.Bushes[1].Access);Refuse(w.Map.StoneDeposits[0].Access);
        foreach(var h in w.Map.Wildlife)Refuse(h.Cell);
        if(w.Map.Water.Count>0)Refuse(w.Map.Water.First());
        var path=Destination(w,bush.Id);Check(w.SetPath(path,true),"Path fixture failed");Refuse(path);w.SetPath(path,false);
        w.ManagedWoodland.Add(path);Refuse(path);w.ManagedWoodland.Remove(path);
        Check(w.PlaceDecoration(path,DecorationKind.Flowers),"Decoration fixture failed");Refuse(path);w.RemoveDecoration(path);
        var position=w.People[0].Position;w.People[0].Position=path.Point;Refuse(path);w.People[0].Position=position;
        string saved=w.SaveJson();Check(!w.MoveBush(-99,path) && saved==w.SaveJson(),"Missing bush mutated world");
        Check(w.MoveBush(bush.Id,Destination(w,bush.Id)),"Habitat move failed");
        Check(habitat.SequenceEqual(w.Map.Wildlife.Select(h=>(h.Id,h.Stock,Capacity:w.HabitatCapacity(h),Recovery:w.HabitatRecovery(h)))) ,"Bush move altered habitat stock/capacity/recovery");Roundtrip(w);
        // Narrow the authored quarry map's empty connecting neck to one walking lane.
        var data=System.Text.Json.Nodes.JsonNode.Parse(World.NewQuarryMap().SaveJson())!;data["Creative"]=true;
        var corridor=World.LoadJson(data.ToJsonString());foreach(var person in corridor.People)corridor.Assign(person.Id,Role.Unassigned);
        for(int x=6;x<=9;x++)for(int z=-3;z<=2;z++)if(z!=0)corridor.Map.Excluded.Add(new(x,z));
        string before=corridor.SaveJson();Check(corridor.BushMoveProblem(corridor.Bushes[0].Id,new(7,0))?.Contains("cut off")==true && !corridor.MoveBush(corridor.Bushes[0].Id,new(7,0)) && corridor.SaveJson()==before,"Bush blocked the only connecting route");
    }
}

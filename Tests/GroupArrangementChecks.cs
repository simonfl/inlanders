using Inlanders.Simulation;
static class GroupArrangementChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    public static (Cell Target,GroupPlan Plan) Find(World w,int[] ids,int turn,bool paths=false)
    {
        var pivot=w.Cottages.Single(c=>c.Id==ids[0]).Cell;
        foreach(var cell in w.Map.Land.OrderBy(c=>(c.Point-pivot.Point).LengthSquared()).ThenBy(c=>c.Z).ThenBy(c=>c.X))
        {
            if(cell==pivot && turn==0)continue;var plan=w.PreviewGroup(ids,cell,turn,paths);if(plan.Result!=null)return(cell,plan);
        }
        throw new Exception("No legal group arrangement");
    }
    public static void Run()
    {
        foreach(bool relaxed in new[]{false,true})
        {
            var w=World.NewWorkingClearing(relaxed);var ids=w.Cottages.Where(c=>c.Kind==BuildingKind.Cottage && c.Cell.X<0).Select(c=>c.Id).ToArray();var pivot=w.Cottages.Single(c=>c.Id==ids[0]).Cell;
            var carried=w.GroupPaths(ids);Check(carried.Length>0,"No internal approaches");string before=w.SaveJson();var (target,plan)=Find(w,ids,0,true);var moved=plan.Result!;
            Check(carried.Select(c=>World.RotateOffset(target,c.X-pivot.X,c.Z-pivot.Z,0)).All(moved.Paths.Contains),"Carried path missing");
            var covered=moved.Cottages.Where(c=>ids.Contains(c.Id)).SelectMany(World.Footprint).ToHashSet();
            Check(w.Paths.Except(carried).Where(c=>!covered.Contains(c)).All(moved.Paths.Contains),"Unrelated path removed");Check(w.SaveJson()==before,"Path proposal mutated source");
            Check(w.PreviewGroup(ids,new(999,999),0,true).Result==null && w.SaveJson()==before,"Invalid carried path changed source");
            var copy=World.LoadJson(moved.SaveJson());for(int i=0;i<600;i++){moved.Tick(.1f);copy.Tick(.1f);}moved.Validate();Check(moved.SaveJson()==copy.SaveJson(),"Carried paths continuation differs");
            Console.WriteLine($"PASS carried approaches {relaxed}: {carried.Length} actual tiles, unrelated paths retained and exact continuation.");
        }
        foreach(bool relaxed in new[]{false,true})foreach(bool ripe in new[]{false,true})
        {
            var w=World.NewWorkingClearing(relaxed);var field=w.Cottages.Single(c=>c.Kind==BuildingKind.VegetableField && c.Cell==new Cell(-1,-1));
            if(ripe){for(int i=0;i<6000 && field.Harvest==0;i++)w.Tick(.1f);Check(field.Harvest>0,"No actual ripe crop");}
            int[] ids={w.Cottages.Single(c=>c.Cell==new Cell(-3,5)).Id,field.Id};string before=w.SaveJson();var (_,plan)=Find(w,ids,0);var moved=plan.Result!;var next=moved.Cottages.Single(c=>c.Id==field.Id);
            Check(w.SaveJson()==before,"Farmstead preview mutated source");Check(next.Harvest==field.Harvest,"Ripe harvest lost");if(!ripe)Check(!next.Planted && next.Growth==0,"Growing crop retained after relocation");
            Check(next.WorkPaused==field.WorkPaused && next.OutputTarget==field.OutputTarget,"Field settings changed");
            int meals=moved.Food.MealConsumptions.Count;var copy=World.LoadJson(moved.SaveJson());for(int i=0;i<3000;i++){moved.Tick(.1f);copy.Tick(.1f);}moved.Validate();Check(moved.SaveJson()==copy.SaveJson(),"Farmstead continuation diverged");Check(moved.Food.MealConsumptions.Count>meals,"No actual meals after rearrangement");
            Console.WriteLine($"PASS farmstead group {relaxed}/{ripe}: crop consequence, settings, actual meals and exact five-minute continuation.");
        }
        foreach(bool relaxed in new[]{false,true})foreach(int turn in new[]{0,1,2,3})
        {
            var w=World.NewWorkingClearing(relaxed);var ids=w.Cottages.Where(c=>c.Cell.X<0 && c.Kind==BuildingKind.Cottage).Select(c=>c.Id).ToArray();string before=w.SaveJson();
            var (target,plan)=Find(w,ids,turn);Check(w.SaveJson()==before,"Preview changed source");var changed=plan.Result!;
            Check(changed.People.Select(p=>p.HomeId).SequenceEqual(w.People.Select(p=>p.HomeId)),"Changed households");
            Check(changed.Stored==w.Stored && changed.Planks==w.Planks && changed.Population==w.Population,"Changed resources/population");
            Check(w.PreviewGroup(ids,new(999,999),turn).Result==null && w.SaveJson()==before,"Invalid partial group applied");
            var copy=World.LoadJson(changed.SaveJson());for(int i=0;i<600;i++){changed.Tick(.1f);copy.Tick(.1f);}changed.Validate();Check(changed.SaveJson()==copy.SaveJson(),"Group continuation differs");
            Console.WriteLine($"PASS group homes {relaxed}/{turn}: target {target}, atomic preview, households/resources and exact 60s continuation.");
        }
    }
}

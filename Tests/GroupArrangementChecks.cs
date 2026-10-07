using Inlanders.Simulation;
static class GroupArrangementChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    public static (Cell Target,GroupPlan Plan) Find(World w,int[] ids,int turn)
    {
        var pivot=w.Cottages.Single(c=>c.Id==ids[0]).Cell;
        foreach(var cell in w.Map.Land.OrderBy(c=>(c.Point-pivot.Point).LengthSquared()).ThenBy(c=>c.Z).ThenBy(c=>c.X))
        {
            if(cell==pivot && turn==0)continue;var plan=w.PreviewGroup(ids,cell,turn);if(plan.Result!=null)return(cell,plan);
        }
        throw new Exception("No legal group arrangement");
    }
    public static void Run()
    {
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

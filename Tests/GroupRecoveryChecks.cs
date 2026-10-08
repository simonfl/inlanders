using Inlanders.Simulation;
static class GroupRecoveryChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    public static void Run()
    {
        foreach(bool relaxed in new[]{false,true})foreach(int turn in new[]{0,1,2,3})
        {
            var original=World.NewWorkingClearing(relaxed);int[] ids={original.Cottages.Single(c=>c.Cell==new Cell(-3,5)).Id,original.Cottages.Single(c=>c.Cell==new Cell(-1,-1)).Id};
            var (_,proposal)=GroupArrangementChecks.Find(original,ids,turn);var w=proposal.Result!;var recovery=original.RememberGroup(w,ids,turn);
            for(int i=0;i<900;i++)w.Tick(.1f);
            var plan=w.PreviewGroupRecovery(recovery);
            for(int attempt=0;plan.Result==null && attempt<120;attempt++){string blocked=w.SaveJson();w.PreviewGroupRecovery(recovery);Check(w.SaveJson()==blocked,"Blocked recovery mutated source");for(int tick=0;tick<10;tick++)w.Tick(.1f);plan=w.PreviewGroupRecovery(recovery);}
            string before=w.SaveJson();plan=w.PreviewGroupRecovery(recovery);Check(plan.Result!=null,"Recovery failed: "+plan.Problem);var restored=plan.Result!;
            Check(before==w.SaveJson(),"Recovery preview changed live world");Check(restored.Food.Time==w.Food.Time && restored.EdibleStored==w.EdibleStored && restored.Stored==w.Stored,"Recovery rewound time/resources");
            Check(ids.All(id=>{var a=original.Cottages.Single(c=>c.Id==id);var b=restored.Cottages.Single(c=>c.Id==id);return a.Cell==b.Cell && a.Rotation==b.Rotation;}),"Original geometry not restored");
            Check(original.Paths.SetEquals(restored.Paths),"Original approaches not restored");var copy=World.LoadJson(restored.SaveJson());for(int i=0;i<600;i++){restored.Tick(.1f);copy.Tick(.1f);}restored.Validate();Check(restored.SaveJson()==copy.SaveJson(),"Recovery continuation differs");
            Check(restored.PreviewGroupRecovery(recovery).Result==null,"Stale recovery accepted");Console.WriteLine($"PASS group recovery {relaxed}/{turn}: geometry and approaches, current time/resources, exact continuation, stale rejection.");
        }
    }
}

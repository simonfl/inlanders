using Inlanders.Simulation;
static class ChallengeContinuationChecks
{
    public static void Run()
    {
        foreach(bool relaxed in new[]{false,true})foreach(bool repair in new[]{false,true})
        {
            var path=repair?$"artifacts/217-repair-{relaxed}-bridge.json":$"artifacts/215-challenge-{relaxed}-False.json";
            var w=World.LoadFile(path);w.Challenge!.Continuing=true;
            var p=ReviewPlacement.Find(w,BuildingKind.Cottage,new(0,-10),(_,_)=>true,"continue")??throw new Exception("No home site");
            w.Place(p.Actual,p.Rotation,BuildingKind.Cottage);
            for(int i=0;i<6000 && w.InvitationProblem()!=null;i++)w.Tick(.1f);
            if(!w.InviteNewcomers() || w.Population!=18 || w.People.TakeLast(2).Any(p=>!p.SharedWorker))throw new Exception("Growth blocked: "+w.InvitationProblem());
            var copy=World.LoadJson(w.SaveJson());for(int i=0;i<40;i++){w.Tick(.1f);copy.Tick(.1f);}
            if(w.SaveJson()!=copy.SaveJson() || !w.Challenge.Complete || w.PublicPlace!.Create().Population!=12)throw new Exception("Continued identity/save");
            w.SaveFile($"artifacts/219-continued-{relaxed}-{repair}.json");
            Console.WriteLine($"PASS post-challenge optional growth to18 and exact continuation: relaxed={relaxed}, repair={repair}");
        }
    }
}

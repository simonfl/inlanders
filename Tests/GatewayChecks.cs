using Inlanders.Simulation;

static class GatewayChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    public static void Run()
    {
        foreach(bool creative in new[]{false,true})foreach(int facing in Enumerable.Range(0,4))foreach(bool pathFirst in new[]{false,true})
        {
            var w=creative?World.NewCreative():World.NewScenario();
            var cell=w.Map.Land.First(c=>w.DecorationProblem(c,DecorationKind.Gateway)==null);
            int logs=w.Stored;string before=w.SaveJson();
            Check(!w.PlaceDecoration(cell,DecorationKind.Gateway,false,4) && before==w.SaveJson(),"Bad facing changed world");
            if(pathFirst)Check(w.SetPath(cell,true),"Path-first fixture failed");
            Check(w.PlaceDecoration(cell,DecorationKind.Gateway,false,facing),"Gateway placement failed");
            if(!pathFirst)Check(w.SetPath(cell,true),"Gateway-first path failed");
            var gate=w.Decorations.Single();Check(!gate.Solid && gate.Facing==facing && w.Paths.Contains(cell) && w.Stored==logs,"Gateway state/cost/path wrong");
            Check(World.FenceOffersConnection(gate,new(1,0))==(facing%2==0) && World.FenceOffersConnection(gate,new(0,1))==(facing%2!=0),"Gateway connection sides wrong");
            w.Validate();string saved=w.SaveJson();Check(World.LoadJson(saved).SaveJson()==saved,"Gateway facing save failed");
            Check(w.SetPath(cell,false) && w.Decorations.Count==1,"Path removal removed gateway");w.SetPath(cell,true);
            if(creative)
            {
                var result=w.PrepareCreativeRemoval(new(Array.Empty<BuildingRemovalTarget>(),new[]{gate},Array.Empty<Cell>()));
                Check(result.World!=null && result.World.Paths.Contains(cell) && result.World.Decorations.Count==0,"Area removal lost path");
            }
            Check(w.RemoveDecoration(cell) && w.Paths.Contains(cell) && w.Stored==logs,"Gateway removal lost path/resources");
        }
        var live=World.NewScenario();var site=live.Place(new(3,0))!;
        bool crossed=false,placed=false;Cell gateway=default;
        for(int i=0;i<6000 && !site.Complete;i++)
        {
            if(!placed)
            {
                var candidate=live.People.Where(p=>p.Carried>0).SelectMany(p=>p.Route).Cast<Cell?>().FirstOrDefault(c=>live.DecorationProblem(c!.Value,DecorationKind.Gateway)==null);
                if(candidate is Cell c){gateway=c;Check(live.PlaceDecoration(c,DecorationKind.Gateway,false,2),"Live cargo placement failed");placed=true;}
            }
            live.Tick(.1f);live.Validate();
            crossed|=placed && live.People.Any(p=>World.At(p)==gateway && p.Carried>0);
        }
        Check(placed && crossed && site.Complete,"Actual loaded worker did not cross gateway and finish delivery");
        var clone=World.LoadJson(live.SaveJson());for(int i=0;i<100;i++){live.Tick(.1f);clone.Tick(.1f);}
        Check(live.SaveJson()==clone.SaveJson(),"Gateway saved continuation diverged");
        foreach(var cell in new[]{live.YardAccess,site.Entrance,live.Trees[0].Access,live.Bushes[0].Cell})
            Check(live.DecorationProblem(cell,DecorationKind.Gateway)!=null,"Service/resource gateway allowed");
        var wet=World.NewLargeMap();Check(!wet.PlaceDecoration(wet.Map.Water.First(),DecorationKind.Gateway),"Water gateway allowed");
        Console.WriteLine("PASS: four gateway facings, free normal/Creative placement, path coexistence/removal, side connections, live loaded traversal, current saves and service protection.");
    }
}

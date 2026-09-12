using Inlanders.Simulation;
using System.Text;

static class StoneStagingPlayable
{
    public static void Run()
    {
        var report=new StringBuilder("# Actual stone staging comparison\n\nNormal quarry map, initial resources and eight residents unchanged. All variants start the clock before planning, pay ordinary costs and retain food/rest. Roles: logger 0, builders 1/2, foragers 3/4, farmer 5, sawyer 6, quarrier 7. The optional hauler replaces builder 2 only after the pile completes. No speed or stock injection. Near-first routes switch to a newly built distant quarry when the eight-stone deposit is exhausted. All routes finish the same distant hall; the first eight stone milestone is reported separately. Travel is actual distance moved, in tiles, attributed to the worker's current role.\n\n| Source policy | Pile policy | Pile ready (s) | First 8 at hall (s) | Hall complete (s) | Builder travel | Quarrier travel | Hauler travel | All travel | Pile building labor (s) |\n|---|---|---:|---:|---:|---:|---:|---:|---:|---:|\n");
        foreach(bool nearFirst in new[]{false,true})foreach(int policy in new[]{0,1,2})
        {
            if(nearFirst && policy==2)continue;
            var w=World.NewQuarryMap();
            Cell Legal(BuildingKind kind,Cell desired)=>w.Map.Land.Where(c=>w.PlacementProblem(c,0,kind)==null).OrderBy(c=>(c.Point-desired.Point).LengthSquared()).ThenBy(c=>c.X).ThenBy(c=>c.Z).First();
            var mill=w.Place(Legal(BuildingKind.Sawmill,new(0,-3)),0,BuildingKind.Sawmill)!;
            Cottage Quarry(int source)
            {
                var deposit=w.Map.StoneDeposits.Single(d=>d.Id==source);
                var cell=w.Map.Land.Where(c=>(c.Point-deposit.Cell.Point).LengthSquared()<=16 && w.PlacementProblem(c,0,BuildingKind.Quarry)==null)
                    .OrderBy(c=>(c.Point-deposit.Cell.Point).LengthSquared()).ThenBy(c=>c.X).ThenBy(c=>c.Z).First();
                return w.Place(cell,0,BuildingKind.Quarry)!;
            }
            var camp=Quarry(nearFirst?0:1);
            var hall=w.Place(new(11,2),0,BuildingKind.GatheringHall)??throw new Exception("Comparison hall blocked");
            Cottage? pile=null;
            if(policy!=0)
            {
                pile=w.Place(nearFirst?new(8,1):new(14,2),nearFirst?2:3,BuildingKind.Stockpile)??throw new Exception("Comparison pile blocked");
                w.SetStorageMaterial(pile.Id,Resource.Stone);w.SetPriority(pile.Id,2);
            }
            w.Assign(6,Role.Sawyer);w.Assign(7,Role.Quarrier);
            float? pileReady=null,eight=null;float buildLabor=0;bool switched=false,hauler=false;
            var travel=Enum.GetValues<Role>().ToDictionary(r=>r,r=>0f);
            for(int i=0;i<24000 && !hall.Complete;i++)
            {
                if(nearFirst && !switched && w.Map.StoneDeposits[0].Remaining==0)
                {w.SetWorkplacePaused(camp.Id,true);camp=Quarry(1);switched=true;}
                if(pile?.Complete==true && pileReady==null){pileReady=w.Food.Time;if(policy==2){w.SetStorageTarget(pile.Id,12);w.Assign(2,Role.Hauler);hauler=true;}}
                if(eight==null && hall.DeliveredStone>=8)eight=w.Food.Time;
                var before=w.People.Select(p=>(p.Position,p.Role)).ToArray();
                buildLabor+=w.People.Count(p=>pile!=null && p.SiteId==pile.Id && p.Task==Work.Building)*.1f;
                w.Tick(.1f);w.Validate();
                for(int n=0;n<w.Population;n++)travel[before[n].Role]+=(w.People[n].Position-before[n].Position).Length();
            }
            if(!hall.Complete || hall.DeliveredStone!=12 || policy!=0 && pileReady==null || policy==2 && !hauler)throw new Exception("Staging comparison did not finish: "+string.Join("; ",w.People.Select(p=>p.Status)));
            string saved=w.SaveJson();if(World.LoadJson(saved).SaveJson()!=saved)throw new Exception("Comparison save differs");
            report.AppendLine($"| {(nearFirst?"Near then distant":"Distant")} | {(policy==0?"None":policy==1?"Direct; no hauler":"Target 12; one builder becomes hauler")} | {pileReady?.ToString("0.0")??"—"} | {eight:0.0} | {w.Food.Time:0.0} | {travel[Role.Builder]:0.0} | {travel[Role.Quarrier]:0.0} | {travel[Role.Hauler]:0.0} | {travel.Values.Sum():0.0} | {buildLabor:0.0} |");
        }
        Directory.CreateDirectory("artifacts/stone-staging");File.WriteAllText("artifacts/stone-staging/playable.md",report.ToString());Console.WriteLine(report);
        Console.WriteLine("PASS: five normal-play complete projects with setup costs, finite extraction, matched starting labor, direct/hauler policies and exact current saves.");
    }
}

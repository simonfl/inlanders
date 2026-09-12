using Inlanders.Simulation;
using System.Text;

namespace Inlanders.Simulation
{
    public sealed partial class World
    {
        // Test-only access to the actual path-cost routine; no new simulation rule.
        internal int ReviewStoneTravel(Cell from,Cell to)=>TravelCost(from,to);
    }
}

static class StoneStagingReview
{
    public static void Run()
    {
        var report=new StringBuilder("# Stone staging route screen\n\nActual path geometry; hypothetical local stone acceptance. Units are weighted path costs, not seconds. Each row evaluates min(12, remaining deposit stock) in two-stone round trips per worker. Deposit 0 has only eight stone and cannot finish the hall alone. Central routing is measured without the optional pile. Setup includes two log-hauling round trips for a four-log stockpile, but excludes construction time, fatigue, contention and first/last-trip offsets.\n\n| Project | Deposit / quantity | Stage | Central route | Local route | Log setup travel | Difference after setup |\n|---|---|---|---:|---:|---:|---:|\n");
        int rows=0;
        foreach(bool remote in new[]{false,true})foreach(int sourceId in new[]{0,1})
        {
            var world=World.NewQuarryMap();
            var desired=remote?new Cell(11,2):new Cell(0,-3);
            var hallCell=world.Map.Land.Where(c=>world.PlacementProblem(c,0,BuildingKind.GatheringHall)==null).OrderBy(c=>(c.Point-desired.Point).LengthSquared()).First();
            var hall=world.Place(hallCell,0,BuildingKind.GatheringHall)!;
            var source=world.Map.StoneDeposits.Single(d=>d.Id==sourceId).Access;
            int quantity=Math.Min(12,world.Map.StoneDeposits.Single(d=>d.Id==sourceId).Remaining),trips=quantity/2;
            int BaselineRound(Cell a,Cell b)=>world.ReviewStoneTravel(a,b)+world.ReviewStoneTravel(b,a);
            long baseline=(long)trips*(BaselineRound(source,world.YardAccess)+BaselineRound(world.YardAccess,hall.Entrance));
            string original=world.SaveJson();
            var anchors=new[]{("near source",source),("near project",hall.Entrance),("near yard",world.YardAccess)};
            foreach(var (name,anchor) in anchors)
            {
                var candidates=world.Map.Land.SelectMany(c=>Enumerable.Range(0,4).Select(r=>(Cell:c,Rotation:r)))
                    .Where(p=>world.PlacementProblem(p.Cell,p.Rotation,BuildingKind.Stockpile)==null)
                    .OrderBy(p=>(p.Cell.Point-anchor.Point).LengthSquared()).Take(12).ToArray();
                var choices=candidates.Select(candidate=>
                {
                    var copy=World.LoadJson(original);var stock=copy.Place(candidate.Cell,candidate.Rotation,BuildingKind.Stockpile)!;
                    int Round(Cell a,Cell b)=>copy.ReviewStoneTravel(a,b)+copy.ReviewStoneTravel(b,a);
                    long central=baseline;
                    long local=(long)trips*(Round(source,stock.Entrance)+Round(stock.Entrance,hall.Entrance));
                    long setup=2L*Round(copy.YardAccess,stock.Entrance);
                    return (central,local,setup,stock.Cell,stock.Rotation);
                }).OrderBy(c=>c.local+c.setup).ToArray();
                if(choices.Length==0)throw new Exception("No legal staging candidates");
                var best=choices[0];report.AppendLine($"| {(remote?"Distant":"Central")} ({hallCell.X},{hallCell.Z}) | {sourceId} / {quantity} | {name} ({best.Cell.X},{best.Cell.Z}) r{best.Rotation} | {best.central} | {best.local} | {best.setup} | {best.central-best.local-best.setup} |");rows++;
                if(world.SaveJson()!=original)throw new Exception("Route comparison changed baseline");
            }
        }
        if(rows!=12)throw new Exception("Missing route scenarios");
        Directory.CreateDirectory("artifacts/stone-staging");File.WriteAllText("artifacts/stone-staging/routes.md",report.ToString());
        Console.WriteLine(report.ToString());
        Console.WriteLine("PASS: 12 staging geometry comparisons, legal footprints/access, four-way candidate orientations and unchanged baseline snapshots. No local stone simulation implemented.");
    }
}

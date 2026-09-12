using Inlanders.Simulation;
using System.Text;

namespace Inlanders.Simulation
{
    public sealed partial class World
    {
        // Review-only evidence: no runtime terrain-edit command is exposed yet.
        internal HashSet<Cell> TerrainReviewReachable()=>Reachable(YardAccess,Blocked);
        internal HashSet<Cell> TerrainReviewProtected()=>new[]{Stockpile,YardAccess}
            .Concat(Trees.SelectMany(t=>new[]{t.Cell,t.Access})).Concat(Bushes.SelectMany(b=>new[]{b.Cell,b.Access}))
            .Concat(Map.StoneDeposits.SelectMany(d=>new[]{d.Cell,d.Access})).Concat(Map.Wildlife.Select(h=>h.Cell))
            .Concat(Cottages.SelectMany(c=>Footprint(c.Cell,c.Rotation,c.Kind).Append(c.Entrance)
                .Concat(c.Kind==BuildingKind.Bridge?new[]{Door(c.Cell,c.Rotation),FarBank(c.Cell,c.Rotation)}:Array.Empty<Cell>())
                .Concat(c.Kind==BuildingKind.FishingDock?new[]{c.Launch}:Array.Empty<Cell>())))
            .Concat(Paths).Concat(ManagedWoodland).Concat(Decorations.Select(d=>d.Cell))
            .Concat(People.Select(At)).Concat(People.SelectMany(p=>p.Route))
            .Concat(People.Where(p=>p.Task!=Work.Waiting).Select(p=>p.Destination))
            .Concat(People.Where(p=>p.Meal is {Reserved:true} or {Carrying:true}).Select(p=>p.Meal!.Seat))
            .Concat(MeetingSpots).ToHashSet();
    }
}

static class TerrainShapingReview
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    // Prototype only: clamp the old gently sloped surface into the allowable height
    // interval at each corner's Manhattan distance from the selected plateau.
    static (float[] Heights,HashSet<Cell> Changed) Shape(MapLayout map,Cell first,Cell last,float target)
    {
        int left=Math.Min(first.X,last.X)-map.MinX,right=Math.Max(first.X,last.X)-map.MinX+1;
        int back=Math.Min(first.Z,last.Z)-map.MinZ,front=Math.Max(first.Z,last.Z)-map.MinZ+1;
        var heights=new float[(map.Width+1)*(map.Depth+1)];
        for(int z=0;z<=map.Depth;z++)for(int x=0;x<=map.Width;x++)
        {
            int distance=Math.Max(0,Math.Max(left-x,x-right))+Math.Max(0,Math.Max(back-z,z-front));
            heights[z*(map.Width+1)+x]=Math.Clamp(map.CornerHeight(x,z),Math.Max(0,target-.4f*distance),Math.Min(4,target+.4f*distance));
        }
        var changed=new HashSet<Cell>();
        for(int z=0;z<map.Depth;z++)for(int x=0;x<map.Width;x++)
            if(new[]{(x,z),(x+1,z),(x,z+1),(x+1,z+1)}.Any(c=>Math.Abs(map.CornerHeight(c.Item1,c.Item2)-heights[c.Item2*(map.Width+1)+c.Item1])>.00001f))
                changed.Add(new(map.MinX+x,map.MinZ+z));
        return (heights,changed);
    }
    static bool Clear(World w,HashSet<Cell> changed)=>changed.All(c=>w.Map.Contains(c) && !w.Map.Water.Contains(c)) && !changed.Overlaps(w.TerrainReviewProtected());
    public static void Run()
    {
        var report=new StringBuilder("# Terrain shaping geometry review\n\nTest-only prototype; no player-facing terrain edit command or preview UI. Shared corner elevations are clamped around a rectangular plateau, with maximum 0.4 rise per cardinal corner edge. The entire changed-cell apron is screened against occupied/protected ground. Paths and current walking routes are protected in this first policy. Heights do not currently affect simulation route costs.\n\n| Scenario | Cottage cell | Target height | Selected cells | Changed cells | Apron cells |\n|---|---|---:|---:|---:|---:|\n");
        foreach(var scenario in new[]{("Flat clearing",false,.4f,false),("Raised terrace",true,.8f,false),("Level an existing slope",true,.8f,true)})
        {
            var w=World.NewCreative(scenario.Item2);foreach(var person in w.People)w.Assign(person.Id,Role.Unassigned);
            string saved=w.SaveJson();var before=w.TerrainReviewReachable();bool found=false;
            foreach(var at in w.Map.Land.OrderBy(c=>c.Point.LengthSquared()))
            {
                var siteCells=World.Footprint(at,0).Append(World.Door(at,0)).ToArray();
                if(!siteCells.All(w.Map.Contains) || w.Map.LevelGround(siteCells)==scenario.Item4)continue;
                var first=new Cell(siteCells.Min(c=>c.X),siteCells.Min(c=>c.Z));var last=new Cell(siteCells.Max(c=>c.X),siteCells.Max(c=>c.Z));
                var draft=Shape(w.Map,first,last,scenario.Item3);
                if(draft.Changed.Count==0 || !Clear(w,draft.Changed))continue;
                var copy=World.LoadJson(saved);copy.Map.Heights=draft.Heights;copy.Map.Validate();
                if(copy.PlacementProblem(at,0)!=null)continue;
                Check(copy.Map.LevelGround(siteCells) && copy.TerrainReviewReachable().SetEquals(before),"Terrace is not buildable or changed route access");
                Check(w.SaveJson()==saved,"Review changed live source");
                var built=copy.Place(at,0);Check(built?.Complete==true,"Actual cottage could not occupy terrace");
                Check(World.LoadJson(copy.SaveJson()).SaveJson()==copy.SaveJson(),"Terrace building save failed");
                Check(draft.Changed.Overlaps(copy.TerrainReviewProtected()),"Undo screen missed intervening building");
                Check(copy.RemoveBuilding(built!.Id),"Cannot remove terrace cottage");
                for(int i=0;i<20;i++){copy.Tick(.1f);copy.Validate();}
                float time=copy.Food.Time;int logs=copy.Stored;
                // The eventual runtime inverse must restore only terrain; it must never load the old world snapshot.
                copy.Map.Heights=(float[])w.Map.Heights.Clone();copy.Validate();
                Check(copy.Food.Time==time && copy.Stored==logs && copy.Map.Heights.SequenceEqual(w.Map.Heights),"Terrain-only inverse rewound simulation");
                Check(World.LoadJson(copy.SaveJson()).SaveJson()==copy.SaveJson(),"Terrain inverse save failed");
                int selected=(last.X-first.X+1)*(last.Z-first.Z+1);
                report.AppendLine($"| {scenario.Item1} | ({at.X},{at.Z}) | {scenario.Item3:0.0} | {selected} | {draft.Changed.Count} | {draft.Changed.Count-selected} |");
                found=true;break;
            }
            Check(found,"No legal usable terrace found: "+scenario.Item1);
        }
        // A neighboring cell changes even when it is outside the selected rectangle.
        var flat=World.NewCreative();foreach(var p in flat.People)flat.Assign(p.Id,Role.Unassigned);
        var edit=Shape(flat.Map,new(3,-3),new(3,-3),.4f);
        Check(edit.Changed.Contains(new(2,-3)),"Shared corner apron missing");
        Check(flat.SetPath(new(2,-3),true),"Apron path fixture failed");Check(!Clear(flat,edit.Changed),"Path outside selection not protected");
        // Original details cannot be reconstructed by applying the opposite elevation delta.
        var contour=new MapLayout{MinX=0,MinZ=0,Width=12,Depth=12,Heights=new float[13*13]};
        for(int z=0;z<=12;z++)for(int x=0;x<=12;x++)contour.Heights[z*13+x]=.4f*Math.Min(Math.Min(x,12-x),Math.Min(z,12-z));
        contour.Validate();var original=(float[])contour.Heights.Clone();contour.Heights=Shape(contour,new(5,5),new(6,6),2.8f).Heights;contour.Validate();
        contour.Heights=Shape(contour,new(5,5),new(6,6),2.0f).Heights;contour.Validate();Check(!original.SequenceEqual(contour.Heights),"Contour-loss proof failed");
        report.AppendLine("\nVerified: legal cottage footprint and entrance on all three terraces; unchanged reachable cells before construction; occupied apron refusal; protection-set overlap detects intervening construction; terrain-only restoration preserves elapsed time and stocks. Raising then lowering does not recover original contours. Production implementation still needs authoritative validation, live apply/undo guards and rendered preview/rebuild checks.");
        Directory.CreateDirectory("artifacts/terrain-shaping");File.WriteAllText("artifacts/terrain-shaping/geometry.md",report.ToString());Console.WriteLine(report);
        Console.WriteLine("PASS: three usable terrace geometries, shared-corner apron protection, route preservation, actual building placement, current saves and terrain-only restoration proof.");
    }
}

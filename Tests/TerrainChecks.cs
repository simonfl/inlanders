using Inlanders.Simulation;
using System.Text.Json.Nodes;

public static class TerrainChecks
{
    static void Check(bool ok, string message) { if (!ok) throw new Exception(message); }
    public static void Run()
    {
        var w = World.NewLargeMap();
        Check(w.Map.SurfaceHeight(-9,-10) == 1.6f && w.Map.SurfaceHeight(-2,11) == 1.6f, "Both raised meadows missing");
        Check(w.Map.SurfaceHeight(-3,3) == 0 && w.Map.Water.All(c => w.Map.SurfaceHeight(c.X,c.Z) == 0), "Yard or water raised");
        Check(World.NewScenario().Map.Heights.Length == 0 && Enumerable.Range(1,5).All(i => World.NewCampaign(i).Map.Heights.Length == 0), "Campaign terrain changed");
        foreach (bool rotated in new[] { false, true })
        {
            var slope = w.Map.Land.First(c => w.Map.SurfaceHeight(c.X,c.Z) is > .2f and < 1.2f &&
                World.Footprint(c,rotated).Append(World.Door(c,rotated)).All(w.Map.Contains));
            Check(w.PlacementProblem(slope,rotated)!.Contains("level ground") && w.Place(slope,rotated) == null, "Slope construction accepted");
        }
        var creative = World.NewCreative(true);
        var plant = creative.Map.Land.First(c => creative.Map.SurfaceHeight(c.X,c.Z) is > .2f and < 1.2f && creative.PlantingProblem(c) == null);
        Check(creative.PlantTree(plant) != null, "Slope planting rejected");
        var path = creative.Map.Land.First(c => creative.Map.SurfaceHeight(c.X,c.Z) is > .2f and < 1.2f && creative.PathProblem(c,false) == null);
        Check(creative.SetPath(path,true), "Slope path rejected");
        var flower = creative.Map.Land.First(c => creative.Map.SurfaceHeight(c.X,c.Z) is > .2f and < 1.2f && creative.DecorationProblem(c,DecorationKind.Flowers) == null);
        Check(creative.PlaceDecoration(flower,DecorationKind.Flowers), "Slope decoration rejected");
        World.LoadJson(creative.SaveJson());

        // An isolated corner rise can be missed by tile-center-only siting checks.
        var corners = new MapLayout { Heights = new float[18*16] };
        corners.Heights[5*18+5] = .2f;
        Check(!corners.LevelGround(new[] { new Cell(-3,-2) }), "Corner-only slope ignored");
        Check(Math.Abs(corners.SurfaceHeight(-3.5f,-2.5f)-.2f)<.0001f, "Corner sampling differs from mesh");

        var bakery=w.Place(new(-10,-11),false,BuildingKind.Bakery);
        Check(bakery != null && w.Place(new(-2,10)) != null, "Hilltop buildings rejected");
        Check(w.Place(new(3,0),false,BuildingKind.Farm) != null && w.Place(new(3,-3),false,BuildingKind.ForagerHut) != null, "Hill economy fixture rejected");
        var roles=new[] { Role.Logger,Role.Logger,Role.Builder,Role.Builder,Role.Farmer,Role.Baker,Role.Forager,Role.Forager };
        for(int i=0;i<roles.Length;i++) w.Assign(i,roles[i]);
        bool climbedWithCargo=false, savedOnSlope=false;
        for(int i=0;i<16000 && w.Food.Bread==0;i++)
        {
            w.Tick(.1f); w.Validate();
            if(w.People.Any(p => p.Carried>0 && w.Map.SurfaceHeight(p.Position.X,p.Position.Y) is > .1f and < 1.5f))
            {
                climbedWithCargo=true;
                if(!savedOnSlope)
                {
                    string json=w.SaveJson(); var copy=World.LoadJson(json);
                    Check(copy.SaveJson()==json,"Uphill save roundtrip changed");
                    for(int j=0;j<20;j++) { w.Tick(.1f); copy.Tick(.1f); }
                    Check(w.SaveJson()==copy.SaveJson(),"Uphill continuation diverged"); savedOnSlope=true;
                }
            }
        }
        Check(w.Food.Bread>0 && bakery!.Complete && climbedWithCargo && savedOnSlope,"Hilltop bakery did not receive and produce real goods");
        var invalid=JsonNode.Parse(w.SaveJson())!; invalid["Map"]!["Heights"]![0]=3f;
        bool rejected=false; try { World.LoadJson(invalid.ToJsonString()); } catch(System.IO.InvalidDataException) { rejected=true; }
        Check(rejected,"An abrupt cliff loaded");
        Check(World.NewScenario().PlacementProblem(new(8,0),false)!.Contains("inside"),"Boundary rejection incorrectly blames slopes");
        var crossing=World.NewCreative(true); crossing.Place(new(7,3),true,BuildingKind.Bridge);
        crossing.Map.Heights[(3-crossing.Map.MinZ)*(crossing.Map.Width+1)+(6-crossing.Map.MinX)] = .2f;
        rejected=false; try { World.LoadJson(crossing.SaveJson()); } catch(System.IO.InvalidDataException) { rejected=true; }
        Check(rejected,"Saved bridge with sloping banks loaded");
        Console.WriteLine("PASS: two raised meadows, flat campaign/river, full-footprint slope rejection, slope paths/planting/decorations, hilltop bakery deliveries, uphill saves and cliff rejection.");
    }
}

using System;
using System.Linq;
using Inlanders.Simulation;

public static class DecorationChecks
{
    static void Check(bool ok,string message) { if(!ok) throw new Exception(message); }
    public static void Run()
    {
        var w=new World(40);
        foreach(var kind in Enum.GetValues<DecorationKind>().Where(k=>k!=DecorationKind.Sunflowers))
        {
            var cell=w.Map.Land.First(c=>w.DecorationProblem(c,kind)==null);
            int logs=w.Stored;
            Check(w.PlaceDecoration(cell,kind,true),"Decoration rejected");
            Check(w.Stored==logs,"Decoration spent timber");
            Check(!w.PlaceDecoration(cell,kind),"Overlapping decoration accepted");
            Check(w.PlantingProblem(cell)!=null,"Planting overlaps decoration");
            Check(w.SetPath(cell,true)==(kind==DecorationKind.Pebbles),"Decoration path rule incorrect");
            w.Validate();
            string saved=w.SaveJson(); var loaded=World.LoadJson(saved);
            Check(loaded.SaveJson()==saved,"Decoration save differs");
            Check(loaded.Decorations.Single().Rotated,"Rotation lost");
            Check(w.RemoveDecoration(cell) && !w.RemoveDecoration(cell),"Removal wrong");
        }
        Check(!w.PlaceDecoration(w.YardAccess,DecorationKind.Fence),"Yard access covered");
        Check(!w.PlaceDecoration(w.Trees[0].Access,DecorationKind.Shrub),"Resource access covered");
        Check(!w.PlaceDecoration(World.At(w.People[0]),DecorationKind.Flowers),"Worker covered");
        var water=World.NewLargeMap();
        Check(!water.PlaceDecoration(water.Map.Water.First(),DecorationKind.Pebbles),"Water decorated");
        // Paint around live work. Successful edits must reroute without losing jobs or resources.
        var site=w.Place(new(3,0))!;
        for(int tick=0;tick<2200;tick++)
        {
            if(tick%17==0)
            {
                var candidate=w.People.SelectMany(p=>p.Route.Skip(1)).Cast<Cell?>()
                    .FirstOrDefault(c=>w.DecorationProblem(c!.Value,DecorationKind.Fence)==null);
                if(candidate!=null) Check(w.PlaceDecoration(candidate.Value,DecorationKind.Fence),"Legal live decoration failed");
            }
            w.Tick(.1f); w.Validate();
            if(tick%83==0 && w.Decorations.Count>0) w.RemoveDecoration(w.Decorations[0].Cell);
        }
        Check(site.Complete,"Decorations stopped reachable construction");
        var clone=World.LoadJson(w.SaveJson());
        for(int i=0;i<100;i++) { w.Tick(.1f); clone.Tick(.1f); }
        Check(w.SaveJson()==clone.SaveJson(),"Decorated settlement continuation diverged");
        Console.WriteLine("PASS: decoration palette, costs, occupancy, path interaction, rotation/saves, removal and live-route conservation.");
    }
}

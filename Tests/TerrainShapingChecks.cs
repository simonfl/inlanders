using Inlanders.Simulation;

static class TerrainShapingChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    static World Fresh(bool large=false){var w=World.NewCreative(large);foreach(var p in w.People)w.Assign(p.Id,Role.Unassigned);return w;}
    static (Cell At,Cell First,Cell Last,TerrainEditPreview Preview) Site(World w,float target,bool? level=null)
    {
        foreach(var at in w.Map.Land.OrderBy(c=>c.Point.LengthSquared()))
        {
            var cells=World.Footprint(at,0).Append(World.Door(at,0)).ToArray();
            if(!cells.All(w.Map.Contains) || level.HasValue && w.Map.LevelGround(cells)!=level.Value)continue;
            var first=new Cell(cells.Min(c=>c.X),cells.Min(c=>c.Z));var last=new Cell(cells.Max(c=>c.X),cells.Max(c=>c.Z));
            var preview=w.PreviewTerrain(first,last,target);
            if(preview.Problem==null && preview.ChangedCells.Count>0)return(at,first,last,preview);
        }
        throw new Exception("No terrace fixture");
    }
    static void UnchangedRefusal(World w,TerrainEditPreview preview)
    {string saved=w.SaveJson();Check(!w.ApplyTerrain(preview) && saved==w.SaveJson(),"Rejected edit mutated village");}
    public static void Run()
    {
        var normal=World.NewScenario();UnchangedRefusal(normal,normal.PreviewTerrain(new(4,0),new(6,2),.4f));
        foreach(var scenario in new[]{(false,.4f,true),(true,.8f,true),(true,.8f,false)})
        {
            var w=Fresh(scenario.Item1);var site=Site(w,scenario.Item2,scenario.Item3);string saved=w.SaveJson();
            for(int i=0;i<5;i++)Check(w.PreviewTerrain(site.First,site.Last,scenario.Item2).Problem==null,"Preview failed");
            Check(w.SaveJson()==saved,"Preview mutated world");
            UnchangedRefusal(Fresh(scenario.Item1),site.Preview);
            Check(w.ApplyTerrain(site.Preview),"Valid Apply failed");w.Validate();
            Check(w.PlacementProblem(site.At,0)==null,"Terrace not buildable");
            var loaded=World.LoadJson(w.SaveJson());Check(loaded.SaveJson()==w.SaveJson() && !loaded.UndoTerrain(),"Save or session-local undo failed");
            var built=w.Place(site.At,0)!;Check(built.Complete && w.TerrainUndoProblem()!=null,"Building failed to block undo");
            string occupied=w.SaveJson();Check(!w.UndoTerrain() && occupied==w.SaveJson(),"Blocked undo mutated world");
            Check(w.RemoveBuilding(built.Id),"Could not clear undo obstruction");
            for(int i=0;i<20;i++){w.Tick(.1f);w.Validate();}
            var expected=World.LoadJson(w.SaveJson());expected.Map.Heights=World.LoadJson(saved).Map.Heights;
            Check(w.UndoTerrain() && w.SaveJson()==expected.SaveJson(),"Undo changed nonterrain state or lost original contour");w.Validate();
            Check(!w.UndoTerrain(),"Undo repeated");
        }
        var flat=Fresh();var firstSite=Site(flat,.4f);var draft=firstSite.Preview;
        var border=draft.ChangedCells.First(c=>c.X<firstSite.First.X || c.X>firstSite.Last.X || c.Z<firstSite.First.Z || c.Z>firstSite.Last.Z);
        Check(flat.SetPath(border,true),"Border path fixture failed");UnchangedRefusal(flat,draft);
        var blockedPreview=flat.PreviewTerrain(firstSite.First,firstSite.Last,.4f);
        Check(blockedPreview.Blocker is {Kind:"Path"} && blockedPreview.Blocker.Cell==border,"Path blocker not identified");
        Check(flat.SetPath(border,false) && flat.ApplyTerrain(draft),"Fresh revalidation failed after clearing path");
        Check(flat.SetPath(border,true),"Undo path fixture failed");
        string withPath=flat.SaveJson();Check(!flat.UndoTerrain() && flat.SaveJson()==withPath,"New border path did not block undo");
        Check(flat.TerrainUndoBlocker() is {Kind:"Path"} undoBlocker && undoBlocker.Cell==border,"Undo blocker not identified");
        Check(flat.SetPath(border,false),"Cannot clear undo path");
        flat.People[0].Route.Enqueue(border);
        Check(flat.TerrainUndoBlocker() is {Kind:"Walking route"} traffic && traffic.Cell==border,"Walking reservation lacks recovery guidance");
        string trafficSave=flat.SaveJson();Check(!flat.UndoTerrain() && flat.SaveJson()==trafficSave,"Blocked traffic Undo mutated world");
        flat.People[0].Route.Clear();Check(flat.TerrainUndoProblem()==null,"Cleared traffic kept Undo blocked");
        var noop=flat.PreviewTerrain(firstSite.First,firstSite.Last,.4f);
        Check(noop.ChangedCells.Count==0 && flat.ApplyTerrain(noop),"No-op failed");
        foreach(float target in new[]{float.NaN,float.PositiveInfinity,-.4f,4.4f,.3f})UnchangedRefusal(flat,flat.PreviewTerrain(firstSite.First,firstSite.Last,target));
        UnchangedRefusal(flat,flat.PreviewTerrain(new(int.MinValue,0),new(int.MaxValue,0),.4f));
        Check(flat.UndoTerrain(),"No-op or rejection erased undo");
        Check(flat.ApplyTerrain(draft),"Reapply original failed");
        var raised=(float[])flat.Map.Heights.Clone();
        Check(flat.ApplyTerrain(flat.PreviewTerrain(firstSite.First,firstSite.Last,0)),"Lowering terrace failed");
        flat.Validate();Check(flat.Map.LevelGround(World.Footprint(firstSite.At,0).Append(World.Door(firstSite.At,0))),"Lowered terrace not level");
        Check(flat.UndoTerrain() && flat.Map.Heights.SequenceEqual(raised),"Undo lowering did not restore terrace");
        var second=flat.PreviewTerrain(firstSite.First,firstSite.Last,.8f);
        // Larger apron may touch occupied ground: select a separate legal site if necessary.
        if(second.Problem!=null)second=Site(flat,.8f).Preview;
        var afterFirst=(float[])flat.Map.Heights.Clone();Check(flat.ApplyTerrain(second),"Second edit failed");
        UnchangedRefusal(flat,draft);Check(flat.UndoTerrain() && flat.Map.Heights.SequenceEqual(afterFirst),"Undo did not restore immediately previous terrain");
        Check(!flat.UndoTerrain(),"Unexpected multi-edit history");
        var live=World.NewCreative();
        for(int i=0;i<100 && !live.People.Any(p=>p.Route.Count>0);i++)live.Tick(.1f);
        var route=live.People.First(p=>p.Route.Count>0).Route.First();
        UnchangedRefusal(live,live.PreviewTerrain(route,route,.4f));
        var liveSite=Site(live,.4f);Check(live.ApplyTerrain(liveSite.Preview),"Unrelated live trips blocked whole village");
        var twin=World.LoadJson(live.SaveJson());
        for(int i=0;i<100;i++){live.Tick(.1f);twin.Tick(.1f);live.Validate();twin.Validate();}
        Check(live.SaveJson()==twin.SaveJson(),"Edited terrain save continuation diverged");
        foreach(var cell in new[]{live.Stockpile,live.Trees[0].Cell,live.Bushes[0].Cell})UnchangedRefusal(live,live.PreviewTerrain(cell,cell,.8f));
        var water=Fresh();var waterSite=Site(water,.4f);var wet=waterSite.Preview.ChangedCells.Last();
        water.Map.Water.Add(wet);UnchangedRefusal(water,waterSite.Preview);
        UnchangedRefusal(water,water.PreviewTerrain(wet,wet,0));
        water.Map.Water.Remove(wet);water.Map.Excluded.Add(wet);UnchangedRefusal(water,waterSite.Preview);
        water.Map.Excluded.Remove(wet);
        water.Map.MinX++;UnchangedRefusal(water,waterSite.Preview);water.Map.MinX--;
        Check(water.ApplyTerrain(waterSite.Preview),"Restored map bounds rejected");
        water.Map.Heights[0]=.1f;string external=water.SaveJson();
        Check(!water.UndoTerrain() && water.SaveJson()==external,"Undo overwrote externally changed terrain");
        Console.WriteLine("PASS: terrain transactions, distinct buildable terraces, stale/border protection, blocked/retry undo, no-ops, bounds, session saves and live trips.");
    }
}

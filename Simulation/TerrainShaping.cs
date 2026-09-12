using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Inlanders.Simulation;

public sealed record TerrainBlocker(Cell Cell,string Kind,string Guidance)
{
    public string Message=>$"{Kind} at {Cell.X}, {Cell.Z}. {Guidance}";
    public string UndoMessage=>$"{Kind} at {Cell.X}, {Cell.Z}. "+(Kind switch
    {
        "Building"=>"Remove the building before retrying Undo.",
        "Path"=>"Remove this path before retrying Undo.",
        "Decoration"=>"Remove this decoration before retrying Undo.",
        "Villager" or "Walking route" or "Work destination" or "Reserved meal seat"=>Guidance,
        _=>"Clear this use of the tile before retrying Undo."
    });
}

public sealed class TerrainEditPreview
{
    internal World Owner { get; }
    internal MapLayout Map { get; }
    internal float[] Before { get; }
    internal float[] After { get; }
    internal (int X,int Z,int Width,int Depth) Bounds { get; }
    public IReadOnlyList<float> Heights { get; }
    public IReadOnlyList<Cell> ChangedCells { get; }
    public string? Problem { get; }
    public TerrainBlocker? Blocker { get; }
    internal TerrainEditPreview(World owner,float[] before,float[] after,Cell[] changed,string? problem,TerrainBlocker? blocker=null)
    {
        Owner=owner;Map=owner.Map;Before=before;After=after;
        Bounds=(Map.MinX,Map.MinZ,Map.Width,Map.Depth);
        Heights=Array.AsReadOnly(after);ChangedCells=Array.AsReadOnly(changed);Problem=problem;Blocker=blocker;
    }
}

public sealed partial class World
{
    // Session-local, deliberately absent from saves. World replacement discards undo.
    private TerrainEditPreview? _terrainUndo;

    private TerrainBlocker? TerrainAreaBlocker(IEnumerable<Cell> changed)
    {
        var protectedCells=new Dictionary<Cell,TerrainBlocker>();
        void Add(IEnumerable<Cell> cells,string kind,string guidance)
        {foreach(var cell in cells)protectedCells.TryAdd(cell,new(cell,kind,guidance));}
        const string avoid="Keep this tile outside the plot and border.";
        Add(new[]{Stockpile},"Village yard",avoid);
        Add(new[]{YardAccess},"Yard access",avoid);
        Add(Trees.Select(t=>t.Cell),"Tree",avoid);Add(Trees.Select(t=>t.Access),"Tree access",avoid);
        Add(Bushes.Select(b=>b.Cell),"Berry bush",avoid);Add(Bushes.Select(b=>b.Access),"Bush access",avoid);
        Add(Map.StoneDeposits.Select(d=>d.Cell),"Stone deposit",avoid);Add(Map.StoneDeposits.Select(d=>d.Access),"Stone access",avoid);
        Add(Map.Wildlife.Select(h=>h.Cell),"Wildlife tracking ground",avoid);
        Add(Cottages.SelectMany(c=>Footprint(c.Cell,c.Rotation,c.Kind)),"Building","Remove the building or change the terrace.");
        Add(Cottages.Select(c=>c.Entrance),"Building entrance",avoid);
        Add(Cottages.Where(c=>c.Kind==BuildingKind.Bridge).SelectMany(c=>new[]{Door(c.Cell,c.Rotation),FarBank(c.Cell,c.Rotation)}),"Bridge access",avoid);
        Add(Cottages.Where(c=>c.Kind==BuildingKind.FishingDock).Select(c=>c.Launch),"Dock launch",avoid);
        Add(Paths,"Path","Remove this path or change the terrace.");
        Add(ManagedWoodland,"Managed woodland",avoid);
        Add(Decorations.Select(d=>d.Cell),"Decoration","Remove this decoration or change the terrace.");
        Add(MeetingSpots,"Meeting spot",avoid);
        Add(People.Select(At),"Villager","Wait for the villager to move, then retry.");
        Add(People.SelectMany(p=>p.Route),"Walking route","Wait for traffic to pass, then retry.");
        Add(People.Where(p=>p.Task!=Work.Waiting).Select(p=>p.Destination),"Work destination","Wait for the task to finish, then retry.");
        Add(People.Where(p=>p.Meal is {Reserved:true} or {Carrying:true}).Select(p=>p.Meal!.Seat),"Reserved meal seat","Wait for the meal to finish, then retry.");
        foreach(var cell in changed)
        {
            if(!Map.Contains(cell) || Map.Water.Contains(cell))return new(cell,"Water or map edge","Keep the terrace and border on dry land.");
            if(protectedCells.TryGetValue(cell,out var blocker))return blocker;
        }
        return null;
    }

    private string? TerrainAreaProblem(IEnumerable<Cell> changed)
    {
        if(!Creative)return "Terrain shaping is available in Creative only.";
        if(Food.Celebrating)return "Wait until supper finishes.";
        return TerrainAreaBlocker(changed)?.Message;
    }

    public TerrainEditPreview PreviewTerrain(Cell first,Cell last,float target)
    {
        var before=(float[])Map.Heights.Clone();
        TerrainEditPreview Reject(string reason)=>new(this,before,before,Array.Empty<Cell>(),reason);
        if(!Creative)return Reject("Terrain shaping is available in Creative only.");
        if(!float.IsFinite(target) || target<0 || target>4 || Math.Abs(target/.4f-MathF.Round(target/.4f))>.0001f)
            return Reject("Choose an elevation from 0 to 4 in steps of 0.4.");
        if(!Map.Contains(first) || !Map.Contains(last))return Reject("Select a rectangle inside the map.");
        int minX=Math.Min(first.X,last.X),maxX=Math.Max(first.X,last.X);
        int minZ=Math.Min(first.Z,last.Z),maxZ=Math.Max(first.Z,last.Z);
        for(int z=minZ;z<=maxZ;z++)for(int x=minX;x<=maxX;x++)
            if(!Map.Contains(new(x,z)) || Map.Water.Contains(new(x,z)))return Reject("Select a rectangle of dry land.");
        int left=minX-Map.MinX,right=maxX-Map.MinX+1,back=minZ-Map.MinZ,front=maxZ-Map.MinZ+1;
        var after=new float[(Map.Width+1)*(Map.Depth+1)];
        for(int z=0;z<=Map.Depth;z++)for(int x=0;x<=Map.Width;x++)
        {
            int distance=Math.Max(0,Math.Max(left-x,x-right))+Math.Max(0,Math.Max(back-z,z-front));
            after[z*(Map.Width+1)+x]=Math.Clamp(Map.CornerHeight(x,z),Math.Max(0,target-.4f*distance),Math.Min(4,target+.4f*distance));
        }
        var changed=new List<Cell>();
        for(int z=0;z<Map.Depth;z++)for(int x=0;x<Map.Width;x++)
            if(Different(x,z)||Different(x+1,z)||Different(x,z+1)||Different(x+1,z+1))changed.Add(new(Map.MinX+x,Map.MinZ+z));
        bool Different(int x,int z)=>Map.CornerHeight(x,z)!=after[z*(Map.Width+1)+x];
        var blocker=TerrainAreaBlocker(changed);
        return new(this,before,after,changed.ToArray(),TerrainAreaProblem(changed),blocker);
    }

    public string? TerrainApplyProblem(TerrainEditPreview preview)
    {
        if(!ReferenceEquals(preview.Owner,this) || !ReferenceEquals(preview.Map,Map) || preview.Bounds!=(Map.MinX,Map.MinZ,Map.Width,Map.Depth))return "The village changed. Select the terrace again.";
        if(preview.Problem!=null)return preview.Problem;
        if(!Map.Heights.SequenceEqual(preview.Before))return "The terrain changed. Preview the terrace again.";
        return TerrainAreaProblem(preview.ChangedCells);
    }

    // Validate a candidate independently; publishing changes heights only, never simulation state.
    private bool ValidTerrainCandidate(float[] heights)
    {
        try{var copy=LoadJson(SaveJson());copy.Map.Heights=(float[])heights.Clone();copy.Validate();return true;}
        catch(InvalidDataException){return false;}
        catch(InvalidOperationException){return false;}
    }
    public bool ApplyTerrain(TerrainEditPreview preview)
    {
        if(TerrainApplyProblem(preview)!=null)return false;
        if(preview.ChangedCells.Count==0)return true;
        if(!ValidTerrainCandidate(preview.After))return false;
        Map.Heights=(float[])preview.After.Clone();_terrainUndo=preview;
        return true;
    }
    public string? TerrainUndoProblem()
    {
        if(_terrainUndo==null)return "No terrain change to undo.";
        if(!ReferenceEquals(_terrainUndo.Map,Map) || _terrainUndo.Bounds!=(Map.MinX,Map.MinZ,Map.Width,Map.Depth) || !Map.Heights.SequenceEqual(_terrainUndo.After))return "The terrain changed since this edit.";
        if(!Creative)return "Terrain shaping is available in Creative only.";
        if(Food.Celebrating)return "Wait until supper finishes.";
        return TerrainAreaBlocker(_terrainUndo.ChangedCells)?.UndoMessage;
    }
    public TerrainBlocker? TerrainUndoBlocker()=>_terrainUndo==null?null:TerrainAreaBlocker(_terrainUndo.ChangedCells);
    public bool UndoTerrain()
    {
        if(TerrainUndoProblem()!=null)return false;
        var edit=_terrainUndo!;
        if(!ValidTerrainCandidate(edit.Before))return false;
        Map.Heights=(float[])edit.Before.Clone();_terrainUndo=null;return true;
    }
}

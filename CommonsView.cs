using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
public partial class Game
{
    private Node3D? _commonsView;
    private bool _commonsMats;
    private World? _commonsVisualWorld;
    private string _commonsVisualKey="";
    private System.Collections.Generic.HashSet<Cell> _commonsGroundExcluded=new();
    private void UpdateCommonsView()
    {
        if(_commonsEntry==null)return;
        _commonsEntry.Visible=_world.CanArrangeCommons;
        _commonsEntry.Text=_world.Commons==null?"Make a shared place":"Rearrange shared place";
        _commonsRemove.Visible=_world.Commons!=null;
        string key=_world.Commons is {} c?$"{c.Center}:{_world.Cottages.Count}:{_world.Trees.Count}:{_world.Bushes.Count}:{_world.Decorations.Count}:"+string.Join(';',c.Places)+":"+string.Join(';',_world.Bushes.Select(b=>b.Cell)):"";
        if(key==_commonsVisualKey && _commonsView!=null && _commonsVisualWorld==_world)return;
        _commonsVisualWorld=_world;
        _commonsVisualKey=key;
        if(_commonsView==null){_commonsView=new();AddChild(_commonsView);}else Clear(_commonsView);
        if(_world.Commons is not {} commons)return;
        // A single low surface gives the place a silhouette without adding an obstacle.
        if(!_commonsMats)
        {
            _commonsGroundExcluded=_world.Cottages.SelectMany(b=>World.Footprint(b.Cell,b.Rotation,b.Kind))
                .Concat(_world.Trees.Select(t=>t.Cell)).Concat(_world.Bushes.Select(b=>b.Cell))
                .Concat(_world.Map.StoneDeposits.Select(d=>d.Cell)).Concat(_world.Decorations.Where(d=>d.Solid).Select(d=>d.Cell)).ToHashSet();
            var points=commons.Places.Append(commons.Center).SelectMany(p=>new[]{new Vector2(p.X-.65f,p.Z-.65f),new Vector2(p.X+.65f,p.Z-.65f),new Vector2(p.X+.65f,p.Z+.65f),new Vector2(p.X-.65f,p.Z+.65f)}).ToArray();
            var hull=Geometry2D.ConvexHull(points);
            CommonsSurface(hull,.012f,new("746850"));
            var center=new Vector2(commons.Center.X,commons.Center.Z);
            CommonsSurface(hull.Select(p=>p.MoveToward(center,.13f)).ToArray(),.025f,new("8b795c"));
        }
        // The older isolated mats remain a development comparison, using identical places and rules.
        foreach(var p in commons.Places)
        {
            Cylinder(_commonsView,OnGround(p.X,p.Z,.025f),.43f,.045f,new(_commonsMats?"b89569":"a08b66"));
            Cylinder(_commonsView,OnGround(p.X,p.Z,.05f),.31f,.012f,new(_commonsMats?"758d7b":"756c56"));
        }
        if(_commonsMats)Box(_commonsView,OnGround(commons.Center.X,commons.Center.Z,.035f),new(.9f,.05f,.9f),new("ccaa79"));
        if(_commonsMats)Box(_commonsView,OnGround(commons.Center.X,commons.Center.Z,.065f),new(.65f,.02f,.65f),new("a9634b"));
    }
    private void CommonsSurface(Vector2[] hull,float height,Color color)
    {
        using var surface=new SurfaceTool();surface.Begin(Godot.Mesh.PrimitiveType.Triangles);
        float minX=hull.Min(p=>p.X),maxX=hull.Max(p=>p.X),minZ=hull.Min(p=>p.Y),maxZ=hull.Max(p=>p.Y);
        foreach(var cell in _world.Map.Land.Where(c=>c.X+.5f>=minX && c.X-.5f<=maxX && c.Z+.5f>=minZ && c.Z-.5f<=maxZ && !_world.Map.Water.Contains(c) && !_commonsGroundExcluded.Contains(c)))
        {
            var tile=new[]{new Vector2(cell.X-.5f,cell.Z-.5f),new Vector2(cell.X+.5f,cell.Z-.5f),new Vector2(cell.X+.5f,cell.Z+.5f),new Vector2(cell.X-.5f,cell.Z+.5f)};
            foreach(var polygon in Geometry2D.IntersectPolygons(hull,tile))
                foreach(int i in Geometry2D.TriangulatePolygon(polygon)){surface.SetNormal(Vector3.Up);surface.AddVertex(OnGround(polygon[i].X,polygon[i].Y,height));}
        }
        var mesh=surface.Commit();var material=Material(color);
        _commonsView!.AddChild(new MeshInstance3D{Mesh=mesh,MaterialOverride=material});
    }
}

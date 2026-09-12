using Godot;
using Inlanders.Simulation;
using System;

public partial class Game
{
    private float Height(float x, float z) => _world.Map.SurfaceHeight(x, z);
    private Vector3 OnGround(float x, float z, float lift = 0) => new(x, Height(x,z) + lift, z);
    private static void Triangle(SurfaceTool surface, Vector3 a, Vector3 b, Vector3 c, Color color)
    {
        surface.SetColor(color); surface.SetNormal((b-a).Cross(c-a).Normalized());
        // Godot front faces use clockwise winding; keep the outward normal above.
        surface.AddVertex(a); surface.AddVertex(c); surface.AddVertex(b);
    }
    private MeshInstance3D SurfaceMesh(Node3D parent, SurfaceTool surface)
    {
        var result = new MeshInstance3D { Mesh = surface.Commit(), MaterialOverride = new StandardMaterial3D { VertexColorUseAsAlbedo = true, Roughness = 1 } };
        parent.AddChild(result); return result;
    }
    private void MakeTerrainSurface()
    {
        var top = new SurfaceTool(); top.Begin(Godot.Mesh.PrimitiveType.Triangles);
        var sides = new SurfaceTool(); sides.Begin(Godot.Mesh.PrimitiveType.Triangles);
        foreach (var cell in _world.Map.Land)
        {
            bool flatBoxes=_world.Map.Heights.Length==0 && !_world.Map.RiverMeadow;
            float lift=flatBoxes?.01f:0; // Match the former instanced grass-box top.
            var a = OnGround(cell.X-.5f,cell.Z-.5f,lift); var b = OnGround(cell.X+.5f,cell.Z-.5f,lift);
            var c = OnGround(cell.X+.5f,cell.Z+.5f,lift); var d = OnGround(cell.X-.5f,cell.Z+.5f,lift);
            var color = GroundTint(cell.X,cell.Z).Lightened(Height(cell.X,cell.Z)*.025f);
            if(_world.Map.RiverMeadow){MeadowTriangle(top,a,d,c);MeadowTriangle(top,a,c,b);}
            else if(_smoothGround){GroundColorTriangle(top,a,d,c);GroundColorTriangle(top,a,c,b);}
            else {Triangle(top,a,d,c,color); Triangle(top,a,c,b,color);}
            void Edge(Vector3 first, Vector3 second, Cell neighbor)
            {
                if (_world.Map.Contains(neighbor) && !_world.Map.Water.Contains(neighbor)) return;
                if(flatBoxes)
                {
                    var lowerFirst=first with{Y=-.07f};var lowerSecond=second with{Y=-.07f};
                    GroundColorTriangle(top,first,second,lowerSecond);GroundColorTriangle(top,first,lowerSecond,lowerFirst);
                    first=lowerFirst;second=lowerSecond;
                }
                var lowFirst = first with { Y = -1.67f }; var lowSecond = second with { Y = -1.67f };
                Triangle(sides,first,second,lowSecond,new("877d62")); Triangle(sides,first,lowSecond,lowFirst,new("877d62"));
            }
            Edge(a,b,new(cell.X,cell.Z-1)); Edge(b,c,new(cell.X+1,cell.Z));
            Edge(c,d,new(cell.X,cell.Z+1)); Edge(d,a,new(cell.X-1,cell.Z));
        }
        SurfaceMesh(_landscape,top).Name = "TerrainSurface"; SurfaceMesh(_landscape,sides).Name = "TerrainSides";
    }
    // Subdivide overlays so their centers and links follow both triangles of a sloping tile.
    private void GroundPatch(Node3D parent, float x, float z, float width, float depth, Color color, float lift = .035f)
    {
        var surface = new SurfaceTool(); surface.Begin(Godot.Mesh.PrimitiveType.Triangles);
        int parts = _world.Map.Heights.Length == 0 ? 1 : 8;
        for (int iz=0; iz<parts; iz++) for (int ix=0; ix<parts; ix++)
        {
            float left=x-width/2+width*ix/parts, back=z-depth/2+depth*iz/parts;
            var a=OnGround(left,back,lift); var b=OnGround(left+width/parts,back,lift);
            var c=OnGround(left+width/parts,back+depth/parts,lift); var d=OnGround(left,back+depth/parts,lift);
            Triangle(surface,a,d,c,color); Triangle(surface,a,c,b,color);
        }
        SurfaceMesh(parent,surface);
    }
    private Basis GroundBasis(float x, float z, bool rotated)
    {
        var right = new Vector3(.2f,Height(x+.1f,z)-Height(x-.1f,z),0).Normalized();
        var back = new Vector3(0,Height(x,z+.1f)-Height(x,z-.1f),.2f).Normalized();
        var up = back.Cross(right).Normalized();
        return new Basis(right,up,right.Cross(up).Normalized()) * new Basis(Vector3.Up,rotated ? Mathf.Pi/2 : 0);
    }
    private Vector3? Ground(Vector2 screen)
    {
        var origin=_camera.ProjectRayOrigin(screen); var direction=_camera.ProjectRayNormal(screen);
        if (_world.Map.Heights.Length == 0) return new Plane(Vector3.Up,0).IntersectsRay(origin,direction);
        float closest=float.PositiveInfinity;
        void Hit(Vector3 a, Vector3 b, Vector3 c)
        {
            var edge1=b-a; var edge2=c-a; var p=direction.Cross(edge2); float det=edge1.Dot(p);
            if (Math.Abs(det)<.00001f) return;
            var t=origin-a; float u=t.Dot(p)/det;
            if(u<0 || u>1) return;
            var q=t.Cross(edge1); float v=direction.Dot(q)/det;
            if(v<0 || u+v>1) return;
            float distance=edge2.Dot(q)/det;
            if(distance>=0 && distance<closest) closest=distance;
        }
        foreach(var cell in _world.Map.Land)
        {
            var a=OnGround(cell.X-.5f,cell.Z-.5f); var b=OnGround(cell.X+.5f,cell.Z-.5f);
            var c=OnGround(cell.X+.5f,cell.Z+.5f); var d=OnGround(cell.X-.5f,cell.Z+.5f);
            Hit(a,d,c); Hit(a,c,b);
        }
        // Water and off-map ground still produce a location for bridge/rejection feedback.
        var flat=new Plane(Vector3.Up,0).IntersectsRay(origin,direction);
        if(flat is Vector3 f)
        {
            var cell=new Cell(Mathf.RoundToInt(f.X),Mathf.RoundToInt(f.Z));
            if ((!_world.Map.Contains(cell) || _world.Map.Water.Contains(cell)) && origin.DistanceTo(f)<closest) return f;
        }
        return float.IsFinite(closest) ? origin+direction*closest : flat;
    }
}

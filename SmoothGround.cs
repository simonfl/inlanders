using Godot;
using System;

public partial class Game
{
    private bool _smoothGround=true; // Before/after comparison only; no saved preference.
    private static Color ContinuousGroundTint(Vector3 at)
    {
        float patch=MathF.Sin(at.X*.31f)*MathF.Cos(at.Z*.27f)*.025f;
        return new Color(.43f+patch,.50f+patch,.33f+patch).Lightened(Math.Max(0,at.Y)*.025f);
    }
    private static void GroundColorTriangle(SurfaceTool surface,Vector3 a,Vector3 b,Vector3 c)
    {
        surface.SetNormal((b-a).Cross(c-a).Normalized());
        foreach(var vertex in new[]{a,c,b}){surface.SetColor(ContinuousGroundTint(vertex));surface.AddVertex(vertex);}
    }
    private void MakeOriginalGrass()
    {
        using var surface=new SurfaceTool();surface.Begin(Godot.Mesh.PrimitiveType.Triangles);
        for(int x=-9;x<=9;x++)for(int z=-8;z<=8;z++)
        {
            var a=new Vector3(x-.5f,-.01f,z-.5f);var b=new Vector3(x+.5f,-.01f,z-.5f);
            var c=new Vector3(x+.5f,-.01f,z+.5f);var d=new Vector3(x-.5f,-.01f,z+.5f);
            GroundColorTriangle(surface,a,d,c);GroundColorTriangle(surface,a,c,b);
            void Rim(Vector3 p,Vector3 q){GroundColorTriangle(surface,p,q,q with{Y=-.08f});GroundColorTriangle(surface,p,q with{Y=-.08f},p with{Y=-.08f});}
            if(z==-8)Rim(a,b);if(x==9)Rim(b,c);if(z==8)Rim(c,d);if(x==-9)Rim(d,a);
        }
        SurfaceMesh(_landscape,surface).Name="OriginalGrass";
    }
}

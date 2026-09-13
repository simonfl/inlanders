using Godot;
using System;

public partial class Game
{
    private bool _smoothGround=true; // Before/after comparison only; no saved preference.
    private Color ContinuousGroundTint(Vector3 at)
    {
        float patch=MathF.Sin(at.X*.31f)*MathF.Cos(at.Z*.27f)*.025f;
        if(_storybookScene)return new Color(.34f+patch,.46f+patch,.28f+patch).Lightened(Math.Max(0,at.Y)*.025f);
        return new Color(.43f+patch,.50f+patch,.33f+patch).Lightened(Math.Max(0,at.Y)*.025f);
    }
    private void GroundColorTriangle(SurfaceTool surface,Vector3 a,Vector3 b,Vector3 c)
    {
        surface.SetNormal((b-a).Cross(c-a).Normalized());
        foreach(var vertex in new[]{a,c,b}){surface.SetColor(ContinuousGroundTint(vertex));surface.AddVertex(vertex);}
    }
    private void MakeOriginalGrass()
    {
        using var surface=new SurfaceTool();surface.Begin(Godot.Mesh.PrimitiveType.Triangles);
        for(int x=-9;x<=9;x++)for(int z=-8;z<=8;z++)
        {
            var a=OnGround(x-.5f,z-.5f,-.01f);var b=OnGround(x+.5f,z-.5f,-.01f);
            var c=OnGround(x+.5f,z+.5f,-.01f);var d=OnGround(x-.5f,z+.5f,-.01f);
            GroundColorTriangle(surface,a,d,c);GroundColorTriangle(surface,a,c,b);
            void Rim(Vector3 p,Vector3 q){GroundColorTriangle(surface,p,q,q with{Y=-.08f});GroundColorTriangle(surface,p,q with{Y=-.08f},p with{Y=-.08f});}
            if(z==-8)Rim(a,b);if(x==9)Rim(b,c);if(z==8)Rim(c,d);if(x==-9)Rim(d,a);
        }
        SurfaceMesh(_landscape,surface).Name="OriginalGrass";
    }
}

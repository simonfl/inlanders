using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
public partial class Game
{
    private static float RayBox(Vector3 origin,Vector3 direction,Aabb box)
    {
        float near=0,far=float.PositiveInfinity;
        for(int axis=0;axis<3;axis++)
        {
            float lo=box.Position[axis],hi=box.End[axis],at=origin[axis],d=direction[axis];
            if(MathF.Abs(d)<.00001f){if(at<lo || at>hi)return float.PositiveInfinity;continue;}
            float a=(lo-at)/d,b=(hi-at)/d;if(a>b)(a,b)=(b,a);near=Math.Max(near,a);far=Math.Min(far,b);
            if(near>far)return float.PositiveInfinity;
        }
        return near;
    }
    private static float RayMesh(Vector3 origin,Vector3 direction,Mesh mesh)
    {
        if(float.IsPositiveInfinity(RayBox(origin,direction,mesh.GetAabb())))return float.PositiveInfinity;
        float nearest=float.PositiveInfinity;
        for(int surface=0;surface<mesh.GetSurfaceCount();surface++)
        {
            var arrays=mesh.SurfaceGetArrays(surface);var vertices=arrays[(int)Godot.Mesh.ArrayType.Vertex].AsVector3Array();
            var index=arrays[(int)Godot.Mesh.ArrayType.Index].VariantType==Variant.Type.Nil?Array.Empty<int>():arrays[(int)Godot.Mesh.ArrayType.Index].AsInt32Array();
            int count=index.Length>0?index.Length:vertices.Length;
            for(int i=0;i+2<count;i+=3)
            {
                var a=vertices[index.Length>0?index[i]:i];var b=vertices[index.Length>0?index[i+1]:i+1];var c=vertices[index.Length>0?index[i+2]:i+2];
                var edge=b-a;var side=c-a;var cross=direction.Cross(side);float determinant=edge.Dot(cross);if(MathF.Abs(determinant)<.000001f)continue;
                var offset=origin-a;float u=offset.Dot(cross)/determinant;if(u<0 || u>1)continue;
                var q=offset.Cross(edge);float v=direction.Dot(q)/determinant;if(v<0 || u+v>1)continue;
                float distance=side.Dot(q)/determinant;if(distance>=0 && distance<nearest)nearest=distance;
            }
        }
        return nearest;
    }
    private (int Person,Cottage? Site) PickVisibleWorld(Vector2 pointer)
    {
        var origin=_camera.ProjectRayOrigin(pointer);var direction=_camera.ProjectRayNormal(pointer);float closest=float.PositiveInfinity;Cottage? site=null;
        void Inspect(Node node,Cottage candidate)
        {
            if(node is MeshInstance3D {Mesh:not null} mesh && mesh.IsVisibleInTree())
            {
                var inverse=mesh.GlobalTransform.AffineInverse();float depth=RayMesh(inverse*origin,inverse.Basis*direction,mesh.Mesh);
                if(depth<closest){closest=depth;site=candidate;}
            }
            foreach(var child in node.GetChildren())Inspect(child,candidate);
        }
        foreach(var candidate in _world.Cottages)if(_cottages.TryGetValue(candidate.Id,out var view))Inspect(view.Body,candidate);
        int person=-1;
        foreach(var p in _world.People)
        {
            var body=PresentedPerson(p.Id);var center=_camera.UnprojectPosition(body+Vector3.Up*.65f);
            float width=Math.Max(5,(_camera.UnprojectPosition(body+_camera.GlobalBasis.X*.38f)-_camera.UnprojectPosition(body)).Length());
            float height=Math.Max(7,MathF.Abs(_camera.UnprojectPosition(body+Vector3.Up*1.35f).Y-_camera.UnprojectPosition(body).Y)*.5f+2);
            var delta=pointer-center;if(delta.X*delta.X/(width*width)+delta.Y*delta.Y/(height*height)>1)continue;
            float depth=(body+Vector3.Up*.65f-origin).Dot(direction);
            if(depth>=0 && depth<closest){person=p.Id;site=null;closest=depth;}
        }
        return(person,site);
    }
}

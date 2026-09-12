using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class Game
{
    // Bake only fixed, ordinary primitive pieces within one model/stage. Named
    // displays and child rigs retain their identity for animation and buffer updates.
    private void BatchStaticGeometry(Node3D parent,bool includeBaked=false)
    {
        var pieces=new List<(MeshInstance3D Mesh,Transform3D Transform)>();
        void Collect(Node3D node,Transform3D transform)
        {
            foreach(var child in node.GetChildren().OfType<Node3D>())
            {
                if(!child.Name.ToString().StartsWith("@") || !child.Visible || child.GetGroups().Count>0) continue;
                var local=transform*child.Transform;
                if(child is MeshInstance3D m && m.GetChildCount()==0 && (m.Mesh is BoxMesh or CylinderMesh or SphereMesh || includeBaked && m.Mesh is ArrayMesh array && array.GetSurfaceCount()==1) &&
                    m.MaterialOverride is StandardMaterial3D material &&
                    material.Transparency==BaseMaterial3D.TransparencyEnum.Disabled &&
                    material.ShadingMode==BaseMaterial3D.ShadingModeEnum.PerPixel &&
                    !material.EmissionEnabled && material.AlbedoTexture==null) pieces.Add((m,local));
                else if(child.GetType()==typeof(Node3D)) Collect(child,local);
            }
        }
        Collect(parent,Transform3D.Identity);
        foreach(var group in pieces.GroupBy(m=>
        {
            var material=(StandardMaterial3D)m.Mesh.MaterialOverride;
            return (material.AlbedoColor,material.Roughness,material.CullMode);
        }))
        {
            if(group.Count()<2) continue;
            using var surface=new SurfaceTool(); surface.Begin(Godot.Mesh.PrimitiveType.Triangles);
            foreach(var piece in group) surface.AppendFrom(piece.Mesh.Mesh,0,piece.Transform);
            if(_traceFrames)_traceMeshes++;
            var combined=new MeshInstance3D { Mesh=surface.Commit(),MaterialOverride=group.First().Mesh.MaterialOverride };
            parent.AddChild(combined);
            foreach(var piece in group) { piece.Mesh.GetParent().RemoveChild(piece.Mesh); piece.Mesh.QueueFree(); }
        }
    }
}

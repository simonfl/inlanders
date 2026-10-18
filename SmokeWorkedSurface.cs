using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Collections.Generic;
public partial class Game
{
    private async void RunWorkedSurfaceSmoke()
    {
        try
        {
            AdoptWorld(World.NewRiverFrontage(false,true,true));_paused=true;CloseManagementUi();
            UpdateWorkedLandscape(true);
            var instance=_workedLand!.GetNode<MeshInstance3D>("LandUseSurface");
            var arrays=instance.Mesh.SurfaceGetArrays(0);
            var vertices=arrays[(int)Godot.Mesh.ArrayType.Vertex].AsVector3Array();
            var normals=arrays[(int)Godot.Mesh.ArrayType.Normal].AsVector3Array();
            var colors=arrays[(int)Godot.Mesh.ArrayType.Color].AsColorArray();
            ArrayMesh Old()
            {
                using var surface=new SurfaceTool();surface.Begin(Godot.Mesh.PrimitiveType.Triangles);
                for(int i=0;i<vertices.Length;i++){if(i%3==0)surface.SetNormal(normals[i]);surface.SetColor(colors[i]);surface.AddVertex(vertices[i]);}
                return surface.Commit();
            }
            using(var old=Old())using(var current=MakeWorkedSurfaceMesh(vertices,normals,colors))
            {
                var a=old.SurfaceGetArrays(0);var b=current.SurfaceGetArrays(0);
                if(!a[(int)Godot.Mesh.ArrayType.Vertex].AsVector3Array().SequenceEqual(b[(int)Godot.Mesh.ArrayType.Vertex].AsVector3Array()) ||
                    !a[(int)Godot.Mesh.ArrayType.Normal].AsVector3Array().SequenceEqual(b[(int)Godot.Mesh.ArrayType.Normal].AsVector3Array()) ||
                    !a[(int)Godot.Mesh.ArrayType.Color].AsColorArray().SequenceEqual(b[(int)Godot.Mesh.ArrayType.Color].AsColorArray()))throw new Exception("Ground mesh differs");
            }
            var oldTimes=new List<double>();var newTimes=new List<double>();
            for(int pair=0;pair<24;pair++)
            {
                foreach(bool old in pair%2==0?new[]{true,false}:new[]{false,true})
                {
                    var timer=System.Diagnostics.Stopwatch.StartNew();using var mesh=old?Old():MakeWorkedSurfaceMesh(vertices,normals,colors);timer.Stop();
                    if(pair>=4)(old?oldTimes:newTimes).Add(timer.Elapsed.TotalMilliseconds);
                }
                await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
            }
            oldTimes.Sort();newTimes.Sort();
            GD.Print($"PAIRED ground submission {vertices.Length} vertices: old median {oldTimes[10]:F3}ms, bulk {newTimes[10]:F3}ms; 20 alternating pairs after4 warmup; exact vertex/normal/color arrays.");
            await Capture("artifacts/209-worked-surface.png");GetTree().Quit();
        }
        catch(Exception e){GD.PrintErr(e);GetTree().Quit(1);}
    }
}

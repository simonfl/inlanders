using Godot;
using Inlanders.Simulation;
using System;
using System.Collections.Generic;

public partial class Game
{
    private void MakeRiverBank()
    {
        if(_world.PublicPlace==null || _plainFarmstead)return;
        var edges=new List<(Vector3 A,Vector3 B,Vector3 Out)>();
        var normals=new Dictionary<Vector3,Vector3>();
        foreach(var cell in _world.Map.Land)
        foreach(var step in new[]{new Cell(1,0),new(0,1),new(-1,0),new(0,-1)})
        {
            if(!_world.Map.Water.Contains(new(cell.X+step.X,cell.Z+step.Z)))continue;
            var outward=new Vector3(step.X,0,step.Z);var along=new Vector3(-step.Z,0,step.X);
            var center=new Vector3(cell.X,0,cell.Z)+outward*.5f;
            var a=center-along*.5f;var b=center+along*.5f;edges.Add((a,b,outward));
            foreach(var p in new[]{a,b})normals[p]=normals.GetValueOrDefault(p)+outward;
        }
        using var bank=new SurfaceTool();bank.Begin(Godot.Mesh.PrimitiveType.Triangles);
        void Tri(Vector3 a,Vector3 b,Vector3 c,Color ca,Color cb,Color cc)
        {
            bank.SetNormal(Vector3.Up);
            foreach(var v in new[]{(a,ca),(c,cc),(b,cb)}){bank.SetColor(v.Item2);bank.AddVertex(v.Item1);}
        }
        foreach(var edge in edges)
        {
            var outerA=edge.A+normals[edge.A].Normalized()*.65f;
            var outerB=edge.B+normals[edge.B].Normalized()*.65f;
            for(int i=0;i<4;i++)
            {
                Vector3 Point(float t,bool wet)
                {
                    var p=wet?outerA.Lerp(outerB,t):edge.A.Lerp(edge.B,t);
                    if(wet)p+=edge.Out*MathF.Sin(Mathf.Pi*t)*.05f;
                    p.Y=wet?-.068f:Height(p.X,p.Z)+.003f;return p;
                }
                var a=Point(i/4f,false);var b=Point((i+1)/4f,false);
                var c=Point((i+1)/4f,true);var d=Point(i/4f,true);
                var earth=new Color("8c876a");var shallow=new Color("6e9188");
                Tri(a,b,c,earth,earth,shallow);Tri(a,c,d,earth,shallow,shallow);
            }
        }
        SurfaceMesh(_landscape,bank).Name="RiverSiltBank";
    }
}

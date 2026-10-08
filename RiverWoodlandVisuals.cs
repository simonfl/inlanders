using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
public partial class Game
{
    private void MakeRiverWoodlandContext(Func<float,float,Vector3> ground)
    {
        // A continuous, irregular western mass, wholly outside workable land.
        // Stable sampling and spacing replace repeated four-tree formations.
        var map=_world.Map;var land=map.Land.ToArray();var random=new Random(170);var points=new List<Vector2>();
        var root=new Node3D{Name="RiverWoodlandContext"};_landscape.AddChild(root);
        for(int attempt=0;attempt<420 && points.Count<85;attempt++)
        {
            float z=map.MinZ-4+(float)random.NextDouble()*(map.Depth+8);
            float boundary=land.Where(c=>MathF.Abs(c.Z-z)<1.5f).Select(c=>(float)c.X).DefaultIfEmpty(map.MinX+2).Min();
            float edge=boundary-1.1f-.55f*(1+MathF.Sin(z*.37f));
            float x=edge-(float)random.NextDouble()*5.5f;var at=new Vector2(x,z);
            if(map.Contains(new(Mathf.RoundToInt(x),Mathf.RoundToInt(z))) || points.Any(p=>p.DistanceSquaredTo(at)<1.1f) || FarmsteadWater(x,z))continue;
            points.Add(at);
            float scale=.72f+(float)random.NextDouble()*.70f;
            var tree=new Node3D{Position=ground(x,z),Scale=new(scale,scale*(.8f+(float)random.NextDouble()*.4f),scale),Rotation=new(0,(float)random.NextDouble()*Mathf.Tau,0)};root.AddChild(tree);
            var tint=new Color("566c50").Lightened((float)random.NextDouble()*.14f);
            Cylinder(tree,new(0,.65f,0),.09f,1.3f,new("666452"));
            if(random.NextDouble()<.65)
            {
                Cylinder(tree,new(0,1.10f,0),.8f,1.45f,tint,.12f);
                Cylinder(tree,new(.08f,1.72f,0),.55f,1.2f,tint.Lightened(.045f),0);
            }
            else
            {
                Mesh(tree,new SphereMesh{Radius=.82f,Height=1.55f,RadialSegments=7,Rings=4},new(0,1.55f,0),tint);
                Mesh(tree,new SphereMesh{Radius=.55f,Height=.95f,RadialSegments=7,Rings=3},new(.38f,1.12f,.25f),tint.Darkened(.08f));
            }
        }
        BatchStaticGeometry(root);
    }
}

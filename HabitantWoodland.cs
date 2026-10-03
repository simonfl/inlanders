using Godot;
using System;
public partial class Game
{
    private Node3D MakeHabitantTree(Vector3 at,float scale)
    {
        int variant=Math.Abs((int)MathF.Round(at.X)*17+(int)MathF.Round(at.Z)*31)%3;
        var tree=new Node3D{Position=at,Scale=Vector3.One*scale};AddChild(tree);
        Color bark=new(variant==1?"b5b1a0":"665442");
        Cylinder(tree,new(0,1,0),variant==1?.11f:.15f,2,bark,.07f);
        var crown=new Node3D{Position=new(0,.85f,0)};tree.AddChild(crown);
        crown.AddToGroup("foliage");crown.SetMeta("breeze_phase",at.X*.61f+at.Z*.37f);
        if(variant==0)
        {
            Cylinder(crown,new(0,.42f,0),.92f,1.20f,new("475e45"),.16f);
            Cylinder(crown,new(.04f,1.02f,0),.70f,1.22f,new("566e4c"),.10f);
            Cylinder(crown,new(-.02f,1.58f,0),.44f,1.12f,new("647d53"),0);
        }
        else
        {
            var branch=Cylinder(tree,new(.20f,1.36f,0),.065f,.75f,bark,.025f);branch.RotationDegrees=new(0,0,-38);
            Color leaf=new(variant==1?"81945f":"637c4c");
            foreach(var p in new[]{new Vector3(-.38f,.82f,.08f),new(.35f,1.05f,.10f),new(.03f,1.48f,-.08f)})
                Mesh(crown,new SphereMesh{Radius=.62f,Height=1.10f,RadialSegments=7,Rings=3},p,leaf.Lightened(p.Y*.02f));
            if(variant==1)foreach(float y in new[]{.45f,.92f,1.37f})Box(tree,new(0,y,.10f),new(.13f,.035f,.02f),new("6f6d5e"));
        }
        return tree;
    }
}
